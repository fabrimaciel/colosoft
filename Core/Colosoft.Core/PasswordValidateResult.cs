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
	/// Resultado da validação de senha, checa se a senha possui os requisitos mínimos de segurança
	/// </summary>
	public class PasswordValidateResult
	{
		/// <summary>
		/// Construtor da classe
		/// </summary>
		/// <param name="isOk">Indica se a senha está ou não no padrão</param>
		/// <param name="message">Mensagem a ser apresentada para o usuário ao criar/altera senha</param>
		/// <param name="strength">Indica a força de segurança da senha</param>
		public PasswordValidateResult(bool isOk = true, string message = null, PasswordStrength strength = PasswordStrength.None)
		{
			IsOk = isOk;
			Message = message ?? String.Empty;
			Strength = strength;
		}

		/// <summary>
		/// Indica se a senha está ou não no padrão
		/// </summary>
		public bool IsOk
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem a ser apresentada para o usuário ao criar/altera senha
		/// </summary>
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Indica a força de segurança da senha
		/// </summary>
		public PasswordStrength Strength
		{
			get;
			set;
		}
	}
}
