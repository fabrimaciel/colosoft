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

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Identificador da consulta.
	/// </summary>
	internal class QueryIdentifier : IComparable
	{
		private string _query = string.Empty;

		private ulong _refCount;

		/// <summary>
		/// Textp da consulta.
		/// </summary>
		public string Query
		{
			get
			{
				return _query;
			}
			set
			{
				_query = value;
			}
		}

		/// <summary>
		/// Quantidade de referencias.
		/// </summary>
		public ulong ReferenceCount
		{
			get
			{
				return _refCount;
			}
			set
			{
				_refCount = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="query">Texto da consulta.</param>
		public QueryIdentifier(string query)
		{
			_query = query.ToLower();
			_refCount = 1;
		}

		/// <summary>
		/// Adiciona uma nova referencia para consulta.
		/// </summary>
		public void AddRef()
		{
			lock (this)
				_refCount++;
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			int num = 0;
			if((obj != null) && (obj is QueryIdentifier))
			{
				QueryIdentifier identifier = (QueryIdentifier)obj;
				if(identifier._refCount > _refCount)
					return -1;
				if(identifier._refCount < _refCount)
					num = 1;
			}
			return num;
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			QueryIdentifier identifier = obj as QueryIdentifier;
			if(identifier == null)
				return _query.Equals(obj.ToString().ToLower());
			return this.Query.Equals(identifier.Query);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return _query.GetHashCode();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _query.ToLower();
		}
	}
}
