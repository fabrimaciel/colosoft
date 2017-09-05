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
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization;

namespace Colosoft.Query
{
	/// <summary>
	/// Representa o resultado de uma consulta.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetQueryResultSchema")]
	public sealed class QueryResult : IEnumerable<Record>, IXmlSerializable, ISerializable, Colosoft.Serialization.ICompactSerializable, IQueryResult, INestedQueryContainer, IQueryCommandContainer
	{
		[NonSerialized]
		private bool _isDisposed;

		[NonSerialized]
		private RecordEnumerator _enumerator;

		private Record.RecordDescriptor _descriptor;

		private IEnumerable<Record> _records;

		[NonSerialized]
		private ReferenceParameter[] _referenceParameters = new ReferenceParameter[0];

		[NonSerialized]
		private QueryInfo _queryInfo;

		[NonSerialized]
		private Func<QueryInfo, IQueryResult> _executer;

		[NonSerialized]
		private QueryCommand _command;

		/// <summary>
		/// Evento acionado quando o resultado for liberado.
		/// </summary>
		public event EventHandler Disposed;

		/// <summary>
		/// Instancia do comando associado.
		/// </summary>
		public QueryCommand Command
		{
			get
			{
				return _command;
			}
			set
			{
				_command = value;
			}
		}

		/// <summary>
		/// Identificador do resultado da consulta.
		/// </summary>
		public int Id
		{
			get
			{
				return _queryInfo != null ? _queryInfo.Id : 0;
			}
		}

		/// <summary>
		/// Descritor dos registros do resultado.
		/// </summary>
		public Record.RecordDescriptor Descriptor
		{
			get
			{
				return _descriptor;
			}
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public QueryResult()
		{
			_descriptor = new Record.RecordDescriptor("Empty", new Record.Field[0]);
			_records = new Record[0];
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="descriptor">Instancia do descritor do resultado.</param>
		/// <param name="records"></param>
		public QueryResult(Record.RecordDescriptor descriptor, IEnumerable<Record> records)
		{
			_descriptor = descriptor;
			_records = records;
		}

		/// <summary>
		/// Cria uma instancia com o registros encontrados e as subconsultas.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="records"></param>
		/// <param name="info">Informações da consulta.</param>
		/// <param name="executer">Ponteiro para o método responsável por executar um consulta com base no QueryInfo</param>
		public QueryResult(Record.RecordDescriptor descriptor, IEnumerable<Record> records, QueryInfo info, Func<QueryInfo, IQueryResult> executer) : this(descriptor, records)
		{
			_queryInfo = info;
			_executer = executer;
			var referenceParameters = new List<ReferenceParameter>();
			if(info != null && info.NestedQueries != null)
			{
				foreach (var query in info.NestedQueries)
				{
					if(query.Parameters != null)
						foreach (var parameter in query.Parameters)
							if(parameter.Value is ReferenceParameter)
							{
								var refParameter = (ReferenceParameter)parameter.Value;
								if(!referenceParameters.Exists(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.ColumnName, refParameter.ColumnName)))
									referenceParameters.Add(refParameter);
							}
					if(query.ExecutePredicate != null && query.ExecutePredicate.RequiredFields != null)
					{
						foreach (var requiredField in query.ExecutePredicate.RequiredFields)
						{
							var columnName = requiredField.Name;
							if(string.IsNullOrEmpty(columnName) && requiredField.Index >= 0 && requiredField.Index < descriptor.Count)
							{
								columnName = descriptor[requiredField.Index].Name;
							}
							if(!string.IsNullOrEmpty(columnName) && !referenceParameters.Exists(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.ColumnName, columnName)))
							{
								referenceParameters.Add(new ReferenceParameter(columnName));
							}
						}
					}
				}
			}
			_referenceParameters = referenceParameters.ToArray();
		}

