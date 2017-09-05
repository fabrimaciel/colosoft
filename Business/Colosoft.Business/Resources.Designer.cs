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

namespace Colosoft.Business.Properties
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
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Business.Properties.Resources", typeof(Resources).Assembly);
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
		///   Looks up a localized string similar to O registro da entidade &apos;{0}&apos; com a chave &apos;{1}&apos; já existem na lista pertencente a entidade &apos;{2}&apos;..
		/// </summary>
		internal static string BaseEntityList_EntityInstanceExists
		{
			get
			{
				return ResourceManager.GetString("BaseEntityList_EntityInstanceExists", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Sem pai.
		/// </summary>
		internal static string BaseEntityList_WithoutOwner
		{
			get
			{
				return ResourceManager.GetString("BaseEntityList_WithoutOwner", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Propriedade &apos;{0}&apos; não foi encontrada.
		/// </summary>
		internal static string Entity_PropertyNotFound
		{
			get
			{
				return ResourceManager.GetString("Entity_PropertyNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Ocorreu um erro ao recuperar as informações de eventos da entidade &apos;{0}&apos;.
		/// </summary>
		internal static string EntityEventManager_GetEntityEventInfosError
		{
			get
			{
				return ResourceManager.GetString("EntityEventManager_GetEntityEventInfosError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Ocorreu um erro a recuperar a implementação para o tipo de evento &apos;{0}&apos; para a entidade &apos;{1}&apos; com o export id &apos;{2}&apos;.
		/// </summary>
		internal static string EntityEventManager_GetEventTypeImplementationError
		{
			get
			{
				return ResourceManager.GetString("EntityEventManager_GetEventTypeImplementationError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Expected IEntityOfModel..
		/// </summary>
		internal static string EntityLinkList_ExpectedEntityOfModel
		{
			get
			{
				return ResourceManager.GetString("EntityLinkList_ExpectedEntityOfModel", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível criar a entidade do link &apos;{0}&apos; com a entidade &apos;{1}&apos; &apos;{2}&apos;..
		/// </summary>
		internal static string EntityLinksList_CouldNotCreateTheEntityLink
		{
			get
			{
				return ResourceManager.GetString("EntityLinksList_CouldNotCreateTheEntityLink", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Existe uma entrada de filho duplicada para o nome &apos;{0}&apos;..
		/// </summary>
		internal static string EntityLoaderChildContainer_DuplicateChildEntryWithName
		{
			get
			{
				return ResourceManager.GetString("EntityLoaderChildContainer_DuplicateChildEntryWithName", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to (EntityType {0}) A propriedade uid do filho &apos;{1}&apos; associada com o link &apos;{2}&apos; não foi definida..
		/// </summary>
		internal static string EntityLoaderLinkInfo_ChildUidPropertyUndefined
		{
			get
			{
				return ResourceManager.GetString("EntityLoaderLinkInfo_ChildUidPropertyUndefined", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to (EntityType {0}) A propriedade de chave estrangeira do filho &apos;{1}&apos; associada com o link &apos;{2}&apos; não foi definida..
		/// </summary>
		internal static string EntityLoaderLinkInfo_ForeignKeyPropertyUndefined
		{
			get
			{
				return ResourceManager.GetString("EntityLoaderLinkInfo_ForeignKeyPropertyUndefined", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O filho &apos;{0}&apos; da entidade &apos;{1}&apos; associado com o link &apos;{2}&apos; não foi encontrado.
		/// </summary>
		internal static string EntityLoaderOfModel_ChildOfLinkNotFound
		{
			get
			{
				return ResourceManager.GetString("EntityLoaderOfModel_ChildOfLinkNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Creator undefined on load of type &apos;{0}&apos;..
		/// </summary>
		internal static string EntityLoaderOfModel_CreatorUndefined
		{
			get
			{
				return ResourceManager.GetString("EntityLoaderOfModel_CreatorUndefined", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível carregar os dados dos filhos de &apos;{0}&apos;. {1}.
		/// </summary>
		internal static string EntityLoaderOfModel_FaillOnLoadChildrenFromEntity
		{
			get
			{
				return ResourceManager.GetString("EntityLoaderOfModel_FaillOnLoadChildrenFromEntity", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível carregar os dados do filho &apos;{0}&apos; da entidade &apos;{1}&apos;, pelo motivo: {2}.
		/// </summary>
		internal static string EntityLoaderOfModel_LoadChildFromEntityError
		{
			get
			{
				return ResourceManager.GetString("EntityLoaderOfModel_LoadChildFromEntityError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível carregar os dados da ligação &apos;{0}&apos; da entidade &apos;{1}&apos;, pelo motivo: {2}.
		/// </summary>
		internal static string EntityLoaderOfModel_LoadLinkFromEntityError
		{
			get
			{
				return ResourceManager.GetString("EntityLoaderOfModel_LoadLinkFromEntityError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível carregar os dados da referência &apos;{0}&apos; da entidade &apos;{1}&apos;, pelo motivo: {2}.
		/// </summary>
		internal static string EntityLoaderOfModel_LoadReferenceFromEntityError
		{
			get
			{
				return ResourceManager.GetString("EntityLoaderOfModel_LoadReferenceFromEntityError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível carregar a referência, a propriedade &apos;{0}&apos; não foi encontrada no tipo &apos;{1}&apos;..
		/// </summary>
		internal static string EntityLoaderReference_ParentPropertyNotFound
		{
			get
			{
				return ResourceManager.GetString("EntityLoaderReference_ParentPropertyNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Existe uma entrada de referência duplicada para o nome &apos;{0}&apos;..
		/// </summary>
		internal static string EntityLoaderReferenceContainer_DuplicateReferebceEntryWithName
		{
			get
			{
				return ResourceManager.GetString("EntityLoaderReferenceContainer_DuplicateReferebceEntryWithName", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Customização não identificada indica que o valor de &apos;{0}&apos; é inválido..
		/// </summary>
		internal static string Exception_InvalidCustomizationValue
		{
			get
			{
				return ResourceManager.GetString("Exception_InvalidCustomizationValue", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to A lista da instância não é a {0}..
		/// </summary>
		internal static string Exception_InvalidList
		{
			get
			{
				return ResourceManager.GetString("Exception_InvalidList", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O Item deve ser único na lista..
		/// </summary>
		internal static string Exception_ItemMustBuUniqueInList
		{
			get
			{
				return ResourceManager.GetString("Exception_ItemMustBuUniqueInList", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Chave {0} não encontrada..
		/// </summary>
		internal static string Exception_KeyNotFound
		{
			get
			{
				return ResourceManager.GetString("Exception_KeyNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O campo &apos;{0}&apos; não é válido..
		/// </summary>
		internal static string Exception_ValueDidNotCheckDigits
		{
			get
			{
				return ResourceManager.GetString("Exception_ValueDidNotCheckDigits", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O valor do campo &apos;{0}&apos; possui um caractere inválido..
		/// </summary>
		internal static string Exception_ValueHasInvalidChar
		{
			get
			{
				return ResourceManager.GetString("Exception_ValueHasInvalidChar", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O valor do campo &apos;{0}&apos; é inválido..
		/// </summary>
		internal static string Exception_ValueInvalidByCustomization
		{
			get
			{
				return ResourceManager.GetString("Exception_ValueInvalidByCustomization", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O valor campo &apos;{0}&apos; não tem um comprimento válido..
		/// </summary>
		internal static string Exception_ValueInvalidLength
		{
			get
			{
				return ResourceManager.GetString("Exception_ValueInvalidLength", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O campo &apos;{0}&apos; é requerido ou necessário..
		/// </summary>
		internal static string Exception_ValueIsRquiredOrNecessaryToProperty
		{
			get
			{
				return ResourceManager.GetString("Exception_ValueIsRquiredOrNecessaryToProperty", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O valor do campo &apos;{0}&apos; não está no intervalo permitido.
		/// </summary>
		internal static string Exception_ValueNotInRange
		{
			get
			{
				return ResourceManager.GetString("Exception_ValueNotInRange", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O valor do campo&apos;{0}&apos; não está no formato correto ou está parcialmente preenchido..
		/// </summary>
		internal static string Exception_ValueNotProperlyFormatted
		{
			get
			{
				return ResourceManager.GetString("Exception_ValueNotProperlyFormatted", resourceCulture);
			}
		}
	}
}
