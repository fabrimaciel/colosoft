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

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Classe que armazena informações de sumarização de resultado
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class SummaryResult : System.Xml.Serialization.IXmlSerializable
	{
		private SummaryItem[] _items;

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public SummaryResult()
		{
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description">Descrição do sumário.</param>
		/// <param name="items"></param>
		public SummaryResult(string name, string description, SummaryItem[] items)
		{
			Name = name;
			Description = description;
			_items = items;
		}

		/// <summary>
		/// Construtor padrão da classe
		/// </summary>
		/// <param name="name">Nome do sumário</param>
		/// <param name="description">Descrição do sumário.</param>
		/// <param name="items">Lista de ítens</param>
		public SummaryResult(string name, string description, Dictionary<string, int> items)
		{
			Name = name;
			Description = description;
			_items = new SummaryItem[items.Count];
			int index = 0;
			foreach (KeyValuePair<string, int> newItem in items)
			{
				_items[index] = new SummaryItem(newItem);
				index++;
			}
		}

		/// <summary>
		/// Nome do sumário
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Descriação do sumário.
		/// </summary>
		public string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Ítens do sumário
		/// </summary>
		public SummaryItem[] Items
		{
			get
			{
				return _items;
			}
			set
			{
				_items = value;
			}
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			SearchEngineNamespace.ResolveSchema(xs);
			return new System.Xml.XmlQualifiedName("SummaryResult", SearchEngineNamespace.Data);
		}

		/// <summary>
		/// Recupera o esquema.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				reader.ReadStartElement("Items", SearchEngineNamespace.Data);
				var items = new List<SummaryItem>();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "SummaryItem")
					{
						var item = new SummaryItem();
						((System.Xml.Serialization.IXmlSerializable)item).ReadXml(reader);
						items.Add(item);
					}
					else
						reader.Skip();
				}
				Items = items.ToArray();
				reader.ReadEndElement();
				Name = reader.ReadElementString("Name", SearchEngineNamespace.Data);
				Description = reader.ReadElementString("Description", SearchEngineNamespace.Data);
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("Items", SearchEngineNamespace.Data);
			if(Items != null)
			{
				foreach (System.Xml.Serialization.IXmlSerializable field in Items)
				{
					writer.WriteStartElement("SummaryItem", SearchEngineNamespace.Data);
					field.WriteXml(writer);
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
			writer.WriteStartElement("Name", SearchEngineNamespace.Data);
			writer.WriteString(Name);
			writer.WriteEndElement();
			writer.WriteStartElement("Description", SearchEngineNamespace.Data);
			writer.WriteString(Description);
			writer.WriteEndElement();
		}
	}
}
