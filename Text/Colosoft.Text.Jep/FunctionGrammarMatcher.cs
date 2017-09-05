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

	public class FunctionGrammarMatcher : IGrammarMatcher
	{
		private Token close;

		private Token comma;

		[NonSerialized]
		private NodeFactory nf;

		private Token open;

		public FunctionGrammarMatcher(Token open, Token close, Token comma)
		{
			this.open = open;
			this.close = close;
			this.comma = comma;
		}

		public void Init(JepInstance jep)
		{
			this.nf = jep.NodeFac;
		}

		public INode Match(Lookahead2Enumerator<Token> it, IGrammarParser parser)
		{
			Token token = it.PeekNext();
			if(token == null)
			{
				return null;
			}
			if(!token.IsFunction())
			{
				return null;
			}
			string source = token.GetSource();
			IPostfixMathCommand pfmc = ((FunctionToken)token).GetPfmc();
			if(!this.open.Equals(it.PeekNextnext()))
			{
				return null;
			}
			it.Consume();
			it.Consume();
			if(this.close.Equals(it.PeekNext()))
			{
				if(!pfmc.CheckNumberOfParameters(0))
				{
					throw new Colosoft.Text.Jep.ParseException("Function " + pfmc + " invalid number of arguments 0");
				}
				it.Consume();
				return this.nf.BuildFunctionNode(source, pfmc, new INode[0]);
			}
			List<INode> list = new List<INode>();
			while (true)
			{
				INode item = parser.ParseSubExpression();
				list.Add(item);
				if(this.close.Equals(it.PeekNext()))
				{
					it.Consume();
					if(!pfmc.CheckNumberOfParameters(list.Count))
					{
						throw new Colosoft.Text.Jep.ParseException(string.Concat(new object[] {
							"Function ",
							pfmc,
							" invalid number of arguments ",
							list.Count
						}));
					}
					return this.nf.BuildFunctionNode(source, pfmc, list.ToArray());
				}
				if(!this.comma.Equals(it.PeekNext()))
				{
					break;
				}
				it.Consume();
			}
			throw new Colosoft.Text.Jep.ParseException("Closing bracket not found. Next token is " + it.PeekNext());
		}
	}
}
