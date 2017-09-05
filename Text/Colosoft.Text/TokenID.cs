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
	/// Possíveis identificadores de Token.
	/// </summary>
	public enum TokenID
	{
		/// <summary>
		/// Espaço em branco.
		/// </summary>
		Whitespace = 0,
		/// <summary>
		/// Nova linha.
		/// </summary>
		Newline,
		/// <summary>
		/// Comentário simples
		/// </summary>
		SingleComment,
		/// <summary>
		/// Inicio de MultComentário
		/// </summary>
		BMultiComment,
		/// <summary>
		/// Fim de MultComentário.
		/// </summary>
		EMultiComment,
		/// <summary>
		/// Documento de comentário.
		/// </summary>
		DocComment,
		/// <summary>
		/// Literal do tipo Hexadecimal.
		/// </summary>
		HexLiteral,
		/// <summary>
		/// Literal do tipo inteiro.
		/// </summary>
		IntLiteral,
		/// <summary>
		/// Literal do tipo string
		/// </summary>
		StringLiteral,
		/// <summary>
		/// Literal do tipo decimal.
		/// </summary>
		DecimalLiteral,
		/// <summary>
		/// Literal do tipo de número real.
		/// </summary>
		RealLiteral,
		/// <summary>
		/// Ponto "."
		/// </summary>
		Dot,
		/// <summary>
		/// Aspas
		/// </summary>
		Quote,
		/// <summary>
		/// Sustenito
		/// </summary>
		Hash,
		/// <summary>
		/// Dolar
		/// </summary>
		Dollar,
		/// <summary>
		/// Porcentagem.
		/// </summary>
		Percent,
		/// <summary>
		/// E binário.
		/// </summary>
		BAnd,
		/// <summary>
		/// Aspas simples
		/// </summary>
		SQuote,
		/// <summary>
		/// Estrela.
		/// </summary>
		Star,
		/// <summary>
		/// Soma.
		/// </summary>
		Plus,
		/// <summary>
		/// Virgula.
		/// </summary>
		Comma,
		/// <summary>
		/// Subtração.
		/// </summary>
		Minus,
		/// <summary>
		/// Barra.
		/// </summary>
		Slash,
		/// <summary>
		/// //`
		/// </summary>
		BSQuote,
		/// <summary>
		/// Divisão
		/// </summary>
		Divide,
		/// <summary>
		/// Dois pontos.
		/// </summary>
		Colon,
		/// <summary>
		/// Ponto e virgula.
		/// </summary>
		Semi,
		/// <summary>
		/// Menor.
		/// </summary>
		Less,
		/// <summary>
		/// Igual.
		/// </summary>
		Equal,
		/// <summary>
		/// Maior.
		/// </summary>
		Greater,
		/// <summary>
		/// Pergunta.
		/// </summary>
		Question,
		/// <summary>
		/// Negação.
		/// </summary>
		Not,
		/// <summary>
		/// E.
		/// </summary>
		And,
		/// <summary>
		/// Ou.
		/// </summary>
		Or,
		/// <summary>
		/// ++
		/// </summary>
		PlusPlus,
		/// <summary>
		/// --
		/// </summary>
		MinusMinus,
		/// <summary>
		/// ->
		/// </summary>
		MinusGreater,
		/// <summary>
		/// ==
		/// </summary>
		EqualEqual,
		/// <summary>
		/// !=
		/// </summary>
		NotEqual,
		/// <summary>
		/// Menor igual.
		/// </summary>
		LessEqual,
		/// <summary>
		/// Maior igual.
		/// </summary>
		GreaterEqual,
		/// <summary>
		/// +=
		/// </summary>
		PlusEqual,
		/// <summary>
		/// (
		/// </summary>
		LParen,
		/// <summary>
		/// )
		/// </summary>
		RParen,
		/// <summary>
		/// [
		/// </summary>
		LBracket,
		/// <summary>
		/// ]
		/// </summary>
		RBracket,
		/// <summary>
		/// {
		/// </summary>
		LCurly,
		/// <summary>
		/// }
		/// </summary>
		RCurly,
		/// <summary>
		/// \t
		/// </summary>
		HorizontalTab,
		/// <summary>
		/// /n
		/// </summary>
		NewLine,
		/// <summary>
		/// \r
		/// </summary>
		CarriageReturn,
		/// <summary>
		/// \b
		/// </summary>
		Backspace,
		/// <summary>
		/// \f
		/// </summary>
		Formfeed,
		/// <summary>
		/// \0
		/// </summary>
		NullByte,
		/// <summary>
		/// Tabulação inválida.
		/// </summary>
		InvalidTab,
		/// <summary>
		/// Identificador.
		/// </summary>
		Identifier,
		/// <summary>
		/// Fim.
		/// </summary>
		End,
		/// <summary>
		/// Expressão inválida.
		/// </summary>
		InvalidExpression
	}
}
