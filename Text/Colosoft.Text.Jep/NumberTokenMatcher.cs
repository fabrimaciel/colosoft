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

namespace Colosoft.Text.Jep.ConfigurableParser.Matchers
{
	using Colosoft.Text.Jep;
	using Colosoft.Text.Jep.ConfigurableParser.Tokens;
	using System;
	using System.Text.RegularExpressions;

	public class NumberTokenMatcher : RegExpTokenMatcher
	{
		[NonSerialized]
		private INumberFactory nf;

		public NumberTokenMatcher(string regex) : base(regex)
		{
		}

		public NumberTokenMatcher(Regex pattern) : base(pattern)
		{
		}

		public override Token BuildToken(string s)
		{
			try
			{
				return new NumberToken(s, this.nf.CreateNumber(s));
			}
			catch(ParseException)
			{
				return null;
			}
		}

		public static NumberTokenMatcher DefaultNumberTokenMatcher()
		{
			return new NumberTokenMatcher(@"(\d+\.?\d*)|(\.\d+)");
		}

		public static NumberTokenMatcher ExponentNumberTokenMatcher()
		{
			return new NumberTokenMatcher(@"((\d+\.?\d*)|(\.\d+))(?:[eE][+-]?\d+)?");
		}

		public override void Init(JepInstance jep)
		{
			this.nf = jep.NumFac;
		}
	}
}
