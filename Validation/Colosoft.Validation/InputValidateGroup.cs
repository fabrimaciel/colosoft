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
	/// Grupo de validações de entrada.
	/// </summary>
	public class InputValidateGroup : NotificationObject, System.Xml.Serialization.IXmlSerializable
	{
		private Collections.IObservableCollection<InputValidateGroupItem> _items = new Collections.BaseObservableCollection<InputValidateGroupItem>();

		/// <summary>
		/// Itens.
		/// </summary>
		public Collections.IObservableCollection<InputValidateGroupItem> Items
		{
			get
			{
				return _items;
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
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					switch(reader.LocalName)
					{
					case "Items":
						{
							reader.ReadStartElement();
							while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
							{
								if(reader.LocalName == "InputValidateGroupItem")
								{
									var item = new InputValidateGroupItem();
									((System.Xml.Serialization.IXmlSerializable)item).ReadXml(reader);
									Items.Add(item);
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
			writer.WriteStartElement("Items");
			foreach (System.Xml.Serialization.IXmlSerializable i in Items)
			{
				writer.WriteStartElement("InputValidateGroupItem");
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
