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
	/// Resultado do analizador lexo.
	/// </summary>
	public class LexerResult
	{
		private string _text;

		private List<Expression> _expressions;

		private List<ExpressionLine> _lines;

		private List<ExpressionContainer> _containers;

		/// <summary>
		/// Texto processado.
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
		/// Expressões encontradas.
		/// </summary>
		public List<Expression> Expressions
		{
			get
			{
				return _expressions;
			}
			set
			{
				_expressions = value;
			}
		}

		/// <summary>
		/// Linhas encontradas.
		/// </summary>
		public List<ExpressionLine> Lines
		{
			get
			{
				return _lines;
			}
			set
			{
				_lines = value;
			}
		}

		/// <summary>
		/// Container do resultado.
		/// </summary>
		public List<ExpressionContainer> Containers
		{
			get
			{
				return _containers;
			}
			set
			{
				_containers = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="expressions"></param>
		/// <param name="lines"></param>
		/// <param name="containers">Containers do resultado do analizador.</param>
		public LexerResult(string text, List<Expression> expressions, List<ExpressionLine> lines, List<ExpressionContainer> containers)
		{
			_text = text;
			_expressions = expressions;
			_lines = lines;
			_containers = containers;
		}

		/// <summary>
		/// Compra duas instancias do resultado.
		/// </summary>
		/// <param name="l1"></param>
		/// <param name="l2"></param>
		/// <returns></returns>
		private static bool Equals(LexerResult l1, LexerResult l2)
		{
			if(l1._expressions.Count == l2._expressions.Count)
			{
				for(int i = 0; i < l1._expressions.Count; i++)
				{
					if(l1._expressions[i].Token != l2._expressions[i].Token)
						return false;
				}
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Define estrutura condicional para o operador "=="
		/// </summary>
		/// <param name="l1"></param>
		/// <param name="l2"></param>
		/// <returns></returns>
		public static bool operator ==(LexerResult l1, LexerResult l2)
		{
			return Equals(l1, l2);
		}

		/// <summary>
		/// Define estrutura condicional para o operador "!="
		/// </summary>
		/// <param name="l1"></param>
		/// <param name="l2"></param>
		/// <returns></returns>
		public static bool operator !=(LexerResult l1, LexerResult l2)
		{
			return !Equals(l1, l2);
		}

		/// <summary>
		/// Recupera o código has da instancia.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Compara instancia com outro objeto.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var lexer = obj as Lexer;
			if(ReferenceEquals(lexer, null))
				return false;
			return Equals(this, lexer);
		}
	}
}
