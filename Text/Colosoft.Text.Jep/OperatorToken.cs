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

namespace Colosoft.Text.Jep.ConfigurableParser.Tokens
{
	using Colosoft.Text.Jep;
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class OperatorToken : Token
	{
		private Operator binaryOp;

		private Operator prefixOp;

		private Operator suffixOp;

		private Operator ternaryOp;

		private OperatorToken(OperatorToken ot) : base(ot.GetSource())
		{
			this.binaryOp = ot.binaryOp;
			this.prefixOp = ot.prefixOp;
			this.suffixOp = ot.suffixOp;
			this.ternaryOp = ot.ternaryOp;
		}

		public OperatorToken(Operator op) : base(op.GetSymbol())
		{
			this.SetOp(op);
		}

		public OperatorToken(List<Operator> ops) : base(ops[0].GetSymbol())
		{
			foreach (Operator @operator in ops)
			{
				this.SetOp(@operator);
			}
		}

		public OperatorToken(Operator[] ops) : base(ops[0].GetSymbol())
		{
			foreach (Operator @operator in ops)
			{
				this.SetOp(@operator);
			}
		}

		public void AddOp(Operator op)
		{
			this.SetOp(op);
		}

		public Token CloneToken()
		{
			return new OperatorToken(this);
		}

		public Operator GetBinaryOp()
		{
			return this.binaryOp;
		}

		public Operator GetPrefixOp()
		{
			return this.prefixOp;
		}

		public Operator GetSuffixOp()
		{
			return this.suffixOp;
		}

		public Operator GetTernaryOp()
		{
			return this.ternaryOp;
		}

		public override bool IsBinary()
		{
			return (this.binaryOp != null);
		}

		public override bool IsOperator()
		{
			return true;
		}

		public override bool IsPrefix()
		{
			return (this.prefixOp != null);
		}

		public override bool IsSuffix()
		{
			return (this.suffixOp != null);
		}

		public override bool IsTernary()
		{
			return (this.ternaryOp != null);
		}

		private void SetOp(Operator op)
		{
			if(op.IsBinary())
			{
				this.binaryOp = op;
			}
			else if(op.IsPrefix())
			{
				this.prefixOp = op;
			}
			else if(op.IsSuffix())
			{
				this.suffixOp = op;
			}
			else if(op.IsTernary())
			{
				this.ternaryOp = op;
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder(base.ToString());
			builder.Append(" (");
			bool flag = false;
			if(this.IsBinary())
			{
				builder.Append(this.binaryOp.GetName());
				flag = true;
			}
			if(this.IsPrefix())
			{
				if(flag)
				{
					builder.Append(',');
				}
				builder.Append(this.prefixOp.GetName() + ',');
				flag = true;
			}
			if(this.IsSuffix())
			{
				if(flag)
				{
					builder.Append(',');
				}
				builder.Append(this.suffixOp.GetName() + ',');
				flag = true;
			}
			if(this.IsTernary())
			{
				if(flag)
				{
					builder.Append(',');
				}
				builder.Append(this.ternaryOp.GetName() + ',');
				flag = true;
			}
			builder.Append(")");
			return builder.ToString();
		}
	}
}
