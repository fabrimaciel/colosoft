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
using Colosoft.Runtime;
using System.Collections;

namespace Colosoft.Serialization.IO
{
	/// <summary>
	/// Representa um contexto de serialização.
	/// </summary>
	public class SerializationContext
	{
		private MemoryManager _memManager;

		private string _cacheContext = string.Empty;

		private Dictionary<object, int> _cookieList = new Dictionary<object, int>();

		private Dictionary<int, object> _graphList = new Dictionary<int, object>();

		/// <summary>
		/// Nome do contexto.
		/// </summary>
		public string CacheContext
		{
			get
			{
				return _cacheContext;
			}
			set
			{
				_cacheContext = value;
			}
		}

		/// <summary>
		/// Gerenciador de memória associado.
		/// </summary>
		public MemoryManager MemManager
		{
			get
			{
				return _memManager;
			}
			set
			{
				_memManager = value;
			}
		}

		/// <summary>
		/// Recupera o número do cookie associado com o objeto informado.
		/// </summary>
		/// <param name="graph"></param>
		/// <returns></returns>
		public int GetCookie(object graph)
		{
			if(_cookieList.ContainsKey(graph))
				return _cookieList[graph];
			return -1;
		}

		/// <summary>
		/// Recupera o objeto pelo número do cookie informado.
		/// </summary>
		/// <param name="key">Chave do cookie associado com objeto.</param>
		/// <returns></returns>
		public object GetObject(int key)
		{
			if(key > -1 && key < _graphList.Count)
				return _graphList[key];
			return null;
		}

		/// <summary>
		/// Registra o objeto para ser relembrado.
		/// </summary>
		/// <param name="graph">Instancia que será gerenciada.</param>
		/// <returns></returns>
		public int RememberObject(object graph)
		{
			int count = _graphList.Count;
			_graphList.Add(count, graph);
			_cookieList.Add(graph, count);
			return count;
		}
	}
}
