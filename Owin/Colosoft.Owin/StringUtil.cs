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

namespace Colosoft.Owin.Server
{
	static class StringUtil
	{
		internal static string CheckAndTrimString(string paramValue, string paramName)
		{
			return CheckAndTrimString(paramValue, paramName, true);
		}

		internal static string CheckAndTrimString(string paramValue, string paramName, bool throwIfNull)
		{
			return CheckAndTrimString(paramValue, paramName, throwIfNull, -1);
		}

		internal static string CheckAndTrimString(string paramValue, string paramName, bool throwIfNull, int lengthToCheck)
		{
			if(paramValue == null)
			{
				if(throwIfNull)
				{
					throw new ArgumentNullException(paramName);
				}
				return null;
			}
			string str = paramValue.Trim();
			if(str.Length == 0)
			{
				throw new ArgumentException(string.Format("TrimmedEmptyString {0}", paramName));
			}
			if((lengthToCheck > -1) && (str.Length > lengthToCheck))
			{
				throw new ArgumentException(string.Format("Trimmed String '{0}' Exceed Maximum Length {2}", new object[] {
					paramValue,
					paramName,
					lengthToCheck.ToString(System.Globalization.CultureInfo.InvariantCulture)
				}));
			}
			return str;
		}

		internal static bool Equals(string s1, string s2)
		{
			return ((s1 == s2) || (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)));
		}

		internal static bool EqualsIgnoreCase(string s1, string s2)
		{
			if(string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
			{
				return true;
			}
			if(string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
			{
				return false;
			}
			if(s2.Length != s1.Length)
			{
				return false;
			}
			return (0 == string.Compare(s1, 0, s2, 0, s2.Length, StringComparison.OrdinalIgnoreCase));
		}

		internal static bool EqualsIgnoreCase(string s1, int index1, string s2, int index2, int length)
		{
			return (string.Compare(s1, index1, s2, index2, length, StringComparison.OrdinalIgnoreCase) == 0);
		}

		internal static int GetNullTerminatedByteArray(Encoding enc, string s, out byte[] bytes)
		{
			bytes = null;
			if(s == null)
			{
				return 0;
			}
			bytes = new byte[enc.GetMaxByteCount(s.Length) + 1];
			return enc.GetBytes(s, 0, s.Length, bytes, 0);
		}

		internal static string[] ObjectArrayToStringArray(object[] objectArray)
		{
			string[] array = new string[objectArray.Length];
			objectArray.CopyTo(array, 0);
			return array;
		}

		internal static bool StringArrayEquals(string[] a, string[] b)
		{
			if((a == null) != (b == null))
			{
				return false;
			}
			if(a != null)
			{
				int length = a.Length;
				if(length != b.Length)
				{
					return false;
				}
				for(int i = 0; i < length; i++)
				{
					if(a[i] != b[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		internal static bool StringEndsWith(string s, char c)
		{
			int length = s.Length;
			return ((length != 0) && (s[length - 1] == c));
		}

		internal static bool StringEndsWithIgnoreCase(string s1, string s2)
		{
			int indexA = s1.Length - s2.Length;
			if(indexA < 0)
			{
				return false;
			}
			return (0 == string.Compare(s1, indexA, s2, 0, s2.Length, StringComparison.OrdinalIgnoreCase));
		}

		internal static bool StringStartsWith(string s, char c)
		{
			return ((s.Length != 0) && (s[0] == c));
		}

		internal static bool StringStartsWith(string s1, string s2)
		{
			if(s2.Length > s1.Length)
			{
				return false;
			}
			string str = s1;
			{
				var chPtr = str;
				if(chPtr != null)
				{
					chPtr = chPtr.Substring(System.Runtime.CompilerServices.RuntimeHelpers.OffsetToStringData);
				}
				var str2 = s2;
				{
					var chPtr2 = str2;
					if(chPtr2 != null)
					{
						chPtr2 = chPtr2.Substring(System.Runtime.CompilerServices.RuntimeHelpers.OffsetToStringData);
					}
					var chPtr3 = chPtr.ToArray();
					var chPtr4 = chPtr2.ToArray();
					int length = s2.Length;
					var index = 0;
					while (length-- > 0)
					{
						index++;
						if(chPtr3[index] != chPtr4[index])
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		internal static bool StringStartsWithIgnoreCase(string s1, string s2)
		{
			if(string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
			{
				return false;
			}
			if(s2.Length > s1.Length)
			{
				return false;
			}
			return (0 == string.Compare(s1, 0, s2, 0, s2.Length, StringComparison.OrdinalIgnoreCase));
		}
	}
}
