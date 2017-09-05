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
using System.Text;

namespace QiHe.CodeLib
{
	/// <summary>
	/// Binary data to Hexadecimal string
	/// </summary>
	public class Bin2Hex
	{
		public static string Encode(byte[] data)
		{
			StringBuilder code = new StringBuilder(data.Length * 2);
			foreach (byte bt in data)
			{
				code.Append(bt.ToString("X2"));
			}
			return code.ToString();
		}

		public static string Format(byte[] data)
		{
			StringBuilder code = new StringBuilder();
			int count = 0;
			foreach (byte bt in data)
			{
				code.AppendFormat("{0:X2} ", bt);
				count++;
				if(count == 16)
				{
					code.AppendLine();
					count = 0;
				}
			}
			return code.ToString();
		}

		public static byte[] Decode(string code)
		{
			byte[] bytes = new byte[code.Length / 2];
			for(int i = 0; i < bytes.Length; i++)
			{
				bytes[i] = byte.Parse(code.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
			}
			return bytes;
		}
	}
}
