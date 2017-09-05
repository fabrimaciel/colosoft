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

namespace Colosoft.Data.Caching.Dymanic
{
	/// <summary>
	/// Implementação do contexto de persistencia dinâmico.
	/// </summary>
	public class DynamicPersistenceContext : IPersistenceContext
	{
		private IPersistenceContext _databaseContext;

		private IPersistenceContext _cacheContext;

		private Colosoft.Data.Schema.ITypeSchema _typeSchema;

		private Query.IRecordKeyFactory _recordKeyFactory;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="databaseContext">Instancia do contexto do banco de dados.</param>
		/// <param name="cacheContext">Instancia do contexto do cache</param>
		/// <param name="typeSchema">Instancia dos esquemas dos tipos do sistema.</param>
		/// <param name="recordKeyFactory">Instancia da factory responsável pela criação das chaves de registro.</param>
		public DynamicPersistenceContext(IPersistenceContext databaseContext, IPersistenceContext cacheContext, Colosoft.Data.Schema.ITypeSchema typeSchema, Query.IRecordKeyFactory recordKeyFactory)
		{
			databaseContext.Require("databaseContext").NotNull();
			cacheContext.Require("cacheContext").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			recordKeyFactory.Require("recordKeyFactory").NotNull();
			_databaseContext = databaseContext;
			_cacheContext = cacheContext;
			_typeSchema = typeSchema;
			_recordKeyFactory = recordKeyFactory;
		}

		/// <summary>
		/// Cria uma sessão de persistencia.
		/// </summary>
		/// <returns></returns>
		public IPersistenceSession CreateSession()
		{
			return new DynamicPersistenceSession(_databaseContext.CreateSession(), _cacheContext.CreateSession(), _typeSchema, _recordKeyFactory);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_databaseContext.Dispose();
			_cacheContext.Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
	}
}
