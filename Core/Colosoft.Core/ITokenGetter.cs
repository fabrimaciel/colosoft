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
	/// Assinatura das classe que fornecem
	/// uma form de recupera o token da atual instancia da aplicação.
	/// </summary>
	public interface ITokenGetter
	{
		/// <summary>
		/// Recupera o token.
		/// </summary>
		/// <returns></returns>
		string Get();
	}
	/// <summary>
	/// Implementação padrão do recuperador de token.
	/// </summary>
	public class DefaultTokenGetter : ITokenGetter
	{
		/// <summary>
		/// Recupera o token
		/// </summary>
		/// <returns></returns>
		public string Get()
		{
			var principal = System.Threading.Thread.CurrentPrincipal;
			if(principal != null && principal.Identity is Security.Principal.DefaultIdentity)
			{
				var identity = (Security.Principal.DefaultIdentity)principal.Identity;
				return identity.Token;
			}
			return null;
		}
	}
}
