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

namespace Colosoft.Text.Jep
{
	using Colosoft.Text.Jep.Parser;
	using Colosoft.Text.Jep.Types;
	using System;
	using System.Collections;
	using System.Globalization;
	using System.IO;
	using System.Text;

	public class PrintVisitor : IParserVisitor, IJepComponent
	{
		private int _maxLen = -1;

		public const int COMPLEX_I = 2;

		protected NumberFormatInfo format;

		public const int FULL_BRACKET = 1;

		protected int mode;

		[NonSerialized]
		protected StringBuilder sb;

		private Hashtable specialRules = new Hashtable();

		public void AddSpecialRule(Operator op, IPrintRules rules)
		{
			this.specialRules.Add(op, rules);
		}

		public void Append(string s)
		{
			this.sb.Append(s);
		}

		public virtual string FormatValue(object val)
		{
			StringBuilder builder = new StringBuilder();
			this.FormatValue(val, builder);
			return builder.ToString();
		}

		public virtual void FormatValue(object val, StringBuilder sb1)
		{
			if(this.format != null)
			{
				if(!(val is JepDouble))
				{
					if(val is Complex)
					{
						if((this.mode | 2) == 2)
						{
							sb1.Append(((Complex)val).ToString());
						}
						else
						{
							sb1.Append(((Complex)val).ToString());
						}
					}
					else
					{
						sb1.Append(val);
					}
				}
			}
			else
			{
				sb1.Append(val);
			}
		}

		public virtual IJepComponent GetLightWeightInstance()
		{
			return new PrintVisitor {
				format = this.format,
				_maxLen = this._maxLen,
				mode = this.mode
			};
		}

		public virtual int GetMode()
		{
			return this.mode;
		}

		public virtual bool GetMode(int testmode)
		{
			return ((this.mode & testmode) != 0);
		}

		public void Init(JepInstance jep)
		{
			jep.OpTab.GetOperator(0x16);
			jep.OpTab.GetOperator(0x17);
		}

		public void Print(INode node)
		{
			this.Print(node, Console.Out);
		}

		public void Print(INode node, TextWriter output)
		{
			this.sb = new StringBuilder();
			try
			{
				node.JjtAccept(this, null);
			}
			catch(JepException exception)
			{
				Console.WriteLine("Caught JepException: " + exception.ToString());
			}
			this.PrintWrap(this.sb, output);
		}

		private void PrintBrackets(INode node)
		{
			this.sb.Append("(");
			this.PrintNoBrackets(node);
			this.sb.Append(")");
		}

		public void PrintLine(INode node)
		{
			this.PrintLine(node, Console.Out);
		}

		public void PrintLine(INode node, TextWriter output)
		{
			this.Print(node, output);
			output.WriteLine("");
		}

		private void PrintNoBrackets(INode node)
		{
			node.JjtAccept(this, null);
		}

		public void PrintWrap(StringBuilder sb, TextWriter output)
		{
			if(this._maxLen == -1)
			{
				output.Write(sb);
				return;
			}
			Label_0011:
			if(sb.Length < this._maxLen)
			{
				output.Write(sb);
			}
			else
			{
				int num = this._maxLen - 2;
				for(int i = this._maxLen - 2; i >= 0; i--)
				{
					char c = sb[i];
					if((!char.IsLetterOrDigit(c) && (c != '_')) && (c != '.'))
					{
						num = i;
						break;
					}
				}
				output.WriteLine(sb.ToString(0, num + 1));
				sb.Remove(0, num + 1);
				goto Label_0011;
			}
		}

		public virtual void SetMode(int mode, bool flag)
		{
			this.mode |= mode;
			if(!flag)
			{
				this.mode ^= mode;
			}
		}

		private bool TestLeft(Operator top, INode lhs)
		{
			if((this.mode & 1) == 0)
			{
				if(!(lhs is ASTOpNode))
				{
					return false;
				}
				Operator @operator = ((ASTOpNode)lhs).GetOperator();
				if(top == @operator)
				{
					if(top.IsLeftBinding() && top.IsAssociative())
					{
						return false;
					}
					if(top.UseBindingForPrint())
					{
						return false;
					}
					return true;
				}
				if(top.GetPrecedence() == @operator.GetPrecedence())
				{
					if(@operator.IsLeftBinding() && @operator.IsAssociative())
					{
						return false;
					}
					if(@operator.UseBindingForPrint())
					{
						return false;
					}
					return true;
				}
				if(top.GetPrecedence() > @operator.GetPrecedence())
				{
					return false;
				}
			}
			return true;
		}

