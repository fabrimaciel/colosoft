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
	/// Armazena o resultado da execução da persistencia.
	/// </summary>
	public class PersistenceExecuteResult
	{
		private PersistenceActionResult[] _actionsResult;

		/// <summary>
		/// Resultado das ações da persistencia.
		/// </summary>
		public virtual PersistenceActionResult[] ActionsResult
		{
			get
			{
				return _actionsResult;
			}
		}

		/// <summary>
		/// Identifica se o resulta está com sucesso.
		/// </summary>
		public virtual bool Success
		{
			get
			{
				return ActionsResult != null ? ActionsResult.Any(f => f == null || f.Success) : false;
			}
		}

		/// <summary>
		/// Construtor protegido.
		/// </summary>
		protected PersistenceExecuteResult()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="actionsResult"></param>
		public PersistenceExecuteResult(PersistenceActionResult[] actionsResult)
		{
			_actionsResult = actionsResult;
		}
	}
	/// <summary>
	/// Assinatura das classes responsáveis pela execução
	/// das ações de persistencia.
	/// </summary>
	public interface IPersistenceExecuter : IDisposable
	{
		/// <summary>
		/// Executa as ações informadas.
		/// </summary>
		/// <param name="actions">Instancia das ações que serão executadas.</param>
		/// <param name="executionType">Representa o tipo de execução da operação de persistência.</param>
		/// <returns>Resulta da execução das ações.</returns>
		PersistenceExecuteResult Execute(PersistenceAction[] actions, ExecutionType executionType);
	}
	/// <summary>
	/// Assinatura do classe do executor de persistencia com suporte a transação
	/// </summary>
	public interface IPersistenceExecuterTransactionSupport : IPersistenceExecuter
	{
		/// <summary>
		/// Executa as ações informadas.
		/// </summary>
		/// <param name="actions">Instancia das ações que serão executadas.</param>
		/// <param name="executionType">Representa o tipo de execução da operação de persistência.</param>
		/// <param name="transactionCreator">Método usado para cria a transação.</param>
		/// <returns>Resulta da execução das ações.</returns>
		PersistenceExecuteResult Execute(PersistenceAction[] actions, ExecutionType executionType, Func<IPersistenceTransactionExecuter> transactionCreator);
	}
	/// <summary>
	/// Armazena os dados do resultado do processamento do resultado
	/// a execução da persistencia.
	/// </summary>
	public class PersistenceExecuteResultProcessResult
	{
		private bool _success;

		private IMessageFormattable _message;

		/// <summary>
		/// Identifica se a operação foi realizada com sucesso.
		/// </summary>
		public bool Success
		{
			get
			{
				return _success;
			}
		}

		/// <summary>
		/// Mensagem associada.
		/// </summary>
		public IMessageFormattable Message
		{
			get
			{
				return _message;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="success"></param>
		/// <param name="message"></param>
		public PersistenceExecuteResultProcessResult(bool success, IMessageFormattable message)
		{
			_success = success;
			_message = message;
		}
	}
	/// <summary>
	/// Assinatura da classe responsável por processar 
	/// o resultado da execução de a persistencia.
	/// </summary>
	public interface IPersistenceExecuteResultProcessor
	{
		/// <summary>
		/// Processa o resultado da execução.
		/// </summary>
		/// <param name="executeResult"></param>
		PersistenceExecuteResultProcessResult Process(PersistenceExecuteResult executeResult);
	}
	/// <summary>
	/// Assinatura da classe responsável por criar um executor de persistencia.
	/// </summary>
	public interface IPersistenceExecuterFactory
	{
		/// <summary>
		/// Cria um executor.
		/// </summary>
		/// <returns></returns>
		IPersistenceExecuter CreateExecuter();

		/// <summary>
		/// Define o delegate que será utilizado para a criação do executer.
		/// </summary>
		Func<IPersistenceExecuter> ExecuterCreator
		{
			get;
			set;
		}
	}
}
