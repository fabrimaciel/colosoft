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
	/// Validação de digitos para verificação.
	/// </summary>
	public class InputValidateCheckDigits : NotificationObject, ICheckDigits, System.Xml.Serialization.IXmlSerializable
	{
		private int _digits;

		private int _start;

		private int _limit;

		private int _base;

		/// <summary>
		/// Digitos.
		/// </summary>
		public int Digits
		{
			get
			{
				return _digits;
			}
			set
			{
				if(_digits != value)
				{
					_digits = value;
					RaisePropertyChanged("Digits");
				}
			}
		}

		/// <summary>
		/// Início.
		/// </summary>
		public int Start
		{
			get
			{
				return _start;
			}
			set
			{
				if(_start != value)
				{
					_start = value;
					RaisePropertyChanged("Start");
				}
			}
		}

		/// <summary>
		/// Limite.
		/// </summary>
		public int Limit
		{
			get
			{
				return _limit;
			}
			set
			{
				if(_limit != value)
				{
					_limit = value;
					RaisePropertyChanged("Limit");
				}
			}
		}

		/// <summary>
		/// Base.
		/// </summary>
		public int Base
		{
			get
			{
				return _base;
			}
			set
			{
				if(_base != value)
				{
					_base = value;
					RaisePropertyChanged("Base");
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
			Digits = int.Parse(reader.GetAttribute("digits"));
			Start = int.Parse(reader.GetAttribute("start"));
			Limit = int.Parse(reader.GetAttribute("limit"));
			reader.Skip();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("digits", Digits.ToString());
			writer.WriteAttributeString("start", Start.ToString());
			writer.WriteAttributeString("limit", Limit.ToString());
		}
	}
}
