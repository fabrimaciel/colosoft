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
	/// Tipo da entidade.
	/// </summary>
	public class EntityType : NotificationObject, System.Xml.Serialization.IXmlSerializable
	{
		private IObservableCollection<EntityTypeVersion> _versions = new BaseObservableCollection<EntityTypeVersion>();

		private string _name;

		private string _description;

		/// <summary>
		/// Versões.
		/// </summary>
		public IObservableCollection<EntityTypeVersion> Versions
		{
			get
			{
				return _versions;
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
		/// Instancia da versão ativa do tipo da entidade de negócio.
		/// </summary>
		public EntityTypeVersion CurrentVersion
		{
			get
			{
				var date = ServerData.GetDateTime();
				return Versions.OrderByDescending(f => f.StartDate).Where(f => f.StartDate <= date).FirstOrDefault();
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
			if(reader.MoveToAttribute("Description"))
				Description = reader.ReadContentAsString();
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "Versions" && !reader.IsEmptyElement)
					{
						reader.ReadStartElement();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if(reader.LocalName == "EntityTypeVersion")
							{
								var version = new EntityTypeVersion();
								((System.Xml.Serialization.IXmlSerializable)version).ReadXml(reader);
								Versions.Add(version);
							}
							else
								reader.Skip();
						}
						reader.ReadEndElement();
					}
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
			writer.WriteAttributeString("description", Description);
			writer.WriteStartElement("Versions");
			foreach (System.Xml.Serialization.IXmlSerializable version in Versions)
			{
				writer.WriteStartElement("EntityTypeVersion");
				version.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
