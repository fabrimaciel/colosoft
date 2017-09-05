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

	public class Dot : PostfixMathCommand
	{
		private static Add add = new Add();

		private static Multiply mul = new Multiply();

		public Dot()
		{
			base.numberOfParameters = 2;
		}

		public object DotOp(ArrayList v1, ArrayList v2)
		{
			if(v1.Count != v2.Count)
			{
				throw new EvaluationException("Dot: both sides of dot must be same length");
			}
			int count = v1.Count;
			if(count < 1)
			{
				throw new EvaluationException("Dot: empty ArrayList parsed");
			}
			object obj2 = mul.Mul(v1[0], v2[0]);
			for(int i = 1; i < count; i++)
			{
				obj2 = add.AddOp(obj2, mul.Mul(v1[i], v2[i]));
			}
			return obj2;
		}

		public object DotOp(object param1, object param2)
		{
			if(!(param1 is ArrayList) || !(param2 is ArrayList))
			{
				throw new EvaluationException("Dot: Invalid parameter type, both arguments must be ArrayList");
			}
			return this.DotOp((ArrayList)param1, (ArrayList)param2);
		}

		public override void Run(Stack<object> inStack)
		{
			base.CheckStack(inStack);
			object obj2 = inStack.Pop();
			object obj3 = inStack.Pop();
			inStack.Push(this.DotOp(obj3, obj2));
		}
	}
}
