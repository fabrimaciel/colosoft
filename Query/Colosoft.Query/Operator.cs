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
	/// Representa um operação de uma condicional.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetOperatorSchema")]
	public class Operator : ConditionalTerm
	{
		/// <summary>
		/// Texto do operador.
		/// </summary>
		public string Op
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera o nome qualificado para o tipo,
		/// </summary>
		public override System.Xml.XmlQualifiedName QualifiedName
		{
			get
			{
				return new System.Xml.XmlQualifiedName("Operator", Namespaces.Query);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Operator()
		{
		}

		/// <summary>
		/// Cria a instancia com o texto do operação.
		/// </summary>
		/// <param name="operatorText"></param>
		public Operator(string operatorText)
		{
			Op = operatorText;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected Operator(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Op = info.GetString("Op");
		}

		private static System.Xml.XmlQualifiedName GetOperatorSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("Operator", Namespaces.Query);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Op", Op);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		protected override void ReadXml(System.Xml.XmlReader reader)
		{
			Op = reader.ReadElementContentAsString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteValue(Op);
		}

		/// <summary>
		/// Recupera o texto da instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Op;
		}

		/// <summary>
		/// Clona o operador.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new Operator(Op);
		}

		/// <summary>
		/// Faz a serialização compacta
		/// </summary>
		/// <param name="writer"></param>
		protected override void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(Op);
		}

		/// <summary>
		/// Faz a desserialização do objeto.
		/// </summary>
		/// <param name="reader"></param>
		protected override void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			Op = reader.ReadString();
		}
	}
}
