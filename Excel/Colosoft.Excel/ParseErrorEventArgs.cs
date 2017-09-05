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

namespace Colosoft.Excel.Csv
{
	/// <summary>
	/// Provides data for the <see cref="M:CsvReader.ParseError"/> event.
	/// </summary>
	public class ParseErrorEventArgs : EventArgs
	{
		/// <summary>
		/// Contains the error that occured.
		/// </summary>
		private MalformedCsvException _error;

		/// <summary>
		/// Contains the action to take.
		/// </summary>
		private ParseErrorAction _action;

		/// <summary>
		/// Initializes a new instance of the ParseErrorEventArgs class.
		/// </summary>
		/// <param name="error">The error that occured.</param>
		/// <param name="defaultAction">The default action to take.</param>
		public ParseErrorEventArgs(MalformedCsvException error, ParseErrorAction defaultAction) : base()
		{
			_error = error;
			_action = defaultAction;
		}

		/// <summary>
		/// Gets the error that occured.
		/// </summary>
		/// <value>The error that occured.</value>
		public MalformedCsvException Error
		{
			get
			{
				return _error;
			}
		}

		/// <summary>
		/// Gets or sets the action to take.
		/// </summary>
		/// <value>The action to take.</value>
		public ParseErrorAction Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}
	}
}
