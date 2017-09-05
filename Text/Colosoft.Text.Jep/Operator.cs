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
	using System;
	using System.Text;

	[Serializable]
	public class Operator
	{
		public const int ASSOCIATIVE = 0x10;

		public const int BINARY = 2;

		private Operator binaryInverseOperator;

		public const int COMMUTATIVE = 0x20;

		public const int COMPOSITE = 0x1000;

		private Operator[] distribOver;

		public const int EQUIVALENCE = 0x1c0;

		public int flags;

		private Operator inverseOperator;

		private int key;

		public const int LEFT = 4;

		private string name;

		public const int NARY = 3;

		public const int NO_ARGS = 0;

		public const int NOT_IN_PARSER = 0x4000;

		private IPostfixMathCommand pfmc;

		private int precedence;

		public const int PREFIX = 0x200;

		public const int REFLEXIVE = 0x40;

		public const int RIGHT = 8;

		private Operator rootOperator;

		public const int SELF_INVERSE = 0x800;

		public const int SUFFIX = 0x400;

		private string symbol;

		public const int SYMMETRIC = 0x80;

		public const int TERNARY = 0x8000;

		public const int TRANSITIVE = 0x100;

		public const int UNARY = 1;

		public const int USE_BINDING_FOR_PRINT = 0x2000;

		public Operator(string name, IPostfixMathCommand pfmc, int flags)
		{
			this.precedence = -1;
			this.distribOver = new Operator[0];
			this.name = name;
			this.pfmc = pfmc;
			this.symbol = name;
			this.flags = flags;
		}

		public Operator(string name, IPostfixMathCommand pfmc, int flags, int precedence) : this(name, pfmc, flags)
		{
			this.precedence = precedence;
		}

		public Operator(string name, string symbol, IPostfixMathCommand pfmc, int flags)
		{
			this.precedence = -1;
			this.distribOver = new Operator[0];
			this.name = name;
			this.pfmc = pfmc;
			this.symbol = symbol;
			this.flags = flags;
		}

		public Operator(string name, string symbol, IPostfixMathCommand pfmc, int flags, int precedence)
		{
			this.precedence = -1;
			this.distribOver = new Operator[0];
			this.name = name;
			this.pfmc = pfmc;
			this.symbol = symbol;
			this.precedence = precedence;
			this.flags = flags;
		}

		public Operator GetBinaryInverseOp()
		{
			return this.binaryInverseOperator;
		}

		public int GetBinding()
		{
			return (this.flags & 12);
		}

		public int GetFlags()
		{
			return this.flags;
		}

		public Operator GetInverseOp()
		{
			return this.inverseOperator;
		}

		public int GetKey()
		{
			return this.key;
		}

		public string GetName()
		{
			return this.name;
		}

		public IPostfixMathCommand GetPFMC()
		{
			return this.pfmc;
		}

		public int GetPrecedence()
		{
			return this.precedence;
		}

		public Operator GetRootOp()
		{
			return this.rootOperator;
		}

		public string GetSymbol()
		{
			return this.symbol;
		}

		public bool IsAssociative()
		{
			return ((this.flags & 0x10) == 0x10);
		}

		public bool IsBinary()
		{
			return ((this.flags & 3) == 2);
		}

		public bool IsCommutative()
		{
			return ((this.flags & 0x20) == 0x20);
		}

		public bool IsComposite()
		{
			return ((this.flags & 0x1000) == 0x1000);
		}

		public bool IsDistributiveOver(Operator op)
		{
			for(int i = 0; i < this.distribOver.Length; i++)
			{
				if(op == this.distribOver[i])
				{
					return true;
				}
			}
			return false;
		}

		public bool IsEquivalence()
		{
			return ((this.flags & 0x1c0) == 0x1c0);
		}

		public bool IsLeftBinding()
		{
			return ((this.flags & 4) == 4);
		}

		public bool IsNary()
		{
			return ((this.flags & 3) == 3);
		}

		public bool IsPrefix()
		{
			return ((this.flags & 0x200) == 0x200);
		}

		public bool IsReflexive()
		{
			return ((this.flags & 0x40) == 0x40);
		}

		public bool IsRightBinding()
		{
			return ((this.flags & 8) == 8);
		}

		public bool IsSelfInverse()
		{
			return ((this.flags & 0x800) == 0x800);
		}

		public bool IsSuffix()
		{
			return ((this.flags & 0x400) == 0x400);
		}

		public bool IsSymmetric()
		{
			return ((this.flags & 0x80) == 0x80);
		}

		public bool IsTernary()
		{
			return ((this.flags & 0x8000) == 0x8000);
		}

		public bool IsTransitive()
		{
			return ((this.flags & 0x100) == 0x100);
		}

		public bool IsUnary()
		{
			return ((this.flags & 3) == 1);
		}

		public bool NotInParser()
		{
			return ((this.flags & 0x4000) == 0x4000);
		}

		public int NumArgs()
		{
			return (this.flags & 3);
		}

		public void SetBinaryInverseOp(Operator inv)
		{
			this.binaryInverseOperator = inv;
		}

		public void SetDistributiveOver(Operator op)
		{
			int length = this.distribOver.Length;
			Operator[] operatorArray = new Operator[length + 1];
			for(int i = 0; i < length; i++)
			{
				operatorArray[i] = this.distribOver[i];
			}
			operatorArray[length] = op;
			this.distribOver = operatorArray;
		}

		public void SetFlag(int flag, bool val)
		{
			if(val)
			{
				this.flags |= flag;
			}
			else
			{
				this.flags &= ~flag;
			}
		}

		public void SetInverseOp(Operator inv)
		{
			this.inverseOperator = inv;
		}

		public void SetKey(int key)
		{
			this.key = key;
		}

		public void SetPFMC(IPostfixMathCommand pfmc)
		{
			this.pfmc = pfmc;
		}

		public virtual void SetPrecedence(int i)
		{
			this.precedence = i;
		}

		public void SetRootOp(Operator root)
		{
			this.rootOperator = root;
		}

		public void SetSymbol(string sym)
		{
			this.symbol = sym;
		}

		public string ToFullString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("Operator: \"" + this.GetSymbol() + "\"");
			if(!this.GetName().Equals(this.GetSymbol()))
			{
				builder.Append(" " + this.GetName());
			}
			switch(this.NumArgs())
			{
			case 0:
				builder.Append(" no arguments,");
				break;
			case 1:
				builder.Append(" unary,");
				break;
			case 2:
				builder.Append(" binary,");
				break;
			case 3:
				builder.Append(" variable number of arguments,");
				break;
			}
			if(this.IsTernary())
			{
				builder.Append(" ternary,");
			}
			else if(this.IsPrefix())
			{
				builder.Append(" Prefix,");
			}
			else if(this.IsSuffix())
			{
				builder.Append(" suffix,");
			}
			else
			{
				builder.Append(" infix,");
			}
			if(this.GetBinding() == 4)
			{
				builder.Append(" left binding,");
			}
			else if(this.GetBinding() == 8)
			{
				builder.Append(" right binding,");
			}
			if(this.IsAssociative())
			{
				builder.Append(" associative,");
			}
			if(this.IsCommutative())
			{
				builder.Append(" commutative,");
			}
			builder.Append(" precedence " + this.GetPrecedence() + ",");
			if(this.IsEquivalence())
			{
				builder.Append(" equivalence relation,");
			}
			else
			{
				if(this.IsReflexive())
				{
					builder.Append(" reflexive,");
				}
				if(this.IsSymmetric())
				{
					builder.Append(" symmetric,");
				}
				if(this.IsTransitive())
				{
					builder.Append(" transitive,");
				}
			}
			builder.Replace(',', '.', builder.Length - 1, 1);
			return builder.ToString();
		}

		public override string ToString()
		{
			return this.ToFullString();
		}

		public bool UseBindingForPrint()
		{
			return ((this.flags & 0x2000) == 0x2000);
		}
	}
}
