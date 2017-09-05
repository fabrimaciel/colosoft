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
	/// Contrato para o fluxo de controle de validação e máscara
	/// </summary>
	public interface IValidationManager
	{
		/// <summary>
		/// Inicializa o gerenciador.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Inicializa de forma assincrona o gerenciador de validação.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult BeginInitialize(AsyncCallback callback, object state);

		/// <summary>
		/// Finaliza a inicialização assincrona do gerenciador.
		/// </summary>
		/// <param name="asyncResult"></param>
		void EndInitialize(IAsyncResult asyncResult);

		/// <summary>
		/// Valida um objeto
		/// </summary>
		/// <param name="targetObject">Objeto a ser validado</param>
		/// <param name="validationResult">Objeto aonde o resultado da validação será anexado</param>
		void Validate(IStateControl targetObject, ref ValidationResult validationResult);

		/// <summary>
		/// Valida uma propriedade de um objeto
		/// </summary>
		/// <param name="targetObject">Objeto alvo</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="propertyValue">Valor da propriedade</param>
		/// <param name="validationResult">Objeto aonde o resultado da validação será anexado</param>
		void ValidateProperty(IStateControl targetObject, string propertyName, object propertyValue, ref ValidationResult validationResult);

		/// <summary>
		/// Valida uma propriedade de um objeto
		/// </summary>
		/// <param name="targetObject">Objeto alvo</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="propertyValue">Valor da propriedade</param>
		/// <returns>Objeto aonde o resultado da validação será anexado</returns>
		ValidationResult ValidateProperty(IStateControl targetObject, string propertyName, object propertyValue);

		/// <summary>
		/// Reseta os dados da instancia.
		/// </summary>
		void Reset();

		/// <summary>
		/// Carrega as setagens de uma entidade.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade</param>
		/// <returns></returns>
		IEnumerable<IPropertySettingsInfo> LoadSettings(Colosoft.Reflection.TypeName entityTypeName);

		/// <summary>
		/// Carrega a customização de especialização da entidade, caso haja.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade</param>
		/// <returns></returns>
		IEntitySpecialization LoadSpecilization(Colosoft.Reflection.TypeName entityTypeName);

		/// <summary>
		/// Cria o estado da propriedade.
		/// </summary>
		/// <param name="owner">Proprietário do item que será criado.</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="information">Informações de estado da propriedade</param>
		/// <param name="uiContext">Nome do contexto de usuário</param>
		/// <param name="culture">Cultura que será usada na criação.</param>
		/// <returns></returns>
		IStatebleItem CreatePropertyState(IStateble owner, string propertyName, IPropertySettingsInfo information, string uiContext, System.Globalization.CultureInfo culture);

		/// <summary>
		/// Carrega as informações das propriedades de um determinado tipo.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade.</param>
		/// <returns></returns>
		IEnumerable<IEntityPropertyInfo> LoadTypeProperties(Colosoft.Reflection.TypeName entityTypeName);

		/// <summary>
		/// Carrega as informações das propriedades de um determinado tipo.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade.</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		IEntityPropertyInfo LoadTypeProperty(Colosoft.Reflection.TypeName entityTypeName, string propertyName);

		/// <summary>
		/// Carrega os labels de uma propriedade.
		/// </summary>
		/// <param name="typeName">Tipo</param>
		/// <param name="uiContext">Contexto com o usuário</param>
		/// <returns></returns>
		IEnumerable<PropertyLabelInfo> GetLabels(Colosoft.Reflection.TypeName typeName, string uiContext);

		/// <summary>
		/// Carrega os labels de uma propriedade.
		/// </summary>
		/// <param name="typeName">Tipo</param>
		/// <param name="uiContext">Contexto com o usuário</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		IEnumerable<PropertyLabelInfo> GetLabels(Colosoft.Reflection.TypeName typeName, string uiContext, string propertyName);

		/// <summary>
		/// Carrega os labels de uma propriedade.
		/// </summary>
		/// <param name="typeName">Tipo</param>
		/// <param name="uiContext">Contexto com o usuário</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		PropertyLabelInfo GetLabel(Colosoft.Reflection.TypeName typeName, string uiContext, string propertyName);

		/// <summary>
		/// Carrega as informações de validação de entrada para um grupo de regra de entrada específico.
		/// </summary>
		/// <param name="inputRulesGroupId"></param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		IInputValidationInfo GetInputValidateInfo(int inputRulesGroupId, string uiContext);
	}
}
