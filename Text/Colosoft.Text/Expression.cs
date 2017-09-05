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
	/// Representa um expressão de texto.
	/// </summary>
	public class Expression
	{
		/// <summary>
		/// Ponto inicial da expressão.
		/// </summary>
		private int _beginPoint;

		/// <summary>
		/// Ponto final da expressão.
		/// </summary>
		private int _length;

		/// <summary>
		/// Texto da expressão.
		/// </summary>
		private string _text;

		/// <summary>
		/// Linha que a expressão está contida.
		/// </summary>
		private ExpressionLine _line;

		/// <summary>
		/// Container aonde a expressão está contida.
		/// </summary>
		private ExpressionContainer _container;

		/// <summary>
		/// Posição da expressão no container.
		/// </summary>
		private int _containerPosition;

		/// <summary>
		/// Token relacionado a expressão.
		/// </summary>
		private int _token;

		/// <summary>
		/// Container especial que a expressão está contida.
		/// </summary>
		private ContainerChars _currentSpecialContainer;

		/// <summary>
		/// Ponto inicial da expressão.
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
		/// Texto da expressão.
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		/// <summary>
		/// Linha que a expressão está contida.
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
		/// Container aonde a expressão está contida.
		/// </summary>
		public ExpressionContainer Container
		{
			get
			{
				return _container;
			}
			set
			{
				_container = value;
			}
		}

		/// <summary>
		/// Posição da expressão no container.
		/// </summary>
		public int ContainerPosition
		{
			get
			{
				return _containerPosition;
			}
			set
			{
				_containerPosition = value;
			}
		}

		/// <summary>
		/// Token associado com a expressão.
		/// </summary>
		public int Token
		{
			get
			{
				return _token;
			}
			set
			{
				_token = value;
			}
		}

		/// <summary>
		/// Container especial que a expressão está contida.
		/// </summary>
		public ContainerChars CurrentSpecialContainer
		{
			get
			{
				return _currentSpecialContainer;
			}
			set
			{
				_currentSpecialContainer = value;
			}
		}

		/// <summary>
		/// Cria uma nova expressão com o texto informado.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="beginPoint"></param>
		/// <param name="line"></param>
		/// <param name="text"></param>
		public Expression(ExpressionContainer container, int beginPoint, ExpressionLine line, string text)
		{
			_container = container;
			_beginPoint = beginPoint;
			_length = text == null ? 0 : text.Length;
			_line = line;
			_line.Expressions.Add(this);
			_text = text;
			_token = (int)TokenID.Identifier;
		}

		/// <summary>
		/// Cria uma nova expressão e recupera do comando o segmento de texto esperado.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="beginPoint"></param>
		/// <param name="length"></param>
		/// <param name="line"></param>
		/// <param name="command"></param>
		public Expression(ExpressionContainer container, int beginPoint, int length, ExpressionLine line, string command)
		{
			_container = container;
			_beginPoint = beginPoint;
			_length = length;
			_line = line;
			_line.Expressions.Add(this);
			_text = command.Substring(beginPoint, length);
			_token = (int)TokenID.Identifier;
		}

		/// <summary>
		/// Cria uma nova expressão recuperando do comando o segmento de texto e definindo o identificador do token.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="beginPoint"></param>
		/// <param name="length"></param>
		/// <param name="line"></param>
		/// <param name="command"></param>
		/// <param name="tokenID"></param>
		public Expression(ExpressionContainer container, int beginPoint, int length, ExpressionLine line, string command, int tokenID)
		{
			_container = container;
			_beginPoint = beginPoint;
			_length = length;
			_line = line;
			_line.Expressions.Add(this);
			_text = command.Substring(beginPoint, length);
			_token = tokenID;
		}

		/// <summary>
		/// Cria uma nova expressão define um char como texto.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="beginPoint"></param>
		/// <param name="line"></param>
		/// <param name="command"></param>
		public Expression(ExpressionContainer container, int beginPoint, ExpressionLine line, char command)
		{
			_container = container;
			_beginPoint = beginPoint;
			_line = line;
			_line.Expressions.Add(this);
			_length = 1;
			_text = new string(command, 1);
		}

		/// <summary>
		/// Recupera a string que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _text;
		}
	}
}
