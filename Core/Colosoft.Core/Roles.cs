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

namespace Colosoft.Security
{
	/// <summary>
	/// Classe que gerencia os papéis dos sistema.
	/// </summary>
	public static class Roles
	{
		private static IRoleProvider _provider;

		private static string _defaultProviderName;

		private static bool _initialized;

		private static Exception _initializeException;

		private static object _lock = new object();

		private static List<IRoleProvider> _providers;

		/// <summary>
		/// Nome do provedor padrão do sistema.
		/// </summary>
		public static string DefaultProviderName
		{
			get
			{
				return _defaultProviderName;
			}
			set
			{
				_defaultProviderName = value;
			}
		}

		/// <summary>
		/// Provedor de papéis associado com a instancia.
		/// </summary>
		public static IRoleProvider Provider
		{
			get
			{
				Initialize();
				return _provider;
			}
		}

		/// <summary>
		/// Armazena a lista dos provedores carregados.
		/// </summary>
		public static List<IRoleProvider> Providers
		{
			get
			{
				Initialize();
				return Roles._providers;
			}
		}

		/// <summary>
		/// Initializa os dados do membership.
		/// </summary>
		private static void Initialize()
		{
			if(_initialized)
			{
				if(_initializeException != null)
					throw _initializeException;
			}
			else
			{
				if(_initializeException != null)
					throw _initializeException;
				lock (_lock)
				{
					if(_initialized)
					{
						if(_initializeException != null)
							throw _initializeException;
					}
					else
					{
						try
						{
							_providers = new List<IRoleProvider>();
							ServiceLocatorValidator.Validate();
							var instances = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetAllInstances<IRoleProvider>();
							foreach (var provider in instances)
								_providers.Add(provider);
							if(string.IsNullOrEmpty(DefaultProviderName))
							{
								_provider = _providers.FirstOrDefault();
								if(_provider != null)
									DefaultProviderName = _provider.Name;
							}
							else
								_provider = _providers.Find(f => f.Name == DefaultProviderName);
						}
						catch(Exception exception)
						{
							_initializeException = exception;
							throw;
						}
						if(_provider == null)
						{
							_initializeException = new InvalidOperationException(Properties.Resources.InvalidOperation_DefaultRolesProviderNotFound);
							throw _initializeException;
						}
						_initialized = true;
					}
				}
			}
		}

		/// <summary>
		/// Adiciona os usuário informados para os papéis informados.
		/// </summary>
		/// <param name="usernames"></param>
		/// <param name="roleNames"></param>
		public static void AddUsersToRoles(string[] usernames, string[] roleNames)
		{
			Initialize();
			SecurityUtility.CheckArrayParameter(ref roleNames, true, true, true, 0, "roleName");
			SecurityUtility.CheckArrayParameter(ref usernames, true, true, true, 0, "usernames");
			Provider.AddUsersToRoles(usernames, roleNames);
		}

		/// <summary>
		/// Apaga o papel do sistema.
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public static bool DeleteRole(string roleName)
		{
			Initialize();
			SecurityUtility.CheckParameter(ref roleName, true, true, true, 0, "roleName");
			return Provider.DeleteRole(roleName);
		}

		/// <summary>
		/// Recupera todos os papéis associados com o usuário.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static string[] GetRolesForUser(string username)
		{
			Initialize();
			SecurityUtility.CheckParameter(ref username, true, false, true, 0, "username");
			if(username.Length < 1)
				return new string[0];
			return Provider.GetRolesForUser(username);
		}
	}
}
