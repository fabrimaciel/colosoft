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

using System.Collections.Generic;
using System.Linq;
using Colosoft.Data.Schema;
using GDA;
using System.Data.SqlClient;
using System;
using GDA.Interfaces;
using Colosoft.Security.Profile;

namespace Colosoft.Query.Database.Generic
{
	/// <summary>
	/// Implementação genérica de um origem de dados para consulta.
	/// </summary>
	public abstract class GenericSqlQueryDataSource : SqlQueryDataSource
	{
		private IQueryTranslator _translator;

		/// <summary>
		/// Instancia do tradutor associado.
		/// </summary>
		protected IQueryTranslator Translator
		{
			get
			{
				return _translator;
			}
		}

		/// <summary>
		/// Identifica se é para ignorar o último campo quando se trabalha com paginação.
		/// </summary>
		protected abstract bool IgnoreLastFieldWithPaging
		{
			get;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="serviceLocator">Localizador de serviço.</param>
		/// <param name="typeSchema">Objeto que retorna o esquema de dados</param>
		/// <param name="translator">Tradutor de nomes.</param>
		/// <param name="providerLocator">Localizador de provedor</param>
		public GenericSqlQueryDataSource(Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator, ITypeSchema typeSchema, IQueryTranslator translator, IProviderLocator providerLocator) : base(serviceLocator, typeSchema, providerLocator)
		{
			_translator = translator;
		}

		/// <summary>
		/// Executa as várias consultas.
		/// </summary>
		/// <param name="queries"></param>
		/// <returns></returns>
		public override IEnumerable<IQueryResult> Execute(QueryInfo[] queries)
		{
			queries.Require("queries").NotNull();
			var result = new IQueryResult[queries.Length];
			Dictionary<QueryExecutionGroup, IList<int>> providerDictionary = new Dictionary<QueryExecutionGroup, IList<int>>(QueryExecutionGroup.Comparer);
			for(int i = 0; i < queries.Length; i++)
			{
				string providerName = ProviderLocator.GetProviderName(queries[i]);
				var group = new QueryExecutionGroup(providerName, queries[i].IsolationLevel);
				IList<int> indexes;
				if(providerDictionary.TryGetValue(group, out indexes))
				{
					indexes.Add(i);
				}
				else
				{
					indexes = new List<int>();
					indexes.Add(i);
					providerDictionary.Add(group, indexes);
				}
			}
			IStoredProcedureTransaction storedProcedureTransaction = null;
			foreach (var provider in providerDictionary)
			{
				using (var session = CreateSession(provider.Key))
				{
					RegisterSession(session);
					foreach (var index in provider.Value)
					{
						IQueryResult queryResult = null;
						try
						{
							if(queries[index].StoredProcedureName == null)
							{
								SqlQueryParser parser = CreateParser(queries[index]);
								string queryString = parser.GetText();
								try
								{
									queryResult = ExecuteQuery(session, queryString, queries[index]);
								}
								catch(GDAException ex)
								{
									throw new GDAException(string.Format("{0}. Query: {1}", ex.Message, queries[index].ToString()), ex.InnerException);
								}
							}
							else
							{
								if(storedProcedureTransaction == null)
								{
									storedProcedureTransaction = new GDAStoredProcedureTransaction(session, provider.Key.ProviderName);
									if(!queries[index].IgnoreRegisterUserInfo)
										RegisterUserInfo(storedProcedureTransaction);
								}
								queryResult = ExecuteStoredProcedure(session, queries[index]);
							}
						}
						catch
						{
							if(session is GDATransaction)
								((GDATransaction)session).Rollback();
							throw;
						}
						yield return queryResult;
					}
					if(session is GDATransaction)
						((GDATransaction)session).Commit();
				}
			}
		}

		/// <summary>
		/// Cria a sessão de conexão com base no dados do grupo de execução.
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		protected virtual GDASession CreateSession(QueryExecutionGroup group)
		{
			IProviderConfiguration providerConfiguration = null;
			if(string.IsNullOrEmpty(group.ProviderName))
				providerConfiguration = GDASettings.DefaultProviderConfiguration;
			else
				providerConfiguration = GDASettings.GetProviderConfiguration(group.ProviderName);
			if(group.IsolationLevel == System.Transactions.IsolationLevel.Unspecified)
				return new GDASession(providerConfiguration);
			else
			{
				var transaction = new GDATransaction(providerConfiguration, Convert(group.IsolationLevel));
				transaction.BeginTransaction();
				return transaction;
			}
		}

		/// <summary>
		/// Registra as informações do usuário na transação do storedprocedure.
		/// </summary>
		/// <param name="transaction"></param>
		protected override void RegisterUserInfo(IStoredProcedureTransaction transaction)
		{
			if(!Colosoft.Security.UserContext.UserProcessing)
			{
				var profile = ProfileManager.CurrentProfileInfo;
				var user = Colosoft.Security.UserContext.Current.User;
				if(profile != null && user != null)
				{
					var query = "CREATE TABLE #UserInfo (ProfileId int, UserId int)";
					var parameters = new GDAParameter[] {
						new GDAParameter("?profileId", profile.ProfileId),
						new GDAParameter("?userKey", int.Parse(user.UserKey))
					};
					var session = ((GDAStoredProcedureTransaction)transaction).Session;
					var da = new DataAccess(session.ProviderConfiguration);
					var r1 = da.ExecuteCommand(session, query);
					var query1 = "INSERT INTO #UserInfo(ProfileId, UserId) VALUES(?profileId, ?userKey)";
					da.ExecuteCommand(session, query1, parameters);
				}
			}
		}

		/// <summary>
		/// Executa as várias consultas.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="queryInfo"></param>
		/// <returns></returns>
		protected virtual IQueryResult Execute(GDASession session, QueryInfo queryInfo)
		{
			if(session.State == GDASessionState.Closed)
				throw new QueryException("Database connection is closed to execute query");
			queryInfo.Require("queries").NotNull();
			string providerName = ProviderLocator.GetProviderName(queryInfo);
			var providerConfiguration = GDASettings.GetProviderConfiguration(providerName);
			if(queryInfo.StoredProcedureName == null)
			{
				SqlQueryParser parser = CreateParser(queryInfo);
				string queryString = parser.GetText();
				try
				{
					return ExecuteQuery(session, queryString, queryInfo);
				}
				catch(GDAException ex)
				{
					throw new QueryException(string.Format("{0}. Query: {1}", ex.Message, queryInfo.ToString()), ex.InnerException);
				}
			}
			else
				return ExecuteStoredProcedure(session, queryInfo);
		}

		/// <summary>
		/// Executa a consulta.
		/// </summary>
		/// <param name="commandText">Texto do comando.</param>
		/// <param name="queryInfo">Informações da query</param>
		/// <returns>Retorna resultado da query</returns>
		protected override IQueryResult ExecuteQuery(string commandText, QueryInfo queryInfo)
		{
			var group = new QueryExecutionGroup(ProviderLocator.GetProviderName(queryInfo), queryInfo.IsolationLevel);
			GDASession session = CreateSession(group);
			try
			{
				RegisterSession(session);
				var result = ExecuteQuery(session, commandText, queryInfo);
				if(result != null)
				{
					EventHandler methodHandler = null;
					methodHandler = new EventHandler((sender, e) =>  {
						result.Disposed -= methodHandler;
						if(session is GDATransaction)
							((GDATransaction)session).Commit();
						session.Dispose();
					});
					result.Disposed += methodHandler;
					return result;
				}
			}
			catch
			{
				if(session is GDATransaction)
					((GDATransaction)session).Rollback();
				session.Dispose();
				throw;
			}
			if(session is GDATransaction)
				((GDATransaction)session).Commit();
			session.Dispose();
			return null;
		}

		/// <summary>
		/// Executa uma procedure no sistema.
		/// </summary>
		/// <param name="queryInfo">Informações da procedure.</param>
		/// <returns>Resultado da consulta.</returns>
		protected override IQueryResult ExecuteStoredProcedure(QueryInfo queryInfo)
		{
			var group = new QueryExecutionGroup(ProviderLocator.GetProviderName(queryInfo), queryInfo.IsolationLevel);
			GDASession session = CreateSession(group);
			try
			{
				if(!queryInfo.IgnoreRegisterUserInfo)
					RegisterUserInfo(new GDAStoredProcedureTransaction(session, group.ProviderName));
				RegisterSession(session);
				var result = ExecuteStoredProcedure(session, queryInfo);
				if(result != null)
				{
					EventHandler methodHandler = null;
					methodHandler = new EventHandler((sender, e) =>  {
						result.Disposed -= methodHandler;
						if(session is GDATransaction)
							((GDATransaction)session).Commit();
						session.Dispose();
					});
					result.Disposed += methodHandler;
					return result;
				}
			}
			catch
			{
				if(session is GDATransaction)
					((GDATransaction)session).Rollback();
				session.Dispose();
				throw;
			}
			if(session is GDATransaction)
				((GDATransaction)session).Commit();
			session.Dispose();
			return null;
		}

		/// <summary>
		/// Gera uma StoredProcedure do GDA.
		/// </summary>
		/// <param name="queryInfo">Informações da procedure.</param>
		/// <param name="outputParametersIndexes">Índices de parâmetro de output.</param>
		/// <returns>Retorna StoredProcedure do GDA.</returns>
		protected virtual GDAStoredProcedure GenerateGDAStoredProcedure(QueryInfo queryInfo, out IEnumerable<int> outputParametersIndexes)
		{
			var indexes = new List<int>();
			var storedProcedureName = (TranslatedStoredProcedureName)Translator.GetName(queryInfo.StoredProcedureName);
			var procedure = new GDAStoredProcedure(!string.IsNullOrEmpty(storedProcedureName.Schema) ? string.Format("{0}.{1}", storedProcedureName.Schema, storedProcedureName.Name) : storedProcedureName.Name);
			procedure.CommandTimeout = queryInfo.CommandTimeout;
			for(int i = 0; i < queryInfo.Parameters.Count; i++)
			{
				var value = queryInfo.Parameters[i].Value;
				if(value is QueryInfo || value is Queryable)
					value = null;
				var paramDirection = (System.Data.ParameterDirection)((int)queryInfo.Parameters[i].Direction);
				procedure.AddParameter(queryInfo.Parameters[i].Name, value, paramDirection);
				if(queryInfo.Parameters[i].Direction != ParameterDirection.Input)
					indexes.Add(i);
			}
			outputParametersIndexes = indexes;
			return procedure;
		}

		/// <summary>
		/// Executa uma procedure no sistema.
		/// </summary>
		/// <param name="queryInfo">Informações da procedure.</param>
		/// <param name="session">Sessão do banco.</param>
		/// <returns>Resultado da consulta.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		protected virtual QueryResult ExecuteStoredProcedure(GDASession session, QueryInfo queryInfo)
		{
			var da = new DataAccess(session.ProviderConfiguration);
			IEnumerable<int> outputParametersIndexes;
			var procedure = GenerateGDAStoredProcedure(queryInfo, out outputParametersIndexes);
			procedure.CommandTimeout = queryInfo.CommandTimeout;
			IEnumerator<GDADataRecord> dataRecordEnumerator = da.LoadResult(session, procedure).GetEnumerator();
			IEnumerable<Record> records = null;
			Record.RecordDescriptor descriptor = null;
			foreach (int index in outputParametersIndexes)
				queryInfo.Parameters[index].Value = procedure[index];
			if(dataRecordEnumerator.MoveNext())
			{
				var dataRecord = dataRecordEnumerator.Current;
				var fields = new List<Record.Field>();
				for(int i = 0; i < dataRecord.FieldCount; i++)
				{
					var value = dataRecord[i].GetValue();
					var name = dataRecord.GetName(i);
					if(string.IsNullOrEmpty(name))
						name = DataAccessConstants.ReturnValueColumnName;
					fields.Add(new Record.Field(name, dataRecord.GetFieldType(i)));
				}
				descriptor = new Record.RecordDescriptor("descriptor", fields);
				var firstRecord = GetRecord(descriptor, dataRecord);
				records = GetRecords(descriptor, firstRecord, dataRecordEnumerator);
			}
			else
				records = new Record[0];
			var result = new QueryResult(descriptor, records, queryInfo, f => Execute(session, f));
			result.Validate().ThrowInvalid();
			return result;
		}

		/// <summary>
		/// Recupera valor do dataRecord.
		/// </summary>
		/// <param name="ordinal">Posição ordinal do campo que será recuperado.</param>
		/// <param name="dataRecord">Objeto dataRecord sobre qual será recuperado.</param>
		/// <param name="fieldType">Tipo do campo.</param>
		protected virtual object GetValue(int ordinal, GDADataRecord dataRecord, Type fieldType)
		{
			return dataRecord[ordinal].GetValue();
		}

		/// <summary>
		/// Recupera o tipo do campo.
		/// </summary>
		/// <param name="ordinal">Posição do campo.</param>
		/// <param name="dataRecord">O record sobre qual será buscado o campo.</param>
		/// <returns>Tipo do campo.</returns>
		protected virtual Type GetFieldType(int ordinal, GDADataRecord dataRecord)
		{
			return dataRecord.GetFieldType(ordinal);
		}

		/// <summary>
		/// Cria um parâmetro para a consulta.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="value">Valor do parâmetro.</param>
		/// <returns>Objeto do tipo <see cref="GDAParameter"/>.</returns>
		protected virtual GDAParameter CreateParameter(string name, object value)
		{
			if(value is QueryInfo || value is Queryable)
				value = null;
			if(value is DateTime)
			{
				var date = (DateTime)value;
				if(date.Kind == DateTimeKind.Unspecified)
					value = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, DateTimeKind.Local);
			}
			return new GDAParameter(name, value);
		}

