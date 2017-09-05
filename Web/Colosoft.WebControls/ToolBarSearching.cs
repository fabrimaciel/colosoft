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
using System.Collections;
using System.Data;

namespace Colosoft.WebControls.GridView
{
	internal class ToolBarSearching
	{
		private GridView _grid;

		public ToolBarSearching(GridView grid)
		{
			_grid = grid;
		}

		private string ConstructFilterExpression(DataView view, SearchEventArgs args)
		{
			bool flag;
			if(view != null)
			{
				flag = (view.ToTable().Columns[args.SearchColumn].DataType == typeof(string)) || (view.ToTable().Columns[args.SearchColumn].DataType == typeof(DateTime));
			}
			else
			{
				var column = _grid.Columns.FromDataField(args.SearchColumn);
				if(column.SearchDataType == SearchDataType.NotSet)
				{
					throw new ArgumentException("SearchDataType for the respective column must be set in order to get custom search string (where clause)");
				}
				flag = (column.SearchDataType == SearchDataType.String) || (column.SearchDataType == SearchDataType.Date);
			}
			string format = flag ? "[{0}] {1} '{2}'" : "[{0}] {1} {2}";
			string str2 = "[{0}] {1} ({2})";
			string str3 = "[{0}] LIKE '{1}'";
			string str4 = "[{0}] NOT LIKE '{1}'";
			switch(args.SearchOperation)
			{
			case SearchOperation.IsEqualTo:
				return string.Format(format, args.SearchColumn, "=", args.SearchString);
			case SearchOperation.IsLessThan:
				return string.Format(format, args.SearchColumn, "<", args.SearchString);
			case SearchOperation.IsLessOrEqualTo:
				return string.Format(format, args.SearchColumn, "<=", args.SearchString);
			case SearchOperation.IsGreaterThan:
				return string.Format(format, args.SearchColumn, ">", args.SearchString);
			case SearchOperation.IsGreaterOrEqualTo:
				return string.Format(format, args.SearchColumn, ">=", args.SearchString);
			case SearchOperation.IsIn:
				return string.Format(str2, args.SearchColumn, "in", args.SearchString);
			case SearchOperation.IsNotIn:
				return string.Format(str2, args.SearchColumn, "not in", args.SearchString);
			case SearchOperation.BeginsWith:
				return string.Format(str3, args.SearchColumn, args.SearchString + "%");
			case SearchOperation.DoesNotBeginWith:
				return string.Format(str4, args.SearchColumn, args.SearchString + "%");
			case SearchOperation.EndsWith:
				return string.Format(str3, args.SearchColumn, "%" + args.SearchString);
			case SearchOperation.DoesNotEndWith:
				return string.Format(str4, args.SearchColumn, "%" + args.SearchString);
			case SearchOperation.Contains:
				return string.Format(str3, args.SearchColumn, "%" + args.SearchString + "%");
			case SearchOperation.DoesNotContain:
				return string.Format(str4, args.SearchColumn, "%" + args.SearchString + "%");
			}
			throw new Exception("Invalid search operation.");
		}

		private string ConstructLinqFilterExpression(DataView view, SearchEventArgs args)
		{
			bool flag;
			if(view != null)
			{
				flag = view.ToTable().Columns[args.SearchColumn].DataType == typeof(string);
			}
			else
			{
				var column = _grid.Columns.FromDataField(args.SearchColumn);
				if(column.SearchDataType == SearchDataType.NotSet)
				{
					throw new ArgumentException("SearchDataType for the respective column must be set in order to get custom search string (where clause)");
				}
				flag = column.SearchDataType == SearchDataType.String;
			}
			string format = flag ? "{0} {1} \"{2}\"" : "{0} {1} {2}";
			switch(args.SearchOperation)
			{
			case SearchOperation.IsEqualTo:
				return string.Format(format, args.SearchColumn, "=", args.SearchString);
			case SearchOperation.IsLessThan:
				return string.Format(format, args.SearchColumn, "<", args.SearchString);
			case SearchOperation.IsLessOrEqualTo:
				return string.Format(format, args.SearchColumn, "<=", args.SearchString);
			case SearchOperation.IsGreaterThan:
				return string.Format(format, args.SearchColumn, ">", args.SearchString);
			case SearchOperation.IsGreaterOrEqualTo:
				return string.Format(format, args.SearchColumn, ">=", args.SearchString);
			case SearchOperation.BeginsWith:
				return string.Format("{0}.BeginsWith(\"{1}\")", args.SearchColumn, args.SearchString);
			case SearchOperation.DoesNotBeginWith:
				return string.Format("!{0}.BeginsWith(\"{1}\")", args.SearchColumn, args.SearchString);
			case SearchOperation.EndsWith:
				return string.Format("{0}.EndsWith(\"{1}\")", args.SearchColumn, args.SearchString);
			case SearchOperation.DoesNotEndWith:
				return string.Format("!{0}.EndsWith(\"{1}\")", args.SearchColumn, args.SearchString);
			case SearchOperation.Contains:
				return string.Format("{0}.Contains(\"{1}\")", args.SearchColumn, args.SearchString);
			case SearchOperation.DoesNotContain:
				return string.Format("!{0}.Contains(\"{1}\")", args.SearchColumn, args.SearchString);
			}
			throw new Exception("Invalid search operation.");
		}

		private SearchOperation GetSearchOperationFromString(string searchOperation)
		{
			switch(searchOperation)
			{
			case "eq":
				return SearchOperation.IsEqualTo;
			case "ne":
				return SearchOperation.IsNotEqualTo;
			case "lt":
				return SearchOperation.IsLessThan;
			case "le":
				return SearchOperation.IsLessOrEqualTo;
			case "gt":
				return SearchOperation.IsGreaterThan;
			case "ge":
				return SearchOperation.IsGreaterOrEqualTo;
			case "in":
				return SearchOperation.IsIn;
			case "ni":
				return SearchOperation.IsNotIn;
			case "bw":
				return SearchOperation.BeginsWith;
			case "bn":
				return SearchOperation.DoesNotEndWith;
			case "ew":
				return SearchOperation.EndsWith;
			case "en":
				return SearchOperation.DoesNotEndWith;
			case "cn":
				return SearchOperation.Contains;
			case "nc":
				return SearchOperation.DoesNotContain;
			}
			throw new Exception("Search operation not known: " + searchOperation);
		}

		public string GetWhereClause(DataView view, bool isLinq)
		{
			string str = isLinq ? " && " : " AND ";
			string str2 = string.Empty;
			foreach (Column column in _grid.Columns)
			{
				string str3 = _grid.Page.Request[column.QualifiedName];
				if(!string.IsNullOrEmpty(str3))
				{
					var args2 = new SearchEventArgs();
					args2.SearchColumn = column.QualifiedName;
					args2.SearchString = str3;
					args2.SearchOperation = column.SearchToolBarOperation;
					SearchEventArgs e = args2;
					_grid.OnSearching(e);
					if(!e.Cancel)
					{
						string str4 = (str2.Length > 0) ? str : "";
						string str5 = isLinq ? this.ConstructLinqFilterExpression(view, e) : this.ConstructFilterExpression(view, e);
						str2 = str2 + str4 + str5;
					}
					_grid.OnSearched(new EventArgs());
				}
			}
			return str2;
		}

		public void PerformSearch(DataView view, bool isLinq)
		{
			try
			{
				view.RowFilter = this.GetWhereClause(view, isLinq);
			}
			catch(Exception)
			{
			}
		}
	}
}
