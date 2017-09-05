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
	/// Implementação do resultado da persistencia.
	/// </summary>
	class DynamicPersistenceExecuteResult : PersistenceExecuteResult
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
		/// Construtor padrãi.
		/// </summary>
		/// <param name="actions"></param>
		/// <param name="executionType"></param>
		/// <param name="executeResult"></param>
		public DynamicPersistenceExecuteResult(PersistenceAction[] actions, ExecutionType executionType, PersistenceExecuteResult executeResult)
		{
			_actions = actions;
			_excutionType = executionType;
			_executeResult = executeResult;
		}
	}
}
