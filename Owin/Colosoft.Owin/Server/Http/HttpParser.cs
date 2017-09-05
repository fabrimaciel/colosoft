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
using System.Threading.Tasks;
using Colosoft.Owin.Server.Infrastructure;

namespace Colosoft.Owin.Server.Http
{
	/// <summary>
	/// Classe com métodos para auxiliar na manipulação de dados HTTP.
	/// </summary>
	static class HttpParser
	{
		/// <summary>
		/// Executa o parser da requisição.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="onRequestLine"></param>
		/// <param name="onHeader"></param>
		public static void ParseRequest(Stream stream, Action<string, string, string> onRequestLine, Action<string, string> onHeader)
		{
			string method;
			string path;
			string protocol;
			Parse3Tokens(stream, out method, out path, out protocol);
			onRequestLine(method, path, protocol);
			ParseHeaders(stream, onHeader);
		}

		/// <summary>
		/// Executa o parser da resposta.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="onResponseLine"></param>
		/// <param name="onHeader"></param>
		public static void ParseResponse(Stream stream, Action<string, int, string> onResponseLine, Action<string, string> onHeader)
		{
			string protocol;
			string statusCode;
			string reasonPhrase;
			Parse3Tokens(stream, out protocol, out statusCode, out reasonPhrase);
			onResponseLine(protocol, Int32.Parse(statusCode), reasonPhrase);
			ParseHeaders(stream, onHeader);
		}

		private static void Parse3Tokens(Stream stream, out string token1, out string token2, out string token3)
		{
			var reader = new StringReader(stream.ReadLine());
			token1 = reader.ReadUntilWhitespace();
			reader.SkipWhitespace();
			token2 = reader.ReadUntilWhitespace();
			reader.SkipWhitespace();
			token3 = reader.ReadLine();
		}

		/// <summary>
		/// Executa o parser dos cabeçalhos.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="onHeader"></param>
		private static void ParseHeaders(Stream stream, Action<string, string> onHeader)
		{
			string headerLine = null;
			while (true)
			{
				headerLine = stream.ReadLine();
				if(headerLine == String.Empty)
				{
					break;
				}
				var headerReader = new StringReader(headerLine);
				string key = headerReader.ReadUntil(c => c == ':');
				headerReader.Read();
				headerReader.SkipWhitespace();
				string value = headerReader.ReadToEnd();
				onHeader(key, value);
			}
		}
	}
}
