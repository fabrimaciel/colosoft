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
using System.Collections;
using System.Collections.Generic;
using Colosoft.Excel.Csv.Resources;

namespace Colosoft.Excel.Csv
{
	public partial class CsvReader
	{
		/// <summary>
		/// Supports a simple iteration over the records of a <see cref="T:CsvReader"/>.
		/// </summary>
		public struct RecordEnumerator : IEnumerator<string[]>, IEnumerator
		{
			/// <summary>
			/// Contains the enumerated <see cref="T:CsvReader"/>.
			/// </summary>
			private CsvReader _reader;

			/// <summary>
			/// Contains the current record.
			/// </summary>
			private string[] _current;

			/// <summary>
			/// Contains the current record index.
			/// </summary>
			private long _currentRecordIndex;

			/// <summary>
			/// Initializes a new instance of the <see cref="T:RecordEnumerator"/> class.
			/// </summary>
			/// <param name="reader">The <see cref="T:CsvReader"/> to iterate over.</param>
			/// <exception cref="T:ArgumentNullException">
			///		<paramref name="reader"/> is a <see langword="null"/>.
			/// </exception>
			public RecordEnumerator(CsvReader reader)
			{
				if(reader == null)
					throw new ArgumentNullException("reader");
				_reader = reader;
				_current = null;
				_currentRecordIndex = reader._currentRecordIndex;
			}

			/// <summary>
			/// Gets the current record.
			/// </summary>
			public string[] Current
			{
				get
				{
					return _current;
				}
			}

			/// <summary>
			/// Advances the enumerator to the next record of the CSV.
			/// </summary>
			/// <returns><see langword="true"/> if the enumerator was successfully advanced to the next record, <see langword="false"/> if the enumerator has passed the end of the CSV.</returns>
			public bool MoveNext()
			{
				if(_reader._currentRecordIndex != _currentRecordIndex)
					throw new InvalidOperationException(ExceptionMessage.EnumerationVersionCheckFailed);
				if(_reader.ReadNextRecord())
				{
					_current = new string[_reader._fieldCount];
					_reader.CopyCurrentRecordTo(_current);
					_currentRecordIndex = _reader._currentRecordIndex;
					return true;
				}
				else
				{
					_current = null;
					_currentRecordIndex = _reader._currentRecordIndex;
					return false;
				}
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first record in the CSV.
			/// </summary>
			public void Reset()
			{
				if(_reader._currentRecordIndex != _currentRecordIndex)
					throw new InvalidOperationException(ExceptionMessage.EnumerationVersionCheckFailed);
				_reader.MoveTo(-1);
				_current = null;
				_currentRecordIndex = _reader._currentRecordIndex;
			}

			/// <summary>
			/// Gets the current record.
			/// </summary>
			object IEnumerator.Current
			{
				get
				{
					if(_reader._currentRecordIndex != _currentRecordIndex)
						throw new InvalidOperationException(ExceptionMessage.EnumerationVersionCheckFailed);
					return this.Current;
				}
			}

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				_reader = null;
				_current = null;
			}
		}
	}
}
