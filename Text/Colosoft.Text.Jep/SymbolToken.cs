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
	using System;

	public class SymbolToken : Token
	{
		private bool rhsImpMul;

		public SymbolToken(string source) : base(source)
		{
		}

		public SymbolToken(string source, bool implicitMul) : base(source)
		{
			this.rhsImpMul = implicitMul;
		}

		public Token CloneToken()
		{
			return new SymbolToken(base.source, this.rhsImpMul);
		}

		public override bool IsImplicitMulRhs()
		{
			return this.rhsImpMul;
		}

		public bool IsRhsImpMul()
		{
			return this.rhsImpMul;
		}

		public override bool IsSymbol()
		{
			return true;
		}

		public void SetRhsImpMul(bool rhsImpMul)
		{
			this.rhsImpMul = rhsImpMul;
		}
	}
}
