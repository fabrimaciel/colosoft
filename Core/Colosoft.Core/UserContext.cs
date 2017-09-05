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
using System.Globalization;
using System.Linq;
using System.Text;

namespace Colosoft.Security
{
	/// <summary>
	/// Possíveis métodos de recuperação dos dados do usuário
	/// a partir do name do Identity no Principal.
	/// </summary>
	public enum UserContextGetUserMode
	{
		/// <summary>
		/// Identifica se é para recuperar os dados do usuário com base na sua chave.
		/// </summary>
		GetByKey,
		/// <summary>
		/// Identifica se é para recuperar os dados do usuário com base no nome.
		/// </summary>
		GetByName
	}
	/// <summary>
	/// Armazena os dados do contexto do usuário autenticado no sistema.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	[Serializable]
	public class UserContext : System.ComponentModel.INotifyPropertyChanged, System.Runtime.Serialization.ISerializable
	{
		private static Func<UserContext, System.Security.Principal.IPrincipal> _principalGetter;

		private static Queue<System.Threading.Thread> _contextThreads = new Queue<System.Threading.Thread>();

		private static bool _userProcessing = false;

		private static bool _supportGetUserByToken = true;

		private static UserContextGetUserMode _getUserMode = UserContextGetUserMode.GetByKey;

		private IUser _user;

		private Threading.SimpleMonitor _loadingUserMonitor = new Threading.SimpleMonitor();

		private Profile.ProfileContext _profileContext = new Profile.ProfileContext();

		private Security.Principal.DefaultPrincipal _principal;

		/// <summary>
		/// Evento acionado quando um propriedade for alterada.
		/// </summary>
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Identifica se está processando os dados do usuário.
		/// </summary>
		public static bool UserProcessing
		{
			get
			{
				return _userProcessing;
			}
		}

		/// <summary>
		/// Identifica se tem suporte em recupera os dados do usuário pelo token informado.
		/// </summary>
		public static bool SupportGetUserByToken
		{
			get
			{
				return _supportGetUserByToken;
			}
			set
			{
				_supportGetUserByToken = value;
			}
		}

		/// <summary>
		/// Modo de recuperação dos dados do usuário. Padrão GetByKey.
		/// </summary>
		public static UserContextGetUserMode GetUserMode
		{
			get
			{
				return _getUserMode;
			}
			set
			{
				_getUserMode = value;
			}
		}

		/// <summary>
		/// Contexto do perfil.
		/// </summary>
		internal Profile.ProfileContext ProfileContext
		{
			get
			{
				return _profileContext;
			}
		}

		/// <summary>
		/// Recupera a unica instancia do contexto do usuário do sistema.
		/// </summary>
		public static UserContext Current
		{
			get
			{
				var userContext = Runtime.RuntimeValueStorage.Instance.GetValue("ColosoftUserContext") as UserContext;
				if(userContext == null)
				{
					userContext = new UserContext();
					var foundPrincial = false;
					if(_principalGetter != null)
					{
						var principal = _principalGetter(userContext);
						if(principal != null)
						{
							if(!(principal is Principal.DefaultPrincipal))
								userContext._principal = new Principal.DefaultPrincipal(principal.Identity);
							else
								userContext._principal = (Principal.DefaultPrincipal)principal;
							foundPrincial = true;
						}
					}
					if(!foundPrincial)
					{
						var principal = System.Threading.Thread.CurrentPrincipal;
						Security.Principal.DefaultIdentity identity = null;
						if(principal != null && principal.Identity != null)
						{
							if(principal.Identity is Security.Principal.DefaultIdentity)
								identity = (Security.Principal.DefaultIdentity)principal.Identity;
							else
								identity = new Security.Principal.DefaultIdentity(principal.Identity.Name, "", principal.Identity.IsAuthenticated, principal.Identity.AuthenticationType);
						}
						else
							identity = new Security.Principal.DefaultIdentity("", "");
						userContext._principal = new Principal.DefaultPrincipal(identity);
					}
					Runtime.RuntimeValueStorage.Instance.SetValue("ColosoftUserContext", userContext);
					userContext.ConfigureThread(System.Threading.Thread.CurrentThread);
					lock (_contextThreads)
						foreach (var i in _contextThreads)
							userContext.ConfigureThread(i);
				}
				return userContext;
			}
		}

		/// <summary>
		/// Principal associado com a instancia.
		/// </summary>
		public System.Security.Principal.IPrincipal Principal
		{
			get
			{
				return _principal;
			}
		}

