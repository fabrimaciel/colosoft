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
using System.IO;
using System.Linq;
using System.Text;

namespace Colosoft.Web
{
	/// <summary>
	///     Represents a single file extracted from a multipart/form-data
	///     stream.
	/// </summary>
	public class FilePart : IDisposable
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="FilePart" /> class.
		/// </summary>
		/// <param name="name">
		///     The name of the input field used for the upload.
		/// </param>
		/// <param name="fileName">
		///     The name of the file.
		/// </param>
		/// <param name="data">
		///     The file data.
		/// </param>
		public FilePart(string name, string fileName, Stream data) : this(name, fileName, data, "text/plain", "form-data")
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="FilePart" /> class.
		/// </summary>
		/// <param name="name">
		///     The name of the input field used for the upload.
		/// </param>
		/// <param name="fileName">
		///     The name of the file.
		/// </param>
		/// <param name="data">
		///     The file data.
		/// </param>
		/// <param name="contentType">
		///     The content type.
		/// </param>
		/// <param name="contentDisposition">
		///     The content disposition.
		/// </param>
		public FilePart(string name, string fileName, Stream data, string contentType, string contentDisposition)
		{
			Name = name;
			FileName = fileName.Split(Path.GetInvalidFileNameChars()).Last();
			Data = data;
			ContentType = contentType;
			ContentDisposition = contentDisposition;
		}

		/// <summary>
		/// Destructors
		/// </summary>
		~FilePart()
		{
			Dispose(false);
		}

		/// <summary>
		///     Gets the data.
		/// </summary>
		public Stream Data
		{
			get;
			private set;
		}

		/// <summary>
		///     Gets or sets the file name.
		/// </summary>
		public string FileName
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the name.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the content-type. Defaults to text/plain if unspecified.
		/// </summary>
		public string ContentType
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the content-disposition. Defaults to form-data if unspecified.
		/// </summary>
		public string ContentDisposition
		{
			get;
			set;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(Data != null)
				Data.Dispose();
			Data = null;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
	}
}
