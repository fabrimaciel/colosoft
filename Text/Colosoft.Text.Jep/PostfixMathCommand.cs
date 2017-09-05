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

	[Serializable]
	public abstract class PostfixMathCommand : IPostfixMathCommand
	{
		[NonSerialized]
		protected int curNumberOfParameters;

		protected int numberOfParameters;

		public PostfixMathCommand()
		{
			this.numberOfParameters = 0;
			this.curNumberOfParameters = 0;
		}

		public PostfixMathCommand(int nParam)
		{
			this.numberOfParameters = nParam;
			this.curNumberOfParameters = nParam;
		}

		public virtual bool CheckNumberOfParameters(int n)
		{
			return ((this.numberOfParameters == -1) || (this.numberOfParameters == n));
		}

		protected void CheckStack(Stack<object> inStack)
		{
			if(inStack == null)
			{
				throw new EvaluationException("Stack argument null");
			}
		}

		public int GetNumberOfParameters()
		{
			return this.numberOfParameters;
		}

		public virtual void Run(Stack<object> s)
		{
			throw new EvaluationException("Run() method of PostfixMathCommand called");
		}

		public void SetCurNumberOfParameters(int n)
		{
			this.curNumberOfParameters = n;
		}
	}
}
