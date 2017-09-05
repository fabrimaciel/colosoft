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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Text.InterpreterExpression
{
	class SpecialContainerExpression : Expression
	{
		/// <summary>
		/// Caracter container da expressão.
		/// </summary>
		private char _containerChar;

		private int _containerToken;

		/// <summary>
		/// Caracter container da expressão.
		/// </summary>
		public char ContainerChar
		{
			get
			{
				return _containerChar;
			}
		}

		/// <summary>
		/// Token do caracter do container.
		/// </summary>
		public int ContainerToken
		{
			get
			{
				return _containerToken;
			}
			set
			{
				_containerToken = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="container">Container da expressão.</param>
		/// <param name="beginPoint"></param>
		/// <param name="length"></param>
		/// <param name="line"></param>
		/// <param name="command"></param>
		/// <param name="containerChar"></param>
		public SpecialContainerExpression(ExpressionContainer container, int beginPoint, int length, ExpressionLine line, string command, char containerChar) : base(container, beginPoint, length, line, command)
		{
			Token = (int)TokenID.StringLiteral;
			_containerChar = containerChar;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _containerChar + base.Text + _containerChar;
		}
	}
}
