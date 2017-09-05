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

namespace Colosoft.Query.Database.Properties
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
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Query.Database.Properties.Resources", typeof(Resources).Assembly);
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
		///   Looks up a localized string similar to Entity fullname is empty..
		/// </summary>
		internal static string ArgumentException_EntityFullNameIsEmpty
		{
			get
			{
				return ResourceManager.GetString("ArgumentException_EntityFullNameIsEmpty", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to The parameter &apos;{0}&apos; is duplicated..
		/// </summary>
		internal static string ConditionalParserException_DuplicateParameter
		{
			get
			{
				return ResourceManager.GetString("ConditionalParserException_DuplicateParameter", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Property &apos;{0}&apos; not mapped for type &apos;{1}&apos;..
		/// </summary>
		internal static string Exception_PropertyNotMappedForType
		{
			get
			{
				return ResourceManager.GetString("Exception_PropertyNotMappedForType", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to There is an group by entry empty..
		/// </summary>
		internal static string InvalidOperationException_FoundEmptyGroupByEntry
		{
			get
			{
				return ResourceManager.GetString("InvalidOperationException_FoundEmptyGroupByEntry", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Not found entity from alias &apos;{0}&apos;..
		/// </summary>
		internal static string InvalidOperationException_NotFoundEntityFromAlias
		{
			get
			{
				return ResourceManager.GetString("InvalidOperationException_NotFoundEntityFromAlias", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Not found properties for type metadata &apos;{0}&apos;..
		/// </summary>
		internal static string InvalidOperationException_NotFoundPropertiesForTypeMetadata
		{
			get
			{
				return ResourceManager.GetString("InvalidOperationException_NotFoundPropertiesForTypeMetadata", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Type metadata not found by fullname &apos;{0}&apos;..
		/// </summary>
		internal static string InvalidOperationException_TypeMetadataNotFoundByFullName
		{
			get
			{
				return ResourceManager.GetString("InvalidOperationException_TypeMetadataNotFoundByFullName", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to The type &apos;{0}&apos; has a composite primary key and does not support TableId..
		/// </summary>
		internal static string NotSupportedException_TableIdOnlySupportedInNonCompositePrimaryKeyTable
		{
			get
			{
				return ResourceManager.GetString("NotSupportedException_TableIdOnlySupportedInNonCompositePrimaryKeyTable", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to The type &apos;{0}&apos; of ConditionalTerm is not supported. .
		/// </summary>
		internal static string NotSupportedException_TypeOfConditionalTermNotSupported
		{
			get
			{
				return ResourceManager.GetString("NotSupportedException_TypeOfConditionalTermNotSupported", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to SqlQueryDataSource &apos;{0}&apos; undefined..
		/// </summary>
		internal static string SqlQueryDataSourceUndefined
		{
			get
			{
				return ResourceManager.GetString("SqlQueryDataSourceUndefined", resourceCulture);
			}
		}
	}
}
