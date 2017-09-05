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
using Colosoft.Data.Schema;
using Microsoft.Practices.ServiceLocation;
using Colosoft.Query;

namespace Colosoft.Data.Database
{
	/// <summary>
	/// Implementação de <see cref="IPersistenceExecuter"/> para acesso direto ao banco de dados.
	/// </summary>
	public abstract class PersistenceExecuter : IPersistenceExecuter, IPersistenceExecuterTransactionSupport
	{
		private IServiceLocator _serviceLocator;

		private ITypeSchema _typeSchema;

		private Dictionary<string, IPrimaryKeyRepository> _primaryKeyRepositoryDictionary = new Dictionary<string, IPrimaryKeyRepository>();

		private object _lockObject = new object();

		/// <summary>
		/// Localizador do serviçe de IoC
		/// </summary>
		protected IServiceLocator ServiceLocator
		{
			get
			{
				return _serviceLocator;
			}
		}

		/// <summary>
		/// Classe de acesso aos metadados
		/// </summary>
		protected ITypeSchema TypeSchema
		{
			get
			{
				return _typeSchema;
			}
		}

		/// <summary>
		/// Construtor padrão   
		/// </summary>
		/// <param name="locator">Inteface de IoC</param>
		/// <param name="typeSchema">Classe de recuperação de metadados</param>
		public PersistenceExecuter(IServiceLocator locator, ITypeSchema typeSchema)
		{
			_serviceLocator = locator;
			_typeSchema = typeSchema;
		}

		/// <summary>
		/// Executa.
		/// </summary>
		/// <param name="actions">Ações a serem executadas</param>
		/// <param name="executionType">Representa o tipo de execução da operação de persistência.</param>
		/// <returns>Retorna um vetor resultados das ações</returns>
		public PersistenceExecuteResult Execute(PersistenceAction[] actions, ExecutionType executionType)
		{
			return Execute(actions, executionType, null);
		}

		/// <summary>
		/// Cria o repositório de chave primária.
		/// </summary>
		/// <param name="providerName">Nome do provider do repositório.</param>
		/// <returns></returns>
		protected abstract IPrimaryKeyRepository CreatePrimaryKeyRepository(string providerName);

		/// <summary>
		/// Cria o parser para a ação informada.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="providerName">Nome do provider associado.</param>
		/// <returns></returns>
		protected abstract PersistenceSqlParser CreateParser(PersistenceAction action, string providerName);

		/// <summary>
		/// Recupera o repositório de chaves primárias.
		/// </summary>
		/// <param name="providerName"></param>
		/// <returns></returns>
		protected IPrimaryKeyRepository GetKeyRepository(string providerName)
		{
			IPrimaryKeyRepository repository;
			if(!_primaryKeyRepositoryDictionary.TryGetValue(providerName, out repository))
			{
				var primaryKeyRepository = CreatePrimaryKeyRepository(providerName);
				primaryKeyRepository.ProviderName = providerName;
				_primaryKeyRepositoryDictionary.Add(providerName, primaryKeyRepository);
			}
			return _primaryKeyRepositoryDictionary[providerName];
		}

