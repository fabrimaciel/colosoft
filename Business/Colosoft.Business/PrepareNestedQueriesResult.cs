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

namespace Colosoft.Business
{
	/// <summary>
	/// Armazena os dados da preparação da consultas aninhadas
	/// de uma determinada entidade.
	/// </summary>
	public class PrepareNestedQueriesResult
	{
		private Query.Queryable _queryable;

		private IEntityLoader _entityLoader;

		private string _uiContext;

		private IEntityTypeManager _entityTypeManager;

		private IList<EntityLoaderCreatorArgs> _parentResult;

		private Queue<Exception> _exceptions;

		/// <summary>
		/// Gerenciador dos tipos de entidade.
		/// </summary>
		public IEntityTypeManager EntityTypeManager
		{
			get
			{
				return _entityTypeManager;
			}
		}

		/// <summary>
		/// Contexto visual.
		/// </summary>
		public string UiContext
		{
			get
			{
				return _uiContext;
			}
		}

		/// <summary>
		/// Loader da entidade associada.
		/// </summary>
		public IEntityLoader EntityLoader
		{
			get
			{
				return _entityLoader;
			}
			internal set
			{
				_entityLoader = value;
			}
		}

		/// <summary>
		/// Consulta associada.
		/// </summary>
		public Query.Queryable Queryable
		{
			get
			{
				return _queryable;
			}
		}

		/// <summary>
		/// Resultados carregados para o pai da consulta.
		/// </summary>
		public IList<EntityLoaderCreatorArgs> ParentResult
		{
			get
			{
				return _parentResult;
			}
		}

		/// <summary>
		/// Erros ocorridos.
		/// </summary>
		public Queue<Exception> Exceptions
		{
			get
			{
				return _exceptions;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="queryable">Consulta associada.</param>
		/// <param name="entityLoader"></param>
		/// <param name="uiContext"></param>
		/// <param name="entityTypeManager"></param>
		/// <param name="parentResult"></param>
		/// <param name="exceptions"></param>
		public PrepareNestedQueriesResult(Query.Queryable queryable, IEntityLoader entityLoader, string uiContext, IEntityTypeManager entityTypeManager, IList<EntityLoaderCreatorArgs> parentResult, Queue<Exception> exceptions)
		{
			_queryable = queryable;
			_entityLoader = entityLoader;
			_uiContext = uiContext;
			_entityTypeManager = entityTypeManager;
			_parentResult = parentResult;
			_exceptions = exceptions;
		}
	}
}
