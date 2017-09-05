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
	/// Armazena as informações de um notificação.
	/// </summary>
	public class NotificationInfo
	{
		/// <summary>
		/// Mensagem da notificação.
		/// </summary>
		public IMessageFormattable Message
		{
			get;
			set;
		}

		/// <summary>
		/// Legenda da notificação.
		/// </summary>
		public IMessageFormattable Caption
		{
			get;
			set;
		}

		/// <summary>
		/// Resultado padrão da notificação.
		/// </summary>
		public MessageResult DefaultMessageResult
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo da notificação.
		/// </summary>
		public NotificationType Type
		{
			get;
			set;
		}

		/// <summary>
		/// Opção do resultado da mensagem.
		/// </summary>
		public MessageResultOption Option
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="option"></param>
		/// <param name="caption"></param>
		/// <param name="defaultMessageResult"></param>
		/// <param name="type"></param>
		public NotificationInfo(IMessageFormattable message, IMessageFormattable caption = null, MessageResultOption option = MessageResultOption.OK, MessageResult defaultMessageResult = MessageResult.None, NotificationType type = NotificationType.None)
		{
			if(message == null)
				throw new ArgumentNullException("message");
			Message = message;
			Caption = caption;
			Option = option;
			DefaultMessageResult = defaultMessageResult;
			Type = type;
		}
	}
}
