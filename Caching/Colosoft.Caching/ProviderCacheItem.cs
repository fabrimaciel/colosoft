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
using Colosoft.Caching.Dependencies;

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa um item do provedor do cache.
	/// </summary>
	public class ProviderCacheItem
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="value">Valor do item.</param>
		public ProviderCacheItem(object value)
		{
			this.Value = value;
		}

		/// <summary>
		/// Data absoluta para expirar o item.
		/// </summary>
		public DateTime AbsoluteExpiration
		{
			get;
			set;
		}

		/// <summary>
		/// Dependencia associada.
		/// </summary>
		public CacheDependency Dependency
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do grupo onde o item está inserido.
		/// </summary>
		public string Group
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do subgrupo onde o item está inserido.
		/// </summary>
		public string SubGroup
		{
			get;
			set;
		}

		/// <summary>
		/// Prioridade do item.
		/// </summary>
		public CacheItemPriority ItemPriority
		{
			get;
			set;
		}

		/// <summary>
		/// Tags nomeadadas associadas.
		/// </summary>
		public NamedTagsDictionary NamedTags
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a resincronização do item está abilitada quando ele expirar.
		/// </summary>
		public bool ResyncItemOnExpiration
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do provedor de resincronização.
		/// </summary>
		public string ResyncProviderName
		{
			get;
			set;
		}

		/// <summary>
		/// Data de corte de expiração.
		/// </summary>
		public TimeSpan SlidingExpiration
		{
			get;
			set;
		}

		/// <summary>
		/// Tags associadas.
		/// </summary>
		public Tag[] Tags
		{
			get;
			set;
		}

		/// <summary>
		/// Valor do item.
		/// </summary>
		public object Value
		{
			get;
			set;
		}
	}
}
