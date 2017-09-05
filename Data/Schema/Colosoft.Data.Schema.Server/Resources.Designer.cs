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

namespace Colosoft.Data.Schema.Server.Properties
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
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Data.Schema.Server.Properties.Resources", typeof(Resources).Assembly);
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
		///   Looks up a localized string similar to Assembly &apos;{0}&apos; not mapped into the database..
		/// </summary>
		internal static string AssemblyNotMapped
		{
			get
			{
				return ResourceManager.GetString("AssemblyNotMapped", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Fail on load table and property data..
		/// </summary>
		internal static string Exception_FailOnLoadTableAndPropertyData
		{
			get
			{
				return ResourceManager.GetString("Exception_FailOnLoadTableAndPropertyData", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Fault on load properties metadata..
		/// </summary>
		internal static string Exception_FaultOnLoadPropertiesMetadata
		{
			get
			{
				return ResourceManager.GetString("Exception_FaultOnLoadPropertiesMetadata", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Fault on load types metadata..
		/// </summary>
		internal static string Exception_FaultOnLoadTypesMetadata
		{
			get
			{
				return ResourceManager.GetString("Exception_FaultOnLoadTypesMetadata", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Identity of table &apos;{0}&apos; is not integer..
		/// </summary>
		internal static string Exception_IdentityNotInteger
		{
			get
			{
				return ResourceManager.GetString("Exception_IdentityNotInteger", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Property &apos;{0}&apos; not found in type &apos;{1}&apos;..
		/// </summary>
		internal static string Exception_PropertyNotFound
		{
			get
			{
				return ResourceManager.GetString("Exception_PropertyNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to ForeignMember &apos;{0}&apos; of virtual id {1} and type &apos;{2}&apos; not inserted into the database.
		/// </summary>
		internal static string ForeignMemberNotInserted
		{
			get
			{
				return ResourceManager.GetString("ForeignMemberNotInserted", resourceCulture);
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
		///   Looks up a localized string similar to Property &apos;{0}&apos; already exists on TypeMetadata &apos;{1}&apos;..
		/// </summary>
		internal static string PropertyAlreadyExistsTypeMetadata
		{
			get
			{
				return ResourceManager.GetString("PropertyAlreadyExistsTypeMetadata", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to TypeCode of PropertyMetadata &apos;{0}&apos; not matched with TypeCode of TypeMetadata &apos;{1}&apos;..
		/// </summary>
		internal static string PropertyAndTypeNotMatched
		{
			get
			{
				return ResourceManager.GetString("PropertyAndTypeNotMatched", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Propertycode &apos;{0}&apos; not mapped into the database..
		/// </summary>
		internal static string PropertyCodeNotMapped
		{
			get
			{
				return ResourceManager.GetString("PropertyCodeNotMapped", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Typecode &apos;{0}&apos; not mapped into the database..
		/// </summary>
		internal static string TypeCodeNotMapped
		{
			get
			{
				return ResourceManager.GetString("TypeCodeNotMapped", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Type &apos;{0}&apos; not mapped into the database..
		/// </summary>
		internal static string TypeNotMapped
		{
			get
			{
				return ResourceManager.GetString("TypeNotMapped", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Virtual id of inserting entity &apos;{0}&apos; is not negative.
		/// </summary>
		internal static string VirtualIdNotNegative
		{
			get
			{
				return ResourceManager.GetString("VirtualIdNotNegative", resourceCulture);
			}
		}
	}
}
