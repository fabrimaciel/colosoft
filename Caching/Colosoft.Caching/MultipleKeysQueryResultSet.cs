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
	/// Implementação do resultado da consulta para multiplicas chaves
	/// de acesso ao cache.
	/// </summary>
	public class MultipleKeysQueryResultSet : QueryResultSet
	{
		private int _defaultEntityIndex;

		private CacheDataSource.LinkResult _result;

		/// <summary>
		/// Instancia do resultado associado.
		/// </summary>
		internal CacheDataSource.LinkResult Result
		{
			get
			{
				return _result;
			}
		}

		/// <summary>
		/// Relação das chaves do resultado.
		/// </summary>
		public override System.Collections.ArrayList SearchKeysResult
		{
			get
			{
				if(base.SearchKeysResult == null)
				{
					var keys = new System.Collections.ArrayList();
					foreach (var i in _result.Items)
						keys.Add(i[_defaultEntityIndex]);
					base.SearchKeysResult = keys;
				}
				return base.SearchKeysResult;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public MultipleKeysQueryResultSet()
		{
		}

		/// <summary>
		/// Cria a instancia com os valores inciciais.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="defaultEntityIndex"></param>
		internal MultipleKeysQueryResultSet(CacheDataSource.LinkResult result, int defaultEntityIndex)
		{
			_result = result;
			_defaultEntityIndex = defaultEntityIndex;
		}
	}
}
