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
using System.Globalization;
using System.Linq;
using System.Text;

namespace Colosoft.Serialization
{
	/// <summary>
	/// Transformador de dados binários.
	/// </summary>
	public sealed class BinaryTransformer
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		private BinaryTransformer()
		{
		}

		/// <summary>
		/// Recupera o byte da representação hexadecimal.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private static byte GetByteFromHexRepresentation(string text, int index)
		{
			byte halfByteFromHexRepresentation = GetHalfByteFromHexRepresentation(text, index);
			return (byte)(GetHalfByteFromHexRepresentation(text, index + 1) + (halfByteFromHexRepresentation * 0x10));
		}

		/// <summary>
		/// Recupera os bytes da representação Hexadecimal.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static byte[] GetBytesFromHexRepresentation(string text)
		{
			int length = text.Length;
			if(length == 0)
			{
				return null;
			}
			if((length % 2) != 0)
			{
				return null;
			}
			int num2 = length / 2;
			byte[] buffer = new byte[num2];
			for(int i = 0; i < num2; i++)
			{
				buffer[i] = GetByteFromHexRepresentation(text, i * 2);
			}
			return buffer;
		}

		/// <summary>
		/// Recupera o character do meio byte.
		/// </summary>
		/// <param name="halfByte"></param>
		/// <returns></returns>
		private static char GetChar(byte halfByte)
		{
			if(halfByte < 10)
			{
				return (char)(0x30 + halfByte);
			}
			return (char)((0x41 + halfByte) - 10);
		}

		/// <summary>
		/// Recupera o meio byte da representa hexadecimal.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private static byte GetHalfByteFromHexRepresentation(string text, int index)
		{
			char ch = text[index];
			if(('0' <= ch) && (ch <= '9'))
			{
				return (byte)(ch - '0');
			}
			if(('A' <= ch) && (ch <= 'F'))
			{
				return (byte)((ch - 'A') + 10);
			}
			return 0;
		}

		/// <summary>
		/// Recuper o texto dos dados informados.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string GetString(byte[] data)
		{
			return GetString(data, 0, data.Length);
		}

		/// <summary>
		/// Recupera o texto do byte informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetString(byte value)
		{
			StringBuilder builder = new StringBuilder(6);
			GetString(builder, value);
			return builder.ToString();
		}

		/// <summary>
		/// Recupera o texto do valor.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="value"></param>
		public static void GetString(StringBuilder builder, byte value)
		{
			switch(value)
			{
			case 2:
				builder.Append("[STX]");
				return;
			case 3:
				builder.Append("[ETX]");
				return;
			case 13:
				builder.Append("[CR]");
				return;
			}
			char ch = GetChar((byte)(value & 15));
			char ch2 = GetChar((byte)((value & 240) >> 4));
			builder.Append('[');
			builder.Append(ch2);
			builder.Append(ch);
			builder.Append(']');
		}

		/// <summary>
		/// Recupera o texto dos dados informados.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string GetString(byte[] data, Encoding encoding)
		{
			return GetString(data, 0, data.Length, encoding);
		}

		/// <summary>
		/// Recupera o texto dos dados informados.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string GetString(byte[] data, int start, int length)
		{
			StringBuilder builder = new StringBuilder();
			int num = start + length;
			if(data.Length < num)
			{
				num = data.Length;
			}
			for(int i = start; i < num; i++)
			{
				byte num3 = data[i];
				if((0x1f < num3) && (num3 < 0x7f))
				{
					builder.Append((char)num3);
				}
				else
				{
					GetString(builder, num3);
				}
			}
			return builder.ToString();
		}

		/// <summary>
		/// Recupera o texto dos dados informados.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string GetString(byte[] data, int start, int length, Encoding encoding)
		{
			if(encoding.Equals(Encoding.ASCII))
			{
				return GetString(data, start, length);
			}
			try
			{
				if(data.Length <= 0)
				{
					return string.Empty;
				}
				if(data.Length < start)
				{
					return string.Empty;
				}
				if(data.Length < (start + length))
				{
					length = data.Length - start;
				}
				if(length <= 0)
				{
					return string.Empty;
				}
				return encoding.GetString(data, start, length);
			}
			catch(ArgumentNullException)
			{
			}
			catch(ArgumentOutOfRangeException)
			{
			}
			catch(DecoderFallbackException)
			{
			}
			return string.Empty;
		}

		/// <summary>
		/// Verifica se possui caracteres inválidos.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool HasInvalidCharacters(string text)
		{
			try
			{
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				for(int i = 0; i < bytes.Length; i++)
				{
					if(bytes[i] <= 0x1f)
					{
						return true;
					}
					if(0x7f <= bytes[i])
					{
						return true;
					}
				}
			}
			catch(ArgumentNullException)
			{
			}
			catch(EncoderFallbackException)
			{
			}
			return false;
		}

		/// <summary>
		/// Remove os caracteres inválidos.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string TrimInvalidCharacters(string text)
		{
			try
			{
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				for(int i = 0; i < bytes.Length; i++)
				{
					if(bytes[i] <= 0x1f)
					{
						return Encoding.UTF8.GetString(bytes, 0, i);
					}
					if(0x7f <= bytes[i])
					{
						return Encoding.UTF8.GetString(bytes, 0, i);
					}
				}
			}
			catch(ArgumentNullException)
			{
			}
			catch(EncoderFallbackException)
			{
			}
			return text;
		}

		/// <summary>
		/// Tenta recupera o valor inteiro.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <param name="encoding"></param>
		/// <param name="integer"></param>
		/// <returns></returns>
		public static bool TryGetInteger(byte[] data, int start, int length, Encoding encoding, out int integer)
		{
			integer = 0;
			try
			{
				if(data == null)
				{
					return false;
				}
				if(data.Length <= 0)
				{
					return false;
				}
				if(data.Length < start)
				{
					return false;
				}
				if(length <= 0)
				{
					return false;
				}
				string str = encoding.GetString(data, start, length);
				if(string.IsNullOrEmpty(str))
				{
					return false;
				}
				integer = int.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
				return true;
			}
			catch(ArgumentNullException)
			{
			}
			catch(ArgumentOutOfRangeException)
			{
			}
			catch(DecoderFallbackException)
			{
			}
			catch(FormatException)
			{
			}
			catch(OverflowException)
			{
			}
			return false;
		}
	}
}
