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
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class MinMax : PostfixMathCommand
	{
		private Comparative comp;

		public MinMax(bool isMin)
		{
			this.comp = new Comparative(isMin ? 0 : 1);
			base.numberOfParameters = -1;
		}

		public override bool CheckNumberOfParameters(int n)
		{
			return (n > 0);
		}

		public object MinMaxOp(ArrayList vals)
		{
			int num = 1;
			object obj2 = vals[0];
			while (num < vals.Count)
			{
				if(this.comp.Compare(vals[num], obj2))
				{
					obj2 = vals[num];
				}
				num++;
			}
			return obj2;
		}

		public override void Run(Stack<object> stack)
		{
			base.CheckStack(stack);
			if(base.curNumberOfParameters < 1)
			{
				this.ThrowAtLeastOneExcep();
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
			stack.Push(this.MinMaxOp(vals));
		}

		private void ThrowAtLeastOneExcep()
		{
			throw new EvaluationException("At least one argument is required");
		}
	}
}
