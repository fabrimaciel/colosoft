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
using System.Collections.Specialized;

namespace Colosoft.Collections
{
	/// <summary>
	/// Implementação de uma coleção observada ordenada.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public class SortableObservableCollection<T> : IObservableCollection<T>, IDisposable, System.Collections.IList
	{
		/// <summary>
		/// Coleção de origem.
		/// </summary>
		private IObservableCollection<T> _source;

		/// <summary>
		/// Relação dos indices dos itens da coleção.
		/// </summary>
		private BaseObservableCollection<int> _indexes = new BaseObservableCollection<int>();

		/// <summary>
		/// Comparador utilizado pela instancia.
		/// </summary>
		private IComparer<T> _comparer;

		/// <summary>
		/// Container das propriedade usadas pelo comparer.
		/// </summary>
		private IPropertiesContainer _comparerPropertiesContainer;

		private object _syncObject = new object();

		/// <summary>
		/// Evento acionado quando a coleção for alterada.
		/// </summary>
		public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Evento acionado quando uma propriedade for alterada.
		/// </summary>
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Quantidade de itens na coleçao.
		/// </summary>
		public int Count
		{
			get
			{
				return _source.Count;
			}
		}

		/// <summary>
		/// Identifica se a co~leção é somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return _source.IsReadOnly;
			}
		}

		/// <summary>
		/// Recupera o item que está no indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public T this[int index]
		{
			get
			{
				return _source[_indexes[index]];
			}
			set
			{
				_source[_indexes[index]] = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="source">Relação da coleção de origem dos dados.</param>
		/// <param name="comparer">Comparador que será utilizado para ordenar os itens.</param>
		public SortableObservableCollection(IObservableCollection<T> source, IComparer<T> comparer)
		{
			source.Require("source").NotNull();
			comparer.Require("comparer").NotNull();
			_source = source;
			_comparer = comparer;
			_comparerPropertiesContainer = comparer as IPropertiesContainer;
			_source.CollectionChanged += SourceCollectionChanged;
			_indexes = new BaseObservableCollection<int>();
			for(var i = 0; i < _source.Count; i++)
				_indexes.Add(i);
			foreach (var i in _source)
				RegisterItem(i);
			CalculateIndexes();
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~SortableObservableCollection()
		{
			Dispose(false);
		}

		/// <summary>
		/// Método acionado quando uma propriedade for alterada.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(this.PropertyChanged != null)
				this.PropertyChanged(this, e);
		}

		/// <summary>
		/// Notifica a alteração de uma propriedade.
		/// </summary>
		/// <param name="propertyName"></param>
		private void OnPropertyChanged(string propertyName)
		{
			this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Registra o item controlado pela coleção.
		/// </summary>
		/// <param name="item"></param>
		private void RegisterItem(T item)
		{
			if(item is System.ComponentModel.INotifyPropertyChanged)
				((System.ComponentModel.INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
		}

		/// <summary>
		/// Remove registro do item controlado pela coleção.
		/// </summary>
		/// <param name="item"></param>
		private void UnregisterItem(T item)
		{
			if(item is System.ComponentModel.INotifyPropertyChanged)
				((System.ComponentModel.INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
		}

		/// <summary>
		/// Método acionado quando a propriedade de uma dos itens da coleção for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(_comparerPropertiesContainer != null && _comparerPropertiesContainer.ContainsProperty(e.PropertyName))
			{
				var item = (T)sender;
				var sourceIndex = _source.IndexOf(item);
				if(sourceIndex >= 0)
				{
					for(var i = 0; i < _indexes.Count; i++)
					{
						if(_indexes[i] == sourceIndex)
						{
							if((i > 0 && _comparer.Compare(item, _source[_indexes[i - 1]]) < 0) || (i < _indexes.Count && _comparer.Compare(item, _source[_indexes[i + 1]]) > 0))
							{
								CalculateIndexes();
							}
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Calcula os indíces da coleção.
		/// </summary>
		private void CalculateIndexes()
		{
			CalculateIndexesWithoutNotifyChanges();
			_indexes.CollectionChanged += IndexesCollectionChanged;
			_indexes.OnCollectionReset();
		}

		/// <summary>
		/// Calcula os indíces da coleção sem notificar as alterações.
		/// </summary>
		private void CalculateIndexesWithoutNotifyChanges()
		{
			if(_indexes.Count > 1)
			{
				var start = 0;
				var index = 0;
				while (start < _indexes.Count)
				{
					index = start;
					var item = _source[_indexes[index]];
					for(var i = start + 1; i < _indexes.Count; i++)
						if(_comparer.Compare(_source[_indexes[i]], item) < 0)
						{
							index = i;
							item = _source[_indexes[index]];
						}
					if(index != start)
					{
						var aux = _indexes[index];
						_indexes[index] = _indexes[start];
						_indexes[start] = aux;
					}
					start++;
				}
			}
		}

		/// <summary>
		/// Método acionado quando a coleção de origem for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if(e.Action == NotifyCollectionChangedAction.Add)
			{
				for(var i = 0; i < e.NewItems.Count; i++)
				{
					var item = (T)e.NewItems[i];
					var inserted = false;
					for(var j = 0; j < _indexes.Count; j++)
						if(_comparer.Compare(_source[_indexes[j]], item) > 0)
						{
							_indexes.Insert(j, e.NewStartingIndex + i);
							RegisterItem(item);
							inserted = true;
							break;
						}
					if(!inserted)
						_indexes.Add(e.NewStartingIndex + i);
				}
			}
			else if(e.Action == NotifyCollectionChangedAction.Move)
			{
				for(var i = 0; i < e.NewItems.Count; i++)
				{
					var newIndex = -1;
					var oldIndex = -1;
					for(var j = 0; (newIndex < 0 || oldIndex < 0) && j < _indexes.Count; j++)
					{
						if(newIndex < 0 && _indexes[j] == e.NewStartingIndex + i)
							newIndex = j;
						else if(oldIndex < 0 && _indexes[j] == e.OldStartingIndex + i)
							oldIndex = j;
					}
					if(newIndex >= 0 && oldIndex >= 0)
					{
						var aux = _indexes[newIndex];
						_indexes[newIndex] = _indexes[oldIndex];
						_indexes[oldIndex] = aux;
					}
				}
			}
			else if(e.Action == NotifyCollectionChangedAction.Remove)
			{
				for(var i = 0; i < e.OldItems.Count; i++)
				{
					for(var j = 0; j < _indexes.Count; j++)
					{
						if(_indexes[j] == e.OldStartingIndex + i)
						{
							UnregisterItem((T)e.OldItems[i]);
							_indexes.CollectionChanged -= IndexesCollectionChanged;
							_indexes.Clear();
							for(var x = 0; x < _source.Count; x++)
								_indexes.Add(x);
							try
							{
								CalculateIndexesWithoutNotifyChanges();
							}
							finally
							{
								_indexes.CollectionChanged += IndexesCollectionChanged;
							}
							if(CollectionChanged != null)
								CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems[i], j));
							break;
						}
					}
				}
			}
			else if(e.Action == NotifyCollectionChangedAction.Replace)
			{
				CalculateIndexes();
			}
			else if(e.Action == NotifyCollectionChangedAction.Reset)
			{
				try
				{
					_indexes.CollectionChanged -= IndexesCollectionChanged;
					foreach (var item in _source)
						UnregisterItem(item);
					for(var i = 0; i < _source.Count; i++)
						_indexes.Add(i);
				}
				finally
				{
					_indexes.CollectionChanged += IndexesCollectionChanged;
				}
				CalculateIndexes();
			}
		}

		/// <summary>
		/// Método acionado quando a coleção de indices for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void IndexesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if(CollectionChanged != null && e.Action != NotifyCollectionChangedAction.Remove)
			{
				IList<T> newItems = null;
				IList<T> oldItems = null;
				if(e.NewItems != null)
				{
					newItems = new List<T>();
					foreach (int index in e.NewItems)
						newItems.Add(_source[index]);
				}
				if(e.OldItems != null)
				{
					oldItems = new List<T>();
					foreach (int index in e.OldItems)
						oldItems.Add(_source[index]);
				}
				NotifyCollectionChangedEventArgs args = null;
				switch(e.Action)
				{
				case NotifyCollectionChangedAction.Replace:
					args = new NotifyCollectionChangedEventArgs(e.Action, newItems.FirstOrDefault(), oldItems.FirstOrDefault(), e.NewStartingIndex);
					break;
				case NotifyCollectionChangedAction.Move:
					args = new NotifyCollectionChangedEventArgs(e.Action, newItems.FirstOrDefault(), e.NewStartingIndex, e.OldStartingIndex);
					break;
				case NotifyCollectionChangedAction.Add:
					args = new NotifyCollectionChangedEventArgs(e.Action, newItems.FirstOrDefault(), e.NewStartingIndex);
					break;
				case NotifyCollectionChangedAction.Remove:
					break;
				default:
					args = new NotifyCollectionChangedEventArgs(e.Action);
					break;
				}
				CollectionChanged(this, args);
			}
		}

		/// <summary>
		/// Recupera o indice do item na coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int IndexOf(T item)
		{
			var originalIndex = _source.IndexOf(item);
			if(originalIndex >= 0)
				return _indexes.IndexOf(originalIndex);
			return originalIndex;
		}

		/// <summary>
		/// Insere o item na coleção
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		public void Insert(int index, T item)
		{
			_source.Add(item);
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			_source.RemoveAt(_indexes[index]);
		}

		/// <summary>
		/// Adiciona o item na coleção.
		/// </summary>
		/// <param name="item"></param>
		public void Add(T item)
		{
			_source.Add(item);
		}

		/// <summary>
		/// Limpa a coleção.
		/// </summary>
		public void Clear()
		{
			_source.Clear();
		}

		/// <summary>
		/// Verifica se o item informado existe na coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T item)
		{
			return _source.Contains(item);
		}

		/// <summary>
		/// Cropa os dados para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			_source.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Remove o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(T item)
		{
			return _source.Remove(item);
		}

		/// <summary>
		/// Novo o item.
		/// </summary>
		/// <param name="oldIndex"></param>
		/// <param name="newIndex"></param>
		public void Move(int oldIndex, int newIndex)
		{
			_source.Move(_indexes[oldIndex], _indexes[newIndex]);
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			foreach (var index in _indexes)
				yield return _source[index];
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			foreach (var index in _indexes)
				yield return _source[index];
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_source != null)
			{
				_source.CollectionChanged -= SourceCollectionChanged;
				foreach (var item in _source)
					UnregisterItem(item);
			}
			_source = null;
			_indexes.Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Adiciona o valor a coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int System.Collections.IList.Add(object value)
		{
			Add((T)value);
			return 1;
		}

		/// <summary>
		/// Verifica se contém o valor informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		bool System.Collections.IList.Contains(object value)
		{
			var value2 = (T)value;
			return this.Contains(value2);
		}

		/// <summary>
		/// Recupera o indice do item na coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int System.Collections.IList.IndexOf(object value)
		{
			var value2 = (T)value;
			return this.IndexOf(value2);
		}

		/// <summary>
		/// Insere o item no posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		void System.Collections.IList.Insert(int index, object value)
		{
			var value2 = (T)value;
			this.Insert(index, value2);
		}

		/// <summary>
		/// Identifica se a coleção possui um tamanho fixado.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Remove o item.
		/// </summary>
		/// <param name="value"></param>
		void System.Collections.IList.Remove(object value)
		{
			var value2 = (T)value;
			this.Remove(value2);
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (T)value;
			}
		}

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				return _syncObject;
			}
		}
	}
}
