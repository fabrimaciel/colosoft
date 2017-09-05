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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Implementação do Parse para requisições com MultiPart.
	/// </summary>
	class HttpMultiPartRequestParser : HttpRequestParser
	{
		private static readonly byte[] MoreBoundary = Encoding.ASCII.GetBytes("\r\n");

		/// <summary>
		/// Bytes que identificam o fim do delimitador.
		/// </summary>
		private static readonly byte[] EndBoundary = Encoding.ASCII.GetBytes("--");

		private Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private ParserState _state = ParserState.BeforeFirstHeaders;

		private readonly byte[] _firstBoundary;

		private readonly byte[] _separatorBoundary;

		private bool _readingFile;

		private System.IO.MemoryStream _fieldStream;

		private System.IO.Stream _fileStream;

		private string _fileName;

		private bool _disposed;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="contentLength"></param>
		/// <param name="boundary"></param>
		public HttpMultiPartRequestParser(HttpClient client, int contentLength, string boundary) : base(client, contentLength)
		{
			if(boundary == null)
				throw new ArgumentNullException("boundary");
			_firstBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
			_separatorBoundary = Encoding.ASCII.GetBytes("\r\n--" + boundary);
			Client.MultiPartItems = new List<HttpMultiPartItem>();
		}

		/// <summary>
		/// Executa o parser.
		/// </summary>
		public override void Parse()
		{
			switch(_state)
			{
			case ParserState.BeforeFirstHeaders:
				ParseFirstHeader();
				break;
			case ParserState.ReadingHeaders:
				ParseHeaders();
				break;
			case ParserState.ReadingContent:
				ParseContent();
				break;
			case ParserState.ReadingBoundary:
				ParseBoundary();
				break;
			}
		}

		/// <summary>
		/// Executa o parser do primeiro cabeçalho.
		/// </summary>
		private void ParseFirstHeader()
		{
			bool? atBoundary = Client.ReadBuffer.AtBoundary(_firstBoundary, ContentLength);
			if(atBoundary.HasValue)
			{
				if(!atBoundary.Value)
					throw new System.ServiceModel.ProtocolException("Expected multipart content to start with the boundary");
				_state = ParserState.ReadingHeaders;
				ParseHeaders();
			}
		}

		/// <summary>
		/// Executa o parser dos cabeçalhos.
		/// </summary>
		private void ParseHeaders()
		{
			string line;
			while ((line = Client.ReadBuffer.ReadLine()) != null)
			{
				string[] parts;
				if(line.Length == 0)
				{
					string contentDispositionHeader;
					if(!_headers.TryGetValue("Content-Disposition", out contentDispositionHeader))
						throw new System.ServiceModel.ProtocolException("Expected Content-Disposition header with multipart");
					parts = contentDispositionHeader.Split(';');
					_readingFile = false;
					for(int i = 0; i < parts.Length; i++)
					{
						string part = parts[i].Trim();
						if(part.StartsWith("filename=", StringComparison.OrdinalIgnoreCase))
						{
							_readingFile = true;
							break;
						}
					}
					if(_readingFile)
					{
						_fileName = System.IO.Path.GetTempFileName();
						_fileStream = System.IO.File.Create(_fileName, 4096, System.IO.FileOptions.DeleteOnClose);
					}
					else
					{
						if(_fieldStream == null)
						{
							_fieldStream = new System.IO.MemoryStream();
						}
						else
						{
							_fieldStream.Position = 0;
							_fieldStream.SetLength(0);
						}
					}
					_state = ParserState.ReadingContent;
					ParseContent();
					return;
				}
				parts = line.Split(new[] {
					':'
				}, 2);
				if(parts.Length != 2)
					throw new System.ServiceModel.ProtocolException("Received header without colon");
				_headers[parts[0].Trim()] = parts[1].Trim();
			}
		}

		/// <summary>
		/// Executa o parser do conteúdo.
		/// </summary>
		private void ParseContent()
		{
			bool result = Client.ReadBuffer.CopyToStream(_readingFile ? _fileStream : _fieldStream, ContentLength, _separatorBoundary);
			if(result)
			{
				string value = null;
				System.IO.Stream stream = null;
				if(!_readingFile)
				{
					value = Encoding.ASCII.GetString(_fieldStream.ToArray());
				}
				else
				{
					stream = _fileStream;
					_fileStream = null;
					stream.Position = 0;
				}
				Client.MultiPartItems.Add(new HttpMultiPartItem(_headers, value, stream));
				_headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				_state = ParserState.ReadingBoundary;
				ParseBoundary();
			}
		}

		/// <summary>
		/// Executa o parser do limitador.
		/// </summary>
		private void ParseBoundary()
		{
			bool? atMore = Client.ReadBuffer.AtBoundary(MoreBoundary, ContentLength);
			if(atMore.HasValue)
			{
				if(atMore.Value)
				{
					_state = ParserState.ReadingHeaders;
					ParseHeaders();
				}
				else
				{
					bool? atEnd = Client.ReadBuffer.AtBoundary(EndBoundary, ContentLength);
					if(!atEnd.Value)
						throw new System.ServiceModel.ProtocolException("Unexpected content after boundary");
					EndParsing();
				}
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if(!_disposed)
			{
				if(_fieldStream != null)
				{
					_fieldStream.Dispose();
					_fieldStream = null;
				}
				if(_fileStream != null)
				{
					_fileStream.Dispose();
					_fileStream = null;
				}
				_disposed = true;
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Possíveis estados do parser.
		/// </summary>
		enum ParserState
		{
			BeforeFirstHeaders,
			ReadingHeaders,
			ReadingContent,
			ReadingBoundary
		}
	}
}