		/// <summary>
		/// Cria a instancia com base em um queryable.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="records"></param>
		/// <param name="queryable"></param>
		internal QueryResult(Record.RecordDescriptor descriptor, IEnumerable<Record> records, Queryable queryable) : this(descriptor, records)
		{
			_queryInfo = queryable.CreateQueryInfo();
			var referenceParameters = new List<ReferenceParameter>();
			if(queryable.NestedQueries != null)
			{
				foreach (var query in queryable.NestedQueries)
				{
					foreach (var parameter in QueryParameter.GetParameters(query, false))
						if(parameter.Value is ReferenceParameter)
						{
							var refParameter = (ReferenceParameter)parameter.Value;
							if(!referenceParameters.Exists(f => f.ColumnName == refParameter.ColumnName))
								referenceParameters.Add(refParameter);
						}
					if(query.ExecutePredicate != null && query.ExecutePredicate.RequiredFields != null)
					{
						foreach (var requiredField in query.ExecutePredicate.RequiredFields)
						{
							var columnName = requiredField.Name;
							if(string.IsNullOrEmpty(columnName) && requiredField.Index >= 0 && requiredField.Index < descriptor.Count)
							{
								columnName = descriptor[requiredField.Index].Name;
							}
							if(!string.IsNullOrEmpty(columnName) && !referenceParameters.Exists(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.ColumnName, columnName)))
							{
								referenceParameters.Add(new ReferenceParameter(columnName));
							}
						}
					}
				}
			}
			_referenceParameters = referenceParameters.ToArray();
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private QueryResult(SerializationInfo info, StreamingContext context)
		{
			_descriptor = info.GetValue("Descriptor", typeof(Record.RecordDescriptor)) as Record.RecordDescriptor;
			var count = info.GetInt32("C");
			var records = new List<Record>();
			for(var i = 0; i < count; i++)
			{
				var values = new object[_descriptor.Count];
				for(var j = 0; j < _descriptor.Count; j++)
					values[j] = info.GetValue(string.Format("{0}_{1}", i, j), _descriptor[j].Type);
				records.Add(_descriptor.CreateRecord(values));
			}
			_records = records;
		}

		/// <summary>
		/// Destrutor;
		/// </summary>
		~QueryResult()
		{
			Dispose(false);
		}

		/// <summary>
		/// Recupera o esquema do registro.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetQueryResultSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new XmlQualifiedName("QueryResult", Namespaces.Query);
		}

		/// <summary>
		/// Recupera o enumerador dos itens do resultado.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Record> GetEnumerator()
		{
			if(_enumerator == null)
				_enumerator = new RecordEnumerator(this, _records.GetEnumerator());
			return _enumerator;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			if(_enumerator == null)
				_enumerator = new RecordEnumerator(this, _records.GetEnumerator());
			return _enumerator;
		}

		/// <summary>
		/// Converte para o resulta do tipo informado.
		/// </summary>
		/// <typeparam name="Model">Tipo deo modelo que será recuperado.</typeparam>
		/// <returns></returns>
		public QueryResult<Model> To<Model>()
		{
			return new QueryResult<Model>(this);
		}

		/// <summary>
		/// Converte para o resulta do tipo informado.
		/// </summary>
		/// <typeparam name="Model">Tipo deo modelo que será recuperado.</typeparam>
		/// <param name="bindStrategy">Estratégia de vinculação.</param>
		/// <param name="objectCreator"></param>
		/// <returns></returns>
		public QueryResult<Model> To<Model>(IQueryResultBindStrategy bindStrategy, IQueryResultObjectCreator objectCreator = null)
		{
			return new QueryResult<Model>(this, bindStrategy, objectCreator);
		}

