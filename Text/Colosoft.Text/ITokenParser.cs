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
	/// Assinatura das classes responsáveis por fazer um parser do token.
	/// </summary>
	public interface ITokenParser
	{
		/// <summary>
		/// Recupera o token que o caracter representa.
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		int Parse(char character);

		/// <summary>
		/// Recupera o token que o termo representa. 
		/// </summary>
		/// <param name="term"></param>
		/// <returns></returns>
		int Parse(string term);

		/// <summary>
		/// Recupera o termo com base no token.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		string GetTerm(int token);

		/// <summary>
		/// Recupera o caracter do token.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		char GetCharacter(int token);
	}
	/// <summary>
	/// Implementação padrão do Parser dos tokens.
	/// </summary>
	public class DefaultTokenParser : ITokenParser
	{
		/// <summary>
		/// Recupera o identificador do token.
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		protected virtual TokenID ParseTokenID(char character)
		{
			switch(character)
			{
			case ' ':
				return TokenID.Whitespace;
			case '.':
				return TokenID.Dot;
			case '"':
				return TokenID.Quote;
			case '#':
				return TokenID.Hash;
			case '$':
				return TokenID.Dollar;
			case '%':
				return TokenID.Percent;
			case '&':
				return TokenID.BAnd;
			case '\'':
				return TokenID.SQuote;
			case '*':
				return TokenID.Star;
			case '+':
				return TokenID.Plus;
			case ',':
				return TokenID.Comma;
			case '-':
				return TokenID.Minus;
			case '/':
				return TokenID.Slash;
			case '`':
				return TokenID.BSQuote;
			case '\\':
				return TokenID.Divide;
			case ':':
				return TokenID.Colon;
			case ';':
				return TokenID.Semi;
			case '<':
				return TokenID.Less;
			case '=':
				return TokenID.Equal;
			case '>':
				return TokenID.Greater;
			case '?':
				return TokenID.Question;
			case '!':
				return TokenID.Not;
			case '(':
				return TokenID.LParen;
			case ')':
				return TokenID.RParen;
			case '[':
				return TokenID.LBracket;
			case ']':
				return TokenID.RBracket;
			case '{':
				return TokenID.LCurly;
			case '}':
				return TokenID.RCurly;
			case '\r':
				return TokenID.CarriageReturn;
			case '\t':
				return TokenID.HorizontalTab;
			case '\0':
				return TokenID.NullByte;
			case '\n':
				return TokenID.Newline;
			case '\f':
				return TokenID.Formfeed;
			default:
				return TokenID.InvalidExpression;
			}
		}

		/// <summary>
		/// Recupera o identificador do token.
		/// </summary>
		/// <param name="term"></param>
		/// <returns></returns>
		protected virtual TokenID ParseTokenID(string term)
		{
			switch(term)
			{
			case " ":
				return TokenID.Whitespace;
			case ".":
				return TokenID.Dot;
			case "\"":
				return TokenID.Quote;
			case "#":
				return TokenID.Hash;
			case "$":
				return TokenID.Dollar;
			case "%":
				return TokenID.Percent;
			case "&":
				return TokenID.BAnd;
			case "'":
				return TokenID.SQuote;
			case "*":
				return TokenID.Star;
			case "+":
				return TokenID.Plus;
			case ",":
				return TokenID.Comma;
			case "-":
				return TokenID.Minus;
			case "/":
				return TokenID.Slash;
			case "`":
				return TokenID.BSQuote;
			case "\\":
				return TokenID.Divide;
			case ":":
				return TokenID.Colon;
			case ";":
				return TokenID.Semi;
			case "<":
				return TokenID.Less;
			case "=":
				return TokenID.Equal;
			case ">":
				return TokenID.Greater;
			case "?":
				return TokenID.Question;
			case "!":
				return TokenID.Not;
			case "(":
				return TokenID.LParen;
			case ")":
				return TokenID.RParen;
			case "[":
				return TokenID.LBracket;
			case "]":
				return TokenID.RBracket;
			case "{":
				return TokenID.LCurly;
			case "}":
				return TokenID.RCurly;
			case "&&":
				return TokenID.And;
			case "||":
				return TokenID.Or;
			case "++":
				return TokenID.PlusPlus;
			case "--":
				return TokenID.MinusMinus;
			case "->":
				return TokenID.MinusGreater;
			case "==":
				return TokenID.EqualEqual;
			case "!=":
				return TokenID.NotEqual;
			case "<=":
				return TokenID.LessEqual;
			case ">=":
				return TokenID.GreaterEqual;
			case "+=":
				return TokenID.PlusEqual;
			case "!>":
				return TokenID.Less;
			case "!<":
				return TokenID.Greater;
			case "\r":
				return TokenID.CarriageReturn;
			case "\t":
				return TokenID.HorizontalTab;
			case "\0":
				return TokenID.NullByte;
			case "\n":
				return TokenID.Newline;
			case "\f":
				return TokenID.Formfeed;
			default:
				return TokenID.InvalidExpression;
			}
		}

		/// <summary>
		/// Recupera o termo com base no token.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		protected virtual string GetTerm(TokenID token)
		{
			switch(token)
			{
			case TokenID.Whitespace:
				return " ";
			case TokenID.Dot:
				return ".";
			case TokenID.Quote:
				return "\"";
			case TokenID.Hash:
				return "#";
			case TokenID.Dollar:
				return "$";
			case TokenID.Percent:
				return "%";
			case TokenID.BAnd:
				return "&";
			case TokenID.SQuote:
				return "'";
			case TokenID.Star:
				return "*";
			case TokenID.Plus:
				return "+";
			case TokenID.Comma:
				return ",";
			case TokenID.Minus:
				return "-";
			case TokenID.Slash:
				return "/";
			case TokenID.BSQuote:
				return "`";
			case TokenID.Divide:
				return "\\";
			case TokenID.Colon:
				return ":";
			case TokenID.Semi:
				return ";";
			case TokenID.Less:
				return "<";
			case TokenID.Equal:
				return "=";
			case TokenID.Greater:
				return ">";
			case TokenID.Question:
				return "?";
			case TokenID.Not:
				return "!";
			case TokenID.LParen:
				return "(";
			case TokenID.RParen:
				return ")";
			case TokenID.LBracket:
				return "[";
			case TokenID.RBracket:
				return "]";
			case TokenID.LCurly:
				return "{";
			case TokenID.RCurly:
				return "}";
			case TokenID.And:
				return "&&";
			case TokenID.Or:
				return "||";
			case TokenID.PlusPlus:
				return "++";
			case TokenID.MinusMinus:
				return "--";
			case TokenID.MinusGreater:
				return "->";
			case TokenID.EqualEqual:
				return "==";
			case TokenID.NotEqual:
				return "!=";
			case TokenID.LessEqual:
				return "<=";
			case TokenID.GreaterEqual:
				return ">=";
			case TokenID.PlusEqual:
				return "+=";
			case TokenID.CarriageReturn:
				return "\r";
			case TokenID.HorizontalTab:
				return "\t";
			case TokenID.NullByte:
				return "\0";
			case TokenID.Newline:
				return "\n";
			case TokenID.Formfeed:
				return "\f";
			}
			return null;
		}

		/// <summary>
		/// Recupera o caracter do token.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="result">Caractere associado com o token.</param>
		/// <returns>True caso o valor tenha sido encontrado.</returns>
		public virtual bool GetCharacter(TokenID token, out char result)
		{
			switch(token)
			{
			case TokenID.Whitespace:
				result = ' ';
				break;
			case TokenID.Dot:
				result = '.';
				break;
			case TokenID.Quote:
				result = '"';
				break;
			case TokenID.Hash:
				result = '#';
				break;
			case TokenID.Dollar:
				result = '$';
				break;
			case TokenID.Percent:
				result = '%';
				break;
			case TokenID.BAnd:
				result = '&';
				break;
			case TokenID.SQuote:
				result = '\'';
				break;
			case TokenID.Star:
				result = '*';
				break;
			case TokenID.Plus:
				result = '+';
				break;
			case TokenID.Comma:
				result = ',';
				break;
			case TokenID.Minus:
				result = '-';
				break;
			case TokenID.Slash:
				result = '/';
				break;
			case TokenID.BSQuote:
				result = '`';
				break;
			case TokenID.Divide:
				result = '\\';
				break;
			case TokenID.Colon:
				result = ':';
				break;
			case TokenID.Semi:
				result = ';';
				break;
			case TokenID.Less:
				result = '<';
				break;
			case TokenID.Equal:
				result = '=';
				break;
			case TokenID.Greater:
				result = '>';
				break;
			case TokenID.Question:
				result = '?';
				break;
			case TokenID.Not:
				result = '!';
				break;
			case TokenID.LParen:
				result = '(';
				break;
			case TokenID.RParen:
				result = ')';
				break;
			case TokenID.LBracket:
				result = '[';
				break;
			case TokenID.RBracket:
				result = ']';
				break;
			case TokenID.LCurly:
				result = '{';
				break;
			case TokenID.RCurly:
				result = '}';
				break;
			case TokenID.CarriageReturn:
				result = '\r';
				break;
			case TokenID.HorizontalTab:
				result = '\t';
				break;
			case TokenID.NullByte:
				result = '\0';
				break;
			case TokenID.Newline:
				result = '\n';
				break;
			case TokenID.Formfeed:
				result = '\f';
				break;
			default:
				result = '\0';
				return false;
			}
			return true;
		}

		/// <summary>
		/// Recupera o token que o caracter representa.
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		public virtual int Parse(char character)
		{
			return (int)ParseTokenID(character);
		}

		/// <summary>
		/// Recupera o token que o termo representa. 
		/// </summary>
		/// <param name="term"></param>
		/// <returns></returns>
		public virtual int Parse(string term)
		{
			return (int)ParseTokenID(term);
		}

		/// <summary>
		/// Recupera o termo com base no token.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public virtual string GetTerm(int token)
		{
			var term = GetTerm((TokenID)token);
			if(term == null)
				throw new InvalidOperationException("Invalid Token");
			return term;
		}

		/// <summary>
		/// Recupera o caracter do token.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public char GetCharacter(int token)
		{
			char result;
			if(!GetCharacter((TokenID)token, out result))
				throw new InvalidOperationException("Invalid Token");
			return result;
		}
	}
}
