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
	/// Armazena os dados do evento da entidade.
	/// </summary>
	public class EntityTypeVersionEvent : NotificationObject, System.Xml.Serialization.IXmlSerializable
	{
		private Colosoft.Domain.EntityEventType _type;

		private string _sequence;

		/// <summary>
		/// Tipo do evento.
		/// </summary>
		public Colosoft.Domain.EntityEventType Type
		{
			get
			{
				return _type;
			}
			set
			{
				if(_type != value)
				{
					_type = value;
					RaisePropertyChanged("Type");
				}
			}
		}

		/// <summary>
		/// Sequencia do evento.
		/// </summary>
		public string Sequence
		{
			get
			{
				return _sequence;
			}
			set
			{
				if(_sequence != value)
				{
					_sequence = value;
					RaisePropertyChanged("Sequence");
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
			Type = (Colosoft.Domain.EntityEventType)Enum.Parse(typeof(Colosoft.Domain.EntityEventType), reader.GetAttribute("type"));
			Sequence = reader.GetAttribute("sequence");
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("type", Type.ToString());
			writer.WriteAttributeString("sequence", Sequence);
		}
	}
}
