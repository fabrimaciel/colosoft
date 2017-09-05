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

namespace Colosoft.Security
{
	/// <summary>
	/// Representa um conjunto de permissões para um papel do sistema.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public sealed class RolePermissionSet : IXmlSerializable
	{
		private System.Security.PermissionSet _permissions = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.None);

		/// <summary>
		/// Nome do papel.
		/// </summary>
		public string Role
		{
			get;
			set;
		}

		/// <summary>
		/// Permissões que garante acesso.
		/// </summary>
		public System.Security.PermissionSet Permissions
		{
			get
			{
				return _permissions;
			}
		}

		/// <summary>
		/// Recupera os dados das permissões pela leitura de um xml.
		/// </summary>
		/// <param name="reader"></param>
		public void PermissionsFromXml(System.Xml.XmlReader reader)
		{
			var element = FromXml(reader);
			_permissions = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.None);
			_permissions.FromXml(element);
		}

		/// <summary>
		/// Carrega os dados das permissões.
		/// </summary>
		/// <param name="securityElement"></param>
		public void PermissionsFromXml(System.Security.SecurityElement securityElement)
		{
			securityElement.Require("securityElement").NotNull();
			_permissions = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.None);
			_permissions.FromXml(securityElement);
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			RolePermissionNamespace.ResolveRolePermissionSchema(xs);
			return new System.Xml.XmlQualifiedName("RolePermissionSet", RolePermissionNamespace.Data);
		}

		/// <summary>
		/// Escreve o elemento de securança no XmlWriter.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		private static void WriteXml(System.Xml.XmlWriter writer, System.Security.SecurityElement element)
		{
			if(element == null)
				return;
			writer.WriteStartElement(element.Tag);
			if(element.Attributes != null)
				foreach (System.Collections.DictionaryEntry i in element.Attributes)
					writer.WriteAttributeString(i.Key.ToString(), i.Value.ToString());
			if(element.Children != null)
				foreach (System.Security.SecurityElement i in element.Children)
					WriteXml(writer, i);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Recupera os dados do xml.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		private static System.Security.SecurityElement FromXml(System.Xml.XmlReader reader)
		{
			var element = new System.Security.SecurityElement(reader.Name);
			for(var i = 0; i < reader.AttributeCount; i++)
			{
				reader.MoveToAttribute(i);
				element.AddAttribute(reader.Name, reader.Value);
			}
			reader.MoveToContent();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var child = FromXml(reader);
					element.AddChild(child);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			return element;
		}

		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			if(reader.IsEmptyElement)
				reader.Skip();
			else
			{
				reader.ReadStartElement();
				this.Role = reader.ReadContentAsString();
				reader.ReadEndElement();
			}
			if(!reader.IsEmptyElement)
			{
				var element = FromXml(reader);
				Permissions.FromXml(element);
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("RolePermissionSet", "http://colosoft.com.br/2013/security");
			writer.WriteElementString("Role", Role);
			WriteXml(writer, _permissions.ToXml());
			writer.WriteEndElement();
		}
	}
}