		private bool TestMid(Operator top, INode rhs)
		{
			if((this.mode & 1) == 0)
			{
				if(!(rhs is ASTOpNode))
				{
					return false;
				}
				Operator @operator = ((ASTOpNode)rhs).GetOperator();
				if(top == @operator)
				{
					return false;
				}
				if(top.GetPrecedence() == @operator.GetPrecedence())
				{
					return false;
				}
				if(top.GetPrecedence() > @operator.GetPrecedence())
				{
					return false;
				}
			}
			return true;
		}

		private bool TestRight(Operator top, INode rhs)
		{
			if((this.mode & 1) == 0)
			{
				if(!(rhs is ASTOpNode))
				{
					return false;
				}
				Operator @operator = ((ASTOpNode)rhs).GetOperator();
				if(top == @operator)
				{
					return (!top.IsRightBinding() && !top.IsAssociative());
				}
				if(top.GetPrecedence() == @operator.GetPrecedence())
				{
					if(top.IsLeftBinding() && top.IsAssociative())
					{
						return false;
					}
					return true;
				}
				if(top.GetPrecedence() > @operator.GetPrecedence())
				{
					return false;
				}
			}
			return true;
		}

		public string ToString(INode node)
		{
			this.sb = new StringBuilder();
			try
			{
				node.JjtAccept(this, null);
			}
			catch(JepException exception)
			{
				Console.WriteLine("Caught JepException: " + exception.ToString());
			}
			return this.sb.ToString();
		}

		public virtual object Visit(ASTConstant node, object data)
		{
			object val = node.Value;
			this.FormatValue(val, this.sb);
			return data;
		}

		public virtual object Visit(ASTFunNode node, object data)
		{
			this.sb.Append(node.GetName() + "(");
			for(int i = 0; i < node.JjtGetNumChildren(); i++)
			{
				if(i > 0)
				{
					this.sb.Append(",");
				}
				node.JjtGetChild(i).JjtAccept(this, null);
			}
			this.sb.Append(")");
			return null;
		}

		public virtual object Visit(ASTOpNode node, object data)
		{
			if(node is IPrintRules)
			{
				((IPrintRules)node).Append(node, this);
				return null;
			}
			if(node.GetOperator() == null)
			{
				throw new JepException("Null operator in Print for " + node);
			}
			if(this.specialRules.ContainsKey(node.GetOperator()))
			{
				((IPrintRules)this.specialRules[node.GetOperator()]).Append(node, this);
				return null;
			}
			if(node.GetOperator().IsUnary())
			{
				return this.VisitUnary(node, data);
			}
			if(node.GetOperator().IsBinary())
			{
				Operator op = node.GetOperator();
				if(node.JjtGetNumChildren() != 2)
				{
					return this.VisitNaryBinary(node, op);
				}
				INode lhs = node.JjtGetChild(0);
				INode rhs = node.JjtGetChild(1);
				if(this.TestLeft(op, lhs))
				{
					this.PrintBrackets(lhs);
				}
				else
				{
					this.PrintNoBrackets(lhs);
				}
				this.sb.Append(node.GetOperator().GetSymbol());
				if(this.TestRight(op, rhs))
				{
					this.PrintBrackets(rhs);
				}
				else
				{
					this.PrintNoBrackets(rhs);
				}
			}
			return null;
		}

		public virtual object Visit(ASTVarNode node, object data)
		{
			this.sb.Append(node.GetName());
			return data;
		}

		private object VisitNaryBinary(ASTFunNode node, Operator op)
		{
			int num = node.JjtGetNumChildren();
			for(int i = 0; i < num; i++)
			{
				if(i > 0)
				{
					this.sb.Append(op.GetSymbol());
				}
				INode rhs = node.JjtGetChild(i);
				if(this.TestMid(op, rhs))
				{
					this.PrintBrackets(rhs);
				}
				else
				{
					this.PrintNoBrackets(rhs);
				}
			}
			return null;
		}

		private object VisitUnary(ASTOpNode node, object data)
		{
			INode node2 = node.JjtGetChild(0);
			if(node.GetOperator().IsPrefix())
			{
				this.sb.Append(node.GetOperator().GetSymbol());
			}
			if(node2 is ASTOpNode)
			{
				this.PrintBrackets(node2);
			}
			else
			{
				this.PrintNoBrackets(node2);
			}
			if(node.GetOperator().IsSuffix())
			{
				this.sb.Append(node.GetOperator().GetSymbol());
			}
			return data;
		}

		public int MaxLen
		{
			get
			{
				return this._maxLen;
			}
			set
			{
				this._maxLen = value;
			}
		}

		public interface IPrintRules
		{
			void Append(INode node, PrintVisitor pv);
		}
	}
}
