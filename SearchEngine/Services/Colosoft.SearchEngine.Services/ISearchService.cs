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

namespace Colosoft.SearchEngine.Services
{
	/// <summary>
	/// Serviço de pesquisa.
	/// </summary>
	[ServiceContract]
	public interface ISearchService
	{
		/// <summary>
		/// Executa a pesquisa.
		/// </summary>
		/// <param name="parameters">Parametros da pesquisa.</param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options">Opções da pesquisa.</param>
		/// <param name="sort">Dados da ordenação.</param>
		/// <param name="startRow">Linha de inicio para recupera os itens do resultado.</param>
		/// <returns></returns>
		[OperationContract]
		SearchResult Search(Parameter[] parameters, Parameter[] filters, SearchOptions options, Sort sort, int startRow);

		/// <summary>
		/// Executa a pesquisa do tipo fulltext.
		/// </summary>
		/// <param name="text">Texto que será pesquisado.</param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options">Opções da pesquisa.</param>
		/// <param name="sort">Dados da ordenação.</param>
		/// <param name="startRow">Linha de inicio para recupera os itens do resultado.</param>
		/// <returns></returns>
		[OperationContract]
		SearchResult SearchFullText(string text, Parameter[] filters, SearchOptions options, Sort sort, int startRow);

		/// <summary>
		/// Recupera as palavras do rank de mais pesquisadas.
		/// </summary>
		/// <param name="quatity"></param>
		/// <returns></returns>
		[OperationContract]
		WordRank[] GetWordsRank(int quatity);

		/// <summary>
		/// Recupera as palavras do rank de mais pesquisadas.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		IndexRank[] GetWordsIndexRank();

		/// <summary>
		/// Recupera a quantidade de registros do resultado da consulta.
		/// </summary>
		/// <param name="parameters">Parametros da pesquisa.</param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options">Opções da pesquisa.</param>
		/// <returns></returns>
		[OperationContract]
		int SearchCount(Parameter[] parameters, Parameter[] filters, SearchOptions options);

		/// <summary>
		/// Recupera a quantidade de registros do resultado da consulta.
		/// </summary>
		/// <param name="text">Texto que será pesquisado.</param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options">Opções da pesquisa.</param>
		/// <returns></returns>
		[OperationContract]
		int SearchFullTextCount(string text, Parameter[] filters, SearchOptions options);
	}
}
