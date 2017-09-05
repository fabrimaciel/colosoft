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

namespace Colosoft.Text.Jep.ConfigurableParser.Tokens
{
	using Colosoft.Text.Jep;
	using Colosoft.Text.Jep.ConfigurableParser.Matchers;
	using System;

	public class MultiLineToken : Token, ITokenMatcher
	{
		private string buffer;

		private ITokenBuilder completeMatcher;

		private ITokenMatcher endMatcher;

		public MultiLineToken(string source, ITokenMatcher em, ITokenBuilder builder) : base(source)
		{
			this.buffer = source;
			this.endMatcher = em;
			this.completeMatcher = builder;
		}

		public void Append(string line)
		{
			this.buffer = this.buffer + '\n' + line;
		}

		public Token GetCompleteToken()
		{
			return this.completeMatcher.BuildToken(this.buffer);
		}

		public void Init(JepInstance j)
		{
		}

		public Token Match(string line)
		{
			Token token = this.endMatcher.Match(line);
			if(token == null)
			{
				this.Append(line);
				return token;
			}
			this.Append(token.GetSource());
			return token;
		}
	}
}