		/// <summary>
		/// Recupera a instancia do usuário logado no sistema.
		/// </summary>
		public virtual IUser User
		{
			get
			{
				System.Security.Principal.IPrincipal currentPrincipal = Principal;
				if((currentPrincipal != null) && (currentPrincipal.Identity != null))
				{
					var identity = currentPrincipal.Identity;
					if(!identity.IsAuthenticated)
						return null;
					if(_user == null || _user.UserName != identity.Name)
					{
						if(!_loadingUserMonitor.Busy)
						{
							_loadingUserMonitor.Enter();
							using (_loadingUserMonitor)
							{
								try
								{
									_userProcessing = true;
									if(SupportGetUserByToken && identity is Security.Principal.DefaultIdentity)
										_user = Security.Membership.GetUserByToken(((Security.Principal.DefaultIdentity)identity).Token);
									else if(GetUserMode == UserContextGetUserMode.GetByKey)
										_user = Security.Membership.GetUserByKey(identity.Name);
									else if(GetUserMode == UserContextGetUserMode.GetByName)
										_user = Security.Membership.GetUser(identity.Name);
								}
								finally
								{
									_userProcessing = false;
								}
							}
						}
					}
					return _user;
				}
				return null;
			}
			protected set
			{
				if(_user != value)
				{
					_user = value;
					RaisePropertyChanged("User");
				}
			}
		}

		/// <summary>
		/// Token do usuário do contexto.
		/// </summary>
		public string Token
		{
			get
			{
				if(Tokens.TokenGetters.Count > 0)
				{
					foreach (var i in Tokens.TokenGetters.ToArray())
					{
						var token = i.Get();
						if(!string.IsNullOrEmpty(token))
							return token;
					}
				}
				var principal = Principal;
				if(principal.Identity is Security.Principal.DefaultIdentity)
				{
					var identity = (Security.Principal.DefaultIdentity)principal.Identity;
					return identity.Token;
				}
				return null;
			}
		}

