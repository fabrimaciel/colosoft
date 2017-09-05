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
	using Colosoft.Text.Jep.Reals;
	using Colosoft.Text.Jep.Types;
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class Average : PostfixMathCommand, IRealUnaryFunction
	{
		private static Add add = new Add();

		private static Divide div = new Divide();

		public Average()
		{
			base.numberOfParameters = -1;
		}

		public object AverageOp(ArrayList vals)
		{
			int num = 0;
			object obj2 = null;
			while (num < vals.Count)
			{
				if(num == 0)
				{
					obj2 = vals[num];
				}
				else
				{
					obj2 = add.AddOp(obj2, vals[num]);
				}
				num++;
			}
			if(obj2 == null)
			{
				throw new EvaluationException("Can not calculate average of empty array");
			}
			return div.DivOp(obj2, new JepDouble((double)vals.Count));
		}

		public object AverageOp(object param)
		{
			if(param is Complex)
			{
				return param;
			}
			if(param is JepDouble)
			{
				return param;
			}
			if(!(param is ArrayList))
			{
				throw new EvaluationException("Invalid parameter type");
			}
			return this.AverageOp((ArrayList)param);
		}

		public override bool CheckNumberOfParameters(int n)
		{
			return (n > 0);
		}

		public double Evaluate(double val)
		{
			return val;
		}

		public override void Run(Stack<object> stack)
		{
			base.CheckStack(stack);
			if(base.curNumberOfParameters < 1)
			{
				throw new EvaluationException("At least one argument is required");
			}
			ArrayList vals = new ArrayList(base.curNumberOfParameters);
			if((base.curNumberOfParameters == 1) && (stack.Peek() is ArrayList))
			{
				vals.AddRange((ArrayList)stack.Pop());
			}
			else
			{
				for(int i = 0; i < base.curNumberOfParameters; i++)
				{
					vals.Add(stack.Pop());
				}
			}
			if(vals.Count == 0)
			{
				this.ThrowAtLeastOneExcep();
			}
			stack.Push(this.AverageOp(vals));
		}

		private void ThrowAtLeastOneExcep()
		{
			throw new EvaluationException("At least one argument is required");
		}
	}
}
