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

using Colosoft.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Validation.Configuration
{
	/// <summary>
	/// Grupo de regras de entrada.
	/// </summary>
	public class InputRulesGroup : NotificationObject, System.Xml.Serialization.IXmlSerializable
	{
		private static int _nextInputRulesGroupId = 1;

		private int _inputRulesGroupId = _nextInputRulesGroupId++;

		private Guid _uid = Guid.NewGuid();

		private string _name;

		private string _description;

		private IObservableCollection<InputRulesInputRulesGroup> _inputRules = new BaseObservableCollection<InputRulesInputRulesGroup>();

		/// <summary>
		/// Identificador do grupo.
		/// </summary>
		public int InputRulesGroupId
		{
			get
			{
				return _inputRulesGroupId;
			}
		}

		/// <summary>
		/// Regras de entrada associadas.
		/// </summary>
		public IObservableCollection<InputRulesInputRulesGroup> InputRules
		{
			get
			{
				return _inputRules;
			}
		}

		/// <summary>
		/// Identificador único do grupo.
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
		/// Descrição.
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				if(_description != value)
				{
					_description = value;
					RaisePropertyChanged("Description");
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
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					switch(reader.LocalName)
					{
					case "Description":
						Description = reader.ReadElementContentAsString();
						break;
					case "InputRules":
						{
							reader.ReadStartElement();
							while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
							{
								if(reader.LocalName == "InputRulesInputRulesGroup")
								{
									var inputRule = new InputRulesInputRulesGroup();
									((System.Xml.Serialization.IXmlSerializable)inputRule).ReadXml(reader);
									InputRules.Add(inputRule);
								}
								else
									reader.Skip();
							}
							reader.ReadEndElement();
						}
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
			writer.WriteElementString("Description", Description);
			writer.WriteStartElement("InputRules");
			foreach (System.Xml.Serialization.IXmlSerializable i in InputRules)
			{
				writer.WriteStartElement("InputRulesInputRulesGroup");
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
