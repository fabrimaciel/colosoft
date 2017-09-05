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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Descritor de grupo.
	/// </summary>
	public class GroupDescriptor : SortDescriptor
	{
		private Infrastructure.Implementation.AggregateFunctionCollection _aggregateFunctions;

		private object _displayContent;

		/// <summary>
		/// Funções de agregação.
		/// </summary>
		public Infrastructure.Implementation.AggregateFunctionCollection AggregateFunctions
		{
			get
			{
				return (_aggregateFunctions = _aggregateFunctions ?? new Infrastructure.Implementation.AggregateFunctionCollection());
			}
		}

		/// <summary>
		/// Conteúdo de exibição.
		/// </summary>
		public object DisplayContent
		{
			get
			{
				if(_displayContent == null)
					return base.Member;
				return _displayContent;
			}
			set
			{
				_displayContent = value;
			}
		}

		/// <summary>
		/// Tipo do membro.
		/// </summary>
		public Type MemberType
		{
			get;
			set;
		}

		/// <summary>
		/// Direçãoi de ordenação ciclica.
		/// </summary>
		public void CycleSortDirection()
		{
			base.SortDirection = GetNextSortDirection(new ListSortDirection?(base.SortDirection));
		}

		/// <summary>
		/// Recupera a próximo direção de ordenação.
		/// </summary>
		/// <param name="sortDirection"></param>
		/// <returns></returns>
		private static ListSortDirection GetNextSortDirection(ListSortDirection? sortDirection)
		{
			ListSortDirection valueOrDefault = sortDirection.GetValueOrDefault();
			if(sortDirection.HasValue && (valueOrDefault == ListSortDirection.Ascending))
			{
				return ListSortDirection.Descending;
			}
			return ListSortDirection.Ascending;
		}

		/// <summary>
		/// Serializa os dados para o dicionário informado.
		/// </summary>
		/// <param name="json"></param>
		protected override void Serialize(IDictionary<string, object> json)
		{
			base.Serialize(json);
			if(this.AggregateFunctions.Any<AggregateFunction>())
			{
				json["aggregates"] = ((IEnumerable<JsonObject>)AggregateFunctions).ToJson();
			}
		}
	}
}
