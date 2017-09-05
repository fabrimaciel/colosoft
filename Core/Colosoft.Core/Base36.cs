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

namespace Colosoft.Licensing
{
	class BASE36
	{
		private const string _charList = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		private static readonly char[] _charArray = _charList.ToCharArray();

		public static long Decode(string input)
		{
			long _result = 0;
			double _pow = 0;
			for(int _i = input.Length - 1; _i >= 0; _i--)
			{
				char _c = input[_i];
				int pos = _charList.IndexOf(_c);
				if(pos > -1)
					_result += pos * (long)Math.Pow(_charList.Length, _pow);
				else
					return -1;
				_pow++;
			}
			return _result;
		}

		public static string Encode(ulong input)
		{
			StringBuilder _sb = new StringBuilder();
			do
			{
				_sb.Append(_charArray[input % (ulong)_charList.Length]);
				input /= (ulong)_charList.Length;
			}
			while (input != 0);
			return Reverse(_sb.ToString());
		}

		private static string Reverse(string s)
		{
			var charArray = s.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}
	}
}
