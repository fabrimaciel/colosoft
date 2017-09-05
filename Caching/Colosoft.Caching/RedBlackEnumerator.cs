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

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Implementa do enumerador para pecorrer a arvore red-black.
	/// </summary>
	public sealed class RedBlackEnumerator : IDictionaryEnumerator, IEnumerator
	{
		private RedBlackNode _sentinelNode;

		/// <summary>
		/// Identifica que está trabalhando em orden ascendente.
		/// </summary>
		private bool _ascending;

		private object _objValue;

		private IComparable _ordKey;

		private Stack<RedBlackNode> _stack;

		/// <summary>
		/// Instancia da entrada de um dicionário.
		/// </summary>
		DictionaryEntry IDictionaryEnumerator.Entry
		{
			get
			{
				return new DictionaryEntry();
			}
		}

		/// <summary>
		/// Chave do atual item.
		/// </summary>
		object IDictionaryEnumerator.Key
		{
			get
			{
				return _ordKey;
			}
		}

		/// <summary>
		/// Valor do atual item.
		/// </summary>
		object IDictionaryEnumerator.Value
		{
			get
			{
				return _objValue;
			}
		}

		/// <summary>
		/// Recupera o atual item.
		/// </summary>
		object IEnumerator.Current
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public RedBlackEnumerator()
		{
		}

		/// <summary>
		/// Cria a instancia com os parametros necessários.
		/// </summary>
		/// <param name="tnode">Nó terminal.</param>
		/// <param name="ascending">True para pecorre em ordem ascendente.</param>
		/// <param name="sentinelNode">Nó sentinela.</param>
		public RedBlackEnumerator(RedBlackNode tnode, bool ascending, RedBlackNode sentinelNode)
		{
			_stack = new Stack<RedBlackNode>();
			_ascending = ascending;
			_sentinelNode = sentinelNode;
			if(ascending)
			{
				while (tnode != _sentinelNode)
				{
					_stack.Push(tnode);
					tnode = tnode.Left;
				}
			}
			else
			{
				while (tnode != _sentinelNode)
				{
					_stack.Push(tnode);
					tnode = tnode.Right;
				}
			}
		}

		/// <summary>
		/// Verifcica se existem mais elementos para ser pecorridos.
		/// </summary>
		/// <returns></returns>
		public bool HasMoreElements()
		{
			return (_stack != null && _stack.Count > 0);
		}

		/// <summary>
		/// Movimenta para o próximo elemento.
		/// </summary>
		/// <returns></returns>
		public bool MoveNext()
		{
			if(this.HasMoreElements())
			{
				this.NextElement();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Recupera o próximo elemento.
		/// </summary>
		/// <returns></returns>
		public object NextElement()
		{
			if(_stack.Count == 0)
				throw new RedBlackException("Element not found");
			RedBlackNode node = _stack.Peek();
			if(_ascending)
			{
				if(node.Right == _sentinelNode)
				{
					for(RedBlackNode node2 = _stack.Pop(); HasMoreElements() && (_stack.Peek().Right == node2); node2 = _stack.Pop())
						;
				}
				else
				{
					for(RedBlackNode node3 = node.Right; node3 != _sentinelNode; node3 = node3.Left)
						_stack.Push(node3);
				}
			}
			else if(node.Left == _sentinelNode)
			{
				for(RedBlackNode node4 = _stack.Pop(); this.HasMoreElements() && (_stack.Peek().Left == node4); node4 = _stack.Pop())
					;
			}
			else
			{
				for(RedBlackNode node5 = node.Left; node5 != _sentinelNode; node5 = node5.Right)
					_stack.Push(node5);
			}
			_ordKey = node.Key;
			_objValue = node.Data;
			return node.Key;
		}

		/// <summary>
		/// Resenta o enumerador.
		/// </summary>
		void IEnumerator.Reset()
		{
		}
	}
}
