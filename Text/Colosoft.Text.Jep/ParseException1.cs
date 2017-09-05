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

namespace Colosoft.Text.Jep.Parser
{
	using Colosoft.Text.Jep.ConfigurableParser.Tokens;
	using System;
	using System.Text;

	public class ParseException : Exception
	{
		public Token currentToken;

		protected string eol;

		public int[][] expectedTokenSequences;

		protected bool specialConstructor;

		public string[] tokenImage;

		public ParseException()
		{
			this.eol = Environment.NewLine;
			this.specialConstructor = false;
		}

		public ParseException(string message) : base(message)
		{
			this.eol = Environment.NewLine;
			this.specialConstructor = false;
		}

		public ParseException(Token currentTokenVal, int[][] expectedTokenSequencesVal, string[] tokenImageVal) : base("")
		{
			this.eol = Environment.NewLine;
			this.specialConstructor = true;
			this.currentToken = currentTokenVal;
			this.expectedTokenSequences = expectedTokenSequencesVal;
			this.tokenImage = tokenImageVal;
		}

		protected string AddEscapes(string str)
		{
			StringBuilder builder = new StringBuilder();
			for(int i = 0; i < str.Length; i++)
			{
				char ch;
				switch(str[i])
				{
				case '\'':
				{
					builder.Append(@"\'");
					continue;
				}
				case '0':
				{
					continue;
				}
				case '\\':
				{
					builder.Append(@"\\");
					continue;
				}
				case '\b':
				{
					builder.Append(@"\b");
					continue;
				}
				case '\t':
				{
					builder.Append(@"\t");
					continue;
				}
				case '\n':
				{
					builder.Append(@"\n");
					continue;
				}
				case '\f':
				{
					builder.Append(@"\f");
					continue;
				}
				case '\r':
				{
					builder.Append(@"\r");
					continue;
				}
				case '"':
				{
					builder.Append("\\\"");
					continue;
				}
				}
				if(((ch = str[i]) < ' ') || (ch > '~'))
				{
					string str2 = "0000" + ((int)ch).ToString("x");
					builder.Append(@"\u" + str2.Substring(str2.Length - 4, str2.Length));
				}
				else
				{
					builder.Append(ch);
				}
			}
			return builder.ToString();
		}

		public override string Message
		{
			get
			{
				if(!this.specialConstructor)
				{
					return base.Message;
				}
				StringBuilder builder = new StringBuilder();
				int length = 0;
				for(int i = 0; i < this.expectedTokenSequences.Length; i++)
				{
					if(length < this.expectedTokenSequences[i].Length)
					{
						length = this.expectedTokenSequences[i].Length;
					}
					for(int j = 0; j < this.expectedTokenSequences[i].Length; j++)
					{
						builder.Append(this.tokenImage[this.expectedTokenSequences[i][j]]).Append(" ");
					}
					if(this.expectedTokenSequences[i][this.expectedTokenSequences[i].Length - 1] != 0)
					{
						builder.Append("...");
					}
					builder.Append(this.eol).Append("    ");
				}
				string str = "Encountered \"";
				str = str + "." + this.eol;
				if(this.expectedTokenSequences.Length == 1)
				{
					str = str + "Was expecting:" + this.eol + "    ";
				}
				else
				{
					str = str + "Was expecting one of:" + this.eol + "    ";
				}
				return (str + builder.ToString());
			}
		}
	}
}
