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
	/// Representa uma validação.
	/// </summary>
	public class Validation : NotificationObject, System.Xml.Serialization.IXmlSerializable
	{
		private static int _nextValidationId = 0;

		private int _validationId = _nextValidationId++;

		private Guid _uid;

		private Guid _validationTypeUid;

		private string _name;

		private string _description;

		private IObservableCollection<ValidationParameter> _parameters = new BaseObservableCollection<ValidationParameter>();

		/// <summary>
		/// Parametros.
		/// </summary>
		public IObservableCollection<ValidationParameter> Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Identificador único.
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
		/// Identificador da validação.
		/// </summary>
		public int ValidationId
		{
			get
			{
				return _validationId;
			}
		}

		/// <summary>
		/// Identificador do tipo de validação.
		/// </summary>
		public Guid ValidationTypeUid
		{
			get
			{
				return _validationTypeUid;
			}
			set
			{
				if(_validationTypeUid != value)
				{
					_validationTypeUid = value;
					RaisePropertyChanged("ValidationTypeUid");
				}
			}
		}

		/// <summary>
		/// Nome da validação.
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
		/// Descrição da validação.
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
			if(reader.MoveToAttribute("validationTypeUid"))
				ValidationTypeUid = Guid.Parse(reader.ReadContentAsString());
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
							if(reader.LocalName == "ValidationParameter")
							{
								var parameter = new ValidationParameter();
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
			writer.WriteAttributeString("validationType", ValidationTypeUid.ToString());
			writer.WriteElementString("Description", Description);
			writer.WriteStartElement("Parameters");
			foreach (System.Xml.Serialization.IXmlSerializable parameter in Parameters)
			{
				writer.WriteStartElement("ValidationParameter");
				parameter.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