		/// <summary>
		/// Recupera os dados do registro.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="record"></param>
		/// <returns></returns>
		protected Record GetRecord(Record.RecordDescriptor descriptor, GDADataRecord record)
		{
			var values = new object[descriptor.Count];
			for(int i = 0; i < values.Length; i++)
			{
				var field = descriptor[i];
				var fieldName = field.Name;
				if(fieldName == DataAccessConstants.ReturnValueColumnName)
					fieldName = "";
				values[i] = GetValue(i, record, field.Type);
				if(field.Type == typeof(object) && values[i] is byte[])
					field.ChangeFieldType(typeof(byte[]));
			}
			return descriptor.CreateRecord(values);
		}

		/// <summary>
		/// Recupera a enumeração 
		/// </summary>
		/// <param name="descriptor">Descritor dos campos dos registros.</param>
		/// <param name="firstRecord">Instancia do primeiro registro encontrado.</param>
		/// <param name="recordsEnumerator">Enumerador dos registros</param>
		/// <returns></returns>
		protected IEnumerable<Record> GetRecords(Record.RecordDescriptor descriptor, Record firstRecord, IEnumerator<GDADataRecord> recordsEnumerator)
		{
			return new RecordsEnumerable(this, descriptor, firstRecord, recordsEnumerator);
		}

