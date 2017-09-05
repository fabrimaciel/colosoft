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
	/// Representa um termo de parametro.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetVariableSchema")]
	public class Variable : ConditionalTerm
	{
		/// <summary>
		/// Nome do parametro.
		/// </summary>
		public string Name
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
				return new System.Xml.XmlQualifiedName("Variable", Namespaces.Query);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Variable()
		{
		}

		/// <summary>
		/// Cria a instancia com o nome do parametro;
		/// </summary>
		/// <param name="name"></param>
		public Variable(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected Variable(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Name = info.GetString("Name");
		}

		private static System.Xml.XmlQualifiedName GetParameterSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("Variable", Namespaces.Query);
		}

		/// <summary>
		/// Recupera os dados para a serializa~çao.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Name", Name);
		}

		/// <summary>
		/// Escreve os dados da instancia no xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Name", Name);
		}

		/// <summary>
		/// Lê os dados do xml para a instancia.
		/// </summary>
		/// <param name="reader"></param>
		protected override void ReadXml(System.Xml.XmlReader reader)
		{
			Name = reader.GetAttribute("Name");
			reader.Skip();
		}

		/// <summary>
		/// Texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// Clona a variável.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new Variable(Name);
		}

		/// <summary>
		/// Faz a serialização do objeto.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(Name);
		}

		/// <summary>
		/// Faz a desserialização do objeto.
		/// </summary>
		/// <param name="reader"></param>
		protected override void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			Name = reader.ReadString();
		}
	}
}
