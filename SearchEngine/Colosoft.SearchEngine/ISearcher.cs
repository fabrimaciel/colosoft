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
using System.Text;

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Classe responsável pela pesquisa.
	/// </summary>
	public interface ISearcher
	{
		/// <summary>
		/// Instancia responsável pela manutenção do pesquisador.
		/// </summary>
		ISearcherMaintenance Maintenance
		{
			get;
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Realiza uma pesquisa.
		/// </summary>
		/// <param name="parameters">Parametros que serão usados no pesquisa.</param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options">Opções da pesquisa.</param>
		/// <param name="sort">Dados da ordenação do resultado.</param>
		/// <returns></returns>
		ISearchResult Search(SearchParameter[] parameters, FilterParameter[] filters, SearchOptions options, Sort sort);

		/// <summary>
		/// Realiza um pesquisa no sistema e recupera a quantidade do resultado.
		/// </summary>
		/// <param name="parameters">Parametros que serão usados no pesquisa.</param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options">Opções da pesquisa.</param>
		/// <returns></returns>
		int SearchCount(SearchParameter[] parameters, FilterParameter[] filters, SearchOptions options);

		/// <summary>
		/// Realiza uma pesquisa no sistema.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="filters">Filtros da pesquisa.</param>
		/// <param name="options"></param>
		/// <param name="sort"></param>
		/// <returns></returns>
		ISearchResult Search(string text, FilterParameter[] filters, SearchOptions options, Sort sort);

		/// <summary>
		/// Realiza uma pesquisa no sistema e recupera a quantidade do resultado.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="filters"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		int SearchCount(string text, FilterParameter[] filters, SearchOptions options);
	}
}
