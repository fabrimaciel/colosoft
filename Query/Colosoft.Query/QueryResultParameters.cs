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
using System.Xml.Serialization;
using Colosoft.Serialization;
using System.Xml;

namespace Colosoft.Query
{
	/// <summary>
	/// Classe que contém resultado da consulta mais seus parâmetros.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetQueryResultParametersSchema")]
	public sealed class QueryResultParameters : IXmlSerializable, ISerializable, ICompactSerializable
	{
		/// <summary>
		/// Construtor Vazio.
		/// </summary>
		public QueryResultParameters()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="result">Resultado da consulta.</param>
		/// <param name="parameters">Parâmetros da consulta.</param>
		public QueryResultParameters(IQueryResult result, QueryParameterCollection parameters)
		{
			Result = result;
			Parameters = parameters;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private QueryResultParameters(SerializationInfo info, StreamingContext context)
		{
			Result = info.GetValue("Result", typeof(QueryResult)) as QueryResult;
			var count = info.GetInt32("Count");
			for(int i = 0; i < count; i++)
			{
				Parameters.Add(info.GetValue(string.Format("Parameter_{0}", i), typeof(QueryParameter)) as QueryParameter);
			}
		}

		/// <summary>
		/// Resultado de consulta.
		/// </summary>
		public IQueryResult Result
		{
			get;
			set;
		}

		/// <summary>
		/// Parâmetros da consulta.
		/// </summary>
		public QueryParameterCollection Parameters
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera o esquema do registro.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetQueryResultParametersSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new XmlQualifiedName("QueryResultParameters", Namespaces.Query);
		}

		/// <summary>
		/// Deserializa a instância.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			((Colosoft.Serialization.ICompactSerializable)Result).Deserialize(reader);
			((Colosoft.Serialization.ICompactSerializable)Parameters).Deserialize(reader);
		}

		/// <summary>
		/// Serializa instância.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			((Colosoft.Serialization.ICompactSerializable)Result).Serialize(writer);
			((Colosoft.Serialization.ICompactSerializable)Parameters).Serialize(writer);
		}

		/// <summary>
		/// Recupera os dados do objeto.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Result", Result);
			var index = 0;
			info.AddValue("Count", Parameters.Count);
			for(int i = 0; i < Parameters.Count; i++)
			{
				info.AddValue(string.Format("Parameter_{0}", i), Parameters[i]);
			}
			info.AddValue("C", index);
		}

		/// <summary>
		/// Recupera schema da instância.
		/// </summary>
		/// <returns></returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Implementação da leitura de xml para a interface <see cref="IXmlSerializable"/>
		/// </summary>
		/// <param name="reader"></param>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if(!reader.IsEmptyElement && reader.LocalName == "Result")
				{
					reader.MoveToElement();
					Result = new QueryResult();
					((System.Xml.Serialization.IXmlSerializable)Result).ReadXml(reader);
				}
				else if(!reader.IsEmptyElement && reader.LocalName == "Parameters")
				{
					reader.ReadStartElement("Parameters", Namespaces.Query);
					Parameters = new QueryParameterCollection();
					while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
					{
						var parameter = new QueryParameter();
						((System.Xml.Serialization.IXmlSerializable)parameter).ReadXml(reader);
						Parameters.Add(parameter);
					}
					reader.ReadEndElement();
				}
				else
					reader.Skip();
			}
			reader.ReadEndElement();
		}

		/// <summary>
		/// Implementação da escrita de xml para a interface <see cref="IXmlSerializable"/>
		/// </summary>
		/// <param name="writer"></param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("Result");
			if(Result != null)
				((IXmlSerializable)Result).WriteXml(writer);
			writer.WriteEndElement();
			writer.WriteStartElement("Parameters", Namespaces.Query);
			if(Parameters != null)
			{
				foreach (System.Xml.Serialization.IXmlSerializable i in Parameters)
				{
					writer.WriteStartElement("QueryParameter", Namespaces.Query);
					i.WriteXml(writer);
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
		}
	}
}
