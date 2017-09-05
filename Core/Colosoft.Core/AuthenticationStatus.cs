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
	/// Enumerador dos tipos de retorno de autenticação
	/// </summary>
	public enum AuthenticationStatus
	{
		/// <summary>
		/// Credenciais válidas
		/// </summary>
		Success,
		/// <summary>
		/// Nome de usuário ou senha incorreta
		/// </summary>
		InvalidUserNameOrPassword,
		/// <summary>
		/// Domínio inválido
		/// </summary>
		InvalidDomain,
		/// <summary>
		/// Senha expirada
		/// </summary>
		PasswordExpired,
		/// <summary>
		/// Credenciais válidas porém com alerta
		/// </summary>
		PasswordWarning,
		/// <summary>
		/// Ocorreu algum erro durante a autenticação
		/// </summary>
		ErrorInValidate,
		/// <summary>
		/// Erro pois autenticação requer captcha
		/// </summary>
		CaptchaRequired,
		/// <summary>
		/// Informações do captcha incorretas
		/// </summary>
		InvalidCaptcha,
		/// <summary>
		/// Erro ao criar controle de token
		/// </summary>
		ErrorTokenControl,
		/// <summary>
		/// Se já existe um token aberto para o usuário em questão
		/// </summary>
		DuplicateToken,
		/// <summary>
		/// Identifica erro na comunicação com o servidor de autenticação.
		/// </summary>
		ErrorInCommunication,
		/// <summary>
		/// Identifica um erro desconhecido.
		/// </summary>
		UnknownError
	}
}
