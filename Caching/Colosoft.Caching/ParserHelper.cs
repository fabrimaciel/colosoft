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
using Colosoft.Text.Parser;
using Colosoft.Logging;
using Colosoft.Caching.Queries;

namespace Colosoft.Caching.Util
{
	/// <summary>
	/// Classe responsável por auxiliar no parser de uma consulta.
	/// </summary>
	internal class ParserHelper
	{
		private Reduction _currentReduction;

		private ILogger _logger;

		/// <summary>
		/// Redução processada.
		/// </summary>
		public Reduction CurrentReduction
		{
			get
			{
				return _currentReduction;
			}
		}

		/// <summary>
		/// <see cref="ILogger"/> associado.
		/// </summary>
		private ILogger Logger
		{
			get
			{
				return _logger;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="logger"></param>
		public ParserHelper(ILogger logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Executa o parser na consulta.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public ParseMessage Parse(string query)
		{
			using (var parser = new CQLParser("Colosoft.Caching.Resources.cql.cgt", _logger))
			{
				using (var source = new System.IO.StringReader(query))
				{
					ParseMessage message = parser.Parse(source, true);
					_currentReduction = parser.CurrentReduction;
					return message;
				}
			}
		}
	}
}
