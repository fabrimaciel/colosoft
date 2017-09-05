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
	using System.Collections.Generic;
	using System.Text;

	public class OperatorTable : IJepComponent
	{
		protected int _numOps = 0x19;

		public const int OP_ADD = 1;

		public const int OP_AND = 0x10;

		public const int OP_ASSIGN = 0x13;

		public const int OP_CROSS = 0x15;

		public const int OP_DIVIDE = 6;

		public const int OP_DOT = 20;

		public const int OP_ELEMENT = 0x17;

		public const int OP_EQ = 12;

		public const int OP_GE = 14;

		public const int OP_GT = 10;

		public const int OP_LE = 13;

		public const int OP_LIST = 0x16;

		public const int OP_LT = 11;

		public const int OP_MOD = 7;

		public const int OP_MULTIPLY = 5;

		public const int OP_NE = 15;

		public const int OP_NEGATE = 3;

		public const int OP_NOP = 0;

		public const int OP_NOT = 0x12;

		public const int OP_OR = 0x11;

		public const int OP_POWER = 9;

		public const int OP_RANGE = 0x18;

		public const int OP_RECIP = 8;

		public const int OP_SUBTRACT = 2;

		public const int OP_UPLUS = 4;

		protected Operator[] ops;

		public Operator AddOperator(Operator op)
		{
			return this.AddOperator(this.NumOps, op);
		}

		public Operator AddOperator(Operator op, Operator existingOp)
		{
			this.AddOperator(this.NumOps, op);
			op.SetPrecedence(existingOp.GetPrecedence());
			return op;
		}

		public Operator AddOperator(int key, Operator op)
		{
			this.NumOps = key + 1;
			this.ops[key] = op;
			op.SetKey(key);
			return op;
		}

		public Operator AddOperator(int key, Operator op, Operator existingOp)
		{
			this.AddOperator(key, op);
			op.SetPrecedence(existingOp.GetPrecedence());
			return op;
		}

		public Operator AppendOperator(Operator op, Operator existingOp)
		{
			return this.AppendOperator(this.NumOps, op, existingOp);
		}

		public Operator AppendOperator(int key, Operator op, Operator existingOp)
		{
			this.AddOperator(key, op);
			int i = existingOp.GetPrecedence() + 1;
			for(int j = 0; j < this.ops.Length; j++)
			{
				if((this.ops[j] != null) && (this.ops[j].GetPrecedence() >= i))
				{
					this.ops[j].SetPrecedence(this.ops[j].GetPrecedence() + 1);
				}
			}
			op.SetPrecedence(i);
			return op;
		}

		public Operator GetAdd()
		{
			return this.ops[1];
		}

		public Operator GetAnd()
		{
			return this.ops[0x10];
		}

		public Operator GetAssign()
		{
			return this.ops[0x13];
		}

		public Operator GetCross()
		{
			return this.ops[0x15];
		}

		public Operator GetDivide()
		{
			return this.ops[6];
		}

		public Operator GetDot()
		{
			return this.ops[20];
		}

		public Operator GetEle()
		{
			return this.ops[0x17];
		}

		public Operator GetEQ()
		{
			return this.ops[12];
		}

		public Operator GetGE()
		{
			return this.ops[14];
		}

		public Operator GetGT()
		{
			return this.ops[10];
		}

		public Operator GetLE()
		{
			return this.ops[13];
		}

		public IJepComponent GetLightWeightInstance()
		{
			return this;
		}

		public Operator GetList()
		{
			return this.ops[0x16];
		}

		public Operator GetLT()
		{
			return this.ops[11];
		}

		public Operator GetMod()
		{
			return this.ops[7];
		}

		public Operator GetMultiply()
		{
			return this.ops[5];
		}

		public Operator GetNE()
		{
			return this.ops[15];
		}

		public Operator GetNop()
		{
			return this.ops[0];
		}

		public Operator GetNot()
		{
			return this.ops[0x12];
		}

		public Operator GetOperator(int key)
		{
			if(this.ops == null)
			{
				return null;
			}
			if(this.ops.Length <= key)
			{
				return null;
			}
			return this.ops[key];
		}

		public ICollection<Operator> GetOperators()
		{
			List<Operator> list = new List<Operator>();
			if(this.ops != null)
			{
				for(int i = 0; i < this.ops.Length; i++)
				{
					if(this.ops[i] != null)
					{
						list.Add(this.ops[i]);
					}
				}
			}
			return list;
		}

		public Operator GetOperatorsByName(string name)
		{
			if(this.ops != null)
			{
				for(int i = 0; i < this.ops.Length; i++)
				{
					if((this.ops[i] != null) && this.ops[i].GetName().Equals(name))
					{
						return this.ops[i];
					}
				}
			}
			return null;
		}

		public List<Operator> GetOperatorsBySymbol(string symbol)
		{
			List<Operator> list = new List<Operator>();
			if(this.ops != null)
			{
				for(int i = 0; i < this.ops.Length; i++)
				{
					if((this.ops[i] != null) && this.ops[i].GetSymbol().Equals(symbol))
					{
						list.Add(this.ops[i]);
					}
				}
			}
			return list;
		}

		public Operator GetOr()
		{
			return this.ops[0x11];
		}

		public Operator GetPower()
		{
			return this.ops[9];
		}

		public Operator GetRange()
		{
			return this.ops[0x18];
		}

		public Operator GetSubtract()
		{
			return this.ops[2];
		}

		public Operator GetUDivide()
		{
			return this.ops[8];
		}

		public Operator GetUMinus()
		{
			return this.ops[3];
		}

		public Operator GetUPlus()
		{
			return this.ops[4];
		}

		public virtual void Init(JepInstance jep)
		{
		}

		public Operator InsertOperator(Operator op, Operator existingOp)
		{
			return this.InsertOperator(this.NumOps, op, existingOp);
		}

		public Operator InsertOperator(int key, Operator op, Operator existingOp)
		{
			this.AddOperator(key, op);
			int precedence = existingOp.GetPrecedence();
			for(int i = 0; i < this.ops.Length; i++)
			{
				if((this.ops[i] != null) && (this.ops[i].GetPrecedence() >= precedence))
				{
					this.ops[i].SetPrecedence(this.ops[i].GetPrecedence() + 1);
				}
			}
			op.SetPrecedence(precedence);
			return op;
		}

		public void RemoveOperator(Operator op)
		{
			for(int i = 0; i < this._numOps; i++)
			{
				if(this.ops[i] == op)
				{
					this.ops[i] = null;
				}
			}
		}

		public Operator ReplaceOperator(Operator oldOp, Operator op)
		{
			for(int i = 0; i < this._numOps; i++)
			{
				if(this.ops[i] == oldOp)
				{
					this.ops[i] = op;
					op.SetPrecedence(oldOp.GetPrecedence());
				}
			}
			return op;
		}

		protected bool SetBinaryInverseOp(int key1, int key2)
		{
			Operator @operator = this.GetOperator(key1);
			Operator inv = this.GetOperator(key2);
			if((@operator == null) || (inv == null))
			{
				return false;
			}
			@operator.SetBinaryInverseOp(inv);
			return true;
		}

		protected bool SetDistributiveOver(int key1, int key2)
		{
			Operator @operator = this.GetOperator(key1);
			Operator root = this.GetOperator(key2);
			if((@operator == null) || (root == null))
			{
				return false;
			}
			@operator.SetRootOp(root);
			return true;
		}

		protected bool SetInverseOp(int key1, int key2)
		{
			Operator @operator = this.GetOperator(key1);
			Operator inv = this.GetOperator(key2);
			if((@operator == null) || (inv == null))
			{
				return false;
			}
			@operator.SetInverseOp(inv);
			return true;
		}

		public bool SetPrecedenceTable(int[][] precArray)
		{
			bool flag = true;
			for(int i = 0; i < precArray.Length; i++)
			{
				for(int j = 0; j < precArray[i].Length; j++)
				{
					Operator @operator = this.GetOperator(precArray[i][j]);
					if(@operator != null)
					{
						@operator.SetPrecedence(i);
					}
					else
					{
						flag = false;
					}
				}
			}
			return flag;
		}

		protected bool SetRootOp(int key1, int key2)
		{
			Operator @operator = this.GetOperator(key1);
			Operator root = this.GetOperator(key2);
			if((@operator == null) || (root == null))
			{
				return false;
			}
			@operator.SetRootOp(root);
			return true;
		}

		protected void SetStandardOperatorRelations()
		{
			this.SetInverseOp(1, 3);
			this.SetBinaryInverseOp(1, 2);
			this.SetRootOp(2, 1);
			this.SetInverseOp(2, 3);
			this.SetRootOp(3, 1);
			this.SetBinaryInverseOp(3, 2);
			this.SetInverseOp(5, 8);
			this.SetBinaryInverseOp(5, 6);
			this.SetRootOp(6, 5);
			this.SetInverseOp(6, 8);
			this.SetRootOp(8, 5);
			this.SetBinaryInverseOp(8, 6);
			this.SetDistributiveOver(3, 1);
			this.SetDistributiveOver(3, 2);
			this.SetDistributiveOver(5, 1);
			this.SetDistributiveOver(5, 2);
			this.SetDistributiveOver(5, 3);
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			int num = 0x7fffffff;
			int num2 = -2147483648;
			for(int i = 0; i < this._numOps; i++)
			{
				if(this.ops[i] != null)
				{
					int precedence = this.ops[i].GetPrecedence();
					if(precedence < num)
					{
						num = precedence;
					}
					if(precedence > num2)
					{
						num2 = precedence;
					}
				}
			}
			for(int j = num; j <= num2; j++)
			{
				for(int k = 0; k < this._numOps; k++)
				{
					if((this.ops[k] != null) && (this.ops[k].GetPrecedence() == j))
					{
						builder.Append(this.ops[k].ToFullString());
						builder.Append('\n');
					}
				}
			}
			return builder.ToString();
		}

		public int NumOps
		{
			get
			{
				return this._numOps;
			}
			set
			{
				if(this.ops == null)
				{
					this.ops = new Operator[value];
				}
				if(this.ops.Length <= value)
				{
					Operator[] ops = this.ops;
					this.ops = new Operator[value];
					for(int i = 0; i < ops.Length; i++)
					{
						this.ops[i] = ops[i];
					}
				}
				this._numOps = this.ops.Length;
			}
		}
	}
}
