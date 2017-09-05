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
using Colosoft.Data;

namespace Colosoft.Caching
{
	/// <summary>
	/// Persistência para o cache
	/// </summary>
	public class CachePersistenceSession : Colosoft.Data.Schema.SchemaPersistenceSession
	{
		private ICacheProvider _cacheProvider;

		private Colosoft.Data.Schema.ITypeSchema _typeSchema;

		private Lazy<Query.IRecordKeyFactory> _keyFactory;

		/// <summary>
		/// Instancia do esquema dos tipos do sistema.
		/// </summary>
		protected override Colosoft.Data.Schema.ITypeSchema TypeSchema
		{
			get
			{
				return _typeSchema;
			}
		}

		/// <summary>
		/// Provedor de cache.
		/// </summary>
		public ICacheProvider CacheProvider
		{
			get
			{
				return _cacheProvider;
			}
		}

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="cacheProvider">Provider do cache</param>
		/// <param name="typeSchema">Esquema dos tipos do sistema.</param>
		/// <param name="keyFactory"></param>
		public CachePersistenceSession(ICacheProvider cacheProvider, Colosoft.Data.Schema.ITypeSchema typeSchema, Lazy<Query.IRecordKeyFactory> keyFactory) : base()
		{
			cacheProvider.Require("cacheProvider").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			keyFactory.Require("keyFactory").NotNull();
			_cacheProvider = cacheProvider;
			_typeSchema = typeSchema;
			_keyFactory = keyFactory;
		}

		/// <summary>
		/// Retorna o executor das ações de persistência
		/// </summary>
		/// <returns>Executor do tipo <see cref="IPersistenceExecuter"/></returns>
		protected override IPersistenceExecuter CreateExecuter()
		{
			return new CachePersistenceExecuter(_cacheProvider, _keyFactory, TypeSchema);
		}
	}
}
