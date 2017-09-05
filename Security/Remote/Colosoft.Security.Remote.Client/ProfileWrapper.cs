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
using Colosoft.Security.Profile;

namespace Colosoft.Security.Remote.Client.Wrappers
{
    /// <summary>
    /// Adaptação para receber os dados do perfil pelo serviço.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), Serializable]
    class ProfileWrapper : IProfile //, System.Runtime.Serialization.ISerializable
    {
        #region Local Variables

        private ProfileProviderServiceReference.Profile _instance;

        private ProfileRoleSet _roleSet;
        private Dictionary<string, object> _properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, ProfilePropertyDefinition> _propertyDefinitions;
        private IAuthenticationSource _source;
        [NonSerialized]
        private RemoteProfileProvider _profileProvider;

        #endregion

        #region Properties

        /// <summary>
        /// Identificador do perfil.
        /// </summary>
        public int ProfileId
        {
            get { return _instance.ProfileId; }
        }

        /// <summary>
        /// Nome do perfil.
        /// </summary>
        public string FullName
        {
            get { return _instance.FullName; }
        }

        /// <summary>
        /// Identifica se é um perfil de usuário anonimo.
        /// </summary>
        public bool IsAnonymous
        {
            get { return _instance.IsAnonymous; }
        }

        /// <summary>
        /// Modo de pesquisa associado.
        /// </summary>
        public ProfileSearchMode SearchMode
        {
            get { return _instance.SearchMode; }
        }

        /// <summary>
        /// Data da ultima atividade no perfil.
        /// </summary>
        public DateTimeOffset LastActivityDate
        {
            get { return _instance.LastActivityDate; }
        }

        /// <summary>
        /// Data da última atualização.
        /// </summary>
        public DateTimeOffset LastUpdatedDate
        {
            get { return _instance.LastUpdatedDate; }
        }

        /// <summary>
        /// Grupo de marcadores do perfil.
        /// </summary>
        public int? MarkGroupId
        {
            get { return _instance.MarkGroupId; }
        }

        /// <summary>
        /// Identificador da árvore de vendedores associado.
        /// </summary>
        public int? SellerTreeId
        {
            get { return _instance.SellerTreeId; }
        }

        /// <summary>
        /// Identificador do intermediador associado.
        /// </summary>
        public int? IntermediateId
        {
            get { return _instance.IntermediateId; }
        }

        /// <summary>
        /// Enumerador da propriedades do perfil.
        /// </summary>
        public IEnumerable<ProfileProperty> Properties
        {
            get 
            {
                foreach (var i in PropertyDefinitions)
                {
                    object value = null;

                    lock (_properties)
                        _properties.TryGetValue(i.Key, out value);

                    yield return new ProfileProperty
                    {
                        Name = i.Value.Name,
                        Description = i.Value.Description,
                        TypeDefinition = i.Value.TypeDefinition,
                        Value = value
                    };
                }
            }
        }

        /// <summary>
        /// Conjunto de papeis do perfil.
        /// </summary>
        public ProfileRoleSet RoleSet
        {
            get { return _roleSet; }
        }

        /// <summary>
        /// Origem da autenticação.
        /// </summary>
        public IAuthenticationSource Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Usuário dono do perfil.
        /// </summary>
        public string UserName
        {
            get { return _instance.UserName; }
        }

        /// <summary>
        /// Recupera uma propriedade do pergil com base no nome informado.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object this[string propertyName]
        {
            get
            {
                return GetPropertyValue(propertyName);
            }
            set
            {
                SetPropertyValue(propertyName, value);
            }
        }

        /// <summary>
        /// Definições das propriedades dos perfis do sistema
        /// </summary>
        public Dictionary<string, ProfilePropertyDefinition> PropertyDefinitions
        {
            get
            {
                if (_propertyDefinitions == null)
                {
                    _propertyDefinitions = new Dictionary<string,ProfilePropertyDefinition>(StringComparer.OrdinalIgnoreCase);
                    foreach(var i in ProfileProvider.GetProfilePropertyDefinitions())
                        _propertyDefinitions.Add(i.Name, i);
                }

                return _propertyDefinitions;
            }
        }

        /// <summary>
        /// Instancia do provedor de perfil remoto.
        /// </summary>
        public RemoteProfileProvider ProfileProvider
        {
            get 
            {
                if (_profileProvider == null)
                    _profileProvider = new RemoteProfileProvider();

                return _profileProvider; 
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="profileProvider"></param>
        public ProfileWrapper(ProfileProviderServiceReference.Profile profile, RemoteProfileProvider profileProvider)
        {
            _profileProvider = profileProvider;
            _instance = profile;
            _roleSet = profile.RoleSet ?? new ProfileRoleSet();

            if (_instance != null)
                _instance.RoleSet = _roleSet;

            //profile.RoleSet = null;

            if (profile.Source != null)
                _source = new AuthenticationSourceWrapper(profile.Source);

            if (profile.Properties != null)
                foreach (var property in profile.Properties)
                    if (property != null)
                        _properties.Add(property.Name, property.Value);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Recupera o valor da propriedade do perfil.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetPropertyValue(string propertyName)
        {
            object value = null;

            if (_properties.TryGetValue(propertyName, out value))
                return value;

            return null;
        }

        /// <summary>
        /// Define o valor da propriedade do perfil.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void SetPropertyValue(string propertyName, object propertyValue)
        {
            ProfilePropertyDefinition propertyDefinition = null;

            if (!PropertyDefinitions.TryGetValue(propertyName, out propertyDefinition))
                throw new InvalidOperationException(string.Format("Not found property '{0}' for profile", propertyName));

            string value = null;

            if (propertyValue != null)
            {
                if (propertyValue is IConvertible)
                    value = ((IConvertible)propertyValue).ToString(System.Globalization.CultureInfo.InvariantCulture);
                else
                    value = propertyValue.ToString();
            }

            ProfileProvider.SetProfilePropertyValue(this, propertyDefinition, value);

            lock (_properties)
                if (_properties.ContainsKey(propertyDefinition.Name))
                    _properties[propertyDefinition.Name] = propertyValue;
                else
                    _properties.Add(propertyDefinition.Name, propertyValue);
        }

        #endregion
    }
}
