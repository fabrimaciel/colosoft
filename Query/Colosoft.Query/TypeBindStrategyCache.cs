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

namespace Colosoft.Query
{
	/// <summary>
	/// Classe responsável por gerenciar o cache das estratégias de vinculação de tipos.
	/// </summary>
	public static class TypeBindStrategyCache
	{
		/// <summary>
		/// Matriz que identifica a quantidade de acessos os itens do cache. 
		/// </summary>
		private static int[] _itemsAccessCount;

		/// <summary>
		/// Armazena os nomes do tipos das estratégias.
		/// </summary>
		private static string[] _itemsTypeNames;

		/// <summary>
		/// Armazena os itens do cache.
		/// </summary>
		private static TypeBindStrategy[] _items;

		private static int _size = 30;

		private static object _objLock = new object();

		/// <summary>
		/// Quantidade de itens que o cache suporta.
		/// </summary>
		public static int Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		/// <summary>
		/// Recupera o item do cache.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="creatorLoader"></param>
		/// <returns>Posição do mapeamento no vetor</returns>
		public static TypeBindStrategy GetItem(Type type, Func<Type, IQueryResultObjectCreator> creatorLoader)
		{
			type.Require("type").NotNull();
			lock (_objLock)
			{
				if(_items == null)
				{
					_itemsAccessCount = new int[_size];
					_itemsTypeNames = new string[_size];
					_items = new TypeBindStrategy[_size];
					for(int i = 0; i < _itemsAccessCount.Length; i++)
						_itemsAccessCount[i] = -1;
				}
				int pos = -1;
				for(int i = 0; i < _itemsTypeNames.Length; i++)
					if(_itemsTypeNames[i] != null && _itemsTypeNames[i] == type.FullName)
					{
						_itemsAccessCount[i]++;
						pos = i;
					}
					else
					{
						if(_itemsAccessCount[i] > int.MinValue)
							_itemsAccessCount[i]--;
					}
				if(pos >= 0)
					return _items[pos];
				pos = 0;
				for(int i = 1; i < _itemsAccessCount.Length; i++)
					if(_itemsAccessCount[i] < _itemsAccessCount[pos])
					{
						pos = i;
					}
				if(pos != -1 && _items[pos] != null)
				{
					System.Diagnostics.Debug.WriteLine("TYPE.BINDING.STRATEGY.CACHE FREE POSITION: " + pos + "; TYPE: " + _items[pos].Type.FullName);
					_items[pos] = null;
				}
				var instance = new TypeBindStrategy(type, creatorLoader == null ? null : creatorLoader(type));
				_itemsAccessCount[pos] = 10;
				_itemsTypeNames[pos] = type.FullName;
				_items[pos] = instance;
				return instance;
			}
		}

		/// <summary>
		/// Recupera o item do cache.
		/// </summary>
		/// <typeparam name="T">Tipo para a vinculação.</typeparam>
		/// <param name="creatorLoader"></param>
		/// <returns></returns>
		public static TypeBindStrategy GetItem<T>(Func<IQueryResultObjectCreator> creatorLoader)
		{
			return GetItem(typeof(T), creatorLoader != null ? new Func<Type, IQueryResultObjectCreator>(type => creatorLoader()) : null);
		}
	}
}
