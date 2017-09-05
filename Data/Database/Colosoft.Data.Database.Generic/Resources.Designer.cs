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

namespace Colosoft.Data.Database.Generic.Properties
{
	using System;

	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	public class Resources
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
		public static global::System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if(object.ReferenceEquals(resourceMan, null))
				{
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Data.Database.Generic.Properties.Resources", typeof(Resources).Assembly);
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
		public static global::System.Globalization.CultureInfo Culture
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
		///   Looks up a localized string similar to Database command was not executed successfully.
		///{0}.
		/// </summary>
		public static string Exception_ExecuteDatabaseCommand
		{
			get
			{
				return ResourceManager.GetString("Exception_ExecuteDatabaseCommand", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Identity of table &apos;{0}&apos; is not integer..
		/// </summary>
		public static string Exception_IdentityNotInteger
		{
			get
			{
				return ResourceManager.GetString("Exception_IdentityNotInteger", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Zero rows affected on persistence operation of entity {0} with row version &apos;{1}&apos;..
		/// </summary>
		public static string Exception_RowsNotAffected
		{
			get
			{
				return ResourceManager.GetString("Exception_RowsNotAffected", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Persistence action of type &quot;{0}&quot; not supported..
		/// </summary>
		public static string NotImplementedException_PersistenceTypeNotSupported
		{
			get
			{
				return ResourceManager.GetString("NotImplementedException_PersistenceTypeNotSupported", resourceCulture);
			}
		}
	}
}
