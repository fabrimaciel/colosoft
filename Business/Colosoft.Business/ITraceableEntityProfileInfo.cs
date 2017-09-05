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

namespace Colosoft.Business
{
	/// <summary>
	/// Assinatura do provedor das informações do perfil associado com a entidade rastreável.
	/// </summary>
	public interface ITraceableEntityProfileInfoProvider
	{
		/// <summary>
		/// Recupera as informações do perfil associado com a entidade.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		ITraceableEntityProfileInfo GetProfileInfo(ISimpleTraceableEntity entity);
	}
	/// <summary>
	/// Assinatura das informações do perfil associado com a entidade rastreável.
	/// </summary>
	public interface ITraceableEntityProfileInfo
	{
		/// <summary>
		/// Identificador do perfil.
		/// </summary>
		int ProfileId
		{
			get;
		}

		/// <summary>
		/// Identificador do usuário associado com o perfil.
		/// </summary>
		int UserId
		{
			get;
		}

		/// <summary>
		/// Nome do perfil.
		/// </summary>
		string FullName
		{
			get;
		}

		/// <summary>
		/// Nome do usuário associado.
		/// </summary>
		string UserName
		{
			get;
		}

		/// <summary>
		/// Nome completo do usuário associado.
		/// </summary>
		string UserFullName
		{
			get;
		}
	}
}
