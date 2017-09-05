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
using Colosoft.Text.InterpreterExpression;

namespace Colosoft.Query.Parser
{
	/// <summary>
	/// Possíveis tipos de expressão.
	/// </summary>
	enum SqlExpressionType : short
	{
		Operation,
		StringLiteral,
		NumericLiteral,
		RealLiteral,
		Column,
		Variable,
		Function,
		Comparation,
		Container,
		Table,
		Select,
		Boolean,
		ComparerScalar,
		Constant
	}
	/// <summary>
	/// Representa um expressão sql.
	/// </summary>
	class SqlExpression
	{
		private Expression _value;

		private SqlExpressionType _type;

		/// <summary>
		/// Valor da expressão.
		/// </summary>
		public Expression Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Tipo da expressão.
		/// </summary>
		public SqlExpressionType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="expression">Expressão relacionadas</param>
		internal SqlExpression(Expression expression)
		{
			switch(expression.Token)
			{
			case (int)TokenID.Identifier:
				if(expression.Text[0] == '?' || expression.Text[0] == '@')
					_type = SqlExpressionType.Variable;
				else
					_type = SqlExpressionType.Column;
				break;
			case (int)TokenID.IntLiteral:
			case (int)TokenID.DecimalLiteral:
			case (int)TokenID.RealLiteral:
				_type = SqlExpressionType.NumericLiteral;
				break;
			case (int)TokenID.StringLiteral:
				_type = SqlExpressionType.StringLiteral;
				break;
			case (int)TokenID.Star:
			case (int)TokenID.Plus:
			case (int)TokenID.Slash:
			case (int)TokenID.Minus:
			case (int)SqlTokenID.kLike:
			case (int)SqlTokenID.kBetween:
				_type = SqlExpressionType.Operation;
				break;
			case (int)TokenID.EqualEqual:
			case (int)TokenID.Equal:
			case (int)TokenID.Greater:
			case (int)TokenID.GreaterEqual:
			case (int)TokenID.Less:
			case (int)TokenID.LessEqual:
			case (int)TokenID.NotEqual:
			case (int)SqlTokenID.kNot:
			case (int)SqlTokenID.kIs:
			case (int)SqlTokenID.kIsNull:
				_type = SqlExpressionType.Comparation;
				break;
			case (int)SqlTokenID.kSelect:
				_type = SqlExpressionType.Select;
				break;
			case (int)SqlTokenID.kAnd:
			case (int)SqlTokenID.kOr:
				_type = SqlExpressionType.Boolean;
				break;
			case (int)SqlTokenID.kIn:
			case (int)SqlTokenID.kAny:
			case (int)SqlTokenID.kSome:
			case (int)SqlTokenID.kAll:
				_type = SqlExpressionType.ComparerScalar;
				break;
			case (int)SqlTokenID.kNull:
			case (int)SqlTokenID.kYear:
			case (int)SqlTokenID.kMonth:
			case (int)SqlTokenID.kDay:
			case (int)SqlTokenID.kHour:
			case (int)SqlTokenID.kMinute:
			case (int)SqlTokenID.kSecond:
				_type = SqlExpressionType.Constant;
				break;
			default:
				_type = SqlExpressionType.Column;
				break;
			}
			_value = expression;
		}

		/// <summary>
		/// Construtor completo.
		/// </summary>
		/// <param name="value">Expressão.</param>
		/// <param name="type">Tipo da expressão.</param>
		public SqlExpression(Expression value, SqlExpressionType type)
		{
			_type = type;
			if(type == SqlExpressionType.Column && value.Length > 0 && (value.Text[0] == '?' || value.Text[0] == '@'))
				_type = SqlExpressionType.Variable;
			_value = value;
		}

		public override string ToString()
		{
			return _value.Text;
		}
	}
}
