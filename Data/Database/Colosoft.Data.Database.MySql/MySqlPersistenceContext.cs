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

namespace Colosoft.Data.Database.MySql
{
	/// <summary>
	/// Implementação do contexto de persistencia do MySql.
	/// </summary>
	public class MySqlPersistenceContext : IPersistenceContext
	{
		private Schema.ITypeSchema _typeSchema;

		private Microsoft.Practices.ServiceLocation.IServiceLocator _serviceLocator;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="serviceLocator"></param>
		/// <param name="typeSchema"></param>
		public MySqlPersistenceContext(Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator, Schema.ITypeSchema typeSchema)
		{
			_serviceLocator = serviceLocator;
			_typeSchema = typeSchema;
		}

		/// <summary>
		/// Destrutor
		/// </summary>
		~MySqlPersistenceContext()
		{
			Dispose(false);
		}

		/// <summary>
		/// Cria uma nova instancia do executor.
		/// </summary>
		/// <returns></returns>
		private IPersistenceExecuter CreateExecuter()
		{
			return new MySqlPersistenceExecuter(_serviceLocator, _typeSchema);
		}

		/// <summary>
		/// Cria uma nova sessão de persistencia.
		/// </summary>
		/// <returns></returns>
		public IPersistenceSession CreateSession()
		{
			return new Schema.SchemaPersistenceSession(_typeSchema, CreateExecuter);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
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