		/// <summary>
		/// Informações regionais do usuário.
		/// </summary>
		public CultureInfo CurrentCultureInfo
		{
			get
			{
				return Globalization.Culture.SystemCulture;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public UserContext()
		{
		}

		/// <summary>
		/// Construtor usado na deserialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected UserContext(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			_profileContext = (Profile.ProfileContext)info.GetValue("ProfileContext", typeof(Profile.ProfileContext));
			_principal = (Security.Principal.DefaultPrincipal)info.GetValue("Principal", typeof(Security.Principal.DefaultPrincipal));
		}

		/// <summary>
		/// Configura a thread;
		/// </summary>
		/// <param name="contextThread"></param>
		private bool ConfigureThread(System.Threading.Thread contextThread)
		{
			if(contextThread.ExecutionContext != null)
			{
				var setPrincipalInternalMethod = typeof(System.Threading.Thread).GetMethod("SetPrincipalInternal", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				if(setPrincipalInternalMethod != null)
				{
					try
					{
						setPrincipalInternalMethod.Invoke(contextThread, new object[] {
							_principal
						});
						return true;
					}
					catch
					{
					}
				}
				var getLogicalCallContextMethod = typeof(System.Threading.Thread).GetMethod("GetLogicalCallContext", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				if(getLogicalCallContextMethod == null)
				{
					return false;
				}
				System.Runtime.Remoting.Messaging.LogicalCallContext callContext = null;
				try
				{
					callContext = (System.Runtime.Remoting.Messaging.LogicalCallContext)getLogicalCallContextMethod.Invoke(contextThread, null);
				}
				catch
				{
					return false;
				}
				if(callContext != null)
				{
					var principalProperty = typeof(System.Runtime.Remoting.Messaging.LogicalCallContext).GetProperty("Principal", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
					if(principalProperty == null)
						throw new InvalidOperationException("PrincipalProperty not found");
					if(callContext == null)
						throw new InvalidOperationException("callContext undefined");
					principalProperty.SetValue(callContext, _principal, null);
				}
			}
			return true;
		}

		/// <summary>
		/// Configura a thread a thread informada com os parametros do contexto do usuário.
		/// </summary>
		/// <param name="contextThread"></param>
		/// <param name="principal"></param>
		public static void ConfigureThread(System.Threading.Thread contextThread, System.Security.Principal.IPrincipal principal)
		{
			try
			{
				var getLogicalCallContextMethod = typeof(System.Threading.Thread).GetMethod("GetLogicalCallContext", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				var callContext = (System.Runtime.Remoting.Messaging.LogicalCallContext)getLogicalCallContextMethod.Invoke(contextThread, null);
				var principalProperty = typeof(System.Runtime.Remoting.Messaging.LogicalCallContext).GetProperty("Principal", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				principalProperty.SetValue(callContext, principal, null);
			}
			catch
			{
			}
		}

		/// <summary>
		/// Recupera o principal associado com a thread.
		/// </summary>
		/// <param name="thread"></param>
		/// <returns></returns>
		public static System.Security.Principal.IPrincipal GetPrincipal(System.Threading.Thread thread)
		{
			try
			{
				var getLogicalCallContextMethod = typeof(System.Threading.Thread).GetMethod("GetLogicalCallContext", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				var callContext = (System.Runtime.Remoting.Messaging.LogicalCallContext)getLogicalCallContextMethod.Invoke(thread, null);
				var principalProperty = typeof(System.Runtime.Remoting.Messaging.LogicalCallContext).GetProperty("Principal", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				return principalProperty.GetValue(callContext, null) as System.Security.Principal.IPrincipal;
			}
			catch
			{
			}
			return null;
		}

		/// <summary>
		/// Define a entidade.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="token"></param>
		/// <param name="isAuthenticate"></param>
		[System.Security.SecurityCritical]
		private void SetIdentity(string userName, string token, bool isAuthenticate)
		{
			if(Principal.Identity is Security.Principal.DefaultIdentity)
			{
				var identity = (Security.Principal.DefaultIdentity)Principal.Identity;
				identity.Name = userName;
				identity.Token = token;
				identity.IsAuthenticated = isAuthenticate;
			}
			ProfileContext.CurrentProfileData = null;
			ProfileContext.Profile = null;
			ProfileContext.ProfileInfo = null;
			if(!ConfigureThread(System.Threading.Thread.CurrentThread))
				System.Threading.Thread.CurrentPrincipal = Principal;
			lock (_contextThreads)
			{
				foreach (var contextThread in _contextThreads)
				{
					ConfigureThread(contextThread);
				}
			}
			RaisePropertyChanged("User", "Token");
		}

		/// <summary>
		/// Dispara a notificação de alteração de uma propriedade.
		/// </summary>
		/// <param name="propertyNames"></param>
		protected void RaisePropertyChanged(params string[] propertyNames)
		{
			if(PropertyChanged != null && propertyNames != null)
				foreach (var propertyName in propertyNames)
					PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Configura o contexto do usuário.
		/// </summary>
		/// <param name="thread"></param>
		/// <param name="principalGetter">Ponteiro do método usado para recuperar a instancia do principal.</param>
		public static void Setup(System.Threading.Thread thread = null, Func<UserContext, System.Security.Principal.IPrincipal> principalGetter = null)
		{
			_principalGetter = principalGetter;
			if(thread == null)
				thread = System.Threading.Thread.CurrentThread;
			lock (_contextThreads)
			{
				if(!_contextThreads.Any(f => f.ManagedThreadId == thread.ManagedThreadId))
				{
					if(Current != null)
						Current.ConfigureThread(thread);
					_contextThreads.Enqueue(thread);
				}
			}
		}

		/// <summary>
		/// Remove a configuração.
		/// </summary>
		public static void UnSetup()
		{
			lock (_contextThreads)
			{
				var thread = System.Threading.Thread.CurrentThread;
				if(_contextThreads.Any(f => f.ManagedThreadId == thread.ManagedThreadId))
				{
					var aux = new Queue<System.Threading.Thread>();
					while (_contextThreads.Count > 0)
					{
						var i = _contextThreads.Dequeue();
						if(i != thread)
							aux.Enqueue(i);
					}
					while (aux.Count > 0)
						_contextThreads.Enqueue(aux.Dequeue());
				}
			}
		}

		/// <summary>
		/// Libera o usuário do atual contexto.
		/// </summary>
		[System.Security.SecurityCritical]
		public virtual bool SignOut()
		{
			var result = Membership.Provider != null && Membership.LogOut(Token);
			if(result)
				SetIdentity("", "", false);
			return result;
		}

		/// <summary>
		/// Define os parametros de autenticação.
		/// </summary>
		/// <param name="userName">Nome do usuário autenticado.</param>
		/// <param name="token">Token da autenticação.</param>
		[System.Security.SecurityCritical]
		public virtual void SetAuth(string userName, string token)
		{
			SetIdentity(userName, token, true);
		}

		/// <summary>
		/// Define os parametros de autenticação.
		/// </summary>
		/// <param name="user">Instancia com os dados do usuário.</param>
		/// <param name="token">Token da autenticação.</param>
		[System.Security.SecurityCritical]
		public virtual void SetAuth(IUser user, string token)
		{
			user.Require("user").NotNull();
			_user = user;
			SetIdentity(user.UserName, token, true);
		}

		/// <summary>
		/// Realiza a autenticação do usuário no sistema.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="parameters">Parametros que serão usados na autenticação.</param>
		/// <returns></returns>
		public virtual IValidateUserResult Authenticate(string userName, string password, params SecurityParameter[] parameters)
		{
			var result = Membership.ValidateUser(userName, password, parameters);
			if(result.Status == AuthenticationStatus.Success)
				User = result.User;
			else
				User = null;
			return result;
		}

		/// <summary>
		/// Valida os dados do token.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public virtual IValidateUserResult ValidateToken(string token)
		{
			var result = Membership.ValidateToken(token);
			if(result.Status == AuthenticationStatus.Success)
				User = result.User;
			else
				User = null;
			return result;
		}

		/// <summary>
		/// Executa uma verificação do token no servidor.
		/// </summary>
		/// <returns></returns>
		public TokenPingResult Ping()
		{
			return Tokens.Ping(Token);
		}

		/// <summary>
		/// Marca as mensagens como lidas.
		/// </summary>
		/// <param name="dispatcherIds">Identificadores dos despachos.</param>
		public void MarkMessageAsRead(IEnumerable<int> dispatcherIds)
		{
			Tokens.MarkMessageAsRead(dispatcherIds);
		}

		/// <summary>
		/// Recupera os dados serializados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase"), System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("ProfileContext", _profileContext, typeof(Profile.ProfileContext));
			info.AddValue("Principal", _principal, typeof(Security.Principal.DefaultPrincipal));
		}
	}
}
