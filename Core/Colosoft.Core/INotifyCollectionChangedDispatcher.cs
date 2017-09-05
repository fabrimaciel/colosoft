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
	/// Possíveis prioridades usadas pelo log.
	/// </summary>
	public enum NotifyCollectionChangedDispatcherPriority
	{
		/// <summary>
		/// Entrada de baixa prioridade.
		/// </summary>
		Low = 1,
		/// <summary>
		/// Entrada de prioridade normal.
		/// </summary>
		Normal = 2,
		/// <summary>
		/// Entrada de alta prioridade.
		/// </summary>
		High = 3
	}
	/// <summary>
	/// Assinatura das classes que disparam as nodificações
	/// de alteração da coleção.
	/// </summary>
	public interface INotifyCollectionChangedDispatcher : System.Collections.Specialized.INotifyCollectionChanged
	{
		/// <summary>
		/// Adiciona o evento que será acionado quando a coleção for alterada.
		/// </summary>
		/// <param name="eventHandler"></param>
		/// <param name="priority"></param>
		void AddCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventHandler eventHandler, NotifyCollectionChangedDispatcherPriority priority);

		/// <summary>
		/// Remove o evento registrado para ser acionado quando a coleção for alterada.
		/// </summary>
		/// <param name="eventHandler"></param>
		void RemoveCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventHandler eventHandler);
	}
}
