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
using System.Collections;

namespace Colosoft
{
	/// <summary>
	/// Implementação da coleção de propriedades de uma exception.
	/// </summary>
	public sealed class ExceptionPropertyCollection : IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		private Dictionary<string, object> _properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Copia os dados da coleção informada.
		/// </summary>
		/// <param name="properties"></param>
		public void Copy(ExceptionPropertyCollection properties)
		{
			this.Copy(properties, false);
		}

		/// <summary>
		/// Copia os dados da cõleção de informada.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="replace">True identifica se é para substituir se a propriedade já existir.</param>
		public void Copy(ExceptionPropertyCollection properties, bool replace)
		{
			foreach (KeyValuePair<string, object> pair in (IEnumerable<KeyValuePair<string, object>>)properties)
			{
				if(!_properties.ContainsKey(pair.Key) || replace)
					_properties[pair.Key] = pair.Value;
			}
		}

		/// <summary>
		/// Recupera os dados contidos no leitor de XML informado.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static IEnumerable<KeyValuePair<string, object>> FromXml(System.Xml.XmlReader reader)
		{
			var list = new List<KeyValuePair<string, object>>();
			while (reader.NodeType == System.Xml.XmlNodeType.Element)
			{
				string attribute = reader.GetAttribute("name");
				if(!string.IsNullOrEmpty(attribute))
				{
					reader.Read();
					list.Add(new KeyValuePair<string, object>(attribute, Serialization.XmlHelper.ObjectFromXmlElement(reader)));
					reader.Read();
				}
				else
				{
					reader.ReadOuterXml();
				}
			}
			return list;
		}

		/// <summary>
		/// Remove a propriedade com base no nome informado.
		/// </summary>
		/// <param name="name"></param>
		public void Remove(string name)
		{
			_properties.Remove(name);
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, byte value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, DateTime value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, byte[] value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, DateTime[] value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, decimal[] value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, char value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, decimal value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, Guid value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, bool[] value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, double value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, short value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, int value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, double[] value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, long value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, Guid[] value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, bool value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, float value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, string value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, short[] value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, int[] value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, long[] value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, float[] value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Define o valor da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="value">Valor da propriedade.</param>
		public void Set(string name, string[] value)
		{
			_properties[name] = value;
		}

		/// <summary>
		/// Recupera o enumerador das propriedades.
		/// </summary>
		/// <returns></returns>
		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return _properties.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador das propriedades.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _properties.GetEnumerator();
		}

		/// <summary>
		/// Salva os dados para o XMLWriter informado.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="elementName"></param>
		internal void ToXml(System.Xml.XmlWriter writer, string elementName)
		{
			writer.WriteStartElement(elementName);
			writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
			writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
			foreach (KeyValuePair<string, object> pair in _properties)
			{
				writer.WriteStartElement("property");
				writer.WriteAttributeString("name", pair.Key);
				Serialization.XmlHelper.ObjectToXmlElement(writer, "value", pair.Value);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Quantidade de propriedades na coleção.
		/// </summary>
		public int Count
		{
			get
			{
				return _properties.Count;
			}
		}
	}
}
