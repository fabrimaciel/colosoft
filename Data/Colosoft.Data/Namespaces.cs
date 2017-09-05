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

namespace Colosoft.Data
{
	static class Namespaces
	{
		public const string Data = "http://tecms.com.br/2011/webservices/data";

		public const string Query = "http://tecms.com.br/2011/webservices/query";

		public const string SchemaInstance = "http://www.w3.org/2001/XMLSchema-instance";

		private static System.Xml.Schema.XmlSchema _persistenceSchema;

		/// <summary>
		/// Instancia do esquema da consulta.
		/// </summary>
		public static System.Xml.Schema.XmlSchema PersistenceSchema
		{
			get
			{
				if(_persistenceSchema == null)
				{
					var path = "Colosoft.Data.Xsd.Persistence.xsd";
					System.Xml.Schema.XmlSchema schema = null;
					var schemaSerializer = new System.Xml.Serialization.XmlSerializer(typeof(System.Xml.Schema.XmlSchema));
					using (var stream = typeof(Namespaces).Assembly.GetManifestResourceStream(path))
					{
						if(stream == null)
							return null;
						schema = (System.Xml.Schema.XmlSchema)schemaSerializer.Deserialize(new System.Xml.XmlTextReader(stream), null);
						_persistenceSchema = schema;
					}
				}
				return _persistenceSchema;
			}
		}

		/// <summary>
		/// Resolve o esquema da consulta.
		/// </summary>
		/// <param name="xs"></param>
		public static void ResolveSchema(this System.Xml.Schema.XmlSchemaSet xs)
		{
			var querySchema = PersistenceSchema;
			if(!xs.Contains(querySchema))
			{
				xs.XmlResolver = new System.Xml.XmlUrlResolver();
				xs.Add(querySchema);
			}
		}
	}
}
