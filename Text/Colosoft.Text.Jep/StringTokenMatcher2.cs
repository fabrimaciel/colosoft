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
	using System.Text;

	public class StringTokenMatcher2 : ITokenMatcher
	{
		private char delim;

		private bool includeQuotes;

		public StringTokenMatcher2(char delim, bool incQuotes)
		{
			this.delim = delim;
			this.includeQuotes = incQuotes;
		}

		private Token BuildToken(string source, string uquote)
		{
			return new StringToken(source, uquote, this.delim, this.includeQuotes);
		}

		public static StringTokenMatcher2 DoubleQuoteStringMatcher()
		{
			return new StringTokenMatcher2('"', false);
		}

		public void Init(JepInstance j)
		{
		}

		public Token Match(string s)
		{
			if(s[0] == this.delim)
			{
				StringBuilder builder = new StringBuilder();
				for(int i = 1; i < s.Length; i++)
				{
					char ch = s[i];
					if(ch == this.delim)
					{
						return this.BuildToken(s.Substring(0, i + 1), builder.ToString());
					}
					if(ch == '\\')
					{
						int num3;
						char ch2 = s[++i];
						switch(ch2)
						{
						case '"':
						{
							builder.Append('"');
							continue;
						}
						case '\'':
						{
							builder.Append('\'');
							continue;
						}
						case '\\':
						{
							builder.Append('\\');
							continue;
						}
						case 'r':
						{
							builder.Append('\r');
							continue;
						}
						case 't':
						{
							builder.Append('\t');
							continue;
						}
						case 'u':
						{
							try
							{
								char[] chArray = Convert.ToString(int.Parse(s.Substring(i + 1, i + 5))).ToCharArray();
								builder.Append(chArray);
								i += 4;
							}
							catch(Exception)
							{
							}
							continue;
						}
						case 'n':
						{
							builder.Append('\n');
							continue;
						}
						case 'b':
						{
							builder.Append('\b');
							continue;
						}
						case 'f':
						{
							builder.Append('\f');
							continue;
						}
						}
						char ch3 = (s.Length > (i + 1)) ? s[i + 1] : 'Z';
						char ch4 = (s.Length > (i + 2)) ? s[i + 2] : 'Z';
						if((ch2 < '0') || (ch2 > '8'))
						{
							throw new ParseException(@"Illegal escape sequence \" + ch2);
						}
						if((ch3 >= '0') && (ch3 <= '8'))
						{
							if((ch4 >= '0') && (ch4 <= '8'))
							{
								num3 = int.Parse(s.Substring(i, i + 3));
								i += 2;
							}
							else
							{
								num3 = int.Parse(s.Substring(i, i + 2));
								i++;
							}
						}
						else
						{
							num3 = int.Parse(s.Substring(i, i + 1));
						}
						if(num3 > 0xff)
						{
							return null;
						}
						char[] chArray2 = Convert.ToString(num3).ToCharArray();
						builder.Append(chArray2);
						continue;
					}
					builder.Append(ch);
				}
			}
			return null;
		}

		public static StringTokenMatcher2 SingleQuoteStringMatcher()
		{
			return new StringTokenMatcher2('\'', false);
		}

		public static StringTokenMatcher2 SingleQuoteStringMatcher(bool incQuotes)
		{
			return new StringTokenMatcher2('\'', incQuotes);
		}
	}
}
