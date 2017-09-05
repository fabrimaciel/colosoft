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
	/// Classe que armazena um ítem de sumário
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class SummaryItem : System.Xml.Serialization.IXmlSerializable
	{
		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public SummaryItem()
		{
		}

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="item">ìtes</param>
		public SummaryItem(KeyValuePair<string, int> item)
		{
			Label = item.Key;
			Count = item.Value;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="count"></param>
		public SummaryItem(string label, int count)
		{
			Label = label;
			Count = count;
		}

		/// <summary>
		/// Label do ítem
		/// </summary>
		public string Label
		{
			get;
			set;
		}

		/// <summary>
		/// Quantidade de elementos
		/// </summary>
		public int Count
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			SearchEngineNamespace.ResolveSchema(xs);
			return new System.Xml.XmlQualifiedName("SummaryItem", SearchEngineNamespace.Data);
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
			reader.ReadStartElement();
			var count = 0;
			int.TryParse(reader.ReadElementString("Count", SearchEngineNamespace.Data), out count);
			Count = count;
			Label = reader.ReadElementString("Label", SearchEngineNamespace.Data);
			reader.ReadEndElement();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteElementString("Count", SearchEngineNamespace.Data, Count.ToString());
			writer.WriteElementString("Label", SearchEngineNamespace.Data, Label);
		}
	}
}
