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

	internal class StringTokenMatcher : RegExpTokenMatcher
	{
		private char delim;

		public StringTokenMatcher(Regex pattern, char delim) : base(pattern)
		{
			this.delim = delim;
		}

		public override Token BuildToken(string s)
		{
			throw new JepException("Attempt to call BuildToken.");
		}

		private Token BuildToken(string source, string uquote)
		{
			return new StringToken(source, uquote, this.delim, false);
		}

		public static StringTokenMatcher DoubleQuoteStringMatcher()
		{
			return new StringTokenMatcher(new Regex("\"([^\"\\\\]*(\\\\.[^\"\\\\]*)*)\""), '"');
		}

		public override void Init(JepInstance j)
		{
		}

		public override Token Match(string s)
		{
			System.Text.RegularExpressions.Match match = base.pattern.Match(s);
			if(match.Success)
			{
				string uquote = match.Groups[1].Value.Replace("\\\"", "\"");
				return this.BuildToken(match.Value, uquote);
			}
			return null;
		}

		public static StringTokenMatcher SingleQuoteStringMatcher()
		{
			return new StringTokenMatcher(new Regex("'([^']*)'"), '\'');
		}
	}
}
