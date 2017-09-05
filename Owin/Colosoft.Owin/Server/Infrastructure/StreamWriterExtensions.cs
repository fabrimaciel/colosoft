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
	static class StreamWriterExtensions
	{
		static string HttpNewLine = "\r\n";

		public static void WriteHttpLine(this StreamWriter writer)
		{
			writer.Write(HttpNewLine);
		}

		public static void WriteHttpLine(this StreamWriter writer, string format, object arg0, object arg1)
		{
			writer.Write(format + HttpNewLine, arg0, arg1);
		}

		public static void WriteHttpLine(this StreamWriter writer, string format, object arg0, object arg1, object arg2)
		{
			writer.Write(format + HttpNewLine, arg0, arg1, arg2);
		}
	}
}
