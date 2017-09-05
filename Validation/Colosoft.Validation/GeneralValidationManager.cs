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

namespace Colosoft.Validation.Configuration
{
	/// <summary>
	/// Argumentos para a criação de um StatebleItem.
	/// </summary>
	public class StatebleItemCreatorEventArgs : EventArgs
	{
		/// <summary>
		/// Identificador.
		/// </summary>
		public string Identifier
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da propriedade.
		/// </summary>
		public string PropertyName
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se copia o valor.
		/// </summary>
		public bool CopyValue
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se recarrega as configurações.
		/// </summary>
		public bool ReloadSettings
		{
			get;
			set;
		}

		/// <summary>
		/// Opções.
		/// </summary>
		public InputRulesOptions Options
		{
			get;
			set;
		}

		/// <summary>
		/// Rótulo da propriedade.
		/// </summary>
		public IPropertyLabel Label
		{
			get;
			set;
		}

		/// <summary>
		/// Parse.
		/// </summary>
		public IParse Parse
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Assinatura do método usado para criar um item com estado.
	/// </summary>
	/// <param name="owner"></param>
	/// <param name="e"></param>
	/// <returns></returns>
	public delegate IStatebleItem StatebleItemCreatorHandler (IStateble owner, StatebleItemCreatorEventArgs e);
	/// <summary>
	/// Classe usada para gerenciar as validações do sistema.
	/// </summary>
	public abstract class GeneralValidationManager : ValidationManager
	{
		private Logging.ILogger _logger;

		/// <summary>
		/// Armazena a relação dos criadores da validações
		/// </summary>
		private Dictionary<int, IValidatorCreator> _validatorCreators = new Dictionary<int, IValidatorCreator>();

		/// <summary>
		/// Armazena a relação da validação com o validador
		/// </summary>
		private Dictionary<int, IValidator> _validationValidator = new Dictionary<int, IValidator>();

