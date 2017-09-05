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
using Colosoft.Query;
using System.Data;
using Microsoft.Practices.ServiceLocation;
using Colosoft.Data.Schema;

namespace Colosoft.Data.Database.Generic
{
	/// <summary>
	/// Implementação do PersistenceExectuter usando o GDA
	/// </summary>
	public abstract class GenericPersistenceExecuter : PersistenceExecuter
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
		/// Construtor padrão   
		/// </summary>
		/// <param name="locator">Inteface de IoC</param>
		/// <param name="typeSchema">Classe de recuperação de metadados</param>
		/// <param name="translator">Instancia do tradutor.</param>
		public GenericPersistenceExecuter(IServiceLocator locator, ITypeSchema typeSchema, IQueryTranslator translator) : base(locator, typeSchema)
		{
			_translator = translator;
		}

		/// <summary>
		/// Cria um parâmetro para a consulta.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="value">Valor do parâmetro.</param>
		/// <param name="direction"></param>
		/// <returns>Objeto do tipo <see cref="GDAParameter"/>.</returns>
		protected GDAParameter CreateParameter(string name, object value, System.Data.ParameterDirection direction)
		{
			return CreateParameter(name, value, 0, direction);
		}

		/// <summary>
		/// Cria um parâmetro para a consulta.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="value">Valor do parâmetro.</param>
		/// <param name="size">Tamanho do parametro.</param>
		/// <param name="direction"></param>
		/// <returns>Objeto do tipo <see cref="GDAParameter"/>.</returns>
		protected virtual GDAParameter CreateParameter(string name, object value, int size, System.Data.ParameterDirection direction)
		{
			return new GDAParameter(name, value, direction) {
				Size = size
			};
		}

		/// <summary>
		/// Cria um parâmetro para a consulta.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="value">Valor do parâmetro.</param>
		/// <returns>Objeto do tipo <see cref="GDAParameter"/>.</returns>
		protected virtual GDAParameter CreateParameter(string name, object value)
		{
			return new GDAParameter(name, value);
		}

		/// <summary>
		/// Executa o comando no banco de dados 
		/// </summary>
		/// <param name="commandText">Texto do comando a ser executado</param>
		/// <param name="action">Texto do comando</param>
		/// <param name="transaction">Transação do comando</param>
		/// <param name="primaryKeyMappings">Dicionário que mapeia ids virtuais para reais</param>
		/// <returns>Retorna resultado da ação de persistência</returns>
		protected override PersistenceActionResult ExecuteCommand(string commandText, PersistenceAction action, IPersistenceTransactionExecuter transaction, Dictionary<int, int> primaryKeyMappings)
		{
			switch(action.Type)
			{
			case PersistenceActionType.Insert:
				return ExecuteInsertCommand(commandText, action, transaction, primaryKeyMappings);
			case PersistenceActionType.Update:
				return ExecuteUpdateCommand(commandText, action, transaction, primaryKeyMappings);
			case PersistenceActionType.Delete:
				return ExecuteDeleteCommand(commandText, action, transaction, primaryKeyMappings);
			case PersistenceActionType.ExecuteProcedure:
				return ExecuteProcedureCommand(action, transaction);
			default:
				return new PersistenceActionResult {
					Success = false,
					AffectedRows = -1,
					FailureMessage = ResourceMessageFormatter.Create(() => Properties.Resources.NotImplementedException_PersistenceTypeNotSupported, action.Type.ToString()).Format()
				};
			}
		}

		/// <summary>
		/// Registra as informações do usuário no banco de dados.
		/// </summary>
		protected override void RegisterUserInfo(IPersistenceTransactionExecuter transaction)
		{
		}

		/// <summary>
		/// Inicia uma transação.
		/// </summary>
		/// <returns>Retorna a transação.</returns>
		protected override IPersistenceTransactionExecuter BeginTransaction(string providerName)
		{
			var transactionExecuter = ServiceLocator.GetInstance<IPersistenceTransactionExecuter>();
			transactionExecuter.ProviderName = !string.IsNullOrEmpty(providerName) ? providerName : GDA.GDASettings.DefaultProviderName ?? "Default";
			return transactionExecuter;
		}

