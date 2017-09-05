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

namespace Colosoft.Caching.Configuration.Dom
{
	/// <summary>
	/// Armazena os dados das notificações.
	/// </summary>
	[Serializable]
	public class Notifications : ICloneable
	{
		private bool _cacheClear;

		private bool _itemAdd;

		private bool _itemRemove;

		private bool _itemUpdate;

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			Notifications notifications = new Notifications();
			notifications.ItemAdd = ItemAdd;
			notifications.ItemRemove = ItemRemove;
			notifications.ItemUpdate = ItemUpdate;
			notifications.CacheClear = CacheClear;
			return notifications;
		}

		/// <summary>
		/// Identifica se é para notificar quando o cache for limpo.
		/// </summary>
		[ConfigurationAttribute("cache-clear")]
		public bool CacheClear
		{
			get
			{
				return _cacheClear;
			}
			set
			{
				_cacheClear = value;
			}
		}

		/// <summary>
		/// Identifica se é para notificar quando um novo item for adicionado no cache.
		/// </summary>
		[ConfigurationAttribute("item-add")]
		public bool ItemAdd
		{
			get
			{
				return _itemAdd;
			}
			set
			{
				_itemAdd = value;
			}
		}

		/// <summary>
		/// Identifica se é para notificar quando um item for removido.
		/// </summary>
		[ConfigurationAttribute("item-remove")]
		public bool ItemRemove
		{
			get
			{
				return _itemRemove;
			}
			set
			{
				_itemRemove = value;
			}
		}

		/// <summary>
		/// Identifica se é para notificar quando um item for atualizado.
		/// </summary>
		[ConfigurationAttribute("item-update")]
		public bool ItemUpdate
		{
			get
			{
				return _itemUpdate;
			}
			set
			{
				_itemUpdate = value;
			}
		}
	}
}
