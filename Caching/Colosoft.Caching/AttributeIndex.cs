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

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Representa um indice a atributos.
	/// </summary>
	public class AttributeIndex : IQueryIndex
	{
		/// <summary>
		/// Nome do cache.
		/// </summary>
		protected string _cacheName;

		/// <summary>
		/// Instancia a tabela do indices.
		/// </summary>
		protected Hashtable _indexTable = new Hashtable();

		/// <summary>
		/// Quantidade de itens no indice.
		/// </summary>
		public int Size
		{
			get
			{
				if(_indexTable != null)
					return _indexTable.Count;
				return 0;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="attribList">Lista inicial dos atributos.</param>
		/// <param name="cacheName">Nome do cache.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public AttributeIndex(IEnumerable<string> attribList, string cacheName)
		{
			_cacheName = cacheName;
			this.Initialize(attribList);
		}

		/// <summary>
		/// Cria um IIndexStore.
		/// </summary>
		/// <returns></returns>
		private IIndexStore CreateIndexStore()
		{
			return new RBStore(_cacheName);
		}

		/// <summary>
		/// Converte a chave informada para uma tag.
		/// </summary>
		/// <param name="indexKey"></param>
		/// <returns></returns>
		protected string ConvertToNamedTagKey(string indexKey)
		{
			return ("$NamedTagAttribute$" + indexKey);
		}

		/// <summary>
		/// Verifica se a chave informada é chave de tag nomeada.
		/// </summary>
		/// <param name="indexKey"></param>
		/// <returns></returns>
		protected bool IsNamedTagKey(string indexKey)
		{
			return indexKey.StartsWith("$NamedTagAttribute$");
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="attribList">Atributos que serão usados para inicializar a instancia.</param>
		protected virtual void Initialize(IEnumerable<string> attribList)
		{
			if(attribList != null)
			{
				IIndexStore store = null;
				IEnumerator enumerator = attribList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					store = CreateIndexStore();
					_indexTable[enumerator.Current] = store;
				}
				store = CreateIndexStore();
				_indexTable["$Tag$"] = store;
			}
		}

		/// <summary>
		/// Adiciona uma chave e um valor para o indice.
		/// </summary>
		/// <param name="key">Chave que será adicionada.</param>
		/// <param name="value">Valor que será adicionado.</param>
		public virtual void AddToIndex(object key, object value)
		{
			if(!(value is Hashtable))
				throw new ArgumentException("Value is not Hashtable");
			IDictionaryEnumerator enumerator = (value as Hashtable).GetEnumerator();
			while (enumerator.MoveNext())
			{
				string indexKey = (string)enumerator.Key;
				IIndexStore store = _indexTable[indexKey] as IIndexStore;
				if(store == null)
				{
					if(indexKey == "$Tag$")
					{
						store = CreateIndexStore();
						_indexTable[indexKey] = store;
					}
					else
					{
						string str2 = this.ConvertToNamedTagKey(indexKey);
						store = _indexTable[str2] as IIndexStore;
						if(store == null)
						{
							store = CreateIndexStore();
							_indexTable[str2] = store;
						}
					}
				}
				if(store != null)
				{
					object obj2 = enumerator.Value;
					if(obj2 != null)
						store.Add(obj2, key);
					else
						store.Add(CacheNull.Value, key);
				}
			}
		}

		/// <summary>
		/// Limpa os dados do indice.
		/// </summary>
		public void Clear()
		{
			IDictionaryEnumerator enumerator = _indexTable.GetEnumerator();
			while (enumerator.MoveNext())
				(enumerator.Value as IIndexStore).Clear();
		}

		/// <summary>
		/// Recupera o enumerado do os items do indice.
		/// </summary>
		/// <returns></returns>
		public IDictionaryEnumerator GetEnumerator()
		{
			return _indexTable.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerado dos dados do indice.
		/// </summary>
		/// <param name="typeName">Nome do tipo que será usado para filtra os dados.</param>
		/// <param name="forTag">Identifica ser o enumerado será recuperado para tags.</param>
		/// <returns></returns>
		public IDictionaryEnumerator GetEnumerator(string typeName, bool forTag)
		{
			IDictionaryEnumerator enumerator = _indexTable.GetEnumerator();
			if(!forTag)
			{
				while (enumerator.MoveNext())
				{
					IIndexStore store = enumerator.Value as IIndexStore;
					if(((string)enumerator.Key) != "$Tag$")
						return store.GetEnumerator();
				}
			}
			else if(_indexTable.Contains("$Tag$"))
			{
				IIndexStore store2 = _indexTable["$Tag$"] as IIndexStore;
				return store2.GetEnumerator();
			}
			return null;
		}

		/// <summary>
		/// Recupera a fonte de armazenamento pelo nome do atributo.
		/// </summary>
		/// <param name="attribName"></param>
		/// <returns></returns>
		public IIndexStore GetStore(string attribName)
		{
			IIndexStore store = null;
			if(_indexTable.Contains(attribName))
				return (_indexTable[attribName] as IIndexStore);
			string key = this.ConvertToNamedTagKey(attribName);
			if(_indexTable.Contains(key))
				store = _indexTable[key] as IIndexStore;
			return store;
		}

		/// <summary>
		/// Remove a chave e o valor do indice.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public virtual void RemoveFromIndex(object key, object value)
		{
			IDictionaryEnumerator enumerator = (value as Hashtable).GetEnumerator();
			while (enumerator.MoveNext())
			{
				string str = (string)enumerator.Key;
				if(_indexTable.Contains(str) || _indexTable.Contains(str = ConvertToNamedTagKey(str)))
				{
					IIndexStore store = _indexTable[str] as IIndexStore;
					object obj2 = enumerator.Value;
					if(obj2 != null)
						store.Remove(obj2, key);
					else
						store.Remove(CacheNull.Value, key);
					if(store.Size == 0)
					{
						if((str == "$Tag$") || IsNamedTagKey(str))
							_indexTable.Remove(str);
						else
							_indexTable[str] = CreateIndexStore();
					}
				}
			}
		}
	}
}
