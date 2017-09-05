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
using System.Threading.Tasks;

namespace Colosoft.Web
{
	/// <summary>
	/// Assinatura de uma aplicação WEB.
	/// </summary>
	interface IHttpApplication
	{
		/// <summary>
		/// Evento acionado quando a autenticação for requisitada.
		/// </summary>
		event EventHandler AuthenticateRequest;

		/// <summary>
		/// Evento acionado no início da requisição.
		/// </summary>
		event EventHandler BeginRequest;

		/// <summary>
		/// Evento acionado no fim da requisição.
		/// </summary>
		event EventHandler EndRequest;

		/// <summary>
		/// Contexto associado.
		/// </summary>
		Colosoft.Owin.Server.HttpContext Context
		{
			get;
		}

		/// <summary>
		/// Método acionado quando for solicita a autenticação na requisição.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnAuthenticateRequest(object sender, EventArgs e);

		/// <summary>
		/// Método acionado quando começar a requisição. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnBeginRequest(object sender, EventArgs e);

		/// <summary>
		/// Método acionado quando a requisição for finalizada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnEndRequest(object sender, EventArgs e);
	}
}
