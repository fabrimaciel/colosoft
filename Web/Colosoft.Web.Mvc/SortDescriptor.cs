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
	/// Descritor de ordenação.
	/// </summary>
	public class SortDescriptor : JsonObject, IDescriptor
	{
		/// <summary>
		/// Nome do membro.
		/// </summary>
		public string Member
		{
			get;
			set;
		}

		/// <summary>
		/// Direção da ordenação,
		/// </summary>
		public ListSortDirection SortDirection
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SortDescriptor() : this(null, ListSortDirection.Ascending)
		{
		}

		/// <summary>
		/// Cria a instancia com o membro e a direção de ordenação.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="order"></param>
		public SortDescriptor(string member, ListSortDirection order)
		{
			Member = member;
			SortDirection = order;
		}

		/// <summary>
		/// Deserializa os dados.
		/// </summary>
		/// <param name="source"></param>
		public void Deserialize(string source)
		{
			string[] items = source.Split('-');
			if(items.Length > 1)
			{
				Member = items[0];
			}
			string str = items.Last<string>();
			SortDirection = (str == "desc") ? ListSortDirection.Descending : ListSortDirection.Ascending;
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <returns></returns>
		public string Serialize()
		{
			return "{0}-{1}".FormatWith(Member, ((SortDirection == ListSortDirection.Ascending) ? "asc" : "desc"));
		}

		/// <summary>
		/// Serializa os dados para o dicionário informado.
		/// </summary>
		/// <param name="json"></param>
		protected override void Serialize(IDictionary<string, object> json)
		{
			json["field"] = this.Member;
			json["dir"] = (this.SortDirection == ListSortDirection.Ascending) ? "asc" : "desc";
		}
	}
}
