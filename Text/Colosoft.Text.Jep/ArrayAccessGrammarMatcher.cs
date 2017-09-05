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
	using System.Collections;

	public class ArrayAccessGrammarMatcher : IGrammarMatcher
	{
		private Token close;

		[NonSerialized]
		private NodeFactory nf;

		private Token open;

		[NonSerialized]
		private OperatorTable ot;

		public ArrayAccessGrammarMatcher(SymbolToken open, SymbolToken close)
		{
			this.open = open;
			this.close = close;
		}

		public void Init(JepInstance jep)
		{
			this.nf = jep.NodeFac;
			this.ot = jep.OpTab;
		}

		public INode Match(Lookahead2Enumerator<Token> it, IGrammarParser parser)
		{
			Token token = it.PeekNext();
			if(token == null)
			{
				return null;
			}
			if(!token.IsIdentifier())
			{
				return null;
			}
			if(!this.open.Equals(it.PeekNextnext()))
			{
				return null;
			}
			it.Consume();
			ArrayList list = new ArrayList(4);
			list.Add(this.nf.BuildVariableNodeCheckUndeclared(token.GetSource()));
			while (this.open.Equals(it.PeekNext()))
			{
				it.Consume();
				INode node = parser.ParseSubExpression();
				list.Add(node);
				if(!this.close.Equals(it.PeekNext()))
				{
					throw new Colosoft.Text.Jep.ParseException("Closing bracket not found, next token is " + it.PeekNext());
				}
				it.Consume();
			}
			INode[] arguments = new INode[list.Count];
			arguments = (INode[])list.ToArray(Type.GetType("Colosoft.Text.Jep.Parser.INode"));
			return this.nf.BuildOperatorNode(this.ot.GetOperator(0x17), arguments);
		}
	}
}
