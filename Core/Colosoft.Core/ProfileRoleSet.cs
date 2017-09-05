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

namespace Colosoft.Security.Profile
{
	/// <summary>
	/// Representa um conjunto de papéis de permissão para os perfis.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class ProfileRoleSet : IXmlSerializable
	{
		/// <summary>
		/// Nome do conjunto.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Conjunto de permissões dos papéis.
		/// </summary>
		public RolePermissionSet[] RolePermissionSets
		{
			get;
			set;
		}

		/// <summary>
		/// Verifica se a permissão informada é permitida no conjunto.
		/// </summary>
		/// <param name="permission"></param>
		/// <returns></returns>
		public bool IsAllowed(System.Security.IPermission permission)
		{
			if(permission == null)
				throw new ArgumentNullException("permission");
			if(RolePermissionSets == null)
				return false;
			foreach (RolePermissionSet i in RolePermissionSets)
			{
				foreach (System.Security.IPermission j in i.Permissions)
				{
					if(permission.IsSubsetOf(j))
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			RolePermissionNamespace.ResolveRolePermissionSchema(xs);
			return new System.Xml.XmlQualifiedName("ProfileRoleSet", RolePermissionNamespace.Data);
		}

		/// <summary>
		/// Não utilizar
		/// </summary>
		/// <returns></returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê um xml e deserializa o objeto
		/// </summary>
		/// <param name="reader">objeto leitor</param>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("Name"))
				this.Name = reader.ReadContentAsString();
		}

		/// <summary>
		/// Converte o objeto em XML
		/// </summary>
		/// <param name="writer">Objeto a ser escrito</param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Name", this.Name);
		}
	}
}
