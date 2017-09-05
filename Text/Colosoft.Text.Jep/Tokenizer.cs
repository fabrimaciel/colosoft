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

namespace Colosoft.Text.Jep.ConfigurableParser
{
	using Colosoft.Text.Jep;
	using Colosoft.Text.Jep.ConfigurableParser.Matchers;
	using Colosoft.Text.Jep.ConfigurableParser.Tokens;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	public class Tokenizer
	{
		protected string currentLine;

		protected int currentLineNumber;

		protected int currentPos;

		public static TerminatorToken EOF = new TerminatorToken("\x0004");

		private static string eol = Environment.NewLine;

		public static TerminatorToken EOL = new TerminatorToken(eol);

		protected List<ITokenMatcher> matchers;

		protected TextReader sr;

		protected List<Token> tokens;

		public Tokenizer(TextReader sr, List<ITokenMatcher> matchers)
		{
			if(eol == null)
			{
				eol = "\n";
			}
			this.sr = sr;
			this.matchers = matchers;
		}

		private Token NextToken(string substr)
		{
			if(substr.Length != 0)
			{
				foreach (ITokenMatcher matcher in this.matchers)
				{
					Token token;
					try
					{
						token = matcher.Match(substr);
					}
					catch(ParseException exception)
					{
						exception.SetPosition(this.currentLineNumber, this.currentPos);
						throw exception;
					}
					if(token != null)
					{
						token.SetPosition(this.currentLineNumber, this.currentPos);
						this.currentPos += token.GetLength();
						return token;
					}
				}
			}
			return null;
		}

		private Token NextTokenMultiLine()
		{
			string currentLine;
			if(this.currentLine == null)
			{
				try
				{
					this.currentLine = this.sr.ReadLine();
					this.currentLineNumber++;
				}
				catch(IOException exception)
				{
					ParseException exception2 = new ParseException("", exception);
					exception2.SetPosition(this.currentLineNumber, this.currentPos);
					throw exception2;
				}
				if(this.currentLine == null)
				{
					return EOF;
				}
				this.currentPos = 0;
			}
			if(this.currentPos >= this.currentLine.Length)
			{
				try
				{
					this.currentLine = this.sr.ReadLine();
					this.currentLineNumber++;
				}
				catch(IOException exception3)
				{
					ParseException exception4 = new ParseException("", exception3);
					exception4.SetPosition(this.currentLineNumber, this.currentPos);
					throw exception4;
				}
				if(this.currentLine == null)
				{
					return EOF;
				}
				this.currentPos = 0;
				return new WhiteSpaceToken(eol);
			}
			if(this.currentPos == 0)
			{
				currentLine = this.currentLine;
			}
			else
			{
				currentLine = this.currentLine.Substring(this.currentPos);
			}
			Token token = this.NextToken(currentLine);
			if(token == null)
			{
				ParseException exception5 = new ParseException("Could not match text '" + currentLine + "'.", this.currentLineNumber, this.currentPos);
				throw exception5;
			}
			return token;
		}

		private Token ReadMultiLine(MultiLineToken t)
		{
			Token token;
			do
			{
				try
				{
					this.currentLine = this.sr.ReadLine();
					this.currentLineNumber++;
				}
				catch(IOException exception)
				{
					ParseException exception2 = new ParseException("", exception);
					exception2.SetPosition(this.currentLineNumber, this.currentPos);
					throw exception2;
				}
				if(this.currentLine == null)
				{
					throw new ParseException("Comment not closed", this.currentLineNumber, this.currentPos);
				}
				token = t.Match(this.currentLine);
			}
			while (token == null);
			this.currentPos = token.GetLength();
			return t.GetCompleteToken();
		}

		public List<Token> Scan()
		{
			this.tokens = new List<Token>();
			while (true)
			{
				Token item = this.NextTokenMultiLine();
				if(item == EOF)
				{
					if(this.tokens.Count == 0)
					{
						return null;
					}
					break;
				}
				if(item is TerminatorToken)
				{
					break;
				}
				if(item is MultiLineToken)
				{
					item = this.ReadMultiLine((MultiLineToken)item);
				}
				this.tokens.Add(item);
			}
			return this.tokens;
		}

		public static string ToString(List<Token> toks)
		{
			StringBuilder builder = new StringBuilder();
			foreach (Token token in toks)
			{
				builder.Append(token.ToString());
				builder.Append('\n');
			}
			return builder.ToString();
		}
	}
}
