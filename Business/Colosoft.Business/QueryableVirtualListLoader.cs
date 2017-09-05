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
using Colosoft.Collections;

namespace Colosoft.Business
{
	/// <summary>
	/// Implementação do loader para resultado de consulta que serão usados em listas virtuais.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class QueryableVirtualListLoader<T> : Collections.VirtualListLoader<T>
	{
		private Colosoft.Query.Queryable _queryable;

		private string _countExpression;

		private Colosoft.Query.IQueryResultBindStrategy _bindStrategy;

		private Colosoft.Query.IQueryResultObjectCreator _objectCreator;

		private Func<object, T> _castHandler;

		/// <summary>
		/// Método usado para fazer o cast do tipo do resultado.
		/// </summary>
		public Func<object, T> CastHandler
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
		/// Construtor padrão.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="countExpression"></param>
		public QueryableVirtualListLoader(Colosoft.Query.Queryable queryable, string countExpression)
		{
			_queryable = queryable;
			_countExpression = countExpression;
		}

		/// <summary>
		/// Cria a instancia com o suporte para estratégia de vinculação.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="countExpression"></param>
		/// <param name="bindStrategy"></param>
		/// <param name="objectCreator"></param>
		/// <param name="castHandler">Instancia do manipulador que vai realizar o cast do item do resultado.</param>
		public QueryableVirtualListLoader(Colosoft.Query.Queryable queryable, string countExpression, Colosoft.Query.IQueryResultBindStrategy bindStrategy, Colosoft.Query.IQueryResultObjectCreator objectCreator, Func<object, T> castHandler) : this(queryable, countExpression)
		{
			_bindStrategy = bindStrategy;
			_objectCreator = objectCreator;
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
			using (var enumerator = query.Execute().GetEnumerator())
			{
				if(enumerator.MoveNext())
					return enumerator.Current.GetInt32(0);
				return 0;
			}
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
		public override VirtualListLoaderResult<T> Load(IObservableCollection virtualList, int startRow, int pageSize, bool needItemsCount, object referenceObject = null)
		{
			var countQuery = (Colosoft.Query.Queryable)_queryable.Clone();
			countQuery.NestedQueries.Clear();
			if(_countExpression != null)
				countQuery = countQuery.Count(_countExpression);
			else
				countQuery = countQuery.Count();
			countQuery.Sort = null;
			if(needItemsCount)
				return new VirtualListLoaderResult<T>(null, ExecuteCount(countQuery));
			var multiQuery = _queryable.SourceContext.CreateMultiQuery();
			IEnumerable<T> result = null;
			var queryable2 = ((Colosoft.Query.Queryable)_queryable.Clone()).Skip(startRow).Take(pageSize);
			if(_bindStrategy == null && _objectCreator == null)
				multiQuery.Add<T>(queryable2, (mq, q, r) =>  {
					var items = new List<T>();
					foreach (var i in r)
					{
						if(i is IConnectedEntity)
							((IConnectedEntity)i).Connect(mq.SourceContext);
						items.Add(i);
					}
					result = items;
				});
			else
				multiQuery.Add(queryable2, (mq, q, r) =>  {
					var items = new List<T>();
					if(CastHandler == null)
						foreach (var item in r)
						{
							if(item is IConnectedEntity)
								((IConnectedEntity)item).Connect(mq.SourceContext);
							items.Add((T)item);
						}
					else
						foreach (var item in r)
						{
							if(item is IConnectedEntity)
								((IConnectedEntity)item).Connect(mq.SourceContext);
							items.Add(CastHandler(item));
						}
					result = items;
				}, _bindStrategy, _objectCreator);
			multiQuery.Execute();
			return new VirtualListLoaderResult<T>(result);
		}
	}
}