		/// <summary>
		/// Registra na sessão
		/// </summary>
		/// <param name="session"></param>
		protected virtual void RegisterSession(GDASession session)
		{
		}

		/// <summary>
		/// Recupera o descritor dos registro.
		/// </summary>
		/// ?<param name="queryInfo">Informações da consulta.</param>
		/// <param name="query">Consulta nativa do GDA.</param>
		/// <param name="dataRecord">Registro com os dados.</param>
		/// <returns></returns>
		protected virtual Record.RecordDescriptor GetRecordDescriptor(QueryInfo queryInfo, GDA.Sql.NativeQuery query, GDADataRecord dataRecord)
		{
			var fields = new List<Record.Field>();
			int fieldCount;
			if(IgnoreLastFieldWithPaging && query.TakeCount > 0 && query.SkipCount > 0)
				fieldCount = dataRecord.FieldCount - 1;
			else
				fieldCount = dataRecord.FieldCount;
			for(int i = 0; i < fieldCount; i++)
			{
				var fieldName = dataRecord.GetName(i);
				var fieldType = GetFieldType(i, dataRecord);
				fields.Add(new Record.Field(fieldName, fieldType));
			}
			return new Record.RecordDescriptor("descriptor", fields);
		}

		/// <summary>
		/// Verifica se no texto do comando possui o nome do parametro informado.
		/// </summary>
		/// <param name="commandText">Texto do comando.</param>
		/// <param name="parameterName">Nome do parametro.</param>
		/// <returns></returns>
		private static bool ContainsParameter(string commandText, string parameterName)
		{
			return !string.IsNullOrEmpty(commandText) && !string.IsNullOrEmpty(parameterName) && commandText.Contains(parameterName);
		}

