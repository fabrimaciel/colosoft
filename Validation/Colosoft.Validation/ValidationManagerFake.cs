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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Validation
{
	/// <summary>
	/// Implementação fake do ValidationManager.
	/// </summary>
	public class ValidationManagerFake : Colosoft.Validation.IValidationManager
	{
		/// <summary>
		/// Inicializa o gerenciador.
		/// </summary>
		public void Initialize()
		{
		}

		/// <summary>
		/// Inicializa o gerenciador de forma assincrona.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IAsyncResult BeginInitialize(AsyncCallback callback, object state)
		{
			var asyncResult = new Colosoft.Threading.AsyncResult(callback, state);
			if(callback != null)
				callback(asyncResult);
			return asyncResult;
		}

		/// <summary>
		/// Finaliza a inicialização assincrona.
		/// </summary>
		/// <param name="result"></param>
		public void EndInitialize(IAsyncResult result)
		{
		}

		/// <summary>
		/// Recupera o labels associados com o tipo informado.
		/// </summary>
		/// <param name="typeName">Tipo a ser analisado</param>
		/// <param name="uiContext">Contexto da interface com o usuário</param>
		/// <returns>Enumerador das labels.</returns>
		public IEnumerable<PropertyLabelInfo> GetLabels(Colosoft.Reflection.TypeName typeName, string uiContext)
		{
			yield break;
		}

		/// <summary>
		/// Recupera o labels associados com o tipo informado.
		/// </summary>
		/// <typeparam name="T">Tipo a ser analisado</typeparam>
		/// <param name="uiContext">Contexto da interface com o usuário</param>
		/// <returns>Enumerador das labels.</returns>
		public IEnumerable<PropertyLabelInfo> GetLabels<T>(string uiContext)
		{
			yield break;
		}

		/// <summary>
		/// Recupera o label para um determinada propriedade.
		/// </summary>
		/// <param name="typeName">Tipo a ser analisado</param>
		/// <param name="uiContext"></param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		public PropertyLabelInfo GetLabel(Colosoft.Reflection.TypeName typeName, string uiContext, string propertyName)
		{
			return null;
		}

		/// <summary>
		/// Recupera o label para um determinada propriedade.
		/// </summary>
		/// <typeparam name="T">Tipo a ser analisado</typeparam>
		/// <param name="uiContext"></param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		public PropertyLabelInfo GetLabel<T>(string uiContext, string propertyName)
		{
			return null;
		}

		/// <summary>
		/// Retorna um enumerador com todas as propriedades e suas respectivas máscaras de um tipo
		/// </summary>
		/// <param name="typeName">Tipo a ser analisado</param>
		/// <param name="uiContext">Contexto da interface com o usuário</param>
		/// <returns>Enumerado de propriedades e máscaras</returns>
		public IEnumerable<PropertyMaskInfo> GetMasks(Colosoft.Reflection.TypeName typeName, string uiContext)
		{
			yield break;
		}

		/// <summary>
		/// Retorna um enumerador com todas as propriedades e suas respectivas máscaras de um tipo
		/// </summary>
		/// <typeparam name="T">Tipo a ser analisado</typeparam>
		/// <param name="uiContext">Contexto da interface com o usuário</param>
		/// <returns>Enumerado de propriedades e máscaras</returns>
		public IEnumerable<PropertyMaskInfo> GetMasks<T>(string uiContext) where T : IStateControl
		{
			yield break;
		}

		/// <summary>
		/// Carrega as máscaras de uma propriedade de um tipo
		/// </summary>
		/// <typeparam name="T">Tipo a ser analisado</typeparam>
		/// <param name="uiContext">Contexto da interface com o usuário</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns>Lista de máscaras</returns>
		public PropertyMaskInfo GetMasks<T>(string uiContext, string propertyName) where T : IStateControl
		{
			return null;
		}

		/// <summary>
		/// Busca as validações associadas a um determinado tipo
		/// </summary>
		/// <typeparam name="T">Tipo</typeparam>
		/// <returns>Enumerador com as propriedades e suas validações</returns>
		public IEnumerable<PropertyValidationInfo> GetValidations<T>()
		{
			yield break;
		}

		/// <summary>
		/// Busca as validações associadas a uma propriedade de um determinado tipo
		/// </summary>
		/// <typeparam name="T">Tipo</typeparam>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns>Validações da propriedade</returns>
		public PropertyValidationInfo GetValidation<T>(string propertyName)
		{
			return null;
		}

		/// <summary>
		/// Busca as validações associadas a um determinado tipo
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns>Enumerador com as propriedades e suas validações</returns>
		public IEnumerable<PropertyValidationInfo> GetValidations(Colosoft.Reflection.TypeName typeName)
		{
			yield break;
		}

		/// <summary>
		/// Busca as validações associadas a uma propriedade de um determinado tipo
		/// </summary>
		/// <param name="typeName">Tipo</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns>Validações da propriedade</returns>
		public PropertyValidationInfo GetValidation(Colosoft.Reflection.TypeName typeName, string propertyName)
		{
			return null;
		}

		/// <summary>
		/// Valida um objeto
		/// </summary>
		/// <param name="targetObject">Objeto a ser validado</param>
		/// <returns>Resultado da validação</returns>
		public ValidationResult Validate(IStateControl targetObject)
		{
			return new ValidationResult {
				IsValid = true
			};
		}

		/// <summary>
		/// Valida um objeto
		/// </summary>
		/// <param name="targetObject">Objeto a ser validado</param>
		/// <param name="validationResult">Objeto aonde o resultado da validação será anexado</param>
		public void Validate(IStateControl targetObject, ref ValidationResult validationResult)
		{
		}

		/// <summary>
		/// Valida uma propriedade de um objeto
		/// </summary>
		/// <param name="targetObject">Objeto alvo</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="propertyValue">Valor da propriedade</param>
		/// <param name="validationResult">Objeto aonde o resultado da validação será anexado</param>
		public void ValidateProperty(IStateControl targetObject, string propertyName, object propertyValue, ref ValidationResult validationResult)
		{
		}

		/// <summary>
		/// Valida uma propriedade de um objeto
		/// </summary>
		/// <param name="targetObject">Objeto alvo</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="propertyValue">Valor da propriedade</param>
		/// <returns>Objeto aonde o resultado da validação será anexado</returns>
		public ValidationResult ValidateProperty(IStateControl targetObject, string propertyName, object propertyValue)
		{
			return new ValidationResult {
				IsValid = true
			};
		}

		/// <summary>
		/// Registra os eventos de um objeto
		/// </summary>
		/// <param name="typeName"></param>
		public void RegisterEvents(Colosoft.Reflection.TypeName typeName)
		{
		}

		/// <summary>
		/// Reseta os dados da instancia.
		/// </summary>
		public void Reset()
		{
		}

		/// <summary>
		/// Carrega as setagens de uma entidade.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade</param>
		/// <returns></returns>
		public IEnumerable<IPropertySettingsInfo> LoadSettings(Colosoft.Reflection.TypeName entityTypeName)
		{
			yield break;
		}

		/// <summary>
		/// Carrega a customização de especialização da entidade, caso haja.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade</param>
		/// <returns></returns>
		public IEntitySpecialization LoadSpecilization(Colosoft.Reflection.TypeName entityTypeName)
		{
			return null;
		}

		/// <summary>
		/// Carrega o estado da propriedade.
		/// </summary>
		/// <param name="owner">Proprietário do item.</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="information">Informações de estado da propriedade</param>
		/// <param name="uiContext">Identificador do contexto de usuário</param>
		/// <param name="culture">Cultura que será usada na criação.</param>
		/// <returns></returns>
		public IStatebleItem CreatePropertyState(IStateble owner, string propertyName, IPropertySettingsInfo information, string uiContext, System.Globalization.CultureInfo culture)
		{
			return null;
		}

		/// <summary>
		/// Carrega as informações das propriedades de um determinado tipo.
		/// </summary>
		/// <param name="entityType">Tipo da entidade.</param>
		/// <returns></returns>
		public IEnumerable<IEntityPropertyInfo> LoadTypeProperties(Colosoft.Reflection.TypeName entityType)
		{
			return null;
		}

		/// <summary>
		/// Carrega as informações das propriedades de um determinado tipo.
		/// </summary>
		/// <param name="entityType">Tipo da entidade.</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		public IEntityPropertyInfo LoadTypeProperty(Colosoft.Reflection.TypeName entityType, string propertyName)
		{
			return null;
		}

		/// <summary>
		/// Carrega os labels de uma propriedade.
		/// </summary>
		/// <param name="typeName">Tipo</param>
		/// <param name="uiContext">Contexto com o usuário</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		public IEnumerable<PropertyLabelInfo> GetLabels(Colosoft.Reflection.TypeName typeName, string uiContext, string propertyName)
		{
			yield break;
		}

		/// <summary>
		/// Carrega as informações de validação de entrada para um grupo de regra de entrada específico.
		/// </summary>
		/// <param name="inputRulesGroupId"></param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		public IInputValidationInfo GetInputValidateInfo(int inputRulesGroupId, string uiContext)
		{
			return null;
		}
	}
}
