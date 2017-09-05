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
	/// Classe com método para tratar os erros ocorridos no sistema.
	/// </summary>
	public static class Error
	{
		private static Func<IExceptionNotification> _exceptionNotificationFactory;

		/// <summary>
		/// Instancia do método usado para criar o Notification dos exceptions.
		/// </summary>
		public static Func<IExceptionNotification> ExceptionNotificationFactory
		{
			get
			{
				return _exceptionNotificationFactory;
			}
			set
			{
				_exceptionNotificationFactory = value;
			}
		}

		/// <summary>
		/// Cria uma notificação de erro do sistema.
		/// </summary>
		/// <returns></returns>
		private static IExceptionNotification CreateNotification()
		{
			if(_exceptionNotificationFactory != null)
				return _exceptionNotificationFactory();
			return new ExceptionNotification();
		}

		/// <summary>
		/// Exibe a mensagem do erro ocorrido.
		/// </summary>
		/// <param name="exception"></param>
		public static void Show(Exception exception)
		{
			exception.Require("exception").NotNull();
			var notification = CreateNotification();
			IMessageFormattable message = null;
			if(exception is DetailsException)
				message = ((DetailsException)exception).MessageFormattable;
			else
				message = exception.Message.GetFormatter();
			if(notification != null)
			{
				notification.Configure(message, exception);
				notification.ShowDialog();
			}
		}

		/// <summary>
		/// Exibe a mensagem o erro informado.
		/// </summary>
		/// <param name="message">Mensagem do erro.</param>
		/// <param name="exception">Instancia do erro ocorrido.</param>
		public static void Show(IMessageFormattable message, Exception exception)
		{
			var notification = CreateNotification();
			if(notification != null)
			{
				notification.Configure(message, exception);
				notification.ShowDialog();
			}
		}
	}
}
