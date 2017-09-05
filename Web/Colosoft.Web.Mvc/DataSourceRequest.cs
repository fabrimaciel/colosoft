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
	/// Representa a requisição da fonte de dados.
	/// </summary>
	public class DataSourceRequest
	{
		/// <summary>
		/// Descritores dos agregadores.
		/// </summary>
		public IList<AggregateDescriptor> Aggregates
		{
			get;
			set;
		}

		/// <summary>
		/// Descritores dos filtros.
		/// </summary>
		public IList<IFilterDescriptor> Filters
		{
			get;
			set;
		}

		/// <summary>
		/// Descritores dos grupos.
		/// </summary>
		public IList<GroupDescriptor> Groups
		{
			get;
			set;
		}

		/// <summary>
		/// Página de dados.
		/// </summary>
		public int Page
		{
			get;
			set;
		}

		/// <summary>
		/// Tamanho da página.
		/// </summary>
		public int PageSize
		{
			get;
			set;
		}

		/// <summary>
		/// Ordenações.
		/// </summary>
		public IList<SortDescriptor> Sorts
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public DataSourceRequest()
		{
			Page = 1;
			Aggregates = new List<AggregateDescriptor>();
		}
	}
}
