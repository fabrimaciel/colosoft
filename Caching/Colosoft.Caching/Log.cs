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

namespace Colosoft.Caching.Configuration.Dom
{
	/// <summary>
	/// Armazena os dados do log.
	/// </summary>
	[Serializable]
	public class Log : ICloneable
	{
		private bool _enabled = true;

		private bool _traceDebug;

		private bool _traceErrors = true;

		private bool _traceNotices;

		private bool _traceWarnings;

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			Log log = new Log();
			log.Enabled = Enabled;
			log.TraceDebug = TraceDebug;
			log.TraceErrors = TraceErrors;
			log.TraceNotices = TraceNotices;
			log.TraceWarnings = TraceWarnings;
			return log;
		}

		/// <summary>
		/// Identifica se o log está habilitado.
		/// </summary>
		[ConfigurationAttribute("enabled")]
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}

		/// <summary>
		/// Identifica se é para fazer um trace de debug.
		/// </summary>
		[ConfigurationAttribute("trace-debug")]
		public bool TraceDebug
		{
			get
			{
				return _traceDebug;
			}
			set
			{
				_traceDebug = value;
			}
		}

		/// <summary>
		/// Identifica se é para fazer o trace dos erros.
		/// </summary>
		[ConfigurationAttribute("trace-errors")]
		public bool TraceErrors
		{
			get
			{
				return _traceErrors;
			}
			set
			{
				_traceErrors = value;
			}
		}

		/// <summary>
		/// Identifica se é para fazer o trace das notificações.
		/// </summary>
		[ConfigurationAttribute("trace-notices")]
		public bool TraceNotices
		{
			get
			{
				return _traceNotices;
			}
			set
			{
				_traceNotices = value;
			}
		}

		/// <summary>
		/// Identifica se é para fazer o trace dos warnings.
		/// </summary>
		[ConfigurationAttribute("trace-warnings")]
		public bool TraceWarnings
		{
			get
			{
				return _traceWarnings;
			}
			set
			{
				_traceWarnings = value;
			}
		}
	}
}
