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
	using System.Collections.Generic;

	public class ListOrBracketGrammarMatcher : IGrammarMatcher
	{
		private Token close;

		private Token comma;

		[NonSerialized]
		private Operator list;

		[NonSerialized]
		private NodeFactory nf;

		private Token open;

		public ListOrBracketGrammarMatcher(Token open, Token close, Token comma)
		{
			this.open = open;
			this.close = close;
			this.comma = comma;
		}

		public void Init(JepInstance jep)
		{
			this.nf = jep.NodeFac;
			this.list = jep.OpTab.GetOperator(0x16);
		}

		public INode Match(Lookahead2Enumerator<Token> it, IGrammarParser parser)
		{
			if(!this.open.Equals(it.PeekNext()))
			{
				return null;
			}
			it.Consume();
			List<INode> list = new List<INode>();
			while (true)
			{
				INode item = parser.ParseSubExpression();
				list.Add(item);
				if(this.close.Equals(it.PeekNext()))
				{
					it.Consume();
					if(list.Count == 1)
					{
						return list[0];
					}
					return this.nf.BuildOperatorNode(this.list, list.ToArray());
				}
				if(!this.comma.Equals(it.PeekNext()))
				{
					break;
				}
				it.Consume();
			}
			throw new Colosoft.Text.Jep.ParseException("Closing bracket not found");
		}
	}
}
