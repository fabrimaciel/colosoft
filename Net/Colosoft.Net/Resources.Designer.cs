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

namespace Colosoft.Net.Properties
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
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Net.Properties.Resources", typeof(Resources).Assembly);
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
		///   Looks up a localized string similar to Adicionando item duplicado.
		/// </summary>
		internal static string Argument_AddingDuplicate
		{
			get
			{
				return ResourceManager.GetString("Argument_AddingDuplicate", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O nó do endereço do serviço é inválido..
		/// </summary>
		internal static string Argument_InvalidServiceAddressNode
		{
			get
			{
				return ResourceManager.GetString("Argument_InvalidServiceAddressNode", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Ocorreu um erro ao verificar o token da requisição.
		/// </summary>
		internal static string Exception_TokenCheckingError
		{
			get
			{
				return ResourceManager.GetString("Exception_TokenCheckingError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Token vazio.
		/// </summary>
		internal static string FaultException_EmptyToken
		{
			get
			{
				return ResourceManager.GetString("FaultException_EmptyToken", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Token inválido..
		/// </summary>
		internal static string FaultException_InvalidToken
		{
			get
			{
				return ResourceManager.GetString("FaultException_InvalidToken", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Ocorreu um erro ao definir os parametros de autenticação..
		/// </summary>
		internal static string FaultException_SetAuthError
		{
			get
			{
				return ResourceManager.GetString("FaultException_SetAuthError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Impossível recupera o endereço &apos;{0}&apos; do provedor..
		/// </summary>
		internal static string ImpossibleRecoverAddressProviderAddress
		{
			get
			{
				return ResourceManager.GetString("ImpossibleRecoverAddressProviderAddress", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Binding node not found in configuration &apos;{0}&apos;..
		/// </summary>
		internal static string InvalidOperation_BindingNodeNotFound
		{
			get
			{
				return ResourceManager.GetString("InvalidOperation_BindingNodeNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Endereço &apos;{0}&apos; não encontrado..
		/// </summary>
		internal static string ServiceConfiguration_AddressNotFound
		{
			get
			{
				return ResourceManager.GetString("ServiceConfiguration_AddressNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Endereço &apos;{0}&apos; não encontrado no provedor de endereços com o contexto &apos;{1}&apos;..
		/// </summary>
		internal static string ServiceConfiguration_AddressNotFoundInAddressProvider
		{
			get
			{
				return ResourceManager.GetString("ServiceConfiguration_AddressNotFoundInAddressProvider", resourceCulture);
			}
		}
	}
}
