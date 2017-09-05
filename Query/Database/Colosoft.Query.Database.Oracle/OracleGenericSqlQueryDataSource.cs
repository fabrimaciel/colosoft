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

using Colosoft.Data.Schema;
using Colosoft.Query.Database.Generic;
using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Query.Database.Oracle
{
	/// <summary>
	/// Implementação genérica de um origem de dados para consulta.
	/// </summary>
	public class OracleGenericSqlQueryDataSource : GenericSqlQueryDataSource
	{
		/// <summary>
		/// Identifica se é para ignorar o último campo quando se trabalha com paginação.
		/// </summary>
		protected override bool IgnoreLastFieldWithPaging
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="serviceLocator">Localizador de serviço.</param>
		/// <param name="typeSchema">Objeto que retorna o esquema de dados</param>
		/// <param name="providerLocator">Lozalizador de provedor.</param>
		public OracleGenericSqlQueryDataSource(Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator, ITypeSchema typeSchema, IProviderLocator providerLocator) : base(serviceLocator, typeSchema, new OracleQueryTranslator(typeSchema), providerLocator)
		{
		}

		/// <summary>      
		/// Construtor estático faz o remapeamento do tipo RefCursor para o DbType Object.
		/// </summary>
		static OracleGenericSqlQueryDataSource()
		{
			#if !DEVART
			#if UNMANAGED
			            var t = typeof(global::Oracle.DataAccess.Client.OracleConnection).Assembly.GetTypes().FirstOrDefault(f => f.Name == "OraDb_DbTypeTable");
#else
			var t = typeof(global::Oracle.ManagedDataAccess.Client.OracleConnection).Assembly.GetTypes().FirstOrDefault(f => f.Name == "OraDb_DbTypeTable");
			#endif
			var field = t.GetField("dbTypeToOracleDbTypeMapping", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			var dbTypeToOracleDbTypeMapping = (int[])field.GetValue(null);
			dbTypeToOracleDbTypeMapping[13] = 0x79;
			dbTypeToOracleDbTypeMapping[6] = 0x7C;
			#endif
		}

		/// <summary>
		/// Registra a sessão de persistencia.
		/// </summary>
		/// <param name="session"></param>
		protected override void RegisterSession(GDASession session)
		{
			var dataAccess = new DataAccess(session.ProviderConfiguration);
			dataAccess.ExecuteCommand(session, "ALTER SESSION SET NLS_SORT=BINARY_AI");
			dataAccess.ExecuteCommand(session, "ALTER SESSION SET NLS_COMP=LINGUISTIC");
		}

		/// <summary>
		/// Registra as informações do usuário.
		/// </summary>
		/// <param name="transaction"></param>
		protected override void RegisterUserInfo(IStoredProcedureTransaction transaction)
		{
		}

		/// <summary>
		/// Cria o parser para ser utilizado.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected override SqlQueryParser CreateParser(QueryInfo query)
		{
			return new OracleQueryParser(Translator, TypeSchema, new OracleTakeParametersParser()) {
				Query = query,
				UseTakeParameter = true
			};
		}

		/// <summary>
		/// Execute a storead procedure.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="queryInfo"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		protected override QueryResult ExecuteStoredProcedure(GDASession session, QueryInfo queryInfo)
		{
			DataAccess da = session != null ? new DataAccess(session.ProviderConfiguration) : new DataAccess();
			var indexes = new List<int>();
			var storedProcedureName = (TranslatedStoredProcedureName)Translator.GetName(queryInfo.StoredProcedureName);
			var script = "BEGIN " + (!string.IsNullOrEmpty(storedProcedureName.Schema) ? string.Format("\"{0}\".\"{1}\"", storedProcedureName.Schema, storedProcedureName.Name) : string.Format("\"{0}\"", storedProcedureName.Name)) + " (";
			for(var i = 1; i <= queryInfo.Parameters.Count + 1; i++)
				script += ":" + i + ",";
			script = script.Substring(0, script.Length - 1) + "); END;";
			var parameters = new GDA.Collections.GDAParameterCollection();
			for(int i = 0; i < queryInfo.Parameters.Count; i++)
			{
				var paramDirection = (System.Data.ParameterDirection)((int)queryInfo.Parameters[i].Direction);
				parameters.Add(new GDAParameter(":" + (i + 1), queryInfo.Parameters[i].Value, paramDirection));
				if(queryInfo.Parameters[i].Direction != ParameterDirection.Input)
					indexes.Add(i);
			}
			#if DEVART
			            // Cria o parametro é que referência do cursor
            var refCursorParameter = new GDAParameter(":" + (queryInfo.Parameters.Count + 1), null) { NativeDbType = Devart.Data.Oracle.OracleDbType.Cursor, Direction = System.Data.ParameterDirection.Output };
#else
			var refCursorParameter = new GDAParameter(":" + (queryInfo.Parameters.Count + 1), null) {
				DbType = (System.Data.DbType)13,
				Direction = System.Data.ParameterDirection.Output
			};
			#endif
			parameters.Add(refCursorParameter);
			da.ExecuteCommand(session, System.Data.CommandType.Text, queryInfo.CommandTimeout, script, parameters.ToArray());
			IEnumerator<GDADataRecord> dataRecordEnumerator = NavigateRefCursor(refCursorParameter).GetEnumerator();
			IEnumerable<Record> records = null;
			Record.RecordDescriptor descriptor = null;
			foreach (int index in indexes)
				queryInfo.Parameters[index].Value = parameters[index].Value;
			try
			{
				if(dataRecordEnumerator.MoveNext())
				{
					var dataRecord = dataRecordEnumerator.Current;
					var fields = new List<Record.Field>();
					for(int i = 0; i < dataRecord.FieldCount; i++)
					{
						var value = dataRecord[i].GetValue();
						var name = dataRecord.GetName(i);
						if(string.IsNullOrEmpty(name))
							name = "return_value";
						fields.Add(new Record.Field(name, dataRecord.GetFieldType(i)));
					}
					descriptor = new Record.RecordDescriptor("descriptor", fields);
					var firstRecord = GetRecord(descriptor, dataRecord);
					records = GetRecords(descriptor, firstRecord, dataRecordEnumerator);
				}
				else
				{
					dataRecordEnumerator.Dispose();
					records = new Record[0];
				}
			}
			catch
			{
				dataRecordEnumerator.Dispose();
				throw;
			}
			var result = new QueryResult(descriptor, records, queryInfo, f => Execute(session, f));
			result.Validate().ThrowInvalid();
			return result;
		}

		/// <summary>
		/// Navega pelo cursor.
		/// </summary>
		/// <param name="refCursorParameter"></param>
		/// <returns></returns>
		private static IEnumerable<GDADataRecord> NavigateRefCursor(GDAParameter refCursorParameter)
		{
			if(refCursorParameter.Value != null)
			{
				#if UNMANAGED
				var cursor = refCursorParameter.Value as global::Oracle.DataAccess.Types.OracleRefCursor;
#elif DEVART
				var cursor = refCursorParameter.Value as Devart.Data.Oracle.OracleCursor;
#else
				var cursor = refCursorParameter.Value as global::Oracle.ManagedDataAccess.Types.OracleRefCursor;
				#endif
				System.Data.IDataReader reader = null;
				if(cursor != null)
					reader = cursor.GetDataReader();
				else if(refCursorParameter.Value is System.Data.IDataReader)
					reader = (System.Data.IDataReader)refCursorParameter.Value;
				if(reader != null)
					while (reader.Read())
					{
						yield return new GDADataRecord(reader, null);
					}
			}
		}

		/// <summary>
		/// Gera procedure do GDA.
		/// </summary>
		/// <param name="queryInfo">Informações da procedure.</param>
		/// <param name="outputParametersIndexes">Índices de parâmetros de saída.</param>
		/// <returns>Retorna a procedure em um objeto do tipo <see cref="GDAStoredProcedure"/></returns>
		protected override GDAStoredProcedure GenerateGDAStoredProcedure(QueryInfo queryInfo, out IEnumerable<int> outputParametersIndexes)
		{
			var indexes = new List<int>();
			var storedProcedureName = (TranslatedStoredProcedureName)Translator.GetName(queryInfo.StoredProcedureName);
			var procedure = new GDAStoredProcedure(!string.IsNullOrEmpty(storedProcedureName.Schema) ? string.Format("\"{0}\".\"{1}\"", storedProcedureName.Schema, storedProcedureName.Name) : string.Format("\"{0}\"", storedProcedureName.Name));
			for(int i = 0; i < queryInfo.Parameters.Count; i++)
			{
				var paramDirection = (System.Data.ParameterDirection)((int)queryInfo.Parameters[i].Direction);
				procedure.AddParameter(":" + (i + 1), queryInfo.Parameters[i].Value, paramDirection);
				if(queryInfo.Parameters[i].Direction != ParameterDirection.Input)
					indexes.Add(i);
			}
			#if DEVART
			            procedure.AddParameter(new GDAParameter(":" + (queryInfo.Parameters.Count + 1), null)
            {
                Direction = System.Data.ParameterDirection.Output,
                NativeDbType = Devart.Data.Oracle.OracleDbType.Cursor
            });
#else
			procedure.AddOutputParameter(":" + (queryInfo.Parameters.Count + 1), (System.Data.DbType)13);
			#endif
			outputParametersIndexes = indexes;
			return procedure;
		}

		/// <summary>
		/// Recupera valor do dataRecord.
		/// </summary>
		/// <param name="ordinal">Posição ordinal do campo que será recuperado.</param>
		/// <param name="fieldType">Tipo do campo.</param>
		/// <param name="dataRecord">Objeto dataRecord sobre qual será recuperado.</param>
		protected override object GetValue(int ordinal, GDADataRecord dataRecord, Type fieldType)
		{
			#if UNMANAGED
			            var dataReader = (global::Oracle.DataAccess.Client.OracleDataReader)dataRecord.BaseDataRecord;
#elif DEVART
			            var dataReader = (Devart.Data.Oracle.OracleDataReader)dataRecord.BaseDataRecord;
#else
			var dataReader = (global::Oracle.ManagedDataAccess.Client.OracleDataReader)dataRecord.BaseDataRecord;
			#endif
			var fieldIndex = ordinal;
			var dataType = dataReader.GetDataTypeName(fieldIndex);
			if(dataType == "Date" && !dataReader.IsDBNull(fieldIndex))
				return dataRecord.GetDateTime(fieldIndex);
			else if((dataType == "TimeStampTZ" || fieldType == typeof(DateTime)) && !dataReader.IsDBNull(fieldIndex))
				#if DEVART
				                return dataReader.GetOracleTimeStamp(fieldIndex).ToDateTimeOffset();
#else
				return dataReader.GetOracleTimeStampTZ(fieldIndex).ToDateTimeOffset();
			#endif
			return dataRecord[fieldIndex].GetValue();
		}

		/// <summary>
		/// Recupera o tipo do campo.
		/// </summary>
		/// <param name="ordinal">Posição ordinal do campo.</param>
		/// <param name="dataRecord">O record sobre qual será buscado o campo.</param>
		/// <returns>Tipo do campo.</returns>
		protected override Type GetFieldType(int ordinal, GDADataRecord dataRecord)
		{
			#if UNMANAGED
			            var dataReader = (global::Oracle.DataAccess.Client.OracleDataReader)dataRecord.BaseDataRecord;
#elif DEVART
			            var dataReader = (Devart.Data.Oracle.OracleDataReader)dataRecord.BaseDataRecord;
#else
			var dataReader = (global::Oracle.ManagedDataAccess.Client.OracleDataReader)dataRecord.BaseDataRecord;
			#endif
			var type = dataReader.GetFieldType(ordinal);
			return (type == typeof(DateTime)) ? typeof(DateTimeOffset) : type;
		}

		/// <summary>
		/// Cria um parâmetro para a consulta.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="value">Valor do parâmetro.</param>
		/// <returns>Objeto do tipo <see cref="GDAParameter"/>.</returns>
		protected override GDAParameter CreateParameter(string name, object value)
		{
			if(value is QueryInfo || value is Queryable)
				value = null;
			if(value is DateTime)
			{
				var date = (DateTime)value;
				if(date.Kind == DateTimeKind.Unspecified)
					value = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, DateTimeKind.Local);
			}
			if(value != null && value.GetType() == typeof(DateTimeOffset))
			{
				var date = (DateTimeOffset)value;
				#if UNMANAGED
				
#elif DEVART
				                value = new Devart.Data.Oracle.OracleTimeStamp(date.DateTime, date.Offset.ToString());
#else
				value = new global::Oracle.ManagedDataAccess.Types.OracleTimeStampTZ(date.DateTime, date.Offset.ToString());
				#endif
				GDAParameter parameter = new GDAParameter(name, value);
				parameter.DbType = System.Data.DbType.DateTime;
				return parameter;
			}
			else
				return new GDAParameter(name, value);
		}

		/// <summary>
		/// Recupera o descritor dos campos do registro.
		/// </summary>
		/// <param name="queryInfo"></param>
		/// <param name="query"></param>
		/// <param name="dataRecord"></param>
		/// <returns></returns>
		protected override Record.RecordDescriptor GetRecordDescriptor(QueryInfo queryInfo, GDA.Sql.NativeQuery query, GDADataRecord dataRecord)
		{
			if(queryInfo.Projection.Count > 0)
			{
				var fields = new List<Record.Field>();
				int fieldCount;
				if(IgnoreLastFieldWithPaging && query.TakeCount > 0 && query.SkipCount > 0)
					fieldCount = dataRecord.FieldCount - 1;
				else
					fieldCount = dataRecord.FieldCount;
				var projection = queryInfo.Projection.Select(f => f.Alias).ToArray();
				if(fieldCount >= projection.Length)
				{
					for(int i = 0; i < fieldCount; i++)
					{
						var fieldName = (i < projection.Length ? projection[i] : dataRecord.GetName(i)) ?? dataRecord.GetName(i);
						var fieldType = GetFieldType(i, dataRecord);
						fields.Add(new Record.Field(fieldName, fieldType));
					}
					return new Record.RecordDescriptor("descriptor", fields);
				}
			}
			return base.GetRecordDescriptor(queryInfo, query, dataRecord);
		}
	}
}
