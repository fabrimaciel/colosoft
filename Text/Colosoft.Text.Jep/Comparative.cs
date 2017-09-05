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
	using Colosoft.Text.Jep.Types;
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class Comparative : PostfixMathCommand
	{
		public const int EQ = 5;

		public const int GE = 3;

		public const int GT = 1;

		protected int id;

		public const int LE = 2;

		public const int LT = 0;

		public const int NE = 4;

		private double tolerance;

		public Comparative(int id_in)
		{
			this.id = id_in;
			base.numberOfParameters = 2;
			this.tolerance = 1E-06;
		}

		public bool Compare(object param1, object param2)
		{
			switch(this.id)
			{
			case 0:
				return this.Lt(param1, param2);
			case 1:
				return this.Gt(param1, param2);
			case 2:
				return this.Le(param1, param2);
			case 3:
				return this.Ge(param1, param2);
			case 4:
				return this.Ne(param1, param2);
			case 5:
				return this.Eq(param1, param2);
			}
			return false;
		}

		public bool Eq(object param1, object param2)
		{
			if((param1 is Complex) && (param2 is Complex))
			{
				return ((Complex)param1).IsEqual((Complex)param2, this.tolerance);
			}
			if((param1 is Complex) && (param2 is JepDouble))
			{
				return ((Complex)param1).IsEqual(new Complex((JepDouble)param2), this.tolerance);
			}
			if((param2 is Complex) && (param1 is JepDouble))
			{
				return ((Complex)param2).IsEqual(new Complex((JepDouble)param1), this.tolerance);
			}
			if((param1 is JepDouble) && (param2 is JepDouble))
			{
				double num = ((JepDouble)param1).Value;
				double num2 = ((JepDouble)param2).Value;
				return (num == num2);
			}
			if((param1 is bool) && (param2 is bool))
			{
				bool flag = (bool)param1;
				bool flag2 = (bool)param2;
				return (flag == flag2);
			}
			if((param1 is JepDouble) && (param2 is bool))
			{
				double num3 = ((JepDouble)param1).Value;
				double num4 = ((bool)param2) ? 1.0 : 0.0;
				return (num3 == num4);
			}
			if((param1 is string) && (param2 is string))
			{
				string str = (string)param1;
				string str2 = (string)param2;
				return str.Equals(str2);
			}
			if((param1 is bool) && (param2 is JepDouble))
			{
				double num5 = ((bool)param1) ? 1.0 : 0.0;
				double num6 = ((JepDouble)param2).Value;
				return (num5 == num6);
			}
			if(!(param1 is ArrayList) || !(param2 is ArrayList))
			{
				return param1.Equals(param2);
			}
			if(((ArrayList)param1).Count != ((ArrayList)param2).Count)
			{
				return false;
			}
			for(int i = 0; i < ((ArrayList)param1).Count; i++)
			{
				if(!this.Eq(((ArrayList)param1)[i], ((ArrayList)param2)[i]))
				{
					return false;
				}
			}
			return true;
		}

		public bool Ge(object param1, object param2)
		{
			if((param1 is Complex) || (param2 is Complex))
			{
				throw new EvaluationException(">= not defined for complex numbers");
			}
			if((param1 is JepDouble) && (param2 is JepDouble))
			{
				double num = ((JepDouble)param1).Value;
				double num2 = ((JepDouble)param2).Value;
				return (num >= num2);
			}
			if(!(param1 is string) || !(param2 is string))
			{
				throw new EvaluationException(">= not defined for objects of type " + param1.GetType().Name + " and " + param2.GetType().Name);
			}
			string str = (string)param1;
			string strB = (string)param2;
			return (str.CompareTo(strB) >= 0);
		}

		public double GetTolerance()
		{
			return this.tolerance;
		}

		public bool Gt(object param1, object param2)
		{
			if((param1 is Complex) || (param2 is Complex))
			{
				throw new EvaluationException("> not defined for complex numbers");
			}
			if((param1 is JepDouble) && (param2 is JepDouble))
			{
				double num = ((JepDouble)param1).Value;
				double num2 = ((JepDouble)param2).Value;
				return (num > num2);
			}
			if(!(param1 is string) || !(param2 is string))
			{
				throw new EvaluationException("> not defined for objects of type " + param1.GetType().Name + " and " + param2.GetType().Name);
			}
			string str = (string)param1;
			string strB = (string)param2;
			return (str.CompareTo(strB) > 0);
		}

		public bool Le(object param1, object param2)
		{
			if((param1 is Complex) || (param2 is Complex))
			{
				throw new EvaluationException("<= not defined for complex numbers");
			}
			if((param1 is JepDouble) && (param2 is JepDouble))
			{
				double num = ((JepDouble)param1).Value;
				double num2 = ((JepDouble)param2).Value;
				return (num <= num2);
			}
			if(!(param1 is string) || !(param2 is string))
			{
				throw new EvaluationException("<= not defined for objects of type " + param1.GetType().Name + " and " + param2.GetType().Name);
			}
			string str = (string)param1;
			string strB = (string)param2;
			return (str.CompareTo(strB) <= 0);
		}

		public bool Lt(object param1, object param2)
		{
			if((param1 is Complex) || (param2 is Complex))
			{
				throw new EvaluationException("< not defined for complex numbers");
			}
			if((param1 is JepDouble) && (param2 is JepDouble))
			{
				double num = ((JepDouble)param1).Value;
				double num2 = ((JepDouble)param2).Value;
				return (num < num2);
			}
			if(!(param1 is string) || !(param2 is string))
			{
				throw new EvaluationException("< not defined for objects of type " + param1.GetType().Name + " and " + param2.GetType().Name);
			}
			string str = (string)param1;
			string strB = (string)param2;
			return (str.CompareTo(strB) < 0);
		}

		public bool Ne(object param1, object param2)
		{
			if((param1 is Complex) && (param2 is Complex))
			{
				return !((Complex)param1).IsEqual((Complex)param2, this.tolerance);
			}
			if((param1 is Complex) && (param2 is JepDouble))
			{
				return !((Complex)param1).IsEqual(new Complex((JepDouble)param2), this.tolerance);
			}
			if((param2 is Complex) && (param1 is JepDouble))
			{
				return !((Complex)param2).IsEqual(new Complex((JepDouble)param1), this.tolerance);
			}
			if((param1 is JepDouble) && (param2 is JepDouble))
			{
				double num = ((JepDouble)param1).Value;
				double num2 = ((JepDouble)param2).Value;
				return (num != num2);
			}
			if((param1 is bool) && (param2 is bool))
			{
				bool flag = (bool)param1;
				bool flag2 = (bool)param2;
				return (flag != flag2);
			}
			if((param1 is string) && (param2 is string))
			{
				string str = (string)param1;
				string str2 = (string)param2;
				return !str.Equals(str2);
			}
			if((param1 is JepDouble) && (param2 is bool))
			{
				double num3 = ((JepDouble)param1).Value;
				double num4 = ((bool)param2) ? 1.0 : 0.0;
				return (num3 != num4);
			}
			if((param1 is bool) && (param2 is JepDouble))
			{
				double num5 = ((bool)param1) ? 1.0 : 0.0;
				double num6 = ((JepDouble)param2).Value;
				return (num5 != num6);
			}
			if(!(param1 is ArrayList) || !(param2 is ArrayList))
			{
				return !param1.Equals(param2);
			}
			if(((ArrayList)param1).Count != ((ArrayList)param2).Count)
			{
				return true;
			}
			for(int i = 0; i < ((ArrayList)param1).Count; i++)
			{
				if(!this.Eq(((ArrayList)param1)[i], ((ArrayList)param2)[i]))
				{
					return true;
				}
			}
			return false;
		}

		public override void Run(Stack<object> inStack)
		{
			base.CheckStack(inStack);
			object obj2 = inStack.Pop();
			object obj3 = inStack.Pop();
			if(this.Compare(obj3, obj2))
			{
				inStack.Push(true);
			}
			else
			{
				inStack.Push(false);
			}
		}

		public void SetTolerance(double d)
		{
			this.tolerance = d;
		}
	}
}
