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
using Colosoft.Serialization;

namespace Colosoft.Data
{
	/// <summary>
	/// Delegate que responsável pela chama de retorno de uma ação de persistencia.
	/// </summary>
	/// <param name="action">Dados da ação.</param>
	/// <param name="result">Resultado da ação.</param>
	public delegate void PersistenceActionCallback (PersistenceAction action, PersistenceActionResult result);
	/// <summary>
	/// Armazena os argumentos da ação de persistencia executada.
	/// </summary>
	public class PersistenceActionExecutedArgs : EventArgs
	{
		private bool _success;

		private Exception _error;

		/// <summary>
		/// Identifica se a ação foi executada com sucesso.
		/// </summary>
		public bool Success
		{
			get
			{
				return _success;
			}
		}

		/// <summary>
		/// Erro ocorrido.
		/// </summary>
		public Exception Error
		{
			get
			{
				return _error;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="success"></param>
		/// <param name="error"></param>
		public PersistenceActionExecutedArgs(bool success, Exception error)
		{
			_success = success;
			_error = error;
		}
	}
	/// <summary>
	/// Assinatura do método acionado quando uma ção de persistencia for executada.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void PersistenceActionExecutedHandler (object sender, PersistenceActionExecutedArgs e);
	/// <summary>
	/// Possíveis ações de persistencia.
	/// </summary>
	public enum PersistenceActionType
	{
		/// <summary>
		/// INSERT
		/// </summary>
		Insert,
		/// <summary>
		/// UPDATE
		/// </summary>
		Update,
		/// <summary>
		/// DELETE
		/// </summary>
		Delete,
		/// <summary>
		/// EXECUTE PROCEDURE
		/// </summary>
		ExecuteProcedure
	}
	/// <summary>
	/// Representa uma ação de persistencia.
	/// </summary>
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	[Serializable]
	public sealed class PersistenceAction : System.Xml.Serialization.IXmlSerializable, ICompactSerializable
	{
		private List<PersistenceAction> _alternativeActions = new List<PersistenceAction>();

		private List<PersistenceAction> _beforeActions = new List<PersistenceAction>();

		private List<PersistenceAction> _afterActions = new List<PersistenceAction>();

		private Colosoft.Query.StoredProcedureName _storedProcedureName;

		private PersistenceParameterCollection _parameters = new PersistenceParameterCollection();

		private Query.QueryInfo _query;

		private Colosoft.Query.ConditionalContainer _conditional;

		private string _entityFullName;

		private int _commandTimeout = 30;

		private string[] _changedProperties;

		/// <summary>
		/// Evento acionado quando a ação de persistencia for executada.
		/// </summary>
		public event PersistenceActionExecutedHandler Executed;

		/// <summary>
		/// Identificador da ação.
		/// </summary>
		public int ActionId
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo da ação.
		/// </summary>
		public PersistenceActionType Type
		{
			get;
			set;
		}

		/// <summary>
		/// Instancia associada com a ação.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public object Instance
		{
			get;
			set;
		}

		/// <summary>
		/// Callback registrado para a ação.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public PersistenceActionCallback Callback
		{
			get;
			set;
		}

		/// <summary>
		/// Nome completo da entidade associada com a ação.
		/// </summary>
		public string EntityFullName
		{
			get
			{
				return _entityFullName;
			}
			set
			{
				_entityFullName = value;
			}
		}

		/// <summary>
		/// Nome do provedor de configuração para StoredProcedure.
		/// </summary>
		public string ProviderName
		{
			get;
			set;
		}

		/// <summary>
		/// Lista dos parametros da ação de persistencia.
		/// </summary>
		public PersistenceParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
			internal set
			{
				_parameters = value;
			}
		}

		/// <summary>
		/// Identifica se tabela sobre a qual ocorrerá ação de persistência possui controle de versão.
		/// </summary>
		public long? RowVersion
		{
			get;
			set;
		}

		/// <summary>
		/// Condicional para a execução da ação.
		/// </summary>
		public Colosoft.Query.ConditionalContainer Conditional
		{
			get
			{
				return _conditional;
			}
			set
			{
				_conditional = value;
			}
		}

		/// <summary>
		/// Ações alternativas caso ocorra uma falha.
		/// </summary>
		public List<PersistenceAction> AlternativeActions
		{
			get
			{
				return _alternativeActions;
			}
		}

		/// <summary>
		/// Ações que devem ser executadas antes.
		/// </summary>
		public List<PersistenceAction> BeforeActions
		{
			get
			{
				return _beforeActions;
			}
		}

		/// <summary>
		/// Ações que devem ser executadas depois.
		/// </summary>
		public List<PersistenceAction> AfterActions
		{
			get
			{
				return _afterActions;
			}
		}

		/// <summary>
		/// Nome da StoredProcedure a ser executada ou nulo.
		/// </summary>
		public Colosoft.Query.StoredProcedureName StoredProcedureName
		{
			get
			{
				return _storedProcedureName;
			}
			set
			{
				_storedProcedureName = value;
			}
		}

		/// <summary>
		/// Consulta usada na ação de persistencia.
		/// </summary>
		public Query.QueryInfo Query
		{
			get
			{
				return _query;
			}
			set
			{
				_query = value;
			}
		}

		/// <summary>
		/// Propriedade alteraras pela ação.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public string[] ChangedProperties
		{
			get
			{
				return _changedProperties;
			}
			set
			{
				_changedProperties = value;
			}
		}

		/// <summary>
		/// Tempo de espera, em segundos, antes do termino da execução do comando e geração de um erro.
		/// O valor padrão é 30 segundos.
		/// </summary>
		public int CommandTimeout
		{
			get
			{
				return _commandTimeout;
			}
			set
			{
				_commandTimeout = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public PersistenceAction()
		{
		}

		/// <summary>
		/// Cria uma nova instancia já definindo os valores da ação.
		/// </summary>
		/// <param name="actionId">Identificador da ação.</param>
		/// <param name="type">Tipo de ação.</param>
		/// <param name="entityFullName">Nome da entidade associada com a ação.</param>
		/// <param name="parameters">Lista dos parametros da ação de persistencia.</param>
		/// <param name="instance">Instancia associada com a ação.</param>
		/// <param name="providerName"></param>
		/// <param name="callback">Instancia do callback registrado para a ação.</param>
		/// <param name="rowVersion">Identifica se tabela sobre a qual ocorrerá ação de persistência possui controle de versão.</param>
		/// <param name="conditional">Condicional para a execução da ação.</param>
		/// <param name="query">Consulta para a execução da ação.</param>
		public PersistenceAction(int actionId, PersistenceActionType type, string entityFullName, string providerName, IEnumerable<PersistenceParameter> parameters, object instance, PersistenceActionCallback callback, long? rowVersion, Query.ConditionalContainer conditional = null, Query.QueryInfo query = null)
		{
			this.ActionId = actionId;
			this.EntityFullName = entityFullName;
			this.ProviderName = providerName;
			if(parameters != null)
				this.Parameters.AddRange(parameters);
			this.Instance = instance;
			this.Callback = callback;
			this.Conditional = conditional;
			this.Query = query;
			this.Type = type;
			this.RowVersion = rowVersion;
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
			return new System.Xml.XmlQualifiedName("PersistenceAction", Namespaces.Data);
		}

		/// <summary>
		/// Recupera o esquema da ação.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê os dados do XML.
		/// </summary>
		/// <param name="reader"></param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToAttribute("ActionId");
			ActionId = reader.ReadContentAsInt();
			reader.MoveToAttribute("Type");
			var typeString = reader.ReadContentAsString();
			Type = (PersistenceActionType)Enum.Parse(typeof(PersistenceActionType), typeString);
			reader.MoveToAttribute("EntityFullName");
			EntityFullName = reader.ReadContentAsString();
			reader.MoveToAttribute("ProviderName");
			ProviderName = reader.ReadContentAsString();
			reader.MoveToAttribute("RowVersion");
			var RowVersionString = reader.ReadContentAsString();
			if(!string.IsNullOrEmpty(RowVersionString))
				RowVersion = long.Parse(RowVersionString);
			else
				RowVersion = null;
			if(reader.MoveToAttribute("CommandTimeout"))
			{
				var timeout = 0;
				if(int.TryParse(reader.ReadContentAsString(), out timeout))
					CommandTimeout = timeout;
			}
			reader.MoveToElement();
			reader.ReadStartElement();
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
				Parameters = new PersistenceParameterCollection(parameters);
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement)
			{
				var conditional = new Colosoft.Query.ConditionalContainer();
				((System.Xml.Serialization.IXmlSerializable)conditional).ReadXml(reader);
				this.Conditional = conditional;
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement("AlternativeActions");
				var actions = new List<PersistenceAction>();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "PersistenceAction")
					{
						var persistenceAction = new PersistenceAction();
						((System.Xml.Serialization.IXmlSerializable)persistenceAction).ReadXml(reader);
						actions.Add(persistenceAction);
					}
					else
						reader.Skip();
				}
				AlternativeActions.AddRange(actions);
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement("BeforeActions");
				var actions = new List<PersistenceAction>();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "PersistenceAction")
					{
						var persistenceAction = new PersistenceAction();
						((System.Xml.Serialization.IXmlSerializable)persistenceAction).ReadXml(reader);
						actions.Add(persistenceAction);
					}
					else
						reader.Skip();
				}
				BeforeActions.AddRange(actions);
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement("AfterActions");
				var actions = new List<PersistenceAction>();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "PersistenceAction")
					{
						var persistenceAction = new PersistenceAction();
						((System.Xml.Serialization.IXmlSerializable)persistenceAction).ReadXml(reader);
						actions.Add(persistenceAction);
					}
					else
						reader.Skip();
				}
				AfterActions.AddRange(actions);
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			_storedProcedureName = ReadItem<Colosoft.Query.StoredProcedureName>(reader, "StoredProcedureName");
			if(!reader.IsEmptyElement)
			{
				var query = new Colosoft.Query.QueryInfo();
				((System.Xml.Serialization.IXmlSerializable)query).ReadXml(reader);
				this.Query = query;
			}
			else
				reader.Skip();
			reader.ReadEndElement();
		}

		/// <summary>
		/// Escreve os dados para XML.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("xmlns", "i", null, Namespaces.SchemaInstance);
			writer.WriteAttributeString("xmlns", "q", null, Namespaces.Query);
			writer.WriteAttributeString("ActionId", ActionId.ToString());
			writer.WriteAttributeString("Type", Type.ToString());
			writer.WriteAttributeString("EntityFullName", EntityFullName);
			writer.WriteAttributeString("ProviderName", ProviderName ?? "");
			writer.WriteAttributeString("RowVersion", (RowVersion.HasValue) ? RowVersion.ToString() : "");
			writer.WriteAttributeString("CommandTimeout", CommandTimeout.ToString());
			writer.WriteStartElement("Parameters", Namespaces.Data);
			if(Parameters != null)
				foreach (System.Xml.Serialization.IXmlSerializable parameter in Parameters)
				{
					writer.WriteStartElement("Parameter", Namespaces.Data);
					parameter.WriteXml(writer);
					writer.WriteEndElement();
				}
			writer.WriteEndElement();
			writer.WriteStartElement("Conditional");
			if(Conditional != null)
				((System.Xml.Serialization.IXmlSerializable)Conditional).WriteXml(writer);
			writer.WriteEndElement();
			writer.WriteStartElement("AlternativeActions");
			foreach (System.Xml.Serialization.IXmlSerializable action in AlternativeActions)
			{
				writer.WriteStartElement("PersistenceAction", Namespaces.Data);
				action.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("BeforeActions");
			foreach (System.Xml.Serialization.IXmlSerializable action in BeforeActions)
			{
				writer.WriteStartElement("PersistenceAction", Namespaces.Data);
				action.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("AfterActions");
			foreach (System.Xml.Serialization.IXmlSerializable action in AfterActions)
			{
				writer.WriteStartElement("PersistenceAction", Namespaces.Data);
				action.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			WriteItem<Colosoft.Query.StoredProcedureName>(writer, "StoredProcedureName", _storedProcedureName ?? Colosoft.Query.StoredProcedureName.Empty);
			writer.WriteStartElement("Query");
			if(Query != null)
				((System.Xml.Serialization.IXmlSerializable)Query).WriteXml(writer);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Desserializa o objeto.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(Serialization.IO.CompactReader reader)
		{
			ActionId = reader.ReadInt32();
			Type = (PersistenceActionType)reader.ReadInt32();
			EntityFullName = reader.ReadString();
			ProviderName = reader.ReadString();
			var RowVersionString = reader.ReadString();
			if(!string.IsNullOrEmpty(RowVersionString))
				RowVersion = long.Parse(RowVersionString);
			else
				RowVersion = null;
			var parameters = new List<PersistenceParameter>();
			var count = reader.ReadInt32();
			for(var i = 0; i < count; i++)
			{
				var parameter = new PersistenceParameter();
				((ICompactSerializable)parameter).Deserialize(reader);
				parameters.Add(parameter);
			}
			Parameters = new PersistenceParameterCollection(parameters);
			if(reader.ReadBoolean())
			{
				var conditional = new Colosoft.Query.ConditionalContainer();
				((ICompactSerializable)conditional).Deserialize(reader);
				this.Conditional = conditional;
			}
			var actions = new List<PersistenceAction>();
			count = reader.ReadInt32();
			for(var i = 0; i < count; i++)
			{
				var persistenceAction = new PersistenceAction();
				((ICompactSerializable)persistenceAction).Deserialize(reader);
				actions.Add(persistenceAction);
			}
			AlternativeActions.AddRange(actions);
			var beforeActions = new List<PersistenceAction>();
			count = reader.ReadInt32();
			for(var i = 0; i < count; i++)
			{
				var persistenceAction = new PersistenceAction();
				((ICompactSerializable)persistenceAction).Deserialize(reader);
				beforeActions.Add(persistenceAction);
			}
			BeforeActions.AddRange(beforeActions);
			var afterActions = new List<PersistenceAction>();
			count = reader.ReadInt32();
			for(var i = 0; i < count; i++)
			{
				var persistenceAction = new PersistenceAction();
				((ICompactSerializable)persistenceAction).Deserialize(reader);
				afterActions.Add(persistenceAction);
			}
			AfterActions.AddRange(afterActions);
			if(reader.ReadBoolean())
			{
				_storedProcedureName = new Colosoft.Query.StoredProcedureName();
				((ICompactSerializable)_storedProcedureName).Deserialize(reader);
			}
			if(reader.ReadBoolean())
			{
				var query = new Colosoft.Query.QueryInfo();
				((ICompactSerializable)query).Deserialize(reader);
				this.Query = query;
			}
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Serialization.IO.CompactWriter writer)
		{
			writer.Write(ActionId);
			writer.Write((int)Type);
			writer.Write(EntityFullName);
			writer.Write(ProviderName);
			writer.Write((RowVersion.HasValue) ? RowVersion.ToString() : "");
			if(Parameters != null)
			{
				writer.Write(Parameters.Count);
				foreach (ICompactSerializable parameter in Parameters)
					parameter.Serialize(writer);
			}
			else
				writer.Write(0);
			if(Conditional != null)
			{
				writer.Write(true);
				((ICompactSerializable)Conditional).Serialize(writer);
			}
			else
				writer.Write(false);
			writer.Write(AlternativeActions.Count);
			foreach (ICompactSerializable action in AlternativeActions)
				action.Serialize(writer);
			writer.Write(BeforeActions.Count);
			foreach (ICompactSerializable action in BeforeActions)
				action.Serialize(writer);
			writer.Write(AfterActions.Count);
			foreach (ICompactSerializable action in AfterActions)
				action.Serialize(writer);
			if(_storedProcedureName != null)
			{
				writer.Write(true);
				((ICompactSerializable)_storedProcedureName).Serialize(writer);
			}
			else
			{
				writer.Write(false);
			}
			if(Query != null)
			{
				writer.Write(true);
				((ICompactSerializable)Query).Serialize(writer);
			}
			else
				writer.Write(false);
		}

		/// <summary>
		/// Escreve os dados do item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="writer"></param>
		/// <param name="itemName">Nome do item.</param>
		/// <param name="item">Instancia do item.</param>
		private static void WriteItem<T>(System.Xml.XmlWriter writer, string itemName, T item) where T : System.Xml.Serialization.IXmlSerializable
		{
			writer.WriteStartElement(itemName, Namespaces.Query);
			if(item != null)
				((System.Xml.Serialization.IXmlSerializable)item).WriteXml(writer);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Recupera o item do Reader.
		/// </summary>
		/// <typeparam name="T">Tipo da instancia que será preechida.</typeparam>
		/// <param name="reader"></param>
		/// <param name="itemName">Nome do item.</param>
		private static T ReadItem<T>(System.Xml.XmlReader reader, string itemName) where T : System.Xml.Serialization.IXmlSerializable, new()
		{
			if(reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == itemName && (!reader.IsEmptyElement || reader.HasAttributes))
			{
				T item = new T();
				item.ReadXml(reader);
				return item;
			}
			else
				reader.Skip();
			return default(T);
		}

		/// <summary>
		/// Notifica que a execução foi realizada com sucesso.
		/// </summary>
		public void NotifyExecution()
		{
			if(Executed != null)
				Executed(this, new PersistenceActionExecutedArgs(true, null));
		}

		/// <summary>
		/// Notifica um erro na execução
		/// </summary>
		/// <param name="error"></param>
		public void NotifyError(Exception error)
		{
			if(Executed != null)
				Executed(this, new PersistenceActionExecutedArgs(false, error));
		}

		/// <summary>
		/// Recupera o texto que representa a ação.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Uid: {0}, Type: {1}, Entity: {2}, RowVersion: {3}, Param: {4}, Conditional: {5}]", this.ActionId, this.Type, this.EntityFullName, this.RowVersion, this.Parameters != null ? this.Parameters.Count : 0, Conditional);
		}
	}
}
