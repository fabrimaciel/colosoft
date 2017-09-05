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

	public class ArcCosineH : PostfixMathCommand, IRealUnaryFunction
	{
		public ArcCosineH()
		{
			base.numberOfParameters = 1;
		}

		public object AcoshOp(object param)
		{
			if(param is Complex)
			{
				return ((Complex)param).Acosh();
			}
			if(!(param is JepDouble))
			{
				throw new EvaluationException("Invalid parameter type");
			}
			double doubleValue = ((JepDouble)param).DoubleValue;
			if(doubleValue >= 1.0)
			{
				return new JepDouble(Math.Log(doubleValue + Math.Sqrt((doubleValue * doubleValue) - 1.0)));
			}
			Complex complex = new Complex(((JepDouble)param).DoubleValue, 0.0);
			return complex.Acosh();
		}

		public double Evaluate(double val)
		{
			if(val >= 1.0)
			{
				return Math.Log(val + Math.Sqrt((val * val) - 1.0));
			}
			return double.NaN;
		}

		public override void Run(Stack<object> inStack)
		{
			base.CheckStack(inStack);
			object param = inStack.Pop();
			inStack.Push(this.AcoshOp(param));
		}
	}
}
