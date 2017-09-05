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

namespace Colosoft.Security.Remote.Server
{
	/// <summary>
	/// Armazena as informações do provedor do usuário.
	/// </summary>
	public class UserProviderInfo
	{
		/// <summary>
		/// Identifica se está habilitado resetar a senha.
		/// </summary>
		public bool EnablePasswordReset
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a recuperação de senha está habilitada.
		/// </summary>
		public bool EnablePasswordRetrieval
		{
			get;
			set;
		}

		/// <summary>
		/// Número máximo de senhas inválidas.
		/// </summary>
		public int MaxInvalidPasswordAttempts
		{
			get;
			set;
		}

		/// <summary>
		/// Quantidade minima de caracteres não alfanuméricos.
		/// </summary>
		public int MinRequiredNonAlphanumericCharacters
		{
			get;
			set;
		}

		/// <summary>
		/// Comprimento minimo requerido para a senha.
		/// </summary>
		public int MinRequiredPasswordLength
		{
			get;
			set;
		}

		/// <summary>
		/// Formato da senha.
		/// </summary>
		public PasswordFormat PasswordFormat
		{
			get;
			set;
		}

		/// <summary>
		/// Expressão regular usada para varifica se a senha é forte.
		/// </summary>
		public string PasswordStrengthRegularExpression
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se é requerido pergunta e resposta.
		/// </summary>
		public bool RequiresQuestionAndAnswer
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public UserProviderInfo()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public UserProviderInfo(IUserProvider userProvider)
		{
			if(userProvider == null)
				throw new ArgumentNullException("userProvider");
			this.EnablePasswordReset = userProvider.EnablePasswordReset;
			this.EnablePasswordRetrieval = userProvider.EnablePasswordRetrieval;
			this.MaxInvalidPasswordAttempts = userProvider.MaxInvalidPasswordAttempts;
			this.MinRequiredNonAlphanumericCharacters = userProvider.MinRequiredNonAlphanumericCharacters;
			this.MinRequiredPasswordLength = userProvider.MinRequiredPasswordLength;
			this.PasswordFormat = userProvider.PasswordFormat;
			this.PasswordStrengthRegularExpression = userProvider.PasswordStrengthRegularExpression;
			this.RequiresQuestionAndAnswer = userProvider.RequiresQuestionAndAnswer;
		}
	}
}
