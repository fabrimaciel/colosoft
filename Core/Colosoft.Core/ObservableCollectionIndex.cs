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

namespace Colosoft.Collections
{
	/// <summary>
	/// Implementação básica de um indice da coleção observada.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ObservableCollectionIndex<T> : IObservableCollectionIndex<T>
	{
		private string _name;

		private IObservableCollection<T> _collection;

		private string[] _watchProperties;

		private Func<T, object> _keyGetter;

		private System.Collections.IComparer _comparer;

		private object _syncRoot = new object();

		/// <summary>
		/// Nome do indice.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Propriedade que são monitoradas pelo indice.
		/// </summary>
		public string[] WatchProperties
		{
			get
			{
				return _watchProperties;
			}
		}

		/// <summary>
		/// Func usado para recuperar o valor da chave.
		/// </summary>
		protected Func<T, object> KeyGetter
		{
			get
			{
				return _keyGetter;
			}
		}

		/// <summary>
		/// Comparador do indice.
		/// </summary>
		protected virtual System.Collections.IComparer Comparer
		{
			get
			{
				return _comparer;
			}
		}

		/// <summary>
		/// Coleção onde estão os dados reais.
		/// </summary>
		protected IObservableCollection<T> Collection
		{
			get
			{
				return _collection;
			}
		}

		/// <summary>
		/// Instancia usada para sincronizar as operações.
		/// </summary>
		protected object SyncRoot
		{
			get
			{
				return _syncRoot;
			}
		}

		/// <summary>
		/// Recupera o itens pelo indice informado.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public abstract IEnumerable<T> this[object key]
		{
			get;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do indice.</param>
		/// <param name="collection">Coleção que será observada.</param>
		/// <param name="watchProperties">Relação das propriedades assistidas.</param>
		/// <param name="keyGetter">Ponteiro do método usado para recupera o valor da chave do item.</param>
		/// <param name="comparer">Comparador que será utilizado.</param>
		public ObservableCollectionIndex(string name, IObservableCollection<T> collection, string[] watchProperties, Func<T, object> keyGetter, System.Collections.IComparer comparer)
		{
			name.Require("name").NotNull().NotEmpty();
			collection.Require("collection").NotNull();
			keyGetter.Require("keyGetter").NotNull();
			comparer.Require("comparer").NotNull();
			_name = name;
			_collection = collection;
			_watchProperties = watchProperties ?? new string[0];
			_keyGetter = keyGetter;
			_comparer = comparer;
			_collection.CollectionChanged += CollectionCollectionChanged;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~ObservableCollectionIndex()
		{
			Dispose(false);
		}

		/// <summary>
		/// Reseta o indice.
		/// </summary>
		public virtual void Reset()
		{
			lock (SyncRoot)
			{
				Initialize();
			}
		}

		/// <summary>
		/// Inicializa o indice.
		/// </summary>
		protected virtual void Initialize()
		{
			for(var i = 0; i < _collection.Count; i++)
			{
				OnAdded(_collection[i], i);
			}
		}

		/// <summary>
		/// Método usado para tratar os novos itens adicionados.
		/// </summary>
		/// <param name="item">Item adicionado</param>
		/// <param name="index">Indice do item na coleção de origem.</param>
		protected virtual void OnAdded(T item, int index)
		{
		}

		/// <summary>
		/// Método acionado quando o item for removido.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="index"></param>
		protected virtual void OnRemoved(T item, int index)
		{
		}

		/// <summary>
		/// Método acionado quando a coleção for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CollectionCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				for(var i = 0; i < e.NewItems.Count; i++)
					OnAdded((T)e.NewItems[i], e.NewStartingIndex + i);
			}
			else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				for(var i = 0; i < e.OldItems.Count; i++)
					OnRemoved((T)e.OldItems[i], e.OldStartingIndex + i);
			}
			else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
			{
				for(var i = 0; i < e.OldItems.Count; i++)
					OnRemoved((T)e.OldItems[i], e.OldStartingIndex + i);
				for(var i = 0; i < e.NewItems.Count; i++)
					OnAdded((T)e.NewItems[i], e.NewStartingIndex + i);
			}
			else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
			{
				Reset();
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_collection.CollectionChanged -= CollectionCollectionChanged;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
