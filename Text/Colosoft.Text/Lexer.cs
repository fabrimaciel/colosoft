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
	/// Representa um analizador lexo.
	/// </summary>
	public class Lexer
	{
		/// <summary>
		/// Possíveis estados do analizador.
		/// </summary>
		public enum State
		{
			/// <summary>
			/// Identifica o estado 0.
			/// </summary>
			S0,
			/// <summary>
			/// Identifica o estado 1.
			/// </summary>
			S1,
			/// <summary>
			/// Identifica o estado 2.
			/// </summary>
			S2,
			/// <summary>
			/// Identifica o estado 3.
			/// </summary>
			S3,
			/// <summary>
			/// Identifica o estado 4.
			/// </summary>
			S4,
			/// <summary>
			/// Identifica o estado 5.
			/// </summary>
			S5,
			/// <summary>
			/// Identifica o estado 6.
			/// </summary>
			S6,
			/// <summary>
			/// Identifica o estado 7.
			/// </summary>
			S7,
			/// <summary>
			/// Identifica o estado 8.
			/// </summary>
			S8,
			/// <summary>
			/// Identifica o estado final do analizador.
			/// </summary>
			End
		}

		private SortedList<string, int> _keywords;

		private ITokenParser _tokenParser;

		private ILexerConfiguration _configuration;

		/// <summary>
		/// Parse do token associado com a instancia.
		/// </summary>
		public virtual ITokenParser TokenParser
		{
			get
			{
				return _tokenParser;
			}
		}

		/// <summary>
		/// Instancia da configuração do analizador.
		/// </summary>
		public ILexerConfiguration Configuration
		{
			get
			{
				return _configuration;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="tokenParser"></param>
		/// <param name="configuration"></param>
		public Lexer(ITokenParser tokenParser, ILexerConfiguration configuration)
		{
			_tokenParser = tokenParser;
			_configuration = configuration;
			_keywords = configuration.GetKeywords();
		}

		/// <summary>
		/// Executa a análise lexa no comando.
		/// </summary>
		/// <param name="text">Texto que será analizado.</param>
		/// <returns></returns>
		public LexerResult Execute(string text)
		{
			var lines = new List<ExpressionLine>();
			var expressions = new List<Expression>();
			Stack<ExpressionContainer> containersStack = new Stack<ExpressionContainer>();
			var containerId = 1;
			var containers = new List<ExpressionContainer>();
			int beginPos = 0;
			int pos = 0;
			State state = State.S0;
			char usingSpecialContainerChar = TokenParser.GetCharacter((int)TokenID.Quote);
			ExpressionLine lastLine = new ExpressionLine(pos);
			char current = char.MinValue;
			int currentToken = (int)TokenID.InvalidExpression;
			lines.Add(lastLine);
			bool end = false;
			while (!end || (pos == text.Length && (state == State.S8 || state == State.S3 || state == State.S2)))
			{
				current = pos == text.Length ? text[pos - 1] : text[pos];
				currentToken = TokenParser.Parse(current);
				switch(state)
				{
				case State.S0:
					if(Array.Exists<char>(Configuration.Spaces, f => f == current))
					{
						state = State.S0;
						if(currentToken == (int)TokenID.Newline)
						{
							lastLine.Length = pos - lastLine.BeginPoint;
							lastLine = new ExpressionLine(pos);
							lines.Add(lastLine);
						}
						else
						{
						}
						beginPos = pos + 1;
					}
					else if(Array.Exists<ContainerChars>(Configuration.StringContainers, f => f.Start == current))
					{
						beginPos = pos + 1;
						usingSpecialContainerChar = Array.Find<ContainerChars>(Configuration.StringContainers, f => f.Start == current).Stop;
						state = State.S1;
					}
					else if(Array.Exists<char>(Configuration.Tabs, f => f == current))
					{
						beginPos = pos;
						state = State.S2;
					}
					else if(Configuration.IsAlpha(text[pos]))
						state = State.S3;
					else if(char.IsDigit(text[pos]))
						state = State.S8;
					else if(Array.Exists<ContainerChars>(Configuration.Containers, f => f.Start == current))
					{
						Expression e = new Expression(containersStack.Count > 0 ? containersStack.Peek() : null, pos, lastLine, text[pos]);
						e.Token = currentToken;
						var containerChars = Array.Find(Configuration.Containers, f => f.Start == current).Clone();
						expressions.Add(e);
						containersStack.Push(new ExpressionContainer(containerId++, pos + 1, containerChars, lastLine));
						beginPos = pos + 1;
						state = State.S0;
					}
					else if(Array.Exists<ContainerChars>(Configuration.Containers, f => f.Stop == current))
					{
						if(containersStack.Count == 0)
						{
							throw new LexerException(String.Format("Not open tag for caracter {0}. Line: {1} - Col: {2}.", text[pos], lines.Count, pos - lastLine.BeginPoint));
						}
						else if(containersStack.Peek().ContainerChars.Stop == text[pos])
						{
							var ce = containersStack.Pop();
							ce.Length = pos - ce.BeginPoint;
							containers.Add(ce);
							Expression e = new Expression(containersStack.Count > 0 ? containersStack.Peek() : null, pos, lastLine, text[pos]);
							e.Token = currentToken;
							expressions.Add(e);
							if(containersStack.Count == 0)
							{
							}
							beginPos = pos + 1;
							state = State.S0;
						}
						else
							throw new LexerException(String.Format("Expected caracter {0}. Line: {1} - Col: {2}.", containersStack.Peek().ContainerChars.Stop, lines.Count, pos - lastLine.BeginPoint));
					}
					else
					{
						throw new LexerException(String.Format("Invalid caracter '{0}' in expression context.", text[pos]));
					}
					break;
				case State.S1:
					if(current == usingSpecialContainerChar)
					{
						if(!Configuration.SupportASCIIChar || ((pos > 0 && TokenParser.Parse(text[pos - 1]) != Configuration.ASCIICharPrefix) || (pos > 1 && TokenParser.Parse(text[pos - 2]) == Configuration.ASCIICharPrefix)))
						{
							var specialToken = TokenParser.Parse(usingSpecialContainerChar);
							if(specialToken == (int)TokenID.RBracket || specialToken == (int)TokenID.BSQuote)
							{
								Expression e = new Expression(containersStack.Count > 0 ? containersStack.Peek() : null, beginPos, pos - beginPos, lastLine, text, (int)TokenID.Identifier);
								e.CurrentSpecialContainer = new ContainerChars(text[beginPos - 1], text[pos]);
								expressions.Add(e);
							}
							else
							{
								SpecialContainerExpression sce = new SpecialContainerExpression(containersStack.Count > 0 ? containersStack.Peek() : null, beginPos, pos - beginPos, lastLine, text, usingSpecialContainerChar);
								expressions.Add(sce);
								sce.ContainerToken = specialToken;
							}
							beginPos = pos + 1;
							state = State.S0;
						}
					}
					break;
				case State.S2:
					if(text[pos - 1] == TokenParser.GetCharacter((int)TokenID.Minus) && char.IsDigit(text[pos]))
					{
						if(text.Length < 2 || (text.Length > 2 && Array.IndexOf<char>(Configuration.Tabs, text[pos - 2]) >= 0) || (text.Length > 2 && Array.IndexOf<char>(Configuration.Spaces, text[pos - 2]) != -1) || (text.Length > 2 && Configuration.Containers.Any(f => f.Start == text[pos - 2])))
						{
							state = State.S8;
							continue;
						}
					}
					int joinTokenID;
					if(!end)
						joinTokenID = TokenParser.Parse(text[pos - 1].ToString() + text[pos].ToString());
					else
						joinTokenID = TokenParser.Parse(text[pos - 1].ToString());
					if(Array.Exists<int>(Configuration.ExpressionJoinsTokenIDs, f => f == joinTokenID))
					{
						Expression e = new Expression(containersStack.Count > 0 ? containersStack.Peek() : null, beginPos, 2, lastLine, text);
						e.Token = TokenParser.Parse(e.Text);
						if(e.Token == (int)TokenID.InvalidExpression)
							e.Token = (int)TokenID.InvalidTab;
						expressions.Add(e);
						beginPos = pos + 1;
						state = State.S0;
					}
					else
					{
						TabExpression te = new TabExpression(containersStack.Count > 0 ? containersStack.Peek() : null, beginPos, lastLine, text);
						te.Token = TokenParser.Parse(text[beginPos]);
						expressions.Add(te);
						beginPos = pos;
						pos--;
						state = State.S0;
					}
					break;
				case State.S3:
					if(pos == text.Length || !Configuration.IsAlphanumeric(text[pos]))
					{
						Expression e = new Expression(containersStack.Count > 0 ? containersStack.Peek() : null, beginPos, pos - beginPos, lastLine, text);
						if(char.IsDigit(e.Text[0]) || e.Text[0] == '-')
						{
							if(e.Text.Length > 2 && (e.Text[1] == 'x' || e.Text[1] == 'X') && System.Text.RegularExpressions.Regex.IsMatch(e.Text.Substring(2), "[0-9]"))
							{
								e.Token = (int)TokenID.HexLiteral;
							}
							else if(System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^([-]|[0-9])[0-9]*$"))
							{
								e.Token = (int)TokenID.IntLiteral;
							}
							else if(System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$"))
							{
								e.Token = (int)TokenID.RealLiteral;
							}
							var aux = _tokenParser.Parse(e.Text.ToLower());
							if(aux == (int)TokenID.InvalidExpression)
								e.Token = (int)TokenID.Identifier;
							else
								e.Token = aux;
						}
						else if(_keywords.ContainsKey(e.Text.ToLower()))
						{
							e.Token = _keywords[e.Text.ToLower()];
						}
						else
						{
							var aux = _tokenParser.Parse(e.Text.ToLower());
							if(aux == (int)TokenID.InvalidExpression)
								e.Token = (int)TokenID.Identifier;
							else
								e.Token = aux;
						}
						expressions.Add(e);
						beginPos = pos;
						pos--;
						state = State.S0;
					}
					break;
				case State.S8:
					if(pos == text.Length || !Configuration.IsNumeric(text[pos]))
					{
						Expression e = new Expression(containersStack.Count > 0 ? containersStack.Peek() : null, beginPos, pos - beginPos, lastLine, text);
						if(e.Text.Length > 2 && (e.Text[1] == 'x' || e.Text[1] == 'X') && System.Text.RegularExpressions.Regex.IsMatch(e.Text.Substring(2), "[0-9]"))
						{
							e.Token = (int)TokenID.HexLiteral;
						}
						else if(System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9]"))
						{
							e.Token = (int)TokenID.IntLiteral;
						}
						else if(System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$"))
						{
							e.Token = (int)TokenID.RealLiteral;
						}
						else
							throw new LexerException("Expected number or hexadecimal.");
						expressions.Add(e);
						beginPos = pos;
						pos--;
						state = State.S0;
					}
					break;
				}
				pos++;
				end = (pos == text.Length);
			}
			lastLine.Length = pos - lastLine.BeginPoint;
			if(state != State.S0)
				throw new LexerException("Invalid expression.");
			if(containersStack.Count > 0)
				throw new LexerException(String.Format("{0} expected. Line: {1} - Col: {2} -- opened in Line: {3} - Col: {4}", containersStack.Peek().ContainerChars.Stop, lines.Count, lastLine.Length, containersStack.Peek().Line.BeginPoint, containersStack.Peek().BeginPoint));
			return new LexerResult(text, expressions, lines, containers);
		}
	}
}
