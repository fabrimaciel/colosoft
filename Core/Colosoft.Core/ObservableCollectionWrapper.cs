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

namespace Colosoft.Collections
{
	/// <summary>
	/// Implementação do wrapper de uma coleção observada.
	/// </summary>
	/// <typeparam name="TDestination"></typeparam>
	[Serializable, System.Diagnostics.DebuggerTypeProxy(typeof(ObservableCollectionDebugView<>)), System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public class ObservableCollectionWrapper<TDestination> : IObservableCollection<TDestination>, IList
	{
		private System.Collections.IList _sourceList;

		private IObservableCollection _source;

		/// <summary>
		/// Evento acionado quando a coleção for alterada.
		/// </summary>
		public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged {
			add
			{
				_source.CollectionChanged += value;
			}
			remove {
				_source.CollectionChanged -= value;
			}
		}

		/// <summary>
		/// Evento acionado quando uma propriedade for alterada.
		/// </summary>
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged {
			add
			{
				_source.PropertyChanged += value;
			}
			remove {
				_source.PropertyChanged -= value;
			}
		}

		/// <summary>
		/// Recupera e define o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public TDestination this[int index]
		{
			get
			{
				return (TDestination)_sourceList[index];
			}
			set
			{
				_sourceList[index] = value;
			}
		}

		/// <summary>
		/// Quantidade de itens na coleção.
		/// </summary>
		public int Count
		{
			get
			{
				return _sourceList.Count;
			}
		}

		/// <summary>
		/// Identifica se a coleção é readonly.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return _sourceList.IsReadOnly;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="source"></param>
		public ObservableCollectionWrapper(IObservableCollection source)
		{
			source.Require("source").NotNull();
			_source = source;
			_sourceList = (System.Collections.IList)source;
		}

		/// <summary>
		/// Move o item de uma posição para outra.
		/// </summary>
		/// <param name="oldIndex">Indice antigo</param>
		/// <param name="newIndex">Novo indice.</param>
		public void Move(int oldIndex, int newIndex)
		{
			_source.Move(oldIndex, newIndex);
		}

		/// <summary>
		/// Recupera o indice do item na coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int IndexOf(TDestination item)
		{
			return _sourceList.IndexOf(item);
		}

		/// <summary>
		/// Insere o item na coleção.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		public void Insert(int index, TDestination item)
		{
			_sourceList.Insert(index, item);
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			_sourceList.RemoveAt(index);
		}

		/// <summary>
		/// Adiciona o item na coleção.
		/// </summary>
		/// <param name="item"></param>
		public void Add(TDestination item)
		{
			_sourceList.Add(item);
		}

		/// <summary>
		/// Limpa a coleção.
		/// </summary>
		public void Clear()
		{
			_sourceList.Clear();
		}

		/// <summary>
		/// Verifica se o item está contido na coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(TDestination item)
		{
			return _sourceList.Contains(item);
		}

		/// <summary>
		/// Copia os itens para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(TDestination[] array, int arrayIndex)
		{
			array.Require("array").NotNull();
			var aux = new object[array.Length - arrayIndex];
			_sourceList.CopyTo(aux, 0);
			for(var i = 0; i < aux.Length; i++)
				array[i + arrayIndex] = (TDestination)aux[i];
		}

		/// <summary>
		/// Remove o item da coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(TDestination item)
		{
			var count = Count;
			_sourceList.Remove(item);
			return Count < count;
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<TDestination> GetEnumerator()
		{
			return new Enumerator(_source.GetEnumerator());
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new Enumerator(_source.GetEnumerator());
		}

		class Enumerator : IEnumerator<TDestination>
		{
			private System.Collections.IEnumerator _enumerator;

			public TDestination Current
			{
				get
				{
					return (TDestination)_enumerator.Current;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="enumerator"></param>
			public Enumerator(System.Collections.IEnumerator enumerator)
			{
				_enumerator = enumerator;
			}

			public void Dispose()
			{
				if(_enumerator is IDisposable)
					((IDisposable)_enumerator).Dispose();
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return _enumerator.Current;
				}
			}

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}
		}

		/// <summary>
		/// Adiciona o valor informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int IList.Add(object value)
		{
			return _sourceList.Add(value);
		}

		/// <summary>
		/// Verifica se na coleção existe o valor informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		bool IList.Contains(object value)
		{
			return _sourceList.Contains(value);
		}

		/// <summary>
		/// Recupera o indice do valor na coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int IList.IndexOf(object value)
		{
			return _sourceList.IndexOf(value);
		}

		/// <summary>
		/// Insere o valor na coleção.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		void IList.Insert(int index, object value)
		{
			_sourceList.Insert(index, value);
		}

		/// <summary>
		/// Identifica se possui um tamanho fixo.
		/// </summary>
		bool IList.IsFixedSize
		{
			get
			{
				return _sourceList.IsFixedSize;
			}
		}

		/// <summary>
		/// Identifica se é somente leitura.
		/// </summary>
		bool IList.IsReadOnly
		{
			get
			{
				return _sourceList.IsReadOnly;
			}
		}

		/// <summary>
		/// Remove o valor informado.
		/// </summary>
		/// <param name="value"></param>
		void IList.Remove(object value)
		{
			_sourceList.Remove(value);
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		void IList.RemoveAt(int index)
		{
			_sourceList.RemoveAt(index);
		}

		/// <summary>
		/// Recupera e define a instancia do item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		object IList.this[int index]
		{
			get
			{
				return _sourceList[index];
			}
			set
			{
				_sourceList[index] = value;
			}
		}

		/// <summary>
		/// Copia os itens para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		void ICollection.CopyTo(Array array, int index)
		{
			_sourceList.CopyTo(array, index);
		}

		/// <summary>
		/// Identifica se é uma instancia sincronizada.
		/// </summary>
		bool ICollection.IsSynchronized
		{
			get
			{
				return _sourceList.IsSynchronized;
			}
		}

		/// <summary>
		/// Objeto de sincronização.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return _sourceList.SyncRoot;
			}
		}
	}
}
