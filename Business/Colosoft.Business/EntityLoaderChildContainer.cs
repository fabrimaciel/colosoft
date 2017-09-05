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
	/// Possíveis prioridades de para salvar os filhos de uma entidade.
	/// </summary>
	public enum EntityChildSavePriority
	{
		/// <summary>
		/// Salva depois da entidade.
		/// </summary>
		AfterEntity,
		/// <summary>
		/// Salva antes da entidade.
		/// </summary>
		BeforeEntity
	}
	/// <summary>
	/// Representa o container dos filhos carregados.
	/// </summary>
	public class EntityLoaderChildContainer : IEnumerable<KeyValuePair<string, IEntity>>, IDisposable
	{
		private Dictionary<string, IEntity> _items;

		/// <summary>
		/// Armazena a relação dos items que foram carregados
		/// </summary>
		private List<string> _itemsLoaded = new List<string>();

		/// <summary>
		/// Recupera a instancia do filho pelo nome informado.
		/// </summary>
		/// <param name="name">Nome do filho.</param>
		/// <returns></returns>
		public IEntity this[string name]
		{
			get
			{
				return _items[name];
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="items"></param>
		internal EntityLoaderChildContainer(IEnumerable<Tuple<string, IEntity>> items)
		{
			items.Require("items").NotNull();
			_items = new Dictionary<string, IEntity>();
			foreach (var i in items)
			{
				if(_items.ContainsKey(i.Item1))
					throw new DetailsInvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderChildContainer_DuplicateChildEntryWithName, i.Item1));
				_items.Add(i.Item1, i.Item2);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public EntityLoaderChildContainer()
		{
			_items = new Dictionary<string, IEntity>();
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~EntityLoaderChildContainer()
		{
			Dispose(false);
		}

		/// <summary>
		/// Recupera a instancia do filho pelo nome informado.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public TEntity GetSingle<TEntity>(string name) where TEntity : IEntity
		{
			if(_items == null)
				throw new ObjectDisposedException("EntityLoaderChildContainer");
			IEntity result = null;
			if(_items.TryGetValue(name, out result))
			{
				var index = _itemsLoaded.BinarySearch(name);
				if(index < 0)
					_itemsLoaded.Insert(~index, name);
				return (TEntity)result;
			}
			throw new Exception(string.Format("Child with name '{0}' not found", name));
		}

		/// <summary>
		/// Recupera a instancia do filho pelo nome informado.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public IEntityChildrenList<TEntity> Get<TEntity>(string name) where TEntity : IEntity
		{
			if(_items == null)
				throw new ObjectDisposedException("EntityLoaderChildContainer");
			IEntity result = null;
			if(_items.TryGetValue(name, out result))
			{
				var index = _itemsLoaded.BinarySearch(name);
				if(index < 0)
					_itemsLoaded.Insert(~index, name);
				return (IEntityChildrenList<TEntity>)result;
			}
			throw new Exception(string.Format("Child with name '{0}' not found", name));
		}

		/// <summary>
		/// Adiciona um novo filho para o container.
		/// </summary>
		/// <param name="childName">Nome do filho.</param>
		/// <param name="entity">Instancia da entidade filha.</param>
		/// <returns></returns>
		public EntityLoaderChildContainer Add(string childName, IEntity entity)
		{
			childName.Require("childName").NotNull().NotEmpty();
			if(_items.ContainsKey(childName))
				throw new ArgumentException(string.Format("Child with name '{0}' exists in container", childName));
			_items.Add(childName, entity);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator<KeyValuePair<string, IEntity>> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_items != null)
			{
				if(_items.Count != _itemsLoaded.Count)
				{
					int index = 0;
					foreach (var i in _items)
					{
						index = _itemsLoaded.BinarySearch(i.Key);
						if(index < 0 && i.Value != null)
							i.Value.Dispose();
						else if(index >= 0)
							_itemsLoaded.RemoveAt(index);
					}
				}
				_items.Clear();
				_itemsLoaded.Clear();
				_items = null;
				_itemsLoaded = null;
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
