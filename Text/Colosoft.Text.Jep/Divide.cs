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

	public class Divide : PostfixMathCommand, IRealBinaryFunction
	{
		public Divide()
		{
			base.numberOfParameters = 2;
		}

		public Complex DivOp(Complex c1, Complex c2)
		{
			return c1.Div(c2);
		}

		public Complex DivOp(Complex c, JepDouble d)
		{
			return new Complex(c.Re / d.DoubleValue, c.Im / d.DoubleValue);
		}

		public Complex DivOp(JepDouble d, Complex c)
		{
			Complex complex = new Complex(d.DoubleValue, 0.0);
			return complex.Div(c);
		}

		public JepDouble DivOp(JepDouble d1, JepDouble d2)
		{
			return new JepDouble(d1.DoubleValue / d2.DoubleValue);
		}

		public ArrayList DivOp(ArrayList v, object d)
		{
			ArrayList list = new ArrayList(v.Count);
			for(int i = 0; i < v.Count; i++)
			{
				list.Add(this.DivOp(v[i], d));
			}
			return list;
		}

		public object DivOp(object param1, object param2)
		{
			if(param1 is Complex)
			{
				if(param2 is Complex)
				{
					return this.DivOp((Complex)param1, (Complex)param2);
				}
				if(param2 is JepDouble)
				{
					return this.DivOp((Complex)param1, (JepDouble)param2);
				}
			}
			else if(param1 is JepDouble)
			{
				if(param2 is Complex)
				{
					return this.DivOp((JepDouble)param1, (Complex)param2);
				}
				if(param2 is JepDouble)
				{
					return this.DivOp((JepDouble)param1, (JepDouble)param2);
				}
			}
			else if(param1 is ArrayList)
			{
				return this.DivOp((ArrayList)param1, param2);
			}
			throw new EvaluationException("Invalid parameter type");
		}

		public double Evaluate(double l, double r)
		{
			return (l / r);
		}

		public override void Run(Stack<object> inStack)
		{
			base.CheckStack(inStack);
			object obj2 = inStack.Pop();
			object obj3 = inStack.Pop();
			inStack.Push(this.DivOp(obj3, obj2));
		}
	}
}
