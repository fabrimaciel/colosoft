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
	using System;
	using System.Collections.Generic;

	public class Sum : PostfixMathCommand
	{
		private static Add addFun = new Add();

		public Sum()
		{
			base.numberOfParameters = -1;
		}

		public override bool CheckNumberOfParameters(int n)
		{
			return (n > 0);
		}

		public override void Run(Stack<object> stack)
		{
			base.CheckStack(stack);
			if(base.curNumberOfParameters < 1)
			{
				throw new EvaluationException("No arguments for Sum");
			}
			object obj2 = stack.Pop();
			for(int i = 1; i < base.curNumberOfParameters; i++)
			{
				object obj3 = stack.Pop();
				obj2 = addFun.AddOp(obj3, obj2);
			}
			stack.Push(obj2);
		}
	}
}
