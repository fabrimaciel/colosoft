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

namespace Colosoft.Security.Permissions
{
	/// <summary>
	/// Armazena os dados do resultado de uma verificação de permissão.
	/// </summary>
	public class CheckPermissionResult
	{
		private bool _success;

		private IMessageFormattable _message;

		private IMessageFormattable _title;

		/// <summary>
		/// Identifi
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
		/// Titulo da mensagem.
		/// </summary>
		public IMessageFormattable Title
		{
			get
			{
				return _title;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="success"></param>
		/// <param name="message"></param>
		/// <param name="title"></param>
		public CheckPermissionResult(bool success, IMessageFormattable message = null, IMessageFormattable title = null)
		{
			_success = success;
			_message = message ?? MessageFormattable.Empty;
			_title = title ?? MessageFormattable.Empty;
		}

		/// <summary>
		/// Converte implicitamente para um Boolean.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator bool(CheckPermissionResult value)
		{
			if(value == null)
				return false;
			return value.Success;
		}
	}
}
