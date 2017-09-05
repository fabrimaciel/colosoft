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
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class List : PostfixMathCommand
	{
		public List()
		{
			base.numberOfParameters = -1;
		}

		public override void Run(Stack<object> inStack)
		{
			ArrayList list;
			base.CheckStack(inStack);
			if(base.curNumberOfParameters > 0)
			{
				list = new ArrayList(base.curNumberOfParameters);
				for(int i = base.curNumberOfParameters - 1; i >= 0; i--)
				{
					object obj2 = inStack.Pop();
					list.Add(obj2);
				}
				list.Reverse();
			}
			else
			{
				list = new ArrayList(0);
			}
			inStack.Push(list);
		}
	}
}
