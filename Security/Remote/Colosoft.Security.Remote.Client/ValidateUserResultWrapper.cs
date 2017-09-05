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

namespace Colosoft.Security.Remote.Client.Wrappers
{
    /// <summary>
    /// Adaptação para receber os dados do resultado da validação do usuário
    /// pelo serviço.
    /// </summary>
    [Serializable]
    public class ValidateUserResultWrapper : IValidateUserResultError
    {
        #region Properties

        /// <summary>
        /// Situação da autenticação.
        /// </summary>
        public AuthenticationStatus Status { get; set; }

        /// <summary>
        /// Mensagem a ser apresentada
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Usuário autenticado em caso de sucesso
        /// </summary>
        public IUser User { get; set; }

        /// <summary>
        /// Indica que a próxima tentativa de login deverá conter o captcha
        /// </summary>
        public Colosoft.Security.CaptchaSupport.CaptchaInfo Captcha { get; set; }

        /// <summary>
        /// Data em que o password irá expirar
        /// </summary>
        public DateTimeOffset? ExpireDate { get; set; }

        /// <summary>
        /// Token que identifica a autenticação
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Endereço do serviço do provedor do usuário.
        /// </summary>
        public Colosoft.Net.ServiceAddress UserProviderServiceAddress { get; set; }

        /// <summary>
        /// Endereço do serviço do provedor de perfil.
        /// </summary>
        public Colosoft.Net.ServiceAddress ProfileProviderServiceAddress { get; set; }

        /// <summary>
        /// Endereço do serviço do provedor de endereços de serviços.
        /// </summary>
        public Colosoft.Net.ServiceAddress ServiceAddressProviderServiceAddress { get; set; }

        /// <summary>
        /// Indica que a autenticação está em processo de reinicialização de senha
        /// </summary>
        public bool IsProcess { get; set; }

        /// <summary>
        /// Error associado.
        /// </summary>
        public Exception Error { get; set; }

        #endregion
    }
}
