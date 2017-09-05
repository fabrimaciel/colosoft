﻿/* 
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

namespace Colosoft.Excel.Csv.Resources
{
	using System;

	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class ExceptionMessage
	{
		private static global::System.Resources.ResourceManager resourceMan;

		private static global::System.Globalization.CultureInfo resourceCulture;

		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal ExceptionMessage()
		{
		}

		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if(object.ReferenceEquals(resourceMan, null))
				{
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Excel.Csv.Resources.ExceptionMessage", typeof(ExceptionMessage).Assembly);
					resourceMan = temp;
				}
				return resourceMan;
			}
		}

		/// <summary>
		///   Overrides the current thread's CurrentUICulture property for all
		///   resource lookups using this strongly typed resource class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Globalization.CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Buffer size must be 1 or more..
		/// </summary>
		internal static string BufferSizeTooSmall
		{
			get
			{
				return ResourceManager.GetString("BufferSizeTooSmall", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Cannot move to a previous record in forward-only mode..
		/// </summary>
		internal static string CannotMovePreviousRecordInForwardOnly
		{
			get
			{
				return ResourceManager.GetString("CannotMovePreviousRecordInForwardOnly", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Cannot read record at index &apos;{0}&apos;..
		/// </summary>
		internal static string CannotReadRecordAtIndex
		{
			get
			{
				return ResourceManager.GetString("CannotReadRecordAtIndex", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Enumeration has either not started or has already finished..
		/// </summary>
		internal static string EnumerationFinishedOrNotStarted
		{
			get
			{
				return ResourceManager.GetString("EnumerationFinishedOrNotStarted", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Collection was modified; enumeration operation may not execute..
		/// </summary>
		internal static string EnumerationVersionCheckFailed
		{
			get
			{
				return ResourceManager.GetString("EnumerationVersionCheckFailed", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to &apos;{0}&apos; field header not found..
		/// </summary>
		internal static string FieldHeaderNotFound
		{
			get
			{
				return ResourceManager.GetString("FieldHeaderNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Field index must be included in [0, FieldCount[. Specified field index was : &apos;{0}&apos;..
		/// </summary>
		internal static string FieldIndexOutOfRange
		{
			get
			{
				return ResourceManager.GetString("FieldIndexOutOfRange", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to The CSV appears to be corrupt near record &apos;{0}&apos; field &apos;{1} at position &apos;{2}&apos;. Current raw data : &apos;{3}&apos;..
		/// </summary>
		internal static string MalformedCsvException
		{
			get
			{
				return ResourceManager.GetString("MalformedCsvException", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to &apos;{0}&apos; is not a supported missing field action..
		/// </summary>
		internal static string MissingFieldActionNotSupported
		{
			get
			{
				return ResourceManager.GetString("MissingFieldActionNotSupported", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to No current record..
		/// </summary>
		internal static string NoCurrentRecord
		{
			get
			{
				return ResourceManager.GetString("NoCurrentRecord", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to The CSV does not have headers (CsvReader.HasHeaders property is false)..
		/// </summary>
		internal static string NoHeaders
		{
			get
			{
				return ResourceManager.GetString("NoHeaders", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to The number of fields in the record is greater than the available space from index to the end of the destination array..
		/// </summary>
		internal static string NotEnoughSpaceInArray
		{
			get
			{
				return ResourceManager.GetString("NotEnoughSpaceInArray", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to &apos;{0}&apos; is not a valid ParseErrorAction while inside a ParseError event..
		/// </summary>
		internal static string ParseErrorActionInvalidInsideParseErrorEvent
		{
			get
			{
				return ResourceManager.GetString("ParseErrorActionInvalidInsideParseErrorEvent", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to &apos;{0}&apos; is not a supported ParseErrorAction..
		/// </summary>
		internal static string ParseErrorActionNotSupported
		{
			get
			{
				return ResourceManager.GetString("ParseErrorActionNotSupported", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to This operation is invalid when the reader is closed..
		/// </summary>
		internal static string ReaderClosed
		{
			get
			{
				return ResourceManager.GetString("ReaderClosed", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Record index must be 0 or more..
		/// </summary>
		internal static string RecordIndexLessThanZero
		{
			get
			{
				return ResourceManager.GetString("RecordIndexLessThanZero", resourceCulture);
			}
		}
	}
}
