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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Colosoft.Runtime.Remoting
{
	/// <summary>
	/// Classe de log da comunicação.
	/// </summary>
	public class Log
	{
		private readonly System.Diagnostics.TraceSwitch _traceSwitch;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		public Log(string name, string description)
		{
			_traceSwitch = new System.Diagnostics.TraceSwitch(name, description);
		}

		/// <summary>
		/// Escreve uma mensagem de erro.
		/// </summary>
		/// <param name="message"></param>
		public void Error(string message)
		{
			Trace.WriteLineIf(TraceLevel.Error <= _traceSwitch.Level, string.Format(CultureInfo.InvariantCulture, "{0} [Error]\t{1}", DateTime.Now, message));
		}

		/// <summary>
		/// Escreve uma mensagem.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Error(string format, params object[] args)
		{
			this.Error(string.Format(CultureInfo.InvariantCulture, format, args));
		}

		/// <summary>
		/// Escreve uma mensagem informativa.
		/// </summary>
		/// <param name="message"></param>
		public void Info(string message)
		{
			Trace.WriteLineIf(TraceLevel.Info <= _traceSwitch.Level, string.Format(CultureInfo.InvariantCulture, "{0} [Info]\t{1}", DateTime.Now, message));
		}

		/// <summary>
		/// Escreve uma mensagem informativa.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Info(string format, params object[] args)
		{
			this.Info(string.Format(CultureInfo.InvariantCulture, format, args));
		}

		/// <summary>
		/// Escreve uma mensagem.
		/// </summary>
		/// <param name="message"></param>
		public void Verbose(string message)
		{
			Trace.WriteLineIf(TraceLevel.Verbose <= this._traceSwitch.Level, string.Format(CultureInfo.InvariantCulture, "{0} [Verbose]\t{1}", DateTime.Now, message));
		}

		/// <summary>
		/// Escreve uma mensagem.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Verbose(string format, params object[] args)
		{
			this.Verbose(string.Format(CultureInfo.InvariantCulture, format, args));
		}

		/// <summary>
		/// Escreve uma mensagem de alerta.
		/// </summary>
		/// <param name="message"></param>
		public void Warn(string message)
		{
			Trace.WriteLineIf(TraceLevel.Warning <= this._traceSwitch.Level, string.Format(CultureInfo.InvariantCulture, "{0} [Warn]\t{1}", DateTime.Now, message));
		}

		/// <summary>
		/// Escreve uma mensagem de alerta.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Warn(string format, params object[] args)
		{
			this.Warn(string.Format(CultureInfo.InvariantCulture, format, args));
		}
	}
}
