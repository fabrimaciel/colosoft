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
	/// Configuração padrão do lexer.
	/// </summary>
	public class DefaultLexerConfiguration : ILexerConfiguration
	{
		private char[] _tabs;

		private char[] _joinTabs;

		private ContainerChars[] _containers;

		private char[] _spaces;

		private ContainerChars[] _stringContainers;

		private int[] _expressionJoinsTokenIDs;

		/// <summary>
		/// Recupera os tokens da tabulação.
		/// </summary>
		public char[] Tabs
		{
			get
			{
				return _tabs;
			}
		}

		/// <summary>
		/// Recupera o tokens de junção de tabulação.
		/// </summary>
		public char[] JoinTabs
		{
			get
			{
				return _joinTabs;
			}
		}

		/// <summary>
		/// Containers que serão usados.
		/// </summary>
		public ContainerChars[] Containers
		{
			get
			{
				return _containers;
			}
		}

		/// <summary>
		/// Containers de String que serão usados.
		/// </summary>
		public ContainerChars[] StringContainers
		{
			get
			{
				return _stringContainers;
			}
		}

		/// <summary>
		/// Recupera os tokens de espaço.
		/// </summary>
		public char[] Spaces
		{
			get
			{
				return _spaces;
			}
		}

		/// <summary>
		/// Identifica se o analizador suporta caracteres ASCII.
		/// </summary>
		public bool SupportASCIIChar
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Prefixo para caractere ASCII. '\'
		/// </summary>
		public char ASCIICharPrefix
		{
			get
			{
				return '\\';
			}
		}

		/// <summary>
		/// Identificadores dos token que são usados para identificar uma junção de expressões.
		/// </summary>
		public int[] ExpressionJoinsTokenIDs
		{
			get
			{
				return _expressionJoinsTokenIDs;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public DefaultLexerConfiguration()
		{
			_tabs = new char[] {
				'+',
				'-',
				'*',
				'/',
				'<',
				'>',
				'=',
				'!',
				';',
				':',
				',',
				'&',
				'|',
				'%',
				'.'
			};
			_joinTabs = new char[] {
				'+',
				'-',
				'=',
				'>',
				'|',
				'&',
				'/',
				'*'
			};
			_containers = new char[][] {
				new char[] {
					'(',
					')'
				},
				new char[] {
					'{',
					'}'
				}
			}.Select(f => new ContainerChars(f[0], f[1])).ToArray();
			_spaces = new char[] {
				' ',
				'\n',
				'\t',
				'\r'
			};
			_stringContainers = new char[][] {
				new char[] {
					'"',
					'"'
				},
				new char[] {
					'\'',
					'\''
				},
				new char[] {
					'`',
					'`'
				},
				new char[] {
					'#',
					'#'
				},
				new char[] {
					'[',
					']'
				}
			}.Select(f => new ContainerChars(f[0], f[1])).ToArray();
			_expressionJoinsTokenIDs = new TokenID[] {
				TokenID.And,
				TokenID.Or,
				TokenID.PlusPlus,
				TokenID.MinusMinus,
				TokenID.MinusGreater,
				TokenID.EqualEqual,
				TokenID.NotEqual,
				TokenID.Greater,
				TokenID.Less,
				TokenID.GreaterEqual,
				TokenID.LessEqual,
				TokenID.PlusEqual,
				TokenID.BMultiComment,
				TokenID.EMultiComment,
				TokenID.SingleComment,
				TokenID.Equal
			}.Select(f => (int)f).ToArray();
		}

		/// <summary>
		/// Recupera as palavras chave que serão usadas no analizador.
		/// </summary>
		/// <returns></returns>
		public virtual SortedList<string, int> GetKeywords()
		{
			return new SortedList<string, int>();
		}

		/// <summary>
		/// Verifica se o caracter passado é alfanumerico.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public virtual bool IsAlphanumeric(char c)
		{
			return ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_' || c == '@');
		}

		/// <summary>
		/// Verifica se o caracter passado está contido no alfabeto.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public virtual bool IsAlpha(char c)
		{
			return ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '@' || c == '?');
		}

		/// <summary>
		/// Verifica se o caracter pertence aos numeros decimais e hexadecimais
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public virtual bool IsNumeric(char c)
		{
			return ((c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'F') || (c >= '0' && c <= '9') || c == '.');
		}
	}
}
