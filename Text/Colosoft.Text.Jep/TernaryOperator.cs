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

namespace Colosoft.Text.Jep.ConfigurableParser
{
	using Colosoft.Text.Jep;
	using System;

	public class TernaryOperator : Operator
	{
		private RhsTernaryOperator rhs;

		private string symbol2;

		public TernaryOperator(string name, string lhsSymbol, string rhsSymbol, IPostfixMathCommand pfmc, int flags) : base(name, lhsSymbol, pfmc, flags)
		{
			this.symbol2 = rhsSymbol;
			this.rhs = new RhsTernaryOperator(name, rhsSymbol, pfmc, flags);
			this.rhs.SetLhsOp(this);
		}

		public TernaryOperator(string name, string lhsSymbol, string rhsSymbol, IPostfixMathCommand pfmc, int flags, int precedence) : base(name, lhsSymbol, pfmc, flags, precedence)
		{
			this.symbol2 = rhsSymbol;
			this.rhs = new RhsTernaryOperator(name, rhsSymbol, pfmc, flags, precedence);
			this.rhs.SetLhsOp(this);
		}

		public RhsTernaryOperator GetRhsOperator()
		{
			return this.rhs;
		}

		public string GetRhsSymbol()
		{
			return this.symbol2;
		}

		public override void SetPrecedence(int i)
		{
			base.SetPrecedence(i);
			this.rhs.SetPrecedence(i);
		}

		public class RhsTernaryOperator : Operator
		{
			private TernaryOperator lhsOp;

			public RhsTernaryOperator(string name, string symbol, IPostfixMathCommand pfmc, int flags) : base(name, symbol, pfmc, flags)
			{
			}

			public RhsTernaryOperator(string name, string symbol, IPostfixMathCommand pfmc, int flags, int precedence) : base(name, symbol, pfmc, flags, precedence)
			{
			}

			public TernaryOperator GetLhsOp()
			{
				return this.lhsOp;
			}

			public void SetLhsOp(TernaryOperator lhsOp)
			{
				this.lhsOp = lhsOp;
			}
		}
	}
}
