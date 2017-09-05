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
	using Colosoft.Text.Jep.ConfigurableParser;
	using Colosoft.Text.Jep.ConfigurableParser.Tokens;
	using Colosoft.Text.Jep.Parser;
	using System;

	public class RoundBracketGrammarMatcher : IGrammarMatcher
	{
		private Token close;

		private Token open;

		public RoundBracketGrammarMatcher(Token open, Token close)
		{
			this.open = open;
			this.close = close;
		}

		public void Init(JepInstance jep)
		{
		}

		public INode Match(Lookahead2Enumerator<Token> it, IGrammarParser parser)
		{
			if(!this.open.Equals(it.PeekNext()))
			{
				return null;
			}
			it.Consume();
			INode node = parser.ParseSubExpression();
			if(!this.close.Equals(it.PeekNext()))
			{
				throw new Colosoft.Text.Jep.ParseException("Closing bracket not found");
			}
			it.Consume();
			return node;
		}
	}
}