		/// <summary>
		/// Executa a consulta.
		/// </summary>
		/// <param name="session">Sessão da conexão do banco de dados.</param>
		/// <param name="commandText">Texto do comando.</param>
		/// <param name="queryInfo">Informações da query</param>
		/// <returns>Retorna resultado da query</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private QueryResult ExecuteQuery(GDASession session, string commandText, QueryInfo queryInfo)
		{
			var query = new GDA.Sql.NativeQuery(commandText) {
				Query.QueryParameter.GetParameters(queryInfo, false).Select(f => CreateParameter(f.Name, f.Value)).GroupBy(f => f.ParameterName).Where(f => ContainsParameter(commandText, f.Key)).Select(f => f.FirstOrDefault())
			};
			query.CommandTimeout = queryInfo.CommandTimeout;
			if(queryInfo.TakeParameters != null)
			{
				if(queryInfo.TakeParameters.Skip > 0)
					query.Skip(queryInfo.TakeParameters.Skip);
				if(queryInfo.TakeParameters.Take > 0)
					query.Take(queryInfo.TakeParameters.Take);
			}
			IEnumerator<GDADataRecord> dataRecordEnumerator = query.ToDataRecords(session).GetEnumerator();
			IEnumerable<Record> records = null;
			Record.RecordDescriptor descriptor = null;
			if(dataRecordEnumerator.MoveNext())
			{
				var dataRecord = dataRecordEnumerator.Current;
				descriptor = GetRecordDescriptor(queryInfo, query, dataRecord);
				var firstRecord = GetRecord(descriptor, dataRecord);
				records = GetRecords(descriptor, firstRecord, dataRecordEnumerator);
			}
			else
				records = new Record[0];
			var result = new QueryResult(descriptor, records, queryInfo, f => Execute(session, f));
			result.Validate().ThrowInvalid();
			return result;
		}

