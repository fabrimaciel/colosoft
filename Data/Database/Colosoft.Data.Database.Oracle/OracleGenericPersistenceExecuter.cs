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
using GDA;
using Colosoft.Data.Schema;
using Microsoft.Practices.ServiceLocation;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using Colosoft.Query;

#if UNMANAGED
using Oracle.DataAccess.Types;
#elif DEVART
using Devart.Data.Oracle;
#else
using Oracle.ManagedDataAccess.Types;

#endif
using Colosoft.Data.Database.Generic;

namespace Colosoft.Data.Database.Oracle
{
	/// <summary>
	/// Implementação do PersistenceExectuter usando o GDA para o Oracle
	/// </summary>
	public class OracleGenericPersistenceExecuter : GenericPersistenceExecuter
	{
		/// <summary>
		/// Construtor padrão   
		/// </summary>
		/// <param name="locator">Inteface de IoC</param>
		/// <param name="typeSchema">Classe de recuperação de metadados</param>
		public OracleGenericPersistenceExecuter(IServiceLocator locator, ITypeSchema typeSchema) : base(locator, typeSchema, new Query.Database.Oracle.OracleQueryTranslator(typeSchema))
		{
		}

		/// <summary>
		/// Construtor estático faz o remapeamento do tipo RefCursor para o DbType Object.
		/// </summary>
		static OracleGenericPersistenceExecuter()
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
		/// Registra as informações do usuário.
		/// </summary>
		/// <param name="transaction"></param>
		protected override void RegisterUserInfo(IPersistenceTransactionExecuter transaction)
		{
			var trans2 = ((PersistenceTransactionExecuter)transaction).Transaction;
			var dataAccess = new DataAccess(trans2.ProviderConfiguration);
			dataAccess.ExecuteCommand(trans2, "ALTER SESSION SET NLS_SORT=BINARY_CI");
			dataAccess.ExecuteCommand(trans2, "ALTER SESSION SET NLS_COMP=LINGUISTIC");
			var profile = Security.Profile.ProfileManager.CurrentProfileInfo;
			var user = Security.UserContext.Current.User;
			if(profile != null && user != null)
			{
				var parameters = new GDAParameter[] {
					new GDAParameter("?profileId", profile.ProfileId),
					new GDAParameter("?userKey", int.Parse(user.UserKey))
				};
				var command = new StringBuilder().AppendLine("DECLARE").AppendLine("v_exists NUMBER := 0;").AppendLine("BEGIN").AppendLine("SELECT COUNT(*) INTO v_exists FROM sys.all_tables WHERE OWNER = 'SECURITY' AND TABLE_NAME = 'USERINFO' AND TEMPORARY = 'Y';").AppendLine("IF v_exists = 0 THEN").AppendLine("EXECUTE IMMEDIATE 'CREATE GLOBAL TEMPORARY TABLE \"SECURITY\".\"USERINFO\"(\"PROFILEID\" NUMBER(9), \"USERID\" NUMBER(9)) ON COMMIT DELETE ROWS';").AppendLine("ELSE").AppendLine("DELETE FROM \"SECURITY\".\"USERINFO\";").AppendLine("END IF;").AppendLine("INSERT INTO \"SECURITY\".\"USERINFO\"(\"PROFILEID\", \"USERID\") VALUES(?profileId, ?userKey);").AppendLine("END;");
				dataAccess.ExecuteCommand(trans2, command.ToString(), parameters);
			}
		}

		/// <summary>
		/// Cria o parser para a açõa informada.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="providerName">Nome do provider associado.</param>
		/// <returns></returns>
		protected override PersistenceSqlParser CreateParser(PersistenceAction action, string providerName)
		{
			return new OraclePersistenceSqlParser(Translator, TypeSchema, null) {
				Action = action,
				PrimaryKeyRepository = GetKeyRepository(providerName)
			};
		}

		/// <summary>
		/// Cria o repositório de chave primária.
		/// </summary>
		/// <param name="contractName"></param>
		/// <returns></returns>
		protected override IPrimaryKeyRepository CreatePrimaryKeyRepository(string contractName)
		{
			return this.ServiceLocator.GetInstance<IOraclePrimaryKeyRepository>(contractName + "OraclePrimaryKeyRepository");
		}

