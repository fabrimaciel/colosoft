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
using System.Runtime.Serialization;
using Colosoft.Serialization;

namespace Colosoft.Query
{
	/// <summary>
	/// Armazena os paremetros do take.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public sealed class TakeParameters : ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, ICompactSerializable
	{
		private int _take;

		private int _skip;

		/// <summary>
		/// Quantidade de registros que devem ser recuperados.
		/// </summary>
		public int Take
		{
			get
			{
				return _take;
			}
			set
			{
				_take = value;
			}
		}

		/// <summary>
		/// Quantidade de registros que devem ser saltados.
		/// </summary>
		public int Skip
		{
			get
			{
				return _skip;
			}
			set
			{
				_skip = value;
			}
		}

		/// <summary>
		///Construtor padrão sem parâmetros 
		/// </summary>
		public TakeParameters()
		{
		}

		/// <summary>
		/// Construtor que preenche os dados.
		/// </summary>
		/// <param name="take"></param>
		/// <param name="skip"></param>
		public TakeParameters(int take, int skip)
		{
			_take = take;
			_skip = skip;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private TakeParameters(SerializationInfo info, StreamingContext context)
		{
			_take = info.GetInt32("Count");
			_skip = info.GetInt32("Skip");
		}

		/// <summary>
		/// Recupera os dados do objeto.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Count", _take);
			info.AddValue("Skip", _skip);
		}

		/// <summary>
		/// String que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Count:{0}, Skip:{1}]", _take, _skip);
		}

		/// <summary>
		/// Clona TakeParameters.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new TakeParameters(_take, _skip);
		}

		/// <summary>
		/// Recupera o esquema do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("TakeParameters", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			var value = reader.GetAttribute("Take");
			if(!string.IsNullOrEmpty(value))
				_take = int.Parse(value);
			value = reader.GetAttribute("Skip");
			if(!string.IsNullOrEmpty(value))
				_skip = int.Parse(value);
			reader.MoveToElement();
			reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Take", _take.ToString());
			writer.WriteAttributeString("Skip", _skip.ToString());
		}

		/// <summary>
		/// Desserializa o objeto
		/// </summary>
		/// <param name="reader"></param>
		void ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			_take = reader.ReadInt32();
			_skip = reader.ReadInt32();
		}

		/// <summary>
		/// Serializa o objeto
		/// </summary>
		/// <param name="writer"></param>
		void ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(_take);
			writer.Write(_skip);
		}
	}
}
