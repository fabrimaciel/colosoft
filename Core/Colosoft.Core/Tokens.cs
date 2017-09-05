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
	/// Classe responsável pelo gerenciamento dos tokens do sistema.
	/// </summary>
	public class Tokens
	{
		private static ITokenProvider _provider;

		private static string _defaultProviderName;

		private static bool _initialized;

		private static Exception _initializeException;

		private static object _lock = new object();

		private static List<ITokenProvider> _providers;

		private static List<ITokenGetter> _tokenGetters = new List<ITokenGetter>();

		/// <summary>
		/// Recuperadores de token do sistem.
		/// </summary>
		public static IList<ITokenGetter> TokenGetters
		{
			get
			{
				return _tokenGetters;
			}
		}

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
		public static ITokenProvider Provider
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
		public static List<ITokenProvider> Providers
		{
			get
			{
				Initialize();
				return _providers;
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
						ServiceLocatorValidator.Validate();
						try
						{
							_providers = new List<ITokenProvider>();
							var instances = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetAllInstances<ITokenProvider>();
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
						catch(System.Reflection.ReflectionTypeLoadException exception)
						{
							throw exception.LoaderExceptions.First();
						}
						catch(Exception exception)
						{
							_initializeException = exception;
							throw;
						}
						if(_provider == null)
						{
							_initializeException = new InvalidOperationException(Properties.Resources.InvalidOperation_DefaultTokenProviderNotFound);
							throw _initializeException;
						}
						_initialized = true;
					}
				}
			}
		}

		/// <summary>
		/// Verifica se um token está ou não válido
		/// </summary>
		/// <param name="token">token</param>
		/// <returns>Objeto com o resultado da consulta</returns>
		public static TokenConsultResult Check(string token)
		{
			return Provider.Check(token);
		}

		/// <summary>
		/// Executa uma verificação do token no servidor.
		/// </summary>
		/// <param name="token">Token</param>
		/// <returns></returns>
		public static TokenPingResult Ping(string token)
		{
			return Provider.Ping(token);
		}

		/// <summary>
		/// Marca as mensagens como lidas.
		/// </summary>
		/// <param name="dispatcherIds">Identificadores dos despachos.</param>
		public static void MarkMessageAsRead(IEnumerable<int> dispatcherIds)
		{
			Provider.MarkMessageAsRead(dispatcherIds);
		}

		/// <summary>
		/// Define o perfil para o token.
		/// </summary>
		/// <param name="token">Token.</param>
		/// <param name="profileId">Informações do perfil.</param>
		/// <returns></returns>
		public static TokenSetProfileResult SetProfile(string token, int profileId)
		{
			return Provider.SetProfile(token, profileId);
		}

		/// <summary>
		/// Fecha os tokens em aberto de um usuário.
		/// </summary>
		/// <param name="userId">Identificador do usuário</param>
		public static void CloseUserTokens(int userId)
		{
			Provider.CloseUserTokens(userId);
		}

		/// <summary>
		/// Fecha os tokens em aberto de um usuário.
		/// </summary>
		/// <param name="userId">Identificador do usuário.</param>
		/// <param name="applicationName">Nome da aplicação associada.</param>
		public static void CloseUserTokens(int userId, string applicationName)
		{
			if(Provider is ITokenApplicationProvider)
				((ITokenApplicationProvider)Provider).CloseUserTokens(userId, applicationName);
			else
				Provider.CloseUserTokens(userId);
		}

		/// <summary>
		/// Reseta os provedores de token do sistema.
		/// </summary>
		public static void Reset()
		{
			_initializeException = null;
			_providers = null;
			_initialized = false;
		}
	}
}
