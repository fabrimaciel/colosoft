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
	using Colosoft.Text.Jep.Reals;
	using Colosoft.Text.Jep.Types;
	using System;
	using System.Collections.Generic;

	public class Floor : PostfixMathCommand, IRealUnaryFunction
	{
		public Floor()
		{
			base.numberOfParameters = 1;
		}

		public double Evaluate(double val)
		{
			return Math.Floor(val);
		}

		public object FloorOp(object param)
		{
			if(!(param is JepDouble))
			{
				throw new EvaluationException("Invalid parameter type");
			}
			return new JepDouble(Math.Floor(((JepDouble)param).DoubleValue));
		}

		public override void Run(Stack<object> inStack)
		{
			base.CheckStack(inStack);
			object param = inStack.Pop();
			inStack.Push(this.FloorOp(param));
		}
	}
}
