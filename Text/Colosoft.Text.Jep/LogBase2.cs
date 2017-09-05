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

	public class LogBase2 : PostfixMathCommand
	{
		private Complex CLOG2;

		private double LOG2;

		public LogBase2()
		{
			base.numberOfParameters = 1;
			this.LOG2 = Math.Log(2.0);
			this.CLOG2 = new Complex(Math.Log(2.0), 0.0);
		}

		public object logbase2(object param)
		{
			if(param is Complex)
			{
				return ((Complex)param).Log().Div(this.CLOG2);
			}
			if(!(param is JepDouble))
			{
				throw new EvaluationException("Invalid parameter type");
			}
			double doubleValue = ((JepDouble)param).DoubleValue;
			if(doubleValue >= 0.0)
			{
				return new JepDouble(Math.Log(doubleValue) / this.LOG2);
			}
			if(double.IsNaN(doubleValue))
			{
				return new JepDouble(double.NaN);
			}
			Complex complex = new Complex(doubleValue);
			return complex.Log().Div(this.CLOG2);
		}

		public override void Run(Stack<object> inStack)
		{
			base.CheckStack(inStack);
			object param = inStack.Pop();
			inStack.Push(this.logbase2(param));
		}
	}
}
