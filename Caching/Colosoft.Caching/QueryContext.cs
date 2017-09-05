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
using Colosoft.Caching.Data;
using Colosoft.Caching.Local;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Representa o contexto de uma pesquisa.
	/// </summary>
	internal class QueryContext
	{
		private LocalCacheBase _cache;

		private AttributeIndex _index;

		private bool _populateTree = true;

		private QueryResultSet _resultSet = new QueryResultSet();

		private string _typeName = string.Empty;

		/// <summary>
		/// Valores dos atributos da consulta.
		/// </summary>
		public IDictionary AttributeValues
		{
			get;
			set;
		}

		/// <summary>
		/// Instancia do cache associada.
		/// </summary>
		public LocalCacheBase Cache
		{
			get
			{
				return _cache;
			}
		}

		/// <summary>
		/// Nome do contexto do cache.
		/// </summary>
		public string CacheContext
		{
			get;
			set;
		}

		/// <summary>
		/// Indice dos atributos.
		/// </summary>
		public AttributeIndex Index
		{
			get
			{
				if(_index == null)
				{
					IQueryIndex index1 = null;
					if(this.IndexManager.IndexMap.TryGetValue(this.TypeName, out index1))
						_index = index1 as AttributeIndex;
				}
				return _index;
			}
			set
			{
				_index = value;
			}
		}

		/// <summary>
		/// Gerenciador dos indices.
		/// </summary>
		public QueryIndexManager IndexManager
		{
			get
			{
				return ((IndexedLocalCache)_cache).IndexManager;
			}
		}

		/// <summary>
		/// Identifica se a arvore está populada.
		/// </summary>
		public bool PopulateTree
		{
			get
			{
				return _populateTree;
			}
			set
			{
				_populateTree = value;
			}
		}

		/// <summary>
		/// Conjunto do resultado.
		/// </summary>
		internal QueryResultSet ResultSet
		{
			get
			{
				return _resultSet;
			}
			set
			{
				_resultSet = value;
			}
		}

		/// <summary>
		/// Arvore associada.
		/// </summary>
		public SRTree Tree
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do tipo associado.
		/// </summary>
		public string TypeName
		{
			get
			{
				return _typeName;
			}
			set
			{
				_typeName = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cache">Instancia do cache local associado com o contexto.</param>
		public QueryContext(LocalCacheBase cache)
		{
			_cache = cache;
			this.Tree = new SRTree();
		}

		/// <summary>
		/// Recupera o valor de uma entrada pela chave informada.
		/// </summary>
		/// <param name="key">Chave que representa a entrada no cache.</param>
		/// <param name="operationContext">Contexto da operação.</param>
		/// <returns></returns>
		public object Get(object key, OperationContext operationContext)
		{
			return this.Cache.Get(key, operationContext).DeflattedValue(this.CacheContext);
		}
	}
}
