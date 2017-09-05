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

namespace Colosoft.Data.Schema.Properties
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
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Data.Schema.Properties.Resources", typeof(Resources).Assembly);
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
		///   Looks up a localized string similar to Property {0} not found in mapper for {1}..
		/// </summary>
		internal static string Exception_SchemaPersistenceSession_PropertyNotFoundInMapping
		{
			get
			{
				return ResourceManager.GetString("Exception_SchemaPersistenceSession_PropertyNotFoundInMapping", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Type metadata &apos;{0}&apos; not found..
		/// </summary>
		internal static string Exception_TypeMetadataNotFound
		{
			get
			{
				return ResourceManager.GetString("Exception_TypeMetadataNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocurred when get property type &apos;{0}&apos; from property &apos;{1}&apos; of entity &apos;{2}&apos;..
		/// </summary>
		internal static string PersistenceActionResultProcessor_GetPropertyTypeFromPropertyMetadataError
		{
			get
			{
				return ResourceManager.GetString("PersistenceActionResultProcessor_GetPropertyTypeFromPropertyMetadataError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Existe um ação de inserção duplicada da entidade &apos;{0}&apos; com o VirtualId &apos;{1}&apos;..
		/// </summary>
		internal static string SchemaPersistenceSessionValidator_DuplicateInsertionAction
		{
			get
			{
				return ResourceManager.GetString("SchemaPersistenceSessionValidator_DuplicateInsertionAction", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Foi informado um valor inválido (&apos;{0}&apos;) para a propriedade identidade &apos;{1}&apos; da entidade &apos;{2}&apos; na operação de inserção..
		/// </summary>
		internal static string SchemaPersistenceSessionValidator_NonNegativeIdentityProperty
		{
			get
			{
				return ResourceManager.GetString("SchemaPersistenceSessionValidator_NonNegativeIdentityProperty", resourceCulture);
			}
		}
	}
}
