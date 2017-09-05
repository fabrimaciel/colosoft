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

	public class ParseException : JepException
	{
		private int columnNumber;

		public ParseException jccpe;

		private int lineNumber;

		public ParseException()
		{
			this.lineNumber = -1;
			this.columnNumber = -1;
		}

		public ParseException(string message) : base(message)
		{
			this.lineNumber = -1;
			this.columnNumber = -1;
		}

		public ParseException(string message, Exception cause) : base(message, cause)
		{
			this.lineNumber = -1;
			this.columnNumber = -1;
			if(cause is ParseException)
			{
				this.ProcessJCCPE((ParseException)cause);
			}
		}

		public ParseException(string message, int lineNumber, int columnNumber) : base(message)
		{
			this.lineNumber = -1;
			this.columnNumber = -1;
			this.lineNumber = lineNumber;
			this.columnNumber = columnNumber;
		}

		public int GetColumnNumber()
		{
			return this.columnNumber;
		}

		public int GetLineNumber()
		{
			return this.lineNumber;
		}

		private void ProcessJCCPE(ParseException jccpe)
		{
			this.jccpe = jccpe;
		}

		public void SetPosition(int lineNum, int colNum)
		{
			this.lineNumber = lineNum;
			this.columnNumber = colNum;
		}

		public override string Message
		{
			get
			{
				if(this.jccpe != null)
				{
					return this.jccpe.Message;
				}
				if((this.lineNumber != -1) && (this.columnNumber != 1))
				{
					return string.Concat(new object[] {
						"Line ",
						this.lineNumber,
						" column ",
						this.columnNumber,
						": ",
						base.Message
					});
				}
				return base.Message;
			}
		}
	}
}
