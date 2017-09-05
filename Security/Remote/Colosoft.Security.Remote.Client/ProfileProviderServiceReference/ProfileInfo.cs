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

namespace Colosoft.Security.Remote.Client.ProfileProviderServiceReference
{
    /// <summary>
    /// Representa as informações do perfil.
    /// </summary>
    [System.Xml.Serialization.XmlRoot("ProfileInfo")]
    [System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
    [Serializable]
    public sealed class ProfileInfo : IXmlSerializable
    {
        #region Properties

        /// <summary>
        /// Identificador do perfil.
        /// </summary>
        public int ProfileId { get; set; }

        /// <summary>
        /// Nome do usuário do perfil
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Nome do perfil
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Modo de pesquisa associado.
        /// </summary>
        public Colosoft.Security.Profile.ProfileSearchMode SearchMode { get; set; }

        /// <summary>
        /// Data da ultima atividade com o perfil.
        /// </summary>
        public DateTimeOffset LastActivityDate { get; set; }

        /// <summary>
        /// Data da ultima vez que o perfil foi atualizado.
        /// </summary>
        public DateTimeOffset LastUpdatedDate { get; set; }

        /// <summary>
        /// Origem da autenticação.
        /// </summary>
        public AuthenticationSource Source { get; set; }

        /// <summary>
        /// Identifica se o usuário do perfil é anonimo.
        /// </summary>
        public bool IsAnonymous { get; set; }

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
        public ProfileInfo()
        {

        }

        /// <summary>
        /// Preenche a classe com os dados da instancia informada.
        /// </summary>
        /// <param name="info"></param>
        public ProfileInfo(Colosoft.Security.Profile.ProfileInfo info)
        {
            this.ProfileId = info.ProfileId;
            this.FullName = info.FullName;
            this.IsAnonymous = info.IsAnonymous;
            this.LastActivityDate = info.LastActivityDate;
            this.LastUpdatedDate = info.LastUpdatedDate;
            this.Source = info.Source == null ? null : new AuthenticationSource(info.Source);
            this.UserName = info.UserName;
            this.MarkGroupId = info.MarkGroupId;
        }

        #endregion

        /// <summary>
        /// Recupera o esquema XML do tipo.
        /// </summary>
        /// <param name="xs"></param>
        /// <returns></returns>
        public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
        {
            Colosoft.Security.RolePermissionNamespace.ResolveRolePermissionSchema(xs);
            Namespaces.ResolveQuerySchema(xs);
            return new System.Xml.XmlQualifiedName("ProfileInfo", Namespaces.Data);
        }

        /// <summary>
        /// Lê os dados serializados no xml.
        /// </summary>
        /// <param name="reader"></param>
        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();
            this.ProfileId = int.Parse(reader.ReadElementString("ProfileId"));
            this.UserName = reader.ReadElementString("UserName");
            this.FullName = reader.ReadElementString("FullName");

            var searchMode = Security.Profile.ProfileSearchMode.All;
            if (Enum.TryParse<Colosoft.Security.Profile.ProfileSearchMode>(reader.ReadElementString("SearchMode"), out searchMode))
                this.SearchMode = searchMode;

            LastActivityDate = reader.ReadDateTimeOffset();
            LastUpdatedDate = reader.ReadDateTimeOffset();

            if (!reader.IsEmptyElement)
            {
                Source = new AuthenticationSource();
                ((IXmlSerializable)this.Source).ReadXml(reader);
            }
            else
                reader.Skip();

            if (reader.LocalName == "IsAnonymous" && !reader.IsEmptyElement)
                this.IsAnonymous = reader.ReadElementContentAsBoolean();
            else
                reader.Skip();

            // MarkGroupId
            if (!reader.IsEmptyElement)
                this.MarkGroupId = reader.ReadElementContentAsInt();
            else
                reader.Skip();

            // SellerTreeId
            if (!reader.IsEmptyElement)
                this.SellerTreeId = reader.ReadElementContentAsInt();
            else
                reader.Skip();

            // IntermediateId
            if (!reader.IsEmptyElement)
                this.IntermediateId = reader.ReadElementContentAsInt();
            else
                reader.Skip();

            reader.ReadEndElement();
        }

        /// <summary>
        /// Escreve os dados no XML.
        /// </summary>
        /// <param name="writer"></param>
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("ProfileId", ProfileId.ToString());
            writer.WriteElementString("UserName", this.UserName);
            writer.WriteElementString("FullName", this.FullName);
            writer.WriteElementString("SearchMode", this.SearchMode.ToString());

            writer.WriteDateTimeOffset(LastActivityDate, "LastActivityDate");
            writer.WriteDateTimeOffset(LastUpdatedDate, "LastUpdatedDate");

            writer.WriteStartElement("Source", null);
            if (Source != null)
                ((IXmlSerializable)Source).WriteXml(writer);

            writer.WriteEndElement();

            writer.WriteStartElement("IsAnonymous", null);
            writer.WriteValue(IsAnonymous);
            writer.WriteEndElement();
            
            writer.WriteStartElement("MarkGroupId");
            if (MarkGroupId.HasValue) writer.WriteValue(MarkGroupId.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("SellerTreeId");
            if (SellerTreeId.HasValue) writer.WriteValue(SellerTreeId.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("IntermediateId");
            if (IntermediateId.HasValue) writer.WriteValue(IntermediateId.Value);
            writer.WriteEndElement();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }
    }
}