		/// <summary>
		/// Processa as subconsultas do resultado.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private void ProcessNestedQueries(IEnumerable<object[]> recordsReferenceValues)
		{
			if(recordsReferenceValues == null || _queryInfo == null)
			{
				if(_queryInfo != null)
					_queryInfo.FinishNestedQueriesProcess(0);
				return;
			}
			var recordsCount = 0;
			int[] recordValuesPositions = GetRecordPositionsFromReferenceParameters();
			foreach (var refValues in recordsReferenceValues)
			{
				recordsCount++;
				IRecord referenceRecord = CreateRecordFromReferenceParameterValues(recordValuesPositions, refValues);
				foreach (var query in GetQueryInfos(refValues))
				{
					IQueryResult result = null;
					try
					{
						if(referenceRecord == null || query.CanExecute(referenceRecord))
						{
							result = _executer(query);
						}
						else
							result = new QueryResult(new Record.RecordDescriptor("EmptyResult", new Record.Field[0]), new Record[0]);
					}
					catch(Exception ex)
					{
						if(query.ExecuteObserver != null)
							query.ExecuteObserver.Error(query, new QueryFailedInfo(QueryFailedReason.Error, ex.Message.GetFormatter(), ex));
						return;
					}
					try
					{
						var values = new ReferenceParameterValueCollection();
						if(_referenceParameters != null)
							for(var i = 0; i < _referenceParameters.Length; i++)
								values.Add(new ReferenceParameterValue(_referenceParameters[i].ColumnName, refValues[i]));
						if(query.ExecuteObserver != null)
							query.ExecuteObserver.Executed(query, values, result);
					}
					finally
					{
						if(result is IDisposable)
						{
							((IDisposable)result).Dispose();
						}
						else
						{
							var enumerator = result.GetEnumerator();
							while (enumerator != null && enumerator.MoveNext())
								;
						}
					}
					_queryInfo.NotifyNestedQueryProcessed(query, recordsCount);
				}
				_queryInfo.FinishNestedQueriesRecordProcess(recordsCount);
			}
			if(_queryInfo != null)
				_queryInfo.FinishNestedQueriesProcess(recordsCount);
		}

		/// <summary>
		/// Cria o registro de dados apartir dos valores dos parametros de ferencia.
		/// </summary>
		/// <param name="recordValuesPositions">
		/// Vetor com as posições dos valores dos parametros de referencias
		/// que serão vinculados com o registro
		/// </param>
		/// <param name="refValues"></param>
		/// <returns></returns>
		private IRecord CreateRecordFromReferenceParameterValues(int[] recordValuesPositions, object[] refValues)
		{
			IRecord referenceRecord = null;
			if(_descriptor != null && recordValuesPositions != null)
			{
				var recordValues = new object[recordValuesPositions.Length];
				for(var i = 0; i < recordValuesPositions.Length; i++)
				{
					var fieldIndex = recordValuesPositions[i];
					if(fieldIndex >= 0)
					{
						recordValues[i] = refValues[fieldIndex];
					}
				}
				referenceRecord = _descriptor.CreateRecord(recordValues);
			}
			return referenceRecord;
		}

		/// <summary>
		/// Método usado para recuperar as posição do registro equivalente do 
		/// descriptor com base nos parametros de referencia.
		/// </summary>
		/// <returns></returns>
		private int[] GetRecordPositionsFromReferenceParameters()
		{
			int[] recordValuesPositions = null;
			if(_descriptor != null && _referenceParameters != null && _referenceParameters.Length > 0)
			{
				recordValuesPositions = new int[_descriptor.Count];
				for(var i = 0; i < recordValuesPositions.Length; i++)
				{
					var field = _descriptor[i];
					var fieldIndex = 0;
					for(; fieldIndex < _referenceParameters.Length; fieldIndex++)
					{
						if(StringComparer.InvariantCultureIgnoreCase.Equals(_referenceParameters[fieldIndex].ColumnName, field.Name))
						{
							break;
						}
					}
					if(fieldIndex < _referenceParameters.Length)
					{
						recordValuesPositions[i] = fieldIndex;
					}
					else
						recordValuesPositions[i] = -1;
				}
			}
			return recordValuesPositions;
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			return null;
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			var records = new List<Record>();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if(!reader.IsEmptyElement && reader.LocalName == "Descriptor")
				{
					reader.MoveToElement();
					_descriptor = new Record.RecordDescriptor();
					((System.Xml.Serialization.IXmlSerializable)_descriptor).ReadXml(reader);
				}
				else if(!reader.IsEmptyElement && reader.LocalName == "Records")
				{
					reader.ReadStartElement();
					while (reader.NodeType != XmlNodeType.EndElement)
					{
						if(reader.LocalName == "Record")
						{
							var record = new Record(_descriptor);
							((IXmlSerializable)record).ReadXml(reader);
							records.Add(record);
						}
						else
							reader.Skip();
					}
					reader.ReadEndElement();
				}
				else
					reader.Skip();
			}
			reader.ReadEndElement();
			_records = records;
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("Descriptor", Namespaces.Query);
			if(_descriptor != null)
				((System.Xml.Serialization.IXmlSerializable)_descriptor).WriteXml(writer);
			writer.WriteEndElement();
			writer.WriteStartElement("Records", Namespaces.Query);
			foreach (System.Xml.Serialization.IXmlSerializable record in _records)
			{
				writer.WriteStartElement("Record", Namespaces.Query);
				record.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Descriptor", _descriptor);
			var index = 0;
			foreach (var i in _records)
			{
				for(var j = 0; j < _descriptor.Count; j++)
					info.AddValue(string.Format("{0}_{1}", index, j), i[j].GetValue());
				index++;
			}
			info.AddValue("C", index);
		}