		/// <summary>
		/// Executa as ações sobre a transação informada.
		/// </summary>
		/// <param name="actions">Ações que serão executadas.</param>
		/// <param name="executionType">Tipo de execução.</param>
		/// <param name="transaction">Transação que será utilizada.</param>
		/// <param name="primaryKeyMappings">Mapeamentos das chaves primárias.</param>
		/// <param name="alternatives"></param>
		/// <param name="existsErrors"></param>
		/// <returns></returns>
		protected PersistenceActionResult[] Execute(PersistenceAction[] actions, ExecutionType executionType, IPersistenceTransactionExecuter transaction, Dictionary<int, int> primaryKeyMappings, List<Tuple<int, PersistenceActionResult>> alternatives, out bool existsErrors)
		{
			var result = new PersistenceActionResult[actions.Length];
			existsErrors = false;
			for(int i = 0; i < actions.Length; i++)
			{
				PersistenceActionResult[] beforeActions = null;
				try
				{
					string actionCommandText = null;
					if(actions[i].Type != PersistenceActionType.ExecuteProcedure)
					{
						var parser = CreateParser(actions[i], transaction.ProviderName);
						actionCommandText = parser.GetPersistenceCommandText();
						if(actions[i].Conditional == null)
						{
							SwapIds(actions[i], primaryKeyMappings);
							GetId(actions[i], primaryKeyMappings, transaction.ProviderName, transaction);
						}
					}
					SwapNewUidReferences(actions[i], primaryKeyMappings);
					if(actions[i].BeforeActions.Count > 0)
					{
						var existsErrors2 = false;
						beforeActions = Execute(actions[i].BeforeActions.ToArray(), executionType, transaction, primaryKeyMappings, alternatives, out existsErrors2);
						if(existsErrors2)
							existsErrors = true;
						if(existsErrors2 && executionType == ExecutionType.Default)
						{
							result[i] = new PersistenceActionResult {
								Success = false,
								BeforeActions = beforeActions
							};
							break;
						}
					}
					PersistenceActionResult commandResult = null;
					var alternative = alternatives.Where(f => f.Item1 == actions[i].ActionId).FirstOrDefault();
					if(alternative != null)
					{
						commandResult = result[i] = alternative.Item2;
						var existsErrors2 = false;
						commandResult.AlternativeActions = Execute(actions[i].AlternativeActions.ToArray(), executionType, transaction, primaryKeyMappings, alternatives, out existsErrors2);
						if(existsErrors2)
							existsErrors = true;
						else
							commandResult.FailureMessage = null;
						commandResult.Success = !existsErrors2;
						if(existsErrors2 && executionType == ExecutionType.Default)
							break;
					}
					else
					{
						try
						{
							commandResult = result[i] = ExecuteCommand(actionCommandText, actions[i], transaction, primaryKeyMappings);
						}
						catch(Exception ex)
						{
							existsErrors = true;
							result[i] = new PersistenceActionResult() {
								BeforeActions = beforeActions ?? new PersistenceActionResult[0],
								FailureMessage = ex.Message,
								Success = false
							};
							if(actions[i].AlternativeActions.Count > 0 && !alternatives.Where(f => f.Item1 == actions[i].ActionId).Any())
								alternatives.Add(new Tuple<int, PersistenceActionResult>(actions[i].ActionId, result[i]));
							if(executionType == ExecutionType.Default)
								break;
							continue;
						}
					}
					commandResult.BeforeActions = beforeActions;
					if(!commandResult.Success)
					{
						existsErrors = true;
						if(actions[i].AlternativeActions.Count > 0)
						{
							if(!alternatives.Where(f => f.Item1 == actions[i].ActionId).Any())
								alternatives.Add(new Tuple<int, PersistenceActionResult>(actions[i].ActionId, result[i]));
						}
						if(executionType == ExecutionType.Default)
							break;
						continue;
					}
					if(actions[i].AfterActions.Count > 0)
					{
						var existsErrors2 = false;
						commandResult.AfterActions = Execute(actions[i].AfterActions.ToArray(), executionType, transaction, primaryKeyMappings, alternatives, out existsErrors2);
						if(existsErrors2)
							existsErrors = true;
						commandResult.Success = !existsErrors2;
						var failureAction = commandResult.AfterActions.Where(f => !f.Success).FirstOrDefault();
						if(failureAction != null)
						{
							commandResult.Success = false;
						}
						if(existsErrors2 && executionType == ExecutionType.Default)
							break;
					}
				}
				catch(Exception ex)
				{
					existsErrors = true;
					result[i] = new PersistenceActionResult() {
						BeforeActions = beforeActions ?? new PersistenceActionResult[0],
						FailureMessage = ex.Message,
						Success = false
					};
					if(executionType == ExecutionType.Default)
						break;
				}
			}
			if(actions.Length != result.Length)
			{
				existsErrors = true;
				for(var i = 0; i < actions.Length; i++)
					result[i] = new PersistenceActionResult() {
						FailureMessage = "Invalid result length for actions",
						Success = false
					};
			}
			if(!existsErrors)
				transaction.Commited += (sender, e) =>  {
					RetrievePersistenceVolatileProperties(actions, result, transaction);
				};
			return result;
		}

		/// <summary>
		/// Executa uma ação de persistência no banco de dados
		/// </summary>
		/// <param name="commandText">Texto do comando a ser executado</param>
		/// <param name="action">Ação a ser executada</param>
		/// <param name="transaction">Objeto de transação</param>
		/// <param name="primaryKeyMappings">Dicionário que mapeia ids virtuais para reais</param>
		/// <returns>Retorna resultado da ação de persistência</returns>
		protected abstract PersistenceActionResult ExecuteCommand(string commandText, PersistenceAction action, IPersistenceTransactionExecuter transaction, Dictionary<int, int> primaryKeyMappings);

