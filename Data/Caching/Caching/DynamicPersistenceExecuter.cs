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

namespace Colosoft.Data.Caching.Dymanic
{
	/// <summary>
	/// Implementação do executor de persistencia dinamico.
	/// </summary>
	class DynamicPersistenceExecuter : IPersistenceExecuter, IPersistenceExecuteResultProcessor
	{
		/// <summary>
		/// Nome do parametro que armazena o resultado de uma ação de exclusão.
		/// </summary>
		public const string DeleteActionResultParameterName = "DeleteActionResult";

		private IPersistenceExecuter _databaseExecuter;

		private IPersistenceExecuter _cacheExecuter;

		private Colosoft.Data.Schema.PersistenceActionResultProcessor _actionResultProcessor;

		private Colosoft.Data.Schema.ITypeSchema _typeSchema;

		private Colosoft.Caching.ICacheProvider _cacheProvider;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="databaseExecuter">Executor do banco de dados.</param>
		/// <param name="cacheExecuter">Executor do cache.</param>
		/// <param name="cacheProvider"></param>
		/// <param name="typeSchema"></param>
		/// <param name="recordKeyFactory"></param>
		public DynamicPersistenceExecuter(IPersistenceExecuter databaseExecuter, IPersistenceExecuter cacheExecuter, Colosoft.Caching.ICacheProvider cacheProvider, Colosoft.Data.Schema.ITypeSchema typeSchema, Query.IRecordKeyFactory recordKeyFactory)
		{
			_databaseExecuter = databaseExecuter;
			_cacheExecuter = cacheExecuter;
			_cacheProvider = cacheProvider;
			_typeSchema = typeSchema;
			_actionResultProcessor = new Schema.PersistenceActionResultProcessor(typeSchema, recordKeyFactory);
		}

		/// <summary>
		/// Executa as ações de persistencia informadas.
		/// </summary>
		/// <param name="actions"></param>
		/// <param name="executionType"></param>
		/// <returns></returns>
		public PersistenceExecuteResult Execute(PersistenceAction[] actions, ExecutionType executionType)
		{
			if(_cacheProvider != null && _cacheProvider.Cache != null && (!_cacheProvider.IsInitialized || !_cacheProvider.Cache.IsLoaded) && ExistsCacheType(actions))
			{
				if(!_cacheProvider.Cache.IsLoaded)
				{
					var resetEvent = new System.Threading.ManualResetEvent(false);
					while (!_cacheProvider.Cache.IsLoaded && !resetEvent.WaitOne(50))
						;
				}
			}
			var result = _databaseExecuter.Execute(actions, executionType);
			if(!result.Success)
				return result;
			return new DynamicPersistenceExecuteResult(actions, executionType, result);
		}

		/// <summary>
		/// Verifica se existe algum tipo das ações que está é indexado pelo cache.
		/// </summary>
		/// <param name="actions"></param>
		/// <returns></returns>
		private bool ExistsCacheType(IEnumerable<PersistenceAction> actions)
		{
			if(actions == null)
				return false;
			foreach (var action in actions)
			{
				var typeMetadata = _typeSchema.GetTypeMetadata(action.EntityFullName);
				if(typeMetadata.IsCache)
					return true;
				if(ExistsCacheType(action.AfterActions) || ExistsCacheType(action.BeforeActions) || ExistsCacheType(action.AlternativeActions))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_cacheExecuter != null)
			{
				_cacheExecuter.Dispose();
				_cacheExecuter = null;
			}
			if(_databaseExecuter != null)
			{
				_databaseExecuter.Dispose();
				_databaseExecuter = null;
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
		/// Processa o resultado da execução.
		/// </summary>
		/// <param name="executeResult"></param>
		public PersistenceExecuteResultProcessResult Process(PersistenceExecuteResult executeResult)
		{
			var dynamicResult = executeResult as DynamicPersistenceExecuteResult;
			if(dynamicResult == null)
				return new PersistenceExecuteResultProcessResult(true, null);
			var actions = dynamicResult.Actions;
			var result = dynamicResult.ActionsResult;
			if(actions != null && actions.Length > 0 && result != null && result.Length > 0)
			{
				var actionsFixed = Colosoft.Data.Schema.PersistenceActionResultProcessor.FixActionsResults(actions, result).ToArray();
				var cacheResult = _cacheExecuter.Execute(actionsFixed, dynamicResult.ExcutionType);
				if(!cacheResult.Success)
				{
					var failAction = cacheResult.ActionsResult.FirstOrDefault(f => f != null && !f.Success);
					var failureMessage = failAction != null ? failAction.GetRecursiveFailureMessage().GetFormatter() : MessageFormattable.Empty;
					return new PersistenceExecuteResultProcessResult(false, failureMessage);
				}
				actionsFixed = Colosoft.Data.Schema.PersistenceActionResultProcessor.FixActionsResults(actionsFixed, cacheResult.ActionsResult).ToArray();
				_actionResultProcessor.Process(actionsFixed);
			}
			return new PersistenceExecuteResultProcessResult(true, null);
		}
	}
}