		/// <summary>
		/// Recupera os registros serializados no leitor informado.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static IEnumerable<Record> GetRecords(Colosoft.Serialization.IO.CompactReader reader)
		{
			var hasDescriptor = reader.ReadByte() == 1;
			if(hasDescriptor)
			{
				var descriptor = new Record.RecordDescriptor();
				((Colosoft.Serialization.ICompactSerializable)descriptor).Deserialize(reader);
				while (reader.ReadByte() == 1)
				{
					var r = new Record(descriptor);
					((Colosoft.Serialization.ICompactSerializable)r).Deserialize(reader);
					yield return r;
				}
			}
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		void Colosoft.Serialization.ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			var hasDescriptor = reader.ReadByte() == 1;
			if(hasDescriptor)
			{
				_descriptor = new Record.RecordDescriptor();
				((Colosoft.Serialization.ICompactSerializable)_descriptor).Deserialize(reader);
				var records = new List<Record>();
				while (reader.ReadByte() == 1)
				{
					var r = new Record(_descriptor);
					((Colosoft.Serialization.ICompactSerializable)r).Deserialize(reader);
					records.Add(r);
				}
				_records = records;
			}
			else
				_records = new Record[0];
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		void Colosoft.Serialization.ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write((byte)(_descriptor == null ? 0 : 1));
			if(_descriptor != null)
			{
				((Colosoft.Serialization.ICompactSerializable)_descriptor).Serialize(writer);
				foreach (var i in _records)
				{
					writer.Write((byte)1);
					((Colosoft.Serialization.ICompactSerializable)i).Serialize(writer);
				}
				writer.Write((byte)0);
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			if(!_isDisposed)
			{
				_isDisposed = true;
				if(_enumerator != null)
				{
					_enumerator.Dispose();
					_enumerator = null;
				}
				if(_records is IDisposable)
				{
					((IDisposable)_records).Dispose();
					_records = new Record[0];
				}
				if(Disposed != null)
					Disposed(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Recupera os parametros de references que devem ser usados pelo resultado.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ReferenceParameter> GetReferenceParameters()
		{
			if(_referenceParameters == null)
				return new ReferenceParameter[0];
			return _referenceParameters;
		}

		/// <summary>
		/// Recupera o resultado da consulta com base no valores da referencia.
		/// </summary>
		/// <param name="referenceValues"></param>
		/// <returns></returns>
		public IEnumerable<QueryInfo> GetQueryInfos(object[] referenceValues)
		{
			if(_queryInfo != null && _queryInfo.NestedQueries != null)
			{
				foreach (var query in _queryInfo.NestedQueries)
				{
					QueryInfo query2 = (QueryInfo)query.Clone();
					if(query2.Parameters != null)
					{
						foreach (var j in query2.Parameters)
						{
							if(j.Value is ReferenceParameter)
							{
								var refParameter = (ReferenceParameter)j.Value;
								var found = false;
								for(var i = 0; i < _referenceParameters.Length; i++)
									if(StringComparer.InvariantCultureIgnoreCase.Equals(refParameter.ColumnName, _referenceParameters[i].ColumnName))
									{
										j.Value = referenceValues[i];
										found = true;
										break;
									}
								if(!found)
								{
								}
							}
						}
					}
					yield return query2;
				}
			}
		}

		/// <summary>
		/// Recupera o resultado da consulta com base no valores da referencia.
		/// </summary>
		/// <param name="referenceValues"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IEnumerable<Tuple<QueryInfo, IQueryResult>> GetQueryResults(object[] referenceValues)
		{
			if(_queryInfo != null && _queryInfo.NestedQueries != null)
			{
				if(_executer == null)
					throw new NotSupportedException("Executer undefined");
				int[] recordValuesPositions = GetRecordPositionsFromReferenceParameters();
				var infos = GetQueryInfos(referenceValues);
				return new GetQueryResultEnumerable(this, infos, referenceValues, recordValuesPositions);
			}
			return new Tuple<QueryInfo, IQueryResult>[0];
		}

		/// <summary>
		/// Enumerador do resultado da ume consulta.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		public class RecordEnumerator : IEnumerator<Record>
		{
			private QueryResult _queryResult;

			private bool _isDisposed;

			private IEnumerator<Record> _enumerator;

			private bool _ignoreProcessNestedQueries;

			private bool _disposeQueryResult = true;

			/// <summary>
			/// Coleção que armazena os valores de referencia dos registros.
			/// </summary>
			private List<object[]> _recordsReferenceValues = new List<object[]>();

			/// <summary>
			/// Identifica se é para libera a instancia do QueryResult no final da iteração.
			/// </summary>
			public bool DisposeQueryResult
			{
				get
				{
					return _disposeQueryResult;
				}
				set
				{
					_disposeQueryResult = value;
				}
			}

			/// <summary>
			/// Instancia do atual registro.
			/// </summary>
			public Record Current
			{
				get
				{
					return _enumerator.Current;
				}
			}

			/// <summary>
			/// Instancia do atual registro.
			/// </summary>
			object System.Collections.IEnumerator.Current
			{
				get
				{
					return _enumerator.Current;
				}
			}

			/// <summary>
			/// Identifica se é para ignorar a execução de subqueries.
			/// </summary>
			public bool IgnoreProcessNestedQueries
			{
				get
				{
					return _ignoreProcessNestedQueries;
				}
				set
				{
					_ignoreProcessNestedQueries = value;
				}
			}

			/// <summary>
			/// Relação dos valores de referencia dos registros.
			/// </summary>
			public IEnumerable<object[]> RecordsReferenceValues
			{
				get
				{
					return _recordsReferenceValues;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="queryResult"></param>
			/// <param name="recordEnumerator"></param>
			internal RecordEnumerator(QueryResult queryResult, IEnumerator<Record> recordEnumerator)
			{
				_queryResult = queryResult;
				_enumerator = recordEnumerator;
			}

			/// <summary>
			/// Destrutor.
			/// </summary>
			~RecordEnumerator()
			{
				Dispose(false);
			}

			/// <summary>
			/// Libera a instancia
			/// </summary>
			/// <param name="disposing"></param>
			protected virtual void Dispose(bool disposing)
			{
				if(!IgnoreProcessNestedQueries && !_isDisposed)
				{
					_isDisposed = true;
					_enumerator.Dispose();
					_queryResult.ProcessNestedQueries(_recordsReferenceValues);
					if(DisposeQueryResult)
						_queryResult.Dispose();
				}
				else
				{
					if(DisposeQueryResult && !_isDisposed)
						_queryResult.Dispose();
				}
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
			public void Dispose()
			{
				Dispose(true);
			}

			/// <summary>
			/// Move para o próximo registro.
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				if(_enumerator.MoveNext())
				{
					if(_queryResult != null && Current != null)
						Current.QueryResult = _queryResult;
					object[] refValues = null;
					if(Current != null && _queryResult._referenceParameters != null && _queryResult._referenceParameters.Length > 0)
					{
						var refParameters = _queryResult._referenceParameters;
						refValues = new object[refParameters.Length];
						for(int i = 0; i < refParameters.Length; i++)
						{
							try
							{
								var value = Current[refParameters[i].ColumnName];
								refValues[i] = value.GetValue();
							}
							catch
							{
								throw;
							}
						}
					}
					_recordsReferenceValues.Add(refValues ?? new object[0]);
					return true;
				}
				Dispose();
				return false;
			}

			/// <summary>
			/// Reseta o enumerador.
			/// </summary>
			public void Reset()
			{
				_enumerator.Reset();
			}
		}

		/// <summary>
		/// Implementação do Enumberable para percorrer os resultados das consultas.
		/// </summary>
		class GetQueryResultEnumerable : IEnumerable<Tuple<QueryInfo, IQueryResult>>, IDisposable
		{
			private object[] _referenceValues;

			private int[] _recordValuesPositions;

			private IEnumerable<QueryInfo> _infos;

			private QueryResult _queryResult;

			private GetQueryResultEnumerator _currentEnumerator;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="queryResult"></param>
			/// <param name="infos"></param>
			/// <param name="referenceValues"></param>
			/// <param name="recordValuesPositions"></param>
			public GetQueryResultEnumerable(QueryResult queryResult, IEnumerable<QueryInfo> infos, object[] referenceValues, int[] recordValuesPositions)
			{
				_queryResult = queryResult;
				_infos = infos;
				_referenceValues = referenceValues;
				_recordValuesPositions = recordValuesPositions;
			}

			/// <summary>
			/// Recupera o enumerador associado.
			/// </summary>
			/// <returns></returns>
			public IEnumerator<Tuple<QueryInfo, IQueryResult>> GetEnumerator()
			{
				_currentEnumerator = new GetQueryResultEnumerator(this, _infos.GetEnumerator());
				return _currentEnumerator;
			}

			/// <summary>
			/// Recupera o enumerador associado.
			/// </summary>
			/// <returns></returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				_currentEnumerator = new GetQueryResultEnumerator(this, _infos.GetEnumerator());
				return _currentEnumerator;
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				if(_currentEnumerator != null)
				{
					_currentEnumerator.Dispose();
					_currentEnumerator = null;
				}
			}

			/// <summary>
			/// Implementação do Enumerator
			/// </summary>
			class GetQueryResultEnumerator : IEnumerator<Tuple<QueryInfo, IQueryResult>>
			{
				private GetQueryResultEnumerable _owner;

				private IEnumerator<QueryInfo> _infoEnumerator;

				private Tuple<QueryInfo, IQueryResult> _current;

				public Tuple<QueryInfo, IQueryResult> Current
				{
					get
					{
						return _current;
					}
				}

				object System.Collections.IEnumerator.Current
				{
					get
					{
						return _current;
					}
				}

				/// <summary>
				/// Construtor padrão.
				/// </summary>
				/// <param name="owner"></param>
				/// <param name="infoEnumerator"></param>
				public GetQueryResultEnumerator(GetQueryResultEnumerable owner, IEnumerator<QueryInfo> infoEnumerator)
				{
					_owner = owner;
					_infoEnumerator = infoEnumerator;
				}

				/// <summary>
				/// Libera a instancia.
				/// </summary>
				public void Dispose()
				{
					_infoEnumerator.Dispose();
					if(_current != null && _current.Item2 != null)
						_current.Item2.Dispose();
					_current = null;
				}

				/// <summary>
				/// Move para o próximo item.
				/// </summary>
				/// <returns></returns>
				[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
				public bool MoveNext()
				{
					if(_infoEnumerator.MoveNext())
					{
						if(_current != null && _current.Item2 != null)
							_current.Item2.Dispose();
						_current = null;
						var queryInfo = _infoEnumerator.Current;
						IRecord referenceRecord = _owner._queryResult.CreateRecordFromReferenceParameterValues(_owner._recordValuesPositions, _owner._referenceValues);
						IQueryResult queryResult = null;
						if(referenceRecord == null || queryInfo.CanExecute(referenceRecord))
						{
							queryResult = _owner._queryResult._executer(queryInfo);
						}
						else
							queryResult = new QueryResult(new Record.RecordDescriptor("EmptyDescriptor", new Record.Field[0]), new Record[0]);
						_current = new Tuple<QueryInfo, IQueryResult>(queryInfo, queryResult);
						return true;
					}
					return false;
				}

				/// <summary>
				/// Reseta o enumerador.
				/// </summary>
				public void Reset()
				{
				}
			}
		}
	}
}
