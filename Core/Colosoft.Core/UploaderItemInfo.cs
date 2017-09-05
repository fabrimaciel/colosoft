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

namespace Colosoft.Net
{
	/// <summary>
	/// Armazena as informações do item do uploader.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class UploaderItemInfo : System.Xml.Serialization.IXmlSerializable
	{
		private string _uid = Guid.NewGuid().ToString();

		private UploaderItemAttributeSet _attributes = new UploaderItemAttributeSet();

		private long _length;

		/// <summary>
		/// Identificador unico das informações.
		/// </summary>
		public string Uid
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
		/// Atributos associados com o item.
		/// </summary>
		public UploaderItemAttributeSet Attributes
		{
			get
			{
				return _attributes;
			}
		}

		/// <summary>
		/// Tamanho do item.
		/// </summary>
		public long Length
		{
			get
			{
				return _length;
			}
			set
			{
				_length = value;
			}
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public UploaderItemInfo()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="length">Tamanho do item.</param>
		/// <param name="attributes">Atributos associados.</param>
		public UploaderItemInfo(long length, params UploaderItemAttribute[] attributes)
		{
			_length = length;
			if(attributes != null)
				foreach (var i in attributes)
					if(i != null)
						_attributes.Add(i.Name, i.Value);
		}

		/// <summary>
		/// Cria a instancia com base nos dados contidos no item informado.
		/// </summary>
		/// <param name="item"></param>
		public UploaderItemInfo(IUploaderItem item)
		{
			item.Require("item").NotNull();
			_length = item.Length;
			if(item.Attributes != null)
				foreach (var i in item.Attributes)
					if(i != null)
						_attributes.Add(i.Name, i.Value);
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			NetNamespace.ResolveSchema(xs);
			return new System.Xml.XmlQualifiedName("UploaderItemInfo", NetNamespace.Data);
		}

		/// <summary>
		/// Recupera o esquema de serialização.
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
			_uid = reader.GetAttribute("Uid");
			long.TryParse(reader.GetAttribute("Length"), out _length);
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				if(!reader.IsEmptyElement)
				{
					var set = new UploaderItemAttributeSet();
					((System.Xml.Serialization.IXmlSerializable)set).ReadXml(reader);
					_attributes = set;
				}
				else
					reader.Skip();
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
			writer.WriteAttributeString("Uid", Uid);
			writer.WriteAttributeString("Length", Length.ToString());
			writer.WriteStartElement("Attributes");
			if(Attributes != null)
				((System.Xml.Serialization.IXmlSerializable)Attributes).WriteXml(writer);
			writer.WriteEndElement();
		}
	}
}
