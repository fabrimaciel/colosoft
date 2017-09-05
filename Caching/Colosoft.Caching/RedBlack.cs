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
using Colosoft.Caching.Util;
using System.Collections;
using Colosoft.Globalization;

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Implementação de uma árvore 
	/// </summary>
	public class RedBlack
	{
		/// <summary>
		/// Possíveis tipos de comparações.
		/// </summary>
		public enum COMPARE
		{
			/// <summary>
			/// Igual.
			/// </summary>
			EQ,
			/// <summary>
			/// Diferente.
			/// </summary>
			NE,
			/// <summary>
			/// Menor que.
			/// </summary>
			LT,
			/// <summary>
			/// Maior que.
			/// </summary>
			GT,
			/// <summary>
			/// Menor igual.
			/// </summary>
			LTEQ,
			/// <summary>
			/// Maior igual.
			/// </summary>
			GTEQ,
			/// <summary>
			/// Expressão regular
			/// </summary>
			REGEX,
			/// <summary>
			/// Interseção com a expressão regular
			/// </summary>
			IREGEX
		}

		private string _cacheName;

		private bool _collision;

		private RedBlackNode _sentinelNode;

		/// <summary>
		/// Armazena a quantidade de itens na árvore.
		/// </summary>
		private int _size;

		private RedBlackNode _lastNodeFound;

		/// <summary>
		/// Nó principal da árvore.
		/// </summary>
		private RedBlackNode _rbTree;

		/// <summary>
		/// Identifica se a árvore está vazia.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return _rbTree == null;
			}
		}

		/// <summary>
		/// Maior chave a árvore.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public IComparable MaxKey
		{
			get
			{
				RedBlackNode rbTree = _rbTree;
				if(rbTree == null || rbTree == _sentinelNode)
					throw new RedBlackException("RedBlack tree is empty");
				while (rbTree.Right != _sentinelNode)
					rbTree = rbTree.Right;
				_lastNodeFound = rbTree;
				return rbTree.Key;
			}
		}

		/// <summary>
		/// Menor chave da árvore.
		/// </summary>
		public IComparable MinKey
		{
			get
			{
				RedBlackNode rbTree = _rbTree;
				if(rbTree != null && rbTree != _sentinelNode)
				{
					while (rbTree.Left != _sentinelNode)
						rbTree = rbTree.Left;
					_lastNodeFound = rbTree;
					return rbTree.Key;
				}
				return null;
			}
		}

		/// <summary>
		/// Nó sentinela.
		/// </summary>
		public RedBlackNode SentinelNode
		{
			get
			{
				return _sentinelNode;
			}
		}

		/// <summary>
		/// Tamanho da árvore.
		/// </summary>
		public int Size
		{
			get
			{
				return _size;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public RedBlack()
		{
			_sentinelNode = new RedBlackNode();
			_sentinelNode.Left = _sentinelNode.Right = _sentinelNode;
			_sentinelNode.Parent = null;
			_sentinelNode.Color = 1;
			_rbTree = _sentinelNode;
			_lastNodeFound = _sentinelNode;
		}

		/// <summary>
		/// Cria uma instancia com o nome do cache.
		/// </summary>
		/// <param name="cacheName">Nome do cache.</param>
		public RedBlack(string cacheName) : this()
		{
			_cacheName = cacheName;
		}

		/// <summary>
		/// Compara a instancia informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private static int Compare(IComparable x, IComparable y)
		{
			if(x is bool && y is int)
				y = (int)y != 0;
			var yIsNull = CacheNull.IsNull(y);
			var xIsNull = CacheNull.IsNull(x);
			if(xIsNull || yIsNull)
			{
				if(!xIsNull && yIsNull)
					return 1;
				else if(!yIsNull && xIsNull)
					return -1;
				return 0;
			}
			if(x is string || y is string)
			{
				return Culture.StringComparer.Compare(x is string ? (string)x : (x != null ? x.ToString() : null), y is string ? (string)y : (y != null ? y.ToString() : null));
			}
			if((x is int || x is short || x is long) && y != null && y.GetType().IsEnum)
				y = Convert.ToInt32(y);
			return x.CompareTo(y);
		}

		/// <summary>
		/// Restaura o nó depois de apagar.
		/// </summary>
		/// <param name="x">Instancia do nó que foi apagado</param>
		private void RestoreAfterDelete(RedBlackNode x)
		{
			while ((x != _rbTree) && (x.Color == 1))
			{
				RedBlackNode right;
				if(x == x.Parent.Left)
				{
					right = x.Parent.Right;
					if(right.Color == 0)
					{
						right.Color = 1;
						x.Parent.Color = 0;
						this.RotateLeft(x.Parent);
						right = x.Parent.Right;
					}
					if((right.Left.Color == 1) && (right.Right.Color == 1))
					{
						right.Color = 0;
						x = x.Parent;
					}
					else
					{
						if(right.Right.Color == 1)
						{
							right.Left.Color = 1;
							right.Color = 0;
							this.RotateRight(right);
							right = x.Parent.Right;
						}
						right.Color = x.Parent.Color;
						x.Parent.Color = 1;
						right.Right.Color = 1;
						this.RotateLeft(x.Parent);
						x = _rbTree;
					}
				}
				else
				{
					right = x.Parent.Left;
					if(right.Color == 0)
					{
						right.Color = 1;
						x.Parent.Color = 0;
						this.RotateRight(x.Parent);
						right = x.Parent.Left;
					}
					if((right.Right.Color == 1) && (right.Left.Color == 1))
					{
						right.Color = 0;
						x = x.Parent;
					}
					else
					{
						if(right.Left.Color == 1)
						{
							right.Right.Color = 1;
							right.Color = 0;
							this.RotateLeft(right);
							right = x.Parent.Left;
						}
						right.Color = x.Parent.Color;
						x.Parent.Color = 1;
						right.Left.Color = 1;
						this.RotateRight(x.Parent);
						x = _rbTree;
					}
				}
			}
			x.Color = 1;
		}

		/// <summary>
		/// Restaura o nó depois de inserir.
		/// </summary>
		/// <param name="x"></param>
		private void RestoreAfterInsert(RedBlackNode x)
		{
			while ((x != _rbTree) && (x.Parent.Color == 0))
			{
				RedBlackNode right;
				if(x.Parent == x.Parent.Parent.Left)
				{
					right = x.Parent.Parent.Right;
					if((right != null) && (right.Color == 0))
					{
						x.Parent.Color = 1;
						right.Color = 1;
						x.Parent.Parent.Color = 0;
						x = x.Parent.Parent;
					}
					else
					{
						if(x == x.Parent.Right)
						{
							x = x.Parent;
							this.RotateLeft(x);
						}
						x.Parent.Color = 1;
						x.Parent.Parent.Color = 0;
						this.RotateRight(x.Parent.Parent);
					}
				}
				else
				{
					right = x.Parent.Parent.Left;
					if((right != null) && (right.Color == 0))
					{
						x.Parent.Color = 1;
						right.Color = 1;
						x.Parent.Parent.Color = 0;
						x = x.Parent.Parent;
					}
					else
					{
						if(x == x.Parent.Left)
						{
							x = x.Parent;
							this.RotateRight(x);
						}
						x.Parent.Color = 1;
						x.Parent.Parent.Color = 0;
						this.RotateLeft(x.Parent.Parent);
					}
				}
			}
			_rbTree.Color = 1;
		}

		/// <summary>
		/// Apaga o nó informado.
		/// </summary>
		/// <param name="z">Instancia do nó que será removido.</param>
		private void Delete(RedBlackNode z)
		{
			RedBlackNode right;
			RedBlackNode x = new RedBlackNode();
			if(z.Left == _sentinelNode || z.Right == _sentinelNode)
				right = z;
			else
			{
				right = z.Right;
				while (right.Left != _sentinelNode)
					right = right.Left;
			}
			if(right.Left != _sentinelNode)
				x = right.Left;
			else
				x = right.Right;
			x.Parent = right.Parent;
			if(right.Parent != null)
			{
				if(right == right.Parent.Left)
					right.Parent.Left = x;
				else
					right.Parent.Right = x;
			}
			else
				_rbTree = x;
			if(right != z)
			{
				z.Key = right.Key;
				z.Data = right.Data;
			}
			if(right.Color == 1)
				RestoreAfterDelete(x);
			_lastNodeFound = _sentinelNode;
		}

		/// <summary>
		/// Adiciona um novo item para a árvore.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		public void Add(IComparable key, object data)
		{
			if(key == null || data == null)
				throw new RedBlackException("RedBlackNode key and data must not be null");
			int num = 0;
			RedBlackNode x = new RedBlackNode();
			RedBlackNode rbTree = _rbTree;
			while (rbTree != _sentinelNode)
			{
				x.Parent = rbTree;
				num = Compare(key, rbTree.Key);
				if(num == 0)
				{
					_collision = true;
					break;
				}
				if(num > 0)
				{
					rbTree = rbTree.Right;
					_collision = false;
				}
				else
				{
					rbTree = rbTree.Left;
					_collision = false;
				}
			}
			if(_collision)
				rbTree.Data[data] = null;
			else
			{
				x.Key = key;
				x.Data.Add(data, null);
				x.Left = _sentinelNode;
				x.Right = _sentinelNode;
				if(x.Parent != null)
				{
					if(Compare(x.Key, x.Parent.Key) > 0)
						x.Parent.Right = x;
					else
						x.Parent.Left = x;
				}
				else
					_rbTree = x;
				this.RestoreAfterInsert(x);
				_lastNodeFound = x;
				_size++;
			}
		}

		/// <summary>
		/// Limpa todos os nós da árvore.
		/// </summary>
		public void Clear()
		{
			_rbTree = _sentinelNode;
			_size = 0;
			_collision = false;
		}

		/// <summary>
		/// Verifica se na árvore contem algum nó como a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Contains(IComparable key)
		{
			RedBlackNode rbTree = _rbTree;
			while (rbTree != _sentinelNode)
			{
				int num = Compare(rbTree.Key, key);
				if(num == 0)
				{
					_lastNodeFound = rbTree;
					return true;
				}
				if(num > 0)
					rbTree = rbTree.Left;
				else
					rbTree = rbTree.Right;
			}
			return false;
		}

		/// <summary>
		/// Recupera o enumerador para os elementos da árvore.
		/// </summary>
		/// <returns></returns>
		public RedBlackEnumerator Elements()
		{
			return this.Elements(true);
		}

		/// <summary>
		/// Recupera o enumerado para os elementos da árvore.
		/// </summary>
		/// <param name="ascending">True para recupera em ordem ascendente.</param>
		/// <returns></returns>
		public RedBlackEnumerator Elements(bool ascending)
		{
			return new RedBlackEnumerator(_rbTree, ascending, _sentinelNode);
		}

		/// <summary>
		/// Recupera um dado da árvore pela chave informada.
		/// </summary>
		/// <param name="key">Chave que será comparada.</param>
		/// <param name="compareType">Tipo de comparação.</param>
		/// <returns></returns>
		public object GetData(IComparable key, COMPARE compareType)
		{
			string str;
			WildcardEnabledRegex regex;
			ArrayList list = new ArrayList();
			RedBlackNode rbTree = _rbTree;
			IDictionaryEnumerator enumerator = this.GetEnumerator();
			Hashtable result = null;
			Hashtable hashtable2 = null;
			switch(compareType)
			{
			case COMPARE.EQ:
				while (rbTree != _sentinelNode)
				{
					int num = Compare(rbTree.Key, key);
					if(num == 0)
					{
						_lastNodeFound = rbTree;
						list.AddRange(rbTree.Data.Keys);
						return list;
					}
					if(num > 0)
						rbTree = rbTree.Left;
					else
						rbTree = rbTree.Right;
				}
				return list;
			case COMPARE.NE:
				result = new Hashtable();
				while (enumerator.MoveNext())
				{
					if(Compare(((IComparable)enumerator.Key), key) != 0)
					{
						IDictionaryEnumerator enumerator2 = (enumerator.Value as Hashtable).GetEnumerator();
						while (enumerator2.MoveNext())
							result[enumerator2.Key] = enumerator2.Value;
					}
				}
				return new ArrayList(result.Keys);
			case COMPARE.LT:
				result = new Hashtable();
				while (enumerator.MoveNext())
				{
					if(Compare(((IComparable)enumerator.Key), key) < 0)
					{
						IDictionaryEnumerator enumerator4 = (enumerator.Value as Hashtable).GetEnumerator();
						while (enumerator4.MoveNext())
							result[enumerator4.Key] = enumerator4.Value;
					}
				}
				return new ArrayList(result.Keys);
			case COMPARE.GT:
				result = new Hashtable();
				while (enumerator.MoveNext())
				{
					if(Compare(((IComparable)enumerator.Key), key) > 0)
					{
						IDictionaryEnumerator enumerator3 = (enumerator.Value as Hashtable).GetEnumerator();
						while (enumerator3.MoveNext())
							result[enumerator3.Key] = enumerator3.Value;
					}
				}
				return new ArrayList(result.Keys);
			case COMPARE.LTEQ:
				result = new Hashtable();
				while (enumerator.MoveNext())
				{
					if(Compare(((IComparable)enumerator.Key), key) > 0)
						break;
					IDictionaryEnumerator enumerator6 = (enumerator.Value as Hashtable).GetEnumerator();
					while (enumerator6.MoveNext())
						result[enumerator6.Key] = enumerator6.Value;
				}
				break;
			case COMPARE.GTEQ:
				result = new Hashtable();
				while (enumerator.MoveNext())
				{
					if(Compare(((IComparable)enumerator.Key), key) >= 0)
					{
						IDictionaryEnumerator enumerator5 = (enumerator.Value as Hashtable).GetEnumerator();
						while (enumerator5.MoveNext())
							result[enumerator5.Key] = enumerator5.Value;
					}
				}
				return new ArrayList(result.Keys);
			case COMPARE.REGEX:
				result = new Hashtable();
				str = key as string;
				regex = new WildcardEnabledRegex(str);
				while (enumerator.MoveNext())
				{
					if((enumerator.Key is string) && regex.IsMatch((string)enumerator.Key))
					{
						IDictionaryEnumerator enumerator7 = (enumerator.Value as Hashtable).GetEnumerator();
						while (enumerator7.MoveNext())
							result[enumerator7.Key] = enumerator7.Value;
					}
				}
				return new ArrayList(result.Keys);
			case COMPARE.IREGEX:
			{
				result = new Hashtable();
				str = key as string;
				regex = new WildcardEnabledRegex(str);
				hashtable2 = new Hashtable();
				while (enumerator.MoveNext())
				{
					if(enumerator.Key is string)
					{
						if(regex.IsMatch((string)enumerator.Key))
						{
							IDictionaryEnumerator enumerator8 = (enumerator.Value as Hashtable).GetEnumerator();
							while (enumerator8.MoveNext())
								hashtable2[enumerator8.Key] = enumerator8.Value;
						}
						else
						{
							IDictionaryEnumerator enumerator9 = (enumerator.Value as Hashtable).GetEnumerator();
							while (enumerator9.MoveNext())
								result[enumerator9.Key] = enumerator9.Value;
						}
					}
				}
				ArrayList list2 = new ArrayList(result.Keys);
				for(int i = list2.Count - 1; i >= 0; i--)
					if(hashtable2.ContainsKey(list2[i]))
						list2.RemoveAt(i);
				return list2;
			}
			default:
				return list;
			}
			return new ArrayList(result.Keys);
		}

		/// <summary>
		/// Recupera o enumerador da arvore.
		/// </summary>
		/// <returns></returns>
		public RedBlackEnumerator GetEnumerator()
		{
			return this.Elements(true);
		}

		/// <summary>
		/// Recupera as chaves da árvore.
		/// </summary>
		/// <returns></returns>
		public RedBlackEnumerator Keys()
		{
			return this.Keys(true);
		}

		/// <summary>
		/// Recupera as chaves da árvore na ordem informada.
		/// </summary>
		/// <param name="ascending">True se é para recupera as chaves em ordem ascedente.</param>
		/// <returns></returns>
		public RedBlackEnumerator Keys(bool ascending)
		{
			return new RedBlackEnumerator(_rbTree, ascending, _sentinelNode);
		}

		/// <summary>
		/// Remove o nó com a chave informada.
		/// </summary>
		/// <param name="indexKey">Chave que será pesquisada para ser removida.</param>
		public void Remove(IComparable indexKey)
		{
			this.Remove(indexKey, null);
		}

		/// <summary>
		/// Remove o nó com a chave informada.
		/// </summary>
		/// <param name="indexKey">Chave associada com o nó.</param>
		/// <param name="cacheKey">Chave do cache.</param>
		public void Remove(IComparable indexKey, object cacheKey)
		{
			bool flag = false;
			if(indexKey == null)
				throw new RedBlackException("RedBlackNode key is null");
			try
			{
				RedBlackNode lastNodeFound;
				if(Compare(indexKey, _lastNodeFound.Key) == 0)
					lastNodeFound = _lastNodeFound;
				else
				{
					lastNodeFound = _rbTree;
					while (lastNodeFound != _sentinelNode)
					{
						int num = Compare(indexKey, lastNodeFound.Key);
						if(num == 0)
							break;
						if(num < 0)
							lastNodeFound = lastNodeFound.Left;
						else
							lastNodeFound = lastNodeFound.Right;
					}
					if(lastNodeFound == _sentinelNode)
						return;
				}
				try
				{
					if(cacheKey != null)
					{
						if(lastNodeFound.Data.Contains(cacheKey))
						{
							lastNodeFound.Data.Remove(cacheKey);
							flag = false;
						}
					}
					else
					{
					}
				}
				catch(Exception)
				{
					return;
				}
			}
			catch(Exception)
			{
				throw;
			}
			if(flag)
				_size--;
		}

		/// <summary>
		/// Remove o maior nó.
		/// </summary>
		public void RemoveMax()
		{
			if(_rbTree == null)
				throw new RedBlackException("RedBlackNode is null");
			this.Remove(this.MaxKey);
		}

		/// <summary>
		/// Remove o menor nó.
		/// </summary>
		public void RemoveMin()
		{
			if(_rbTree == null)
				throw new RedBlackException("RedBlackNode is null");
			this.Remove(this.MinKey);
		}

		/// <summary>
		/// Roda o nó para a esquerda.
		/// </summary>
		/// <param name="x"></param>
		public void RotateLeft(RedBlackNode x)
		{
			RedBlackNode right = x.Right;
			x.Right = right.Left;
			if(right.Left != _sentinelNode)
				right.Left.Parent = x;
			if(right != _sentinelNode)
				right.Parent = x.Parent;
			if(x.Parent != null)
			{
				if(x == x.Parent.Left)
					x.Parent.Left = right;
				else
					x.Parent.Right = right;
			}
			else
				_rbTree = right;
			right.Left = x;
			if(x != _sentinelNode)
				x.Parent = right;
		}

		/// <summary>
		/// Roda o nó para a direita.
		/// </summary>
		/// <param name="x"></param>
		public void RotateRight(RedBlackNode x)
		{
			RedBlackNode left = x.Left;
			x.Left = left.Right;
			if(left.Right != _sentinelNode)
			{
				left.Right.Parent = x;
			}
			if(left != _sentinelNode)
			{
				left.Parent = x.Parent;
			}
			if(x.Parent != null)
			{
				if(x == x.Parent.Right)
				{
					x.Parent.Right = left;
				}
				else
				{
					x.Parent.Left = left;
				}
			}
			else
			{
				_rbTree = left;
			}
			left.Right = x;
			if(x != _sentinelNode)
			{
				x.Parent = left;
			}
		}

		/// <summary>
		/// Compara a instancia com o objeto informado.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return (((obj != null) && (obj is RedBlackNode)) && ((this == obj) || this.ToString().Equals(((RedBlackNode)obj).ToString())));
		}

		/// <summary>
		/// Hashcode da instancia.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return 0;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("Size: {0}", Size);
		}
	}
}
