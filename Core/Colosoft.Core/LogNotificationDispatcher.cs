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
using Colosoft.Notifications;

namespace Colosoft.Logging
{
	/// <summary>
	/// Implementação do despachante de notificações para o log.
	/// </summary>
	public class LogNotificationDispatcher : INotificationDispatcher
	{
		/// <summary>
		/// <see cref="ILogger"/> associado com a instancia.
		/// </summary>
		public ILogger Logger
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="logger"></param>
		public LogNotificationDispatcher(ILogger logger)
		{
			logger.Require("logger").NotNull();
			Logger = logger;
		}

		/// <summary>
		/// Despacha a notificação.
		/// </summary>
		/// <param name="notification"></param>
		/// <returns></returns>
		public MessageResult Dispatch(NotificationInfo notification)
		{
			Log.Write(notification.Message, Category.Debug, Priority.None);
			if(notification.Option == MessageResultOption.OK)
				return MessageResult.OK;
			else if(notification.Option == MessageResultOption.OKCancel)
				return MessageResult.OK;
			else if(notification.Option == MessageResultOption.YesNo || notification.Option == MessageResultOption.YesNoCancel)
				return MessageResult.Yes;
			return MessageResult.None;
		}
	}
}
