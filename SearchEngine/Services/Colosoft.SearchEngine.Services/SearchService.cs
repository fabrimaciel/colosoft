/* 
 * Colosoft Framework - generic framework to assist in development on the .NET platform
 * Copyright (C) 2013  <http://www.colosoft.com.br/framework> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Security.Permissions;
using Colosoft.SearchEngine;

namespace Colosoft.SearchEngine.Services
{
	/// <summary>
	/// Implementação do serviço de pesquisa.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class SearchService : ISearchService
	{
		const string SearchServiceRole = "SearchService";

		/// <summary>
		/// Instancia do gerenciador das pesquisas;
		/// </summary>
		private ISearcher _searcher;

		private ISearchStatistics _searchStatistics;

		private readonly System.Globalization.CultureInfo _currentCulture;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SearchService()
		{
			_searcher = ContainerManager.ApplicationContainer.GetExportedValue<ISearcher>();
			_searchStatistics = ContainerManager.ApplicationContainer.GetExportedValue<ISearchStatistics>();
			_currentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
			_searcher.Initialize();
		}

		/// <summary>
		/// Converter o valor do objeto para uma string.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private string ConvertToString(object value)
		{
			if(value == null)
				return null;
			if(value is IConvertible)
				return ((IConvertible)value).ToString(_currentCulture);
			return value.ToString();
		}

		/// <summary>
		/// Extrai o resultado da pesquisa.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="options"></param>
		/// <param name="startRow"></param>
		/// <returns></returns>
		private SearchResult ExtractResult(ISearchResult result, SearchOptions options, int startRow)
		{
			var elements = result.Elements.Skip(startRow).Take(options.HitsPerPage).ToArray();
			var items = elements.Select(f => new SearchResultItem {
				Uid = f.Uid,
				ChannelId = f.ChannelId,
				Values = f.Values.Select(x => ConvertToString(x)).ToArray()
			}).ToArray();
			var channelsFieldsIds = elements.Select(f => f.ChannelId).Distinct().ToArray();
			var channels = result.ChannelFields.Where(f => channelsFieldsIds.Any(x => x == f.ChannelId)).Select(f => new Services.ChannelFields() {
				ChannelId = f.ChannelId,
				Fields = f.Select(x => new FieldInfo {
					Name = x.Name,
					TypeFullName = x.Type.FullName
				}).ToArray()
			});
			return new Services.SearchResult {
				Count = result.Count,
				ChannelsFields = channels.ToArray(),
				StartRow = startRow,
				Summaries = result.Summary.ToArray(),
				Items = items
			};
		}

		/// <summary>
		/// Converte os parametros de pesquisa.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		private static SearchParameter[] ConvertToSearchs(IEnumerable<Parameter> parameters)
		{
			if(parameters == null)
				return null;
			return parameters.Select(f => new SearchParameter(f.Name, f.Values) {
				SearchType = f.SearchType
			}).ToArray();
		}

		/// <summary>
		/// Converte os parametros de filtro.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		private static FilterParameter[] ConvertToFilters(IEnumerable<Parameter> parameters)
		{
			if(parameters == null)
				return null;
			return parameters.Select(f => new FilterParameter(f.Name, f.Values) {
				SearchType = f.SearchType
			}).ToArray();
		}

		/// <summary>
		/// Recupera a quantidade de registros do resultado da consulta.
		/// </summary>
		/// <param name="parameters">Parametros da pesquisa.</param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options">Opções da pesquisa.</param>
		/// <returns></returns>
		[PrincipalPermission(SecurityAction.Demand, Role = SearchServiceRole)]
		public int SearchCount(Parameter[] parameters, Parameter[] filters, SearchOptions options)
		{
			if(parameters == null)
				throw new InvalidOperationException("Invalid Parameters");
			else if(options == null)
				throw new ArgumentNullException("options");
			return _searcher.SearchCount(ConvertToSearchs(parameters), ConvertToFilters(filters), options);
		}

		/// <summary>
		/// Recupera a quantidade de registros do resultado da consulta.
		/// </summary>
		/// <param name="text">Texto que será pesquisado.</param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options">Opções da pesquisa.</param>
		/// <returns></returns>
		[PrincipalPermission(SecurityAction.Demand, Role = SearchServiceRole)]
		public int SearchFullTextCount(string text, Parameter[] filters, SearchOptions options)
		{
			if(text == null)
				throw new ArgumentNullException("text");
			else if(options == null)
				throw new ArgumentNullException("options");
			return _searcher.SearchCount(text, ConvertToFilters(filters), options);
		}

		/// <summary>
		/// Executa a pesquisa.
		/// </summary>
		/// <param name="parameters">Parametros da pesquisa.</param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options">Opções da pesquisa.</param>
		/// <param name="sort">Dados da ordenação.</param>
		/// <param name="startRow">Linha de inicio para recupera os itens do resultado.</param>
		/// <returns></returns>
		[PrincipalPermission(SecurityAction.Demand, Role = SearchServiceRole)]
		public SearchResult Search(Parameter[] parameters, Parameter[] filters, SearchOptions options, Sort sort, int startRow)
		{
			if(parameters == null)
				throw new InvalidOperationException("Invalid Parameters");
			else if(options == null)
				throw new ArgumentNullException("options");
			using (var result = _searcher.Search(ConvertToSearchs(parameters), ConvertToFilters(filters), options, sort))
			{
				return ExtractResult(result, options, startRow);
			}
		}

		/// <summary>
		/// Executa a pesquisa do tipo fulltext.
		/// </summary>
		/// <param name="text">Texto que será pesquisado.</param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options">Opções da pesquisa.</param>
		/// <param name="sort">Dados da ordenação.</param>
		/// <param name="startRow">Linha de inicio para recupera os itens do resultado.</param>
		/// <returns></returns>
		[PrincipalPermission(SecurityAction.Demand, Role = SearchServiceRole)]
		public SearchResult SearchFullText(string text, Parameter[] filters, SearchOptions options, Sort sort, int startRow)
		{
			if(text == null)
				throw new ArgumentNullException("text");
			else if(options == null)
				throw new ArgumentNullException("options");
			using (var result = _searcher.Search(text, ConvertToFilters(filters), options, sort))
				return ExtractResult(result, options, startRow);
		}

		/// <summary>>
		/// Recupera as palavras do rank de mais pesquisadas.
		/// </summary>
		/// <param name="quatity"></param>
		/// <returns></returns>
		[PrincipalPermission(SecurityAction.Demand, Role = SearchServiceRole)]
		public WordRank[] GetWordsRank(int quatity)
		{
			return _searchStatistics.GetWords(quatity).ToArray();
		}

		/// <summary>
		/// Recupera as palavras do rank de mais pesquisadas.
		/// </summary>
		/// <returns></returns>
		[PrincipalPermission(SecurityAction.Demand, Role = SearchServiceRole)]
		public IndexRank[] GetWordsIndexRank()
		{
			return _searchStatistics.GetWordsIndexRank().ToArray();
		}
	}
}
