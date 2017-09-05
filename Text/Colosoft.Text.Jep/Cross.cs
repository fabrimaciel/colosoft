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

	public class Cross : PostfixMathCommand
	{
		private static Multiply mul = new Multiply();

		private static Subtract sub = new Subtract();

		public Cross()
		{
			base.numberOfParameters = 2;
		}

		public object CrossOp(ArrayList lhs, ArrayList rhs)
		{
			int count = lhs.Count;
			if(count != rhs.Count)
			{
				throw new EvaluationException("Cross: both sides must be of same length");
			}
			if(count == 3)
			{
				ArrayList list = new ArrayList(3);
				list.Add(sub.SubOp(mul.Mul(lhs[1], rhs[2]), mul.Mul(lhs[2], rhs[1])));
				list.Add(sub.SubOp(mul.Mul(lhs[2], rhs[0]), mul.Mul(lhs[0], rhs[2])));
				list.Add(sub.SubOp(mul.Mul(lhs[0], rhs[1]), mul.Mul(lhs[1], rhs[0])));
				return list;
			}
			if(count != 2)
			{
				throw new EvaluationException("Cross: both sides must be either 2 or 3 dimensions");
			}
			return sub.SubOp(mul.Mul(lhs[0], rhs[1]), mul.Mul(lhs[1], rhs[0]));
		}

		public object CrossOp(object param1, object param2)
		{
			if(!(param1 is ArrayList) || !(param2 is ArrayList))
			{
				throw new EvaluationException("Cross: Invalid parameter type, both arguments must be vectors");
			}
			return this.CrossOp((ArrayList)param1, (ArrayList)param2);
		}

		public override void Run(Stack<object> inStack)
		{
			base.CheckStack(inStack);
			object obj2 = inStack.Pop();
			object obj3 = inStack.Pop();
			inStack.Push(this.CrossOp(obj3, obj2));
		}
	}
}
