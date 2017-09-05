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
using System.Configuration;

namespace Colosoft.Security.Profile
{
	/// <summary>
	/// Assinatura de um perfil do sistema.
	/// </summary>
	public interface IProfile
	{
		/// <summary>
		/// Identificador do perfil.
		/// </summary>
		int ProfileId
		{
			get;
		}

		/// <summary>
		/// Nome do perfil
		/// </summary>
		string FullName
		{
			get;
		}

		/// <summary>
		/// Identifica se o perfil é para um usuário anonimo.
		/// </summary>
		bool IsAnonymous
		{
			get;
		}

		/// <summary>
		/// Recupera a data da ultima atividade realizada sobre o perfil.
		/// </summary>
		DateTimeOffset LastActivityDate
		{
			get;
		}

		/// <summary>
		/// Recupera a data da ultima alteração realizada sobre o perfil.
		/// </summary>
		DateTimeOffset LastUpdatedDate
		{
			get;
		}

		/// <summary>
		/// Nome do usuário associado com o perfil.
		/// </summary>
		string UserName
		{
			get;
		}

		/// <summary>
		/// Modo de pesquisa associado com o perfil.
		/// </summary>
		ProfileSearchMode SearchMode
		{
			get;
		}

		/// <summary>
		/// Origem da autenticação.
		/// </summary>
		IAuthenticationSource Source
		{
			get;
		}

		/// <summary>
		/// Conjunto dos papéis e permissões do perfil.
		/// </summary>
		ProfileRoleSet RoleSet
		{
			get;
		}

		/// <summary>
		/// Recupera e define o valor de uma propriedade do perfil.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		object this[string propertyName]
		{
			get;
			set;
		}

		/// <summary>
		/// Propriedades do perfil.
		/// </summary>
		IEnumerable<ProfileProperty> Properties
		{
			get;
		}

		/// <summary>
		/// Grupo de marcadores associado ao perfil.
		/// </summary>
		int? MarkGroupId
		{
			get;
		}

		/// <summary>
		/// Identificador da árvore de vendedores associada com o perfil.
		/// </summary>
		int? SellerTreeId
		{
			get;
		}

		/// <summary>
		/// Identificador do intermediador associado com o perfil.
		/// </summary>
		int? IntermediateId
		{
			get;
		}

		/// <summary>
		/// Recupera o valor da propriedade.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		object GetPropertyValue(string propertyName);

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propertyValue"></param>
		void SetPropertyValue(string propertyName, object propertyValue);
	}
}
