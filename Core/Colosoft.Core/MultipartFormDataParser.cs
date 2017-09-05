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
	///     Provides methods to parse a
	///     <see href="http://www.ietf.org/rfc/rfc2388.txt">
	///         <c>multipart/form-data</c>
	///     </see>
	///     stream into it's parameters and file data.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A parameter is defined as any non-file data passed in the multipart stream. For example
	///         any form fields would be considered a parameter.
	///     </para>
	///     <para>
	///         The parser determines if a section is a file or not based on the presence or absence
	///         of the filename argument for the Content-Type header. If filename is set then the section
	///         is assumed to be a file, otherwise it is assumed to be parameter data.
	///     </para>
	/// </remarks>
	/// <example>
	///     <code lang="C#"> 
	///       Stream multipartStream = GetTheMultipartStream();
	///       string boundary = GetTheBoundary();
	///       var parser = new MultipartFormDataParser(multipartStream, boundary, Encoding.UTF8);
	///  
	///       // Grab the parameters (non-file data). Key is based on the name field
	///       var username = parser.Parameters["username"].Data;
	///       var password = parser.parameters["password"].Data;
	///       
	///       // Grab the first files data
	///       var file = parser.Files.First();
	///       var filename = file.FileName;
	///       var filestream = file.Data;
	///   </code>
	///     <code lang="C#">
	///     // In the context of WCF you can get the boundary from the HTTP
	///     // request
	///     public ResponseClass MyMethod(Stream multipartData)
	///     {
	///         // First we need to get the boundary from the header, this is sent
	///         // with the HTTP request. We can do that in WCF using the WebOperationConext:
	///         var type = WebOperationContext.Current.IncomingRequest.Headers["Content-Type"];
	/// 
	///         // Now we want to strip the boundary out of the Content-Type, currently the string
	///         // looks like: "multipart/form-data; boundary=---------------------124123qase124"
	///         var boundary = type.Substring(type.IndexOf('=')+1);
	/// 
	///         // Now that we've got the boundary we can parse our multipart and use it as normal
	///         var parser = new MultipartFormDataParser(data, boundary, Encoding.UTF8);
	/// 
	///         ...
	///     }
	///   </code>
	/// </example>
	public class MultipartFormDataParser : IDisposable
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
		///     with an input stream. Boundary will be automatically detected based on the
		///     first line of input.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		public MultipartFormDataParser(Stream stream) : this(stream, null, Encoding.UTF8, 4096)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
		///     with the boundary and input stream.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		/// <param name="boundary">
		///     The multipart/form-data boundary. This should be the value
		///     returned by the request header.
		/// </param>
		public MultipartFormDataParser(Stream stream, string boundary) : this(stream, boundary, Encoding.UTF8, 4096)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
		///     with the input stream and stream encoding. Boundary is automatically
		///     detected.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		/// <param name="encoding">
		///     The encoding of the multipart data
		/// </param>
		public MultipartFormDataParser(Stream stream, Encoding encoding) : this(stream, null, encoding, 4096)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
		///     with the boundary, input stream and stream encoding.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		/// <param name="boundary">
		///     The multipart/form-data boundary. This should be the value
		///     returned by the request header.
		/// </param>
		/// <param name="encoding">
		///     The encoding of the multipart data
		/// </param>
		public MultipartFormDataParser(Stream stream, string boundary, Encoding encoding) : this(stream, boundary, encoding, 4096)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
		///     with the stream, input encoding and buffer size. Boundary is automatically
		///     detected.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		/// <param name="encoding">
		///     The encoding of the multipart data
		/// </param>
		/// <param name="binaryBufferSize">
		///     The size of the buffer to use for parsing the multipart form data. This must be larger
		///     then (size of boundary + 4 + # bytes in newline).
		/// </param>
		public MultipartFormDataParser(Stream stream, Encoding encoding, int binaryBufferSize) : this(stream, null, encoding, binaryBufferSize)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
		///     with the boundary, stream, input encoding and buffer size.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		/// <param name="boundary">
		///     The multipart/form-data boundary. This should be the value
		///     returned by the request header.
		/// </param>
		/// <param name="encoding">
		///     The encoding of the multipart data
		/// </param>
		/// <param name="binaryBufferSize">
		///     The size of the buffer to use for parsing the multipart form data. This must be larger
		///     then (size of boundary + 4 + # bytes in newline).
		/// </param>
		public MultipartFormDataParser(Stream stream, string boundary, Encoding encoding, int binaryBufferSize) : this(stream, boundary, encoding, binaryBufferSize, null)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
		///     with the boundary, stream, input encoding and buffer size.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		/// <param name="boundary">
		///     The multipart/form-data boundary. This should be the value
		///     returned by the request header.
		/// </param>
		/// <param name="encoding">
		///     The encoding of the multipart data
		/// </param>
		/// <param name="binaryBufferSize">
		///     The size of the buffer to use for parsing the multipart form data. This must be larger
		///     then (size of boundary + 4 + # bytes in newline).
		/// </param>
		public MultipartFormDataParser(Stream stream, string boundary, Encoding encoding, int binaryBufferSize, FilePartStreamCreator filePartCreator)
		{
			Files = new List<FilePart>();
			Parameters = new List<ParameterPart>();
			var streamingParser = new StreamingMultipartFormDataParser(stream, boundary, encoding, binaryBufferSize);
			streamingParser.ParameterHandler += parameterPart => Parameters.Add(parameterPart);
			streamingParser.FileHandler += (name, fileName, type, disposition, buffer, bytes) =>  {
				if(Files.Count == 0 || name != Files[Files.Count - 1].Name)
				{
					Files.Add(new FilePart(name, fileName, filePartCreator != null ? filePartCreator(this, new FilePartStreamCreatorArgs(fileName)) : new MemoryStream(), type, disposition));
				}
				Files[Files.Count - 1].Data.Write(buffer, 0, bytes);
			};
			streamingParser.Run();
			foreach (var file in Files)
			{
				file.Data.Position = 0;
			}
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~MultipartFormDataParser()
		{
			Dispose(false);
		}

		/// <summary>
		///     Gets the mapping of parameters parsed files. The name of a given field
		///     maps to the parsed file data.
		/// </summary>
		public List<FilePart> Files
		{
			get;
			private set;
		}

		/// <summary>
		///     Gets the parameters. Several ParameterParts may share the same name.
		/// </summary>
		public List<ParameterPart> Parameters
		{
			get;
			private set;
		}

		/// <summary>
		/// Returns true if the parameter has any values. False otherwise
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <returns>True if the parameter exists. False otherwise</returns>
		public bool HasParameter(string name)
		{
			return Parameters.Any(p => p.Name == name);
		}

		/// <summary>
		/// Returns the value of a parameter or null if it doesn't exist. 
		/// 
		/// You should only use this method if you're sure the parameter has only one value. 
		/// 
		/// If you need to support multiple values use GetParameterValues.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <returns>The value of the parameter</returns>
		public string GetParameterValue(string name)
		{
			return Parameters.FirstOrDefault(p => p.Name == name).Data;
		}

		/// <summary>
		/// Returns the values of a parameter or an empty enumerable if the parameter doesn't exist.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <returns>The values of the parameter</returns>
		public IEnumerable<string> GetParameterValues(string name)
		{
			return Parameters.Where(p => p.Name == name).Select(p => p.Data);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			foreach (var file in Files)
				file.Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		public class FilePartStreamCreatorArgs : EventArgs
		{
			/// <summary>
			/// Nome do arquivo.
			/// </summary>
			public string FileName
			{
				get;
				set;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="fileName"></param>
			public FilePartStreamCreatorArgs(string fileName)
			{
				FileName = fileName;
			}
		}

		/// <summary>
		/// Assinatura do método acionado para criar a stream do arquivo recebido.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public delegate System.IO.Stream FilePartStreamCreator (object sender, FilePartStreamCreatorArgs e);
	}
}
