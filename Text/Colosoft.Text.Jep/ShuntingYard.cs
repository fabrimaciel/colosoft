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

namespace Colosoft.Text.Jep.ConfigurableParser
{
	using Colosoft.Text.Jep;
	using Colosoft.Text.Jep.ConfigurableParser.Matchers;
	using Colosoft.Text.Jep.ConfigurableParser.Tokens;
	using Colosoft.Text.Jep.DataStructures;
	using Colosoft.Text.Jep.Parser;
	using System;
	using System.Collections.Generic;

	public class ShuntingYard : IGrammarParser
	{
		protected readonly bool DUMP;

		protected Lookahead2Enumerator<Token> it;

		protected JepInstance jep;

		protected List<IGrammarMatcher> matchers;

		protected Stack<INode> nodes = new Stack<INode>();

		protected Stack<Operator> ops = new Stack<Operator>();

		protected static Operator sentinel = new Operator("Sentinel", null, 0);

		public ShuntingYard(JepInstance jep, List<IGrammarMatcher> gm)
		{
			this.jep = jep;
			this.matchers = gm;
		}

		private bool CompareOps(Operator op1, Operator op2)
		{
			if(op1.IsTernary() && op2.IsTernary())
			{
				return ((op1 is TernaryOperator.RhsTernaryOperator) && (op2 is TernaryOperator.RhsTernaryOperator));
			}
			if(op1 == sentinel)
			{
				return false;
			}
			if(op2 == sentinel)
			{
				return true;
			}
			if(op2.IsPrefix() && op1.IsBinary())
			{
				return false;
			}
			return ((op1.GetPrecedence() < op2.GetPrecedence()) || ((op1.GetPrecedence() == op2.GetPrecedence()) && op1.IsLeftBinding()));
		}

		protected void DumpState(string msg)
		{
			Console.WriteLine(string.Concat(new object[] {
				msg,
				"\t",
				this.it.PeekNext(),
				"\t",
				this.it.PeekNextnext()
			}));
			Console.WriteLine("Nodes: " + this.nodes.ToString());
			Console.WriteLine("Ops: [");
			foreach (Operator @operator in this.ops)
			{
				Console.WriteLine(@operator.GetName() + " ");
			}
			Console.WriteLine("]");
			Console.WriteLine();
		}

		private void Expression()
		{
			if(this.DUMP)
			{
				this.DumpState("E");
			}
			this.PrefixSuffix();
			if(this.DUMP)
			{
				this.DumpState("E2");
			}
			for(Token token = this.it.PeekNext(); token != null; token = this.it.PeekNext())
			{
				if(token.IsBinary())
				{
					this.PushOp(((OperatorToken)this.it.PeekNext()).GetBinaryOp());
					this.it.Consume();
					if(this.DUMP)
					{
						this.DumpState("E3");
					}
					this.PrefixSuffix();
				}
				else if(token.IsTernary())
				{
					Operator ternaryOp = ((OperatorToken)this.it.PeekNext()).GetTernaryOp();
					this.PushOp(ternaryOp);
					this.it.Consume();
					this.PrefixSuffix();
				}
				else
				{
					if(!token.IsImplicitMulRhs())
					{
						break;
					}
					if(!this.jep.ImplicitMul)
					{
						throw new Colosoft.Text.Jep.ParseException("Implicit multiplication not enabled");
					}
					this.PrefixSuffix();
					INode rhs = this.nodes.Pop();
					INode lhs = this.nodes.Pop();
					INode item = this.jep.NodeFac.BuildOperatorNode(this.jep.OpTab.GetOperator(5), lhs, rhs);
					this.nodes.Push(item);
				}
				if(this.DUMP)
				{
					this.DumpState("E3");
				}
			}
			while (!sentinel.Equals(this.ops.Peek()))
			{
				this.PopOp();
				if(this.DUMP)
				{
					this.DumpState("E4");
				}
			}
		}

		public Lookahead2Enumerator<Token> GetIterator()
		{
			return this.it;
		}

