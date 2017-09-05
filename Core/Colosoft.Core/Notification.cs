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

namespace Colosoft.Notifications
{
	/// <summary>
	/// Classe responsável por gerenciar as mensagens do sistema.
	/// </summary>
	public static class Notification
	{
		/// <summary>
		/// Instancia do dispacher que será 
		/// </summary>
		private static INotificationDispatcher _dispacher;

		private static object _objLock = new object();

		/// <summary>
		/// Recupera a instancia padrão do dispachante de notificações do sistema.
		/// </summary>
		public static INotificationDispatcher Dispatcher
		{
			get
			{
				if(_dispacher == null)
					lock (_objLock)
					{
						if(_dispacher != null)
							return _dispacher;
						try
						{
							_dispacher = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<INotificationDispatcher>();
						}
						catch(Exception ex)
						{
							throw new InvalidOperationException(Properties.Resources.InvalidOperation_FailOnLoadNotificationDispatcher, ex);
						}
					}
				return _dispacher;
			}
		}

		/// <summary>
		/// Método de desempenha o evento Dispached.
		/// </summary>
		/// <param name="notification"></param>
		/// <returns></returns>
		private static MessageResult OnDispatched(NotificationInfo notification)
		{
			return Dispatcher.Dispatch(notification);
		}

		/// <summary>
		/// Despacha uma mensagem de notificação.
		/// </summary>
		/// <param name="message">Mensagem que será enviada.</param>
		/// <returns></returns>
		public static MessageResult Dispatch(IMessageFormattable message)
		{
			return OnDispatched(new NotificationInfo(message));
		}

		/// <summary>
		/// Despacha uma mensagem de notificação.
		/// </summary>
		/// <param name="message">Mensagem que será enviada.</param>
		/// <param name="type">Tipo da notificação.</param>
		/// <returns></returns>
		public static MessageResult Dispatch(IMessageFormattable message, NotificationType type)
		{
			return OnDispatched(new NotificationInfo(message, type: type));
		}

		/// <summary>
		///  Despacha uma mensagem de notificação.
		/// </summary>
		/// <param name="message">Mensagem que será enviada.</param>
		/// <param name="caption">Título da notificação.</param>
		/// <returns></returns>
		public static MessageResult Dispatch(IMessageFormattable message, IMessageFormattable caption)
		{
			return OnDispatched(new NotificationInfo(message, caption));
		}

		/// <summary>
		/// Despacha uma mensagem de notificação.
		/// </summary>
		/// <param name="message">Mensagem que será enviada.</param>
		/// <param name="caption">Título da notificação.</param>
		/// <param name="resultOption">Opção do resultado da mensagem.</param>
		/// <returns></returns>
		public static MessageResult Dispatch(IMessageFormattable message, IMessageFormattable caption, MessageResultOption resultOption)
		{
			return OnDispatched(new NotificationInfo(message, caption, resultOption));
		}

		/// <summary>
		/// Despacha uma mensagem de notificação
		/// </summary>
		/// <param name="message">Mensagem que será enviada.</param>
		/// <param name="caption">Título da notificação.</param>
		/// <param name="resultOption">Opção do resultado da mensagem.</param>
		/// <param name="type">Tipo da notificação.</param>
		/// <param name="dafaultMessageResult">Resultado padrão.</param>
		/// <returns></returns>
		public static MessageResult Dispatch(IMessageFormattable message, IMessageFormattable caption, MessageResultOption resultOption, NotificationType type, MessageResult dafaultMessageResult)
		{
			return OnDispatched(new NotificationInfo(message, caption, resultOption, dafaultMessageResult, type));
		}
	}
}
