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

	[Serializable]
	public abstract class Token
	{
		private int columnNumber = -1;

		private int lineNumber = -1;

		protected string source;

		protected Token(string source)
		{
			this.source = source;
		}

		public override bool Equals(object arg)
		{
			if((arg == null) || !(arg is Token))
			{
				return false;
			}
			return ((arg == this) || this.GetSource().Equals(((Token)arg).GetSource()));
		}

		public int GetColumnNumber()
		{
			return this.columnNumber;
		}

		public override int GetHashCode()
		{
			return this.GetSource().GetHashCode();
		}

		public int GetLength()
		{
			return this.source.Length;
		}

		public int GetLineNumber()
		{
			return this.lineNumber;
		}

		public string GetSource()
		{
			return this.source;
		}

		public virtual bool IsBinary()
		{
			return false;
		}

		public virtual bool IsComment()
		{
			return false;
		}

		public virtual bool IsFunction()
		{
			return false;
		}

		public virtual bool IsIdentifier()
		{
			return false;
		}

		public virtual bool IsImplicitMulRhs()
		{
			return false;
		}

		public virtual bool IsNumber()
		{
			return false;
		}

		public virtual bool IsOperator()
		{
			return false;
		}

		public virtual bool IsPrefix()
		{
			return false;
		}

		public virtual bool IsString()
		{
			return false;
		}

		public virtual bool IsSuffix()
		{
			return false;
		}

		public virtual bool IsSymbol()
		{
			return false;
		}

		public bool IsTerminal()
		{
			return false;
		}

		public virtual bool IsTernary()
		{
			return false;
		}

		public virtual bool IsWhiteSpace()
		{
			return false;
		}

		public void SetPosition(int line, int column)
		{
			this.lineNumber = line;
			this.columnNumber = column;
		}

		public override string ToString()
		{
			return (base.GetType().Name + ":" + this.source);
		}
	}
}
