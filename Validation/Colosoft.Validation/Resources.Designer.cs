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

namespace Colosoft.Validation.Properties
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
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.Validation.Properties.Resources", typeof(Resources).Assembly);
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
		///   Looks up a localized string similar to Não foi possível carregar o código-fonte dos grupos de comando do sistema..
		/// </summary>
		internal static string Compiler_CommandGroupBuilder_FailOnGetCommandGroupsFromRepository
		{
			get
			{
				return ResourceManager.GetString("Compiler_CommandGroupBuilder_FailOnGetCommandGroupsFromRepository", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível recupera o BUILDER local..
		/// </summary>
		internal static string Compiler_CommandGroupBuilderManager_FailOnGetLocalBuilder
		{
			get
			{
				return ResourceManager.GetString("Compiler_CommandGroupBuilderManager_FailOnGetLocalBuilder", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível recupera o BUILDER remoto..
		/// </summary>
		internal static string Compiler_CommandGroupBuilderManager_FailOnGetRemoteBuilder
		{
			get
			{
				return ResourceManager.GetString("Compiler_CommandGroupBuilderManager_FailOnGetRemoteBuilder", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Grupo de comandos {0} não existe para o tipo {1}.
		/// </summary>
		internal static string Error_CommandGroup_NotExists
		{
			get
			{
				return ResourceManager.GetString("Error_CommandGroup_NotExists", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Validação de tipo exclisivo duplicada na lista.
		/// </summary>
		internal static string Error_ExclusiveValidatioTypeDuplicatedInList
		{
			get
			{
				return ResourceManager.GetString("Error_ExclusiveValidatioTypeDuplicatedInList", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Error({1}) on Load Assembly of type &apos;{0}&apos;..
		/// </summary>
		internal static string Error_OnLoadAssembly
		{
			get
			{
				return ResourceManager.GetString("Error_OnLoadAssembly", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocurred when create validator to validation &apos;{0}&apos;.
		///Details: &apos;{1}&apos;.
		/// </summary>
		internal static string GeneralValidationManager_CreateValidatorToValidationError
		{
			get
			{
				return ResourceManager.GetString("GeneralValidationManager_CreateValidatorToValidationError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocurred when load validation type &apos;{0}&apos;. 
		///Details: &apos;{1}&apos;.
		/// </summary>
		internal static string GeneralValidationManager_LoadValidationTypeError
		{
			get
			{
				return ResourceManager.GetString("GeneralValidationManager_LoadValidationTypeError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Type FullName not found for validation &apos;{0}&apos;..
		/// </summary>
		internal static string GeneralValidationManager_TypeFullNameNotFound
		{
			get
			{
				return ResourceManager.GetString("GeneralValidationManager_TypeFullNameNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to The assembly &apos;{0}&apos; from validation type &apos;{1}&apos; not found..
		/// </summary>
		internal static string GeneralValidationManager_ValidationTypeAssemblyNotFound
		{
			get
			{
				return ResourceManager.GetString("GeneralValidationManager_ValidationTypeAssemblyNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error ocurred when get type &apos;{0}&apos; from validation type &apos;{1}&apos;.
		///Details: &apos;{2}&apos;.
		/// </summary>
		internal static string GeneralValidationManager_ValidationTypeGetTypeError
		{
			get
			{
				return ResourceManager.GetString("GeneralValidationManager_ValidationTypeGetTypeError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível recupera o binário do grupo de comando &apos;{0}&apos;..
		/// </summary>
		internal static string ValidationConfiguration_FailOnGetCommandGroup
		{
			get
			{
				return ResourceManager.GetString("ValidationConfiguration_FailOnGetCommandGroup", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Propriedade não foi encontrada.
		/// </summary>
		internal static string ValidationManager_PropertyNotFound
		{
			get
			{
				return ResourceManager.GetString("ValidationManager_PropertyNotFound", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível converter o valor &apos;{0}&apos; do parametro &apos;{1}&apos;..
		/// </summary>
		internal static string ValidatorCreator_FailOnConvertParameterValue
		{
			get
			{
				return ResourceManager.GetString("ValidatorCreator_FailOnConvertParameterValue", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Não foi possível criar o validador &apos;{0}&apos;..
		/// </summary>
		internal static string ValidatorCreator_FailOnCreateValidator
		{
			get
			{
				return ResourceManager.GetString("ValidatorCreator_FailOnCreateValidator", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O campo &apos;{0}&apos; : &apos;{1}&apos; possui dígitos inválidos..
		/// </summary>
		internal static string Validators_CheckDigitsValidator_MessageTemplate
		{
			get
			{
				return ResourceManager.GetString("Validators_CheckDigitsValidator_MessageTemplate", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O campo &apos;{0}&apos; : &apos;{1}&apos; está com o tamanho inválido..
		/// </summary>
		internal static string Validators_LengthValidator_MessageTemplate
		{
			get
			{
				return ResourceManager.GetString("Validators_LengthValidator_MessageTemplate", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O campo &apos;{0}&apos; : &apos;{1}&apos; é necessária..
		/// </summary>
		internal static string Validators_NecessaryValidator_MessageTemplate
		{
			get
			{
				return ResourceManager.GetString("Validators_NecessaryValidator_MessageTemplate", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O campo &apos;{0}&apos; : &apos;{1}&apos; não pode ficar vazio..
		/// </summary>
		internal static string Validators_NotNullValidator_MessageTemplate
		{
			get
			{
				return ResourceManager.GetString("Validators_NotNullValidator_MessageTemplate", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O campo &apos;{0}&apos; : &apos;{1}&apos; está com  valor fora da faixa permitida..
		/// </summary>
		internal static string Validators_RangeValidator_MessageTemplate
		{
			get
			{
				return ResourceManager.GetString("Validators_RangeValidator_MessageTemplate", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O campo &apos;{0}&apos; : &apos;{1}&apos; estão com o tamanho inválido..
		/// </summary>
		internal static string Validators_StringLengthValidation_MessageTemplate
		{
			get
			{
				return ResourceManager.GetString("Validators_StringLengthValidation_MessageTemplate", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to O campo &apos;{0}&apos; : &apos;{1}&apos; está com caracteres inválidos..
		/// </summary>
		internal static string Validators_ValidCharsValidator_MessageTemplate
		{
			get
			{
				return ResourceManager.GetString("Validators_ValidCharsValidator_MessageTemplate", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to sem nome.
		/// </summary>
		internal static string ValidatorsHelper_Unamed
		{
			get
			{
				return ResourceManager.GetString("ValidatorsHelper_Unamed", resourceCulture);
			}
		}
	}
}