		/// <summary>
		/// Registra as informações do usuário no banco de dados.
		/// </summary>
		protected abstract void RegisterUserInfo(IPersistenceTransactionExecuter transaction);

		/// <summary>
		/// Configura a transação.
		/// </summary>
		/// <param name="transaction"></param>
		protected virtual void ConfigureTransaction(IPersistenceTransactionExecuter transaction)
		{
		}

		/// <summary>
		/// Recupera as propriedades volatéis.           
		/// </summary>
		/// <param name="actions">Ações de persistência.</param>
		/// <param name="actionResult">resultado das ações de persistência.</param>
		/// <param name="transaction">Transação usada na execução.</param>
		protected virtual void RetrievePersistenceVolatileProperties(PersistenceAction[] actions, PersistenceActionResult[] actionResult, IPersistenceTransactionExecuter transaction)
		{
		}

		/// <summary>
		/// Delega para classe que a implementa  o objeto de transação
		/// </summary>
		/// <returns>Objeto da transação</returns>
		protected abstract IPersistenceTransactionExecuter BeginTransaction(string entityFullName);

		/// <summary>
		/// Executa.
		/// </summary>
		/// <param name="actions">Ações a serem executadas</param>
		/// <param name="executionType">Representa o tipo de execução da operação de persistência.</param>
		/// <param name="transactionCreator"></param>
		/// <returns>Retorna um vetor resultados das ações</returns>
		private PersistenceExecuteResult Execute(PersistenceAction[] actions, ExecutionType executionType, Func<IPersistenceTransactionExecuter> transactionCreator)
		{
			actions.Require("actions").NotNull();
			bool existErrors = false;
			var primaryKeyMappings = new Dictionary<int, int>();
			PersistenceActionResult[] result = null;
			var alternatives = new List<Tuple<int, PersistenceActionResult>>();
			var alternativesCount = 0;
			while (true)
			{
				IPersistenceTransactionExecuter transaction;
				if(transactionCreator == null)
				{
					if(actions[0].Type == PersistenceActionType.ExecuteProcedure)
						transaction = BeginTransaction(actions[0].ProviderName);
					else
						transaction = BeginTransaction(TypeSchema.GetTypeMetadata(actions[0].EntityFullName).TableName.Catalog);
				}
				else
					transaction = transactionCreator();
				try
				{
					ConfigureTransaction(transaction);
					RegisterUserInfo(transaction);
					alternativesCount = alternatives.Count;
					result = Execute(actions, executionType, transaction, primaryKeyMappings, alternatives, out existErrors);
					if(existErrors || executionType == ExecutionType.ExecuteAndRollback)
					{
						if(transactionCreator == null)
							transaction.Rollback();
						if(alternativesCount == alternatives.Count)
							break;
					}
					else
					{
						if(transactionCreator == null)
							transaction.Commit();
						break;
					}
				}
				finally
				{
					if(transactionCreator == null && transaction != null)
						transaction.Dispose();
				}
			}
			return new PersistenceExecuteResult(result);
		}

		/// <summary>
		/// Realiza a troca das referencias de novos Uids.
		/// </summary>
		/// <param name="action">Ação de persistência sobre a qual será executada as trocas</param>
		/// <param name="primaryKeyMappings">Dicionário que mapeia ids virtuais para reais</param>
		private void SwapNewUidReferences(PersistenceAction action, Dictionary<int, int> primaryKeyMappings)
		{
			int realId = 0;
			foreach (var parameter in action.Parameters)
			{
				if(parameter.Value is NewUidReference)
				{
					var virtualId = ((NewUidReference)parameter.Value).Uid;
					if(primaryKeyMappings.TryGetValue(virtualId, out realId))
						parameter.Value = realId;
					else
						throw new Colosoft.DetailsException(string.Format("Not found real value for NewUidReference '{0}' from parameter '{1}'", virtualId, parameter.Name).GetFormatter());
				}
			}
			if(action.Conditional != null)
				foreach (var parameter in action.Conditional)
				{
					if(parameter.Value is NewUidReference)
					{
						var virtualId = ((NewUidReference)parameter.Value).Uid;
						if(primaryKeyMappings.TryGetValue(virtualId, out realId))
							parameter.Value = realId;
						else
							throw new Colosoft.DetailsException(string.Format("Not found real value for NewUidReference '{0}' from parameter '{1}'", virtualId, parameter.Name).GetFormatter());
					}
				}
		}

