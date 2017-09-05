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

namespace Colosoft.Query
{
	static class SchemasHelper
	{
		private static Dictionary<string, System.Xml.Schema.XmlSchema> _schemas = new Dictionary<string, System.Xml.Schema.XmlSchema>();

		/// <summary>
		/// Recupera o esquema da biblioteca.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static System.Xml.Schema.XmlSchema GetSchema(string name)
		{
			var path = string.Format("Colosoft.Query.Schemas.{0}.xsd", name);
			System.Xml.Schema.XmlSchema schema = null;
			if(_schemas.TryGetValue(path, out schema))
				return schema;
			var schemaSerializer = new System.Xml.Serialization.XmlSerializer(typeof(System.Xml.Schema.XmlSchema));
			using (var stream = typeof(SchemasHelper).Assembly.GetManifestResourceStream(path))
			{
				if(stream == null)
					return null;
				schema = (System.Xml.Schema.XmlSchema)schemaSerializer.Deserialize(new System.Xml.XmlTextReader(stream), null);
				_schemas.Add(path, schema);
			}
			return schema;
		}

		/// <summary>
		/// Recupera o valor string do elemento do reader.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="name"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		public static string ReadString(this System.Xml.XmlReader reader, string name, string ns)
		{
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement(name, ns);
				if(reader.HasValue)
				{
					var value = reader.ReadString();
					reader.ReadEndElement();
					return value;
				}
			}
			else
				reader.Skip();
			return null;
		}
	}
}
