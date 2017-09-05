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

namespace Colosoft.Validation.Configuration
{
	static class Namespaces
	{
		public const string Validation = "http://colosoft.com.br/2015/validation";

		public const string SchemaInstance = "http://www.w3.org/2001/XMLSchema-instance";

		private static object _objLock = new object();

		private static System.Xml.Schema.XmlSchema _validationSchema;

		/// <summary>
		/// Instancia do esquema da consulta.
		/// </summary>
		public static System.Xml.Schema.XmlSchema ValidationSchema
		{
			get
			{
				if(_validationSchema == null)
					lock (_objLock)
						if(_validationSchema == null)
							_validationSchema = GetSchema();
				return _validationSchema;
			}
		}

		/// <summary>
		/// Recupera o esquema da biblioteca.
		/// </summary>
		/// <returns></returns>
		public static System.Xml.Schema.XmlSchema GetSchema()
		{
			var path = "Colosoft.Validation.Schemas.Validation.xsd";
			System.Xml.Schema.XmlSchema schema = null;
			var schemaSerializer = new System.Xml.Serialization.XmlSerializer(typeof(System.Xml.Schema.XmlSchema));
			using (var stream = typeof(Namespaces).Assembly.GetManifestResourceStream(path))
			{
				if(stream == null)
					return null;
				schema = (System.Xml.Schema.XmlSchema)schemaSerializer.Deserialize(new System.Xml.XmlTextReader(stream), null);
			}
			return schema;
		}

		/// <summary>
		/// Resolve o esquema da consulta.
		/// </summary>
		/// <param name="xs"></param>
		public static void ResolveQuerySchema(this System.Xml.Schema.XmlSchemaSet xs)
		{
			var querySchema = ValidationSchema;
			if(!xs.Contains(querySchema))
			{
				xs.XmlResolver = new System.Xml.XmlUrlResolver();
				xs.Add(querySchema);
			}
		}
	}
}
