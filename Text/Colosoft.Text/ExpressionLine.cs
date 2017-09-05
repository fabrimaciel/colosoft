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
	/// <summary>
	/// Representa a linha de uma expressão.
	/// </summary>
	public class ExpressionLine
	{
		/// <summary>
		/// Ponto inicial da expressão.
		/// </summary>
		private int _beginPoint;

		/// <summary>
		/// Ponto final da expressão.
		/// </summary>
		private int _length;

		private List<Expression> _expressions = new List<Expression>();

		/// <summary>
		/// Ponto inicial da expressão.
		/// </summary>
		public int BeginPoint
		{
			get
			{
				return _beginPoint;
			}
			internal set
			{
				_beginPoint = value;
			}
		}

		/// <summary>
		/// Ponto final da expressão.
		/// </summary>
		public int Length
		{
			get
			{
				return _length;
			}
			internal set
			{
				_length = value;
			}
		}

		/// <summary>
		/// Expressões contidas na linha
		/// </summary>
		public List<Expression> Expressions
		{
			get
			{
				return _expressions;
			}
		}

		/// <summary>
		/// Cria uma linha de expressão informando a posição inicial.
		/// </summary>
		/// <param name="beginPoint"></param>
		public ExpressionLine(int beginPoint)
		{
			this._beginPoint = beginPoint;
		}

		/// <summary>
		/// Recupera a string que representa o conteúdo da linha.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			foreach (Expression ex in _expressions)
				s.Append(ex.ToString()).Append(" ");
			return s.ToString();
		}
	}
}
