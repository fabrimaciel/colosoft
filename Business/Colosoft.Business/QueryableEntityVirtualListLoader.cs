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

using Colosoft.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Business
{
	/// <summary>
	/// Implementação do loader para 
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	class QueryableEntityVirtualListLoader<TEntity> : Collections.VirtualListLoader<TEntity> where TEntity : class, IEntity
	{
		private IEntityTypeManager _entityTypeManager;

		private IEntityLoader _entityLoader;

		private Colosoft.Query.Queryable _queryable;

		private string _countExpression;

		private Func<object, TEntity> _castHandler;

		private string _uiContext;

		private bool _isLazy;

		/// <summary>
		/// Método usado para fazer o cast do tipo do resultado.
		/// </summary>
		public Func<object, TEntity> CastHandler
		{
			get
			{
				return _castHandler;
			}
			set
			{
				_castHandler = value;
			}
		}

		/// <summary>
		/// Cria a instancia com o suporte para estratégia de vinculação.
		/// </summary>
		/// <param name="entityTypeManager">Gerenciador dos tipos de entidades.</param>
		/// <param name="entityLoader">Loader da entidade.</param>
		/// <param name="queryable"></param>
		/// <param name="countExpression"></param>
		/// <param name="isLazy">Identifica se é para fazer uma carga tardia.</param>
		/// <param name="castHandler">Instancia do manipulador que vai realizar o cast do item do resultado.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		public QueryableEntityVirtualListLoader(IEntityTypeManager entityTypeManager, IEntityLoader entityLoader, Colosoft.Query.Queryable queryable, string countExpression, bool isLazy, Func<object, TEntity> castHandler, string uiContext)
		{
			_entityTypeManager = entityTypeManager;
			_entityLoader = entityLoader;
			_queryable = queryable;
			_countExpression = countExpression;
			_isLazy = isLazy;
			_uiContext = uiContext;
			_castHandler = castHandler;
		}

		/// <summary>
		/// Executa a primeira consulta para recuperar o quantidade de registro
		/// da lista.
		/// </summary>
		/// <param name="query">Consulta de deve ser executada.</param>
		/// <returns></returns>
		protected virtual int ExecuteCount(Colosoft.Query.Queryable query)
		{
			return query.Execute().First().GetInt32(0);
		}

		/// <summary>
		/// Método usado para recupera os dados para o preenchimento da lista virtual.
		/// </summary>
		/// <param name="virtualList"></param>
		/// <param name="startRow"></param>
		/// <param name="pageSize"></param>
		/// <param name="needItemsCount"></param>
		/// <param name="referenceObject"></param>
		/// <returns></returns>
		public override VirtualListLoaderResult<TEntity> Load(IObservableCollection virtualList, int startRow, int pageSize, bool needItemsCount, object referenceObject = null)
		{
			var countQuery = (Colosoft.Query.Queryable)_queryable.Clone();
			if(_countExpression != null)
				countQuery.Count(_countExpression);
			else
				countQuery.Count();
			countQuery.Sort = null;
			if(needItemsCount)
				return new VirtualListLoaderResult<TEntity>(null, ExecuteCount(countQuery));
			var sourceContext = _queryable.SourceContext;
			var queryable2 = ((Colosoft.Query.Queryable)_queryable.Clone()).Skip(startRow).Take(pageSize);
			IEnumerable<IEntity> result = null;
			if(_isLazy)
				result = _entityLoader.GetLazyEntities(queryable2.Execute(), sourceContext, _uiContext, _entityTypeManager);
			else
			{
				var preparedQuery = Colosoft.Business.EntityManager.Instance.Prepare<TEntity>(queryable2, sourceContext, _uiContext);
				result = _entityLoader.GetFullEntities(preparedQuery, _queryable.SourceContext);
			}
			if(CastHandler != null)
				result = result.Select(f => CastHandler(f));
			return new VirtualListLoaderResult<TEntity>(result.Select(f => (TEntity)f));
		}
	}
}
