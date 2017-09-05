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

namespace Colosoft.Data.Schema.Local
{
	static class Namespaces
	{
		public const string Schema = "http://colosoft.com.br/2013/data/schema";

		public const string SchemaInstance = "http://www.w3.org/2001/XMLSchema-instance";

		private static object _objLock = new object();

		private static System.Xml.Schema.XmlSchema _querySchema;

		/// <summary>
		/// Instancia do esquema da consulta.
		/// </summary>
		public static System.Xml.Schema.XmlSchema TypeSchemaSchema
		{
			get
			{
				if(_querySchema == null)
					lock (_objLock)
						if(_querySchema == null)
							_querySchema = GetSchema();
				return _querySchema;
			}
		}

		private static Dictionary<string, System.Xml.Schema.XmlSchema> _schemas = new Dictionary<string, System.Xml.Schema.XmlSchema>();

		/// <summary>
		/// Recupera o esquema da biblioteca.
		/// </summary>
		/// <returns></returns>
		public static System.Xml.Schema.XmlSchema GetSchema()
		{
			var path = "Colosoft.Data.Schema.Configuration.TypeSchema.xsd";
			var schemaSerializer = new System.Xml.Serialization.XmlSerializer(typeof(System.Xml.Schema.XmlSchema));
			using (var stream = typeof(Namespaces).Assembly.GetManifestResourceStream(path))
			{
				if(stream == null)
					return null;
				return (System.Xml.Schema.XmlSchema)schemaSerializer.Deserialize(new System.Xml.XmlTextReader(stream), null);
			}
		}

		/// <summary>
		/// Resolve o esquema da consulta.
		/// </summary>
		/// <param name="xs"></param>
		public static void ResolveSchema(this System.Xml.Schema.XmlSchemaSet xs)
		{
			var schema = TypeSchemaSchema;
			if(!xs.Contains(schema))
			{
				xs.XmlResolver = new System.Xml.XmlUrlResolver();
				xs.Add(schema);
			}
		}
	}
}