		/// <summary>
		/// Cria um parâmetro para a consulta.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="value">Valor do parâmetro.</param>
		/// <param name="size"></param>
		/// <param name="direction"></param>
		/// <returns>Objeto do tipo <see cref="GDAParameter"/>.</returns>
		protected override GDAParameter CreateParameter(string name, object value, int size, System.Data.ParameterDirection direction)
		{
			if(value != null && value.GetType() == typeof(DateTimeOffset))
			{
				var date = (DateTimeOffset)value;
				#if DEVART
				                value = new Devart.Data.Oracle.OracleTimeStamp(date.DateTime, date.Offset.ToString());
#else
				value = new OracleTimeStampTZ(date.DateTime, date.Offset.ToString());
				#endif
				GDAParameter parameter = new GDAParameter(name, value, direction) {
					Size = size
				};
				parameter.DbType = DbType.DateTime;
				return parameter;
			}
			else if(value is Guid)
			{
				return new GDAParameter(name, ((Guid)value).ToString(), direction) {
					Size = size
				};
			}
			else
			{
				if(value != null && value.GetType().IsEnum)
					value = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
				return new GDAParameter(name, value, direction) {
					Size = size
				};
			}
		}

		/// <summary>
		/// Cria um parâmetro para a consulta.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="value">Valor do parâmetro.</param>
		/// <returns>Objeto do tipo <see cref="GDAParameter"/>.</returns>
		protected override GDAParameter CreateParameter(string name, object value)
		{
			if(value != null && value.GetType() == typeof(DateTimeOffset))
			{
				var date = (DateTimeOffset)value;
				#if DEVART
				                value = new Devart.Data.Oracle.OracleTimeStamp(date.DateTime, date.Offset.ToString());
#else
				value = new OracleTimeStampTZ(date.DateTime, date.Offset.ToString());
				#endif
				GDAParameter parameter = new GDAParameter(name, value);
				parameter.DbType = DbType.DateTime;
				return parameter;
			}
			else if(value is Guid)
			{
				return new GDAParameter(name, ((Guid)value).ToString());
			}
			else
			{
				if(value != null && value.GetType().IsEnum)
					value = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
				return new GDAParameter(name, value);
			}
		}

