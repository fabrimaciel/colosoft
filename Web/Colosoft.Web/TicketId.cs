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

namespace Colosoft.Web.Security.AccessControl
{
	/// <summary>
	/// Classe responsável por gerar os identificadores.
	/// </summary>
	static class TicketId
	{
		private static char[] _encoding = new char[] {
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'g',
			'h',
			'i',
			'j',
			'k',
			'l',
			'm',
			'n',
			'o',
			'p',
			'q',
			'r',
			's',
			't',
			'u',
			'v',
			'w',
			'x',
			'y',
			'z',
			'0',
			'1',
			'2',
			'3',
			'4',
			'5'
		};

		/// <summary>
		/// Armazena os caracteres legais.
		/// </summary>
		private static bool[] _legalchars = new bool[0x80];

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		static TicketId()
		{
			for(int i = _encoding.Length - 1; i >= 0; i--)
			{
				char index = _encoding[i];
				_legalchars[index] = true;
			}
		}

		/// <summary>
		/// Codifica o buffer para um string.
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		private static string Encode(byte[] buffer)
		{
			char[] chArray = new char[0x18];
			int num2 = 0;
			for(int i = 0; i < 15; i += 5)
			{
				int num4 = ((buffer[i] | (buffer[i + 1] << 8)) | (buffer[i + 2] << 0x10)) | (buffer[i + 3] << 0x18);
				int index = num4 & 0x1f;
				chArray[num2++] = _encoding[index];
				index = (num4 >> 5) & 0x1f;
				chArray[num2++] = _encoding[index];
				index = (num4 >> 10) & 0x1f;
				chArray[num2++] = _encoding[index];
				index = (num4 >> 15) & 0x1f;
				chArray[num2++] = _encoding[index];
				index = (num4 >> 20) & 0x1f;
				chArray[num2++] = _encoding[index];
				index = (num4 >> 0x19) & 0x1f;
				chArray[num2++] = _encoding[index];
				num4 = ((num4 >> 30) & 3) | (buffer[i + 4] << 2);
				index = num4 & 0x1f;
				chArray[num2++] = _encoding[index];
				index = (num4 >> 5) & 0x1f;
				chArray[num2++] = _encoding[index];
			}
			return new string(chArray);
		}

		/// <summary>
		/// Cria um novo identificador;
		/// </summary>
		/// <param name="randgen"></param>
		/// <returns></returns>
		public static string Create(ref System.Security.Cryptography.RandomNumberGenerator randgen)
		{
			if(randgen == null)
				randgen = new System.Security.Cryptography.RNGCryptoServiceProvider();
			byte[] data = new byte[15];
			randgen.GetBytes(data);
			return Encode(data);
		}

		/// <summary>
		/// Verifica se o ID informado é legitimo.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static bool IsLegit(string s)
		{
			if((s == null) || (s.Length != 0x18))
			{
				return false;
			}
			try
			{
				int num = 0x18;
				while (--num >= 0)
				{
					char index = s[num];
					if(!_legalchars[index])
					{
						return false;
					}
				}
				return true;
			}
			catch(IndexOutOfRangeException)
			{
				return false;
			}
		}
	}
}
