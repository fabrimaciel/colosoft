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
	using System.Collections.Generic;

	public class Logical : PostfixMathCommand
	{
		private bool allowNumbers;

		public static readonly int AND = 0;

		private int id;

		public static readonly int OR = 1;

		public Logical(int id_in)
		{
			this.allowNumbers = true;
			this.id = id_in;
			base.numberOfParameters = 2;
		}

		public Logical(int id, bool disallowNumbers)
		{
			this.allowNumbers = true;
			this.id = id;
			this.allowNumbers = !disallowNumbers;
		}

		public override void Run(Stack<object> inStack)
		{
			double doubleValue;
			double num2;
			bool flag;
			base.CheckStack(inStack);
			object obj2 = inStack.Pop();
			object obj3 = inStack.Pop();
			if(this.allowNumbers && (obj3 is JepDouble))
			{
				doubleValue = ((JepDouble)obj3).DoubleValue;
			}
			else
			{
				if(!(obj3 is bool))
				{
					throw new EvaluationException("Logical: require a number or Boolean value on lhs, have " + obj3.GetType().Name);
				}
				doubleValue = ((bool)obj3) ? 1.0 : 0.0;
			}
			if(this.allowNumbers && (obj2 is JepDouble))
			{
				num2 = ((JepDouble)obj2).DoubleValue;
			}
			else
			{
				if(!(obj2 is bool))
				{
					throw new EvaluationException("Logical: require a number or Boolean value on lhs, have " + obj2.GetType().Name);
				}
				num2 = ((bool)obj2) ? 1.0 : 0.0;
			}
			switch(this.id)
			{
			case 0:
				flag = (doubleValue != 0.0) && (num2 != 0.0);
				break;
			case 1:
				flag = (doubleValue != 0.0) || (num2 != 0.0);
				break;
			default:
				flag = false;
				break;
			}
			inStack.Push(flag);
		}
	}
}
