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
    /// Adaptação da classe ServerHost.User.
    /// </summary>
    [Serializable]
    class UserWrapper : IUser
    {
        #region Local Variables

        private ServerHost.User _instance;

        #endregion

        #region Properties

        /// <summary>
        /// Email associado.
        /// </summary>
        public string Email
        {
            get { return _instance.Email; }
        }

        /// <summary>
        /// Nome completo do usuário.
        /// </summary>
        public string FullName
        {
            get { return _instance.FullName; }
        }

        /// <summary>
        /// Identifica se o usuário está online.
        /// </summary>
        public bool IsOnline
        {
            get { return _instance.IsOnline; }
        }

        /// <summary>
        /// Identifica se o usuário foi aprovado.
        /// </summary>
        public bool IsApproved
        {
            get { return _instance.IsApproved; }
        }

        /// <summary>
        /// Nome do provedor de identidade do usuário.
        /// </summary>
        public string IdentityProvider
        {
            get { return _instance.IdentityProvider; }
        }

        /// <summary>
        /// Data da ultima atividade do usuário no sistema.
        /// </summary>
        public DateTimeOffset LastActivityDate
        {
            get { return _instance.LastActivityDate; }
        }

        /// <summary>
        /// Data do ultimo login.
        /// </summary>
        public DateTimeOffset LastLoginDate
        {
            get { return _instance.LastLoginDate; }
        }

        public DateTimeOffset LastPasswordChangedDate
        {
            get { return _instance.LastPasswordChangedDate; }
        }

        /// <summary>
        /// Questão usada para recuperação da senha.
        /// </summary>
        public string PasswordQuestion
        {
            get { return _instance.PasswordQuestion; }
        }

        /// <summary>
        /// Chave que identifica unicamente o usuário.
        /// </summary>
        public string UserKey
        {
            get { return _instance.UserKey; }
        }

        /// <summary>
        /// Chave que identifica unicamente o usuário.
        /// </summary>
        public string UserName
        {
            get { return _instance.UserName; }
        }

        /// <summary>
        /// Data de criação do usuário.
        /// </summary>
        public DateTimeOffset CreationDate
        {
            get { return _instance.CreationDate; }
        }

        /// <summary>
        /// Indica que o usuário deve ignorar o captcha do sistema
        /// </summary>
        public bool IgnoreCaptcha
        {
            get { return false; }
        }

        /// <summary>
        /// Identifica se o usuário está ativo.
        /// </summary>
        public bool IsActive
        {
            get { return _instance.IsActive; }
        }

        #endregion

        #region Constructors

        internal UserWrapper(ServerHost.User user)
        {
            _instance = user;
        }

        #endregion
    }
}
