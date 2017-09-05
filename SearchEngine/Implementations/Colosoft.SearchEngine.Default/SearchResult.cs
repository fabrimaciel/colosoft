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

namespace Colosoft.SearchEngine.Default
{
	/// <summary>
	/// Classe que contém o resultado de uma buusca
	/// </summary>
	public class SearchResult : ISearchResult
	{
		/// <summary>
		/// Esquema dos elementos do resultado
		/// </summary>
		public SchemeChannel Scheme
		{
			get;
			set;
		}

		/// <summary>
		/// Elementos do resultado
		/// </summary>
		public Element[] Elements
		{
			get;
			set;
		}

		/// <summary>
		/// Sumário do resultado
		/// </summary>
		public SummaryResult[] Summary
		{
			get;
			set;
		}

		int ISearchResult.Count
		{
			get
			{
				return Elements.Length;
			}
		}

		IEnumerable<Element> ISearchResult.Elements
		{
			get
			{
				return Elements;
			}
		}

		IEnumerable<SummaryResult> ISearchResult.Summary
		{
			get
			{
				return Summary;
			}
		}
	}
}
