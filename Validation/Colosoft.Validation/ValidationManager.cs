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
using System.Reflection;

namespace Colosoft.Validation
{
	/// <summary>
	/// Implementação base do gerenciador de validação.
	/// </summary>
	public class ValidationManager : IValidationManager, IDisposable
	{
		private readonly static IValidator _necessaryValidator = new Validators.NecessaryValidator();

		/// <summary>
		/// Instancia o gerenciador.
		/// </summary>
		public static IValidationManager Instance
		{
			get
			{
				if(_instance == null)
				{
					try
					{
						_instance = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IValidationManager>();
					}
					catch(Exception ex)
					{
						System.Diagnostics.Trace.WriteLine(ex);
					}
				}
				return _instance;
			}
			set
			{
				_instance = value;
			}
		}

		/// <summary>
		/// Instancia do validador de valor necessário.
		/// </summary>
		public static IValidator NecessaryValidator
		{
			get
			{
				return _necessaryValidator;
			}
		}

		private Logging.ILogger _logger;

		private static IValidationManager _instance;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="logger">Instancia responsável pelo log.</param>
		public ValidationManager(Logging.ILogger logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Aplica a validação.
		/// </summary>
		/// <param name="validation">validação</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="propertyLabel">Label da propriedade.</param>
		/// <param name="targetObject">objeto alvo</param>
		/// <param name="propertyValue">valor da propriedade alvo</param>
		/// <param name="validationResult">resultado</param>
		private static void ApplyValidation(IValidator validation, string propertyName, IPropertyLabel propertyLabel, IStateControl targetObject, object propertyValue, ref ValidationResult validationResult)
		{
			if(validation == null)
				return;
			if(validationResult == null)
				validationResult = new ValidationResult();
			validation.DoValidate(targetObject, propertyName, propertyLabel, propertyValue, validationResult, null);
		}

		/// <summary>
		/// Inicializa o gerenciador.
		/// </summary>
		public virtual void Initialize()
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
			if(!System.Threading.ThreadPool.QueueUserWorkItem(e =>  {
				var asyncResult2 = (Colosoft.Threading.AsyncResult)e;
				try
				{
					Initialize();
				}
				catch(Exception ex)
				{
					asyncResult2.Completed(ex, false);
					return;
				}
				asyncResult2.Completed(null, false);
			}, asyncResult))
				callback(asyncResult);
			return asyncResult;
		}

		/// <summary>
		/// Finaliza a inicialização assincrona.
		/// </summary>
		/// <param name="result"></param>
		public void EndInitialize(IAsyncResult result)
		{
			var asyncResult2 = (Colosoft.Threading.AsyncResult)result;
			if(asyncResult2.Exception != null)
				throw asyncResult2.Exception;
		}

		/// <summary>
		/// Valida um objeto
		/// </summary>
		/// <param name="targetObject">Objeto a ser validado</param>
		/// <returns>Resultado da validação</returns>
		public virtual ValidationResult Validate(IStateControl targetObject)
		{
			var result = new ValidationResult();
			this.Validate(targetObject, ref result);
			return result;
		}

		/// <summary>
		/// Valida um objeto
		/// </summary>
		/// <param name="targetObject">Objeto a ser validado</param>
		/// <param name="validationResult">Objeto aonde o resultado da validação será anexado</param>
		public virtual void Validate(IStateControl targetObject, ref ValidationResult validationResult)
		{
			targetObject.Require("targetObject").NotNull();
			if(targetObject is IDisposableState && ((IDisposableState)targetObject).IsDisposed)
				return;
			if(validationResult == null)
				validationResult = new ValidationResult();
			var properties = targetObject.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(f => f.CanRead && !f.GetCustomAttributes(false).Any(x => x is IgnoreValidationAttribute));
			foreach (PropertyInfo property in properties)
			{
				if(property.Name != "IsLocked")
				{
					var state = targetObject.InstanceState[property.Name];
					if(state != null && state.Validation != null)
					{
						var propertyValue = property.GetValue(targetObject, null);
						ApplyValidation(state.Validation, property.Name, state.Label, targetObject, propertyValue, ref validationResult);
					}
				}
			}
		}

