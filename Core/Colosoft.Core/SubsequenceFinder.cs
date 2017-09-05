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
	///     Provides methods to find a subsequence within a
	///     sequence.
	/// </summary>
	internal class SubsequenceFinder
	{
		public static int Search(byte[] haystack, byte[] needle)
		{
			return Search(haystack, needle, haystack.Length);
		}

		/// <summary>
		///     Finds if a sequence exists within another sequence. 
		/// </summary>
		/// <param name="haystack">
		///     The sequence to search
		/// </param>
		/// <param name="needle">
		///     The sequence to look for
		/// </param>
		/// <param name="haystackLength">
		///     The length of the haystack to consider for searching
		/// </param>
		/// <returns>
		///     The start position of the found sequence or -1 if nothing was found
		/// </returns>
		public static int Search(byte[] haystack, byte[] needle, int haystackLength)
		{
			var charactersInNeedle = new HashSet<byte>(needle);
			var length = needle.Length;
			var index = 0;
			while (index + length <= haystackLength)
			{
				if(charactersInNeedle.Contains(haystack[index + length - 1]))
				{
					var needleIndex = 0;
					while (haystack[index + needleIndex] == needle[needleIndex])
					{
						if(needleIndex == needle.Length - 1)
						{
							return index;
						}
						needleIndex += 1;
					}
					index += 1;
					index += needleIndex;
					continue;
				}
				index += length;
			}
			return -1;
		}
	}
}
