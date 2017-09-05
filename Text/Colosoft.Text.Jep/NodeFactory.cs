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

namespace Colosoft.Text.Jep
{
	using Colosoft.Text.Jep.Parser;
	using System;

	public class NodeFactory : IJepComponent
	{
		[NonSerialized]
		protected IEvaluator ev;

		[NonSerialized]
		protected JepInstance j;

		protected static readonly int JJTCONSTANT = 5;

		protected static readonly int JJTFUNNODE = 4;

		protected static readonly int JJTVARNODE = 3;

		[NonSerialized]
		protected VariableTable vt;

		public ASTConstant BuildConstantNode(ASTConstant node)
		{
			return this.BuildConstantNode(node.Value);
		}

		public ASTConstant BuildConstantNode(object value)
		{
			return new ASTConstant(JJTCONSTANT) {
				Value = value
			};
		}

		public ASTConstant BuildConstantNode(IPostfixMathCommand pfmc, INode[] children)
		{
			object obj2;
			INode node = this.BuildFunctionNode("tmpfun", pfmc, children);
			try
			{
				obj2 = this.ev.Eval(node);
			}
			catch(EvaluationException exception)
			{
				throw new Colosoft.Text.Jep.ParseException("", exception);
			}
			return this.BuildConstantNode(obj2);
		}

		public ASTConstant BuildConstantNode(Operator op, INode[] children)
		{
			return this.BuildConstantNode(op.GetPFMC(), children);
		}

		public ASTConstant BuildConstantNode(Operator op, INode child1)
		{
			return this.BuildConstantNode(op.GetPFMC(), new INode[] {
				child1
			});
		}

		public ASTConstant BuildConstantNode(Operator op, INode child1, INode child2)
		{
			return this.BuildConstantNode(op.GetPFMC(), new INode[] {
				child1,
				child2
			});
		}

		public ASTFunNode BuildFunctionNode(ASTFunNode node, INode[] arguments)
		{
			return this.BuildFunctionNode(node.GetName(), node.GetPFMC(), arguments);
		}

		public ASTFunNode BuildFunctionNode(string name, IPostfixMathCommand pfmc, INode[] arguments)
		{
			if(!pfmc.CheckNumberOfParameters(arguments.Length))
			{
				throw new Colosoft.Text.Jep.ParseException(string.Concat(new object[] {
					"Incorrect number of parameters ",
					arguments.Length,
					" for ",
					name,
					" "
				}));
			}
			ASTFunNode node = new ASTFunNode(JJTFUNNODE);
			node.SetFunction(name, pfmc);
			this.CopyChildren(node, arguments);
			return node;
		}

		public ASTOpNode BuildOperatorNode(Operator op, INode[] arguments)
		{
			ASTOpNode node = new ASTOpNode(JJTFUNNODE) {
				Op = op
			};
			this.CopyChildren(node, arguments);
			return node;
		}

		public ASTOpNode BuildOperatorNode(Operator op, INode child)
		{
			return this.BuildOperatorNode(op, new INode[] {
				child
			});
		}

		public ASTOpNode BuildOperatorNode(Operator op, INode lhs, INode rhs)
		{
			return this.BuildOperatorNode(op, new INode[] {
				lhs,
				rhs
			});
		}

		public ASTOpNode BuildUnfinishedOperatorNode(Operator op)
		{
			return new ASTOpNode(JJTFUNNODE) {
				Op = op
			};
		}

		public ASTVarNode BuildVariableNode(ASTVarNode node)
		{
			return this.BuildVariableNode(node.Var);
		}

		public ASTVarNode BuildVariableNode(Variable var)
		{
			return new ASTVarNode(JJTVARNODE) {
				Var = var
			};
		}

		public ASTVarNode BuildVariableNode(string name)
		{
			ASTVarNode node = new ASTVarNode(JJTVARNODE);
			Variable var = this.vt.AddVariable(name);
			node.Var = var;
			return this.BuildVariableNode(var);
		}

		public ASTVarNode BuildVariableNodeCheckUndeclared(string name)
		{
			if(!this.j.AllowUndeclared && (this.vt.GetVariable(name) == null))
			{
				throw new Colosoft.Text.Jep.ParseException("Variable '" + name + "' undefined. Use jep.SetAllowUndeclared() to allow undeclared variables.");
			}
			return this.BuildVariableNode(name);
		}

		public void CopyChildren(INode node, INode[] children)
		{
			int length = children.Length;
			node.JjtOpen();
			for(int i = 0; i < length; i++)
			{
				children[i].JjtSetParent(node);
				node.JjtAddChild(children[i], i);
			}
			node.JjtClose();
		}

		public IJepComponent GetLightWeightInstance()
		{
			return new NodeFactory();
		}

		public void Init(JepInstance jep)
		{
			this.j = jep;
			this.ev = this.j.Evaluator;
			this.vt = this.j.VarTab;
		}
	}
}
