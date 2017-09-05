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

namespace Colosoft.WebControls.GridView
{
	/// <summary>
	/// Classe responsável por trabalhar com a pesquisa.
	/// </summary>
	internal class Searching
	{
		private GridView _grid;

		private string _searchColunm;

		private string _searchOperation;

		private string _searchString;

		public Searching(GridView grid, string searchColumn, string searchString, string searchOperation)
		{
			_grid = grid;
			_searchColunm = searchColumn;
			_searchString = searchString;
			_searchOperation = searchOperation;
		}

		private string ConstructFilterExpression(DataView view, SearchEventArgs args)
		{
			string format = ((view.ToTable().Columns[args.SearchColumn].DataType == typeof(string)) || (view.ToTable().Columns[args.SearchColumn].DataType == typeof(DateTime))) ? "[{0}] {1} '{2}'" : "[{0}] {1} {2}";
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

		public void PerformSearch(DataView view, string search)
		{
			if(!string.IsNullOrEmpty(search) && Convert.ToBoolean(search))
			{
				var args2 = new SearchEventArgs();
				args2.SearchColumn = _searchColunm;
				args2.SearchString = _searchString;
				args2.SearchOperation = GetSearchOperationFromString(_searchOperation);
				var e = args2;
				_grid.OnSearching(e);
				if(!e.Cancel)
				{
					try
					{
						view.RowFilter = ConstructFilterExpression(view, e);
					}
					catch(Exception)
					{
					}
				}
				_grid.OnSearched(new EventArgs());
			}
		}
	}
}
