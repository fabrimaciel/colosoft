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
using Colosoft.Threading;
using System.Collections;
using Colosoft.Caching.Local;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Classe responsável por gerenciar os indices de pesquisa.
	/// </summary>
	internal class QueryIndexManager
	{
		private AsyncProcessor _asyncProcessor;

		private IndexedLocalCache _cache;

		protected string _cacheName;

		private bool _indexForAll;

		private IDictionary _props;

		private Dictionary<string, IQueryIndex> _indexMap = new Dictionary<string, IQueryIndex>();

		private TypeInfoMap _typeMap;

		/// <summary>
		/// Recupera o mapa dos indices.
		/// </summary>
		protected Dictionary<string, IQueryIndex> IndexMapInternal
		{
			get
			{
				return _indexMap;
			}
			set
			{
				_indexMap = value;
			}
		}

		/// <summary>
		/// Instancia do processador assincrono.
		/// </summary>
		public AsyncProcessor AsyncProcessor
		{
			get
			{
				return _asyncProcessor;
			}
		}

		/// <summary>
		/// Identifica se o indice é para todos.
		/// </summary>
		public bool IndexForAll
		{
			get
			{
				return _indexForAll;
			}
		}

		/// <summary>
		/// Mapa dos indicies.
		/// </summary>
		public Dictionary<string, IQueryIndex> IndexMap
		{
			get
			{
				return _indexMap;
			}
		}

		/// <summary>
		/// Mapa das informações dos tipos.
		/// </summary>
		public TypeInfoMap TypeInfoMap
		{
			get
			{
				return _typeMap;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="props">Propriedades do gerenciador.</param>
		/// <param name="cache">Instancia do cache que será usada.</param>
		/// <param name="cacheName">Nome do cache.</param>
		public QueryIndexManager(IDictionary props, IndexedLocalCache cache, string cacheName)
		{
			_cache = cache;
			_props = props;
			_cacheName = cacheName;
		}

		/// <summary>
		/// Método acioando quando um novo manipulador for adicionado.
		/// </summary>
		/// <param name="handleId"></param>
		private void TypeMap_HandleAdded(int handleId)
		{
			var type = _typeMap.GetTypeName(handleId);
			var attributes = _typeMap.GetAttribList(handleId);
			if(attributes.Count > 0)
				_indexMap[type] = new AttributeIndex(attributes, _cacheName);
			else
				_indexMap[type] = new TypeIndex(type, _indexForAll);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <returns></returns>
		internal virtual bool Initialize()
		{
			bool flag = false;
			if(_props != null)
			{
				if(_props.Contains("index-for-all"))
				{
					_indexForAll = Convert.ToBoolean(_props["index-for-all"]);
					flag = _indexForAll;
				}
				if(_props.Contains("index-classes"))
				{
					Hashtable indexClasses = _props["index-classes"] as Hashtable;
					_typeMap = new TypeInfoMap(indexClasses);
					IDictionaryEnumerator enumerator = indexClasses.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Hashtable hashtable2 = enumerator.Value as Hashtable;
						string type = "";
						if(hashtable2 != null)
						{
							type = (string)hashtable2["id"];
							var attributes = new List<string>();
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
											attributes.Add(hashtable4["id"] as string);
									}
								}
							}
							if(attributes.Count > 0)
								_indexMap[type] = new AttributeIndex(attributes, _cacheName);
							else
								_indexMap[type] = new TypeIndex(type, _indexForAll);
							flag = true;
						}
					}
				}
				else
					_typeMap = new TypeInfoMap(new Hashtable());
			}
			else
			{
				_indexMap["default"] = new VirtualQueryIndex(_cache);
				_typeMap = new TypeInfoMap(new Hashtable());
			}
			_typeMap.HandleAdded += new Action<int>(TypeMap_HandleAdded);
			if(flag)
			{
				_asyncProcessor = new AsyncProcessor("Cache.QueryIndexManager", _cache.Context.Logger);
				_asyncProcessor.Start();
			}
			return flag;
		}

		/// <summary>
		/// Adiciona uma chave e um valor para o indice de forma assincrona.
		/// </summary>
		/// <param name="key">Chave que será adicionada.</param>
		/// <param name="value">Valor que será adicionado.</param>
		public void AsyncAddToIndex(object key, CacheEntry value)
		{
			lock (_asyncProcessor)
				_asyncProcessor.Enqueue(new IndexAddTask(this, key, value));
		}

		/// <summary>
		/// Remove a chave e o valor do indice de forma assincrona.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void AsyncRemoveFromIndex(object key, CacheEntry value)
		{
			lock (_asyncProcessor)
				_asyncProcessor.Enqueue(new IndexRemoveTask(this, key, value));
		}

		/// <summary>
		/// Adiciona uma chave e um valor para o indice.
		/// </summary>
		/// <param name="key">Chave que será adicionada.</param>
		/// <param name="entry">Instancia da entrada do cache</param>
		public virtual void AddToIndex(object key, CacheEntry entry)
		{
			Hashtable queryInfo = entry.QueryInfo["query-info"] as Hashtable;
			if(queryInfo != null)
			{
				lock (IndexMapInternal)
				{
					IDictionaryEnumerator enumerator = queryInfo.GetEnumerator();
					while (enumerator.MoveNext())
					{
						int handle = (int)enumerator.Key;
						string typeName = _typeMap.GetTypeName(handle);
						if(IndexMapInternal.ContainsKey(typeName))
						{
							var indexValues = new Hashtable();
							var attributeValues = new Hashtable();
							var list = (ArrayList)enumerator.Value;
							var attribList = _typeMap.GetAttribList(handle);
							if(list.Count != attribList.Count)
								throw new InvalidOperationException("Attributes count not equals value list count");
							for(int i = 0; i < attribList.Count; i++)
							{
								string attributeName = attribList[i];
								string attributeTypeName = _typeMap.GetAttributes(handle)[attributeName] as string;
								Type conversionType = Type.GetType(attributeTypeName, true, true);
								object value = list[i];
								if(value != null)
								{
									try
									{
										if(conversionType == typeof(DateTime) && !(value is DateTime))
										{
											if(value is DateTimeOffset)
												value = ((DateTimeOffset)value).DateTime;
											else
												value = new DateTime(Convert.ToInt64(value));
										}
										else if(conversionType == typeof(Guid) && (value is string))
											value = Guid.Parse((string)value);
										else
											value = Convert.ChangeType(value, conversionType);
									}
									catch(Exception)
									{
										throw new FormatException(string.Format("Cannot convert ({0})'{1}' to {2}", (list[i] ?? new object()).GetType().FullName, list[i], conversionType.ToString()));
									}
								}
								if((value != null) && (value is string))
									indexValues.Add(attributeName, ((string)value).ToLower());
								else
									indexValues.Add(attributeName, value);
								attributeValues.Add(attributeName, value);
							}
							entry.MetaInformation = new MetaInformation(attributeValues);
							entry.MetaInformation.CacheKey = key as string;
							entry.MetaInformation.Type = _typeMap.GetTypeName(handle);
							((IQueryIndex)IndexMapInternal[typeName]).AddToIndex(key, indexValues);
						}
					}
				}
			}
		}

		/// <summary>
		/// Remove a chave e o valor do indice.
		/// </summary>
		/// <param name="key">Chave que será removida.</param>
		/// <param name="queryInfo">Informações da consulta que serão removidas.</param>
		public virtual void RemoveFromIndex(object key, Hashtable queryInfo)
		{
			if(queryInfo != null)
			{
				lock (IndexMapInternal)
				{
					IDictionaryEnumerator enumerator = ((Hashtable)queryInfo).GetEnumerator();
					while (enumerator.MoveNext())
					{
						int handle = (int)enumerator.Key;
						string typeName = _typeMap.GetTypeName(handle);
						if(IndexMapInternal.ContainsKey(typeName))
						{
							Hashtable indexValues = new Hashtable();
							var list = (ArrayList)enumerator.Value;
							var attribList = _typeMap.GetAttribList(handle);
							if(list.Count != attribList.Count)
								throw new InvalidOperationException("Attributes count not equals value list count");
							for(int i = 0; i < attribList.Count; i++)
							{
								var attributes = _typeMap.GetAttributes(handle);
								var attributeName = attribList[i];
								string attributeType = attributes[attributeName] as string;
								Type conversionType = Type.GetType(attributeType, true, true);
								object value = list[i];
								if(value != null)
								{
									try
									{
										if(conversionType == typeof(DateTime) && !(value is DateTime))
										{
											if(value is DateTimeOffset)
												value = ((DateTimeOffset)value).DateTime;
											else
												value = new DateTime(Convert.ToInt64(value));
										}
										else if(conversionType == typeof(Guid) && (value is string))
											value = Guid.Parse((string)value);
										else
											value = Convert.ChangeType(value, conversionType);
									}
									catch(Exception)
									{
										throw new FormatException(string.Format("Cannot convert ({0})'{1}' to {2}", (list[i] ?? new object()).GetType().FullName, list[i], conversionType.ToString()));
									}
								}
								if((value != null) && (value is string))
									indexValues.Add(attributeName, ((string)value).ToLower());
								else
									indexValues.Add(attributeName, value);
							}
							((IQueryIndex)IndexMapInternal[typeName]).RemoveFromIndex(key, indexValues);
						}
					}
				}
			}
		}

		/// <summary>
		/// Limpa os indices gerenciados.
		/// </summary>
		public void Clear()
		{
			if(IndexMapInternal != null)
			{
				lock (IndexMapInternal)
				{
					IDictionaryEnumerator enumerator = IndexMapInternal.GetEnumerator();
					while (enumerator.MoveNext())
						(enumerator.Value as IQueryIndex).Clear();
				}
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_typeMap != null)
				_typeMap.HandleAdded -= new Action<int>(TypeMap_HandleAdded);
			if(_asyncProcessor != null)
			{
				_asyncProcessor.Stop();
				_asyncProcessor = null;
			}
			if(IndexMapInternal != null)
			{
				IndexMapInternal.Clear();
				IndexMapInternal = null;
			}
			_cache = null;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.Collect();
		}

		/// <summary>
		/// Classe que representa a tarefa assincrona para adicionar uma 
		/// entrada no indice.
		/// </summary>
		private class IndexAddTask : IAsyncTask
		{
			private CacheEntry _entry;

			private QueryIndexManager _indexManager;

			private object _key;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="indexManager">Instancia do gerenciador.</param>
			/// <param name="key">Chave do item.</param>
			/// <param name="value">Valor que será adicionado.</param>
			public IndexAddTask(QueryIndexManager indexManager, object key, CacheEntry value)
			{
				_key = key;
				_entry = value;
				_indexManager = indexManager;
			}

			void IAsyncTask.Process()
			{
				_indexManager.AddToIndex(_key, _entry);
			}
		}

		/// <summary>
		/// Classe que representa a tarefa assincrona para remover uma 
		/// entrada do indice.
		/// </summary>
		private class IndexRemoveTask : IAsyncTask
		{
			private CacheEntry _entry;

			private QueryIndexManager _indexManager;

			private object _key;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="indexManager">Instancia do gerenciador.</param>
			/// <param name="key">Chave do item.</param>
			/// <param name="value">Valor que será adicionado.</param>
			public IndexRemoveTask(QueryIndexManager indexManager, object key, CacheEntry value)
			{
				_key = key;
				_entry = value;
				_indexManager = indexManager;
			}

			void IAsyncTask.Process()
			{
				_indexManager.RemoveFromIndex(_key, _entry.QueryInfo);
			}
		}
	}
}
