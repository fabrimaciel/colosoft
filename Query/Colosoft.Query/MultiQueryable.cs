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
using System.Runtime.Serialization;

namespace Colosoft.Query
{
	/// <summary>
	/// Representa um container de consulta.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public sealed class MultiQueryable : IEnumerable<Queryable>, ISerializable
	{
		private List<Queryable> _queries;

		private Dictionary<int, QueryCallBackWrapper> _callbacks;

		private IQueryDataSource _dataSource;

		private ISourceContext _sourceContext;

		/// <summary>
		/// Construtor padrão
		/// </summary>
		public MultiQueryable()
		{
			_callbacks = new Dictionary<int, QueryCallBackWrapper>();
			_queries = new List<Queryable>();
		}

		/// <summary>
		/// Construtor para serialização binária
		/// </summary>
		/// <param name="info">Informações de serialização</param>
		/// <param name="context">Contexto de serialização</param>
		private MultiQueryable(SerializationInfo info, StreamingContext context)
		{
			var Count = info.GetInt32("C");
			_queries = new List<Queryable>();
			for(int i = 0; i < Count; i++)
			{
				_queries.Add((Queryable)info.GetValue(i.ToString(), typeof(Queryable)));
			}
		}

		/// <summary>
		/// Número de queries no container
		/// </summary>
		public int Count
		{
			get
			{
				return _queries.Count;
			}
		}

		/// <summary>
		/// Instancia da fonte de dados associada.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public IQueryDataSource DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				_dataSource = value;
			}
		}

		/// <summary>
		/// Instancia do contexto de origem associado.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public ISourceContext SourceContext
		{
			get
			{
				return _sourceContext;
			}
			set
			{
				_sourceContext = value;
			}
		}

		/// <summary>
		/// Recupera o enumerador da lista
		/// </summary>
		/// <returns>Retorna o enumerador genérico"/></returns>
		public IEnumerator<Queryable> GetEnumerator()
		{
			return _queries.GetEnumerator();
		}

		/// <summary>
		/// Adiciona uma query
		/// </summary>
		/// <param name="query"><see cref="Queryable"/> a ser adicionado</param>
		public MultiQueryable Add(Queryable query)
		{
			_queries.Add(query);
			return this;
		}

		/// <summary>
		/// Adiciona uma query
		/// </summary>
		/// <param name="query"><see cref="Queryable"/> a ser adicionado</param>
		/// <param name="callback">Função de callback da consulta</param>
		public MultiQueryable Add(Queryable query, QueryCallBack callback)
		{
			_queries.Add(query);
			if(callback != null)
			{
				QueryCallBackWrapper wrapper = new QueryCallBackWrapper();
				wrapper.QueryCallBack = callback;
				_callbacks.Add(_queries.Count - 1, wrapper);
			}
			return this;
		}

		/// <summary>
		/// Adiciona uma query
		/// </summary>
		/// <param name="query"><see cref="Queryable"/> a ser adicionado</param>
		/// <param name="callBack">Função de callback da consulta</param>
		/// <param name="failedCallBack"></param>
		/// <returns></returns>
		public MultiQueryable Add(Queryable query, QueryCallBack callBack, QueryFailedCallBack failedCallBack)
		{
			_queries.Add(query);
			if(callBack != null)
			{
				QueryCallBackWrapper wrapper = new QueryCallBackWrapper();
				wrapper.QueryCallBack = callBack;
				wrapper.QueryFailedCallBack = failedCallBack;
				_callbacks.Add(_queries.Count - 1, wrapper);
			}
			return this;
		}

		/// <summary>
		/// Adiciona uma query
		/// </summary>
		/// <typeparam name="TModel">Tipo de retorno</typeparam>
		/// <param name="query"><see cref="Queryable"/> a ser adicionado</param>
		/// <param name="callback">Função de callback da consulta</param>
		public MultiQueryable Add<TModel>(Queryable query, QueryCallBack<TModel> callback)
		{
			return Add<TModel>(query, callback, null, null, null);
		}

		/// <summary>
		/// Adiciona uma query
		/// </summary>
		/// <typeparam name="TModel">Tipo de retorno</typeparam>
		/// <param name="query"><see cref="Queryable"/> a ser adicionado</param>
		/// <param name="callback">Função de callback da consulta</param>
		/// <param name="failedCallBack"></param>
		public MultiQueryable Add<TModel>(Queryable query, QueryCallBack<TModel> callback, QueryFailedCallBack failedCallBack)
		{
			return Add<TModel>(query, callback, failedCallBack, null, null);
		}

		/// <summary>
		/// Adiciona uma query
		/// </summary>
		/// <typeparam name="TModel">Tipo de retorno</typeparam>
		/// <param name="query"><see cref="Queryable"/> a ser adicionado</param>
		/// <param name="callback">Função de callback da consulta</param>
		/// <param name="bindStrategy">Estratégia de binding</param>
		public MultiQueryable Add<TModel>(Queryable query, QueryCallBack<TModel> callback, IQueryResultBindStrategy bindStrategy)
		{
			return Add<TModel>(query, callback, null, bindStrategy, null);
		}

		/// <summary>
		/// Adiciona uma query
		/// </summary>
		/// <typeparam name="TModel">Tipo de retorno</typeparam>
		/// <param name="query"><see cref="Queryable"/> a ser adicionado</param>
		/// <param name="callback">Função de callback da consulta</param>
		/// <param name="bindStrategy">Estratégia de binding</param>
		/// <param name="objectCreator">Criador de objetos</param>
		public MultiQueryable Add<TModel>(Queryable query, QueryCallBack<TModel> callback, IQueryResultBindStrategy bindStrategy, IQueryResultObjectCreator objectCreator)
		{
			return Add<TModel>(query, callback, null, bindStrategy, objectCreator);
		}

		/// <summary>
		/// Adiciona uma query
		/// </summary>
		/// <typeparam name="TModel">Tipo de retorno</typeparam>
		/// <param name="query"><see cref="Queryable"/> a ser adicionado</param>
		/// <param name="callback">Função de callback da consulta</param>
		/// <param name="failedCallBack"></param>
		/// <param name="bindStrategy">Estratégia de binding</param>
		/// <param name="objectCreator">Criador de objetos</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public MultiQueryable Add<TModel>(Queryable query, QueryCallBack<TModel> callback, QueryFailedCallBack failedCallBack, IQueryResultBindStrategy bindStrategy, IQueryResultObjectCreator objectCreator)
		{
			_queries.Add(query);
			if(bindStrategy == null && objectCreator == null)
			{
				var ts = TypeBindStrategyCache.GetItem(typeof(TModel), t => new QueryResultObjectCreator(t));
				objectCreator = ts;
				bindStrategy = ts;
			}
			if(bindStrategy == null)
				bindStrategy = new TypeBindStrategy(typeof(TModel), objectCreator);
			if(callback != null)
			{
				QueryCallBackWrapper<TModel> wrapper = new QueryCallBackWrapper<TModel>();
				wrapper.QueryCallBack = callback;
				wrapper.QueryFailedCallBack = failedCallBack;
				wrapper.BindStrategy = bindStrategy;
				wrapper.ObjectCreator = objectCreator;
				_callbacks.Add(_queries.Count - 1, wrapper);
			}
			return this;
		}

		/// <summary>
		/// Adiciona uma query
		/// </summary>
		/// <param name="query"><see cref="Queryable"/> a ser adicionado</param>
		/// <param name="callback">Função de callback da consulta</param>
		/// <param name="failedCallBack"></param>
		/// <param name="bindStrategy">Estratégia de binding</param>
		/// <param name="objectCreator">Criador de objetos</param>
		public MultiQueryable Add(Queryable query, BindableQueryCallBack callback, QueryFailedCallBack failedCallBack, IQueryResultBindStrategy bindStrategy, IQueryResultObjectCreator objectCreator)
		{
			bindStrategy.Require("bindStrategy").NotNull();
			objectCreator.Require("objectCreator").NotNull();
			_queries.Add(query);
			if(callback != null)
			{
				var wrapper = new BindableQueryCallBackWrapper();
				wrapper.QueryCallBack = callback;
				wrapper.QueryFailedCallBack = failedCallBack;
				wrapper.BindStrategy = bindStrategy;
				wrapper.ObjectCreator = objectCreator;
				_callbacks.Add(_queries.Count - 1, wrapper);
			}
			return this;
		}

		/// <summary>
		/// Adiciona uma query
		/// </summary>
		/// <param name="query"><see cref="Queryable"/> a ser adicionado</param>
		/// <param name="callback">Função de callback da consulta</param>
		/// <param name="bindStrategy">Estratégia de binding</param>
		/// <param name="objectCreator">Criador de objetos</param>
		public MultiQueryable Add(Queryable query, BindableQueryCallBack callback, IQueryResultBindStrategy bindStrategy, IQueryResultObjectCreator objectCreator)
		{
			bindStrategy.Require("bindStrategy").NotNull();
			objectCreator.Require("objectCreator").NotNull();
			_queries.Add(query);
			if(callback != null)
			{
				var wrapper = new BindableQueryCallBackWrapper();
				wrapper.QueryCallBack = callback;
				wrapper.BindStrategy = bindStrategy;
				wrapper.ObjectCreator = objectCreator;
				_callbacks.Add(_queries.Count - 1, wrapper);
			}
			return this;
		}

		/// <summary>
		/// Remove a query
		/// </summary>
		/// <param name="query"><see cref="Queryable"/> a se remover</param>
		public MultiQueryable Remove(Queryable query)
		{
			_callbacks.Remove(_queries.IndexOf(query));
			_queries.Remove(query);
			return this;
		}

		/// <summary>
		/// Remove a query baseada no index
		/// </summary>
		/// <param name="index">Índice no qual remover</param>
		public MultiQueryable RemoveAt(int index)
		{
			_callbacks.Remove(index);
			_queries.RemoveAt(index);
			return this;
		}

		/// <summary>
		/// Recupera a query baseado no indice
		/// </summary>
		/// <param name="index">Índice do <see cref="Queryable"/></param>
		/// <returns>A query a ser retornada</returns>
		public Queryable this[int index]
		{
			get
			{
				return _queries[index];
			}
			set
			{
				_queries[index] = value;
			}
		}

		/// <summary>
		/// Executa a consulta.
		/// </summary>
		/// <returns>Resultado na forma de um vetor de <see cref="QueryResult"/></returns>
		public IEnumerable<IQueryResult> Execute()
		{
			_dataSource.Require("_dataSource").NotNull();
			QueryInfo[] info = CreateQueryInfos();
			IEnumerable<IQueryResult> result = null;
			try
			{
				result = _dataSource.Execute(info);
			}
			catch(Exception ex)
			{
				for(int i = 0; i < _queries.Count; i++)
				{
					QueryCallBackWrapper callback;
					if(_callbacks.TryGetValue(i, out callback))
						callback.ExecuteFailedCallBack(this, info[i], new QueryFailedInfo(QueryFailedReason.Error, ex.Message.GetFormatter(), ex));
				}
				throw;
			}
			for(int i = 0; i < _queries.Count; i++)
				_queries[i].Parameters = info[i].Parameters;
			if(_callbacks.Where(f => f.Value != null).Any())
			{
				var result2 = new List<IQueryResult>();
				try
				{
					var position = 0;
					var processed = 0;
					using (var resultEnumerator = result.GetEnumerator())
					{
						for(processed = 0; processed < _queries.Count && resultEnumerator.MoveNext(); processed++)
						{
							for(position = 0; position < info.Length; position++)
							{
								if(info[position].Id == resultEnumerator.Current.Id)
									break;
							}
							QueryCallBackWrapper callback;
							if(_callbacks.TryGetValue(position, out callback))
							{
								var queryResult = resultEnumerator.Current;
								callback.ExecuteCallBack(this, info[position], queryResult);
								result2.Add(new QueryResult());
							}
							else
							{
								using (var recordEnumerator = resultEnumerator.Current.GetEnumerator())
								{
									if(recordEnumerator.MoveNext())
									{
										var records = new List<Record>();
										records.Add(recordEnumerator.Current);
										while (recordEnumerator.MoveNext())
											records.Add(recordEnumerator.Current);
										result2.Add(new QueryResult(records[0].Descriptor, records, _queries[position]));
									}
									else
										result2.Add(new QueryResult());
								}
							}
						}
						if(processed < _queries.Count)
							throw new QueryException(ResourceMessageFormatter.Create(() => Properties.Resources.MultiQueryable_MultiQueryableExecuteErrorProcess).Format());
					}
				}
				finally
				{
					if(result is IDisposable)
						if(!(result is IDisposableState) || !((IDisposableState)result).IsDisposed)
							((IDisposable)result).Dispose();
				}
				result = result2;
			}
			return result;
		}

		/// <summary>
		/// Executa a consulta.
		/// </summary>
		/// <returns>Resultado na forma de um vetor de <see cref="QueryResult"/></returns>
		public IEnumerable<IQueryResult> Execute(IQueryDataSource dataSource)
		{
			dataSource.Require("dataSource").NotNull();
			return dataSource.Execute(CreateQueryInfos());
		}

		/// <summary>
		/// Obtém dados para serialização binária
		/// </summary>
		/// <param name="info">Informações de serialização</param>
		/// <param name="context">Contexto de serialização</param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("C", _queries.Count);
			for(int i = 0; i < _queries.Count; i++)
			{
				info.AddValue(i.ToString(), _queries[i]);
			}
		}

		/// <summary>
		/// Retorna o enumerador para coleções antigas
		/// </summary>
		/// <returns>Retorna <see cref="System.Collections.IEnumerator"/></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _queries.GetEnumerator();
		}

		/// <summary>
		/// Cria vetor de <see cref="QueryInfo"/> baseado no objeto <see cref="MultiQueryable"/>
		/// </summary>
		/// <returns>Retorna vetor de <see cref="QueryInfo"/></returns>
		private QueryInfo[] CreateQueryInfos()
		{
			QueryInfo[] queryInfos = new QueryInfo[_queries.Count];
			for(int i = 0; i < _queries.Count; i++)
			{
				queryInfos[i] = _queries[i].CreateQueryInfo();
			}
			return queryInfos;
		}
	}
}