		/// <summary>
		/// Armazena os dados do grupo e execução de consultas. 
		/// </summary>
		public class QueryExecutionGroup
		{
			/// <summary>
			/// Instancia do comparador associado.
			/// </summary>
			public static readonly IEqualityComparer<QueryExecutionGroup> Comparer = new EqualityComparer();

			/// <summary>
			/// Nome provedor de configuração.
			/// </summary>
			public string ProviderName;

			/// <summary>
			/// Nível de isolamento para a execução da consulta.
			/// </summary>
			public System.Transactions.IsolationLevel IsolationLevel;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="providerName"></param>
			/// <param name="isolationLevel"></param>
			public QueryExecutionGroup(string providerName, System.Transactions.IsolationLevel isolationLevel)
			{
				ProviderName = providerName;
				IsolationLevel = isolationLevel;
			}

			/// <summary>
			/// Texto que representa a instancia.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format("{0} : {1}", ProviderName, IsolationLevel);
			}

			/// <summary>
			/// Implementação do comparador.
			/// </summary>
			class EqualityComparer : IEqualityComparer<QueryExecutionGroup>
			{
				/// <summary>
				/// Compara as duas instancia informadas.
				/// </summary>
				/// <param name="x"></param>
				/// <param name="y"></param>
				/// <returns></returns>
				public bool Equals(QueryExecutionGroup x, QueryExecutionGroup y)
				{
					return (x == null && y == null) || (x != null && y != null && x.ProviderName == y.ProviderName && x.IsolationLevel == y.IsolationLevel);
				}

