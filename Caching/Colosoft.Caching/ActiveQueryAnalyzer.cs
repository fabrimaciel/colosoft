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
using System.Collections;
using Colosoft.Caching.Local;
using Colosoft.Caching.Queries.Filters;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Representa o analizador da consulta ativa.
	/// </summary>
	public class ActiveQueryAnalyzer : IQueryOperationsObserver
	{
		private ICacheEventsListener _queryChangesListener;

		private Dictionary<string, ActiveQueryEvaluationIndex> _typeSpecificEvalIndexes;

		private Dictionary<string, Dictionary<string, List<PredicateHolder>>> _typeSpecificPredicates;

		private Dictionary<string, List<PredicateHolder>> _typeSpecificRegisteredPredicates;

		/// <summary>
		/// Hash com os indices dos tipo especifico.
		/// </summary>
		internal Dictionary<string, ActiveQueryEvaluationIndex> TypeSpecificEvalIndexes
		{
			get
			{
				return _typeSpecificEvalIndexes;
			}
			set
			{
				_typeSpecificEvalIndexes = value;
			}
		}

		/// <summary>
		/// Hash com os predicados do tipo especifico.
		/// </summary>
		internal Dictionary<string, Dictionary<string, List<PredicateHolder>>> TypeSpecificPredicates
		{
			get
			{
				return _typeSpecificPredicates;
			}
			set
			{
				_typeSpecificPredicates = value;
			}
		}

		/// <summary>
		/// Hash com os predicados registrados para o tipo espeficio.
		/// </summary>
		internal Dictionary<string, List<PredicateHolder>> TypeSpecificRegisteredPredicates
		{
			get
			{
				return _typeSpecificRegisteredPredicates;
			}
			set
			{
				_typeSpecificRegisteredPredicates = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="queryChangeListener">Instancia que irá escutar os eventos do cache.</param>
		/// <param name="indexedTypes">Hahs com os tipos indexados.</param>
		/// <param name="cacheContext">Nome do contexto.</param>
		internal ActiveQueryAnalyzer(ICacheEventsListener queryChangeListener, IDictionary indexedTypes, string cacheContext)
		{
			_queryChangesListener = queryChangeListener;
			_typeSpecificPredicates = new Dictionary<string, Dictionary<string, List<PredicateHolder>>>();
			_typeSpecificRegisteredPredicates = new Dictionary<string, List<PredicateHolder>>();
			_typeSpecificEvalIndexes = new Dictionary<string, ActiveQueryEvaluationIndex>();
			if(indexedTypes.Contains("indexes"))
				this.InitializeEvalIndexes(indexedTypes["indexes"] as IDictionary, cacheContext);
		}

		/// <summary>
		/// Método acionado quando um noto item é adicionado.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="cacheEntry">Instancia da entrada.</param>
		/// <param name="cache">Instancia do cache.</param>
		/// <param name="cacheContext">Nome do contexto.</param>
		/// <param name="notify">True para notificar</param>
		void IQueryOperationsObserver.OnItemAdded(object key, CacheEntry cacheEntry, LocalCacheBase cache, string cacheContext, bool notify)
		{
		}

		/// <summary>
		/// Método acionado quando o item for removido.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="cacheEntry">Instancia da entrada.</param>
		/// <param name="cache">Instancia do cache.</param>
		/// <param name="cacheContext">Nome do contexto.</param>
		/// <param name="notify">True para notificar</param>
		void IQueryOperationsObserver.OnItemRemoved(object key, CacheEntry cacheEntry, LocalCacheBase cache, string cacheContext, bool notify)
		{
		}

		/// <summary>
		/// Método acionado quando um item é atualizado.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="cacheEntry">Instancia da entrada.</param>
		/// <param name="cache">Instancia do cache.</param>
		/// <param name="cacheContext">Nome do contexto.</param>
		/// <param name="notify">True para notificar</param>
		void IQueryOperationsObserver.OnItemUpdated(object key, CacheEntry cacheEntry, LocalCacheBase cache, string cacheContext, bool notify)
		{
		}

		/// <summary>
		/// Inicializa os indices calculados.
		/// </summary>
		/// <param name="indexedTypes"></param>
		/// <param name="cacheContext"></param>
		private void InitializeEvalIndexes(IDictionary indexedTypes, string cacheContext)
		{
			if((indexedTypes != null) && indexedTypes.Contains("index-classes"))
			{
				IDictionaryEnumerator enumerator = (indexedTypes["index-classes"] as Hashtable).GetEnumerator();
				while (enumerator.MoveNext())
				{
					Hashtable hashtable2 = enumerator.Value as Hashtable;
					string type = "";
					if(hashtable2 != null)
					{
						type = (string)hashtable2["id"];
						var attribList = new List<string>();
						IDictionaryEnumerator enumerator2 = hashtable2.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							Hashtable hashtable3 = enumerator2.Value as Hashtable;
							if(hashtable3 != null)
							{
								IDictionaryEnumerator enumerator3 = hashtable3.GetEnumerator();
								while (enumerator3.MoveNext())
								{
									Hashtable hashtable4 = enumerator3.Value as Hashtable;
									if(hashtable4 != null)
										attribList.Add(hashtable4["id"] as string);
								}
							}
						}
						if(attribList.Count > 0)
						{
							ActiveQueryEvaluationIndex index = new ActiveQueryEvaluationIndex(attribList, cacheContext);
							this.TypeSpecificEvalIndexes[type] = index;
						}
					}
				}
			}
		}

		/// <summary>
		/// Limpa os dados registra na instancia.
		/// </summary>
		public void Clear()
		{
			if(_typeSpecificRegisteredPredicates != null)
				_typeSpecificRegisteredPredicates.Clear();
			if(_typeSpecificPredicates != null)
				_typeSpecificPredicates.Clear();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			this.Clear();
			if(_typeSpecificRegisteredPredicates != null)
				_typeSpecificRegisteredPredicates = null;
			if(_typeSpecificPredicates != null)
				_typeSpecificPredicates = null;
			if(_typeSpecificEvalIndexes != null)
			{
				_typeSpecificEvalIndexes.Clear();
				_typeSpecificEvalIndexes = null;
			}
		}
	}
}