		/// <summary>
		/// Executa um comando de insert no banco de dados.
		/// </summary>
		/// <param name="commandText">Texto do comando a ser executado.</param>
		/// <param name="action">Informação da ação de persistência.</param>
		/// <param name="transaction">Transação do comando.</param>
		/// <param name="primaryKeyMappings">Dicionário que mapeia ids virtuais para reais.</param>
		/// <returns>Retorna resultado da ação de persistência.</returns>
		protected virtual PersistenceActionResult ExecuteInsertCommand(string commandText, PersistenceAction action, IPersistenceTransactionExecuter transaction, Dictionary<int, int> primaryKeyMappings)
		{
			var trans = (PersistenceTransactionExecuter)transaction;
			var typeMetadata = TypeSchema.GetTypeMetadata(action.EntityFullName);
			bool hasRowVersion = typeMetadata.IsVersioned;
			var onlyResultProperties = typeMetadata.GetVolatileProperties().Where(f => action.Parameters.Count(g => g.Name == f.Name) == 0);
			int onlyResultCount = onlyResultProperties.Count();
			int parameterCount = action.Parameters.Count + onlyResultCount;
			if(hasRowVersion)
				parameterCount++;
			var resultParameters = new PersistenceParameter[action.Parameters.Count + onlyResultCount];
			var parameters = new GDAParameter[parameterCount + onlyResultCount];
			for(int i = 0; i < action.Parameters.Count; i++)
			{
				parameters[i] = CreateParameter('?' + action.Parameters[i].Name, action.Parameters[i].Value);
				if(action.Parameters[i].DbType != DbType.AnsiString)
					parameters[i].DbType = action.Parameters[i].DbType;
				resultParameters[i] = action.Parameters[i];
			}
			int index = action.Parameters.Count;
			foreach (var property in onlyResultProperties)
			{
				parameters[index] = CreateParameter('?' + property.Name, "", 0, System.Data.ParameterDirection.InputOutput);
				resultParameters[index] = new PersistenceParameter(property.Name, "");
				index++;
			}
			if(hasRowVersion)
				parameters[index] = CreateParameter('?' + DataAccessConstants.RowVersionPropertyName, action.RowVersion, 0, System.Data.ParameterDirection.Output);
			int affectedRows = 0;
			try
			{
				var da = new DataAccess(trans.Transaction.ProviderConfiguration);
				affectedRows = da.ExecuteCommand(trans.Transaction, CommandType.Text, action.CommandTimeout, commandText, parameters);
			}
			catch(Exception ex)
			{
				Exception ex2 = ex;
				if(ex.InnerException is System.Data.Common.DbException || ex.InnerException is DataException)
					ex2 = ex.InnerException;
				ex = new DataException(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_ExecuteDatabaseCommand, ex2.Message).Format(), ex2);
				action.NotifyError(ex);
				return new PersistenceActionResult() {
					Success = false,
					AffectedRows = affectedRows,
					ActionId = action.ActionId,
					FailureMessage = ex.Message,
					RowVersion = (hasRowVersion) ? (long)parameters[parameters.Length - 1].Value : 0
				};
			}
			var keyRepository = GetKeyRepository(trans.ProviderName);
			if(keyRepository.IsPosCommand(action.EntityFullName))
			{
				IPropertyMetadata identityMetadata = typeMetadata.GetKeyProperties().FirstOrDefault();
				if(identityMetadata != null && identityMetadata.ParameterType == Schema.PersistenceParameterType.IdentityKey)
				{
					for(int i = 0; i < parameters.Length; i++)
					{
						if(action.Parameters[i].Name == identityMetadata.Name)
						{
							int identityValue = 0;
							var key = keyRepository.GetPrimaryKey(transaction, typeMetadata.FullName);
							if(key is int)
								identityValue = (int)key;
							else if(key is uint)
								identityValue = (int)(uint)key;
							else
								identityValue = Convert.ToInt32(key);
							parameters[i].Value = identityValue;
							var virtualIdValue = action.Parameters[i].Value;
							int virtualId = 0;
							if(virtualIdValue is int)
								virtualId = (int)virtualIdValue;
							else if(virtualIdValue is uint)
								virtualId = (int)(uint)virtualIdValue;
							else
								virtualId = Convert.ToInt32(virtualIdValue);
							action.Parameters[i].Value = identityValue;
							primaryKeyMappings.Add(virtualId, identityValue);
							break;
						}
					}
				}
			}
			action.NotifyExecution();
			return new PersistenceActionResult() {
				Success = true,
				AffectedRows = affectedRows,
				Parameters = resultParameters,
				ActionId = action.ActionId,
				RowVersion = (hasRowVersion) ? (long)parameters[parameters.Length - 1].Value : 0
			};
		}

		/// <summary>
		/// Executa um comando de update no banco de dados.
		/// </summary>
		/// <param name="commandText">Texto do comando a ser executado.</param>
		/// <param name="action">Informação da ação de persistência.</param>
		/// <param name="transaction">Transação do comando.</param>
		/// <param name="primaryKeyMappings">Dicionário que mapeia ids virtuais para reais.</param>
		/// <returns>Retorna resultado da ação de persistência.</returns>
		protected virtual PersistenceActionResult ExecuteUpdateCommand(string commandText, PersistenceAction action, IPersistenceTransactionExecuter transaction, Dictionary<int, int> primaryKeyMappings)
		{
			var trans = (PersistenceTransactionExecuter)transaction;
			var typeMetadata = TypeSchema.GetTypeMetadata(action.EntityFullName);
			bool hasRowVersion = typeMetadata.IsVersioned && (action.Conditional == null);
			var actionParameters = action.Parameters.Where(f => !(f.Value is PropertyReference || f.Value is ExpressionParameter || f.Value is QueryInfo || f.Value is Colosoft.Query.Queryable));
			var volatileProperties = typeMetadata.GetVolatileProperties().ToArray();
			var onlyResultProperties = volatileProperties.Where(f => !action.Parameters.Any(g => g.Name == f.Name)).ToArray();
			int parameterCount = actionParameters.Count();
			if(action.Conditional != null)
				parameterCount += action.Conditional.Where(f => !(f.Value is PropertyReference || f.Value is ExpressionParameter || f.Value is QueryInfo || f.Value is Colosoft.Query.Queryable) && !actionParameters.Any(x => x.Name == f.Name)).Count();
			if(action.Query != null)
				parameterCount += action.Query.Parameters.Where(f => !(f.Value is PropertyReference || f.Value is ExpressionParameter || f.Value is QueryInfo || f.Value is Colosoft.Query.Queryable) && !actionParameters.Any(x => x.Name == f.Name)).Count();
			if(hasRowVersion)
				parameterCount++;
			var resultParameters = new PersistenceParameter[action.Parameters.Count + onlyResultProperties.Length];
			var parameters = new GDAParameter[parameterCount];
			int index = 0;
			foreach (var actionParameter in actionParameters)
			{
				var propertyMetadata = typeMetadata[actionParameter.Name];
				if(propertyMetadata == null)
					parameters[index] = CreateParameter(!string.IsNullOrEmpty(actionParameter.Name) && actionParameter.Name.StartsWith("?") ? actionParameter.Name : '?' + actionParameter.Name, actionParameter.Value);
				else
					parameters[index] = CreateParameter('?' + propertyMetadata.Name, actionParameter.Value);
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
				foreach (var param in action.Conditional.Where(f => !(f.Value is PropertyReference || f.Value is ExpressionParameter || f.Value is QueryInfo || f.Value is Colosoft.Query.Queryable) && !actionParameters.Any(x => x.Name == f.Name)))
					parameters[index++] = CreateParameter(param.Name, param.Value);
			if(action.Query != null)
				foreach (var param in action.Query.Parameters.Where(f => !(f.Value is PropertyReference || f.Value is ExpressionParameter || f.Value is QueryInfo || f.Value is Colosoft.Query.Queryable) && !actionParameters.Any(x => x.Name == f.Name)))
					parameters[index++] = CreateParameter(param.Name, param.Value);
			if(hasRowVersion)
				parameters[index++] = CreateParameter('?' + DataAccessConstants.RowVersionPropertyName, action.RowVersion, 0, System.Data.ParameterDirection.Input);
			//// Recupera o parametro para recuperar a quantidade de linhas afetadas
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
					FailureMessage = ResourceMessageFormatter.Create(() => Database.Generic.Properties.Resources.Exception_RowsNotAffected, typeMetadata.FullName, action.RowVersion).Format(),
				};
			var actionResult = new PersistenceActionResult() {
				Success = true,
				AffectedRows = affectedRows,
				Parameters = resultParameters,
				ActionId = action.ActionId,
				RowVersion = (hasRowVersion) ? (long)parameters[parameters.Length - 1].Value : 0
			};
			return actionResult;
		}

		/// <summary>
		/// Executa um comando de delete no banco de dados.
		/// </summary>
		/// <param name="commandText">Texto do comando a ser executado.</param>
		/// <param name="action">Informação da ação de persistência.</param>
		/// <param name="transaction">Transação do comando.</param>
		/// <param name="primaryKeyMappings">Dicionário que mapeia ids virtuais para reais.</param>
		/// <returns>Retorna resultado da ação de persistência.</returns>
		protected virtual PersistenceActionResult ExecuteDeleteCommand(string commandText, PersistenceAction action, IPersistenceTransactionExecuter transaction, Dictionary<int, int> primaryKeyMappings)
		{
			var trans = (PersistenceTransactionExecuter)transaction;
			var typeMetadata = TypeSchema.GetTypeMetadata(action.EntityFullName);
			bool hasRowVersion = typeMetadata.IsVersioned && (action.Conditional == null);
			var actionParameters = action.Parameters.Where(f => !(f.Value is PropertyReference || f.Value is ExpressionParameter || f.Value is QueryInfo || f.Value is Colosoft.Query.Queryable));
			var conditionalParameters = action.Conditional != null ? action.Conditional.Where(f => !(f.Value is PropertyReference || f.Value is ExpressionParameter || f.Value is QueryInfo || f.Value is Colosoft.Query.Queryable) && !actionParameters.Any(x => x.Name == f.Name)).ToArray() : new QueryParameter[0];
			int parameterCount = conditionalParameters.Where(f => !actionParameters.Any(x => x.Name == f.Name)).Count() + actionParameters.Count();
			if(hasRowVersion)
				parameterCount++;
			var parameters = new GDAParameter[parameterCount];
			var index = 0;
			if(action.Conditional == null)
			{
				foreach (var actionParameter in actionParameters)
				{
					var propertyMetadata = typeMetadata[actionParameter.Name];
					if(propertyMetadata == null)
						parameters[index++] = CreateParameter(!string.IsNullOrEmpty(actionParameter.Name) && actionParameter.Name.StartsWith("?") ? actionParameter.Name : '?' + actionParameter.Name, actionParameter.Value);
					else
						parameters[index++] = CreateParameter('?' + propertyMetadata.Name, actionParameter.Value);
				}
			}
			else
			{
				foreach (var param in conditionalParameters.Where(f => !actionParameters.Any(x => x.Name == f.Name)))
				{
					parameters[index] = CreateParameter(param.Name, param.Value);
					index++;
				}
				foreach (var param in actionParameters)
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

		/// <summary>
		/// Executa uma procedure no banco de dados.
		/// </summary>
		/// <param name="action">Informação da ação de persistência.</param>
		/// <param name="transaction">Transação de persistência.</param>
		/// <returns>Retorna resultado da ação de persistência.</returns>
		protected virtual PersistenceActionResult ExecuteProcedureCommand(PersistenceAction action, IPersistenceTransactionExecuter transaction)
		{
			var trans = (PersistenceTransactionExecuter)transaction;
			var outputParametersIndexes = new List<int>();
			var procedure = CreateProcedure(action);
			if(action.Parameters != null)
			{
				for(int i = 0; i < action.Parameters.Count; i++)
				{
					var paramDirection = (System.Data.ParameterDirection)((int)action.Parameters[i].Direction);
					procedure.AddParameter(new GDAParameter(action.Parameters[i].Name, action.Parameters[i].Value, paramDirection) {
						Size = action.Parameters[i].Size
					});
					if(action.Parameters[i].Direction != Colosoft.Query.ParameterDirection.Input)
						outputParametersIndexes.Add(i);
				}
			}
			var da = new DataAccess(trans.Transaction.ProviderConfiguration);
			object result = null;
			try
			{
				result = da.ExecuteScalar(trans.Transaction, procedure);
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
			foreach (int index in outputParametersIndexes)
			{
				action.Parameters[index].Value = procedure[index];
			}
			action.NotifyExecution();
			var presult = new PersistenceActionResult() {
				ActionId = action.ActionId,
				Parameters = action.Parameters.ToArray(),
				Result = result,
				Success = true
			};
			return presult;
		}

		/// <summary>
		/// Cria a procedure.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		protected virtual GDAStoredProcedure CreateProcedure(PersistenceAction action)
		{
			var procedure = new GDAStoredProcedure((!string.IsNullOrEmpty(action.StoredProcedureName.Schema) ? string.Format("\"{0}\".\"{1}\"", action.StoredProcedureName.Schema, action.StoredProcedureName.Name) : string.Format("\"{0}\"", action.StoredProcedureName.Name)));
			return procedure;
		}
	}
}
