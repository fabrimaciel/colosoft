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

namespace Colosoft.Web
{
	/// <summary>
	/// Classe com método para auxiliar na manipulação do http.
	/// </summary>
	public sealed class HttpUtility
	{
		/// <summary>
		/// Converte um valor hexadecimal para inteiro.
		/// </summary>
		/// <param name="h"></param>
		/// <returns></returns>
		private static int HexToInt(char h)
		{
			if((h >= '0') && (h <= '9'))
				return (h - '0');
			if((h >= 'a') && (h <= 'f'))
				return ((h - 'a') + 10);
			if((h >= 'A') && (h <= 'F'))
				return ((h - 'A') + 10);
			return -1;
		}

		/// <summary>
		/// Converter um inteiro para hexdecimal.
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		internal static char IntToHex(int n)
		{
			if(n <= 9)
			{
				return (char)(n + 0x30);
			}
			return (char)((n - 10) + 0x61);
		}

		/// <summary>
		/// Decodifica a URL.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		private static string UrlDecodeStringFromStringInternal(string s, Encoding e)
		{
			int length = s.Length;
			var decoder = new UrlDecoder(length, e);
			for(int i = 0; i < length; i++)
			{
				char ch = s[i];
				if(ch == '+')
				{
					ch = ' ';
				}
				else if((ch == '%') && (i < (length - 2)))
				{
					if((s[i + 1] == 'u') && (i < (length - 5)))
					{
						int num3 = HexToInt(s[i + 2]);
						int num4 = HexToInt(s[i + 3]);
						int num5 = HexToInt(s[i + 4]);
						int num6 = HexToInt(s[i + 5]);
						if(((num3 < 0) || (num4 < 0)) || ((num5 < 0) || (num6 < 0)))
						{
							if((ch & 0xff80) == 0)
								decoder.AddByte((byte)ch);
							else
								decoder.AddChar(ch);
							continue;
						}
						ch = (char)((((num3 << 12) | (num4 << 8)) | (num5 << 4)) | num6);
						i += 5;
						decoder.AddChar(ch);
						continue;
					}
					int num7 = HexToInt(s[i + 1]);
					int num8 = HexToInt(s[i + 2]);
					if((num7 >= 0) && (num8 >= 0))
					{
						byte b = (byte)((num7 << 4) | num8);
						i += 2;
						decoder.AddByte(b);
						continue;
					}
				}
				if((ch & 0xff80) == 0)
					decoder.AddByte((byte)ch);
				else
					decoder.AddChar(ch);
			}
			return decoder.GetString();
		}

		/// <summary>
		/// Codifiac a string para URL.
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <param name="alwaysCreateReturnValue"></param>
		/// <returns></returns>
		private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
		{
			int num = 0;
			int num2 = 0;
			for(int i = 0; i < count; i++)
			{
				char ch = (char)bytes[offset + i];
				if(ch == ' ')
				{
					num++;
				}
				else if(!IsSafe(ch))
				{
					num2++;
				}
			}
			if((!alwaysCreateReturnValue && (num == 0)) && (num2 == 0))
			{
				return bytes;
			}
			byte[] buffer = new byte[count + (num2 * 2)];
			int num4 = 0;
			for(int j = 0; j < count; j++)
			{
				byte num6 = bytes[offset + j];
				char ch2 = (char)num6;
				if(IsSafe(ch2))
				{
					buffer[num4++] = num6;
				}
				else if(ch2 == ' ')
				{
					buffer[num4++] = 0x2b;
				}
				else
				{
					buffer[num4++] = 0x25;
					buffer[num4++] = (byte)IntToHex((num6 >> 4) & 15);
					buffer[num4++] = (byte)IntToHex(num6 & 15);
				}
			}
			return buffer;
		}

		/// <summary>
		/// Verifica se é um caracter seguro.
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		internal static bool IsSafe(char ch)
		{
			if((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9')))
			{
				return true;
			}
			switch(ch)
			{
			case '\'':
			case '(':
			case ')':
			case '*':
			case '-':
			case '.':
			case '_':
			case '!':
				return true;
			}
			return false;
		}

		/// <summary>
		/// Realiza o encode da url para pbytes.
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
		{
			if((bytes == null) && (count == 0))
				return null;
			if(bytes == null)
				throw new ArgumentNullException("bytes");
			if((offset < 0) || (offset > bytes.Length))
				throw new ArgumentOutOfRangeException("offset");
			if((count < 0) || ((offset + count) > bytes.Length))
				throw new ArgumentOutOfRangeException("count");
			return UrlEncodeBytesToBytesInternal(bytes, offset, count, true);
		}

		/// <summary>
		/// Realiza o encode da url.
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static string UrlEncode(byte[] bytes, int offset, int count)
		{
			if(bytes == null)
			{
				return null;
			}
			return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes, offset, count));
		}

		/// <summary>
		/// Realiza o encode da url.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static byte[] UrlEncodeToBytes(string str, Encoding e)
		{
			if(str == null)
			{
				return null;
			}
			byte[] bytes = e.GetBytes(str);
			return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
		}

		/// <summary>
		/// Realiza o encode da url.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string UrlEncode(string str, Encoding e)
		{
			if(str == null)
				return null;
			return Encoding.ASCII.GetString(UrlEncodeToBytes(str, e));
		}

		/// <summary>
		/// Realiza o encode da url.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string UrlEncode(string str)
		{
			if(str == null)
			{
				return null;
			}
			return UrlEncode(str, Encoding.UTF8);
		}

		/// <summary>
		/// Decodifica a url.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string UrlDecode(string str, Encoding e)
		{
			if(str == null)
				return null;
			return UrlDecodeStringFromStringInternal(str, e);
		}

		/// <summary>
		/// Decodifica a url.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string UrlDecode(string str)
		{
			if(str == null)
				return null;
			return UrlDecode(str, Encoding.UTF8);
		}
	}
}
