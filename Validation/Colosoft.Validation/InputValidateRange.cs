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
	/// Representa uma validação de entrada de faixa de tamanho de dados.
	/// </summary>
	public class InputValidateRange : NotificationObject, IRange, System.Xml.Serialization.IXmlSerializable
	{
		private string _fromValue;

		private string _toValue;

		/// <summary>
		/// Valor inicial.
		/// </summary>
		public string FromValue
		{
			get
			{
				return _fromValue;
			}
			set
			{
				if(_fromValue != value)
				{
					_fromValue = value;
					RaisePropertyChanged("FromValue");
				}
			}
		}

		/// <summary>
		/// Valor final.
		/// </summary>
		public string ToValue
		{
			get
			{
				return _toValue;
			}
			set
			{
				if(_toValue != value)
				{
					_toValue = value;
					RaisePropertyChanged("ToValue");
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
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					switch(reader.LocalName)
					{
					case "FromValue":
						FromValue = reader.ReadElementContentAsString();
						break;
					case "ToValue":
						ToValue = reader.ReadElementContentAsString();
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
			writer.WriteElementString("FromValue", FromValue);
			writer.WriteElementString("ToValue", ToValue);
		}
	}
}
