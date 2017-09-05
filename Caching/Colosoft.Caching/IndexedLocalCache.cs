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
using Colosoft.Caching.Queries;
using Colosoft.Caching.Queries.Filters;
using System.Collections;
using Colosoft.Caching.Data;
using Colosoft.Caching.Expiration;
using Colosoft.Caching.Exceptions;
using Colosoft.Caching.Util;

namespace Colosoft.Caching.Local
{
	/// <summary>
	/// Implementação de um cache local indexado.
	/// </summary>
	internal class IndexedLocalCache : LocalCache
	{
		private EnumerationIndex _enumerationIndex;

		private GroupIndexManager _grpIndexManager;

		private QueryIndexManager _queryIndexManager;

		/// <summary>
		/// Lista com o grupo de dados.
		/// </summary>
		public override ArrayList DataGroupList
		{
			get
			{
				if(_grpIndexManager == null)
					return null;
				return _grpIndexManager.DataGroupList;
			}
		}

		/// <summary>
		/// Instancia do gerenciador de indice.
		/// </summary>
		public QueryIndexManager IndexManager
		{
			get
			{
				return _queryIndexManager;
			}
		}

		/// <summary>
		/// Instancia do mapa com as informações dos tipos.
		/// </summary>
		public sealed override TypeInfoMap TypeInfoMap
		{
			get
			{
				if(_queryIndexManager != null)
					return _queryIndexManager.TypeInfoMap;
				return null;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cacheClasses"></param>
		/// <param name="parentCache">Cache pai.</param>
		/// <param name="properties">Propriedades de configuração da instancia.</param>
		/// <param name="listener"></param>
		/// <param name="context"></param>
		/// <param name="activeQueryAnalyzer"></param>
		public IndexedLocalCache(IDictionary cacheClasses, CacheBase parentCache, IDictionary properties, ICacheEventsListener listener, CacheRuntimeContext context, ActiveQueryAnalyzer activeQueryAnalyzer) : base(cacheClasses, parentCache, properties, listener, context, activeQueryAnalyzer)
		{
			_grpIndexManager = new GroupIndexManager();
			IDictionary props = null;
			if(properties.Contains("indexes"))
				props = properties["indexes"] as IDictionary;
			_queryIndexManager = new NamedTagIndexManager(props, this, base._context.CacheRoot.Name);
			if(!_queryIndexManager.Initialize())
				_queryIndexManager = null;
		}

		/// <summary>
		/// Método interno acionado para adicionar um novo item.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="cacheEntry">Instancia da entrada que está sendo adicionada.</param>
		/// <param name="isUserOperation">True se for uma operação do usuário.</param>
		/// <returns>Resultado da operação.</returns>
		internal override CacheAddResult AddInternal(object key, CacheEntry cacheEntry, bool isUserOperation)
		{
			CacheAddResult result = base.AddInternal(key, cacheEntry, isUserOperation);
			switch(result)
			{
			case CacheAddResult.Success:
			case CacheAddResult.SuccessNearEviction:
				_grpIndexManager.AddToGroup(key, cacheEntry.GroupInfo);
				if((_queryIndexManager != null) && (cacheEntry.QueryInfo != null))
					_queryIndexManager.AddToIndex(key, cacheEntry);
				break;
			}
			return result;
		}

		/// <summary>
		/// Método interno para limpa dos dados da instancia.
		/// </summary>
		internal override void ClearInternal()
		{
			base.ClearInternal();
			_grpIndexManager.Clear();
			if(_queryIndexManager != null)
				_queryIndexManager.Clear();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();
			if(_queryIndexManager != null)
			{
				_queryIndexManager.Dispose();
				_queryIndexManager = null;
			}
			if(_grpIndexManager != null)
			{
				_grpIndexManager.Dispose();
				_grpIndexManager = null;
			}
			GC.Collect();
		}

		public override Hashtable GetGroup(object[] keys, string group, string subGroup, OperationContext operationContext)
		{
			Hashtable hashtable = new Hashtable();
			for(int i = 0; i < keys.Length; i++)
			{
				try
				{
					if(_grpIndexManager.KeyExists(keys[i], group, subGroup))
						hashtable[keys[i]] = this.Get(keys[i], operationContext);
				}
				catch(StateTransferException exception)
				{
					hashtable[keys[i]] = exception;
				}
			}
			return hashtable;
		}

		public override CacheEntry GetGroup(object key, string group, string subGroup, ref ulong version, ref object lockId, ref DateTime lockDate, LockExpiration lockExpiration, LockAccessType accessType, OperationContext operationContext)
		{
			if(_grpIndexManager.KeyExists(key, group, subGroup))
			{
				return this.Get(key, ref version, ref lockId, ref lockDate, lockExpiration, accessType, operationContext);
			}
			return null;
		}

		public override Hashtable GetGroupData(string group, string subGroup, OperationContext operationContext)
		{
			ArrayList groupKeys = _grpIndexManager.GetGroupKeys(group, subGroup);
			if(groupKeys == null)
			{
				return null;
			}
			object[] arrayFromCollection = MiscUtil.GetArrayFromCollection(groupKeys);
			return this.Get(arrayFromCollection, operationContext);
		}

		public override GroupInfo GetGroupInfo(object key, OperationContext operationContext)
		{
			CacheEntry entry = this.Get(key, operationContext);
			GroupInfo info = null;
			if(entry == null)
			{
				return info;
			}
			if(entry.GroupInfo != null)
			{
				return new GroupInfo(entry.GroupInfo.Group, entry.GroupInfo.SubGroup);
			}
			return new GroupInfo(null, null);
		}

		public override Hashtable GetGroupInfoBulk(object[] keys, OperationContext operationContext)
		{
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = this.Get(keys, operationContext);
			if(hashtable2 != null)
			{
				IDictionaryEnumerator enumerator = hashtable2.GetEnumerator();
				while (enumerator.MoveNext())
				{
					GroupInfo groupInfo = null;
					CacheEntry entry = (CacheEntry)enumerator.Value;
					if(entry != null)
					{
						groupInfo = entry.GroupInfo;
						if(groupInfo == null)
						{
							groupInfo = new GroupInfo(null, null);
						}
					}
					hashtable.Add(enumerator.Key, groupInfo);
				}
			}
			return hashtable;
		}

		/// <summary>
		/// Recupera o grupo de chaves.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public override ArrayList GetGroupKeys(string group, string subGroup, OperationContext operationContext)
		{
			return _grpIndexManager.GetGroupKeys(group, subGroup);
		}

		internal override ArrayList GetKeysByTag(string[] tags, TagComparisonType comparisonType, OperationContext operationContext)
		{
			switch(comparisonType)
			{
			case TagComparisonType.BY_TAG:
				return ((NamedTagIndexManager)_queryIndexManager).GetByTag(tags[0]);
			case TagComparisonType.ALL_MATCHING_TAGS:
				return ((NamedTagIndexManager)_queryIndexManager).GetAllMatchingTags(tags);
			case TagComparisonType.ANY_MATCHING_TAG:
				return ((NamedTagIndexManager)_queryIndexManager).GetAnyMatchingTag(tags);
			}
			return null;
		}

		public override EnumerationDataChunk GetNextChunk(EnumerationPointer pointer, OperationContext operationContext)
		{
			if(_enumerationIndex == null)
			{
				_enumerationIndex = new EnumerationIndex(this);
			}
			return _enumerationIndex.GetNextChunk(pointer);
		}

		public override bool HasEnumerationPointer(EnumerationPointer pointer)
		{
			if(_enumerationIndex == null)
			{
				return false;
			}
			return _enumerationIndex.Contains(pointer);
		}

		/// <summary>
		/// Método interno usado para inserir uma nova entrada no cache.
		/// </summary>
		/// <param name="key">Chave que representa a entrada.</param>
		/// <param name="cacheEntry">Instancia da entrada.</param>
		/// <param name="isUserOperation">True se for uma operação do usuário.</param>
		/// <param name="oldEntry">Valor da antiga entrada.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns>Resulta da operação.</returns>
		internal override CacheInsResult InsertInternal(object key, CacheEntry cacheEntry, bool isUserOperation, CacheEntry oldEntry, OperationContext operationContext)
		{
			if(oldEntry != null && !CacheHelper.CheckDataGroupsCompatibility(cacheEntry.GroupInfo, oldEntry.GroupInfo))
				return CacheInsResult.IncompatibleGroup;
			CacheInsResult result = base.InsertInternal(key, cacheEntry, isUserOperation, oldEntry, operationContext);
			switch(result)
			{
			case CacheInsResult.Success:
			case CacheInsResult.SuccessNearEvicition:
				_grpIndexManager.AddToGroup(key, cacheEntry.GroupInfo);
				if((_queryIndexManager != null) && (cacheEntry.QueryInfo != null))
					_queryIndexManager.AddToIndex(key, cacheEntry);
				return result;
			case CacheInsResult.SuccessOverwrite:
			case CacheInsResult.SuccessOverwriteNearEviction:
				if(oldEntry != null)
					_grpIndexManager.RemoveFromGroup(key, oldEntry.GroupInfo);
				_grpIndexManager.AddToGroup(key, cacheEntry.GroupInfo);
				if(_queryIndexManager == null)
					return result;
				if(oldEntry != null && oldEntry.QueryInfo != null)
					_queryIndexManager.RemoveFromIndex(key, oldEntry.QueryInfo);
				if(cacheEntry.QueryInfo != null)
					_queryIndexManager.AddToIndex(key, cacheEntry);
				break;
			}
			return result;
		}

		public override Hashtable Remove(string group, string subGroup, bool notify, OperationContext operationContext)
		{
			ArrayList groupKeys = _grpIndexManager.GetGroupKeys(group, subGroup);
			object[] keys = new object[groupKeys.Count];
			int index = 0;
			foreach (object obj2 in groupKeys)
			{
				keys[index] = obj2;
				index++;
			}
			return this.Remove(keys, ItemRemoveReason.Removed, notify, operationContext);
		}

		/// <summary>
		/// Remove a entrada associada com o chave.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="removalReason"></param>
		/// <param name="isUserOperation"></param>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		internal override CacheEntry RemoveInternal(object key, ItemRemoveReason removalReason, bool isUserOperation, OperationContext operationContext)
		{
			CacheEntry entry = base.RemoveInternal(key, removalReason, isUserOperation, operationContext);
			if(entry != null)
			{
				_grpIndexManager.RemoveFromGroup(key, entry.GroupInfo);
				if((_queryIndexManager != null) && (entry.QueryInfo != null))
				{
					_queryIndexManager.RemoveFromIndex(key, entry.QueryInfo);
				}
			}
			return entry;
		}

		/// <summary>
		/// Cria um contexto para a pesquisa com base no predicao
		/// </summary>
		/// <param name="pred"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		internal override QueryContext SearchInternal(Predicate pred, IDictionary values)
		{
			var queryContext = new QueryContext(this);
			queryContext.AttributeValues = values;
			queryContext.CacheContext = base._context.CacheRoot.Name;
			try
			{
				pred.Execute(queryContext, null);
			}
			catch(Exception)
			{
				throw;
			}
			return queryContext;
		}
	}
}
