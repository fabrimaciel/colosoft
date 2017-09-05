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

namespace Colosoft.Security.Profile
{
	/// <summary>
	/// Representa os argumentos do evento acionado quando o atual perfil for alterado.
	/// </summary>
	public class CurrentProfileChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Instancia do novo perfil.
		/// </summary>
		public ProfileInfo ProfileInfo
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Classe usada para gerenciar o perfil carregado no sistema.
	/// </summary>
	public static class ProfileManager
	{
		private static IProfileProvider _provider;

		private static bool _initialized;

		private static Exception _initializeException;

		private static object _initializeLock = new object();

		private static List<IProfileProvider> _providers;

		private static string _defaultProviderName;

		private static ICurrentProfileInfoProvider _currentProfileInfoProvider;

		/// <summary>
		/// Evento acionado quando o perfil atual for alterado.
		/// </summary>
		public static event EventHandler<CurrentProfileChangedEventArgs> CurrentProfileChanged;

		/// <summary>
		/// Contexto do perfil.
		/// </summary>
		private static ProfileContext Context
		{
			get
			{
				return UserContext.Current.ProfileContext;
			}
		}

		/// <summary>
		/// Armazena a lista dos provedores carregados.
		/// </summary>
		public static List<IProfileProvider> Providers
		{
			get
			{
				Initialize();
				return _providers;
			}
		}

		/// <summary>
		/// Nome do perfil padrão.
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
		/// Provedor padrão do sistema.
		/// </summary>
		public static IProfileProvider Provider
		{
			get
			{
				try
				{
					Initialize();
				}
				catch
				{
					_provider = null;
				}
				return _provider;
			}
		}

		/// <summary>
		/// Recupera a instancia do perfil do usuário.
		/// </summary>
		public static IProfile CurrentProfile
		{
			get
			{
				var principal = UserContext.Current.Principal;
				if(principal == null || principal.Identity == null)
					return null;
				if(Context.Profile == null)
				{
					if(Provider == null)
						return null;
					var profileInfo = CurrentProfileInfo;
					if(principal.Identity.IsAuthenticated && profileInfo != null)
						Context.Profile = Provider.GetProfile(profileInfo);
				}
				else if(string.Compare(Context.Profile.UserName, principal.Identity.Name, true) != 0 || !principal.Identity.IsAuthenticated)
				{
					Context.Profile = null;
				}
				return Context.Profile;
			}
		}

		/// <summary>
		/// Recupera as informações do atual perfil.
		/// </summary>
		public static ProfileInfo CurrentProfileInfo
		{
			get
			{
				if(Context.CurrentProfileData != null)
				{
					lock (Context.CurrentProfileDataLock)
					{
						if(Context.CurrentProfileData != null)
						{
							ProfileInfo profileInfo2 = null;
							var profileData = Context.CurrentProfileData;
							try
							{
								profileInfo2 = profileData.ProfileInfo != null ? profileData.ProfileInfo.Value : null;
							}
							catch
							{
								profileInfo2 = null;
							}
							if(profileInfo2 != null)
								SetCurrentProfile(profileInfo2, profileData.IgnoreTokenProvider);
							Context.CurrentProfileData = null;
						}
					}
				}
				if(UserContext.Current != null)
				{
					var principal = UserContext.Current.Principal;
					if(principal == null || principal.Identity == null)
						return null;
					if(principal.Identity.IsAuthenticated)
						return Context.ProfileInfo;
				}
				return null;
			}
		}

		/// <summary>
		/// Instancia do provedro das informações do perfil
		/// </summary>
		public static ICurrentProfileInfoProvider CurrentProfileInfoProvider
		{
			get
			{
				return _currentProfileInfoProvider;
			}
			set
			{
				if(_currentProfileInfoProvider != null)
					_currentProfileInfoProvider.ProfileInfoChanged -= CurrentProfileInfoProviderProfileInfoChanged;
				_currentProfileInfoProvider = value;
				if(_currentProfileInfoProvider != null)
					_currentProfileInfoProvider.ProfileInfoChanged += CurrentProfileInfoProviderProfileInfoChanged;
			}
		}

