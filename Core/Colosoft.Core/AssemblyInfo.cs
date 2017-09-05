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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Armazena as informações do assembly.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class AssemblyInfo : System.Xml.Serialization.IXmlSerializable
	{
		private string _name;

		private DateTime _lastWriteTime;

		private string[] _references;

		/// <summary>
		/// Nome do assembly.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Data da ultima escrita do arquivo do assembly.
		/// </summary>
		public DateTime LastWriteTime
		{
			get
			{
				return _lastWriteTime;
			}
			set
			{
				_lastWriteTime = value;
			}
		}

		/// <summary>
		/// Nome das referencias.
		/// </summary>
		public string[] References
		{
			get
			{
				return _references ?? new string[0];
			}
			set
			{
				_references = value;
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _name ?? "Empty";
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			ReflectionNamespace.ResolveReflectionSchema(xs);
			return new System.Xml.XmlQualifiedName("AssemblyInfo", ReflectionNamespace.Data);
		}

		/// <summary>
		/// Recupera o esquema.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("Name"))
				_name = reader.ReadContentAsString();
			if(reader.MoveToAttribute("LastWriteTime"))
				_lastWriteTime = reader.ReadContentAsDateTime();
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				var refs = new List<string>();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "Reference")
						refs.Add(reader.ReadElementString());
					else
						reader.Skip();
				}
				this.References = refs.ToArray();
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Name", _name);
			writer.WriteStartAttribute("LastWriteTime");
			writer.WriteValue(_lastWriteTime);
			writer.WriteEndAttribute();
			foreach (var reference in _references)
			{
				writer.WriteStartElement("Reference");
				writer.WriteValue(reference);
				writer.WriteEndElement();
			}
		}
	}
}
