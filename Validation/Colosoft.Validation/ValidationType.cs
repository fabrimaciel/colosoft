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
	/// Representa o tipo de validação.
	/// </summary>
	public class ValidationType : NotificationObject, System.Xml.Serialization.IXmlSerializable
	{
		private static int _nextValidationTypeId = 1;

		private int _validationTypeId = _nextValidationTypeId++;

		private Guid _uid = Guid.NewGuid();

		private IObservableCollection<ValidationTypeParameter> _parameters = new BaseObservableCollection<ValidationTypeParameter>();

		private string _name;

		private string _description;

		private Reflection.TypeName _type;

		/// <summary>
		/// Parametros.
		/// </summary>
		public IObservableCollection<ValidationTypeParameter> Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Identificador único do tipo de validação.
		/// </summary>
		public Guid Uid
		{
			get
			{
				return _uid;
			}
			set
			{
				_uid = value;
			}
		}

		/// <summary>
		/// Identificador do tipo de validação.
		/// </summary>
		public int ValidationTypeId
		{
			get
			{
				return _validationTypeId;
			}
		}

		/// <summary>
		/// Nome do tipo de validação.
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
		/// Descrição do tipo de validação.
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
		/// Tipo associado.
		/// </summary>
		public Reflection.TypeName Type
		{
			get
			{
				return _type;
			}
			set
			{
				if(_type != null)
				{
					_type = value;
					RaisePropertyChanged("Type");
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
			if(reader.MoveToAttribute("type"))
			{
				var type = reader.ReadContentAsString();
				if(!string.IsNullOrEmpty(type))
					Type = new Reflection.TypeName(type);
			}
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "Parameters")
					{
						reader.ReadStartElement();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if(reader.LocalName == "ValidationTypeParameter")
							{
								var parameter = new ValidationTypeParameter();
								((System.Xml.Serialization.IXmlSerializable)parameter).ReadXml(reader);
								Parameters.Add(parameter);
							}
							else
								reader.Skip();
						}
						reader.ReadEndElement();
					}
					else if(reader.LocalName == "Description")
					{
						Description = reader.ReadElementContentAsString();
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
			writer.WriteAttributeString("type", Type != null ? Type.AssemblyQualifiedName : null);
			writer.WriteElementString("Description", Description);
			writer.WriteStartElement("Parameters");
			foreach (System.Xml.Serialization.IXmlSerializable parameter in Parameters)
			{
				writer.WriteStartElement("ValidationTypeParameter");
				parameter.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
