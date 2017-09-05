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
using Colosoft.Caching.Data;
using System.Collections;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Implementação de armazenamento de indice utilizando o
	/// algoritmo de Red-Black Tree.
	/// </summary>
	public class RBStore : IIndexStore
	{
		/// <summary>
		/// Instancia da arvore RedBlack associada.
		/// </summary>
		private RedBlack _rbTree;

		/// <summary>
		/// Quantidade de itens armazenados.
		/// </summary>
		public int Size
		{
			get
			{
				if(_rbTree == null)
					return 0;
				return _rbTree.Size;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cacheName">Nome do cache.</param>
		public RBStore(string cacheName)
		{
			_rbTree = new RedBlack(cacheName);
		}

		/// <summary>
		/// Adiciona um novo item para ser armazenado.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Add(object key, object value)
		{
			var key2 = key as IComparable;
			if(_rbTree != null)
				_rbTree.Add(key2, value);
		}

		/// <summary>
		/// Remove o indice informado.
		/// </summary>
		/// <param name="key">Chave do indice.</param>
		/// <param name="value">Valor do indice.</param>
		public void Remove(object key, object value)
		{
			var key2 = key as IComparable;
			if(_rbTree != null && key2 != null)
				_rbTree.Remove(key2, value);
		}

		/// <summary>
		/// Limpa os dados armazenados.
		/// </summary>
		public void Clear()
		{
			if(_rbTree != null)
				_rbTree.Clear();
		}

		/// <summary>
		/// Recupera o indice com base na chave informada.
		/// </summary>
		/// <param name="key">Chave que será pesquisada.</param>
		/// <param name="comparisonType">Tipo de comparação.</param>
		/// <returns></returns>
		public ArrayList GetData(object key, ComparisonType comparisonType)
		{
			if(key == null)
				key = CacheNull.Value;
			RedBlack.COMPARE eQ = RedBlack.COMPARE.EQ;
			ArrayList list = new ArrayList();
			if(_rbTree == null)
				return list;
			switch(comparisonType)
			{
			case ComparisonType.EQUALS:
				eQ = RedBlack.COMPARE.EQ;
				break;
			case ComparisonType.NOT_EQUALS:
				eQ = RedBlack.COMPARE.NE;
				break;
			case ComparisonType.LESS_THAN:
				eQ = RedBlack.COMPARE.LT;
				break;
			case ComparisonType.GREATER_THAN:
				eQ = RedBlack.COMPARE.GT;
				break;
			case ComparisonType.LESS_THAN_EQUALS:
				eQ = RedBlack.COMPARE.LTEQ;
				break;
			case ComparisonType.GREATER_THAN_EQUALS:
				eQ = RedBlack.COMPARE.GTEQ;
				break;
			case ComparisonType.LIKE:
				eQ = RedBlack.COMPARE.REGEX;
				break;
			case ComparisonType.NOT_LIKE:
				eQ = RedBlack.COMPARE.IREGEX;
				break;
			}
			return (_rbTree.GetData(key as IComparable, eQ) as ArrayList);
		}

		/// <summary>
		/// Recupera o enumerado para pecorre os itens armazenados.
		/// </summary>
		/// <returns></returns>
		public IDictionaryEnumerator GetEnumerator()
		{
			if(_rbTree != null)
				return _rbTree.GetEnumerator();
			return new RedBlackEnumerator();
		}
	}
}
