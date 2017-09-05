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
	using Colosoft.Text.Jep.Types;
	using System;
	using System.Collections.Generic;

	public class If : PostfixMathCommand, ICallbackEvaluation
	{
		public If()
		{
			base.numberOfParameters = -1;
		}

		public override bool CheckNumberOfParameters(int n)
		{
			if(n != 3)
			{
				return (n == 4);
			}
			return true;
		}

		public object Evaluate(INode node, IEvaluator pv)
		{
			double re;
			int n = node.JjtGetNumChildren();
			if(!this.CheckNumberOfParameters(n))
			{
				throw new EvaluationException("If operator must have 3 or 4 arguments.");
			}
			object obj2 = pv.Eval(node.JjtGetChild(0));
			if(obj2 is bool)
			{
				if((bool)obj2)
				{
					return pv.Eval(node.JjtGetChild(1));
				}
				return pv.Eval(node.JjtGetChild(2));
			}
			if(obj2 is Complex)
			{
				re = ((Complex)obj2).Re;
			}
			else if(obj2 is JepDouble)
			{
				re = ((JepDouble)obj2).DoubleValue;
				if(double.IsNaN(re))
				{
					return re;
				}
			}
			else
			{
				if(!(obj2 is double))
				{
					throw new EvaluationException("Condition in if operator must be Boolean, Number or Complex.");
				}
				re = (double)obj2;
				if(double.IsNaN(re))
				{
					return re;
				}
			}
			if(re > 0.0)
			{
				return pv.Eval(node.JjtGetChild(1));
			}
			if((n != 3) && (re >= 0.0))
			{
				return pv.Eval(node.JjtGetChild(3));
			}
			return pv.Eval(node.JjtGetChild(2));
		}

		public override void Run(Stack<object> aStack)
		{
			throw new EvaluationException("If: Run method should not have been called");
		}
	}
}
