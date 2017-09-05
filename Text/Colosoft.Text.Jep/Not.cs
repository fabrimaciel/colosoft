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

	public class Not : PostfixMathCommand
	{
		private bool allowNumbers;

		public Not()
		{
			this.allowNumbers = true;
			base.numberOfParameters = 1;
		}

		public Not(bool disallowNumbers)
		{
			this.allowNumbers = true;
			this.allowNumbers = !disallowNumbers;
		}

		public override void Run(Stack<object> inStack)
		{
			base.CheckStack(inStack);
			object obj2 = inStack.Pop();
			if(this.allowNumbers && (obj2 is JepDouble))
			{
				bool item = ((JepDouble)obj2).DoubleValue == 0.0;
				inStack.Push(item);
			}
			else
			{
				if(!(obj2 is bool))
				{
					throw new EvaluationException("Invalid parameter type");
				}
				inStack.Push(!((bool)obj2));
			}
		}
	}
}
