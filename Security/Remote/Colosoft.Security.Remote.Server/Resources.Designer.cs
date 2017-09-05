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

namespace Colosoft.Security.Remote.Server.Properties
{
	using System;

	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class Resources
	{
		private static global::System.Resources.ResourceManager resourceMan;

		private static global::System.Globalization.CultureInfo resourceCulture;

		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Resources()
		{
		}

		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if(object.ReferenceEquals(resourceMan, null))
				{
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Security.Remote.Server.Properties.Resources", typeof(Resources).Assembly);
					resourceMan = temp;
				}
				return resourceMan;
			}
		}

		/// <summary>
		///   Overrides the current thread's CurrentUICulture property for all
		///   resource lookups using this strongly typed resource class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Globalization.CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Usuário necessita de captcha para autenticar.
		/// </summary>
		internal static string Captcha_Necessary
		{
			get
			{
				return ResourceManager.GetString("Captcha_Necessary", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Erro ao inserir controle de token.
		/// </summary>
		internal static string Error_TokenCreate
		{
			get
			{
				return ResourceManager.GetString("Error_TokenCreate", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to There is already an open token for the current user.
		/// </summary>
		internal static string Error_TokenDuplicate
		{
			get
			{
				return ResourceManager.GetString("Error_TokenDuplicate", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Usuário não pode redefinir senha.
		/// </summary>
		internal static string Error_UserCantResetPassword
		{
			get
			{
				return ResourceManager.GetString("Error_UserCantResetPassword", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Not found addresses for service &apos;{0}&apos;..
		/// </summary>
		internal static string Exception_NotFoundAddressesForService
		{
			get
			{
				return ResourceManager.GetString("Exception_NotFoundAddressesForService", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Captcha incorreto.
		/// </summary>
		internal static string Invalid_Captcha
		{
			get
			{
				return ResourceManager.GetString("Invalid_Captcha", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Nome de usuário ou senha inválidos.
		/// </summary>
		internal static string Invalid_UsernameOrPassword
		{
			get
			{
				return ResourceManager.GetString("Invalid_UsernameOrPassword", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não possível recupera o provedor de endereços dos serviços..
		/// </summary>
		internal static string InvalidOperationException_FailOnGetServiceAddressProvider
		{
			get
			{
				return ResourceManager.GetString("InvalidOperationException_FailOnGetServiceAddressProvider", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Server configuration not found for Colosoft.Security.Remote.Server..
		/// </summary>
		internal static string InvalidOperationException_ServerConfigurationNotFound
		{
			get
			{
				return ResourceManager.GetString("InvalidOperationException_ServerConfigurationNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Fluxo de provedor de identidade não encontrado.
		/// </summary>
		internal static string NotFound_IdentityProvideFlow
		{
			get
			{
				return ResourceManager.GetString("NotFound_IdentityProvideFlow", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Ocorreu um erro ao recupera o provedor de perfis.
		///Detalhes: {0}.
		/// </summary>
		internal static string ProfileProviderService_GetProfileProviderError
		{
			get
			{
				return ResourceManager.GetString("ProfileProviderService_GetProfileProviderError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Sua senha expirará em {0} dia(s).
		/// </summary>
		internal static string Warning_Password
		{
			get
			{
				return ResourceManager.GetString("Warning_Password", resourceCulture);
			}
		}
	}
}
