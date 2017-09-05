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

namespace Colosoft.Data
{
	/// <summary>
	/// Possíveis situações do resultado da execução
	/// das a~ções de uma <see cref="IPersistenceSession"/>.
	/// </summary>
	public enum ExecuteActionsResultStatus
	{
		/// <summary>
		/// Todas ações foram executadas com sucesso.
		/// </summary>
		Success,
		/// <summary>
		/// Ocorreu alguma falha na comunicação.
		/// </summary>
		ErrorOnComunication,
		/// <summary>
		/// Ocorreu algum erro ao executar as ações.
		/// </summary>
		Fail
	}
	/// <summary>
	/// Classe que armazena o resulta da execução das ações
	/// executadas sobre uma <see cref="IPersistenceSession"/>.
	/// </summary>
	public class ExecuteActionsResult
	{
		/// <summary>
		/// Representa uma ação dentro do resultado.
		/// </summary>
		public class ExecuteAction
		{
			private ExecuteAction[] _beforeActions;

			private ExecuteAction[] _afterActions;

			private ExecuteAction[] _alternativeActions;

			private PersistenceAction _action;

			private PersistenceActionResult _result;

			/// <summary>
			/// Relação das ações executadas antes.
			/// </summary>
			public ExecuteAction[] BeforeActions
			{
				get
				{
					return _beforeActions ?? new ExecuteAction[0];
				}
			}

			/// <summary>
			/// Relação das ações executas posteriormente.
			/// </summary>
			public ExecuteAction[] AfterActions
			{
				get
				{
					return _afterActions ?? new ExecuteAction[0];
				}
			}

			/// <summary>
			/// Relação das ações alternativas.
			/// </summary>
			public ExecuteAction[] AlternativeActions
			{
				get
				{
					return _alternativeActions ?? new ExecuteAction[0];
				}
			}

			/// <summary>
			/// Ação executada.
			/// </summary>
			public PersistenceAction Action
			{
				get
				{
					return _action;
				}
			}

			/// <summary>
			/// Resultado da execução.
			/// </summary>
			public PersistenceActionResult Result
			{
				get
				{
					return _result;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="action">Instancia da ação.</param>
			/// <param name="result">Resultado.</param>
			/// <param name="beforeActions">Ações executadas anteriormente.</param>
			/// <param name="afterActions">Ações executadas posteriomente.</param>
			/// <param name="alternativeActions">Ações alternativas executadas.</param>
			public ExecuteAction(PersistenceAction action, PersistenceActionResult result, ExecuteAction[] beforeActions, ExecuteAction[] afterActions, ExecuteAction[] alternativeActions)
			{
				action.Require("action").NotNull();
				_action = action;
				_result = result;
				_beforeActions = beforeActions;
				_afterActions = afterActions;
				_alternativeActions = alternativeActions;
			}
		}

		private string _failureMessage;

		/// <summary>
		/// Situação da execução das ações.
		/// </summary>
		public ExecuteActionsResultStatus Status
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera a mensagem de falha do resultado, caso exista.
		/// </summary>
		public string FailureMessage
		{
			get
			{
				return _failureMessage;
			}
			set
			{
				_failureMessage = value;
			}
		}

		/// <summary>
		/// Ações execuadas.
		/// </summary>
		public ExecuteAction[] Actions
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="actions">Ações do resultado.</param>
		/// <param name="status"></param>
		/// <param name="failureMessage"></param>
		public ExecuteActionsResult(ExecuteAction[] actions, ExecuteActionsResultStatus status = ExecuteActionsResultStatus.Success, string failureMessage = null)
		{
			this.Actions = actions;
			this.Status = status;
			_failureMessage = failureMessage;
		}
	}
}
