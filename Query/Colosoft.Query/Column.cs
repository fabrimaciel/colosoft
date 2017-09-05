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
	/// Representa o termo de uma coluna
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetColumnSchema")]
	public class Column : ConditionalTerm
	{
		/// <summary>
		/// Nome da coluna.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Apelido da coluna.
		/// </summary>
		public string Owner
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
				return new System.Xml.XmlQualifiedName("Column", Namespaces.Query);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Column()
		{
		}

		/// <summary>
		/// Cria a instancia com o nome da coluna.
		/// </summary>
		/// <param name="name"></param>
		public Column(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Cria a instancia com base na expressão.
		/// </summary>
		/// <param name="expression"></param>
		internal Column(Parser.SqlExpression expression)
		{
			expression.Require("expression").NotNull();
			var text = expression.Value.Text;
			text = text.Trim();
			var lastEmpty = text.LastIndexOf(' ');
			var splittedExpression = text.Split('.');
			for(var i = lastEmpty; i > 0; i--)
				if(text[i] != ' ')
				{
					if(splittedExpression.Length > 1)
					{
						Name = splittedExpression[1].Substring(0, i + 1).Trim();
						Owner = splittedExpression[0];
					}
					else
						Name = text.Substring(0, i + 1).Trim();
				}
			if(splittedExpression.Length > 1)
			{
				Name = lastEmpty < 0 ? splittedExpression[1] : null;
				Owner = splittedExpression[0];
			}
			else
				Name = lastEmpty < 0 ? text : null;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected Column(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Name = info.GetString("Name");
			Owner = info.GetString("Owner");
		}

		/// <summary>
		/// Recupera o esquena para o tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetColumnSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("Column", Namespaces.Query);
		}

		/// <summary>
		/// Escreve o xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Name", Name);
			writer.WriteAttributeString("Owner", Owner);
		}

		/// <summary>
		/// Lê os dados serializados em XML.
		/// </summary>
		/// <param name="reader"></param>
		protected override void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToAttribute("Name");
			Name = reader.ReadContentAsString();
			reader.MoveToAttribute("Owner");
			Owner = reader.ReadContentAsString();
			reader.MoveToElement();
			reader.Skip();
		}

		/// <summary>
		/// Recupera os dados serializados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Name", Name);
			info.AddValue("Owner", Owner);
		}

		/// <summary>
		/// Faz a serialização compacta
		/// </summary>
		/// <param name="writer"></param>
		protected override void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(Name);
			writer.Write(Owner);
		}

		/// <summary>
		/// Faz a desserialização do objeto.
		/// </summary>
		/// <param name="reader"></param>
		protected override void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			Name = reader.ReadString();
			Owner = reader.ReadString();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(!string.IsNullOrEmpty(Owner))
				return string.Format("'{0}'.'{1}'", Owner, Name);
			else
				return Name;
		}

		/// <summary>
		/// Clona a coluna.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new Column(Name) {
				Owner = this.Owner
			};
		}
	}
}
