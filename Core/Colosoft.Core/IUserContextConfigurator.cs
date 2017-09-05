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
	/// Assinatura de um configurador do contexto do usuário.
	/// </summary>
	public interface IUserContextConfigurator
	{
		/// <summary>
		/// Recupera ao principal para o contexto do usuário.
		/// </summary>
		/// <returns></returns>
		Colosoft.Security.Principal.DefaultPrincipal GetPrincipal();

		/// <summary>
		/// Configura o contexto do usuário.
		/// </summary>
		/// <param name="userContext">Instancia do contexto que deve ser configurada.</param>
		void Configure(UserContext userContext);
	}
}
