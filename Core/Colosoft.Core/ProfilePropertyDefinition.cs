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
using System.Runtime.Serialization;

namespace Colosoft.Security.Profile
{
	/// <summary>
	/// Armazena a definição da propriedade do perfil.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public sealed class ProfilePropertyDefinition : System.Xml.Serialization.IXmlSerializable, ISerializable
	{
		/// <summary>
		/// Identificador unico da definição.
		/// </summary>
		public int Uid
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da propriedade.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Descrição da propriedade.
		/// </summary>
		public string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Definição do tipo.
		/// </summary>
		public Type TypeDefinition
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ProfilePropertyDefinition()
		{
		}

		/// <summary>
		/// Construtor da serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private ProfilePropertyDefinition(SerializationInfo info, StreamingContext context)
		{
			this.Uid = info.GetInt32("Uid");
			this.Name = info.GetString("Name");
			this.Description = info.GetString("Description");
			var typeFullName = info.GetString("Type");
			if(!string.IsNullOrEmpty(typeFullName))
				this.TypeDefinition = Type.GetType(typeFullName, false);
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			RolePermissionNamespace.ResolveRolePermissionSchema(xs);
			return new System.Xml.XmlQualifiedName("ProfilePropertyDefinition", RolePermissionNamespace.Data);
		}

		/// <summary>
		/// Não utilizar
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê um xml e deserializa o objeto
		/// </summary>
		/// <param name="reader">objeto leitor</param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("Uid"))
				Uid = reader.ReadContentAsInt();
			if(reader.MoveToAttribute("Name"))
				Name = reader.ReadContentAsString();
			if(reader.MoveToAttribute("Description"))
				Description = reader.ReadContentAsString();
			if(reader.MoveToAttribute("Type") && reader.HasValue)
				TypeDefinition = Type.GetType(reader.ReadContentAsString(), true);
			reader.MoveToElement();
			reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Uid", this.Uid.ToString());
			writer.WriteAttributeString("Name", this.Name);
			writer.WriteAttributeString("Description", this.Description);
			writer.WriteAttributeString("Type", this.TypeDefinition.FullName);
		}

		/// <summary>
		/// Recupera os dados serializados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Uid", Uid);
			info.AddValue("Name", Name);
			info.AddValue("Description", Description);
			info.AddValue("Type", TypeDefinition != null ? TypeDefinition.FullName : null);
		}
	}
}
