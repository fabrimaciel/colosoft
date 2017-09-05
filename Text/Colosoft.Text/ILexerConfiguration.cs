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
	/// Configuração do analizador.
	/// </summary>
	public interface ILexerConfiguration
	{
		/// <summary>
		/// Recupera os tokens da tabulação.
		/// </summary>
		char[] Tabs
		{
			get;
		}

		/// <summary>
		/// Recupera o tokens de junção de tabulação.
		/// </summary>
		char[] JoinTabs
		{
			get;
		}

		/// <summary>
		/// Identificadores dos token que são usados para identificar uma junção de expressões.
		/// </summary>
		int[] ExpressionJoinsTokenIDs
		{
			get;
		}

		/// <summary>
		/// Containers que serão usados.
		/// </summary>
		ContainerChars[] Containers
		{
			get;
		}

		/// <summary>
		/// Containers de String que serão usados.
		/// </summary>
		ContainerChars[] StringContainers
		{
			get;
		}

		/// <summary>
		/// Recupera os tokens de espaço.
		/// </summary>
		char[] Spaces
		{
			get;
		}

		/// <summary>
		/// Identifica se o analizador suporta caracteres ASCII.
		/// </summary>
		bool SupportASCIIChar
		{
			get;
		}

		/// <summary>
		/// Prefixo para caractere ASCII. '\'
		/// </summary>
		char ASCIICharPrefix
		{
			get;
		}

		/// <summary>
		/// Recupera as palavras chave que serão usadas no analizador.
		/// Keyword : tokenID
		/// </summary>
		/// <returns></returns>
		SortedList<string, int> GetKeywords();

		/// <summary>
		/// Verifica se o caracter passado é alfanumerico.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		bool IsAlphanumeric(char c);

		/// <summary>
		/// Verifica se o caracter passado está contido no alfabeto.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		bool IsAlpha(char c);

		/// <summary>
		/// Verifica se o caracter pertence aos numeros decimais e hexadecimais
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		bool IsNumeric(char c);
	}
}
