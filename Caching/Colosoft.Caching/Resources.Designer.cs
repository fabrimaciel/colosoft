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

namespace Colosoft.Caching.Properties
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
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Caching.Properties.Resources", typeof(Resources).Assembly);
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
		///   Looks up a localized string similar to CacheJoinRowDescriptor of {0} entitys can not be created from object array of {1} values..
		/// </summary>
		internal static string ArgumentOutOfRangeException_ValuesOutOfDescriptorRange
		{
			get
			{
				return ResourceManager.GetString("ArgumentOutOfRangeException_ValuesOutOfDescriptorRange", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocorred where execute query in cache.
		///Details: {0}.
		/// </summary>
		internal static string CachePersistenceExecuter_ExecuteQueryInCacheError
		{
			get
			{
				return ResourceManager.GetString("CachePersistenceExecuter_ExecuteQueryInCacheError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocurred when get property type &apos;{0}&apos; from property &apos;{1}&apos; of entity &apos;{2}&apos;..
		/// </summary>
		internal static string CachePersistenceExecuter_GetPropertyTypeFromPropertyMetadataError
		{
			get
			{
				return ResourceManager.GetString("CachePersistenceExecuter_GetPropertyTypeFromPropertyMetadataError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to PropertyMetadata &apos;{0}&apos; not found for entity of entity &apos;{1}&apos;..
		/// </summary>
		internal static string CachePersistenceExecuter_PropertyMetadataNotFound
		{
			get
			{
				return ResourceManager.GetString("CachePersistenceExecuter_PropertyMetadataNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to TypeMetadata not found for entity &apos;{0}&apos;..
		/// </summary>
		internal static string CachePersistenceExecuter_TypeMetadataNotFound
		{
			get
			{
				return ResourceManager.GetString("CachePersistenceExecuter_TypeMetadataNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Entity of type &apos;{0}&apos; unsupported for storage in cache..
		/// </summary>
		internal static string CachePersistenceExecuter_UnsupportedEntity
		{
			get
			{
				return ResourceManager.GetString("CachePersistenceExecuter_UnsupportedEntity", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Cannot find the type of cache, invalid configuration for cache class &apos;{0}&apos;.
		/// </summary>
		internal static string ConfigurationException_CannotFindTypeOfCache
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_CannotFindTypeOfCache", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to The class specified does not implement ICacheLoader.
		/// </summary>
		internal static string ConfigurationException_ICacheLoaderNotImplemented
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_ICacheLoaderNotImplemented", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Invalid cache store class: &apos;{0}&apos;.
		/// </summary>
		internal static string ConfigurationException_InvalidCacheStorageClass
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_InvalidCacheStorageClass", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Missing assembly name.
		/// </summary>
		internal static string ConfigurationException_MissingAssemblyName
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_MissingAssemblyName", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Missing assembly name for read-thru option.
		/// </summary>
		internal static string ConfigurationException_MissingAssemblyNameForReadThru
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_MissingAssemblyNameForReadThru", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Missing configuration attribute &apos;{0}&apos;.
		/// </summary>
		internal static string ConfigurationException_MissingAttribute
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_MissingAttribute", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Missing cache store class.
		/// </summary>
		internal static string ConfigurationException_MissingCacheStorageClass
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_MissingCacheStorageClass", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Missing class name.
		/// </summary>
		internal static string ConfigurationException_MissingClassName
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_MissingClassName", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Missing class name for read-thru option.
		/// </summary>
		internal static string ConfigurationException_MissingClassNameForReadThru
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_MissingClassNameForReadThru", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Missing configuration option &apos;{0}&apos;.
		/// </summary>
		internal static string ConfigurationException_MissingOption
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_MissingOption", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Missing configuration section &apos;{0}&apos;.
		/// </summary>
		internal static string ConfigurationException_MissingSection
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_MissingSection", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Cannot find cache class &apos;{0}&apos;.
		/// </summary>
		internal static string ConfigurationException_NotFoundClassCache
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_NotFoundClassCache", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Invalid property string, ) misplaced.
		/// </summary>
		internal static string ConfigurationException_PropsConfigReader_CloseParanthesisMisplaced
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_PropsConfigReader_CloseParanthesisMisplaced", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Invalid property string, ) unexpected.
		/// </summary>
		internal static string ConfigurationException_PropsConfigReader_CloseParenthesisUnexpected
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_PropsConfigReader_CloseParenthesisUnexpected", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Invalid EOF.
		/// </summary>
		internal static string ConfigurationException_PropsConfigReader_InvalidEOF
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_PropsConfigReader_InvalidEOF", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Invalid property string, key following a bad token.
		/// </summary>
		internal static string ConfigurationException_PropsConfigReader_KeyFollowingBadToken
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_PropsConfigReader_KeyFollowingBadToken", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Invalid property string, value following a bad token.
		/// </summary>
		internal static string ConfigurationException_PropsConfigReader_ValueFollowingBadToken
		{
			get
			{
				return ResourceManager.GetString("ConfigurationException_PropsConfigReader_ValueFollowingBadToken", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Limit of aggregation is {0} for this AggregateSet.
		/// </summary>
		internal static string Exception_AggregationLimitReached
		{
			get
			{
				return ResourceManager.GetString("Exception_AggregationLimitReached", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error occured while removing items.
		/// </summary>
		internal static string Exception_AnErrorOccuredWhileRemovingItems
		{
			get
			{
				return ResourceManager.GetString("Exception_AnErrorOccuredWhileRemovingItems", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Could not load assembly &quot;{0}&quot;. {1}.
		/// </summary>
		internal static string Exception_CouldNotLoadAssembly
		{
			get
			{
				return ResourceManager.GetString("Exception_CouldNotLoadAssembly", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Key in named tags conflicts with the indexed attribute name of the specified object..
		/// </summary>
		internal static string Exception_DuplicateIndexName
		{
			get
			{
				return ResourceManager.GetString("Exception_DuplicateIndexName", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Not found index  for attribute &apos;{0}&apos; on type &apos;{1}&apos;.
		/// </summary>
		internal static string Exception_NotFoundIndexForAttributeException
		{
			get
			{
				return ResourceManager.GetString("Exception_NotFoundIndexForAttributeException", resourceCulture);
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
		///   Looks up a localized string similar to Unable to instantiate {0}.
		/// </summary>
		internal static string Exception_UnableToInstantiate
		{
			get
			{
				return ResourceManager.GetString("Exception_UnableToInstantiate", resourceCulture);
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
		///   Looks up a localized string similar to Type metadata not ofund by fullname &apos;{0}&apos;..
		/// </summary>
		internal static string InvalidOperationException_TypeMetadataNotFoundByFullName
		{
			get
			{
				return ResourceManager.GetString("InvalidOperationException_TypeMetadataNotFoundByFullName", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Type {0} is not a supported as a ConditionalTerm..
		/// </summary>
		internal static string NotSupportedException_ConditionalTermTypeNotSupported
		{
			get
			{
				return ResourceManager.GetString("NotSupportedException_ConditionalTermTypeNotSupported", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Constant {0} has invalid format..
		/// </summary>
		internal static string NotSupportedException_ConstantTypeNotRecognized
		{
			get
			{
				return ResourceManager.GetString("NotSupportedException_ConstantTypeNotRecognized", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Cache don&apos;t support getting the persistence properties..
		/// </summary>
		internal static string NotSupportedException_GetPropertiesInCacheNotSupported
		{
			get
			{
				return ResourceManager.GetString("NotSupportedException_GetPropertiesInCacheNotSupported", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Cache don&apos;t support complex sort expressions.
		/// </summary>
		internal static string NotSupportedException_InvalidSortEntryFormat
		{
			get
			{
				return ResourceManager.GetString("NotSupportedException_InvalidSortEntryFormat", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Operator {0} not supported..
		/// </summary>
		internal static string NotSupportedException_OperatorNotSupported
		{
			get
			{
				return ResourceManager.GetString("NotSupportedException_OperatorNotSupported", resourceCulture);
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
		///   Looks up a localized string similar to The type &apos;{0}&apos; of ConditionalTerm is not supported. .
		/// </summary>
		internal static string NotSupportedException_TypeOfConditionalTermNotSupported1
		{
			get
			{
				return ResourceManager.GetString("NotSupportedException_TypeOfConditionalTermNotSupported1", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Methods were called after the stream was closed..
		/// </summary>
		internal static string ObjectDisposableException_MethodsWereCalledAfterClose
		{
			get
			{
				return ResourceManager.GetString("ObjectDisposableException_MethodsWereCalledAfterClose", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to IReadThruProvider.LoadFromSource failed.
		/// </summary>
		internal static string OperationFailedException_IReadThruProviderLoadFromSourceFailed
		{
			get
			{
				return ResourceManager.GetString("OperationFailedException_IReadThruProviderLoadFromSourceFailed", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Index is not defined for &apos;{0}&apos;.
		/// </summary>
		internal static string ParserException_IndexIsNotDefined
		{
			get
			{
				return ResourceManager.GetString("ParserException_IndexIsNotDefined", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Incorrect query format. &apos;*&apos; is not supported..
		/// </summary>
		internal static string ParserException_StartIsNotSupported
		{
			get
			{
				return ResourceManager.GetString("ParserException_StartIsNotSupported", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Stream is already locked.
		/// </summary>
		internal static string StreamAlreadyLockedException_StreamIsAlreadyLocked
		{
			get
			{
				return ResourceManager.GetString("StreamAlreadyLockedException_StreamIsAlreadyLocked", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Invalid lock handle.
		/// </summary>
		internal static string StreamInvalidLockException_InvalidLockHandle
		{
			get
			{
				return ResourceManager.GetString("StreamInvalidLockException_InvalidLockHandle", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Stream not found in the cache..
		/// </summary>
		internal static string StreamNotFoundException_StreamNotFound
		{
			get
			{
				return ResourceManager.GetString("StreamNotFoundException_StreamNotFound", resourceCulture);
			}
		}
	}
}
