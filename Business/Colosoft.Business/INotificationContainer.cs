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

namespace Colosoft.Business
{
	/// <summary>
	/// Possível níveis para realizar o despacho das notificações.
	/// </summary>
	public enum DispatchNotificationLevel
	{
		/// <summary>
		/// Despachar mensagens apenas do primeiro nível.
		/// </summary>
		OneLevel,
		/// <summary>
		/// Despachar mensagens de todos os nível;
		/// </summary>
		All
	}
	/// <summary>
	/// Assinatura das classe que contém notificações.
	/// </summary>
	public interface INotificationContainer
	{
		/// <summary>
		/// Despacha as notificações associadas com a instancia.
		/// </summary>
		/// <param name="dispatcher">Instancia responsável por despachar as notificações.</param>
		/// <param name="level">Nível no qual as mensagens serão despachadas.</param>
		void DispatchNotifications(Colosoft.Notifications.INotificationDispatcher dispatcher, DispatchNotificationLevel level);
	}
}
