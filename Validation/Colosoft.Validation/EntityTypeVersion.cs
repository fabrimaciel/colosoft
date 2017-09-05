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
	/// Versão do tipo de uma entidade.
	/// </summary>
	public class EntityTypeVersion : NotificationObject, System.Xml.Serialization.IXmlSerializable
	{
		private IObservableCollection<EntityTypeVersionProperty> _properties = new BaseObservableCollection<EntityTypeVersionProperty>();

		private IObservableCollection<EntityTypeVersionEvent> _events = new BaseObservableCollection<EntityTypeVersionEvent>();

		private string _name;

		private string _description;

		private string _typeName;

		private string _namespace;

		private string _assembly;

		private DateTime _startDate;

		private string _customizationName;

		/// <summary>
		/// Propriedades.
		/// </summary>
		public IObservableCollection<EntityTypeVersionProperty> Properties
		{
			get
			{
				return _properties;
			}
		}

		/// <summary>
		/// Eventos
		/// </summary>
		public IObservableCollection<EntityTypeVersionEvent> Events
		{
			get
			{
				return _events;
			}
		}

		/// <summary>
		/// Nome da versão.
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
		/// Nome do tipo.
		/// </summary>
		public string TypeName
		{
			get
			{
				return _typeName;
			}
			set
			{
				if(_typeName != value)
				{
					_typeName = value;
					RaisePropertyChanged("TypeName", "Type");
				}
			}
		}

		/// <summary>
		/// Namespace.
		/// </summary>
		public string Namespace
		{
			get
			{
				return _namespace;
			}
			set
			{
				if(_namespace != value)
				{
					_namespace = value;
					RaisePropertyChanged("Namespace", "Type");
				}
			}
		}

		/// <summary>
		/// Assembly.
		/// </summary>
		public string Assembly
		{
			get
			{
				return _assembly;
			}
			set
			{
				if(_assembly != value)
				{
					_assembly = value;
					RaisePropertyChanged("Assembly", "Type");
				}
			}
		}

		/// <summary>
		/// Nome do tipo.
		/// </summary>
		public Reflection.TypeName Type
		{
			get
			{
				var name = string.Join(".", new[] {
					Namespace,
					TypeName
				}.Where(f => f != null));
				if(string.IsNullOrEmpty(name))
					return null;
				if(!string.IsNullOrEmpty(Assembly))
					return new Reflection.TypeName(string.Format("{0}, {1}", name, Assembly));
				return new Reflection.TypeName(name);
			}
			set
			{
				if(Type != value)
				{
					if(value != null)
					{
						_typeName = value.Name;
						_namespace = string.Join(".", value.Namespace);
						_assembly = value.AssemblyName.Name;
					}
					else
					{
						_typeName = null;
						_namespace = null;
						_assembly = null;
					}
					RaisePropertyChanged("TypeName", "Namespace", "Assembly");
				}
			}
		}

		/// <summary>
		/// Data de início da versão.
		/// </summary>
		public DateTime StartDate
		{
			get
			{
				return _startDate;
			}
			set
			{
				if(_startDate != value)
				{
					_startDate = value;
					RaisePropertyChanged("StartDate");
				}
			}
		}

		/// <summary>
		/// Nome da customização associada.
		/// </summary>
		public string CustomizationName
		{
			get
			{
				return _customizationName;
			}
			set
			{
				if(_customizationName != value)
				{
					_customizationName = value;
					RaisePropertyChanged("CustomizationName");
				}
			}
		}

		/// <summary>
		/// Número da customização.
		/// </summary>
		public int? CustomizationId
		{
			get
			{
				return null;
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
			if(reader.MoveToAttribute("name"))
				Name = reader.ReadContentAsString();
			if(reader.MoveToAttribute("startDate"))
				StartDate = reader.ReadContentAsDateTime();
			if(reader.MoveToAttribute("customization"))
				CustomizationName = reader.ReadContentAsString();
			if(reader.MoveToAttribute("type"))
			{
				var type = reader.ReadContentAsString();
				Type = !string.IsNullOrEmpty(type) ? new Reflection.TypeName(reader.ReadContentAsString()) : null;
			}
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "Properties" && !reader.IsEmptyElement)
					{
						reader.ReadStartElement();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if(reader.LocalName == "EntityTypeVersionProperty")
							{
								var property = new EntityTypeVersionProperty();
								((System.Xml.Serialization.IXmlSerializable)property).ReadXml(reader);
								Properties.Add(property);
							}
							else
								reader.Skip();
						}
						reader.ReadEndElement();
					}
					else if(reader.LocalName == "Events" && !reader.IsEmptyElement)
					{
						reader.ReadStartElement();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if(reader.LocalName == "EntityTypeVersionEvent")
							{
								var versionEvent = new EntityTypeVersionEvent();
								((System.Xml.Serialization.IXmlSerializable)versionEvent).ReadXml(reader);
								Events.Add(versionEvent);
							}
							else
								reader.Skip();
						}
						reader.ReadEndElement();
					}
					else
						reader.Skip();
				}
				reader.ReadEndElement();
			}
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("name", Name);
			writer.WriteStartAttribute("startDate");
			writer.WriteValue(StartDate);
			writer.WriteEndAttribute();
			writer.WriteAttributeString("customization", CustomizationName);
			writer.WriteAttributeString("type", Type != null ? Type.AssemblyQualifiedName : null);
			writer.WriteStartElement("Properties");
			foreach (System.Xml.Serialization.IXmlSerializable property in Properties)
			{
				writer.WriteStartElement("EntityTypeVersionProperty");
				property.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("Events");
			foreach (System.Xml.Serialization.IXmlSerializable versionEvent in Events)
			{
				writer.WriteStartElement("EntityTypeVersionEvent");
				versionEvent.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
