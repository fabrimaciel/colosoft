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
	public class ListChangedWrapperEventData
	{
		/// <summary>
		/// Instancia do dispatcher associado.
		/// </summary>
		internal Colosoft.Threading.IDispatcher Dispatcher
		{
			get;
			set;
		}

		/// <summary>
		/// Ação associada.
		/// </summary>
		internal System.ComponentModel.ListChangedEventHandler Action
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dispatcher"></param>
		/// <param name="action"></param>
		public ListChangedWrapperEventData(Colosoft.Threading.IDispatcher dispatcher, System.ComponentModel.ListChangedEventHandler action)
		{
			Dispatcher = dispatcher;
			Action = action;
		}

		/// <summary>
		/// Realiza a chamada para tratar o evento.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void Invoke(object sender, System.ComponentModel.ListChangedEventArgs e)
		{
			if(Dispatcher != null && System.Threading.Thread.CurrentThread != Dispatcher.Thread)
				try
				{
					Action(this, e);
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					if(ex.InnerException != null)
						throw ex.InnerException;
					throw;
				}
				catch(Exception ex)
				{
					if(!(ex is NotSupportedException && ex.Source == "PresentationFramework"))
						throw;
				}
			else
				Action(sender, e);
		}
	}
}
