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

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Possíveis estados da execução do build.
	/// </summary>
	public enum BuildExecutionState
	{
		/// <summary>
		/// Identifica se o serviço está ocupado para atender a requisição.
		/// </summary>
		Busy,
		/// <summary>
		/// Identifica que a execução foi adicionada na filha para ser processada.
		/// </summary>
		Queued,
		/// <summary>
		/// Identifica se a execução 
		/// </summary>
		Running,
		/// <summary>
		/// Identifica que já foi finalizada.
		/// </summary>
		Finalized,
		/// <summary>
		/// Identifica que a execução não existe.
		/// </summary>
		NoExists,
		/// <summary>
		/// Identifica que a execução foi abortada.
		/// </summary>
		Aborted
	}
	/// <summary>
	/// Armazena o resultado da execução de um build.
	/// </summary>
	public class BuildExecutionResult
	{
		/// <summary>
		/// Identificador da execução.
		/// </summary>
		public Guid Uid
		{
			get;
			set;
		}

		/// <summary>
		/// Estado da execução.
		/// </summary>
		public BuildExecutionState State
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem associada.
		/// </summary>
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Progresso total da execução.
		/// </summary>
		public int TotalProgress
		{
			get;
			set;
		}

		/// <summary>
		/// Progresso atual da execução.
		/// </summary>
		public int CurrentProgress
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem de erro caso tenha ocorrido.
		/// </summary>
		public string ErrorMessage
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public BuildExecutionResult()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="uid">Identificador da operação.</param>
		/// <param name="state">Estado da execução.</param>
		/// <param name="message"></param>
		public BuildExecutionResult(Guid uid, BuildExecutionState state, string message)
		{
			this.Uid = uid;
			this.State = state;
			this.Message = message;
		}
	}
}
