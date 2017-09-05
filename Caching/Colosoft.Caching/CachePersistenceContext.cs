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

namespace Colosoft.Caching
{
	/// <summary>
	/// Implementação do contexto de persistencia do cache.
	/// </summary>
	public class CachePersistenceContext : Colosoft.Data.IPersistenceContext
	{
		private Lazy<ICacheProvider> _cacheProvider;

		private Lazy<Colosoft.Data.Schema.ITypeSchema> _typeSchema;

		private Lazy<Query.IRecordKeyFactory> _keyFactory;

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="cacheProvider">Objeto do cache</param>
		/// <param name="typeSchema">Esquema dos tipos do sistema.</param>
		/// <param name="keyFactory"></param>
		public CachePersistenceContext(Lazy<ICacheProvider> cacheProvider, Lazy<Colosoft.Data.Schema.ITypeSchema> typeSchema, Lazy<Query.IRecordKeyFactory> keyFactory) : base()
		{
			cacheProvider.Require("cacheProvider").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			keyFactory.Require("keyFactory").NotNull();
			_cacheProvider = cacheProvider;
			_typeSchema = typeSchema;
			_keyFactory = keyFactory;
		}

		/// <summary>
		/// Cria a sessão de persitencia
		/// </summary>
		/// <returns></returns>
		public Colosoft.Data.IPersistenceSession CreateSession()
		{
			return new CachePersistenceSession(_cacheProvider.Value, _typeSchema.Value, _keyFactory);
		}

		/// <summary>
		/// Libera a instância.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Libera a instancia
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
	}
}
