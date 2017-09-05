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
	/// Classe que auxilia na manipulação de vetores.
	/// </summary>
	public static class ArrayUtil
	{
		/// <summary>
		/// Compara os vetores.
		/// </summary>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <returns></returns>
		public static bool Equals(byte[] a1, byte[] a2)
		{
			if(a1.Length != a2.Length)
				return false;
			return ((a1.Length == 0) || Equals(a1, a2, a1.Length));
		}

		/// <summary>
		/// Compara os vetores.
		/// </summary>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static bool Equals(byte[] a1, byte[] a2, int length)
		{
			for(var i = 0; i < length; i++)
			{
				if(a1.Length <= i || a2.Length <= i)
					return false;
				if(a1[i] != a2[i])
					return false;
			}
			return true;
		}

		/// <summary>
		/// Recupera o hash do vetor.
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static int GetHashCode(byte[] array)
		{
			int num = 0;
			foreach (byte num2 in array)
				num += num2;
			return num;
		}
	}
}
