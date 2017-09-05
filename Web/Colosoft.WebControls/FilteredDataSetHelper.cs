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
using System.Text;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Globalization;

namespace Colosoft.WebControls
{
	internal static class FilteredDataSetHelper
	{
		public static DataView CreateFilteredDataView(DataTable table, string sortExpression, string filterExpression, IDictionary filterParameters)
		{
			DataView view = new DataView(table);
			if(!string.IsNullOrEmpty(sortExpression))
			{
				view.Sort = sortExpression;
			}
			if(!string.IsNullOrEmpty(filterExpression))
			{
				bool flag = false;
				object[] args = new object[filterParameters.Count];
				int index = 0;
				foreach (DictionaryEntry entry in filterParameters)
				{
					if(entry.Value == null)
					{
						flag = true;
						break;
					}
					args[index] = entry.Value;
					index++;
				}
				filterExpression = string.Format(CultureInfo.InvariantCulture, filterExpression, args);
				if(!flag)
				{
					view.RowFilter = filterExpression;
				}
			}
			return view;
		}

		public static DataTable GetDataTable(Control owner, object dataObject)
		{
			DataSet set = dataObject as DataSet;
			if(set == null)
			{
				return (dataObject as DataTable);
			}
			if(set.Tables.Count == 0)
				throw new InvalidOperationException(string.Format("DataSet has no tables in {0}.", new object[] {
					owner.ID
				}));
			return set.Tables[0];
		}
	}
}
