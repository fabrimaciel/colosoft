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
	/// Regras de entrada.
	/// </summary>
	public class InputRules : NotificationObject, System.Xml.Serialization.IXmlSerializable
	{
		private Guid _uid = Guid.NewGuid();

		private string _name;

		private Guid? _inputValidateUid;

		private int? _parseId;

		private string _label;

		private InputRulesOptions _options;

		private bool _copyValue;

		/// <summary>
		/// Identificador único da regra de entrada.
		/// </summary>
		public Guid Uid
		{
			get
			{
				return _uid;
			}
			set
			{
				if(_uid != value)
				{
					_uid = value;
					RaisePropertyChanged("Uid");
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
		/// Identivicador da validação de entrada.
		/// </summary>
		public Guid? InputValidateUid
		{
			get
			{
				return _inputValidateUid;
			}
			set
			{
				if(_inputValidateUid != value)
				{
					_inputValidateUid = value;
					RaisePropertyChanged("InputValidateUid");
				}
			}
		}

		/// <summary>
		/// Identificador do conversos
		/// </summary>
		public int? ParseId
		{
			get
			{
				return _parseId;
			}
			set
			{
				if(_parseId != value)
				{
					_parseId = value;
					RaisePropertyChanged("ParseId");
				}
			}
		}

		/// <summary>
		/// Label que será apresentado
		/// </summary>
		public string Label
		{
			get
			{
				return _label;
			}
			set
			{
				if(_label != value)
				{
					_label = value;
					RaisePropertyChanged("Label");
				}
			}
		}

		/// <summary>
		/// Opções(Apenas Leitura, Requerido, etc)
		/// </summary>
		public InputRulesOptions Options
		{
			get
			{
				return _options;
			}
			set
			{
				if(_options != value)
				{
					_options = value;
					RaisePropertyChanged("Options");
				}
			}
		}

		/// <summary>
		/// Identifica se é uma cópia de valor.
		/// </summary>
		public bool CopyValue
		{
			get
			{
				return _copyValue;
			}
			set
			{
				if(_copyValue != value)
				{
					_copyValue = value;
					RaisePropertyChanged("CopyValue");
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
			if(reader.MoveToAttribute("uid"))
				Uid = Guid.Parse(reader.ReadContentAsString());
			if(reader.MoveToAttribute("name"))
				Name = reader.ReadContentAsString();
			if(reader.MoveToAttribute("inputValidate"))
			{
				var inputValidate = reader.ReadContentAsString();
				if(!string.IsNullOrEmpty(inputValidate))
					this.InputValidateUid = Guid.Parse(inputValidate);
			}
			if(reader.MoveToAttribute("parse"))
			{
				var parse = reader.ReadContentAsString();
				if(!string.IsNullOrEmpty(parse))
					ParseId = int.Parse(parse);
			}
			if(reader.MoveToAttribute("options"))
				Options = (InputRulesOptions)Enum.Parse(typeof(InputRulesOptions), reader.ReadContentAsString());
			if(reader.MoveToAttribute("copyValue"))
				CopyValue = reader.ReadContentAsBoolean();
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					switch(reader.LocalName)
					{
					case "Label":
						Label = reader.ReadElementContentAsString();
						break;
					default:
						reader.Skip();
						break;
					}
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("uid", Uid.ToString());
			writer.WriteAttributeString("name", Name);
			writer.WriteAttributeString("inputValidate", InputValidateUid.HasValue ? InputValidateUid.Value.ToString() : "");
			writer.WriteAttributeString("parse", ParseId.HasValue ? ParseId.Value.ToString() : "");
			writer.WriteAttributeString("options", Options.ToString());
			writer.WriteAttributeString("copyValue", CopyValue.ToString().ToLower());
			writer.WriteElementString("Label", Label);
		}
	}
}
