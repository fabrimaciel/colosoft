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

namespace Colosoft.DataStructures
{
	/// <summary>
	/// Implementação da fila de prioridade binária.
	/// </summary>
	public class BinaryPriorityQueue : IPriorityQueue, ICloneable, IList, ICollection, IEnumerable
	{
		/// <summary>
		/// Comparador da instancia.
		/// </summary>
		protected IComparer _comparer;

		/// <summary>
		/// Lista interna.
		/// </summary>
		protected ArrayList _innerList;

		/// <summary>
		/// Quantidade de itens na fila.
		/// </summary>
		public int Count
		{
			get
			{
				return _innerList.Count;
			}
		}

		/// <summary>
		/// Identifica se a instancia é sincronizada.
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				return _innerList.IsSynchronized;
			}
		}

		/// <summary>
		/// Instancia de sincronização.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public BinaryPriorityQueue() : this(Comparer.Default)
		{
		}

		/// <summary>
		/// Cria a instancia com o comparador informado.
		/// </summary>
		/// <param name="comparer">Comparador que será usado.</param>
		public BinaryPriorityQueue(IComparer comparer)
		{
			_innerList = new ArrayList();
			_comparer = comparer;
		}

		/// <summary>
		/// Cria a instancia informando a capacidade inicial da fila.
		/// </summary>
		/// <param name="capacity">Capacidade inicial da fila.</param>
		public BinaryPriorityQueue(int capacity) : this(Comparer.Default, capacity)
		{
		}

		/// <summary>
		/// Cria uma instancia informada o comparador e a capacidade inicial da fila.
		/// </summary>
		/// <param name="comparer">Comparador que será usado.</param>
		/// <param name="capacity">Capacidade inicial da fila.</param>
		public BinaryPriorityQueue(IComparer comparer, int capacity)
		{
			_innerList = new ArrayList();
			_comparer = comparer;
			_innerList.Capacity = capacity;
		}

		/// <summary>
		/// Cria a instancia já com s lista dos itens.
		/// </summary>
		/// <param name="innerList"></param>
		/// <param name="comparer"></param>
		/// <param name="copy">True se deverá ser feita uma cópia da lista.</param>
		protected BinaryPriorityQueue(ArrayList innerList, IComparer comparer, bool copy)
		{
			_innerList = new ArrayList();
			if(copy)
				_innerList = innerList.Clone() as ArrayList;
			else
				_innerList = innerList;
			_comparer = comparer;
		}

		/// <summary>
		/// Recupera a instancia do primeiro item da fila.
		/// </summary>
		/// <returns></returns>
		public virtual object Peek()
		{
			if(_innerList.Count > 0)
				return _innerList[0];
			return null;
		}

		/// <summary>
		/// Remove o primeiro item da fila.
		/// </summary>
		/// <returns></returns>
		public virtual object Pop()
		{
			int num4;
			object obj2 = _innerList[0];
			int i = 0;
			_innerList[0] = _innerList[_innerList.Count - 1];
			_innerList.RemoveAt(_innerList.Count - 1);
			do
			{
				num4 = i;
				int j = (2 * i) + 1;
				int num3 = (2 * i) + 2;
				if((_innerList.Count > j) && (this.OnCompare(i, j) > 0))
					i = j;
				if((_innerList.Count > num3) && (this.OnCompare(i, num3) > 0))
					i = num3;
				if(i != num4)
					this.SwitchElements(i, num4);
			}
			while (i != num4);
			return obj2;
		}

		/// <summary>
		/// Adiciona um novo item na fila.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public virtual int Push(object value)
		{
			int count = _innerList.Count;
			_innerList.Add(value);
			while (count != 0)
			{
				int j = (count - 1) / 2;
				if(this.OnCompare(count, j) >= 0)
					return count;
				this.SwitchElements(count, j);
				count = j;
			}
			return count;
		}

		/// <summary>
		/// Limpa todos os dados da fila.
		/// </summary>
		public virtual void Clear()
		{
			_innerList.Clear();
		}

		/// <summary>
		/// Cria um clone da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new BinaryPriorityQueue(_innerList, _comparer, true);
		}

		/// <summary>
		/// Verifica se na fila existe o valor informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(object value)
		{
			return _innerList.Contains(value);
		}

		/// <summary>
		/// Copia os valores para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array, int index)
		{
			_innerList.CopyTo(array, index);
		}

		/// <summary>
		/// Recupera um cópid de somente leitura.
		/// </summary>
		/// <param name="queue"></param>
		/// <returns></returns>
		public static BinaryPriorityQueue ReadOnly(BinaryPriorityQueue queue)
		{
			return new BinaryPriorityQueue(ArrayList.ReadOnly(queue._innerList), queue._comparer, false);
		}

		/// <summary>
		/// Sincroniza a fila.
		/// </summary>
		/// <param name="queue"></param>
		/// <returns></returns>
		public static BinaryPriorityQueue Syncronized(BinaryPriorityQueue queue)
		{
			return new BinaryPriorityQueue(ArrayList.Synchronized(queue._innerList), queue._comparer, false);
		}

		/// <summary>
		/// Atualiza o item na posição informada.
		/// </summary>
		/// <param name="i"></param>
		public void Update(int i)
		{
			int num4;
			int num = i;
			while (true)
			{
				if(num == 0)
					break;
				num4 = (num - 1) / 2;
				if(this.OnCompare(num, num4) >= 0)
					break;
				this.SwitchElements(num, num4);
				num = num4;
			}
			if(num < i)
				return;
			while (true)
			{
				int j = num;
				int num3 = (2 * num) + 1;
				num4 = (2 * num) + 2;
				if((_innerList.Count > num3) && (this.OnCompare(num, num3) > 0))
					num = num3;
				if((_innerList.Count > num4) && (this.OnCompare(num, num4) > 0))
					num = num4;
				if(num == j)
					return;
				this.SwitchElements(num, j);
			}
		}

		/// <summary>
		/// Compara os elementos nas posições informadas.
		/// </summary>
		/// <param name="position1">Posição do primeiro elemento.</param>
		/// <param name="position2">Posição do segundo elemento.</param>
		/// <returns></returns>
		protected virtual int OnCompare(int position1, int position2)
		{
			return _comparer.Compare(_innerList[position1], _innerList[position2]);
		}

		/// <summary>
		/// Troca os elementos nas posição informadas.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		protected void SwitchElements(int i, int j)
		{
			object obj2 = _innerList[i];
			_innerList[i] = _innerList[j];
			_innerList[j] = obj2;
		}

		/// <summary>
		/// Recupera o enumerador da instancia.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		/// <summary>
		/// Adiciona um novo item.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		int IList.Add(object o)
		{
			return this.Push(o);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		int IList.IndexOf(object value)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Verifica se possui o tamanho fixo.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Verificva se é somente leitura.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Recupera o item pela posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		object IList.this[int index]
		{
			get
			{
				return _innerList[index];
			}
			set
			{
				_innerList[index] = value;
				this.Update(index);
			}
		}
	}
}