		/// <summary>
		/// Recupera as propriedades volatéis.
		/// </summary>
		/// <param name="actions"></param>
		/// <param name="result"></param>
		/// <param name="transaction">Transação usada na execução.</param>
		protected override void RetrievePersistenceVolatileProperties(PersistenceAction[] actions, PersistenceActionResult[] result, IPersistenceTransactionExecuter transaction)
		{
			for(int actionIndex = 0; actionIndex < actions.Length; actionIndex++)
			{
				if(actions[actionIndex].Type == PersistenceActionType.Insert || actions[actionIndex].Type == PersistenceActionType.Update)
				{
					var action = actions[actionIndex];
					var actionResult = result[actionIndex];
					var typeMetadata = TypeSchema.GetTypeMetadata(action.EntityFullName);
					var volatileProperties = typeMetadata.GetVolatileProperties().ToArray();
					if(!typeMetadata.IsVersioned && volatileProperties.Length == 0)
						continue;
					var commandText = new StringBuilder();
					var keysIndex = new List<Tuple<int, IPropertyMetadata>>();
					for(int i = 0; i < action.Parameters.Count; i++)
					{
						var propertyMetadata = typeMetadata[action.Parameters[i].Name];
						if(propertyMetadata.ParameterType == Data.Schema.PersistenceParameterType.IdentityKey || propertyMetadata.ParameterType == Data.Schema.PersistenceParameterType.Key)
						{
							keysIndex.Add(new Tuple<int, IPropertyMetadata>(i, propertyMetadata));
						}
					}
					if(keysIndex.Count == 0)
						continue;
					var parameters = new GDA.Collections.GDAParameterCollection();
					bool isFirst = true;
					commandText.Append("SELECT ");
					var selectCommandText = new StringBuilder();
					if(typeMetadata.IsVersioned)
					{
						selectCommandText.Append("CAST(ORA_ROWSCN AS NUMBER(18,0)) AS ").AppendFormat("\"{0}\"", Query.DataAccessConstants.RowVersionPropertyName);
						isFirst = false;
					}
					foreach (var property in typeMetadata.GetVolatileProperties())
					{
						if(!isFirst)
							selectCommandText.Append(", ");
						isFirst = false;
						if(typeMetadata.IsVersioned && property.Name == Query.DataAccessConstants.RowVersionPropertyName)
							continue;
						selectCommandText.AppendFormat("\"{0}\" AS \"{1}\"", property.ColumnName.ToUpper(), property.Name);
					}
					var tableNameTranslatedName = (Query.Database.TranslatedTableName)Translator.GetName(new EntityInfo(action.EntityFullName));
					isFirst = true;
					commandText.Append(selectCommandText).Append(" FROM ").AppendFormat("\"{0}\".\"{1}\"", tableNameTranslatedName.Schema, tableNameTranslatedName.Name).Append(" WHERE ");
					var paramaterCount = 1;
					foreach (var i in keysIndex)
					{
						if(!isFirst)
							commandText.Append(" AND ");
						isFirst = false;
						var whereParameter = new GDA.GDAParameter("?" + (paramaterCount++), action.Parameters[i.Item1].Value);
						parameters.Add(whereParameter);
						commandText.AppendFormat("\"{0}\"", i.Item2.ColumnName.ToUpper()).Append("=").Append(whereParameter.ParameterName);
					}
					var transaction2 = (PersistenceTransactionExecuter)transaction;
					var da = new DataAccess(transaction2.Transaction.ProviderConfiguration);
					var resultParameters = new List<PersistenceParameter>();
					using (var enumerator = da.LoadResult(transaction2.Transaction, commandText.ToString(), parameters.ToArray()).GetEnumerator())
					{
						if(enumerator.MoveNext())
						{
							var record = enumerator.Current;
							for(var i = 0; i < record.FieldCount; i++)
							{
								var parameterName = record.GetName(i);
								object value = null;
								if(record.BaseDataRecord.GetDataTypeName(i) == "TimeStampTZ")
								{
									#if UNMANAGED
									                                    // Recupera o valor do Oracle
                                    var oracleTimeStampTZ = ((global::Oracle.DataAccess.Client.OracleDataReader)record.BaseDataRecord)
                                        .GetOracleTimeStampTZ(i);
#else
									var oracleTimeStampTZ = ((global::Oracle.ManagedDataAccess.Client.OracleDataReader)record.BaseDataRecord).GetOracleTimeStampTZ(i);
									#endif
									value = new DateTimeOffset((DateTime)oracleTimeStampTZ, TimeSpan.Parse(oracleTimeStampTZ.TimeZone == "+00:00" ? "00:00" : oracleTimeStampTZ.TimeZone));
								}
								else
									value = record.GetValue(i);
								resultParameters.Add(new PersistenceParameter(parameterName, value));
								if(typeMetadata.IsVersioned && StringComparer.InvariantCultureIgnoreCase.Equals(Query.DataAccessConstants.RowVersionPropertyName, parameterName))
									try
									{
										if(value is DBNull || value == null)
											actionResult.RowVersion = 0;
										else
											actionResult.RowVersion = Convert.ToInt64(value);
									}
									catch
									{
									}
							}
						}
					}
					if(actionResult.Parameters != null)
						foreach (var i in actionResult.Parameters)
							if(!resultParameters.Any(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Name, i.Name)))
								resultParameters.Add(i);
					actionResult.Parameters = resultParameters.ToArray();
				}
			}
		}

