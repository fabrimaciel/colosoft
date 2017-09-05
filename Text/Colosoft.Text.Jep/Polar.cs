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

	public class Polar : PostfixMathCommand
	{
		public Polar()
		{
			base.numberOfParameters = 2;
		}

		public override void Run(Stack<object> inStack)
		{
			base.CheckStack(inStack);
			object obj2 = inStack.Pop();
			object obj3 = inStack.Pop();
			if(!(obj3 is JepDouble) || !(obj2 is JepDouble))
			{
				throw new EvaluationException("Complex: Invalid parameter types " + obj3.GetType().Name + " " + obj2.GetType().Name);
			}
			inStack.Push(Complex.PolarValueOf((JepDouble)obj3, (JepDouble)obj2));
		}
	}
}
