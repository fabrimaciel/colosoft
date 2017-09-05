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

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Representa os argumentos do evento acionado quando a atualização for
	/// finalizada.
	/// </summary>
	public class ApplicationLoaderCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
	{
		private bool _success = false;

		private IMessageFormattable _message;

		/// <summary>
		/// Identifica se a operação foi realizada com sucesso.
		/// </summary>
		public bool Success
		{
			get
			{
				return _success;
			}
		}

		/// <summary>
		/// Mensagem associada.
		/// </summary>
		public IMessageFormattable Message
		{
			get
			{
				return _message;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="error">Error na carregamento, caso tenha tenha ocorrido.</param>
		/// <param name="cancelled">True para identificar que a carga foi cancelada.</param>
		/// <param name="userState">Instancia com o estado do usuário.</param>
		/// <param name="message">Mensagem associada.</param>
		/// <param name="success">Identifica se a sincronização foi finalizada com sucesso.</param>
		public ApplicationLoaderCompletedEventArgs(Exception error, bool cancelled, object userState, IMessageFormattable message, bool success) : base(error, cancelled, userState)
		{
			_message = message;
			_success = success;
		}
	}
}
