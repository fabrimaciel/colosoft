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

namespace Colosoft.Query
{
	/// <summary>
	/// Implementação de uma origem de dados dinamica para consulta.
	/// </summary>
	public class DynamicQueryDataSource : IQueryDataSource
	{
		private Microsoft.Practices.ServiceLocation.IServiceLocator _serviceLocator;

		private IProviderLocator _providerLocator;

		private Dictionary<string, IQueryDataSource> _queryDataSources = new Dictionary<string, IQueryDataSource>();

		private object _objLock = new object();

		/// <summary>
		/// Instancia do localizador de providers.
		/// </summary>
		public IProviderLocator ProviderLocator
		{
			get
			{
				return _providerLocator;
			}
		}

		/// <summary>
		/// Identifica se a origem de dados foi inicializada.
		/// </summary>
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="serviceLocator">Localizador dos serviço.</param>
		/// <param name="providerLocator">Localizador dos provider.</param>
		public DynamicQueryDataSource(Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator, IProviderLocator providerLocator)
		{
			serviceLocator.Require("serviceLocator").NotNull();
			providerLocator.Require("providerLocator").NotNull();
			_serviceLocator = serviceLocator;
			_providerLocator = providerLocator;
		}

		/// <summary>
		/// Recupera a origemd de ados
		/// </summary>
		/// <param name="queryInfo"></param>
		/// <returns></returns>
		private IQueryDataSource GetDataSource(QueryInfo queryInfo)
		{
			var providerName = ProviderLocator.GetProviderName(queryInfo);
			IQueryDataSource dataSource = null;
			lock (_objLock)
				if(_queryDataSources.TryGetValue(providerName, out dataSource))
					return dataSource;
			dataSource = _serviceLocator.GetInstance<IQueryDataSource>(string.Format("{0}QueryDataSource", providerName));
			if(dataSource == null)
				throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.DataSourceUndefined).Format());
			lock (_objLock)
			{
				if(!_queryDataSources.ContainsKey(providerName))
					_queryDataSources.Add(providerName, dataSource);
			}
			return dataSource;
		}

		/// <summary>
		/// Executa a consulta.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public IQueryResult Execute(QueryInfo query)
		{
			var dataSource = GetDataSource(query);
			return dataSource.Execute(query);
		}

		/// <summary>
		/// Executa as consultas informadas
		/// </summary>
		/// <param name="queries"></param>
		/// <returns></returns>
		public IEnumerable<IQueryResult> Execute(QueryInfo[] queries)
		{
			var dataSource = GetDataSource(queries.First());
			return dataSource.Execute(queries);
		}
	}
}
