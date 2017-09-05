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

namespace Colosoft.Data
{
	/// <summary>
	/// Implementação do executor dinamico de persistencia.
	/// </summary>
	public class DynamicPersistenceExecuter : IPersistenceExecuter
	{
		private Microsoft.Practices.ServiceLocation.IServiceLocator _serviceLocator;

		private Colosoft.Query.IProviderLocator _providerLocator;

		private Dictionary<string, IPersistenceExecuter> _executers = new Dictionary<string, IPersistenceExecuter>();

		private object _objLock = new object();

		/// <summary>
		/// Instancia o localizador de provedores de acesso.
		/// </summary>
		protected Colosoft.Query.IProviderLocator ProviderLocator
		{
			get
			{
				return _providerLocator;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="serviceLocator"></param>
		/// <param name="providerLocator"></param>
		public DynamicPersistenceExecuter(Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator, Colosoft.Query.IProviderLocator providerLocator)
		{
			serviceLocator.Require("serviceLocator").NotNull();
			providerLocator.Require("providerLocator").NotNull();
			_serviceLocator = serviceLocator;
			_providerLocator = providerLocator;
		}

		/// <summary>
		/// Recupera a origemd de ados
		/// </summary>
		/// <param name="actions"></param>
		/// <returns></returns>
		private IPersistenceExecuter GetExecuter(PersistenceAction[] actions)
		{
			var providerName = this.ProviderLocator.GetProviderName(actions.First().EntityFullName);
			IPersistenceExecuter executer = null;
			lock (_objLock)
				if(_executers.TryGetValue(providerName, out executer))
					return executer;
			executer = _serviceLocator.GetInstance<IPersistenceExecuter>(string.Format("{0}PersistenceExecuter", providerName));
			if(executer == null)
				throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.PersistenceExecuterUndefined).Format());
			lock (_objLock)
			{
				if(!_executers.ContainsKey(providerName))
					_executers.Add(providerName, executer);
			}
			return executer;
		}

		/// <summary>
		/// Executa as ações informadas.
		/// </summary>
		/// <param name="actions"></param>
		/// <param name="executionType"></param>
		/// <returns></returns>
		public PersistenceExecuteResult Execute(PersistenceAction[] actions, ExecutionType executionType)
		{
			var executer = GetExecuter(actions);
			return executer.Execute(actions, executionType);
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
