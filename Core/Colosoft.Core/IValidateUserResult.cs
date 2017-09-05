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

namespace Colosoft.Security
{
	/// <summary>
	/// Assinatura das classes responsáveis pelo resultado da validação do usuário.
	/// </summary>
	public interface IValidateUserResult
	{
		/// <summary>
		/// Situação da autenticação.
		/// </summary>
		Colosoft.Security.AuthenticationStatus Status
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem a ser apresentada
		/// </summary>
		string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Usuário autenticado em caso de sucesso
		/// </summary>
		Colosoft.Security.IUser User
		{
			get;
			set;
		}

		/// <summary>
		/// Indica que a próxima tentativa de login deverá conter o captcha
		/// </summary>
		Colosoft.Security.CaptchaSupport.CaptchaInfo Captcha
		{
			get;
			set;
		}

		/// <summary>
		/// Data em que o password irá expirar
		/// </summary>
		DateTimeOffset? ExpireDate
		{
			get;
			set;
		}

		/// <summary>
		/// Token que identifica a autenticação
		/// </summary>
		string Token
		{
			get;
			set;
		}

		/// <summary>
		/// Indica que é um processo
		/// </summary>
		bool IsProcess
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Assinatura do resultado da validação do usuário que possui detalhes do erro ocorrido.
	/// </summary>
	public interface IValidateUserResultError : IValidateUserResult
	{
		/// <summary>
		/// Error associado.
		/// </summary>
		Exception Error
		{
			get;
		}
	}
}
