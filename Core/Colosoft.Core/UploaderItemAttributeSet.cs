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
	/// Implementação do conjunto de attributos.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class UploaderItemAttributeSet : IEnumerable<UploaderItemAttribute>, System.Xml.Serialization.IXmlSerializable
	{
		private List<UploaderItemAttribute> _attributes = new List<UploaderItemAttribute>();

		/// <summary>
		/// Quantidade de atributos no conjunto.
		/// </summary>
		public int Count
		{
			get
			{
				return _attributes.Count;
			}
		}

		/// <summary>
		/// Recupera e define o valor do atributo.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string this[string name]
		{
			get
			{
				return _attributes.Where(f => f.Name == name).Select(f => f.Value).FirstOrDefault();
			}
			set
			{
				name.Require("name").NotNull().NotEmpty();
				var index = _attributes.FindIndex(f => f.Name == name);
				if(index >= 0)
					_attributes[index].Value = value;
				else
					_attributes.Add(new UploaderItemAttribute(name, value));
			}
		}

		/// <summary>
		/// Recupera e define o atributo pelo indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public UploaderItemAttribute this[int index]
		{
			get
			{
				return _attributes[index];
			}
		}

		/// <summary>
		/// Adiciona o atributo para o conjunto.
		/// </summary>
		/// <param name="name">Nome do atributo.</param>
		/// <param name="value">Valor do atributo.</param>
		public void Add(string name, string value)
		{
			name.Require("name").NotNull().NotEmpty();
			this[name] = value;
		}

		/// <summary>
		/// Remove a instancia do atributo.
		/// </summary>
		/// <param name="attribute"></param>
		public bool Remove(UploaderItemAttribute attribute)
		{
			return _attributes.Remove(attribute);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			_attributes.RemoveAt(index);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Count: {0}]", Count);
		}

		/// <summary>
		/// Recupera o enumerado dos atributos.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<UploaderItemAttribute> GetEnumerator()
		{
			return _attributes.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerado dos atributos.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _attributes.GetEnumerator();
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			NetNamespace.ResolveSchema(xs);
			return new System.Xml.XmlQualifiedName("UploaderItemAttributeSet", NetNamespace.Data);
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
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "UploaderItemAttribute")
					{
						var attribute = new UploaderItemAttribute();
						((System.Xml.Serialization.IXmlSerializable)attribute).ReadXml(reader);
						_attributes.Add(attribute);
					}
					else
						reader.Skip();
				}
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
			foreach (var i in _attributes)
			{
				writer.WriteStartElement("UploaderItemAttribute");
				((System.Xml.Serialization.IXmlSerializable)i).WriteXml(writer);
				writer.WriteEndElement();
			}
		}
	}
}
