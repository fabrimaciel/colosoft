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

namespace Colosoft.Text.Jep.Functions
{
	using Colosoft.Text.Jep;
	using Colosoft.Text.Jep.Parser;
	using System;
	using System.Collections.Generic;

	public class Assign : PostfixMathCommand, ICallbackEvaluation
	{
		public Assign()
		{
			base.numberOfParameters = 2;
		}

		public object Evaluate(INode node, IEvaluator pv)
		{
			if(node.JjtGetNumChildren() != 2)
			{
				throw new EvaluationException("Assignment operator must have 2 operators.");
			}
			object obj2 = pv.Eval(node.JjtGetChild(1));
			INode node2 = node.JjtGetChild(0);
			if(node2 is ASTVarNode)
			{
				ASTVarNode node3 = (ASTVarNode)node2;
				if(!node3.Var.SetValue(obj2))
				{
					throw new EvaluationException("Attempt to set the value of a constant variable");
				}
				return obj2;
			}
			if(!(node2 is ASTFunNode) || !(((ASTFunNode)node2).GetPFMC() is ILValue))
			{
				throw new EvaluationException("Assignment should have a variable or LValue for the lhs.");
			}
			((ILValue)((ASTFunNode)node2).GetPFMC()).Set(pv, node2, obj2);
			return obj2;
		}

		public override void Run(Stack<object> aStack)
		{
			throw new EvaluationException("Assign: run methods should not have been called");
		}
	}
}
