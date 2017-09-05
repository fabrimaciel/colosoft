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

namespace Colosoft.Data.Schema.Local
{
	/// <summary>
	/// Representa os metadados de um tipo base.
	/// </summary>
	public class ReferenceTypeMetadata : System.Xml.Serialization.IXmlSerializable, ICloneable
	{
		private string _name;

		private string _namespace;

		private string _assembly;

		/// <summary>
		/// Nome do tipo.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Espaço de nome onde o tipo está inserido.
		/// </summary>
		public string Namespace
		{
			get
			{
				return _namespace;
			}
			internal set
			{
				_namespace = value;
			}
		}

		/// <summary>
		/// Nome completo do tipo.
		/// </summary>
		public string FullName
		{
			get
			{
				return string.Format("{0}.{1}", Namespace, Name);
			}
		}

		/// <summary>
		/// Nome do assembly onde o tipo está inserido.
		/// </summary>
		public string Assembly
		{
			get
			{
				return _assembly;
			}
			internal set
			{
				_assembly = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ReferenceTypeMetadata()
		{
		}

		/// <summary>
		/// Cria a instancia com os valores iniciais.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="nameSpace"></param>
		/// <param name="assembly"></param>
		public ReferenceTypeMetadata(string name, string nameSpace, string assembly)
		{
			_name = name;
			_namespace = nameSpace;
			_assembly = assembly;
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			_name = reader.GetAttribute("name");
			_namespace = reader.GetAttribute("namespace") ?? _namespace;
			_assembly = reader.GetAttribute("assembly") ?? _assembly;
		}

		/// <summary>
		/// Serializa os dados como Xml.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("name", Name);
			writer.WriteAttributeString("namespace", Namespace);
			writer.WriteAttributeString("assembly", Assembly);
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new ReferenceTypeMetadata(_name, _namespace, _assembly);
		}
	}
}
