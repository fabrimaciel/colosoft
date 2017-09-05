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
	/// Implementação de uma Sphere/Rectangle-tree.
	/// </summary>
	public class SRTree : ICloneable
	{
		private ArrayList _leftList = new ArrayList();

		private ArrayList _rightList = new ArrayList();

		/// <summary>
		/// Lista dos nós da esquerda.
		/// </summary>
		public ArrayList LeftList
		{
			get
			{
				return _leftList;
			}
			set
			{
				if(value != null)
					_leftList = value;
			}
		}

		/// <summary>
		/// Lista dos nós da direita.
		/// </summary>
		public ArrayList RightList
		{
			get
			{
				return _rightList;
			}
			set
			{
				if(value != null)
					_rightList = value;
			}
		}

		/// <summary>
		/// Realiza o um merge com a árvore informada.
		/// </summary>
		/// <param name="tree"></param>
		public void Merge(SRTree tree)
		{
			if(tree == null)
				tree = new SRTree();
			if(tree.RightList == null)
				tree.RightList = new ArrayList();
			if(this.RightList != null)
			{
				IEnumerator enumerator = this.RightList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if(!tree.RightList.Contains(enumerator.Current))
						tree.RightList.Add(enumerator.Current);
				}
			}
		}

		/// <summary>
		/// Popula a árvore com o enumerador informado.
		/// </summary>
		/// <param name="e"></param>
		public void Populate(IDictionaryEnumerator e)
		{
			if(e != null)
			{
				if(_rightList == null)
					_rightList = new ArrayList();
				if(!(e is RedBlackEnumerator))
				{
					while (e.MoveNext())
						_rightList.Add(e.Key);
				}
				else
				{
					while (e.MoveNext())
					{
						Hashtable hashtable = e.Value as Hashtable;
						_rightList.AddRange(hashtable.Keys);
					}
				}
			}
		}

		/// <summary>
		/// Cria um clone com os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			SRTree tree = new SRTree();
			if(this.LeftList != null)
				tree.LeftList = this.LeftList.Clone() as ArrayList;
			if(this.RightList != null)
				tree.RightList = this.RightList.Clone() as ArrayList;
			return tree;
		}

		/// <summary>
		/// Recupera o enumerador da árvore.
		/// </summary>
		/// <returns></returns>
		public SRTreeEnumerator GetEnumerator()
		{
			if(_leftList != null)
				return new SRTreeEnumerator(_leftList);
			return null;
		}

		/// <summary>
		/// Realiza a redução da árvore.
		/// </summary>
		public void Reduce()
		{
			_leftList = _rightList.Clone() as ArrayList;
			_rightList.Clear();
		}

		/// <summary>
		/// Chave um shift da chave informada.
		/// </summary>
		/// <param name="key"></param>
		public void Shift(object key)
		{
			if(_leftList.Contains(key))
			{
				if(_rightList == null)
					_rightList = new ArrayList();
				_leftList.Remove(key);
				_rightList.Add(key);
			}
		}
	}
}
