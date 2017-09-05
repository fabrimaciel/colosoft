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
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc.UI
{
	/// <summary>
	/// Classe com método para auxiliar a manipulaçõa de urls de grid.
	/// </summary>
	public static class GridUrlParameters
	{
		/// <summary>
		/// Nome do parãmetro das agregações.
		/// </summary>
		public static string Aggregates
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do parâmetro do filtro.
		/// </summary>
		public static string Filter
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do parâmetro do grupo.
		/// </summary>
		public static string Group
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do parâmetro do modo.
		/// </summary>
		public static string Mode
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do parâmetro da página.
		/// </summary>
		public static string Page
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do parâmetro do tamanho da página.
		/// </summary>
		public static string PageSize
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do parâmetro da ordenação.
		/// </summary>
		public static string Sort
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor geral.
		/// </summary>
		static GridUrlParameters()
		{
			Sort = "sort";
			Group = "group";
			Page = "page";
			PageSize = "pageSize";
			Filter = "filter";
			Mode = "mode";
			Aggregates = "aggregate";
		}

		/// <summary>
		/// Recupera o dicionário com os parametros com o prefixo informado.
		/// </summary>
		/// <param name="prefix"></param>
		/// <returns></returns>
		public static IDictionary<string, string> ToDictionary(string prefix)
		{
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary[Group] = prefix + Group;
			dictionary[Sort] = prefix + Sort;
			dictionary[Page] = prefix + Page;
			dictionary[PageSize] = prefix + PageSize;
			dictionary[Filter] = prefix + Filter;
			dictionary[Mode] = prefix + Mode;
			return dictionary;
		}
	}
}