		/// <summary>
		/// Executa um comando de insert no banco de dados.
		/// </summary>
		/// <param name="commandText">Texto do comando a ser executado.</param>
		/// <param name="action">Informação da ação de persistência.</param>
		/// <param name="transaction">Transação do comando.</param>
		/// <param name="primaryKeyMappings">Dicionário que mapeia ids virtuais para reais.</param>
		/// <returns>Retorna resultado da ação de persistência.</returns>
		protected override PersistenceActionResult ExecuteInsertCommand(string commandText, PersistenceAction action, IPersistenceTransactionExecuter transaction, Dictionary<int, int> primaryKeyMappings)
		{
			var trans = (PersistenceTransactionExecuter)transaction;
			bool hasRowVersion = (action.RowVersion != null);
			var typeMetadata = TypeSchema.GetTypeMetadata(action.EntityFullName);
			var volatileProperties = typeMetadata.GetVolatileProperties().ToArray();
			var onlyResultProperties = volatileProperties.Where(f => action.Parameters.Count(g => g.Name == f.Name) == 0);
			int onlyResultCount = onlyResultProperties.Count();
			var resultParameters = new PersistenceParameter[action.Parameters.Count + onlyResultCount];
			var parameters = new GDAParameter[action.Parameters.Count + 1];
			bool isPosCommand = GetKeyRepository(trans.ProviderName).IsPosCommand(action.EntityFullName);
			IPropertyMetadata identityMetadata = typeMetadata.GetKeyProperties().FirstOrDefault(f => f.ParameterType == Schema.PersistenceParameterType.IdentityKey);
			for(int i = 0; i < action.Parameters.Count; i++)
			{
				var propertyMetadata = typeMetadata[action.Parameters[i].Name];
				if(propertyMetadata == null)
					parameters[i] = CreateParameter('?' + action.Parameters[i].Name, action.Parameters[i].Value);
				else
					parameters[i] = CreateParameter('?' + propertyMetadata.ColumnName, action.Parameters[i].Value);
				if(action.Parameters[i].DbType != DbType.AnsiString)
					parameters[i].DbType = action.Parameters[i].DbType;
				resultParameters[i] = action.Parameters[i];
				if(isPosCommand && identityMetadata.Name == action.Parameters[i].Name)
					parameters[i].Direction = System.Data.ParameterDirection.InputOutput;
				if(volatileProperties.Any(p => p.Name == action.Parameters[i].Name))
					parameters[i].Direction = System.Data.ParameterDirection.InputOutput;
			}
			int index = action.Parameters.Count;
			foreach (var property in onlyResultProperties)
				resultParameters[index++] = new PersistenceParameter(property.Name, string.Empty);
			parameters[parameters.Length - 1] = CreateParameter('?' + DataAccessConstants.AffectedRowsParameterName, 0, 0, System.Data.ParameterDirection.Output);
			var da = new DataAccess(trans.Transaction.ProviderConfiguration);
			da.ExecuteCommand(trans.Transaction, CommandType.Text, action.CommandTimeout, commandText, parameters);
			int affectedRows = Convert.ToInt32(parameters[parameters.Length - 1].Value);
			if(GetKeyRepository(trans.ProviderName).IsPosCommand(action.EntityFullName))
			{
				if(identityMetadata.ParameterType == Schema.PersistenceParameterType.IdentityKey)
				{
					for(int i = 0; i < parameters.Length; i++)
					{
						if(action.Parameters[i].Name == identityMetadata.Name)
						{
							int identityValue = (int)parameters[i].Value;
							int virtualId = (int)action.Parameters[i].Value;
							action.Parameters[i].Value = identityValue;
							primaryKeyMappings.Add(virtualId, identityValue);
							break;
						}
					}
				}
			}
			var actionResult = new PersistenceActionResult() {
				Success = true,
				AffectedRows = affectedRows,
				Parameters = resultParameters,
				ActionId = action.ActionId,
				RowVersion = 0
			};
			return actionResult;
		}