		/// <summary>
		/// Método que troca os ids virtuais por ids reais
		/// </summary>
		/// <param name="action">Ação de persistência sobre a qual será executada as trocas</param>
		/// <param name="primaryKeyMappings">Dicionário que mapeia ids virtuais para reais</param>
		private void SwapIds(PersistenceAction action, Dictionary<int, int> primaryKeyMappings)
		{
			var typeMetadata = TypeSchema.GetTypeMetadata(action.EntityFullName);
			for(int i = 0; i < action.Parameters.Count; i++)
			{
				var propertyMetadata = typeMetadata[action.Parameters[i].Name];
				if(propertyMetadata == null)
					continue;
				if(propertyMetadata.IsForeignKey || propertyMetadata.ForeignKeyTypeCode.HasValue || propertyMetadata.ParameterType == PersistenceParameterType.IdentityKey)
				{
					int realId;
					int virtualId = 0;
					object value = action.Parameters[i].Value;
					if(value is int)
						virtualId = (int)value;
					else if(value is uint)
						virtualId = (int)(uint)value;
					else
					{
						if(value == null)
							continue;
						var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(int));
						if(converter == null && !converter.CanConvertFrom(value.GetType()))
							continue;
						try
						{
							virtualId = (int)converter.ConvertFrom(value);
						}
						catch(NotSupportedException)
						{
							continue;
						}
					}
					if(primaryKeyMappings.TryGetValue(virtualId, out realId))
					{
						action.Parameters[i].Value = realId;
					}
				}
			}
		}

		/// <summary>
		/// Gera o id em caso de inserção
		/// </summary>
		/// <param name="action">Ação a ser executada</param>
		/// <param name="primaryKeyMappings">Dicionário que mapeia ids virtuais para reais</param>
		/// <param name="providerName">Nome do provedor.</param>
		/// <param name="transaction">Transação.</param>
		private void GetId(PersistenceAction action, Dictionary<int, int> primaryKeyMappings, string providerName, IPersistenceTransactionExecuter transaction)
		{
			var typeMetadata = TypeSchema.GetTypeMetadata(action.EntityFullName);
			if(action.Type == PersistenceActionType.Insert && !GetKeyRepository(providerName).IsPosCommand(typeMetadata.FullName))
			{
				for(int i = 0; i < action.Parameters.Count; i++)
				{
					var propertyMetadata = typeMetadata[action.Parameters[i].Name];
					if(propertyMetadata.ParameterType == PersistenceParameterType.IdentityKey)
					{
						int virtualId = 0;
						object value = action.Parameters[i].Value;
						if(value is int)
							virtualId = (int)value;
						else
						{
							var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(int));
							if(converter == null && !converter.CanConvertFrom(value.GetType()))
								continue;
							try
							{
								virtualId = (int)converter.ConvertFrom(value);
							}
							catch(NotSupportedException)
							{
								continue;
							}
						}
						if(virtualId <= 0)
						{
							var keyRepository = GetKeyRepository(providerName);
							var key = new PersistenceParameter(action.Parameters[i].Name, keyRepository.GetPrimaryKey(transaction, typeMetadata.FullName));
							action.Parameters[i] = key;
							primaryKeyMappings.Add(virtualId, (int)key.Value);
						}
						else
							throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.ForeignMemberNotInserted, propertyMetadata.Name, virtualId.ToString(), typeMetadata.FullName).Format());
						return;
					}
				}
			}
		}

		/// <summary>
		/// Retorna um objeto EntityInfo baseado no nome
		/// </summary>
		/// <param name="name">Nome da entidade</param>
		/// <returns>Retorna um objeto <see cref="EntityInfo"/></returns>
		private EntityInfo GetEntityInfo(string name)
		{
			return new EntityInfo() {
				FullName = name,
				Alias = null
			};
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Executa as ações informadas.
		/// </summary>
		/// <param name="actions">Instancia das ações que serão executadas.</param>
		/// <param name="executionType">Representa o tipo de execução da operação de persistência.</param>
		/// <param name="transactionCreator">Método usado para cria a transação.</param>
		/// <returns>Resulta da execução das ações.</returns>
		PersistenceExecuteResult IPersistenceExecuterTransactionSupport.Execute(PersistenceAction[] actions, ExecutionType executionType, Func<IPersistenceTransactionExecuter> transactionCreator)
		{
			return Execute(actions, executionType, transactionCreator);
		}
	}
}
