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
	/// Representa um container.
	/// </summary>
	public class ExpressionContainer
	{
		private int _id;

		private ContainerChars _containerChars;

		private ExpressionLine _line;

		private int _beginPoint = 0;

		private int _length;

		/// <summary>
		/// Identificador do container.
		/// </summary>
		public int Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		/// <summary>
		/// Caracters do container.
		/// </summary>
		public ContainerChars ContainerChars
		{
			get
			{
				return _containerChars;
			}
			set
			{
				_containerChars = value;
			}
		}

		/// <summary>
		/// Linha onde o container está inserido.
		/// </summary>
		public ExpressionLine Line
		{
			get
			{
				return _line;
			}
			set
			{
				_line = value;
			}
		}

		/// <summary>
		/// Posição inicial do container.
		/// </summary>
		public int BeginPoint
		{
			get
			{
				return _beginPoint;
			}
			set
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
			set
			{
				_length = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="id">Identificador do container.</param>
		/// <param name="beginPoint">Posição inicial do container.</param>
		/// <param name="containerChars">Caracters do container.</param>
		/// <param name="line">Linha onde o container está inserido.</param>
		public ExpressionContainer(int id, int beginPoint, ContainerChars containerChars, ExpressionLine line)
		{
			_id = id;
			_beginPoint = beginPoint;
			_containerChars = containerChars;
			_line = line;
		}
	}
}