		/// <summary>
		/// Executa um comando de update no banco de dados.
		/// </summary>
		/// <param name="commandText">Texto do comando a ser executado.</param>
		/// <param name="action">Informação da ação de persistência.</param>
		/// <param name="transaction">Transação do comando.</param>
		/// <param name="primaryKeyMappings">Dicionário que mapeia ids virtuais para reais.</param>
		/// <returns>Retorna resultado da ação de persistência.</returns>
		protected override PersistenceActionResult ExecuteUpdateCommand(string commandText, PersistenceAction action, IPersistenceTransactionExecuter transaction, Dictionary<int, int> primaryKeyMappings)
		{
			var trans = (PersistenceTransactionExecuter)transaction;
			var typeMetadata = TypeSchema.GetTypeMetadata(action.EntityFullName);
			bool hasRowVersion = typeMetadata.IsVersioned && (action.Conditional == null);
			var actionParameters = action.Parameters.Where(f => !(f.Value is PropertyReference || f.Value is ExpressionParameter || f.Value is QueryInfo || f.Value is Colosoft.Query.Queryable));
			var volatileProperties = typeMetadata.GetVolatileProperties().ToArray();
			var onlyResultProperties = volatileProperties.Where(f => !action.Parameters.Any(g => g.Name == f.Name)).ToArray();
			int parameterCount = actionParameters.Count();
			if(action.Conditional != null)
				parameterCount += action.Conditional.Count();
			if(action.Query != null)
				parameterCount += action.Query.Parameters.Count;
			if(hasRowVersion)
				parameterCount++;
			var resultParameters = new PersistenceParameter[action.Parameters.Count + onlyResultProperties.Length];
			var parameters = new GDAParameter[parameterCount + 1];
			int index = 0;
			foreach (var actionParameter in actionParameters)
			{
				var propertyMetadata = typeMetadata[actionParameter.Name];
				if(propertyMetadata == null)
					parameters[index] = CreateParameter('?' + actionParameter.Name, actionParameter.Value);
				else
					parameters[index] = CreateParameter('?' + propertyMetadata.ColumnName, actionParameter.Value);
				if(actionParameter.DbType != DbType.AnsiString)
					parameters[index].DbType = actionParameter.DbType;
				resultParameters[index] = actionParameter;
				if(volatileProperties.Any(p => p.Name == actionParameter.Name))
					parameters[index].Direction = System.Data.ParameterDirection.InputOutput;
				index++;
			}
			index = actionParameters.Count();
			if(action.Conditional == null)
			{
				var index2 = index;
				foreach (var property in onlyResultProperties)
					resultParameters[index2++] = new PersistenceParameter(property.Name, string.Empty, Query.ParameterDirection.Output);
			}
			else
				foreach (var param in action.Conditional)
					parameters[index++] = CreateParameter(param.Name, param.Value);
			if(action.Query != null)
				foreach (var param in action.Query.Parameters)
					parameters[index++] = CreateParameter(param.Name, param.Value);
			if(hasRowVersion)
				parameters[index++] = CreateParameter('?' + DataAccessConstants.RowVersionPropertyName, action.RowVersion, 0, System.Data.ParameterDirection.Input);
			parameters[index] = CreateParameter('?' + DataAccessConstants.AffectedRowsParameterName, 0, 0, System.Data.ParameterDirection.Output);
			var da = new DataAccess(trans.Transaction.ProviderConfiguration);
			try
			{
				da.ExecuteCommand(trans.Transaction, CommandType.Text, action.CommandTimeout, commandText, parameters);
			}
			catch(GDAException ex)
			{
				Exception ex2 = ex;
				if(ex.InnerException is System.Data.Common.DbException || ex.InnerException is DataException)
					ex2 = ex.InnerException;
				ex2 = new DataException(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_ExecuteDatabaseCommand, ex2.Message).Format(), ex2);
				action.NotifyError(ex2);
				return new PersistenceActionResult {
					Success = false,
					AffectedRows = -1,
					FailureMessage = ex2.Message
				};
			}
			int affectedRows = Convert.ToInt32(parameters[index].Value);
			if(affectedRows == 0 && hasRowVersion)
				return new PersistenceActionResult {
					Success = false,
					AffectedRows = -1,
					FailureMessage = ResourceMessageFormatter.Create(() => Database.Generic.Properties.Resources.Exception_RowsNotAffected, typeMetadata.FullName, action.RowVersion).Format(),
				};
			var actionResult = new PersistenceActionResult() {
				Success = true,
				AffectedRows = affectedRows,
				Parameters = resultParameters,
				ActionId = action.ActionId,
				RowVersion = 0
			};
			return actionResult;
		}

		/// <summary>
		/// Executa uma procedure no banco de dados.
		/// </summary>
		/// <param name="action">Informação da ação de persistência.</param>
		/// <param name="transaction">Transação de persistência.</param>
		/// <returns>Retorna resultado da ação de persistência.</returns>
		protected override PersistenceActionResult ExecuteProcedureCommand(PersistenceAction action, IPersistenceTransactionExecuter transaction)
		{
			var trans = (PersistenceTransactionExecuter)transaction;
			DataAccess da = trans != null ? new DataAccess(trans.Transaction.ProviderConfiguration) : new DataAccess();
			var indexes = new List<int>();
			var storedProcedureName = (Query.Database.TranslatedStoredProcedureName)Translator.GetName(action.StoredProcedureName);
			var script = "BEGIN " + (!string.IsNullOrEmpty(storedProcedureName.Schema) ? string.Format("\"{0}\".\"{1}\"", storedProcedureName.Schema, storedProcedureName.Name) : string.Format("\"{0}\"", storedProcedureName.Name)) + " (";
			for(var i = 1; i <= action.Parameters.Count; i++)
				script += ":" + i + ",";
			script = (action.Parameters.Count > 0 ? script.Substring(0, script.Length - 1) : script) + "); END;";
			var parameters = new GDA.Collections.GDAParameterCollection();
			for(int i = 0; i < action.Parameters.Count; i++)
			{
				var paramDirection = (System.Data.ParameterDirection)((int)action.Parameters[i].Direction);
				var value = action.Parameters[i].Value;
				parameters.Add(CreateParameter(":" + (i + 1), value, action.Parameters[i].Size, paramDirection));
				if(action.Parameters[i].Direction != Query.ParameterDirection.Input)
					indexes.Add(i);
			}
			var parameters2 = parameters.ToArray();
			var result = da.ExecuteCommand(trans.Transaction, CommandType.Text, action.CommandTimeout, script, parameters2);
			foreach (int index in indexes)
				action.Parameters[index].Value = parameters2[index].Value;
			var presult = new PersistenceActionResult() {
				ActionId = action.ActionId,
				Parameters = action.Parameters.ToArray(),
				Result = result,
				Success = true
			};
			return presult;
		}

		/// <summary>
		/// Executa o comando de exclusão.
		/// </summary>
		/// <param name="commandText"></param>
		/// <param name="action"></param>
		/// <param name="transaction"></param>
		/// <param name="primaryKeyMappings"></param>
		/// <returns></returns>
		protected override PersistenceActionResult ExecuteDeleteCommand(string commandText, PersistenceAction action, IPersistenceTransactionExecuter transaction, Dictionary<int, int> primaryKeyMappings)
		{
			var trans = (PersistenceTransactionExecuter)transaction;
			var typeMetadata = TypeSchema.GetTypeMetadata(action.EntityFullName);
			bool hasRowVersion = typeMetadata.IsVersioned && (action.Conditional == null);
			int parameterCount = (action.Conditional != null ? action.Conditional.Count() : 0) + action.Parameters.Count;
			if(hasRowVersion)
				parameterCount++;
			var parameters = new GDAParameter[parameterCount];
			var index = 0;
			if(action.Conditional == null)
			{
				for(int i = 0; i < action.Parameters.Count; i++)
				{
					var propertyMetadata = typeMetadata[action.Parameters[i].Name];
					if(propertyMetadata == null)
						parameters[i] = CreateParameter('?' + action.Parameters[i].Name, action.Parameters[i].Value);
					else
						parameters[i] = CreateParameter('?' + propertyMetadata.ColumnName, action.Parameters[i].Value);
				}
				index = action.Parameters.Count;
			}
			else
			{
				foreach (var param in action.Conditional)
				{
					parameters[index] = CreateParameter(param.Name, param.Value);
					index++;
				}
				foreach (var param in action.Parameters)
				{
					parameters[index] = CreateParameter(param.Name, param.Value);
					index++;
				}
			}
			if(hasRowVersion)
				parameters[index] = CreateParameter('?' + DataAccessConstants.RowVersionPropertyName, action.RowVersion, 0, (System.Data.ParameterDirection)((int)Colosoft.Query.ParameterDirection.InputOutput));
			var da = new DataAccess(trans.Transaction.ProviderConfiguration);
			int affectedRows = 0;
			try
			{
				affectedRows = da.ExecuteCommand(trans.Transaction, CommandType.Text, action.CommandTimeout, commandText, parameters);
			}
			catch(GDAException ex)
			{
				Exception ex2 = ex;
				if(ex.InnerException is System.Data.Common.DbException || ex.InnerException is DataException)
					ex2 = ex.InnerException;
				ex2 = new DataException(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_ExecuteDatabaseCommand, ex2.Message).Format(), ex2);
				action.NotifyError(ex2);
				return new PersistenceActionResult {
					Success = false,
					AffectedRows = -1,
					FailureMessage = ex2.Message
				};
			}
			if(affectedRows == 0 && hasRowVersion)
				return new PersistenceActionResult {
					Success = false,
					AffectedRows = -1,
					FailureMessage = ResourceMessageFormatter.Create(() => Properties.Resources.Exception_RowsNotAffected, typeMetadata.FullName, action.RowVersion).Format()
				};
			action.NotifyExecution();
			var actionResult = new PersistenceActionResult() {
				Success = true,
				AffectedRows = affectedRows,
				Parameters = action.Parameters.ToArray(),
				ActionId = action.ActionId,
				RowVersion = (hasRowVersion) ? (long)action.RowVersion : 0
			};
			return actionResult;
		}
	}
}
