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

namespace Colosoft.Collections
{
	/// <summary>
	/// Dados do evento adaptado da alteração da coleção.
	/// </summary>
	public class CollectionChangedWrapperEventData : IDisposable
	{
		/// <summary>
		/// Instancia do dispatcher associado.
		/// </summary>
		private Colosoft.Threading.IDispatcher _dispatcher;

		/// <summary>
		/// Ação associada.
		/// </summary>
		private System.Collections.Specialized.NotifyCollectionChangedEventHandler _action;

		private NotifyCollectionChangedDispatcherPriority _priority;

		/// <summary>
		/// Prioridade associada.
		/// </summary>
		public NotifyCollectionChangedDispatcherPriority Priority
		{
			get
			{
				return _priority;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dispatcher">Dispatcher que será utilizado pela instancia.</param>
		/// <param name="action">Ação que será acionada.</param>
		public CollectionChangedWrapperEventData(Colosoft.Threading.IDispatcher dispatcher, System.Collections.Specialized.NotifyCollectionChangedEventHandler action) : this(dispatcher, action, NotifyCollectionChangedDispatcherPriority.Normal)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dispatcher">Dispatcher que será utilizado pela instancia.</param>
		/// <param name="action">Ação que será acionada.</param>
		/// <param name="priority">Prioridade;</param>
		public CollectionChangedWrapperEventData(Colosoft.Threading.IDispatcher dispatcher, System.Collections.Specialized.NotifyCollectionChangedEventHandler action, NotifyCollectionChangedDispatcherPriority priority)
		{
			_dispatcher = dispatcher;
			_action = action;
			_priority = priority;
		}

		/// <summary>
		/// Realiza a chamada para tratar o evento.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void Invoke(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if(_dispatcher != null && !_dispatcher.CheckAccess())
				try
				{
					_dispatcher.Invoke(_action, Threading.DispatcherPriority.Normal, new object[] {
						sender,
						e
					});
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					Exception ex2 = ex;
					if(ex.InnerException != null)
						ex2 = ex.InnerException;
					if(!((ex2 is NotSupportedException && ex2.Source == "PresentationFramework") || (ex2 is InvalidOperationException && ex2.Source == "WindowsBase")))
						throw;
				}
				catch(Exception ex)
				{
					if(!((ex is NotSupportedException && ex.Source == "PresentationFramework") || (ex is InvalidOperationException && ex.Source == "WindowsBase")))
						throw;
				}
			else
				try
				{
					_action(sender, e);
				}
				catch(Exception ex)
				{
					if(!((ex is NotSupportedException && ex.Source == "PresentationFramework") || (ex is InvalidOperationException && ex.Source == "WindowsBase")))
						throw;
				}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Libera instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
	}
}
