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

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Implementação do executor de persistencia com a utilização
	/// do processador do resultado das ações.
	/// </summary>
	public class PersistenceExecuterResultProcessorWrapper : IPersistenceExecuter, IPersistenceExecuteResultProcessor
	{
		private PersistenceActionResultProcessor _actionResultProcessor;

		private IPersistenceExecuter _persistenceExecuter;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="persistenceExecuter"></param>
		/// <param name="actionResultProcessor"></param>
		public PersistenceExecuterResultProcessorWrapper(IPersistenceExecuter persistenceExecuter, PersistenceActionResultProcessor actionResultProcessor)
		{
			persistenceExecuter.Require("persistenceExecuter").NotNull();
			actionResultProcessor.Require("actionResultProcessor").NotNull();
			_persistenceExecuter = persistenceExecuter;
			_actionResultProcessor = actionResultProcessor;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~PersistenceExecuterResultProcessorWrapper()
		{
			Dispose(false);
		}

		/// <summary>
		/// Executa as ações de persistencia.
		/// </summary>
		/// <param name="actions">Ações que serão executadas.</param>
		/// <param name="executionType">Tipo de exeução.</param>
		/// <returns></returns>
		public PersistenceExecuteResult Execute(PersistenceAction[] actions, ExecutionType executionType)
		{
			var result = _persistenceExecuter.Execute(actions, executionType);
			if(result == null)
				return null;
			return new PersistenceExecuteResultWrapper(actions, executionType, result);
		}

		/// <summary>
		/// Processa o resultado da execução.
		/// </summary>
		/// <param name="executeResult"></param>
		/// <returns></returns>
		public PersistenceExecuteResultProcessResult Process(PersistenceExecuteResult executeResult)
		{
			PersistenceExecuteResultProcessResult processResult = null;
			if(_persistenceExecuter is IPersistenceExecuteResultProcessor)
				processResult = ((IPersistenceExecuteResultProcessor)_persistenceExecuter).Process(executeResult);
			var wrapperResult = executeResult as PersistenceExecuteResultWrapper;
			var actions = wrapperResult.Actions;
			var result = wrapperResult.ActionsResult;
			if(actions != null && actions.Length > 0 && result != null && result.Length > 0)
			{
				var actionsFixed = Colosoft.Data.Schema.PersistenceActionResultProcessor.FixActionsResults(actions, result).ToArray();
				_actionResultProcessor.Process(actionsFixed);
			}
			if(processResult == null)
				return new PersistenceExecuteResultProcessResult(true, null);
			return processResult;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_persistenceExecuter.Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Implementação do resultado da persistencia.
		/// </summary>
		class PersistenceExecuteResultWrapper : PersistenceExecuteResult
		{
			private PersistenceAction[] _actions;

			private PersistenceExecuteResult _executeResult;

			private ExecutionType _excutionType;

			/// <summary>
			/// Ação associadas.
			/// </summary>
			public PersistenceAction[] Actions
			{
				get
				{
					return _actions;
				}
			}

			/// <summary>
			/// Tipo de execução associada.
			/// </summary>
			public ExecutionType ExcutionType
			{
				get
				{
					return _excutionType;
				}
			}

			/// <summary>
			/// Resultado das ações.
			/// </summary>
			public override PersistenceActionResult[] ActionsResult
			{
				get
				{
					return _executeResult.ActionsResult;
				}
			}

			/// <summary>
			/// Identifica se a execução foi realizada com sucesso.
			/// </summary>
			public override bool Success
			{
				get
				{
					return _executeResult.Success;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="actions"></param>
			/// <param name="executionType"></param>
			/// <param name="executeResult"></param>
			public PersistenceExecuteResultWrapper(PersistenceAction[] actions, ExecutionType executionType, PersistenceExecuteResult executeResult)
			{
				_actions = actions;
				_excutionType = executionType;
				_executeResult = executeResult;
			}
		}
	}
}
