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
	/// Configuração do analizador lexa.
	/// </summary>
	public class SqlDefaultLexerConfiguration : DefaultLexerConfiguration
	{
		private static SortedList<string, int> _keywords;

		/// <summary>
		/// Construtor estático.
		/// </summary>
		static SqlDefaultLexerConfiguration()
		{
			_keywords = new SortedList<string, int>();
			foreach (var i in SqlTokenParser.Keywords)
				_keywords.Add(i.Key, (int)i.Value);
		}

		/// <summary>
		/// Recupera as palavras chave.
		/// </summary>
		/// <returns></returns>
		public override SortedList<string, int> GetKeywords()
		{
			return _keywords;
		}
	}
}
