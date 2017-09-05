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
	/// Representa o argumento para o evento <see cref="LogOutingEventHandle"/>.
	/// </summary>
	public class LogOutingEventArgs : EventArgs
	{
		/// <summary>
		/// Token que está sendo liberado.
		/// </summary>
		public string Token
		{
			get;
			private set;
		}

		/// <summary>
		/// Identifica se a operação foi cancelada.
		/// </summary>
		public bool Cancel
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="token"></param>
		public LogOutingEventArgs(string token)
		{
			Token = token;
		}
	}
	/// <summary>
	/// Representa o argumento para o evento <see cref="LogOutedEventHandle"/>.
	/// </summary>
	public class LogOutedEventArgs : EventArgs
	{
		/// <summary>
		/// Token que está sendo liberado.
		/// </summary>
		public string Token
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="token"></param>
		public LogOutedEventArgs(string token)
		{
			Token = token;
		}
	}
	/// <summary>
	/// Representa os eventos acionados quando o usuário está deslogando do sistema.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void LogOutingEventHandle (object sender, LogOutingEventArgs e);
	/// <summary>
	/// Representa os eventos acionados quando o usuário for deslogado do sistema.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void LogOutedEventHandle (object sender, LogOutedEventArgs e);
	/// <summary>
	/// Valida as credenciais do usuário e gerencia as configurações de usuário
	/// </summary>
	public static class Membership
	{
		private static IUserProvider _provider;

		private static bool _initialized;

		private static Exception _initializeException;

		private static object _lock = new object();

		private static string _defaultProviderName;

		private static List<IUserProvider> _providers;

		/// <summary>
		/// Evento acionado quando o usuário está se delogando do sistema.
		/// </summary>
		public static event LogOutingEventHandle LogOuting;

		/// <summary>
		/// Evento acionado quando o usuário deslogar do sistema.
		/// </summary>
		public static event LogOutedEventHandle LogOuted;

		/// <summary>
		/// Armazena a lista dos provedores carregados.
		/// </summary>
		public static List<IUserProvider> Providers
		{
			get
			{
				Initialize();
				return _providers;
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
		/// Provedor dos usuários.
		/// </summary>
		public static IUserProvider Provider
		{
			get
			{
				Initialize();
				return _provider;
			}
		}

		/// <summary>
		/// Nome do usuário autenticado no sistema.
		/// </summary>
		public static string CurrentUserName
		{
			get
			{
				var principal = UserContext.Current.Principal;
				if(principal != null)
				{
					var identity = principal.Identity;
					if(identity != null && identity.IsAuthenticated)
						return identity.Name;
				}
				return null;
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
							_providers = new List<IUserProvider>();
							ServiceLocatorValidator.Validate();
							var instances = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetAllInstances<IUserProvider>();
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
							_initializeException = new InvalidOperationException(Properties.Resources.InvalidOperation_DefaultUserProviderNotFound);
							throw _initializeException;
						}
						_initialized = true;
					}
				}
			}
		}

		/// <summary>
		/// Recupera a informação da fonte de dados e atualiza a última atividade para o logon atual usuário da associação.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static IUser GetUser(string username)
		{
			return Provider.GetUser(username, true);
		}

		/// <summary>
		/// Recupera os dados do usuário pela sua chave.
		/// </summary>
		/// <param name="userKey">Chave associado com o usuário.</param>
		/// <returns></returns>
		public static IUser GetUserByKey(string userKey)
		{
			return Provider.GetUserByKey(userKey, true);
		}

		/// <summary>
		/// Recupera os dados do usuário pelo token informado.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public static IUser GetUserByToken(string token)
		{
			return Provider.GetUserByToken(token, true);
		}

		/// <summary>
		/// Verifica se o nome e senha do usuário são validos.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="parameters">Parametros que poderão ser usados na autenticação.</param>
		/// <returns></returns>
		public static IValidateUserResult ValidateUser(string username, string password, params SecurityParameter[] parameters)
		{
			return Provider.ValidateUser(username, password, parameters);
		}

		/// <summary>
		/// Valida os dados do token.
		/// </summary>
		/// <param name="token">Token.</param>
		/// <returns></returns>
		public static IValidateUserResult ValidateToken(string token)
		{
			return Provider.ValidateToken(token);
		}

		/// <summary>
		/// Desloga o usuário do sistema
		/// </summary>
		/// <param name="token">Token do usuário</param>
		/// <returns>Sucesso da operação</returns>
		public static bool LogOut(string token)
		{
			if(LogOuting != null)
			{
				var args = new LogOutingEventArgs(token);
				LogOuting(null, args);
				if(args.Cancel)
					return false;
			}
			if(string.IsNullOrEmpty(token))
				return true;
			try
			{
				Provider.LogOut(token);
			}
			catch
			{
			}
			if(LogOuted != null)
				LogOuted(null, new LogOutedEventArgs(token));
			return true;
		}

		/// <summary>
		/// Método usado para alterar a senha do usuário.
		/// </summary>
		/// <param name="username">Nome do usuário.</param>
		/// <param name="oldPassword">Antiga senha.</param>
		/// <param name="newPassword">Nova senha.</param>
		/// <param name="parameters">Parametros da autenticação.</param>
		/// <returns></returns>
		public static ChangePasswordResult ChangePassword(string username, string oldPassword, string newPassword, params SecurityParameter[] parameters)
		{
			return Provider.ChangePassword(username, oldPassword, newPassword, parameters);
		}

		/// <summary>
		/// Inicia o processo de redefinição de senha
		/// </summary>
		/// <param name="userName">Nome do usuário</param>
		/// <returns>Resultado do processo</returns>
		public static ResetPasswordProcessResult RequestPasswordReset(string userName)
		{
			return Provider.RequestPasswordReset(userName);
		}
	}
}
