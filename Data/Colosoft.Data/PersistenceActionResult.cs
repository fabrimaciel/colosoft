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
using Colosoft.Query;
using Colosoft.Serialization;

namespace Colosoft.Data
{
	/// <summary>
	/// Armazena os dados do resultado de uma ação de persistencia do sistema.
	/// </summary>
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	[Serializable]
	public sealed class PersistenceActionResult : System.Xml.Serialization.IXmlSerializable, ICompactSerializable
	{
		private PersistenceActionResult[] _beforeActions;

		private PersistenceActionResult[] _afterActions;

		private PersistenceActionResult[] _alternativeActions;

		[NonSerialized]
		private System.Collections.Hashtable _customAttributes = new System.Collections.Hashtable();

		/// <summary>
		/// Construtor padrão
		/// </summary>
		public PersistenceActionResult()
		{
		}

		/// <summary>
		/// Identificador da ação.
		/// </summary>
		public int ActionId
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a ação foi executada com sucesso.
		/// </summary>
		public bool Success
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem de falha caso tenha ocorrido.
		/// </summary>
		public string FailureMessage
		{
			get;
			set;
		}

		/// <summary>
		/// Quantidade de linhas afetadas.
		/// </summary>
		public int AffectedRows
		{
			get;
			set;
		}

		/// <summary>
		/// Versão da linha do resultado.
		/// </summary>
		public long RowVersion
		{
			get;
			set;
		}

		/// <summary>
		/// Parametros do resultado da ação.
		/// </summary>
		public PersistenceParameter[] Parameters
		{
			get;
			set;
		}

		/// <summary>
		/// Resultado de uma execução de persistência.
		/// </summary>
		public object Result
		{
			get;
			set;
		}

		/// <summary>
		/// Relação dos resultados das ações executadas antes.
		/// </summary>
		public PersistenceActionResult[] BeforeActions
		{
			get
			{
				return _beforeActions ?? new PersistenceActionResult[0];
			}
			set
			{
				_beforeActions = value;
			}
		}

		/// <summary>
		/// Relação dos resultados das ações executadas após.
		/// </summary>
		public PersistenceActionResult[] AfterActions
		{
			get
			{
				return _afterActions ?? new PersistenceActionResult[0];
			}
			set
			{
				_afterActions = value;
			}
		}

		/// <summary>
		/// Relação dos resultados das ações alternativas executadas.
		/// </summary>
		public PersistenceActionResult[] AlternativeActions
		{
			get
			{
				return _alternativeActions ?? new PersistenceActionResult[0];
			}
			set
			{
				_alternativeActions = value;
			}
		}

		/// <summary>
		/// Atributos customizados associados com o resultado.
		/// O valores condidos nessa Hashtable não são serializados.
		/// </summary>
		public System.Collections.Hashtable CustomAttributes
		{
			get
			{
				return _customAttributes;
			}
		}

		/// <summary>
		/// Gera um <see cref="Record"/> a partir de um parâmetro de persistência
		/// </summary>
		/// <returns><see cref="Record"/> gerado</returns>
		public Record ToRecord()
		{
			List<Record.Field> fields = new List<Record.Field>();
			object[] values = new object[Parameters != null ? Parameters.Length : 0];
			for(int i = 0; i < values.Length; i++)
			{
				fields.Add(new Record.Field(Parameters[i].Name, (Parameters[i].Value != null) ? Parameters[i].Value.GetType() : typeof(object)));
				values[i] = Parameters[i].Value;
			}
			Record.RecordDescriptor descriptor = new Record.RecordDescriptor("descriptor", fields);
			return descriptor.CreateRecord(values);
		}

		/// <summary>
		/// Recupera a mensagem de falha recursiva, ou seja, pega a mensagem onde ela realmente ocorreu.
		/// </summary>
		/// <returns></returns>
		public string GetRecursiveFailureMessage()
		{
			var message = new StringBuilder();
			if(GetRecursiveFailureMessage(this, message))
				return message.ToString();
			return FailureMessage;
		}

