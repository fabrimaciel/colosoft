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

namespace Colosoft.Owin.Server.Infrastructure
{
	/// <summary>
	/// Classe com métodos para auxiliar na manipulação de streams.
	/// </summary>
	static class StreamExtensions
	{
		/// <summary>
		/// Lê uma linha.
		/// </summary>
		/// <param name="ns"></param>
		/// <returns></returns>
		public static string ReadLine(this System.IO.Stream ns)
		{
			var builder = new StringBuilder();
			byte cur = 0;
			byte prev = 0;
			while (true)
			{
				if(cur == 10)
				{
					if(prev == 13)
					{
						builder.Remove(builder.Length - 2, 2);
					}
					else
					{
						builder.Remove(builder.Length - 1, 1);
					}
					break;
				}
				prev = cur;
				cur = (byte)ns.ReadByte();
				builder.Append((char)cur);
			}
			return builder.ToString();
		}
	}
}
