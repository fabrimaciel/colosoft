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
	using System.Collections.Generic;

	public class Power : PostfixMathCommand, IRealBinaryFunction
	{
		private bool allowComplexResults = true;

		public Power()
		{
			base.numberOfParameters = 2;
		}

		public double Evaluate(double l, double r)
		{
			int n = (int)r;
			if(r != n)
			{
				return Math.Pow(l, r);
			}
			if(n >= 0)
			{
				return this.PowerOp(l, n);
			}
			return (1.0 / this.PowerOp(l, -n));
		}

		public bool IsAllowComplexResults()
		{
			return this.allowComplexResults;
		}

		public object PowerOp(Complex c1, Complex c2)
		{
			Complex complex = c1.Pow(c2);
			if(complex.Im == 0.0)
			{
				return new JepDouble(complex.Re);
			}
			return complex;
		}

		public object PowerOp(Complex c, JepDouble d)
		{
			Complex complex = c.Pow(d.DoubleValue);
			if(complex.Im == 0.0)
			{
				return new JepDouble(complex.Re);
			}
			return complex;
		}

		public object PowerOp(JepDouble d, Complex c)
		{
			Complex complex2 = new Complex(d.DoubleValue, 0.0).Pow(c);
			if(complex2.Im == 0.0)
			{
				return new JepDouble(complex2.Re);
			}
			return complex2;
		}

		public object PowerOp(JepDouble d1, JepDouble d2)
		{
			int shortValue = d2.ShortValue;
			double doubleValue = d2.DoubleValue;
			if((d1.DoubleValue < 0.0) && (doubleValue != shortValue))
			{
				if(!this.allowComplexResults)
				{
					throw new EvaluationException("Cannot evaluate " + d1.ToString() + "^" + d2.ToString() + " result would be complex");
				}
				Complex complex = new Complex(d1.DoubleValue, 0.0);
				return complex.Pow(d2.DoubleValue);
			}
			if(doubleValue != shortValue)
			{
				return new JepDouble(Math.Pow(d1.DoubleValue, d2.DoubleValue));
			}
			if(doubleValue >= 0.0)
			{
				return new JepDouble(this.PowerOp(d1.DoubleValue, shortValue));
			}
			return new JepDouble(1.0 / this.PowerOp(d1.DoubleValue, -shortValue));
		}

		public double PowerOp(double l, int n)
		{
			double num4;
			double num = l;
			switch(n)
			{
			case 0:
				return 1.0;
			case 1:
				return num;
			case 2:
				return (num * num);
			case 3:
				return (num * (num * num));
			case 4:
				return (num * ((num * num) * num));
			case 5:
				return (num * (((num * num) * num) * num));
			case 6:
				return (num * ((((num * num) * num) * num) * num));
			case 7:
				return (num * (((((num * num) * num) * num) * num) * num));
			case 8:
				return (num * ((((((num * num) * num) * num) * num) * num) * num));
			}
			int num2 = n;
			double num3 = num;
			if((num2 & 1) != 0)
			{
				num4 = num;
			}
			else
			{
				num4 = 1.0;
			}
			for(num2 = num2 >> 1; num2 != 0; num2 = num2 >> 1)
			{
				num3 *= num3;
				if((num2 & 1) != 0)
				{
					num4 *= num3;
				}
			}
			return num4;
		}

		public object PowerOp(object param1, object param2)
		{
			if(param1 is Complex)
			{
				if(param2 is Complex)
				{
					return this.PowerOp((Complex)param1, (Complex)param2);
				}
				if(param2 is JepDouble)
				{
					return this.PowerOp((Complex)param1, (JepDouble)param2);
				}
			}
			else if(param1 is JepDouble)
			{
				if(param2 is Complex)
				{
					return this.PowerOp((JepDouble)param1, (Complex)param2);
				}
				if(param2 is JepDouble)
				{
					return this.PowerOp((JepDouble)param1, (JepDouble)param2);
				}
			}
			throw new EvaluationException("Invalid parameter type");
		}

		public override void Run(Stack<object> inStack)
		{
			base.CheckStack(inStack);
			object obj2 = inStack.Pop();
			object obj3 = inStack.Pop();
			inStack.Push(this.PowerOp(obj3, obj2));
		}

		public void SetAllowComplexResults(bool allowComplexResults)
		{
			this.allowComplexResults = allowComplexResults;
		}
	}
}
