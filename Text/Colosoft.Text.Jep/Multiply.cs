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

	public class Multiply : PostfixMathCommand, IRealBinaryFunction
	{
		public Multiply()
		{
			base.numberOfParameters = -1;
		}

		public override bool CheckNumberOfParameters(int n)
		{
			return (n > 0);
		}

		public double Evaluate(double l, double r)
		{
			return (l * r);
		}

		public Complex Mul(Complex c1, Complex c2)
		{
			return c1.Mul(c2);
		}

		public Complex Mul(Complex c, JepDouble d)
		{
			return c.Mul(d.Value);
		}

		public JepDouble Mul(JepDouble d1, JepDouble d2)
		{
			return new JepDouble(d1.Value * d2.Value);
		}

		public ArrayList Mul(ArrayList v, object d)
		{
			ArrayList list = new ArrayList(v.Count);
			for(int i = 0; i < v.Count; i++)
			{
				list.Add(this.Mul(v[i], d));
			}
			return list;
		}

		public ArrayList Mul(object d, ArrayList v)
		{
			ArrayList list = new ArrayList(v.Count);
			for(int i = 0; i < v.Count; i++)
			{
				list.Add(this.Mul(d, v[i]));
			}
			return list;
		}

		public object Mul(object param1, object param2)
		{
			if(param1 is Complex)
			{
				if(param2 is Complex)
				{
					return this.Mul((Complex)param1, (Complex)param2);
				}
				if(param2 is JepDouble)
				{
					return this.Mul((Complex)param1, (JepDouble)param2);
				}
				if(param2 is ArrayList)
				{
					return this.Mul((Complex)param1, (ArrayList)param2);
				}
			}
			else if(param1 is JepDouble)
			{
				if(param2 is Complex)
				{
					return this.Mul((Complex)param2, (JepDouble)param1);
				}
				if(param2 is JepDouble)
				{
					return this.Mul((JepDouble)param1, (JepDouble)param2);
				}
				if(param2 is ArrayList)
				{
					return this.Mul((JepDouble)param1, (ArrayList)param2);
				}
			}
			else if(param1 is ArrayList)
			{
				return this.Mul((ArrayList)param1, param2);
			}
			throw new EvaluationException("Invalid parameter type");
		}

		public override void Run(Stack<object> stack)
		{
			base.CheckStack(stack);
			object obj2 = stack.Pop();
			for(int i = 1; i < base.curNumberOfParameters; i++)
			{
				object obj3 = stack.Pop();
				obj2 = this.Mul(obj3, obj2);
			}
			stack.Push(obj2);
		}
	}
}
