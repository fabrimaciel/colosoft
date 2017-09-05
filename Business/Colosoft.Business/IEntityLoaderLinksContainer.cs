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
	/// Assinatura de um container de links
	/// </summary>
	public interface IEntityLoaderLinksContainer : IDisposable
	{
		/// <summary>
		/// Recupera a instancia do link pelo nome informado.
		/// </summary>
		/// <param name="name">Nome do link.</param>
		/// <returns></returns>
		IEntity this[string name]
		{
			get;
		}

		/// <param name="linkName">Nome do link.</param>
		/// <param name="entity">Instancia da entidade do link.</param>
		/// <returns></returns>
		IEntityLoaderLinksContainer AddLink(string linkName, IEntity entity);

		/// <summary>
		/// Recupera a instancia do link pelo nome informado.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade.</typeparam>
		/// <param name="name">Nome do link.</param>
		/// <returns></returns>
		TEntity GetSingle<TEntity>(string name) where TEntity : IEntity;

		/// <summary>
		/// Recupera a instancia do filho pelo nome informado.
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade.</typeparam>
		/// <param name="name">Nome do link.</param>
		/// <returns></returns>
		IEntityLinksList<TEntity> Get<TEntity>(string name) where TEntity : IEntity;
	}
	/// <summary>
	/// Implementação do container dos links.
	/// </summary>
	public class EntityLoaderLinksContainer : IEntityLoaderLinksContainer, IDisposable
	{
		private Dictionary<string, IEntity> _items;

		/// <summary>
		/// Armazena a relação dos items que foram carregados
		/// </summary>
		private List<string> _itemsLoaded = new List<string>();

		/// <summary>
		/// Recupera a instancia do link pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
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
		/// <param name="links">Links que serão usados na carga.</param>
		internal EntityLoaderLinksContainer(IEnumerable<Tuple<string, IEntity>> links)
		{
			_items = new Dictionary<string, IEntity>();
			foreach (var i in links)
				_items.Add(i.Item1, i.Item2);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public EntityLoaderLinksContainer()
		{
			_items = new Dictionary<string, IEntity>();
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~EntityLoaderLinksContainer()
		{
			Dispose(false);
		}

		/// <param name="linkName">Nome do link.</param>
		/// <param name="entity">Instancia da entidade do link.</param>
		/// <returns></returns>
		public IEntityLoaderLinksContainer AddLink(string linkName, IEntity entity)
		{
			_items.Add(linkName, entity);
			return this;
		}

		/// <summary>
		/// Recupera a instancia do link pelo nome informado.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public IEntityLinksList<TEntity> Get<TEntity>(string name) where TEntity : IEntity
		{
			if(_items == null)
				throw new ObjectDisposedException("EntityLoaderChildContainer");
			IEntity result = null;
			if(_items.TryGetValue(name, out result))
			{
				var index = _itemsLoaded.BinarySearch(name);
				if(index < 0)
					_itemsLoaded.Insert(~index, name);
				return (IEntityLinksList<TEntity>)result;
			}
			throw new Exception(string.Format("Link with name '{0}' not found", name));
		}

		/// <summary>
		/// Recupera uma entidade simples do link pelo nome informado,
		/// </summary>
		/// <typeparam name="TEntity">Tipo da entidade do link.</typeparam>
		/// <param name="name">Nome do link.</param>
		/// <returns></returns>
		public TEntity GetSingle<TEntity>(string name) where TEntity : IEntity
		{
			if(_items == null)
				throw new ObjectDisposedException("EntityLoaderLinksContainer");
			IEntity result = null;
			if(_items.TryGetValue(name, out result))
			{
				var index = _itemsLoaded.BinarySearch(name);
				if(index < 0)
					_itemsLoaded.Insert(~index, name);
				return (TEntity)result;
			}
			throw new Exception(string.Format("Link if name '{0}' not found", name));
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
