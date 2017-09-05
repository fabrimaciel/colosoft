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

namespace Colosoft.Util
{
	/// <summary>
	/// Classe que calcula o sequence de campos texto
	/// </summary>
	public class Sequence : ISequence
	{
		/// <summary>
		/// Retorna a sequência
		/// </summary>
		/// <param name="prev">Código anterior</param>
		/// <param name="next">Código posterior</param>
		/// <returns></returns>
		public string GetSequence(string prev, string next)
		{
			if(string.IsNullOrEmpty(prev) && string.IsNullOrEmpty(next))
				return "IAAA";
			else if(!string.IsNullOrEmpty(prev) && string.IsNullOrEmpty(next))
			{
				var result = prev;
				IncSequence(ref result);
				return result;
			}
			else if(string.IsNullOrEmpty(prev) && !string.IsNullOrEmpty(next))
			{
				var result = next;
				DecSequence(ref result);
				return result;
			}
			else
			{
				var result = SplitSequence(prev, next);
				return result;
			}
		}

		/// <summary>
		/// Decrementa a sequencia
		/// </summary>
		/// <param name="value"></param>
		private void DecSequence(ref string value)
		{
			int step, index;
			step = 'E' - 'A';
			char[] array = value.ToCharArray();
			for(index = value.Length - 1; index >= 0; index--)
			{
				DelphiFunctions.Dec(ref array[index], step);
				if(array[index] < 'A')
				{
					step = 1;
					DelphiFunctions.Inc(ref array[index], 'Z' - 'A' + 1);
				}
				else
					break;
			}
			value = new string(array);
		}

		/// <summary>
		/// Incrementa a sequencia
		/// </summary>
		/// <param name="value"></param>
		private void IncSequence(ref string value)
		{
			int step, index;
			step = 'E' - 'A';
			char[] array = value.ToCharArray();
			for(index = value.Length - 1; index >= 0; index--)
			{
				DelphiFunctions.Inc(ref array[index], step);
				if(array[index] > 'Z')
				{
					step = 1;
					DelphiFunctions.Dec(ref array[index], 'Z' - 'A' + 1);
				}
				else
					break;
			}
			value = new string(array);
		}

		/// <summary>
		/// Divide sequencia
		/// </summary>
		/// <param name="prev"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		private string SplitSequence(string prev, string next)
		{
			char value, minVal, maxVal;
			int size, index;
			index = 0;
			size = prev.Length - 1;
			while (index <= size && prev[index] == next[index])
				index++;
			if(index >= 0 && index < prev.Length)
				minVal = prev[index];
			else
				minVal = DelphiFunctions.Pred('A');
			if(index >= 0 && index < next.Length)
				maxVal = next[index];
			else
				maxVal = DelphiFunctions.Succ('Z');
			value = (char)(((int)minVal + (int)maxVal) >> 1);
			while (value == minVal || value == maxVal)
			{
				if(index <= size)
					index++;
				maxVal = 'Z';
				if(index > size)
					minVal = 'A';
				else
					minVal = prev[index];
				value = (char)(((int)minVal + (int)maxVal) >> 1);
			}
			var result = prev;
			if(index > size)
				DelphiFunctions.SetLength(ref result, index);
			if(index >= size)
			{
				var resultArray = result.ToCharArray();
				resultArray[index] = value;
				result = new string(resultArray);
			}
			return result;
		}

		/// <summary>
		/// Construtor padrao
		/// </summary>
		public Sequence()
		{
		}
	}
}