		/// <summary>
		/// Valida uma propriedade de um objeto
		/// </summary>
		/// <param name="propertyValue">Valor da propriedade</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="targetObject">Objeto alvo</param>
		/// <param name="validationResult">Objeto aonde o resultado da validação será anexado</param>
		public virtual void ValidateProperty(IStateControl targetObject, string propertyName, object propertyValue, ref ValidationResult validationResult)
		{
			targetObject.Require("targetObject").NotNull();
			propertyName.Require("propertyName").NotNull().NotEmpty();
			var statebleItem = targetObject.InstanceState[propertyName];
			if(statebleItem != null && statebleItem.Validation != null)
				ApplyValidation(statebleItem.Validation, propertyName, statebleItem.Label, targetObject, propertyValue, ref validationResult);
		}

		/// <summary>
		/// Valida uma propriedade de um objeto
		/// </summary>
		/// <param name="propertyValue">Valor da propriedade</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="targetObject">Objeto alvo</param>
		/// <returns>Objeto aonde o resultado da validação será anexado</returns>
		public virtual ValidationResult ValidateProperty(IStateControl targetObject, string propertyName, object propertyValue)
		{
			var result = new ValidationResult();
			this.ValidateProperty(targetObject, propertyName, propertyValue, ref result);
			return result;
		}

		/// <summary>
		/// Reseta os dados da instancia.
		/// </summary>
		public virtual void Reset()
		{
		}

		/// <summary>
		/// Carrega as setagens de uma entidade.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade</param>
		/// <returns></returns>
		public virtual IEnumerable<IPropertySettingsInfo> LoadSettings(Colosoft.Reflection.TypeName entityTypeName)
		{
			yield break;
		}

		/// <summary>
		/// Carrega a customização de especialização da entidade, caso haja.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade</param>
		/// <returns></returns>
		public virtual IEntitySpecialization LoadSpecilization(Colosoft.Reflection.TypeName entityTypeName)
		{
			return null;
		}

		/// <summary>
		/// Carrega o estado da propriedade.
		/// </summary>
		/// <param name="owner">Proprietário do item que será criado.</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="information">Informações de estado da propriedade</param>
		/// <param name="uiContext">Identificador do contexto de usuário</param>
		/// <param name="culture">Cultura que será usada na criação.</param>
		/// <returns></returns>
		public virtual IStatebleItem CreatePropertyState(IStateble owner, string propertyName, IPropertySettingsInfo information, string uiContext, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Carrega as informações das propriedades de um determinado tipo.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade.</param>
		/// <returns></returns>
		public virtual IEnumerable<IEntityPropertyInfo> LoadTypeProperties(Colosoft.Reflection.TypeName entityTypeName)
		{
			yield break;
		}

		/// <summary>
		/// Carrega as informações das propriedades de um determinado tipo.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade.</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		public virtual IEntityPropertyInfo LoadTypeProperty(Colosoft.Reflection.TypeName entityTypeName, string propertyName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Carrega os labels de uma propriedade.
		/// </summary>
		/// <param name="typeName">Tipo</param>
		/// <param name="uiContext">Contexto com o usuário</param>
		/// <returns></returns>
		public virtual IEnumerable<PropertyLabelInfo> GetLabels(Colosoft.Reflection.TypeName typeName, string uiContext)
		{
			yield break;
		}

		/// <summary>
		/// Carrega os labels de uma propriedade.
		/// </summary>
		/// <param name="typeName">Tipo</param>
		/// <param name="uiContext">Contexto com o usuário</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		public virtual IEnumerable<PropertyLabelInfo> GetLabels(Colosoft.Reflection.TypeName typeName, string uiContext, string propertyName)
		{
			yield break;
		}

		/// <summary>
		/// Carrega os labels de uma propriedade.
		/// </summary>
		/// <param name="typeName">Tipo</param>
		/// <param name="uiContext">Contexto com o usuário</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		public virtual PropertyLabelInfo GetLabel(Colosoft.Reflection.TypeName typeName, string uiContext, string propertyName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Carrega as informações de validação de entrada para um grupo de regra de entrada específico.
		/// </summary>
		/// <param name="inputRulesGroupId"></param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		public virtual IInputValidationInfo GetInputValidateInfo(int inputRulesGroupId, string uiContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
