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
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Colosoft.Collections
{
	/// <summary>
	/// Estrutura que representa uma lista virtual, ou seja,
	/// ela contêm itens que ainda não foram carregados.
	/// </summary>
	public class VirtualList<T> : IEnumerable<T>, IEnumerable, IList<T>, IList, ICollection<T>, ICollection, INotifyCollectionChanged, INotifyPropertyChanged, IObservableCollection<T>, IDisposable, IVirtualList, IVirtualListResultProcessor<T>
	{
		private object _instanceInitializingLock = new object();

		/// <summary>
		/// Quantidade de elementos da lista.
		/// </summary>
		protected int _count;

		/// <summary>
		/// Versão da lista.
		/// </summary>
		internal int _version;

		/// <summary>
		/// Objeto usadao para garantir a sincronização da lista.
		/// </summary>
		private object _syncRoot;

		private bool _ignoreLoaderDispose;

		/// <summary>
		/// Evento que sinaliza a mudança da coleção.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Evento que sinaliza a mudança da propriedade.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Evento acionado quando uma página de dados for carregada.
		/// </summary>
		public event DataPageLoadedEventHandler<T> DataPageLoaded;

		/// <summary>
		/// Proprieadade para acessar o count nas classes derivadas.
		/// </summary>
		protected int Count2
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value;
			}
		}

		/// <summary>
		/// Instancia do método usado para recuperar os itens da lista virtual.
		/// </summary>
		public VirtualListLoader<T> Loader
		{
			get;
			protected set;
		}

		/// <summary>
		/// Identifica se a instancia foi inicializada.
		/// </summary>
		public virtual bool InstanceInitialized
		{
			get;
			set;
		}

		/// <summary>
		/// Sessões onde são armazenados os elementos carregados na lista
		/// </summary>
		internal IDataPage<T>[] Sessions
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="pageSize">Tamanho da página da lista virtual.</param>
		/// <param name="loader">Instancia do método que será usado para carregar os dados.</param>
		/// <param name="referenceObject"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public VirtualList(int pageSize, VirtualListLoaderHandler<T> loader, object referenceObject) : this(pageSize, VirtualListLoader<T>.Create(loader), referenceObject)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="pageSize">Tamanho da página da lista virtual.</param>
		/// <param name="loader">Instancia que será usada para carregar os dados.</param>
		/// <param name="referenceObject"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public VirtualList(int pageSize, VirtualListLoader<T> loader, object referenceObject)
		{
			if(loader == null)
				throw new ArgumentNullException("loader");
			PageSize = pageSize;
			Loader = loader;
			ReferenceObject = referenceObject;
			Refresh();
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~VirtualList()
		{
			Dispose(false);
		}

		/// <summary>
		/// Quantidade de itens na lista.
		/// </summary>
		public virtual int Count
		{
			get
			{
				if(!InstanceInitialized)
				{
					InstanceInitialized = true;
					var data = Loader.Load(this, 0, 0, true, ReferenceObject);
					if(data == null)
						throw new InvalidOperationException("LoadResult cannot be null.");
					_count = data.NewCount;
					int numberSessions = PageSize == 0 ? 1 : (int)Math.Ceiling(_count / (double)PageSize);
					if(Sessions != null)
					{
						for(int i = 0; i < Sessions.Length; i++)
						{
							if(Sessions[i] != null)
							{
								Sessions[i].Dispose();
								Sessions[i] = null;
							}
						}
					}
					Sessions = new IDataPage<T>[numberSessions];
				}
				return _count;
			}
			protected set
			{
				_count = value;
			}
		}

		/// <summary>
		/// Identifica se a coleção é somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Instancia do objeto de referencia utilizado na lista.
		/// </summary>
		public object ReferenceObject
		{
			get;
			set;
		}

		/// <summary>
		/// Tamanho da página de dados da lista.
		/// </summary>
		public int PageSize
		{
			get;
			protected set;
		}

		/// <summary>
		/// Carrega os dados.
		/// </summary>
		/// <param name="startRow"></param>
		/// <param name="pageSize"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		protected virtual VirtualListLoaderResult<T> Load(int startRow, int pageSize, int index)
		{
			var data = Loader.Load(this, startRow, pageSize, !InstanceInitialized, ReferenceObject);
			ClearPage(data);
			return data;
		}

		/// <summary>
		/// Limpa uma página.
		/// </summary>
		/// <param name="data"></param>
		protected virtual void ClearPage(VirtualListLoaderResult<T> data)
		{
			if(data == null)
				throw new InvalidOperationException("LoadResult cannot be null.");
			if((data.UpdateCount && (_count == 0 || _count != data.NewCount)) || !InstanceInitialized)
			{
				_count = data.NewCount;
				int numberSessions = PageSize == 0 || _count == 0 ? 1 : (int)Math.Ceiling(_count / (double)PageSize);
				if(Sessions != null)
				{
					for(int i = 0; i < Sessions.Length; i++)
					{
						if(Sessions[i] != null)
						{
							Sessions[i].Dispose();
							Sessions[i] = null;
						}
					}
				}
				Sessions = new IDataPage<T>[numberSessions];
				_version++;
				RaisePropertyChanged("Count");
				RaiseCollectionReset();
			}
		}

		/// <summary>
		/// Cria a páginda de dados.
		/// </summary>
		/// <param name="sessionIndex"></param>
		/// <returns></returns>
		protected virtual IDataPage<T> CreateDataPage(int sessionIndex)
		{
			return new DataPage2<T>();
		}

		/// <summary>
		/// Popula uma página.
		/// </summary>
		/// <param name="data">Dados a se popular.</param>
		/// <param name="index">Índice da página.</param>
		protected virtual void PopulatePage(VirtualListLoaderResult<T> data, int index)
		{
			ClearPage(data);
			int sessionIndex = PageSize == 0 ? 0 : (int)Math.Floor(index / (double)PageSize);
			var page = CreateDataPage(sessionIndex);
			page.Populate(data.Items);
			Sessions[sessionIndex] = page;
			OnDataPageLoaded(new DataPageLoadedEventArgs<T>(page));
		}

		/// <summary>
		/// Método acionado quando uma página de dados for carregada.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDataPageLoaded(DataPageLoadedEventArgs<T> e)
		{
			if(DataPageLoaded != null)
				DataPageLoaded(this, e);
		}

		/// <summary>
		/// Recupera o item na posição informada.
		/// </summary>
		/// <param name="index">Posição do item.</param>
		/// <returns></returns>
		internal protected virtual T GetItem(int index)
		{
			int sessionIndex = PageSize == 0 ? 0 : (int)Math.Floor(index / (double)PageSize);
			if(!InstanceInitialized)
			{
				InstanceInitialized = true;
				var loadResult = Load(sessionIndex * PageSize, PageSize, index);
				if(index >= _count)
					throw new ArgumentOutOfRangeException();
				var page = CreateDataPage(sessionIndex);
				page.Populate(loadResult.Items);
				Sessions[sessionIndex] = page;
				OnDataPageLoaded(new DataPageLoadedEventArgs<T>(page));
			}
			else
			{
				if(index >= _count)
					throw new ArgumentOutOfRangeException();
				if(Sessions[sessionIndex] == null)
				{
					var loadResult = Load(sessionIndex * PageSize, PageSize, index);
					var page = CreateDataPage(sessionIndex);
					page.Populate(loadResult.Items.ToList());
					Sessions[sessionIndex] = page;
					OnDataPageLoaded(new DataPageLoadedEventArgs<T>(page));
				}
			}
			return Sessions[sessionIndex][index - (sessionIndex * PageSize)];
		}

		/// <summary>
		/// Define o item na posição informada.
		/// </summary>
		/// <param name="index">Posição do item.</param>
		/// <param name="value">Valor do item que será definido.</param>
		internal protected virtual void SetItem(int index, T value)
		{
			if(index >= _count)
				throw new ArgumentOutOfRangeException();
			int indexSession = PageSize == 0 ? 0 : (int)Math.Floor(index / (double)PageSize);
			OnSet(index, GetItem(index), value);
			Sessions[indexSession][index - (indexSession * PageSize)] = value;
			this._version++;
			OnSetComplete(index, value);
		}

		/// <summary>
		/// Fires the collection reset event.
		/// </summary>
		protected void RaiseCollectionReset()
		{
			NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			NotifyCollectionChangedEventHandler h = CollectionChanged;
			if(h != null)
				h(this, e);
		}

		/// <summary>
		/// Fires the property changed event.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
			PropertyChangedEventHandler h = PropertyChanged;
			if(h != null)
				h(this, e);
		}

		/// <summary>
		/// Método acionado quando o valor de um item é definido.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		protected virtual void OnSet(int index, T oldValue, T newValue)
		{
		}

		/// <summary>
		/// Método acionado quando o set for completado.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="newValue"></param>
		protected virtual void OnSetComplete(int index, T newValue)
		{
		}

		/// <summary>
		/// Método acionado quando se dar um refresh na lista.
		/// </summary>
		protected virtual void OnRefresh()
		{
		}

		/// <summary>
		/// Limpa os itens da coleção.
		/// </summary>
		public void Clear()
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		/// <summary>
		/// Recupera uma lista assincrona com as referencia da
		/// lista virtual.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public AsyncVirtualList<T> ToAsyncList()
		{
			try
			{
				var result = new AsyncVirtualList<T>(PageSize, Loader, ReferenceObject);
				if(InstanceInitialized)
					result.SetInitialCount(_count);
				return result;
			}
			finally
			{
				_ignoreLoaderDispose = true;
			}
		}

		/// <summary>
		/// Atualiza os dados da lista.
		/// </summary>
		public void Refresh()
		{
			InstanceInitialized = false;
			OnRefresh();
		}

		/// <summary>
		/// Configura a coleção.
		/// </summary>
		/// <param name="pageSize">Tamanho da página de dados.</param>
		public void Configure(int pageSize)
		{
			if(PageSize != pageSize)
			{
				PageSize = pageSize;
				Refresh();
			}
		}

		/// <summary>
		/// Recupera o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public T this[int index]
		{
			get
			{
				return GetItem(index);
			}
			set
			{
				SetItem(index, value);
			}
		}

		/// <summary>
		/// Enumerator usado na lista.
		/// </summary>
		class Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			/// <summary>
			/// Lista em questão.
			/// </summary>
			private VirtualList<T> _list;

			/// <summary>
			/// Atual index.
			/// </summary>
			private int _index;

			/// <summary>
			/// Versão atual da lista
			/// </summary>
			private int _version;

			/// <summary>
			/// Objeto atualmente selecionado.
			/// </summary>
			private T _current;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="list"></param>
			internal Enumerator(VirtualList<T> list)
			{
				_list = list;
				_index = 0;
				_version = list._version;
				_current = default(T);
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
			}

			/// <summary>
			/// Movimenta para o proximo objeto.
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				if(_version != _list._version)
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				if(_index < _list.Count)
				{
					_current = _list[_index];
					_index++;
					return true;
				}
				_index = _list.Count + 1;
				_current = default(T);
				return false;
			}

			/// <summary>
			/// Atual objeto selecionado.
			/// </summary>
			public T Current
			{
				get
				{
					return _current;
				}
			}

			/// <summary>
			/// Atual objeto selecionado.
			/// </summary>
			object IEnumerator.Current
			{
				get
				{
					if((_index == 0) || (_index == (_list.Count + 1)))
						throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
					return this.Current;
				}
			}

			/// <summary>
			/// Reseta a lista.
			/// </summary>
			void IEnumerator.Reset()
			{
				if(this._version != this._list._version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				_index = 0;
				_current = default(T);
			}
		}

		/// <summary>
		/// Recupera o enumerator da lista.
		/// </summary>
		/// <returns></returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		/// <summary>
		/// Recupera o enumerador da lista.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		/// <summary>
		/// Copia os dados para a lista.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(Array array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException("array");
			for(int i = 0; i < array.Length - arrayIndex && i < this.Count; i++)
				array.SetValue(this[i], i + arrayIndex);
		}

		/// <summary>
		/// Identifica se é um lista sincronizada.
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Instancia do objeto es sincronização.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				if(_syncRoot == null)
				{
					System.Threading.Interlocked.CompareExchange(ref _syncRoot, new object(), null);
				}
				return _syncRoot;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		/// <summary>
		/// 
		/// </summary>
		void ICollection<T>.Clear()
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool ICollection<T>.Contains(T item)
		{
			return false;
		}

		/// <summary>
		/// Copia um Array de DataWrapper para a lista.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException("array");
			for(int i = 0; i < array.Length - arrayIndex && i < this.Count; i++)
				array[i + arrayIndex] = this[i];
		}

		/// <summary>
		/// Recupera a contagem.
		/// </summary>
		int ICollection<T>.Count
		{
			get
			{
				return this.Count;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		int IList<T>.IndexOf(T item)
		{
			return -1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		void IList<T>.Insert(int index, T item)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		/// <summary>
		/// Recupera um item da lista.
		/// </summary>
		/// <param name="index">Índice do item.</param>
		/// <returns></returns>
		T IList<T>.this[int index]
		{
			get
			{
				return GetItem(index);
			}
			set
			{
				SetItem(index, value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int IList.Add(object value)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		/// <summary>
		/// 
		/// </summary>
		void IList.Clear()
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		bool IList.Contains(object value)
		{
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int IList.IndexOf(object value)
		{
			return -1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		/// <summary>
		/// 
		/// </summary>
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		bool IList.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		void IList.Remove(object value)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		/// <summary>
		/// Recupera um item da lista.
		/// </summary>
		/// <param name="index">Índice do item.</param>
		/// <returns></returns>
		object IList.this[int index]
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

		/// <summary>
		/// Coia um array para lista.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		void ICollection.CopyTo(Array array, int index)
		{
			this.CopyTo(array, index);
		}

		/// <summary>
		/// Recupera a contagem de items.
		/// </summary>
		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		bool ICollection.IsSynchronized
		{
			get
			{
				return this.IsSynchronized;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		object ICollection.SyncRoot
		{
			get
			{
				return this.SyncRoot;
			}
		}

		void IObservableCollection.Move(int oldIndex, int newIndex)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		/// <summary>
		/// Identifica se possui registradores da carga de página de dados.
		/// </summary>
		bool IVirtualListResultProcessor<T>.HasDataPageLoadRegisters
		{
			get
			{
				return DataPageLoaded != null;
			}
		}

		/// <summary>
		/// Notifica que uma página de dados foi carregada.
		/// </summary>
		/// <param name="e"></param>
		void IVirtualListResultProcessor<T>.DataPageLoaded(DataPageLoadedEventArgs<T> e)
		{
			OnDataPageLoaded(e);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(!_ignoreLoaderDispose)
				Loader.Dispose();
			Loader = null;
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