		/// <summary>
		/// Método acionado quando as informações do perfil do atual usuário forem alteradas.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void CurrentProfileInfoProviderProfileInfoChanged(object sender, CurrentProfileChangedEventArgs e)
		{
			throw new NotImplementedException();
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
				lock (_initializeLock)
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
							_providers = new List<IProfileProvider>();
							ServiceLocatorValidator.Validate();
							foreach (var provider in Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetAllInstances<IProfileProvider>())
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
							_initializeException = new InvalidOperationException(Properties.Resources.InvalidOperation_DefaultProfileProviderNotFound);
							throw _initializeException;
						}
						_initialized = true;
					}
				}
			}
		}

		/// <summary>
		/// Define as informações do perfil.
		/// </summary>
		/// <param name="info">Lazy das informações do perfil</param>
		/// <param name="ignoreTokenProvider"></param>
		public static void SetCurrentProfile(Lazy<ProfileInfo> info, bool ignoreTokenProvider)
		{
			SetCurrentProfile(info, ignoreTokenProvider, UserContext.Current);
		}

		/// <summary>
		/// Define as informações do perfil.
		/// </summary>
		/// <param name="info">Lazy das informações do perfil</param>
		/// <param name="ignoreTokenProvider"></param>
		/// <param name="context">Contexto do usuário.</param>
		public static void SetCurrentProfile(Lazy<ProfileInfo> info, bool ignoreTokenProvider, UserContext context)
		{
			context.ProfileContext.CurrentProfileData = new SetCurrentProfileData {
				ProfileInfo = info,
				IgnoreTokenProvider = ignoreTokenProvider
			};
		}

		/// <summary>
		/// Defin o profile que deve ser carregado para o sistema.
		/// </summary>
		/// <param name="info"></param>
		public static void SetCurrentProfile(ProfileInfo info)
		{
			SetCurrentProfile(info, false);
		}

		/// <summary>
		/// Defin o profile que deve ser carregado para o sistema.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="ignoreTokenProvider">Identifica se é para ignora a definição no provedor do token.</param>
		public static void SetCurrentProfile(ProfileInfo info, bool ignoreTokenProvider)
		{
			SetCurrentProfile(info, ignoreTokenProvider, UserContext.Current);
		}

		/// <summary>
		/// Defin o profile que deve ser carregado para o sistema.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="ignoreTokenProvider">Identifica se é para ignora a definição no provedor do token.</param>
		/// <param name="userContext">Contexto do usuário.</param>
		public static void SetCurrentProfile(ProfileInfo info, bool ignoreTokenProvider, UserContext userContext)
		{
			var context = userContext.ProfileContext;
			context.ProfileInfo = info;
			if(context.ProfileInfo == null)
				context.Profile = null;
			var principal = userContext.Principal;
			if(principal == null)
			{
				context.Profile = null;
				return;
			}
			var identity = principal.Identity;
			if(identity == null)
			{
				context.Profile = null;
				return;
			}
			if(identity.IsAuthenticated && info != null)
			{
				if(Provider == null)
					throw new InvalidOperationException("Provider is null");
				if(!ignoreTokenProvider)
				{
					var setProfileResult = Tokens.SetProfile(userContext.Token, info.ProfileId);
					if(!setProfileResult.Success)
						throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.SetProfileError, info.FullName, userContext.Token, setProfileResult.FailureMessage).Format());
				}
				context.ProfileInfo = info;
			}
			else
				context.Profile = null;
			if(CurrentProfileChanged != null)
				CurrentProfileChanged(null, new CurrentProfileChangedEventArgs {
					ProfileInfo = context.ProfileInfo
				});
		}

		/// <summary>
		/// Define o perfil
		/// </summary>
		/// <param name="profile"></param>
		public static void SetCurrentProfile(IProfile profile)
		{
			SetCurrentProfile(profile, false);
		}

		/// <summary>
		/// Define o perfil
		/// </summary>
		/// <param name="profile"></param>
		/// <param name="ignoreTokenProvider">Identifica se é para ignora a definição no provedor do token.</param>
		public static void SetCurrentProfile(IProfile profile, bool ignoreTokenProvider)
		{
			var principal = UserContext.Current.Principal;
			Context.Profile = profile;
			if(principal == null)
			{
				Context.Profile = null;
				Context.ProfileInfo = null;
				return;
			}
			var identity = principal.Identity;
			if(identity == null)
			{
				Context.Profile = null;
				Context.ProfileInfo = null;
				return;
			}
			if(identity.IsAuthenticated && profile != null)
			{
				if(!ignoreTokenProvider)
				{
					var setProfileResult = Tokens.SetProfile(UserContext.Current.Token, profile.ProfileId);
					if(!setProfileResult.Success)
						throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.SetProfileError, profile.FullName, UserContext.Current.Token, setProfileResult.FailureMessage).Format());
				}
				Context.ProfileInfo = profile.GetInfo();
			}
			else
			{
				Context.ProfileInfo = null;
				Context.Profile = null;
			}
			if(CurrentProfileChanged != null)
				CurrentProfileChanged(null, new CurrentProfileChangedEventArgs {
					ProfileInfo = Context.ProfileInfo
				});
		}

		/// <summary>
		/// Armazena os dados do perfil informado.
		/// </summary>
		[Serializable]
		internal class SetCurrentProfileData
		{
			/// <summary>
			/// Informações do perfil.
			/// </summary>
			public Lazy<ProfileInfo> ProfileInfo;

			/// <summary>
			/// Identifica se é para ignorar o token provider.
			/// </summary>
			public bool IgnoreTokenProvider;
		}
	}
}
