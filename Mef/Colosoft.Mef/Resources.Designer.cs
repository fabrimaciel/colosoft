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

namespace Colosoft.Mef.Properties
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
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Mef.Properties.Resources", typeof(Resources).Assembly);
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
		///   Looks up a localized string similar to Exists duplicate export definition [ContractType: {0}, ContractName: {1}].
		/// </summary>
		internal static string AssemblyRepositoryCatalog_DuplicateExport
		{
			get
			{
				return ResourceManager.GetString("AssemblyRepositoryCatalog_DuplicateExport", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocurred when load parameter &apos;{0} : {1}&apos; or member &apos;{2}&apos;.
		/// </summary>
		internal static string ComposableMemberParameterExcepton_Message
		{
			get
			{
				return ResourceManager.GetString("ComposableMemberParameterExcepton_Message", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Not found export definition for type &apos;{0}&apos;..
		/// </summary>
		internal static string ExportConfigurator_NotFoundExportForType
		{
			get
			{
				return ResourceManager.GetString("ExportConfigurator_NotFoundExportForType", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Assembly &apos;{0}&apos; from export &apos;{1}&apos; not found..
		/// </summary>
		internal static string InvalidOperation_AssemblyFromExportNotFound
		{
			get
			{
				return ResourceManager.GetString("InvalidOperation_AssemblyFromExportNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Fail on get packages..
		/// </summary>
		internal static string InvalidOperation_FailOnGetPackages
		{
			get
			{
				return ResourceManager.GetString("InvalidOperation_FailOnGetPackages", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Assembly &apos;{0}&apos; not found..
		/// </summary>
		internal static string LoggerAssemblyRepositoryCatalogObserver_AssemblyFromExportNotFound
		{
			get
			{
				return ResourceManager.GetString("LoggerAssemblyRepositoryCatalogObserver_AssemblyFromExportNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Ocorreu uma falha e não foi possível carregar o assembly &quot;{0}&quot;. Detalhes: {1}.
		/// </summary>
		internal static string LoggerAssemblyRepositoryCatalogObserver_FailOnAssembly
		{
			get
			{
				return ResourceManager.GetString("LoggerAssemblyRepositoryCatalogObserver_FailOnAssembly", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Ocorreu uma falha e não foi possível carregar o pacote de assemblies (&quot;{0}&quot;). Detalhes: {1}.
		/// </summary>
		internal static string LoggerAssemblyRepositoryCatalogObserver_FailOnLoadPackages
		{
			get
			{
				return ResourceManager.GetString("LoggerAssemblyRepositoryCatalogObserver_FailOnLoadPackages", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Ocorreu uma falha e não foi possível carregar o tipo &quot;{0}&quot;. Detalhes: {1}.
		/// </summary>
		internal static string LoggerAssemblyRepositoryCatalogObserver_FailOnLoadType
		{
			get
			{
				return ResourceManager.GetString("LoggerAssemblyRepositoryCatalogObserver_FailOnLoadType", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Not supported read only dictionary.
		/// </summary>
		internal static string NotSupportedReadOnlyDictionary
		{
			get
			{
				return ResourceManager.GetString("NotSupportedReadOnlyDictionary", resourceCulture);
			}
		}
	}
}
