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

namespace Colosoft.Owin.Server.Infrastructure
{
	/// <summary>
	/// Classe com método para auxiliar na manipulação de leitores de string.
	/// </summary>
	static class StringReaderExtensions
	{
		/// <summary>
		/// Salta espaços em branco.
		/// </summary>
		/// <param name="reader"></param>
		public static void SkipWhitespace(this StringReader reader)
		{
			ReadUntil(reader, c => !Char.IsWhiteSpace(c));
		}

		/// <summary>
		/// Lê até um espaço em branco.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static string ReadUntilWhitespace(this StringReader reader)
		{
			return ReadUntil(reader, c => Char.IsWhiteSpace(c));
		}

		/// <summary>
		/// Lê até o final.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static string ReadToEnd(this StringReader reader)
		{
			return ReadUntil(reader, c => false);
		}

		/// <summary>
		/// Lê até a condição informada.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static string ReadUntil(this StringReader reader, Func<char, bool> predicate)
		{
			var sb = new StringBuilder();
			int ch = -1;
			do
			{
				ch = reader.Peek();
				if(ch == -1 || predicate((char)ch))
				{
					break;
				}
				sb.Append((char)ch);
				reader.Read();
			}
			while (true);
			return sb.ToString();
		}
	}
}
