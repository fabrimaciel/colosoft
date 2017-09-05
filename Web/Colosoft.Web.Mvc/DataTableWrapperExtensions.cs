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

using Colosoft.Web.Mvc.Infrastructure;
using Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc.Extensions
{
	static class DataTableWrapperExtensions
	{
		private static Dictionary<string, object> SerializeGroupItem(DataTable ownerDataTable, IGroup group)
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("Key", group.Key);
			dictionary2.Add("HasSubgroups", group.HasSubgroups);
			dictionary2.Add("Member", group.Member);
			dictionary2.Add("Items", group.Items.SerializeToDictionary(ownerDataTable));
			dictionary2.Add("Subgroups", group.Subgroups.SerializeToDictionary(ownerDataTable));
			Dictionary<string, object> dictionary = dictionary2;
			AggregateFunctionsGroup group2 = group as AggregateFunctionsGroup;
			if(group2 != null)
			{
				dictionary.Add("AggregateFunctionsProjection", group2.AggregateFunctionsProjection);
				dictionary.Add("Aggregates", group2.Aggregates);
			}
			return dictionary;
		}

		public static Dictionary<string, object> SerializeRow(this DataRow row)
		{
			return row.Table.Columns.Cast<DataColumn>().ToDictionary<DataColumn, string, object>(column => column.ColumnName, column => row.Field<object>(column.ColumnName));
		}

		public static Dictionary<string, object> SerializeRow(this DataRowView row)
		{
			DataTable dataTable = row.DataView.Table;
			Dictionary<string, object> owner = new Dictionary<string, object>();
			SerializeRow(dataTable, row, owner);
			return owner;
		}

		private static void SerializeRow(DataTable dataTable, DataRowView row, Dictionary<string, object> owner)
		{
			foreach (DataColumn column in dataTable.Columns)
			{
				owner.Add(column.ColumnName, row.Row.Field<object>(column.ColumnName));
			}
		}

		internal static IEnumerable SerializeToDictionary(this IEnumerable enumerable, DataTable ownerDataTable)
		{
			Func<IGroup, Dictionary<string, object>> selector = null;
			if(!(enumerable is IEnumerable<AggregateFunctionsGroup>) && !(enumerable is IEnumerable<IGroup>))
			{
				return enumerable.OfType<DataRowView>().Select<DataRowView, Dictionary<string, object>>(delegate(DataRowView row) {
					Dictionary<string, object> owner = new Dictionary<string, object>();
					SerializeRow(ownerDataTable, row, owner);
					return owner;
				});
			}
			if(selector == null)
			{
				selector = group => SerializeGroupItem(ownerDataTable, group);
			}
			return enumerable.OfType<IGroup>().Select<IGroup, Dictionary<string, object>>(selector);
		}

		internal static DataTableWrapper WrapAsEnumerable(this DataTable dataTable)
		{
			return new DataTableWrapper(dataTable);
		}
	}
}
