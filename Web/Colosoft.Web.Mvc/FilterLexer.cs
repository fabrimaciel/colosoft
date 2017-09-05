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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation
{
	/// <summary>
	/// Implementação do validador lexo para filtros.
	/// </summary>
	public class FilterLexer
	{
		/// <summary>
		/// Possíveis valores booleanos.
		/// </summary>
		private static readonly string[] Booleans = new string[] {
			"true",
			"false"
		};

		/// <summary>
		/// Possíveis operadores de comparação.
		/// </summary>
		private static readonly string[] ComparisonOperators = new string[] {
			"eq",
			"neq",
			"lt",
			"lte",
			"gt",
			"gte"
		};

		/// <summary>
		/// Possíveis funções.
		/// </summary>
		private static readonly string[] Functions = new string[] {
			"contains",
			"endswith",
			"startswith",
			"notsubstringof",
			"doesnotcontain"
		};

		/// <summary>
		/// Possíveis operadores lógicos.
		/// </summary>
		private static readonly string[] LogicalOperators = new string[] {
			"and",
			"or",
			"not"
		};

		private int _currentCharacterIndex;

		private readonly string _input;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="input"></param>
		public FilterLexer(string input)
		{
			input = input ?? string.Empty;
			_input = input.Trim('~');
		}

		/// <summary>
		/// Recupera o token boolean para o resultado.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private static FilterToken Boolean(string result)
		{
			return new FilterToken {
				TokenType = FilterTokenType.Boolean,
				Value = result
			};
		}

		/// <summary>
		/// Recupera o token de virgula para o resultado.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private static FilterToken Comma(string result)
		{
			return new FilterToken {
				TokenType = FilterTokenType.Comma,
				Value = result
			};
		}

		/// <summary>
		/// Recupera o operador de comparação para o resultado.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private static FilterToken ComparisonOperator(string result)
		{
			return new FilterToken {
				TokenType = FilterTokenType.ComparisonOperator,
				Value = result
			};
		}

		/// <summary>
		/// Recupera o token de data para o resultado. 
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private FilterToken Date(string result)
		{
			this.TryParseString(out result);
			return new FilterToken {
				TokenType = FilterTokenType.DateTime,
				Value = result
			};
		}

		/// <summary>
		/// Recupera o token de função para o resultado.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private static FilterToken Function(string result)
		{
			return new FilterToken {
				TokenType = FilterTokenType.Function,
				Value = result
			};
		}

		/// <summary>
		/// Recupera o token de um identificador para o resultado.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private FilterToken Identifier(string result)
		{
			if(result == "datetime")
			{
				return this.Date(result);
			}
			if(IsComparisonOperator(result))
				return ComparisonOperator(result);
			if(IsLogicalOperator(result))
				return LogicalOperator(result);
			if(IsBoolean(result))
				return Boolean(result);
			if(IsFunction(result))
				return Function(result);
			return Property(result);
		}

		/// <summary>
		/// Verifica se valor informado representa um valor boolean.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool IsBoolean(string value)
		{
			return (Array.IndexOf<string>(Booleans, value) > -1);
		}

		/// <summary>
		/// Verifica se o valor informado é um operador de comparação.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool IsComparisonOperator(string value)
		{
			return (Array.IndexOf<string>(ComparisonOperators, value) > -1);
		}

		/// <summary>
		/// Verifica se o valor informado é uma função.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool IsFunction(string value)
		{
			return (Array.IndexOf<string>(Functions, value) > -1);
		}

		/// <summary>
		/// Verifica se o caracter informado é parte de uma identificador.
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		private static bool IsIdentifierPart(char character)
		{
			if((!char.IsLetter(character) && !char.IsDigit(character)) && (character != '_'))
				return (character == '$');
			return true;
		}

		/// <summary>
		/// Verifica se o caracter informado é o inicio de um identificador.
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		private static bool IsIdentifierStart(char character)
		{
			if((!char.IsLetter(character) && (character != '_')) && (character != '$'))
				return (character == '@');
			return true;
		}

		/// <summary>
		/// Verifica se o valor informado é um operador lógico.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool IsLogicalOperator(string value)
		{
			return (Array.IndexOf<string>(LogicalOperators, value) > -1);
		}

		/// <summary>
		/// Recupera o token identifica que possui um parenteses na esquerada.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private static FilterToken LeftParenthesis(string result)
		{
			return new FilterToken {
				TokenType = FilterTokenType.LeftParenthesis,
				Value = result
			};
		}

		/// <summary>
		/// Recupera o token de um operador lógico para o resultado.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private static FilterToken LogicalOperator(string result)
		{
			if(result == "or")
				return new FilterToken {
					TokenType = FilterTokenType.Or,
					Value = result
				};
			if(result == "and")
				return new FilterToken {
					TokenType = FilterTokenType.And,
					Value = result
				};
			return new FilterToken {
				TokenType = FilterTokenType.Not,
				Value = result
			};
		}

		/// <summary>
		/// Move para o próximo caracter.
		/// </summary>
		/// <returns></returns>
		private char Next()
		{
			_currentCharacterIndex++;
			return this.Peek();
		}

		/// <summary>
		/// Recupera o token de um número.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private static FilterToken Number(string result)
		{
			return new FilterToken {
				TokenType = FilterTokenType.Number,
				Value = result
			};
		}

		/// <summary>
		/// Recupera caracter que está no top.
		/// </summary>
		/// <returns></returns>
		private char Peek()
		{
			return Peek(0);
		}

		/// <summary>
		/// Recupera os n caracters do top.
		/// </summary>
		/// <param name="chars"></param>
		/// <returns></returns>
		private char Peek(int chars)
		{
			if((_currentCharacterIndex + chars) < _input.Length)
			{
				return _input[_currentCharacterIndex + chars];
			}
			return (char)0xffff;
		}

		/// <summary>
		/// Recupera o token da propriedade.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private static FilterToken Property(string result)
		{
			return new FilterToken {
				TokenType = FilterTokenType.Property,
				Value = result
			};
		}

		/// <summary>
		/// Realiza a leitura do texto até o predicado ser válido.
		/// </summary>
		/// <param name="predicate">Predicado que será usado na leitura.</param>
		/// <param name="result"></param>
		/// <returns></returns>
		private string Read(Func<char, bool> predicate, StringBuilder result)
		{
			for(char ch = Peek(); predicate(ch); ch = Next())
				result.Append(ch);
			return result.ToString();
		}

		/// <summary>
		/// Recupera o token do parentese da direita.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private static FilterToken RightParenthesis(string result)
		{
			return new FilterToken {
				TokenType = FilterTokenType.RightParenthesis,
				Value = result
			};
		}

		/// <summary>
		/// Salta os separadores.
		/// </summary>
		private void SkipSeparators()
		{
			for(char ch = this.Peek(); ch == '~'; ch = this.Next())
			{
			}
		}

		/// <summary>
		/// Recupera o token de um texto.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private static FilterToken String(string result)
		{
			return new FilterToken {
				TokenType = FilterTokenType.String,
				Value = result
			};
		}

		/// <summary>
		/// Tokenize.
		/// </summary>
		/// <returns></returns>
		public IList<FilterToken> Tokenize()
		{
			List<FilterToken> list = new List<FilterToken>();
			while (this._currentCharacterIndex < this._input.Length)
			{
				string str;
				if(!TryParseIdentifier(out str))
				{
					if(!TryParseNumber(out str))
					{
						if(!TryParseString(out str))
						{
							if(!TryParseCharacter(out str, '('))
							{
								if(!TryParseCharacter(out str, ')'))
								{
									if(!TryParseCharacter(out str, ','))
									{
										throw new FilterParserException("Expected token");
									}
									list.Add(Comma(str));
								}
								else
								{
									list.Add(RightParenthesis(str));
								}
							}
							else
							{
								list.Add(LeftParenthesis(str));
							}
						}
						else
						{
							list.Add(String(str));
						}
						continue;
					}
					list.Add(Number(str));
				}
				else
				{
					list.Add(this.Identifier(str));
					continue;
				}
			}
			return list;
		}

		/// <summary>
		/// Tenta fazer um parser no character.
		/// </summary>
		/// <param name="character"></param>
		/// <param name="whatCharacter"></param>
		/// <returns></returns>
		private bool TryParseCharacter(out string character, char whatCharacter)
		{
			this.SkipSeparators();
			char ch = this.Peek();
			if(ch != whatCharacter)
			{
				character = null;
				return false;
			}
			this.Next();
			character = ch.ToString();
			return true;
		}

		/// <summary>
		/// Tenta executar um parser para recuper ao identificador.
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		private bool TryParseIdentifier(out string identifier)
		{
			this.SkipSeparators();
			char character = this.Peek();
			StringBuilder result = new StringBuilder();
			if(!IsIdentifierStart(character))
			{
				identifier = null;
				return false;
			}
			result.Append(character);
			this.Next();
			identifier = Read(c =>  {
				if(!IsIdentifierPart(c))
				{
					return c == '.';
				}
				return true;
			}, result);
			return true;
		}

		/// <summary>
		/// Tenta executa o parser para recupera o número.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		private bool TryParseNumber(out string number)
		{
			this.SkipSeparators();
			char ch = this.Peek();
			StringBuilder result = new StringBuilder();
			int decimalSymbols = 0;
			switch(ch)
			{
			case '-':
			case '+':
				result.Append(ch);
				ch = this.Next();
				break;
			}
			if(ch == '.')
			{
				decimalSymbols++;
				result.Append(ch);
				ch = this.Next();
			}
			if(!char.IsDigit(ch))
			{
				number = null;
				return false;
			}
			number = this.Read(delegate(char character) {
				if(character != '.')
				{
					return char.IsDigit(character);
				}
				if(decimalSymbols >= 1)
				{
					throw new FilterParserException("A number cannot have more than one decimal symbol");
				}
				decimalSymbols++;
				return true;
			}, result);
			return true;
		}

		/// <summary>
		/// Tenta executar o parser para recupera o texto.
		/// </summary>
		/// <param name="string"></param>
		/// <returns></returns>
		private bool TryParseString(out string text)
		{
			this.SkipSeparators();
			if(this.Peek() != '\'')
			{
				text = null;
				return false;
			}
			char ch = this.Next();
			StringBuilder result = new StringBuilder();
			text = this.Read(delegate(char character) {
				if(character == 0xffff)
				{
					throw new FilterParserException("Unterminated string");
				}
				if((character == '\'') && (this.Peek(1) == '\''))
				{
					this.Next();
					return true;
				}
				return character != '\'';
			}, result);
			this.Next();
			return true;
		}
	}
}
