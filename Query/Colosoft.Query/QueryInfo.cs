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

namespace Colosoft.Query
{
	/// <summary>
	/// Tipos dos métodos de consulta.
	/// </summary>
	public enum QueryMethod
	{
		/// <summary>
		/// Consulta do tipo de seleção.
		/// </summary>
		Select,
		/// <summary>
		/// Consulta para recuperar a quantidade de registros.
		/// </summary>
		Count,
		/// <summary>
		/// Consulta para recuperar a soma de uma das colunas do resulta.
		/// </summary>
		Sum,
		/// <summary>
		/// Consulta para recuperar o valor máximo de uma coluna.
		/// </summary>
		Max
	}
	/// <summary>
	/// Argumentos do evento de processamento das consultas aninhada.
	/// </summary>
	public class NestedQueriesProcessedEventArgs : EventArgs
	{
		private int _recordsCount;

		/// <summary>
		/// Quantidade de registro processados.
		/// </summary>
		public int RecordsCount
		{
			get
			{
				return _recordsCount;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="recordsCount">Quantidade de registros processados.</param>
		public NestedQueriesProcessedEventArgs(int recordsCount)
		{
			_recordsCount = recordsCount;
		}
	}
	/// <summary>
	/// Argumentos do evento acioando quando um consulta aninhada for processada.
	/// </summary>
	public class NestedQueryProcessedEventArgs : EventArgs
	{
		private int _recordPosition;

		private QueryInfo _nestedQuery;

		/// <summary>
		/// Posição do registro que está sendo processado.
		/// </summary>
		public int RecordPosition
		{
			get
			{
				return _recordPosition;
			}
		}

		/// <summary>
		/// Instancia da consulta aninhada.
		/// </summary>
		public QueryInfo NestedQuery
		{
			get
			{
				return _nestedQuery;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="nestedQuery">Consulta aninhada associada.</param>
		/// <param name="recordPosition">Posição do registro que está sendo processado.</param>
		public NestedQueryProcessedEventArgs(QueryInfo nestedQuery, int recordPosition)
		{
			_nestedQuery = nestedQuery;
			_recordPosition = recordPosition;
		}
	}
	/// <summary>
	/// Argumentos do evento acioando quando um registros das consultas aninhas for processado.
	/// </summary>
	public class NestedQueriesRecordProcessedEventArgs : EventArgs
	{
		private int _recordPosition;

		/// <summary>
		/// Posição do registro que está sendo processado.
		/// </summary>
		public int RecordPosition
		{
			get
			{
				return _recordPosition;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="recordPosition">Posição do registro que está sendo processado.</param>
		public NestedQueriesRecordProcessedEventArgs(int recordPosition)
		{
			_recordPosition = recordPosition;
		}
	}
	/// <summary>
	/// Assinatura do evento acionado quando as consultas aninhadas forem processadas.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void NestedQueriesProcessedHandler (object sender, NestedQueriesProcessedEventArgs e);
	/// <summary>
	/// Assinatura do evento acionado quando um consulta aninhada for processada.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void NestedQueryProcessedHandler (object sender, NestedQueryProcessedEventArgs e);
	/// <summary>
	/// Assinatura do evento acionado quando um registros das consultas aninhadas for processado.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void NestedQueriesRecordProcessedHandler (object sender, NestedQueriesRecordProcessedEventArgs e);
	/// <summary>
	/// Informa
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public sealed class QueryInfo : ISerializable, System.Xml.Serialization.IXmlSerializable, ICompactSerializable, ICloneable
	{
		private static int _defaultCommandTimeout = 90;

		private static readonly QueryInfo[] NestedQueriesEmpty = new QueryInfo[0];

		private static int _generalUidCounter = 0;

		private int _id = _generalUidCounter++;

		[NonSerialized]
		private IQueryExecuteObserver _executeObserver;

		[NonSerialized]
		private IQueryResultObserver _resultObserver;

		private bool _hasRowVersion = false;

		private StoredProcedureName _storedProcedureName;

		private string _storedProcedureProvider;

		private bool _isSelectDistinct = false;

		private bool _ignoreRegisterUserInfo;

		private QueryParameterCollection _parameters;

		private QueryMethod _method;

		private Projection _projection;

		private QueryExecutePredicate _executePredicate;

		private EntityInfo[] _entities;

		private JoinInfo[] _joins;

		private GroupBy _groupby;

		private ConditionalContainer _having;

		private Sort _sort;

		private ConditionalContainer _whereClause;

		private TakeParameters _takeParameters;

		private QueryInfo[] _nestedQueries;

		private UnionInfoCollection _unions;

		[NonSerialized]
		private bool _canUseCache = true;

		private bool _ignoreTypeSchema;

		private string _providerName;

		private System.Transactions.IsolationLevel _isolationLevel = System.Transactions.IsolationLevel.Unspecified;

		private int _commandTimeout = DefaultCommandTimeout;

		/// <summary>
		/// Evento acioando quando as consultas aninhadas forem processadas.
		/// </summary>
		public event NestedQueriesProcessedHandler NestedQueriesProcessed;

		/// <summary>
		/// Evento acioando quando uma consulta aninhada for processada.
		/// </summary>
		public event NestedQueryProcessedHandler NestedQueryProcessed;

		/// <summary>
		/// Evento acionado quando um registro das consultas aninhadas for processado.
		/// </summary>
		public event NestedQueriesRecordProcessedHandler NestedQueriesRecordProcessed;

		/// <summary>
		/// Identifica se é para ignorar o esquema de tipo na consulta.
		/// </summary>
		public bool IgnoreTypeSchema
		{
			get
			{
				return _ignoreTypeSchema;
			}
			set
			{
				_ignoreTypeSchema = value;
			}
		}

		/// <summary>
		/// Nível de isolamento da consulta.
		/// </summary>
		public System.Transactions.IsolationLevel IsolationLevel
		{
			get
			{
				return _isolationLevel;
			}
			set
			{
				_isolationLevel = value;
			}
		}

		/// <summary>
		/// Tempo de espera padrão para a execução de um comando.
		/// </summary>
		public static int DefaultCommandTimeout
		{
			get
			{
				return QueryInfo._defaultCommandTimeout;
			}
			set
			{
				QueryInfo._defaultCommandTimeout = value;
			}
		}

		/// <summary>
		/// Tempo de espera antes de terminar a tentativa de executar um comando e de gerar um erro. (em segundos)
		/// </summary>
		/// <value>O tempo em segundos de esperar o comando executar. O padrão é 30 segundos.</value>
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
		/// Identifica se pode usar o cache para obter o resultado da consulta.
		/// </summary>
		public bool CanUseCache
		{
			get
			{
				return _canUseCache && ((_storedProcedureName == null) || String.IsNullOrWhiteSpace(_storedProcedureName.Name)) && (!_entities.IsNullOrEmpty()) && _whereClause.Conditionals.All(c => ConditionalTerm.HasNoFunctionCall(c));
			}
			set
			{
				_canUseCache = value;
			}
		}

		/// <summary>
		/// Predicado usado na validação da execução da consulta.
		/// </summary>
		public QueryExecutePredicate ExecutePredicate
		{
			get
			{
				return _executePredicate;
			}
			set
			{
				_executePredicate = value;
			}
		}

		/// <summary>
		/// Identificador unico das informações da consulta.
		/// </summary>
		public int Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		/// <summary>
		/// Instancia usada para observar a execução da consulta.
		/// </summary>
		public IQueryExecuteObserver ExecuteObserver
		{
			get
			{
				return _executeObserver;
			}
			set
			{
				_executeObserver = value;
			}
		}

		/// <summary>
		/// Instancia usada para observar o resultado da consulta.
		/// </summary>
		public IQueryResultObserver ResultObserver
		{
			get
			{
				return _resultObserver;
			}
			set
			{
				_resultObserver = value;
			}
		}

		/// <summary>
		/// Nome da StoredProcedure a ser executada ou nulo.
		/// </summary>
		public StoredProcedureName StoredProcedureName
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
		/// Nome do provedor de configurações da StoredProcedure.
		/// </summary>
		public string StoredProcedureProvider
		{
			get
			{
				return _storedProcedureProvider;
			}
			set
			{
				_storedProcedureProvider = value;
			}
		}

		/// <summary>
		/// Define se consulta é com Select Distinct.
		/// </summary>
		public bool IsSelectDistinct
		{
			get
			{
				return _isSelectDistinct;
			}
			set
			{
				_isSelectDistinct = value;
			}
		}

		/// <summary>
		/// Identifica se é para ignorar o registro das informações do usuário
		/// na execução da consulta.
		/// </summary>
		public bool IgnoreRegisterUserInfo
		{
			get
			{
				return _ignoreRegisterUserInfo;
			}
			set
			{
				_ignoreRegisterUserInfo = value;
			}
		}

		/// <summary>
		/// Método que será usado na consulta.
		/// </summary>
		public QueryMethod Method
		{
			get
			{
				return _method;
			}
			set
			{
				_method = value;
			}
		}

		/// <summary>
		/// Projeção da consulta.
		/// </summary>
		public Projection Projection
		{
			get
			{
				return _projection;
			}
			set
			{
				_projection = value;
			}
		}

		/// <summary>
		/// Relação das entidades envolvidas na consulta.
		/// </summary>
		public EntityInfo[] Entities
		{
			get
			{
				return _entities;
			}
			set
			{
				_entities = value;
			}
		}

		/// <summary>
		/// Joins da consulta.
		/// </summary>
		public JoinInfo[] Joins
		{
			get
			{
				return _joins;
			}
			set
			{
				_joins = value;
			}
		}

		/// <summary>
		/// GrouBys da consulta
		/// </summary>
		public GroupBy GroupBy
		{
			get
			{
				return _groupby;
			}
			set
			{
				_groupby = value;
			}
		}

		/// <summary>
		/// Having da consulta.
		/// </summary>
		public ConditionalContainer Having
		{
			get
			{
				return _having;
			}
			set
			{
				_having = value;
			}
		}

		/// <summary>
		/// Ordenação da consulta.
		/// </summary>
		public Sort Sort
		{
			get
			{
				return _sort;
			}
			set
			{
				_sort = value;
			}
		}

		/// <summary>
		/// Clausula condicional da consulta.
		/// </summary>
		public ConditionalContainer WhereClause
		{
			get
			{
				return _whereClause;
			}
			set
			{
				_whereClause = value;
			}
		}

		/// <summary>
		/// Parametros usados na recuperação do resultado da consulta.
		/// </summary>
		public TakeParameters TakeParameters
		{
			get
			{
				return _takeParameters;
			}
			set
			{
				_takeParameters = value;
			}
		}

		/// <summary>
		/// Parametros da consulta.
		/// </summary>
		public QueryParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}

		/// <summary>
		/// Consultas aninhadas.
		/// </summary>
		public QueryInfo[] NestedQueries
		{
			get
			{
				return _nestedQueries ?? NestedQueriesEmpty;
			}
			set
			{
				_nestedQueries = value;
			}
		}

		/// <summary>
		/// Define se a consulta retorna ou não o campo RowVersion.
		/// </summary>
		[XmlIgnore]
		public bool HasRowVersion
		{
			get
			{
				return _hasRowVersion;
			}
			set
			{
				_hasRowVersion = value;
			}
		}

		/// <summary>
		/// Uniões da consulta.
		/// </summary>
		public UnionInfoCollection Unions
		{
			get
			{
				return _unions;
			}
			set
			{
				_unions = value;
			}
		}

		/// <summary>
		/// Nome do provedor que será usado pela consulta.
		/// </summary>
		public string ProviderName
		{
			get
			{
				return _providerName;
			}
			set
			{
				_providerName = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public QueryInfo()
		{
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private QueryInfo(SerializationInfo info, StreamingContext context)
		{
			_id = info.GetInt32("Id");
			var methodText = info.GetString("Method");
			Method = (QueryMethod)Enum.Parse(typeof(QueryMethod), methodText);
			_projection = (Projection)info.GetValue("Projection", typeof(Projection));
			_entities = (EntityInfo[])info.GetValue("Entities", typeof(EntityInfo[]));
			_executePredicate = (QueryExecutePredicate)info.GetValue("ExecutePredicate", typeof(QueryExecutePredicate));
			_groupby = (GroupBy)info.GetValue("GroupBy", typeof(GroupBy));
			_having = (ConditionalContainer)info.GetValue("Having", typeof(ConditionalContainer));
			_sort = (Sort)info.GetValue("Sort", typeof(Sort));
			_whereClause = (ConditionalContainer)info.GetValue("WhereClause", typeof(ConditionalContainer));
			_takeParameters = (TakeParameters)info.GetValue("TakeParameters", typeof(TakeParameters));
			_parameters = (QueryParameterCollection)info.GetValue("Parameters", typeof(QueryParameterCollection));
			_nestedQueries = (QueryInfo[])info.GetValue("NestedQueries", typeof(QueryInfo[]));
			_unions = (UnionInfoCollection)info.GetValue("Unions", typeof(UnionInfoCollection));
			_storedProcedureName = (StoredProcedureName)info.GetValue("StoredProcedureName", typeof(StoredProcedureName));
			_storedProcedureProvider = info.GetString("StoredProcedureProvider");
			_ignoreRegisterUserInfo = info.GetBoolean("IgnoreRegisterUserInfo");
			_isolationLevel = (System.Transactions.IsolationLevel)info.GetInt16("IsolationLevel");
			_commandTimeout = info.GetInt32("CommandTimeout");
			_providerName = info.GetString("ProviderName");
			_ignoreTypeSchema = info.GetBoolean("IgnoreTypeSchema");
		}

		/// <summary>
		/// Verifica se pode executar a consulta com base no 
		/// registro da consulta pai.
		/// </summary>
		/// <param name="parentRecord">Reigstro com os dados do resultado da consulta pai.</param>
		/// <returns></returns>
		public bool CanExecute(IRecord parentRecord)
		{
			if(ExecutePredicate == null)
				return true;
			return ExecutePredicate.Verify(parentRecord);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("Uid : {0} ", Id);
			ToString2(sb, 0);
			return sb.ToString();
		}

		/// <summary>
		/// Recupera o texto que repreesnta a instancia.
		/// </summary>
		/// <param name="sb">StringBuilder para escrever o texto.</param>
		/// <param name="level"></param>
		/// <returns></returns>
		private void ToString2(StringBuilder sb, int level)
		{
			sb.Append('-', level * 3);
			sb.AppendFormat("From {0}", Entities != null ? Entities.FirstOrDefault() : null);
			if(Entities != null && Entities.Length > 1)
				sb.AppendFormat(" ENTITIES ({0})", string.Join(",", Entities.Skip(1).Select(f => f.ToString()).ToArray()));
			if(Joins != null && Joins.Length > 0)
				sb.AppendFormat(" JOINS ({0})", string.Join(",", Joins.Select(f => f.ToString()).ToArray()));
			if(_whereClause != null)
			{
				var whereExpression = _whereClause.ToString();
				if(!string.IsNullOrEmpty(whereExpression))
					sb.AppendFormat(" WHERE ({0})", whereExpression);
			}
			if(Projection != null)
				sb.AppendFormat(" SELECT ({0})", Projection);
			if(_nestedQueries != null)
				foreach (var i in _nestedQueries)
				{
					sb.AppendLine();
					i.ToString2(sb, level + 1);
				}
		}

		/// <summary>
		/// Finaliza o processo da consultas aninhadas.
		/// </summary>
		/// <param name="recordsCount">Quantidade de registros para processar as consulta aninhadas.</param>
		public void FinishNestedQueriesProcess(int recordsCount)
		{
			if(NestedQueriesProcessed != null)
				NestedQueriesProcessed(this, new NestedQueriesProcessedEventArgs(recordsCount));
		}

		/// <summary>
		/// Finaliza o processo do registro das consultas aninhadas.
		/// </summary>
		/// <param name="recordPosition"></param>
		public void FinishNestedQueriesRecordProcess(int recordPosition)
		{
			if(NestedQueriesRecordProcessed != null)
				NestedQueriesRecordProcessed(this, new NestedQueriesRecordProcessedEventArgs(recordPosition));
		}

		/// <summary>
		/// Notifica que uma consulta aninhada for processada.
		/// </summary>
		/// <param name="nestedQuery">Informações da consulta aninhada.</param>
		/// <param name="recordPosition">Posição do registro que está sendo processado.</param>
		public void NotifyNestedQueryProcessed(QueryInfo nestedQuery, int recordPosition)
		{
			if(NestedQueryProcessed != null)
				NestedQueryProcessed(this, new NestedQueryProcessedEventArgs(nestedQuery, recordPosition));
		}

		/// <summary>
		/// Recupera o esquema do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("QueryInfo", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			var attribute = reader.GetAttribute("Id");
			int.TryParse(attribute, out _id);
			attribute = reader.GetAttribute("Method");
			if(!string.IsNullOrEmpty(attribute))
				Enum.TryParse<QueryMethod>(attribute, out _method);
			attribute = reader.GetAttribute("IsSelectDistinct");
			bool.TryParse(attribute, out _isSelectDistinct);
			attribute = reader.GetAttribute("IgnoreRegisterUserInfo");
			bool.TryParse(attribute, out _ignoreRegisterUserInfo);
			attribute = reader.GetAttribute("IsolationLevel");
			if(!Enum.TryParse<System.Transactions.IsolationLevel>(attribute, out _isolationLevel))
				_isolationLevel = System.Transactions.IsolationLevel.Unspecified;
			attribute = reader.GetAttribute("CommandTimeout");
			if(!int.TryParse(attribute, out _commandTimeout))
				_commandTimeout = 30;
			_providerName = reader.GetAttribute("ProviderName");
			_storedProcedureProvider = reader.GetAttribute("StoredProcedureProvider");
			attribute = reader.GetAttribute("IgnoreTypeSchema");
			bool.TryParse(attribute, out _ignoreTypeSchema);
			reader.ReadStartElement();
			_projection = ReadItem<Projection>(reader, "Projection");
			_executePredicate = ReadItem<QueryExecutePredicate>(reader, "ExecutePredicate");
			var entitiesQueue = new Queue<EntityInfo>();
			if(!reader.IsEmptyElement && reader.LocalName == "Entities")
			{
				reader.ReadStartElement("Entities", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var join = new EntityInfo();
					((System.Xml.Serialization.IXmlSerializable)join).ReadXml(reader);
					entitiesQueue.Enqueue(join);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			_entities = entitiesQueue.ToArray();
			var joinsQueue = new Queue<JoinInfo>();
			if(!reader.IsEmptyElement && reader.LocalName == "Joins")
			{
				reader.ReadStartElement("Joins", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var join = new JoinInfo();
					((System.Xml.Serialization.IXmlSerializable)join).ReadXml(reader);
					joinsQueue.Enqueue(join);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			_joins = joinsQueue.ToArray();
			_whereClause = ReadItem<ConditionalContainer>(reader, "Where");
			_sort = ReadItem<Sort>(reader, "Sort");
			_groupby = ReadItem<GroupBy>(reader, "GroupBy");
			_having = ReadItem<ConditionalContainer>(reader, "Having");
			_takeParameters = ReadItem<TakeParameters>(reader, "TakeParameters");
			var nestedQueries = new List<QueryInfo>();
			if(!reader.IsEmptyElement && reader.LocalName == "NestedQueries")
			{
				reader.ReadStartElement("NestedQueries", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var queryInfo = new QueryInfo();
					((System.Xml.Serialization.IXmlSerializable)queryInfo).ReadXml(reader);
					nestedQueries.Add(queryInfo);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			_nestedQueries = nestedQueries.ToArray();
			_parameters = new QueryParameterCollection();
			if(!reader.IsEmptyElement && reader.LocalName == "Parameters")
			{
				reader.ReadStartElement("Parameters", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var parameter = new QueryParameter();
					((System.Xml.Serialization.IXmlSerializable)parameter).ReadXml(reader);
					_parameters.Add(parameter);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			_storedProcedureName = ReadItem<StoredProcedureName>(reader, "StoredProcedureName");
			_unions = new UnionInfoCollection();
			if(!reader.IsEmptyElement && reader.LocalName == "Unions")
			{
				reader.ReadStartElement("Unions", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var union = new UnionInfo();
					((System.Xml.Serialization.IXmlSerializable)union).ReadXml(reader);
					_unions.Add(union);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			reader.ReadEndElement();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("xmlns", "i", null, Namespaces.SchemaInstance);
			writer.WriteAttributeString("xmlns", "q", null, Namespaces.Query);
			writer.WriteAttributeString("Id", _id.ToString());
			writer.WriteAttributeString("Method", _method.ToString());
			writer.WriteAttributeString("StoredProcedureProvider", _storedProcedureProvider ?? "");
			writer.WriteAttributeString("IsSelectDistinct", IsSelectDistinct.ToString());
			writer.WriteAttributeString("IgnoreRegisterUserInfo", IgnoreRegisterUserInfo.ToString());
			writer.WriteAttributeString("IsolationLevel", IsolationLevel.ToString());
			writer.WriteAttributeString("CommandTimeout", CommandTimeout.ToString());
			writer.WriteAttributeString("ProviderName", ProviderName.ToString());
			writer.WriteAttributeString("IgnoreTypeSchema", IgnoreTypeSchema.ToString());
			WriteItem<Projection>(writer, "Projection", _projection);
			WriteItem<QueryExecutePredicate>(writer, "ExecutePredicate", _executePredicate);
			writer.WriteStartElement("Entities", Namespaces.Query);
			if(_entities != null)
				foreach (System.Xml.Serialization.IXmlSerializable i in _entities)
				{
					writer.WriteStartElement("EntityInfo", Namespaces.Query);
					i.WriteXml(writer);
					writer.WriteEndElement();
				}
			writer.WriteEndElement();
			writer.WriteStartElement("Joins", Namespaces.Query);
			if(_joins != null)
				foreach (System.Xml.Serialization.IXmlSerializable i in _joins)
				{
					writer.WriteStartElement("JoinInfo", Namespaces.Query);
					i.WriteXml(writer);
					writer.WriteEndElement();
				}
			writer.WriteEndElement();
			WriteItem<ConditionalContainer>(writer, "Where", _whereClause);
			WriteItem<Sort>(writer, "Sort", _sort);
			WriteItem<GroupBy>(writer, "GroupBy", _groupby);
			WriteItem<ConditionalContainer>(writer, "Having", _having);
			WriteItem<TakeParameters>(writer, "TakeParameters", _takeParameters);
			writer.WriteStartElement("NestedQueries", Namespaces.Query);
			if(_nestedQueries != null)
			{
				foreach (System.Xml.Serialization.IXmlSerializable i in _nestedQueries)
				{
					writer.WriteStartElement("QueryInfo", Namespaces.Query);
					i.WriteXml(writer);
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
			writer.WriteStartElement("Parameters", Namespaces.Query);
			if(_parameters != null)
				foreach (System.Xml.Serialization.IXmlSerializable i in _parameters)
				{
					writer.WriteStartElement("QueryParameter", Namespaces.Query);
					i.WriteXml(writer);
					writer.WriteEndElement();
				}
			writer.WriteEndElement();
			WriteItem<StoredProcedureName>(writer, "StoredProcedureName", _storedProcedureName ?? StoredProcedureName.Empty);
			writer.WriteStartElement("Unions", Namespaces.Query);
			if(_unions != null)
				foreach (System.Xml.Serialization.IXmlSerializable i in _unions)
				{
					writer.WriteStartElement("Union", Namespaces.Query);
					i.WriteXml(writer);
					writer.WriteEndElement();
				}
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
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Id", _id);
			info.AddValue("Method", Method.ToString());
			info.AddValue("Entities", Entities);
			info.AddValue("Projection", Projection);
			info.AddValue("ExecutePredicate", ExecutePredicate);
			info.AddValue("GroupBy", GroupBy);
			info.AddValue("Having", Having);
			info.AddValue("Sort", Sort);
			info.AddValue("WhereClause", WhereClause);
			info.AddValue("TakeParameters", TakeParameters);
			info.AddValue("Parameters", Parameters);
			info.AddValue("NestedQueries", NestedQueries);
			info.AddValue("StoredProcedureName", StoredProcedureName);
			info.AddValue("StoredProcedureProvider", StoredProcedureProvider);
			info.AddValue("IgnoreRegisterUserInfo", IgnoreRegisterUserInfo);
			info.AddValue("IsolationLevel", (short)IsolationLevel);
			info.AddValue("CommandTimeout", CommandTimeout);
			info.AddValue("ProviderName", ProviderName);
			info.AddValue("IgnoreTypeSchema", IgnoreTypeSchema);
		}

		/// <summary>
		/// Desserializa o objeto.
		/// </summary>
		/// <param name="reader"></param>
		void ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			_id = reader.ReadInt32();
			_isSelectDistinct = reader.ReadBoolean();
			_ignoreRegisterUserInfo = reader.ReadBoolean();
			try
			{
				Enum.TryParse<QueryMethod>(reader.ReadString(), out _method);
			}
			catch(OutOfMemoryException)
			{
				throw;
			}
			_isolationLevel = (System.Transactions.IsolationLevel)reader.ReadInt16();
			_commandTimeout = reader.ReadInt32();
			_providerName = reader.ReadString();
			_ignoreTypeSchema = reader.ReadBoolean();
			if(reader.ReadBoolean())
				_storedProcedureProvider = reader.ReadString();
			if(reader.ReadBoolean())
			{
				_projection = new Projection();
				((ICompactSerializable)_projection).Deserialize(reader);
			}
			if(reader.ReadBoolean())
			{
				_executePredicate = new QueryExecutePredicate();
				((ICompactSerializable)_executePredicate).Deserialize(reader);
			}
			var entitiesQueue = new Queue<EntityInfo>();
			while (reader.ReadBoolean())
			{
				var entity = new EntityInfo();
				((ICompactSerializable)entity).Deserialize(reader);
				entitiesQueue.Enqueue(entity);
			}
			_entities = entitiesQueue.ToArray();
			var joinQueue = new Queue<JoinInfo>();
			while (reader.ReadBoolean())
			{
				var join = new JoinInfo();
				((ICompactSerializable)join).Deserialize(reader);
				joinQueue.Enqueue(join);
			}
			_joins = joinQueue.ToArray();
			if(reader.ReadBoolean())
			{
				_whereClause = new ConditionalContainer();
				((ICompactSerializable)_whereClause).Deserialize(reader);
			}
			if(reader.ReadBoolean())
			{
				_sort = new Sort();
				((ICompactSerializable)_sort).Deserialize(reader);
			}
			if(reader.ReadBoolean())
			{
				_groupby = new GroupBy();
				((ICompactSerializable)_groupby).Deserialize(reader);
			}
			if(reader.ReadBoolean())
			{
				_having = new ConditionalContainer();
				((ICompactSerializable)_having).Deserialize(reader);
			}
			if(reader.ReadBoolean())
			{
				_takeParameters = new TakeParameters();
				((ICompactSerializable)_takeParameters).Deserialize(reader);
			}
			var nestedQueries = new List<QueryInfo>();
			while (reader.ReadBoolean())
			{
				var nestedQuery = new QueryInfo();
				((ICompactSerializable)nestedQuery).Deserialize(reader);
				nestedQueries.Add(nestedQuery);
			}
			_nestedQueries = nestedQueries.ToArray();
			if(_parameters == null)
				_parameters = new QueryParameterCollection();
			while (reader.ReadBoolean())
			{
				var parameter = new QueryParameter();
				parameter.Deserialize(reader);
				_parameters.Add(parameter);
			}
			if(reader.ReadBoolean())
			{
				_storedProcedureName = new StoredProcedureName();
				((ICompactSerializable)_storedProcedureName).Deserialize(reader);
			}
			if(_unions == null)
				_unions = new UnionInfoCollection();
			while (reader.ReadBoolean())
			{
				var union = new UnionInfo();
				union.Deserialize(reader);
				_unions.Add(union);
			}
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		void ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(_id);
			writer.Write(_isSelectDistinct);
			writer.Write(_ignoreRegisterUserInfo);
			writer.Write(_method.ToString());
			writer.Write((short)_isolationLevel);
			writer.Write(_commandTimeout);
			writer.Write(_providerName);
			writer.Write(_ignoreTypeSchema);
			if(!string.IsNullOrEmpty(_storedProcedureProvider))
			{
				writer.Write(true);
				writer.Write(_storedProcedureProvider);
			}
			else
			{
				writer.Write(false);
			}
			if(_projection != null)
			{
				writer.Write(true);
				((ICompactSerializable)_projection).Serialize(writer);
			}
			else
			{
				writer.Write(false);
			}
			if(_executePredicate != null)
			{
				writer.Write(true);
				((ICompactSerializable)_executePredicate).Serialize(writer);
			}
			else
			{
				writer.Write(false);
			}
			if(_entities != null)
			{
				foreach (ICompactSerializable i in _entities)
				{
					writer.Write(true);
					i.Serialize(writer);
				}
			}
			writer.Write(false);
			if(_joins != null)
			{
				foreach (ICompactSerializable i in _joins)
				{
					writer.Write(true);
					i.Serialize(writer);
				}
			}
			writer.Write(false);
			if(_whereClause != null)
			{
				writer.Write(true);
				((ICompactSerializable)_whereClause).Serialize(writer);
			}
			else
			{
				writer.Write(false);
			}
			if(_sort != null)
			{
				writer.Write(true);
				((ICompactSerializable)_sort).Serialize(writer);
			}
			else
			{
				writer.Write(false);
			}
			if(_groupby != null)
			{
				writer.Write(true);
				((ICompactSerializable)_groupby).Serialize(writer);
			}
			else
				writer.Write(false);
			if(_having != null)
			{
				writer.Write(true);
				((ICompactSerializable)_having).Serialize(writer);
			}
			else
				writer.Write(false);
			if(_takeParameters != null)
			{
				writer.Write(true);
				((ICompactSerializable)_takeParameters).Serialize(writer);
			}
			else
			{
				writer.Write(false);
			}
			if(_nestedQueries != null)
				foreach (ICompactSerializable i in _nestedQueries)
				{
					writer.Write(true);
					i.Serialize(writer);
				}
			writer.Write(false);
			if(_parameters != null)
				foreach (ICompactSerializable i in _parameters)
				{
					writer.Write(true);
					i.Serialize(writer);
				}
			writer.Write(false);
			if(_storedProcedureName != null)
			{
				writer.Write(true);
				((ICompactSerializable)_storedProcedureName).Serialize(writer);
			}
			else
			{
				writer.Write(false);
			}
			if(_unions != null)
				foreach (ICompactSerializable i in _unions)
				{
					writer.Write(true);
					i.Serialize(writer);
				}
			writer.Write(false);
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new QueryInfo {
				_id = _id,
				_hasRowVersion = this._hasRowVersion,
				_isSelectDistinct = this._isSelectDistinct,
				_ignoreRegisterUserInfo = this._ignoreRegisterUserInfo,
				_storedProcedureName = this._storedProcedureName != null ? (StoredProcedureName)this._storedProcedureName.Clone() : null,
				_storedProcedureProvider = this._storedProcedureProvider,
				_commandTimeout = this._commandTimeout,
				_isolationLevel = this._isolationLevel,
				_parameters = _parameters != null ? (QueryParameterCollection)_parameters.Clone() : null,
				_method = this._method,
				_projection = this._projection != null ? (Query.Projection)this._projection.Clone() : null,
				_executePredicate = this._executePredicate != null ? (Query.QueryExecutePredicate)this._executePredicate.Clone() : null,
				_entities = this._entities != null ? this._entities.Select(f => (EntityInfo)f.Clone()).ToArray() : null,
				_joins = this._joins != null ? this._joins.Select(f => (JoinInfo)f.Clone()).ToArray() : null,
				_groupby = this._groupby != null ? (GroupBy)this._groupby.Clone() : null,
				_having = this._having != null ? (ConditionalContainer)this._having.Clone() : null,
				_sort = this._sort != null ? (Sort)this._sort.Clone() : null,
				_whereClause = this._whereClause != null ? (ConditionalContainer)this._whereClause.Clone() : null,
				_takeParameters = this._takeParameters != null ? (TakeParameters)this._takeParameters.Clone() : null,
				_nestedQueries = this._nestedQueries != null ? (QueryInfo[])this._nestedQueries.Select(f => (QueryInfo)f.Clone()).ToArray() : null,
				_unions = _unions != null ? (UnionInfoCollection)_unions.Clone() : null,
				_executeObserver = this._executeObserver,
				_canUseCache = this._canUseCache,
				_providerName = this._providerName,
				_ignoreTypeSchema = this._ignoreTypeSchema
			};
		}
	}
}
