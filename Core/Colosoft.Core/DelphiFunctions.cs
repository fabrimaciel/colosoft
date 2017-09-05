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
using System.Collections;

namespace Colosoft.Util
{
	/// <summary>
	/// Funções nativas do Delphi traduzidas para C#
	/// </summary>
	public class DelphiFunctions
	{
		/// <summary>
		/// Decrementa um valor ordinal
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static char Pred(char input)
		{
			return (char)((int)input - 1);
		}

		/// <summary>
		/// Incrementa um valor ordinal
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static char Succ(char input)
		{
			return (char)((int)input + 1);
		}

		/// <summary>
		/// Decrementa o ordinal do parâmetro 'input' com o valor 1
		/// </summary>
		/// <param name="input"></param>
		public static void Dec(ref char input)
		{
			Dec(ref input, 1);
		}

		/// <summary>
		/// Decremeta o ordinal do parâmetro 'input' respentando o valor do parâmetro 'value'
		/// </summary>
		/// <param name="input"></param>
		/// <param name="value"></param>
		public static void Dec(ref char input, int value)
		{
			input -= (char)value;
		}

		/// <summary>
		/// Incrementa o ordinal do parâmetro 'input' com o valor 1
		/// </summary>
		/// <param name="input"></param>
		public static void Inc(ref char input)
		{
			Inc(ref input, 1);
		}

		/// <summary>
		/// Incrementa o ordinal do parâmetro 'input' respeitando o valor do parâmetro 'value'
		/// </summary>
		/// <param name="input"></param>
		/// <param name="value"></param>
		public static void Inc(ref char input, int value)
		{
			input += (char)value;
		}

		/// <summary>
		/// Seta o length de uma string
		/// </summary>
		/// <param name="text"></param>
		/// <param name="value"></param>
		public static void SetLength(ref string text, int value)
		{
			if(value < 0)
				return;
			if(value < text.Length)
				text = text.Substring(0, value);
			else
				for(int i = text.Length - 1; i < value; i++)
					text = string.Concat(text, " ");
		}
	}
}