		public INode Parse(IExtendedEnumerator<Token> input)
		{
			INode node;
			if(!input.HasNext())
			{
				return null;
			}
			this.it = new Lookahead2Enumerator<Token>(input);
			try
			{
				node = this.ParseSubExpression();
			}
			catch(Colosoft.Text.Jep.ParseException exception)
			{
				if(this.it.PeekNext() != null)
				{
					exception.SetPosition(this.it.PeekNext().GetLineNumber(), this.it.PeekNext().GetColumnNumber());
				}
				throw exception;
			}
			if(this.it.PeekNext() != null)
			{
				string str = this.it.PeekNext().ToString();
				if(this.it.PeekNextnext() != null)
				{
					str = str + ", " + this.it.PeekNextnext();
				}
				Colosoft.Text.Jep.ParseException exception2 = new Colosoft.Text.Jep.ParseException("Tokens still remaining after parse completed", this.it.PeekNext().GetLineNumber(), this.it.PeekNext().GetColumnNumber());
				throw exception2;
			}
			if(this.nodes.Count != 0)
			{
				throw new Colosoft.Text.Jep.ParseException("Only one node should be of stack after parsing, it has " + this.nodes.Count);
			}
			return node;
		}

		public INode ParseSubExpression()
		{
			this.ops.Push(sentinel);
			try
			{
				this.Expression();
			}
			catch(Colosoft.Text.Jep.ParseException exception)
			{
				throw exception;
			}
			if(!sentinel.Equals(this.ops.Pop()))
			{
				throw new Colosoft.Text.Jep.ParseException("Top of stack should be a sentinel");
			}
			return this.nodes.Pop();
		}

		private void PopOp()
		{
			Operator op = this.ops.Pop();
			if(op.IsBinary())
			{
				INode rhs = this.nodes.Pop();
				INode lhs = this.nodes.Pop();
				INode item = this.jep.NodeFac.BuildOperatorNode(op, lhs, rhs);
				this.nodes.Push(item);
			}
			else if(op.IsUnary())
			{
				INode child = this.nodes.Pop();
				INode node5 = this.jep.NodeFac.BuildOperatorNode(op, child);
				this.nodes.Push(node5);
			}
			else
			{
				if(!op.IsTernary() || !(op is TernaryOperator.RhsTernaryOperator))
				{
					throw new Colosoft.Text.Jep.ParseException("INode on stack should be unary or binary");
				}
				Operator operator2 = this.ops.Pop();
				if(!((TernaryOperator)operator2).GetRhsOperator().Equals(op))
				{
					throw new Colosoft.Text.Jep.ParseException("Next operator should have been matching ternary op. ");
				}
				INode node6 = this.nodes.Pop();
				INode node7 = this.nodes.Pop();
				INode node8 = this.nodes.Pop();
				INode node9 = this.jep.NodeFac.BuildOperatorNode(operator2, new INode[] {
					node8,
					node7,
					node6
				});
				this.nodes.Push(node9);
			}
		}

		private void Prefix()
		{
			foreach (IGrammarMatcher matcher in this.matchers)
			{
				INode item = matcher.Match(this.it, this);
				if(item != null)
				{
					this.nodes.Push(item);
					return;
				}
			}
			Token token = this.it.PeekNext();
			if(token == null)
			{
				throw new Colosoft.Text.Jep.ParseException("Unexpected end of input");
			}
			if(token.IsIdentifier())
			{
				this.it.Consume();
				this.nodes.Push(this.jep.NodeFac.BuildVariableNodeCheckUndeclared(token.GetSource()));
			}
			else if(token.IsNumber())
			{
				this.it.Consume();
				this.nodes.Push(this.jep.NodeFac.BuildConstantNode(((NumberToken)token).GetValue()));
			}
			else if(token.IsString())
			{
				this.it.Consume();
				this.nodes.Push(this.jep.NodeFac.BuildConstantNode(((StringToken)token).GetCompleteString()));
			}
			else
			{
				if(!token.IsPrefix())
				{
					throw new Colosoft.Text.Jep.ParseException("Unexpected token " + token.ToString());
				}
				this.PushOp(((OperatorToken)token).GetPrefixOp());
				this.it.Consume();
				this.PrefixSuffix();
			}
		}

		private void PrefixSuffix()
		{
			this.Prefix();
			while (true)
			{
				Token token = this.it.PeekNext();
				if((token == null) || !token.IsSuffix())
				{
					return;
				}
				this.PushOp(((OperatorToken)token).GetSuffixOp());
				this.it.Consume();
			}
		}

		private void PushOp(Operator op)
		{
			while (this.CompareOps(this.ops.Peek(), op))
			{
				this.PopOp();
			}
			this.ops.Push(op);
		}

		public void SetIterator(Lookahead2Enumerator<Token> it)
		{
			this.it = it;
		}
	}
}
