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
	/// Armazena as opções da pesquisa.
	/// </summary>
	public class SearchOptions
	{
		/// <summary>
		/// Identifica se é para contruir o sumário.
		/// </summary>
		public bool BuildSummary
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica que deve ser gerado um resultado aleatório.
		/// </summary>
		public bool RandomResult
		{
			get;
			set;
		}

		/// <summary>
		/// Total da itens que serão recuperados no resultado.
		/// </summary>
		public int TotalHits
		{
			get;
			set;
		}

		/// <summary>
		/// Quantidade de hits que serão recuperados por página.
		/// </summary>
		public int HitsPerPage
		{
			get;
			set;
		}
	}
}
