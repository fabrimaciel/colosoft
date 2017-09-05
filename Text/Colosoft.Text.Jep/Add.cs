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

	public class Add : PostfixMathCommand, IRealBinaryFunction
	{
		public Add()
		{
			base.numberOfParameters = -1;
		}

		public Complex AddOp(Complex c1, Complex c2)
		{
			return new Complex(c1.Re + c2.Re, c1.Im + c2.Im);
		}

		public Complex AddOp(Complex c, JepDouble d)
		{
			return new Complex(c.Re + d.Value, c.Im);
		}

		public JepDouble AddOp(JepDouble d1, JepDouble d2)
		{
			return new JepDouble(d1.Value + d2.Value);
		}

		public ArrayList AddOp(ArrayList v1, ArrayList v2)
		{
			if(v1.Count != v2.Count)
			{
				throw new EvaluationException(string.Concat(new object[] {
					"Add: dimensions of ArrayLists do not match, ",
					v1.Count,
					", ",
					v2.Count
				}));
			}
			ArrayList list = new ArrayList(v1.Count);
			for(int i = 0; i < v1.Count; i++)
			{
				list.Insert(i, this.AddOp(v1[i], v2[i]));
			}
			return list;
		}

		public object AddOp(object param1, object param2)
		{
			if(param1 is Complex)
			{
				if(param2 is Complex)
				{
					return this.AddOp((Complex)param1, (Complex)param2);
				}
				if(param2 is JepDouble)
				{
					return this.AddOp((Complex)param1, (JepDouble)param2);
				}
			}
			else if(param1 is JepDouble)
			{
				if(param2 is Complex)
				{
					return this.AddOp((Complex)param2, (JepDouble)param1);
				}
				if(param2 is JepDouble)
				{
					return this.AddOp((JepDouble)param1, (JepDouble)param2);
				}
			}
			else
			{
				if((param1 is string) && (param2 is string))
				{
					return (((string)param1) + ((string)param2));
				}
				if((param1 is ArrayList) && (param2 is ArrayList))
				{
					return this.AddOp((ArrayList)param1, (ArrayList)param2);
				}
			}
			throw new EvaluationException("Invalid parameter type");
		}

		public double Evaluate(double l, double r)
		{
			return (l + r);
		}

		public override void Run(Stack<object> stack)
		{
			base.CheckStack(stack);
			object obj2 = stack.Pop();
			for(int i = 1; i < base.curNumberOfParameters; i++)
			{
				object obj3 = stack.Pop();
				obj2 = this.AddOp(obj3, obj2);
			}
			stack.Push(obj2);
		}
	}
}
