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
	/// Validação de tamanho de campo.
	/// </summary>
	public class InputValidateLength : NotificationObject, ILength, System.Xml.Serialization.IXmlSerializable
	{
		private double _minValue;

		private double _maxValue;

		/// <summary>
		/// Valor mínimo.
		/// </summary>
		public double MinValue
		{
			get
			{
				return _minValue;
			}
			set
			{
				if(_minValue != value)
				{
					_minValue = value;
					RaisePropertyChanged("MinValue");
				}
			}
		}

		/// <summary>
		/// Valor máximo.
		/// </summary>
		public double MaxValue
		{
			get
			{
				return _maxValue;
			}
			set
			{
				if(_maxValue != value)
				{
					_maxValue = value;
					RaisePropertyChanged("MaxValue");
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
			MinValue = double.Parse(reader.GetAttribute("minValue"), System.Globalization.CultureInfo.InvariantCulture);
			MaxValue = double.Parse(reader.GetAttribute("maxValue"), System.Globalization.CultureInfo.InvariantCulture);
			reader.Skip();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("minValue", MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteAttributeString("maxValue", MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
	}
}
