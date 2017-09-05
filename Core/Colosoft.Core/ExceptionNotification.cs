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

namespace Colosoft.Diagnostics
{
	/// <summary>
	/// Implementação base para a notificação de um erro.
	/// </summary>
	class ExceptionNotification : IExceptionNotification
	{
		private IMessageFormattable _message;

		private Exception _exception;

		/// <summary>
		/// Configura a notificação do erro.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		public void Configure(IMessageFormattable message, Exception exception)
		{
			_message = message;
			_exception = exception;
		}

		/// <summary>
		/// Exibe o dialogo com o erro ocorrido.
		/// </summary>
		/// <returns></returns>
		public bool? ShowDialog()
		{
			Notifications.Notification.Dispatch(_exception != null ? new Text.JoinMessageFormattable(_message, "\r\n", ExceptionFormatter.FormatException(_exception, true).GetFormatter()) : _message, "Error".GetFormatter(), Notifications.MessageResultOption.OK, Notifications.NotificationType.Error, Notifications.MessageResult.OK);
			return true;
		}
	}
}
