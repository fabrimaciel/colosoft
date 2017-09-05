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

namespace Colosoft.Query
{
	/// <summary>
	/// Representa uma constante.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetConstantSchema")]
	public class Constant : ConditionalTerm
	{
		/// <summary>
		/// Texto que representa a constante.
		/// </summary>
		public string Text
		{
			get;
			set;
		}

		/// <summary>
		/// Nome que qualifica o elemento XML.
		/// </summary>
		public override System.Xml.XmlQualifiedName QualifiedName
		{
			get
			{
				return new System.Xml.XmlQualifiedName("Constant", Namespaces.Query);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Constant()
		{
		}

		/// <summary>
		/// Cria a instancia com o texto informado.
		/// </summary>
		/// <param name="text"></param>
		public Constant(string text)
		{
			this.Text = text;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected Constant(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Text = info.GetString("Text");
		}

		/// <summary>
		/// Clona uma constante.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new Constant(Text);
		}

		/// <summary>
		/// Recupera o esquena para o tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetConstantSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("Constant", Namespaces.Query);
		}

		/// <summary>
		/// Recupera os dados serializados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Text", Text);
		}

		/// <summary>
		/// Lê os dados serializados no xml.
		/// </summary>
		/// <param name="reader"></param>
		protected override void ReadXml(System.Xml.XmlReader reader)
		{
			Text = reader.ReadElementContentAsString();
		}

		/// <summary>
		/// Serializa os dados no XML.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteValue(Text);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Text;
		}

		/// <summary>
		/// Faz a serialização compacta
		/// </summary>
		/// <param name="writer"></param>
		protected override void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(Text);
		}

		/// <summary>
		/// Faz a desserialização compacta
		/// </summary>
		/// <param name="reader"></param>
		protected override void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			Text = reader.ReadString();
		}
	}
}
