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
	using Colosoft.Text.Jep.Parser;
	using Colosoft.Text.Jep.Types;
	using System;
	using System.Collections.Generic;

	public class LazyLogical : PostfixMathCommand, ICallbackEvaluation
	{
		private bool allowNumbers;

		public const int AND = 0;

		private int id;

		public const int OR = 1;

		public LazyLogical(int id_in)
		{
			this.allowNumbers = true;
			this.id = id_in;
			base.numberOfParameters = 2;
		}

		public LazyLogical(int id_in, bool disallowNumbers)
		{
			this.allowNumbers = true;
			this.id = id_in;
			base.numberOfParameters = 2;
			this.allowNumbers = !disallowNumbers;
		}

		public object Evaluate(INode node, IEvaluator pv)
		{
			bool flag;
			object obj2 = pv.Eval(node.JjtGetChild(0));
			if(obj2 is bool)
			{
				flag = (bool)obj2;
			}
			else if(this.allowNumbers && (obj2 is JepDouble))
			{
				flag = ((JepDouble)obj2).DoubleValue != 0.0;
			}
			else
			{
				if(!this.allowNumbers || !(obj2 is double))
				{
					throw new EvaluationException("LazyLogical: left hand argument does not evaluate to Boolean");
				}
				flag = ((double)obj2) != 0.0;
			}
			switch(this.id)
			{
			case 0:
				if(flag)
				{
					break;
				}
				return false;
			case 1:
				if(!flag)
				{
					break;
				}
				return true;
			}
			object obj3 = pv.Eval(node.JjtGetChild(1));
			if(obj3 is bool)
			{
				return (bool)obj3;
			}
			if(this.allowNumbers && (obj3 is JepDouble))
			{
				return (((JepDouble)obj3).DoubleValue != 0.0);
			}
			if(!this.allowNumbers || !(obj3 is double))
			{
				throw new EvaluationException("LazyLogical: right hand argument does not evaluate to Boolean");
			}
			return (((double)obj3) != 0.0);
		}

		public override void Run(Stack<object> inStack)
		{
			throw new EvaluationException("Logical: run methods should not have been called");
		}
	}
}
