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
using Colosoft.Query;
using Colosoft.Data;
using Colosoft.Data.Schema;

namespace Colosoft.DataAccess
{
	static class PersistenceContextManager
	{
		private static object _objLock = new object();

		private static IPersistenceContext _persistenceContext;

		public static IPersistenceContext PersitenceContext
		{
			get
			{
				if(_persistenceContext == null)
					lock (_objLock)
						if(_persistenceContext == null)
						{
							var persistenceContext = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IPersistenceContext>();
							_persistenceContext = persistenceContext;
							return persistenceContext;
						}
				return _persistenceContext;
			}
		}
	}
	/// <summary>
	/// SourceContext padrão para aplicação
	/// </summary>
	public abstract class BaseSourceContext<T> : IDataAccess where T : BaseSourceContext<T>, new()
	{
		private static T _instance;

		private Colosoft.Query.ISourceContext _queryContext;

		private Colosoft.Data.IPersistenceContext _persistenceContext;

		/// <summary>
		/// Instancia do contexto.
		/// </summary>
		public static T Instance
		{
			get
			{
				if(_instance == null)
					_instance = new T();
				return _instance;
			}
		}

		/// <summary>
		/// Construtor privado.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		protected BaseSourceContext()
		{
			try
			{
				Initialize();
			}
			catch(Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Cria um nova consulta.
		/// </summary>
		/// <returns></returns>
		public Colosoft.Query.Queryable CreateQuery()
		{
			return _queryContext.CreateQuery();
		}

		/// <summary>
		/// Cria uma nova consulta.
		/// </summary>
		/// <returns></returns>
		public Colosoft.Query.MultiQueryable CreateMultiQuery()
		{
			return _queryContext.CreateMultiQuery();
		}

		/// <summary>
		/// Cria uma sessão de persistencia.
		/// </summary>
		/// <returns></returns>
		public Colosoft.Data.IPersistenceSession CreateSession()
		{
			return _persistenceContext.CreateSession();
		}

		/// <summary>
		/// Libera a memória dos componentes
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		protected virtual void Initialize()
		{
			var serviceLocator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current;
			IQueryDataSourceSelector _selector = null;
			try
			{
				var selectors = serviceLocator.GetAllInstances<IQueryDataSourceSelector>();
				_selector = selectors.FirstOrDefault();
			}
			catch(Exception)
			{
			}
			if(_selector == null)
			{
				try
				{
					_queryContext = serviceLocator.GetInstance<Colosoft.Query.ISourceContext>();
				}
				catch(Exception ex)
				{
					throw new InvalidOperationException(Properties.Resources.Exception_FailOnLoadQueryContext, ex);
				}
			}
			else
				_queryContext = new QuerySourceContextWrapper(new QueryDataSourceWrapper(_selector));
			try
			{
				_persistenceContext = PersistenceContextManager.PersitenceContext;
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(Properties.Resources.Exception_LoadPersistenceContextError, ex);
			}
		}

		/// <summary>
		/// Libera a memória dos componentes
		/// </summary>
		/// <param name="disposing">indica chamada fora do destrutor</param>
		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_queryContext != null)
				{
					_queryContext.Dispose();
					_queryContext = null;
				}
				if(_persistenceContext != null)
				{
					_persistenceContext.Dispose();
					_persistenceContext = null;
				}
			}
		}
	}
}
