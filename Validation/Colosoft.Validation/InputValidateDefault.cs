﻿/* 
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
	/// Valor padrão da validação de entrada.
	/// </summary>
	public class InputValidateDefault : NotificationObject, IDefaultValue, System.Xml.Serialization.IXmlSerializable
	{
		private string _defaultValue;

		/// <summary>
		/// Valor padrão.
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return _defaultValue;
			}
			set
			{
				if(_defaultValue != value)
				{
					_defaultValue = value;
					RaisePropertyChanged("DefaultValue");
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
				DefaultValue = reader.ReadElementContentAsString();
			reader.Skip();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteString(DefaultValue);
		}
	}
}
