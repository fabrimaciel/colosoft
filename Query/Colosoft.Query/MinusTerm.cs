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

using Colosoft.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Query
{
	/// <summary>
	/// Representa o termo que precede de um sinal negativo.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMinusTermSchema")]
	public class MinusTerm : ConditionalTerm, System.Xml.Serialization.IXmlSerializable, ICompactSerializable
	{
		private ConditionalTerm _term;

		/// <summary>
		/// Termo associado.
		/// </summary>
		public ConditionalTerm Term
		{
			get
			{
				return _term;
			}
			set
			{
				_term = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public MinusTerm()
		{
		}

		/// <summary>
		/// Cria a instancia informa o termo associado.
		/// </summary>
		/// <param name="term"></param>
		public MinusTerm(ConditionalTerm term)
		{
			_term = term;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected MinusTerm(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
			var termType = info.GetString("TermType");
			if(!string.IsNullOrEmpty(termType))
				_term = (ConditionalTerm)info.GetValue("Term", Type.GetType(termType, true));
		}

		/// <summary>
		/// Recupera os dados
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			string termType = null;
			if(_term != null)
			{
				termType = _term.GetType().FullName;
				info.AddValue("TermType", termType);
				info.AddValue("Term", _term);
			}
			else
				info.AddValue("TermType", termType);
		}

		/// <summary>
		/// Faz a desserialização do objeto.
		/// </summary>
		/// <param name="reader"></param>
		protected override void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			if(reader.ReadBoolean())
				_term = GetConditionalTerm(reader);
		}

		/// <summary>
		/// Faz a serialização compacta
		/// </summary>
		/// <param name="writer"></param>
		protected override void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			if(_term != null)
			{
				writer.Write(true);
				writer.Write(_term.GetType().Name);
				((ICompactSerializable)_term).Serialize(writer);
			}
			else
				writer.Write(false);
		}

		/// <summary>
		/// Clona um container condicional.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new MinusTerm {
				_term = _term != null ? (ConditionalTerm)_term.Clone() : null
			};
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Term: {0}]", _term == null ? "NULL" : _term.ToString());
		}

		/// <summary>
		/// Recupera o esquema do registro.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMinusTermSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("MinusTerm", Namespaces.Query);
		}

		/// <summary>
		/// Nome XML que irá representa a o tipo na serialização.
		/// </summary>
		public override System.Xml.XmlQualifiedName QualifiedName
		{
			get
			{
				return new System.Xml.XmlQualifiedName("MinusTerm", Namespaces.Query);
			}
		}

		/// <summary>
		/// Lê os dados serializados para a instancia.
		/// </summary>
		/// <param name="reader"></param>
		protected override void ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			if(!reader.IsEmptyElement || reader.HasAttributes)
			{
				reader.ReadStartElement();
				_term = GetConditionalTerm(reader);
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			reader.ReadEndElement();
		}

		/// <summary>
		/// Salva os dados da instancia no escritor informado.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("Term", Namespaces.Query);
			if(_term != null)
			{
				writer.WriteStartElement(_term.GetType().Name, Namespaces.Query);
				((System.Xml.Serialization.IXmlSerializable)_term).WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
