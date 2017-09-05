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

namespace Colosoft.Query.Properties
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
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Query.Properties.Resources", typeof(Resources).Assembly);
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
		///   Looks up a localized string similar to A parameter with the name &apos;{0}&apos; already exists on the procedure &apos;{1}&apos;..
		/// </summary>
		internal static string ArgumentException_ParameterWithSameNameAlreadyExists
		{
			get
			{
				return ResourceManager.GetString("ArgumentException_ParameterWithSameNameAlreadyExists", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to WhereClause missing in query..
		/// </summary>
		internal static string ArgumentNullException_WhereClause
		{
			get
			{
				return ResourceManager.GetString("ArgumentNullException_WhereClause", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to DataSource undefined..
		/// </summary>
		internal static string DataSourceUndefined
		{
			get
			{
				return ResourceManager.GetString("DataSourceUndefined", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Source context not in the dependency injection container.
		/// </summary>
		internal static string Exception_FailOnLoadDataSourceToSourceContext
		{
			get
			{
				return ResourceManager.GetString("Exception_FailOnLoadDataSourceToSourceContext", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Not found empty constructor for type &apos;{0}&apos;..
		/// </summary>
		internal static string Exception_NotFoundEmptyConstructorForType
		{
			get
			{
				return ResourceManager.GetString("Exception_NotFoundEmptyConstructorForType", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Context undefined..
		/// </summary>
		internal static string InvalidOperationException_ContextUndefined
		{
			get
			{
				return ResourceManager.GetString("InvalidOperationException_ContextUndefined", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Provider of context undefined..
		/// </summary>
		internal static string InvalidOperationException_ProviderOfContextUndefined
		{
			get
			{
				return ResourceManager.GetString("InvalidOperationException_ProviderOfContextUndefined", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Invalid checksum for record.
		///Availables fields: {0}.
		/// </summary>
		internal static string InvalidRecordChecksumException_Message
		{
			get
			{
				return ResourceManager.GetString("InvalidRecordChecksumException_Message", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Cannot possible execute all queries for multiquery..
		/// </summary>
		internal static string MultiQueryable_MultiQueryableExecuteErrorProcess
		{
			get
			{
				return ResourceManager.GetString("MultiQueryable_MultiQueryableExecuteErrorProcess", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocurred until deserialize field &apos;{0}&apos; of type &apos;{1}&apos;. &apos;{2}&apos;.
		/// </summary>
		internal static string Record_DeserializeFieldError
		{
			get
			{
				return ResourceManager.GetString("Record_DeserializeFieldError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocurred until serialize field &apos;{0}&apos; of type &apos;{1}&apos;. &apos;{2}&apos;.
		/// </summary>
		internal static string Record_SerializeFieldError
		{
			get
			{
				return ResourceManager.GetString("Record_SerializeFieldError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Field &apos;{0}&apos; not found on record.
		///Availables fields: {1}.
		/// </summary>
		internal static string RecordFieldNotFoundException_Message
		{
			get
			{
				return ResourceManager.GetString("RecordFieldNotFoundException_Message", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Field &apos;{0}&apos; not found on record.
		///Availables fields: {1}
		///Comand:{2}.
		/// </summary>
		internal static string RecordFieldNotFoundExceptionWithQueryCommand_Message
		{
			get
			{
				return ResourceManager.GetString("RecordFieldNotFoundExceptionWithQueryCommand_Message", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Service locator undefined..
		/// </summary>
		internal static string ServiceLocatorUndefined
		{
			get
			{
				return ResourceManager.GetString("ServiceLocatorUndefined", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocurred when bind value to property &apos;{0}&apos; from type &apos;{1}&apos;. Object of type &apos;{2}&apos; cannot be converted to type &apos;{3}&apos;..
		/// </summary>
		internal static string TypeBindStrategy_ConvertValueError
		{
			get
			{
				return ResourceManager.GetString("TypeBindStrategy_ConvertValueError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocorred when bind value. Object of type &apos;{0}&apos; cannot be converted to type &apos;{1}&apos;.
		/// </summary>
		internal static string TypeBindStrategy_ConvertValueError2
		{
			get
			{
				return ResourceManager.GetString("TypeBindStrategy_ConvertValueError2", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Duplicate field with name &apos;{0}&apos;.
		///TypeBinding: {1}
		///Descriptor: {2}.
		/// </summary>
		internal static string TypeBindStrategy_DuplicateFieldName
		{
			get
			{
				return ResourceManager.GetString("TypeBindStrategy_DuplicateFieldName", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocurred when get value of property &apos;{0}&apos; from instance of type &apos;{1}&apos;..
		/// </summary>
		internal static string TypeBindStrategy_GetPropertyValueError
		{
			get
			{
				return ResourceManager.GetString("TypeBindStrategy_GetPropertyValueError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Query result field no match with projection field.
		///Project fields: {0}
		///Result fields: {1}.
		/// </summary>
		internal static string ValidationQueryResult_NotMatchFields
		{
			get
			{
				return ResourceManager.GetString("ValidationQueryResult_NotMatchFields", resourceCulture);
			}
		}
	}
}
