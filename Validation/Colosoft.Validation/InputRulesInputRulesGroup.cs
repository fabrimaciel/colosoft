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
	/// Representa associação da regra de inetra com o grupo,
	/// </summary>
	public class InputRulesInputRulesGroup : NotificationObject, System.Xml.Serialization.IXmlSerializable
	{
		private string _uiContext;

		private Guid _inputRulesUid;

		private string _name;

		private ControlTypes _controlType = ControlTypes.TextBox;

		/// <summary>
		/// Identificador do contexto de usuário.
		/// </summary>
		public string UIContext
		{
			get
			{
				return _uiContext;
			}
			set
			{
				if(_uiContext != value)
				{
					_uiContext = value;
					RaisePropertyChanged("UIContext");
				}
			}
		}

		/// <summary>
		/// Identificador da regra de entrada associada.
		/// </summary>
		public Guid InputRulesUid
		{
			get
			{
				return _inputRulesUid;
			}
			set
			{
				if(_inputRulesUid != value)
				{
					_inputRulesUid = value;
					RaisePropertyChanged("InputRulesUid");
				}
			}
		}

		/// <summary>
		/// Nome.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(_name != value)
				{
					_name = value;
					RaisePropertyChanged("Name");
				}
			}
		}

		/// <summary>
		/// Tipo do campo.
		/// </summary>
		public ControlTypes ControlType
		{
			get
			{
				return _controlType;
			}
			set
			{
				if(_controlType != value)
				{
					_controlType = value;
					RaisePropertyChanged("ControlType");
				}
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
			UIContext = reader.GetAttribute("uiContext");
			Name = reader.GetAttribute("name");
			InputRulesUid = Guid.Parse(reader.GetAttribute("inputRules"));
			ControlType = (ControlTypes)Enum.Parse(typeof(ControlTypes), reader.GetAttribute("controlType"));
			reader.Skip();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("uiContext", UIContext);
			writer.WriteAttributeString("name", Name);
			writer.WriteAttributeString("inputRules", InputRulesUid.ToString());
			writer.WriteAttributeString("controlType", ControlType.ToString());
		}
	}
}
