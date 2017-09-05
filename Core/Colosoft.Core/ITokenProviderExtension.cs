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

namespace Colosoft.Security
{
	/// <summary>
	/// Armazena os argumento de quando um token é inserido.
	/// </summary>
	public class TokenInsertedEventArgs : EventArgs
	{
		private string _token;

		private int _userId;

		/// <summary>
		/// Token inserido.
		/// </summary>
		public string Token
		{
			get
			{
				return _token;
			}
		}

		/// <summary>
		/// Identificador do usuário associado ao token.
		/// </summary>
		public int UserId
		{
			get
			{
				return _userId;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="userId"></param>
		public TokenInsertedEventArgs(string token, int userId)
		{
			_token = token;
			_userId = userId;
		}
	}
	/// <summary>
	/// Assinatura do evento acionado quando o token é inserido.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void TokenInsertedHandle (object sender, TokenInsertedEventArgs e);
	/// <summary>
	/// Instancia do provedor de token
	/// </summary>
	public interface ITokenProviderExtension
	{
		/// <summary>
		/// Evento acionado quando o token é inserido.
		/// </summary>
		event TokenInsertedHandle TokenInserted;
	}
}
