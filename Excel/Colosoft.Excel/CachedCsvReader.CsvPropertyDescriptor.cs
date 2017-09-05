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

namespace Colosoft.Excel.Csv
{
	public partial class CachedCsvReader : CsvReader
	{
		/// <summary>
		/// Represents a CSV field property descriptor.
		/// </summary>
		private class CsvPropertyDescriptor : PropertyDescriptor
		{
			/// <summary>
			/// Contains the field index.
			/// </summary>
			private int _index;

			/// <summary>
			/// Initializes a new instance of the CsvPropertyDescriptor class.
			/// </summary>
			/// <param name="fieldName">The field name.</param>
			/// <param name="index">The field index.</param>
			public CsvPropertyDescriptor(string fieldName, int index) : base(fieldName, null)
			{
				_index = index;
			}

			/// <summary>
			/// Gets the field index.
			/// </summary>
			/// <value>The field index.</value>
			public int Index
			{
				get
				{
					return _index;
				}
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override object GetValue(object component)
			{
				return ((string[])component)[_index];
			}

			public override void ResetValue(object component)
			{
			}

			public override void SetValue(object component, object value)
			{
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get
				{
					return typeof(CachedCsvReader);
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Type PropertyType
			{
				get
				{
					return typeof(string);
				}
			}
		}
	}
}
