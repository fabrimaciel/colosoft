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
	/// <summary>
	/// Configurações da propriedade.
	/// </summary>
	public class PropertySettings : IPropertySettingsInfo, System.Xml.Serialization.IXmlSerializable
	{
		/// <summary>
		/// Identificador do propriedade.
		/// </summary>
		public string Identifier
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da validação.
		/// </summary>
		public string Validation
		{
			get;
			set;
		}

		/// <summary>
		/// Grupo de regras de entrada.
		/// </summary>
		public Guid InputRulesGroupUid
		{
			get;
			set;
		}

		/// <summary>
		/// Indica que se as setagens devem ser recarregadas.
		/// </summary>
		public bool ReloadSettings
		{
			get;
			set;
		}

		/// <summary>
		/// Indica se a propriedade será ou não copiada ao copiar o objeto.
		/// </summary>
		public bool CopyValue
		{
			get;
			set;
		}

		/// <summary>
		/// Identificador da validação.
		/// </summary>
		public int? ValidationId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Identificador do grupo de regras.
		/// </summary>
		public int? InputRulesGroupId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Recupera o esquema do tipo.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê os dados serializados.
		/// </summary>
		/// <param name="reader"></param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("identifier"))
				Identifier = reader.ReadContentAsString();
			if(reader.MoveToAttribute("validation"))
				Validation = reader.ReadContentAsString();
			if(reader.MoveToAttribute("inputRulesGroupUid"))
				InputRulesGroupUid = Guid.Parse(reader.ReadContentAsString());
			if(reader.MoveToAttribute("reloadSettings"))
				ReloadSettings = reader.ReadContentAsBoolean();
			if(reader.MoveToAttribute("copyValue"))
				ReloadSettings = reader.ReadContentAsBoolean();
			reader.MoveToElement();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("identifier", Identifier);
			writer.WriteAttributeString("validation", Validation);
			writer.WriteAttributeString("inputRulesGroupUid", InputRulesGroupUid.ToString());
			writer.WriteAttributeString("reloadSettings", ReloadSettings.ToString().ToLower());
			writer.WriteAttributeString("copyValue", ReloadSettings.ToString().ToLower());
		}
	}
}