		/// <summary>
		/// Configurações do gerenciador.
		/// </summary>
		abstract protected ValidationSettings Settings
		{
			get;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="logger">Instancia responsável pelo log.</param>
		public GeneralValidationManager(Logging.ILogger logger) : base(logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Cria uma item com estado.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		protected abstract IStatebleItem CreateStatebleItem(IStateble owner, StatebleItemCreatorEventArgs e);

		/// <summary>
		/// Carrega um conversor.
		/// </summary>
		/// <param name="parseId">Identificador do conversor.</param>
		/// <returns></returns>
		private IParse CreateParse(int parseId)
		{
			return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IParse>(parseId.ToString());
		}

		/// <summary>
		/// Recupera o grupo de regras de entrada.
		/// </summary>
		/// <param name="inputRulesGroupId">Identificador do grupo de regras de entrada.</param>
		/// <returns></returns>
		public InputRulesGroup GetInputRulesGroup(int inputRulesGroupId)
		{
			return Settings.InputRulesGroups.FirstOrDefault(f => f.InputRulesGroupId == inputRulesGroupId);
		}

		/// <summary>
		/// Carrega uma validação.
		/// </summary>
		/// <param name="validationId">Identificador da validação</param>
		/// <param name="culture"></param>
		/// <returns></returns>
		private IValidator GetValidator(int validationId, System.Globalization.CultureInfo culture)
		{
			IValidator validator = null;
			bool found = false;
			lock (_validationValidator)
				found = _validationValidator.TryGetValue(validationId, out validator);
			if(!found)
			{
				var validation = Settings.GetValidation(validationId);
				if(validation != null)
				{
					var validationType = Settings.GetValidationType(validation.ValidationTypeUid);
					if(validationType != null && validationType.Type != null)
					{
						IValidatorCreator creator = GetValidatorCreator(validationType, culture);
						if(creator != null)
						{
							var parameters = new List<ParameterValue>();
							foreach (var currentParameter in validationType.Parameters)
							{
								var validationParameter = validation.Parameters.Where(f => f.Name == currentParameter.Name).FirstOrDefault();
								parameters.Add(new ParameterValue(currentParameter.Name, (validationParameter == null) ? currentParameter.DefaultValue : validationParameter.Value));
							}
							try
							{
								validator = creator.CreateValidator(parameters, culture);
							}
							catch(Exception ex)
							{
								_logger.Error(ResourceMessageFormatter.Create(() => Properties.Resources.GeneralValidationManager_CreateValidatorToValidationError, validation.Name, Diagnostics.ExceptionFormatter.FormatException(ex, true)), ex);
							}
						}
					}
				}
				lock (_validationValidator)
					if(_validationValidator.ContainsKey(validationId))
						_validationValidator.Add(validationId, validator);
			}
			return validator;
		}

		/// <summary>
		/// Recupera o criador de validadores.
		/// </summary>
		/// <param name="validationType">Tipo de validação.</param>
		/// <param name="culture"></param>
		/// <returns></returns>
		private IValidatorCreator GetValidatorCreator(ValidationType validationType, System.Globalization.CultureInfo culture)
		{
			IValidatorCreator creator = null;
			bool found = false;
			lock (_validatorCreators)
				found = _validatorCreators.TryGetValue(validationType.ValidationTypeId, out creator);
			if(!found)
			{
				System.Reflection.Assembly assembly = null;
				if(Reflection.AssemblyLoader.Instance.TryGet(validationType.Type.AssemblyName.Name, out assembly))
				{
					Type validatorType = null;
					try
					{
						validatorType = assembly.GetType(validationType.Type.FullName, true, false);
					}
					catch(Exception ex)
					{
						_logger.Error(ResourceMessageFormatter.Create(() => Properties.Resources.GeneralValidationManager_ValidationTypeGetTypeError, validationType.Type.FullName, validationType.Name, Diagnostics.ExceptionFormatter.FormatException(ex, true)), ex);
					}
					if(validationType != null)
					{
						creator = ValidatorCreator.CreateCreator(validatorType, culture);
						lock (_validatorCreators)
							if(!_validatorCreators.ContainsKey(validationType.ValidationTypeId))
								_validatorCreators.Add(validationType.ValidationTypeId, creator);
					}
				}
				else
				{
					_logger.Error(ResourceMessageFormatter.Create(() => Properties.Resources.GeneralValidationManager_ValidationTypeAssemblyNotFound, validationType.Type.AssemblyName.Name, validationType.Name));
				}
			}
			return creator;
		}

		/// <summary>
		/// Insere as validações de entrada na propriedade.
		/// </summary>
		/// <param name="propertryState">Estado atual da propriedade</param>
		/// <param name="aggregateValidator">Agregador</param>
		/// <param name="inputValidateUid">Identificador da validação que será incluída.</param>
		private void InsertValidate(IStatebleItem propertryState, Validators.AggregateValidator aggregateValidator, Guid inputValidateUid)
		{
			var inputValidate = Settings.GetInputValidate(inputValidateUid);
			if(inputValidate != null)
				CalculateInputValidateInfo(propertryState, inputValidate, aggregateValidator, new List<Guid>());
		}

		/// <summary>
		/// Inicializa o gerenciador.
		/// </summary>
		public override void Initialize()
		{
		}

		/// <summary>
		/// Reseta os dados do gerenciador.
		/// </summary>
		public override void Reset()
		{
			base.Reset();
		}

		/// <summary>
		/// Carrega as setagens de uma entidade.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade</param>
		/// <returns></returns>
		public override IEnumerable<IPropertySettingsInfo> LoadSettings(Reflection.TypeName entityTypeName)
		{
			return Settings.GetTypeSettings(entityTypeName);
		}

		/// <summary>
		/// Carrega a customização de especialização da entidade, caso haja.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade</param>
		/// <returns></returns>
		public override IEntitySpecialization LoadSpecilization(Reflection.TypeName entityTypeName)
		{
			var specializationId = Settings.GetEntitySpecializationId(entityTypeName);
			if(specializationId.HasValue)
				return Extensions.ExtensionServiceLocator.Current.GetInstance<IEntitySpecialization>(specializationId.Value);
			return null;
		}

		/// <summary>
		/// Carrega o estado da propriedade.
		/// </summary>
		/// <param name="owner">Proprietário do item que será criado.</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <param name="information">Informações de estado da propriedade</param>
		/// <param name="uiContext">Nome do contexto de usuário</param>
		/// <param name="culture">Cultura que será usada na criação.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public override IStatebleItem CreatePropertyState(IStateble owner, string propertyName, IPropertySettingsInfo information, string uiContext, System.Globalization.CultureInfo culture)
		{
			var args = new StatebleItemCreatorEventArgs();
			args.Identifier = information.Identifier;
			args.PropertyName = propertyName;
			args.CopyValue = information.CopyValue;
			args.ReloadSettings = information.ReloadSettings;
			var labelInfo = GetLabel(owner.Type, uiContext, propertyName);
			if(labelInfo != null)
				args.Label = labelInfo.Label;
			else
				args.Label = new PropertyLabel(propertyName.GetFormatter(), null);
			var result = new Lazy<IStatebleItem>(() => CreateStatebleItem(owner, args));
			var aggregateValidator = new Validators.AggregateValidator();
			if(information.InputRulesGroupId.HasValue)
			{
				var rules = Settings.GetRules(information.InputRulesGroupId.Value, uiContext);
				if(rules != null)
				{
					args.Options = rules.Options;
					if((args.Options & InputRulesOptions.Necessary) == InputRulesOptions.Necessary)
						aggregateValidator.Add(ValidationManager.NecessaryValidator);
					if(!string.IsNullOrEmpty(rules.Label))
						args.Label = new PropertyLabel(rules.Label.GetFormatter(), rules.Label.GetFormatter());
					if(rules.ParseId.HasValue)
						args.Parse = CreateParse(rules.ParseId.Value);
					if(rules.InputValidateUid.HasValue)
						InsertValidate(result.Value, aggregateValidator, rules.InputValidateUid.Value);
				}
			}
			if((information.ValidationId.HasValue) && (information.ValidationId > 0))
			{
				var validator = GetValidator(information.ValidationId.Value, culture);
				if(validator != null)
					aggregateValidator.Add(validator);
			}
			if(aggregateValidator.Count > 0)
				result.Value.Validation = aggregateValidator;
			return result.Value;
		}

		/// <summary>
		/// Carrega as informações das propriedades de um determinado tipo.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade.</param>
		/// <returns></returns>
		public override IEnumerable<IEntityPropertyInfo> LoadTypeProperties(Reflection.TypeName entityTypeName)
		{
			return Settings.LoadTypeProperties(entityTypeName);
		}

		/// <summary>
		/// Carrega as informações das propriedades de um determinado tipo.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade.</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		public override IEntityPropertyInfo LoadTypeProperty(Reflection.TypeName entityTypeName, string propertyName)
		{
			foreach (var i in LoadTypeProperties(entityTypeName))
				if(i.FullName == propertyName)
					return i;
			return null;
		}

		/// <summary>
		/// Carrega os labels de uma propriedade.
		/// </summary>
		/// <param name="typeName">Tipo</param>
		/// <param name="uiContext">Contexto com o usuário</param>
		/// <returns></returns>
		public override IEnumerable<PropertyLabelInfo> GetLabels(Reflection.TypeName typeName, string uiContext)
		{
			return Settings.GetLabels(typeName, uiContext);
		}

		/// <summary>
		/// Carrega os labels de uma propriedade.
		/// </summary>
		/// <param name="typeName">Tipo</param>
		/// <param name="uiContext">Contexto com o usuário</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		public override IEnumerable<PropertyLabelInfo> GetLabels(Reflection.TypeName typeName, string uiContext, string propertyName)
		{
			return GetLabels(typeName, uiContext).Where(f => f.PropertyName == propertyName);
		}

		/// <summary>
		/// Carrega os labels de uma propriedade.
		/// </summary>
		/// <param name="typeName">Tipo</param>
		/// <param name="uiContext">Contexto com o usuário</param>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns></returns>
		public override PropertyLabelInfo GetLabel(Reflection.TypeName typeName, string uiContext, string propertyName)
		{
			return GetLabels(typeName, uiContext, propertyName).FirstOrDefault();
		}

		/// <summary>
		/// Carrega as informações de validação de entrada para um grupo de regra de entrada específico.
		/// </summary>
		/// <param name="inputRulesGroupId"></param>
		/// <param name="uiContext"></param>
		/// <returns></returns>
		public override IInputValidationInfo GetInputValidateInfo(int inputRulesGroupId, string uiContext)
		{
			var result = new InputValidationInfo();
			InputValidate inputValidate = Settings.GetInputValidate(inputRulesGroupId, uiContext);
			if(inputValidate != null)
			{
				var aggregateValidator = new Validators.AggregateValidator();
				CalculateInputValidateInfo(result, inputValidate, aggregateValidator, new List<Guid>());
			}
			return result;
		}

		/// <summary>
		/// Calcula as informações de um Inputvalidate.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="inputValidate"></param>
		/// <param name="aggregateValidator"></param>
		/// <param name="ignoreUids">Identificador da validações que deve ser ignoradas</param>
		private void CalculateInputValidateInfo(IInputValidationInfo info, InputValidate inputValidate, Validators.AggregateValidator aggregateValidator, IList<Guid> ignoreUids)
		{
			if(inputValidate != null && !ignoreUids.Contains(inputValidate.Uid))
			{
				ignoreUids.Add(inputValidate.Uid);
				switch(inputValidate.Type)
				{
				case InputValidateType.CharacterUpperCase:
					info.CharCase = CharacterCase.Upper;
					break;
				case InputValidateType.CharacterLowerCase:
					info.CharCase = CharacterCase.Lower;
					break;
				case InputValidateType.Customization:
					info.Customization = inputValidate.Customization;
					if(info.Customization != null)
						aggregateValidator.Add(new Validators.CustomizationValidator(info.Customization));
					break;
				case InputValidateType.CheckDigits:
					info.CheckDigits = inputValidate.CheckDigits;
					if(info.CheckDigits != null)
						aggregateValidator.Add(new Validators.CheckDigitsValidator(info.CheckDigits));
					break;
				case InputValidateType.DefaultValue:
					info.DefaultValue = inputValidate.Default;
					break;
				case InputValidateType.Group:
					foreach (var i in inputValidate.Group.Items.Select(f => Settings.InputValidates.FirstOrDefault(x => x.Uid == f.InputValidateUid)).Where(f => f != null))
						CalculateInputValidateInfo(info, i, aggregateValidator, ignoreUids);
					break;
				case InputValidateType.IndexedValues:
					info.IndexedValues = inputValidate.IndexedValues;
					break;
				case InputValidateType.Length:
					info.Length = inputValidate.Length;
					if(info.Length != null)
						aggregateValidator.Add(new Validators.LengthValidator(info.Length));
					break;
				case InputValidateType.Mask:
					info.Mask = inputValidate.Mask;
					break;
				case InputValidateType.Range:
					info.Range = inputValidate.Range;
					if(info.Range != null)
						aggregateValidator.Add(new Validators.RangeValidator(info.Range));
					break;
				case InputValidateType.ValidChars:
					info.ValidChars = inputValidate.ValidChars;
					if(info.ValidChars != null)
						aggregateValidator.Add(new Validators.ValidCharsValidator(info.ValidChars));
					break;
				case InputValidateType.Values:
					info.Values = inputValidate.Values;
					break;
				}
			}
		}

		/// <summary>
		/// Implementação do comparador da chaves das mascaras de propriedades.
		/// </summary>
		class PropertyMasksKeyComparer : IEqualityComparer<Tuple<Type, string>>
		{
			public readonly static PropertyMasksKeyComparer Instance = new PropertyMasksKeyComparer();

			public bool Equals(Tuple<Type, string> x, Tuple<Type, string> y)
			{
				return (object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null)) && ((!object.ReferenceEquals(x, null) && !object.ReferenceEquals(y, null)) && x.Item1 == y.Item1 && x.Item2 == y.Item2);
			}

			public int GetHashCode(Tuple<Type, string> obj)
			{
				if(obj == null)
					return 0;
				return obj.Item1.GetHashCode() ^ (obj.Item2 == null ? 1 : obj.Item2.GetHashCode());
			}
		}
	}
}
