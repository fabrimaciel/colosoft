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

namespace Colosoft.Progress
{
	/// <summary>
	/// Implementação o agregador de observers de progresso.
	/// </summary>
	public class AggregateProgressObserver<TObserver> : AggregateObserver<TObserver>, IProgressObserver where TObserver : IProgressObserver
	{
		/// <summary>
		/// Método acionado quando o progresso for alterado
		/// </summary>
		/// <param name="e"></param>
		public void OnProgressChanged(Progress.ProgressChangedEventArgs e)
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnProgressChanged(e);
		}

		/// <summary>
		/// Método acionado quando o progresso for completado.
		/// </summary>
		/// <param name="e"></param>
		public void OnProgressCompleted(Progress.ProgressCompletedEventArgs e)
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnProgressCompleted(e);
		}

		/// <summary>
		/// Método acionado quando o progresso for iniciado.
		/// </summary>
		/// <param name="userState"></param>
		public void OnStart(object userState)
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnStart(userState);
		}
	}
	/// <summary>
	/// Implementação o agregador de observers de progresso.
	/// </summary>
	public class AggregateProgressObserver : AggregateProgressObserver<IProgressObserver>
	{
	}
	/// <summary>
	/// Implementação do agregador de observers de progresso.
	/// </summary>
	public class AggregateProgressWithMessageObserver<TObserver> : AggregateProgressObserver<TObserver>, IProgressWithMessageObserver where TObserver : IProgressWithMessageObserver
	{
		/// <summary>
		/// Método acionado quando o estado da mensagem for alterado.
		/// </summary>
		/// <param name="e"></param>
		public void OnProgressMessageChanged(Progress.ProgressMessageChangedEventArgs e)
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnProgressMessageChanged(e);
		}
	}
	/// <summary>
	/// Implementação do agregador de observers de progresso.
	/// </summary>
	public class AggregateProgressWithMessageObserver : AggregateProgressWithMessageObserver<IProgressWithMessageObserver>
	{
	}
}
