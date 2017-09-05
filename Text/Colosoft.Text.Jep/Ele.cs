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
	using System.Collections;
	using System.Collections.Generic;

	public class Ele : PostfixMathCommand, ILValue
	{
		private int indexShift;

		public Ele()
		{
			this.indexShift = 1;
			base.numberOfParameters = -1;
		}

		public Ele(bool zeroIndex)
		{
			this.indexShift = 1;
			base.numberOfParameters = -1;
			this.indexShift = zeroIndex ? 0 : 1;
		}

		public override bool CheckNumberOfParameters(int n)
		{
			return (n >= 2);
		}

		public override void Run(Stack<object> s)
		{
			if(base.curNumberOfParameters == 2)
			{
				object obj2 = s.Pop();
				object obj3 = s.Pop();
				if(!(obj3 is ArrayList))
				{
					throw new EvaluationException("Ele: lhs must be an instance of ArrayList");
				}
				if(obj2 is JepDouble)
				{
					int intValue = ((JepDouble)obj2).IntValue;
					object item = ((ArrayList)obj3)[intValue - 1];
					s.Push(item);
				}
				else
				{
					if(!(obj2 is ArrayList))
					{
						throw new EvaluationException("Ele: only single dimension arrays supported in JEP");
					}
					ArrayList list = (ArrayList)obj2;
					if(list.Count != 1)
					{
						throw new EvaluationException("Ele: only single dimension arrays supported in JEP");
					}
					int num2 = ((JepDouble)list[0]).IntValue;
					object obj5 = ((ArrayList)obj3)[num2 - 1];
					s.Push(obj5);
				}
			}
			else
			{
				int num3 = base.curNumberOfParameters - 1;
				int[] numArray = new int[num3];
				for(int i = num3 - 1; i >= 0; i--)
				{
					object obj6 = s.Pop();
					if(!(obj6 is JepDouble))
					{
						throw new EvaluationException("Ele: index " + i + " should be a number ");
					}
					numArray[i] = ((JepDouble)obj6).IntValue;
				}
				object obj7 = s.Pop();
				for(int j = 0; j < num3; j++)
				{
					if(!(obj7 is ArrayList))
					{
						throw new EvaluationException("Ele: lhs must be an instance of Vector");
					}
					ArrayList list2 = (ArrayList)obj7;
					obj7 = list2[numArray[j] - this.indexShift];
				}
				s.Push(obj7);
			}
		}

		public void Set(IEvaluator pv, INode node, object value)
		{
			INode node2 = node.JjtGetChild(0);
			if(!(node2 is ASTVarNode))
			{
				throw new EvaluationException("Ele: lhs must be a variable");
			}
			ASTVarNode node3 = (ASTVarNode)node2;
			Variable var = node3.Var;
			if(node.JjtGetNumChildren() == 2)
			{
				object obj2 = pv.Eval(node.JjtGetChild(1));
				int num = -1;
				if(obj2 is JepDouble)
				{
					num = ((JepDouble)obj2).IntValue - 1;
				}
				else
				{
					if(!(obj2 is ArrayList))
					{
						throw new EvaluationException("Ele: rhs must be a number");
					}
					ArrayList list = (ArrayList)obj2;
					if(list.Count != 1)
					{
						throw new EvaluationException("Ele: only single dimension arrays supported in JEP");
					}
					num = ((JepDouble)list[0]).IntValue - this.indexShift;
				}
				object obj3 = var.Value;
				if(!(obj3 is ArrayList))
				{
					throw new EvaluationException("Ele: the value of the variable must be a Vector");
				}
				ArrayList list2 = new ArrayList((ArrayList)obj3);
				list2[num] = value;
				var.SetValue(list2);
			}
			else
			{
				int[] indicies = new int[node.JjtGetNumChildren() - 1];
				for(int i = 0; i < indicies.Length; i++)
				{
					object obj4 = pv.Eval(node.JjtGetChild(i + 1));
					if(!(obj4 is JepDouble))
					{
						throw new EvaluationException("Ele: index should be integers, it is " + obj4);
					}
					indicies[i] = ((JepDouble)obj4).IntValue - this.indexShift;
				}
				object o = var.Value;
				if(!(o is ArrayList))
				{
					throw new EvaluationException("Ele: the value of the variable must be a ArrayList");
				}
				object obj6 = this.SetEle(o, indicies, 0, value);
				var.SetValue(obj6);
			}
		}

		private object SetEle(object o, int[] indicies, int pos, object value)
		{
			ArrayList list = new ArrayList((ArrayList)o);
			if(pos == (indicies.Length - 1))
			{
				list[indicies[pos]] = value;
				return list;
			}
			object obj2 = this.SetEle(list[indicies[pos]], indicies, pos + 1, value);
			list[indicies[pos]] = obj2;
			return list;
		}
	}
}
