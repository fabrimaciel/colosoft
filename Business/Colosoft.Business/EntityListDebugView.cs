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

namespace Colosoft.Business
{
	/// <summary>
	/// Classe usada para servir como auxiliar na exibição do debug.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	sealed class EntityListDebugView<T> where T : IEntity
	{
		private IEntityLinksList<T> _collection;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="collection"></param>
		public EntityListDebugView(IEntityLinksList<T> collection)
		{
			collection.Require("collection").NotNull();
			this._collection = collection;
		}

		/// <summary>
		/// Itens da coleção.
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				if(!_collection.IsLazyLoadState)
				{
					T[] array = new T[_collection.Count];
					this._collection.CopyTo(array, 0);
					return array;
				}
				else
					return new T[0];
			}
		}
	}
}
