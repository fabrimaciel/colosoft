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
	/// Assinatura do validador das seções de persistencia.
	/// </summary>
	public interface IPersistenceSessionValidator
	{
		/// <summary>
		/// Realiza a validação da sessão de persistencia informada.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		PersistenceSessionValidateResult Validate(PersistenceSession session);
	}
	/// <summary>
	/// Implementação da uma agregador de validadores.
	/// </summary>
	public class AggregatePersistenceSessionValidator : IPersistenceSessionValidator
	{
		private List<IPersistenceSessionValidator> _validators = new List<IPersistenceSessionValidator>();

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="validators"></param>
		public AggregatePersistenceSessionValidator(IEnumerable<IPersistenceSessionValidator> validators)
		{
			_validators = new List<IPersistenceSessionValidator>(validators);
		}

		/// <summary>
		/// Realiza a validação da sessão de persistencia informada.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public PersistenceSessionValidateResult Validate(PersistenceSession session)
		{
			PersistenceSessionValidateResult result = null;
			foreach (var validator in _validators)
			{
				result = validator.Validate(session);
				if(!result.Success)
					break;
			}
			return result;
		}
	}
	/// <summary>
	/// Armazena o resultado da validação aplicação sobre a sessão de persistencia.
	/// </summary>
	public class PersistenceSessionValidateResult
	{
		private List<ActionValidateDetails> _errors = new List<ActionValidateDetails>();

		/// <summary>
		/// Relação dos erros ocorridos.
		/// </summary>
		public List<ActionValidateDetails> Errors
		{
			get
			{
				return _errors;
			}
		}

		/// <summary>
		/// Identifica se a validação foi realizada com sucesso.
		/// </summary>
		public bool Success
		{
			get
			{
				return Errors.Count == 0;
			}
		}

		/// <summary>
		/// Adiciona um erro para o resultado.
		/// </summary>
		/// <param name="action">Instancia da ação associada.</param>
		/// <param name="message">Mensagem do ocorrido.</param>
		public void AddError(PersistenceAction action, IMessageFormattable message)
		{
			this.Errors.Add(new ActionValidateDetails(action, message));
		}

		/// <summary>
		/// Detalhes da validação de uma ação.
		/// </summary>
		public class ActionValidateDetails
		{
			private PersistenceAction _action;

			private IMessageFormattable _message;

			/// <summary>
			/// Instancia da ação associada.
			/// </summary>
			public PersistenceAction Action
			{
				get
				{
					return _action;
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
			/// <param name="action">Instancia da ação associada.</param>
			/// <param name="message">Mensagem do ocorrido.</param>
			public ActionValidateDetails(PersistenceAction action, IMessageFormattable message)
			{
				_action = action;
				_message = message;
			}
		}
	}
}
