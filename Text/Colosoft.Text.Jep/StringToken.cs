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
	using System;

	public class StringToken : Token
	{
		private char delim;

		private bool includeQuotes;

		private string str;

		public StringToken(string source, string uquote, char delim, bool incQuotes) : base(source)
		{
			this.str = uquote;
			this.delim = delim;
			this.includeQuotes = incQuotes;
		}

		public string GetCompleteString()
		{
			if(this.includeQuotes)
			{
				return (this.delim + this.str + this.delim);
			}
			return this.str;
		}

		public char GetQuoteChar()
		{
			return this.delim;
		}

		public string GetUnquotedString()
		{
			return this.str;
		}

		public override bool IsString()
		{
			return true;
		}
	}
}
