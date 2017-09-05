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

namespace Colosoft.Security.Remote.Client
{
    static class Namespaces
    {
        #region Constants

        public const string Data = "http://Colosoft.com.br/2011/webservices/remote/profile";
        public const string SchemaInstance = "http://www.w3.org/2001/XMLSchema-instance";

        #endregion

        #region Local Variables

        private static System.Xml.Schema.XmlSchema _persistenceSchema;

        #endregion

        #region Properties

        /// <summary>
        /// Instancia do esquema da consulta.
        /// </summary>
        public static System.Xml.Schema.XmlSchema QuerySchema
        {
            get
            {
                if (_persistenceSchema == null)
                {
                    var path = "Colosoft.Security.Remote.Client.Xsd.Profile.xsd";
                    System.Xml.Schema.XmlSchema schema = null;

                    var schemaSerializer = new System.Xml.Serialization.XmlSerializer(typeof(System.Xml.Schema.XmlSchema));

                    using (var stream = typeof(Namespaces).Assembly.GetManifestResourceStream(path))
                    {
                        if (stream == null) return null;
                        schema = (System.Xml.Schema.XmlSchema)schemaSerializer.Deserialize(
                                    new System.Xml.XmlTextReader(stream), null);

                        _persistenceSchema = schema;
                    }
                }

                return _persistenceSchema;
            }
        }

        #endregion

        /// <summary>
        /// Resolve o esquema da consulta.
        /// </summary>
        /// <param name="xs"></param>
        public static void ResolveQuerySchema(this System.Xml.Schema.XmlSchemaSet xs)
        {
            var querySchema = QuerySchema;

            if (!xs.Contains(querySchema))
            {
                xs.XmlResolver = new System.Xml.XmlUrlResolver();
                xs.Add(querySchema);
            }
        }
    }
}
