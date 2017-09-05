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
	/// Representa o container de referencias carregadas.
	/// </summary>
	public class EntityLoaderReferenceContainer : IEnumerable<KeyValuePair<string, IEntity>>, IDisposable
	{
		private Dictionary<string, IEntity> _items;

		/// <summary>
		/// Armazena a relação dos itens que foram carregados
		/// </summary>
		private List<string> _itemsLoaded = new List<string>();

		/// <summary>
		/// Constrói a instancia com os itens das referencias.
		/// </summary>
		/// <param name="items"></param>
		internal EntityLoaderReferenceContainer(IEnumerable<Tuple<string, IEntity>> items)
		{
			items.Require("items").NotNull();
			_items = new Dictionary<string, IEntity>();
			foreach (var i in items)
			{
				if(_items.ContainsKey(i.Item1))
					throw new DetailsInvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityLoaderReferenceContainer_DuplicateReferebceEntryWithName, i.Item1));
				_items.Add(i.Item1, i.Item2);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public EntityLoaderReferenceContainer()
		{
			_items = new Dictionary<string, IEntity>();
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~EntityLoaderReferenceContainer()
		{
			Dispose(false);
		}

		/// <summary>
		/// Adiciona uma nova referência para o container.
		/// </summary>
		/// <param name="referenceName">Nome da refer.</param>
		/// <param name="entity">Instancia da entidade filha.</param>
		/// <returns></returns>
		public EntityLoaderReferenceContainer Add(string referenceName, IEntity entity)
		{
			referenceName.Require("referenceName").NotNull().NotEmpty();
			if(_items.ContainsKey(referenceName))
				throw new ArgumentException(string.Format("Reference with name '{0}' exists in container", referenceName));
			_items.Add(referenceName, entity);
			return this;
		}

		/// <summary>
		/// Recupera a instancia da referência.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="name">Nome da referência.</param>
		/// <returns></returns>
		public TEntity Get<TEntity>(string name) where TEntity : IEntity
		{
			if(_items == null)
				throw new ObjectDisposedException("EntityLoaderReferenceContainer");
			IEntity result = null;
			if(_items.TryGetValue(name, out result))
			{
				var index = _itemsLoaded.BinarySearch(name);
				if(index < 0)
					_itemsLoaded.Insert(~index, name);
				return (TEntity)result;
			}
			throw new Exception(string.Format("Reference with name '{0}' not found", name));
		}

		/// <summary>
		/// Notifica que a referência associada com o novo foi carregada.
		/// </summary>
		/// <param name="name"></param>
		public void NotifyLoaded(string name)
		{
			if(_items.ContainsKey(name))
			{
				var index = _itemsLoaded.BinarySearch(name);
				if(index < 0)
					_itemsLoaded.Insert(~index, name);
			}
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
