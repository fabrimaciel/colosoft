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

namespace Colosoft.Caching.Dependencies
{
	/// <summary>
	/// Representa uma dependencia baseado em chaves.
	/// </summary>
	[Serializable]
	public class KeyDependency : CacheDependency
	{
		private string[] _cacheKeys;

		private long _startAfterTicks;

		/// <summary>
		/// Chaves associadas.
		/// </summary>
		public string[] CacheKeys
		{
			get
			{
				return _cacheKeys;
			}
		}

		/// <summary>
		/// Sticks que identificam o inicio da verificação de dependencia.
		/// </summary>
		public long StartAfterTicks
		{
			get
			{
				return _startAfterTicks;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public KeyDependency()
		{
		}

		/// <summary>
		/// Cria uma instancia para apenas uma chave.
		/// </summary>
		/// <param name="key">Chave associada com a dependencia.</param>
		public KeyDependency(string key) : this(new string[] {
			key
		}, DateTime.Now)
		{
		}

		/// <summary>
		/// Cria uma instancia para várias chaves.
		/// </summary>
		/// <param name="keys">Chaves associadas com a dependencia.</param>
		public KeyDependency(string[] keys) : this(keys, DateTime.Now)
		{
		}

		/// <summary>
		/// Cria uma instancia para apenas uma chave.
		/// </summary>
		/// <param name="key">Chave associada com a dependencia.</param>
		/// <param name="startAfter"></param>
		public KeyDependency(string key, DateTime startAfter) : this(new string[] {
			key
		}, startAfter)
		{
		}

		/// <summary>
		/// Cria uma instancia para várias chaves.
		/// </summary>
		/// <param name="keys">Chaves associadas com a dependencia.</param>
		/// <param name="startAfter"></param>
		public KeyDependency(string[] keys, DateTime startAfter)
		{
			_cacheKeys = keys;
			_startAfterTicks = startAfter.Ticks;
		}
	}
}
