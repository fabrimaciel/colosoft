﻿/* 
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

	public class Exp : PostfixMathCommand
	{
		public Exp()
		{
			base.numberOfParameters = 1;
		}

		public object ExpOp(object param)
		{
			if(param is Complex)
			{
				Complex complex = (Complex)param;
				double re = complex.Re;
				double im = complex.Im;
				double num3 = Math.Exp(re);
				return new Complex(num3 * Math.Cos(im), num3 * Math.Sin(im));
			}
			if(!(param is JepDouble))
			{
				throw new EvaluationException("Invalid parameter type");
			}
			return new JepDouble(Math.Exp(((JepDouble)param).DoubleValue));
		}

		public override void Run(Stack<object> inStack)
		{
			base.CheckStack(inStack);
			object param = inStack.Pop();
			inStack.Push(this.ExpOp(param));
		}
	}
}