				/// <summary>
				/// Recupera o hash o objeto informado.
				/// </summary>
				/// <param name="obj"></param>
				/// <returns></returns>
				public int GetHashCode(QueryExecutionGroup obj)
				{
					if(obj == null)
						return 0;
					return obj.ToString().GetHashCode();
				}
			}
		}

		/// <summary>
		/// Implementação do enumerable para percorrer os registros.
		/// </summary>
		class RecordsEnumerable : IEnumerable<Record>, IDisposable
		{
			private Record _firstRecord;

			private GenericSqlQueryDataSource _dataSource;

			private Record.RecordDescriptor _descriptor;

			private IEnumerator<GDADataRecord> _enumerator;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="dataSource"></param>
			/// <param name="descriptor"></param>
			/// <param name="firstRecord"></param>
			/// <param name="enumerator"></param>
			public RecordsEnumerable(GenericSqlQueryDataSource dataSource, Record.RecordDescriptor descriptor, Record firstRecord, IEnumerator<GDADataRecord> enumerator)
			{
				_dataSource = dataSource;
				_descriptor = descriptor;
				_enumerator = enumerator;
				_firstRecord = firstRecord;
			}

			/// <summary>
			/// Destrutor.
			/// </summary>
			~RecordsEnumerable()
			{
				Dispose();
			}

			/// <summary>
			/// Recupera o enumerador.
			/// </summary>
			/// <returns></returns>
			public IEnumerator<Record> GetEnumerator()
			{
				return new GetRecordsEnumerator(_dataSource, _descriptor, _firstRecord, _enumerator);
			}

			/// <summary>
			/// Recupera o enumerador.
			/// </summary>
			/// <returns></returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return new GetRecordsEnumerator(_dataSource, _descriptor, _firstRecord, _enumerator);
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				_enumerator.Dispose();
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Implementação do enumerator para percorrer os registros.
		/// </summary>
		class GetRecordsEnumerator : IEnumerator<Record>
		{
			private Record _firstRecord;

			private GenericSqlQueryDataSource _dataSource;

			private Record.RecordDescriptor _descriptor;

			private IEnumerator<GDADataRecord> _enumerator;

			private Record _record;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="dataSource">Oridem dos dados.</param>
			/// <param name="descriptor">Descritor dos registros.</param>
			/// <param name="firstRecord">Instancia do primeiro registro.</param>
			/// <param name="enumerator">Enumerador dos registros.</param>
			public GetRecordsEnumerator(GenericSqlQueryDataSource dataSource, Record.RecordDescriptor descriptor, Record firstRecord, IEnumerator<GDADataRecord> enumerator)
			{
				_dataSource = dataSource;
				_descriptor = descriptor;
				_enumerator = enumerator;
				_firstRecord = firstRecord;
			}

			/// <summary>
			/// Destrutor.
			/// </summary>
			~GetRecordsEnumerator()
			{
				Dispose();
			}

			/// <summary>
			/// Instancia do atual registro.
			/// </summary>
			public Record Current
			{
				get
				{
					return _record;
				}
			}

			/// <summary>
			/// Instancia do atual registro.
			/// </summary>
			object System.Collections.IEnumerator.Current
			{
				get
				{
					return _record;
				}
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				if(_enumerator != null)
				{
					_enumerator.Dispose();
					_enumerator = null;
				}
			}

			/// <summary>
			/// Move para o próximo registro.
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				if(_enumerator == null)
					return false;
				if(_firstRecord != null)
				{
					_record = _firstRecord;
					_firstRecord = null;
					return true;
				}
				if(_enumerator.MoveNext())
				{
					GDADataRecord dataRecord = _enumerator.Current;
					_record = _dataSource.GetRecord(_descriptor, dataRecord);
					return true;
				}
				Dispose();
				return false;
			}

			/// <summary>
			/// Reset.
			/// </summary>
			public void Reset()
			{
				throw new NotImplementedException();
			}
		}
	}
}
