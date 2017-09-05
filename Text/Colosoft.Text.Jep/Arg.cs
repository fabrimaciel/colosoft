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

	public class Arg : PostfixMathCommand
	{
		private JepDouble PI;

		private JepDouble ZERO;

		public Arg()
		{
			base.numberOfParameters = 1;
			this.PI = new JepDouble(3.1415926535897931);
			this.ZERO = new JepDouble(0.0);
		}

		public JepDouble ArgOp(object param)
		{
			if(param is Complex)
			{
				return new JepDouble(((Complex)param).Arg());
			}
			if(!(param is JepDouble))
			{
				throw new EvaluationException("Invalid parameter type");
			}
			if(((JepDouble)param).DoubleValue < 0.0)
			{
				return this.PI;
			}
			return this.ZERO;
		}

		public override void Run(Stack<object> inStack)
		{
			base.CheckStack(inStack);
			object param = inStack.Pop();
			inStack.Push(this.ArgOp(param));
		}
	}
}
