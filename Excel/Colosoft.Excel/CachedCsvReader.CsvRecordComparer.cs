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
using Debug = System.Diagnostics.Debug;
using System.Globalization;

namespace Colosoft.Excel.Csv
{
	public partial class CachedCsvReader : CsvReader
	{
		/// <summary>
		/// Represents a CSV record comparer.
		/// </summary>
		private class CsvRecordComparer : IComparer<string[]>
		{
			/// <summary>
			/// Contains the field index of the values to compare.
			/// </summary>
			private int _field;

			/// <summary>
			/// Contains the sort direction.
			/// </summary>
			private ListSortDirection _direction;

			/// <summary>
			/// Initializes a new instance of the CsvRecordComparer class.
			/// </summary>
			/// <param name="field">The field index of the values to compare.</param>
			/// <param name="direction">The sort direction.</param>
			public CsvRecordComparer(int field, ListSortDirection direction)
			{
				if(field < 0)
					throw new ArgumentOutOfRangeException("field", field, string.Format(CultureInfo.InvariantCulture, Resources.ExceptionMessage.FieldIndexOutOfRange, field));
				_field = field;
				_direction = direction;
			}

			public int Compare(string[] x, string[] y)
			{
				Debug.Assert(x != null && y != null && x.Length == y.Length && _field < x.Length);
				int result = String.Compare(x[_field], y[_field], StringComparison.CurrentCulture);
				return (_direction == ListSortDirection.Ascending ? result : -result);
			}
		}
	}
}