		/// <summary>
		/// Recupera a mensagem de falha de forma recursiva.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		private static bool GetRecursiveFailureMessage(PersistenceActionResult result, StringBuilder message)
		{
			if(!string.IsNullOrEmpty(result.FailureMessage))
				message.AppendFormat("Action: {0} --> ", result.ActionId).AppendLine(result.FailureMessage);
			if(result.BeforeActions != null && result.BeforeActions.Length > 0)
				foreach (var i in result.BeforeActions)
					if(GetRecursiveFailureMessage(i, message))
						return true;
			if(result.AfterActions != null && result.AfterActions.Length > 0)
				foreach (var i in result.AfterActions)
					if(GetRecursiveFailureMessage(i, message))
						return true;
			return !string.IsNullOrEmpty(result.FailureMessage);
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveSchema();
			Colosoft.Query.ConditionalContainer.GetConditionalContainerSchema(xs);
			return new System.Xml.XmlQualifiedName("PersistenceActionResult", Namespaces.Data);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToAttribute("ActionId");
			ActionId = reader.ReadContentAsInt();
			reader.MoveToAttribute("Success");
			Success = bool.Parse(reader.ReadContentAsString());
			reader.MoveToAttribute("AffectedRows");
			AffectedRows = reader.ReadContentAsInt();
			reader.MoveToAttribute("RowVersion");
			RowVersion = reader.ReadContentAsLong();
			reader.MoveToElement();
			reader.ReadStartElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement("FailureMessage");
				FailureMessage = reader.ReadString();
				reader.ReadEndElement();
			}
			else
			{
				FailureMessage = null;
				reader.Skip();
			}
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement("Result");
				Result = reader.ReadContentAsObject();
				reader.ReadEndElement();
			}
			else
			{
				Result = null;
				reader.Skip();
			}
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement("Parameters", Namespaces.Data);
				var parameters = new List<PersistenceParameter>();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "Parameter")
					{
						var parameter = new PersistenceParameter();
						((System.Xml.Serialization.IXmlSerializable)parameter).ReadXml(reader);
						parameters.Add(parameter);
					}
					else
						reader.Skip();
				}
				Parameters = parameters.ToArray();
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement("AlternativeActions", Namespaces.Data);
				var actions = new List<PersistenceActionResult>();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "PersistenceActionResult")
					{
						var action = new PersistenceActionResult();
						((System.Xml.Serialization.IXmlSerializable)action).ReadXml(reader);
						actions.Add(action);
					}
					else
						reader.Skip();
				}
				AlternativeActions = actions.ToArray();
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement("BeforeActions", Namespaces.Data);
				var actions = new List<PersistenceActionResult>();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "PersistenceActionResult")
					{
						var action = new PersistenceActionResult();
						((System.Xml.Serialization.IXmlSerializable)action).ReadXml(reader);
						actions.Add(action);
					}
					else
						reader.Skip();
				}
				BeforeActions = actions.ToArray();
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement("AfterActions", Namespaces.Data);
				var actions = new List<PersistenceActionResult>();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "PersistenceActionResult")
					{
						var action = new PersistenceActionResult();
						((System.Xml.Serialization.IXmlSerializable)action).ReadXml(reader);
						actions.Add(action);
					}
					else
						reader.Skip();
				}
				AfterActions = actions.ToArray();
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			reader.ReadEndElement();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("ActionId", ActionId.ToString());
			writer.WriteAttributeString("Success", Success.ToString());
			writer.WriteAttributeString("AffectedRows", AffectedRows.ToString());
			writer.WriteAttributeString("RowVersion", RowVersion.ToString());
			writer.WriteStartElement("FailureMessage");
			if(!string.IsNullOrEmpty(FailureMessage))
				writer.WriteValue(FailureMessage);
			writer.WriteEndElement();
			writer.WriteStartElement("Result");
			if(Result != null)
				writer.WriteValue(Result);
			writer.WriteEndElement();
			writer.WriteStartElement("Parameters", Namespaces.Data);
			if(Parameters != null)
				foreach (System.Xml.Serialization.IXmlSerializable parameter in Parameters)
				{
					writer.WriteStartElement("Parameter", Namespaces.Data);
					if(parameter != null)
						parameter.WriteXml(writer);
					writer.WriteEndElement();
				}
			writer.WriteEndElement();
			writer.WriteStartElement("AlternativeActions", Namespaces.Data);
			foreach (System.Xml.Serialization.IXmlSerializable parameter in AlternativeActions)
			{
				writer.WriteStartElement("PersistenceActionResult", Namespaces.Data);
				if(parameter != null)
					parameter.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("BeforeActions", Namespaces.Data);
			foreach (System.Xml.Serialization.IXmlSerializable parameter in BeforeActions)
			{
				writer.WriteStartElement("PersistenceActionResult", Namespaces.Data);
				if(parameter != null)
					parameter.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("AfterActions", Namespaces.Data);
			foreach (System.Xml.Serialization.IXmlSerializable parameter in AfterActions)
			{
				writer.WriteStartElement("PersistenceActionResult", Namespaces.Data);
				if(parameter != null)
					parameter.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Desserializa o objeto.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(Serialization.IO.CompactReader reader)
		{
			ActionId = reader.ReadInt32();
			Success = reader.ReadBoolean();
			AffectedRows = reader.ReadInt32();
			RowVersion = reader.ReadInt64();
			FailureMessage = reader.ReadString();
			if(reader.ReadBoolean())
				Result = reader.ReadObject();
			var parameters = new List<PersistenceParameter>();
			while (reader.ReadBoolean())
			{
				PersistenceParameter parameter = new PersistenceParameter();
				((ICompactSerializable)parameter).Deserialize(reader);
				parameters.Add(parameter);
			}
			Parameters = parameters.ToArray();
			var actions = new List<PersistenceActionResult>();
			while (reader.ReadBoolean())
			{
				PersistenceActionResult action = new PersistenceActionResult();
				((ICompactSerializable)action).Deserialize(reader);
				actions.Add(action);
			}
			AlternativeActions = actions.ToArray();
			var beforeActions = new List<PersistenceActionResult>();
			while (reader.ReadBoolean())
			{
				PersistenceActionResult action = new PersistenceActionResult();
				((ICompactSerializable)action).Deserialize(reader);
				beforeActions.Add(action);
			}
			BeforeActions = beforeActions.ToArray();
			var afterActions = new List<PersistenceActionResult>();
			while (reader.ReadBoolean())
			{
				PersistenceActionResult action = new PersistenceActionResult();
				((ICompactSerializable)action).Deserialize(reader);
				afterActions.Add(action);
			}
			AfterActions = afterActions.ToArray();
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Serialization.IO.CompactWriter writer)
		{
			writer.Write(ActionId);
			writer.Write(Success);
			writer.Write(AffectedRows);
			writer.Write(RowVersion);
			writer.Write(FailureMessage);
			if(Result != null)
			{
				writer.Write(true);
				writer.WriteObject(Result);
			}
			else
				writer.Write(false);
			if(Parameters != null)
			{
				foreach (ICompactSerializable parameter in Parameters)
					if(parameter != null)
					{
						writer.Write(true);
						parameter.Serialize(writer);
					}
			}
			writer.Write(false);
			foreach (ICompactSerializable action in AlternativeActions)
				if(action != null)
				{
					writer.Write(true);
					action.Serialize(writer);
				}
			writer.Write(false);
			foreach (ICompactSerializable action in BeforeActions)
				if(action != null)
				{
					writer.Write(true);
					action.Serialize(writer);
				}
			writer.Write(false);
			foreach (ICompactSerializable action in AfterActions)
				if(action != null)
				{
					writer.Write(true);
					action.Serialize(writer);
				}
			writer.Write(false);
		}
	}
}
