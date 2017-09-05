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

namespace Colosoft.Web.Security
{
	/// <summary>
	/// Implementação do módulo responsável pela seguração das
	/// requisições Web.
	/// </summary>
	public sealed class SecurityModule : System.Web.IHttpModule
	{
		/// <summary>
		/// Método acionado para inicializar o módulo.
		/// </summary>
		/// <param name="context"></param>
		public void Init(System.Web.HttpApplication context)
		{
			context.AuthenticateRequest += OnAuthenticateRequest;
			context.BeginRequest += OnBeginRequest;
		}

		/// <summary>
		/// Método acionado quando for recebida uma requisição no servidor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBeginRequest(object sender, EventArgs e)
		{
			var application = sender as System.Web.HttpApplication;
		}

		/// <summary>
		/// Método acionado quando for necessário autenticar a requisição.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAuthenticateRequest(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
