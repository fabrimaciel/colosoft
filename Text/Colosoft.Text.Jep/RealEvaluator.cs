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

namespace Colosoft.Text.Jep.Reals
{
	using Colosoft.Text.Jep;
	using Colosoft.Text.Jep.Functions;
	using Colosoft.Text.Jep.Parser;
	using Colosoft.Text.Jep.Types;
	using System;
	using System.Collections.Generic;

	public class RealEvaluator : IEvaluator, IJepComponent
	{
		[NonSerialized]
		private Stack<object> stack = new Stack<object>();

		public object Eval(INode node)
		{
			return this.Visit(node);
		}

		public object Evaluate(INode node)
		{
			this.stack.Clear();
			return this.Visit(node);
		}

		private static double FromObject(object o)
		{
			if(o == null)
			{
				throw new EvaluationException("Null value encountered");
			}
			if(o is JepDouble)
			{
				return ((JepDouble)o).DoubleValue;
			}
			if(o is bool)
			{
				if(!((bool)o))
				{
					return 0.0;
				}
				return 1.0;
			}
			if(!(o is double))
			{
				throw new EvaluationException(string.Concat(new object[] {
					"Cannot convert ",
					o.GetType().Name,
					" ",
					o,
					" to a double"
				}));
			}
			return (double)o;
		}

		public IJepComponent GetLightWeightInstance()
		{
			return new RealEvaluator();
		}

		public void Init(JepInstance jep)
		{
		}

		protected double Visit(INode node)
		{
			if(node is ASTConstant)
			{
				return this.VisitConstant((ASTConstant)node);
			}
			if(node is ASTVarNode)
			{
				return this.VisitVariable((ASTVarNode)node);
			}
			if(!(node is ASTFunNode))
			{
				throw new EvaluationException("Bad node type");
			}
			return this.VisitFunction((ASTFunNode)node);
		}

		protected double[] VisitChildren(ASTFunNode node)
		{
			double[] numArray = new double[node.JjtGetNumChildren()];
			for(int i = 0; i < numArray.Length; i++)
			{
				numArray[i] = this.Visit(node.JjtGetChild(i));
			}
			return numArray;
		}

		protected double VisitConstant(ASTConstant node)
		{
			return FromObject(node.Value);
		}

		protected double VisitFunction(ASTFunNode node)
		{
			IPostfixMathCommand pFMC = node.GetPFMC();
			if(pFMC == null)
			{
				throw new EvaluationException("PostfixMathCommand for " + node.GetName() + " not found");
			}
			if(pFMC is ICallbackEvaluation)
			{
				return FromObject(((ICallbackEvaluation)pFMC).Evaluate(node, this));
			}
			switch(node.JjtGetNumChildren())
			{
			case 0:
				if(pFMC is IRealNullaryFunction)
				{
					return ((IRealNullaryFunction)pFMC).Evaluate();
				}
				if(pFMC is IRealNaryFunction)
				{
					return ((IRealNaryFunction)pFMC).Evaluate(new double[0]);
				}
				pFMC.SetCurNumberOfParameters(0);
				pFMC.Run(this.stack);
				return FromObject(this.stack.Pop());
			case 1:
			{
				double val = this.Visit(node.JjtGetChild(0));
				if(pFMC is IRealUnaryFunction)
				{
					return ((IRealUnaryFunction)pFMC).Evaluate(val);
				}
				if(pFMC is IRealNaryFunction)
				{
					return ((IRealNaryFunction)pFMC).Evaluate(new double[] {
						val
					});
				}
				this.stack.Push(new JepDouble(val));
				pFMC.SetCurNumberOfParameters(1);
				pFMC.Run(this.stack);
				return FromObject(this.stack.Pop());
			}
			case 2:
			{
				double l = this.Visit(node.JjtGetChild(0));
				double r = this.Visit(node.JjtGetChild(1));
				if(pFMC is IRealBinaryFunction)
				{
					return ((IRealBinaryFunction)pFMC).Evaluate(l, r);
				}
				if(pFMC is IRealNaryFunction)
				{
					return ((IRealNaryFunction)pFMC).Evaluate(new double[] {
						l,
						r
					});
				}
				this.stack.Push(new JepDouble(l));
				this.stack.Push(new JepDouble(r));
				pFMC.SetCurNumberOfParameters(2);
				pFMC.Run(this.stack);
				return FromObject(this.stack.Pop());
			}
			}
			double[] parameters = this.VisitChildren(node);
			if(pFMC is IRealNaryFunction)
			{
				return ((IRealNaryFunction)pFMC).Evaluate(parameters);
			}
			for(int i = 0; i < parameters.Length; i++)
			{
				this.stack.Push(new JepDouble(parameters[i]));
			}
			pFMC.SetCurNumberOfParameters(parameters.Length);
			pFMC.Run(this.stack);
			return FromObject(this.stack.Pop());
		}

		protected double VisitVariable(ASTVarNode node)
		{
			object o = node.Var.Value;
			if(o == null)
			{
				throw new EvaluationException("Variable " + node.GetName() + " has a null value");
			}
			return FromObject(o);
		}
	}
}
