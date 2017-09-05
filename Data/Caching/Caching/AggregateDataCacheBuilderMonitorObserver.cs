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

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Implementação do agregador de observers.
	/// </summary>
	public class AggregateDataCacheBuilderMonitorObserver : Colosoft.Progress.AggregateProgressWithMessageObserver<IDataCacheBuildMonitorObserver>, IDataCacheBuildMonitorObserver
	{
		/// <summary>
		/// Evento acioando quando a escuta da execução do build for terminada.
		/// </summary>
		/// <param name="buildExecutionUid">Identifica dor da execução do build.</param>
		public void OnListenTerminate(Guid buildExecutionUid)
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnListenTerminate(buildExecutionUid);
		}

		/// <summary>
		/// Evento acionado quando ocorre um erro de comunicação.
		/// </summary>
		/// <param name="buildExecutionUid"></param>
		/// <param name="exception"></param>
		public void OnCheckError(Guid buildExecutionUid, Exception exception)
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnCheckError(buildExecutionUid, exception);
		}
	}
}
