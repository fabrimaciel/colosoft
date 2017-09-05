﻿/* 
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
    /// Implemenetação da interface <see cref="IAuthenticationSource"/>.
    /// </summary>
    [Serializable]
    public sealed class AuthenticationSource : IXmlSerializable
    {
        #region Properties

        /// <summary>
        /// Identificador único da origem.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// Nome do origem de autenticação.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Identifica se a origem está ativa.
        /// </summary>
        public bool IsActive { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public AuthenticationSource()
        {

        }

        /// <summary>
        /// Cria a instancia os dados da origem informada.
        /// </summary>
        /// <param name="source"></param>
        public AuthenticationSource(Colosoft.Security.IAuthenticationSource source)
        {
            this.FullName = source.FullName;
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lê os dados do xml.
        /// </summary>
        /// <param name="reader"></param>
        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();
            FullName = reader.ReadElementString("FullName");
            Uid = int.Parse(reader.ReadElementString("Uid"));
            IsActive = bool.Parse(reader.ReadElementString("IsActive"));
            reader.ReadEndElement();
        }

        /// <summary>
        /// Escreve os dados no xml.
        /// </summary>
        /// <param name="writer"></param>
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("FullName", FullName);
            writer.WriteElementString("Uid", Uid.ToString());
            writer.WriteElementString("IsActive", IsActive.ToString());
        }

        #endregion
    }
}
