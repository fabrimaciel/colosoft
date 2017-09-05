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
using System.Xml.Serialization;
using Colosoft.Security.Profile;

namespace Colosoft.Security.Remote.Client.ProfileProviderServiceReference
{
    /// <summary>
    /// Implementação de um perfil do sistema.
    /// </summary>
    [Serializable]
    public sealed class Profile : IXmlSerializable
    {
        #region Properties

        /// <summary>
        /// Identificador do prefil
        /// </summary>
        public int ProfileId { get; set; }

        /// <summary>
        /// Nome do perfíl
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Identifica se o perfil é para um usuário anonimo.
        /// </summary>
        public bool IsAnonymous { get; set; }

        /// <summary>
        /// Modo de pesquisa associado.
        /// </summary>
        public ProfileSearchMode SearchMode { get; set; }

        /// <summary>
        /// Recupera a data da ultima atividade realizada sobre o perfil.
        /// </summary>
        public DateTimeOffset LastActivityDate { get; set; }

        /// <summary>
        /// Recupera a data da ultima alteração realizada sobre o perfil.
        /// </summary>
        public DateTimeOffset LastUpdatedDate { get; set; }

        /// <summary>
        /// Nome do usuário associado com o perfil.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Origem da autenticação.
        /// </summary>
        public AuthenticationSource Source { get; set; }

        /// <summary>
        /// Conjunto dos papéis e permissões do perfil.
        /// </summary>
        public ProfileRoleSet RoleSet { get; set; }

        /// <summary>
        /// Propriedades do perfil.
        /// </summary>
        public ProfileProperty[] Properties { get; set; }

        /// <summary>
        /// Identificador da origem
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// Identificador do usuário
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Identificador do grupo de regras
        /// </summary>
        public int GroupRoleId { get; set; }

        /// <summary>
        /// Identificador do grupo de regras padrão
        /// </summary>
        public int DefaultValueGroupId { get; set; }

        /// <summary>
        /// Grupo de marcadores do perfil.
        /// </summary>
        public int? MarkGroupId { get; set; }

        /// <summary>
        /// Identificador da árvore de vendedores associada.
        /// </summary>
        public int? SellerTreeId { get; set; }

        /// <summary>
        /// Identificador do intermediador associado.
        /// </summary>
        public int? IntermediateId { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Profile()
        {

        }

        #endregion

        #region IXmlSerializable Members

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lê os dados do xml para a instancia.
        /// </summary>
        /// <param name="reader"></param>
        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            DefaultValueGroupId = int.Parse(reader.ReadElementString("DefaultValueGroupId"));
            FullName = reader.ReadElementString("FullName");
            GroupRoleId = int.Parse(reader.ReadElementString("GroupRoleId"));
            IsAnonymous = reader.ReadElementContentAsBoolean();

            ProfileSearchMode searchMode = ProfileSearchMode.All;

            if (Enum.TryParse<ProfileSearchMode>(reader.ReadElementString("SearchMode"), out searchMode))
                SearchMode = searchMode;

            LastActivityDate = reader.ReadDateTimeOffset();
            LastUpdatedDate = reader.ReadDateTimeOffset();

            ProfileId = reader.ReadElementContentAsInt();

            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement("Properties");

                var properties = new List<ProfileProperty>();
                var serializer = new XmlSerializer(typeof(ProfileProperty), new XmlRootAttribute("ProfileProperty"));

                while (reader.LocalName == "ProfileProperty" &&
                       reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    properties.Add((ProfileProperty)serializer.Deserialize(reader));
                }

                this.Properties = properties.ToArray();
                reader.ReadEndElement();
            }
            else
                reader.Skip();

            if (!reader.IsEmptyElement)
            {
                RoleSet = new ProfileRoleSet();
                ((IXmlSerializable)RoleSet).ReadXml(reader);
            }
            else
                reader.Skip();

            if (!reader.IsEmptyElement)
            {
                Source = new AuthenticationSource();
                ((IXmlSerializable)Source).ReadXml(reader);
            }
            else
                reader.Skip();

            SourceId = reader.ReadElementContentAsInt();
            UserId = reader.ReadElementContentAsInt();
            UserName = reader.ReadElementString("UserName");

            // MarkGroupId
            if ( !reader.IsEmptyElement )
                MarkGroupId = reader.ReadElementContentAsInt();
            else
                reader.Skip();

            // SellerTreeId
            if (!reader.IsEmptyElement)
                SellerTreeId = reader.ReadElementContentAsInt();
            else
                reader.Skip();

            // IntermediateId
            if (!reader.IsEmptyElement)
                IntermediateId = reader.ReadElementContentAsInt();
            else
                reader.Skip();

            reader.ReadEndElement();
        }

        /// <summary>
        /// Escreve os dados da instancia no xml.
        /// </summary>
        /// <param name="writer"></param>
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("DefaultValueGroupId", DefaultValueGroupId.ToString());
            writer.WriteElementString("FullName", FullName);
            writer.WriteElementString("GroupRoleId", GroupRoleId.ToString());

            writer.WriteStartElement("IsAnonymous");
            writer.WriteValue(IsAnonymous);
            writer.WriteEndElement();

            writer.WriteElementString("SearchMode", SearchMode.ToString());

            writer.WriteDateTimeOffset(LastActivityDate, "LastActivityDate");
            writer.WriteDateTimeOffset(LastUpdatedDate, "LastUpdatedDate");
            writer.WriteElementString("ProfileId", ProfileId.ToString());


            writer.WriteStartElement("Properties");
            if (Properties != null)
            {
                var serializer = new XmlSerializer(typeof(ProfileProperty), new XmlRootAttribute("ProfileProperty"));
                foreach (IXmlSerializable i in Properties)
                    serializer.Serialize(writer, i);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("RoleSet");
            if (RoleSet != null)
                ((IXmlSerializable)RoleSet).WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("Source");
            if (Source != null)
                ((IXmlSerializable)Source).WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteElementString("SourceId", SourceId.ToString());
            writer.WriteElementString("UserId", UserId.ToString());
            writer.WriteElementString("UserName", UserName);
            writer.WriteStartElement("MarkGroupId");
            if (MarkGroupId.HasValue)
                writer.WriteValue(MarkGroupId.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("SellerTreeId");
            if (SellerTreeId.HasValue) writer.WriteValue(SellerTreeId.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("IntermediateId");
            if (IntermediateId.HasValue) writer.WriteValue(IntermediateId.Value);
            writer.WriteEndElement();
        }

        #endregion
    }
}
