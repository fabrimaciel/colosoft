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
using Colosoft.Validation;
using Colosoft.Util;
using Colosoft.Collections;

namespace Colosoft.Business
{
	/// <summary>
	/// Contrato para tipos que conterão informações das propriedades
	/// </summary>
	public class BusinessEntityPropertyState : NotificationObject, Colosoft.Validation.IStatebleItem, IDisposable
	{
		private CharacterCase _charCase;

		/// <summary>
		/// Lista de validações
		/// </summary>
		private IValidator _validation;

		/// <summary>
		/// Outros atributos da propriedade
		/// </summary>
		private Dictionary<string, object> _otherAttributes;

		/// <summary>
		/// Nome da propriedade
		/// </summary>
		private string _propertyName;

		/// <summary>
		/// Label da propriedade
		/// </summary>
		private IPropertyLabel _label;

		/// <summary>
		/// Sequência de valores indexados
		/// </summary>
		private IEnumerable<IIndexedValue> _indexedValues;

		/// <summary>
		/// Validação de dígitos verificadores.
		/// </summary>
		private ICheckDigits _checkDigits;

		/// <summary>
		/// Validação de valor padrão.
		/// </summary>
		private IDefaultValue _defaultValue;

		/// <summary>
		/// Validação de tamanho.
		/// </summary>
		private ILength _length;

		/// <summary>
		/// Máscara da propriedade.
		/// </summary>
		private IMask _mask;

		/// <summary>
		/// Validação de caracteres válidos.
		/// </summary>
		private IValidChars _validChars;

		/// <summary>
		/// Lista de valores.
		/// </summary>
		private IEnumerable<IPropertyValue> _values;

		/// <summary>
		/// Indicador de necessidade da cópia da propriedade na cópia da instância pai.
		/// </summary>
		private bool _copyValue;

		/// <summary>
		/// Validação de faixa.
		/// </summary>
		private IRange _range;

		/// <summary>
		/// Indica a necessidade de recarga das propriedades.
		/// </summary>
		private bool _reloadSettings;

		/// <summary>
		/// Identificador da configuração.
		/// </summary>
		private string _identifier;

		/// <summary>
		/// Opções de tela da propriedade.
		/// </summary>
		private InputRulesOptions _options;

		/// <summary>
		/// Conversor.
		/// </summary>
		private IParse _parser;

		/// <summary>
		/// Indicador de configuração atualizada.
		/// </summary>
		private bool _isConfigured;

		/// <summary>
		/// Customização do inputvalidate.
		/// </summary>
		private IInputValidateCustomization _customization;

		/// <summary>
		/// Dono da propriedade.
		/// </summary>
		private IStateble _owner;

		/// <summary>
		/// Caso de entrada de caracteres.
		/// </summary>
		public CharacterCase CharCase
		{
			get
			{
				return _charCase;
			}
			set
			{
				if(_charCase != value)
				{
					_charCase = value;
					RaisePropertyChanged("CharCase");
				}
			}
		}

		/// <summary>
		/// Nome da propriedade
		/// </summary>
		public string PropertyName
		{
			get
			{
				return _propertyName;
			}
			set
			{
				_propertyName = value;
				RaisePropertyChanged("PropertyName");
			}
		}

		/// <summary>
		/// Retorna o atributo desejado
		/// </summary>
		/// <param name="attributeName">Nome do atributo</param>
		/// <returns>Atributo</returns>
		object this[string attributeName]
		{
			get
			{
				if(_otherAttributes != null && _otherAttributes.ContainsKey(attributeName))
				{
					return _otherAttributes[attributeName];
				}
				return null;
			}
			set
			{
				Owner.NotifyStatePropertyChanged(string.Format("{0}.{1}", PropertyName, attributeName));
				if(_otherAttributes == null)
					_otherAttributes = new Dictionary<string, object>();
				if(_otherAttributes.ContainsKey(attributeName))
					_otherAttributes[attributeName] = value;
				else
					_otherAttributes.Add(attributeName, value);
			}
		}

		/// <summary>
		/// Validações da propriedade
		/// </summary>
		public IValidator Validation
		{
			get
			{
				return _validation;
			}
			set
			{
				if(_validation != value)
				{
					_validation = value;
					RaisePropertyChanged("Validation");
				}
			}
		}

		/// <summary>
		/// Label da propriedade.
		/// </summary>
		public IPropertyLabel Label
		{
			get
			{
				return _label;
			}
			set
			{
				if(_label != value)
				{
					_label = value;
					RaisePropertyChanged("Label");
				}
			}
		}

		/// <summary>
		/// Coleção de valores indexados.
		/// </summary>
		public IEnumerable<IIndexedValue> IndexedValues
		{
			get
			{
				return _indexedValues;
			}
			set
			{
				if(_indexedValues != value)
				{
					_indexedValues = value;
					RaisePropertyChanged("IndexedValues");
				}
			}
		}

		/// <summary>
		/// Validação de dígitos verificadores.
		/// </summary>
		public ICheckDigits CheckDigits
		{
			get
			{
				return _checkDigits;
			}
			set
			{
				if(_checkDigits != value)
				{
					_checkDigits = value;
					RaisePropertyChanged("CheckDigits");
				}
			}
		}

		/// <summary>
		/// Validação de valor padrão.
		/// </summary>
		public IDefaultValue DefaultValue
		{
			get
			{
				return _defaultValue;
			}
			set
			{
				if(_defaultValue != value)
				{
					_defaultValue = value;
					RaisePropertyChanged("DefaultValue");
				}
			}
		}

		/// <summary>
		/// Validação de tamanho.
		/// </summary>
		public ILength Length
		{
			get
			{
				return _length;
			}
			set
			{
				if(_length != value)
				{
					_length = value;
					RaisePropertyChanged("Length");
				}
			}
		}

		/// <summary>
		/// Máscara aplicada à propriedade
		/// </summary>
		public IMask Mask
		{
			get
			{
				return _mask;
			}
			set
			{
				if(_mask != value)
				{
					_mask = value;
					RaisePropertyChanged("Mask");
				}
			}
		}

		/// <summary>
		/// Validação de caracteres válidos.
		/// </summary>
		public IValidChars ValidChars
		{
			get
			{
				return _validChars;
			}
			set
			{
				if(_validChars != value)
				{
					_validChars = value;
					RaisePropertyChanged("ValidChars");
				}
			}
		}

		/// <summary>
		/// Lista de valores.
		/// </summary>
		public IEnumerable<IPropertyValue> Values
		{
			get
			{
				return _values;
			}
			set
			{
				if(_values != value)
				{
					_values = value;
					RaisePropertyChanged("Values");
				}
			}
		}

		/// <summary>
		/// Se o valor será copiado ao se copiar o ítem.
		/// </summary>
		public bool CopyValue
		{
			get
			{
				return _copyValue;
			}
			set
			{
				if(_copyValue != value)
				{
					_copyValue = value;
					RaisePropertyChanged("CopyValue");
				}
			}
		}

		/// <summary>
		/// Validação de faixa.
		/// </summary>
		public IRange Range
		{
			get
			{
				return _range;
			}
			set
			{
				if(_range != value)
				{
					_range = value;
					RaisePropertyChanged("Range");
				}
			}
		}

		/// <summary>
		/// Indica que as propriedades devem ser recarregadas.
		/// </summary>
		public bool ReloadSettings
		{
			get
			{
				return _reloadSettings;
			}
			set
			{
				if(_reloadSettings != value)
				{
					_reloadSettings = value;
					RaisePropertyChanged("ReloadSettings");
				}
			}
		}

		/// <summary>
		/// Identificador da configuração.
		/// </summary>
		public string Identifier
		{
			get
			{
				return _identifier;
			}
			set
			{
				if(String.Equals(_identifier, value))
				{
					_identifier = value;
					RaisePropertyChanged("Identifier");
				}
			}
		}

		/// <summary>
		/// Opções de tela da propriedade.
		/// </summary>
		public InputRulesOptions Options
		{
			get
			{
				return _options;
			}
			set
			{
				if(_options != value)
				{
					_options = value;
					RaisePropertyChanged("Options", "IsReadOnly", "IsEnabled", "IsSorted", "IsRequired", "IsNecessary", "IsHidden", "Visibility");
				}
			}
		}

		/// <summary>
		/// Conversor.
		/// </summary>
		public IParse Parser
		{
			get
			{
				return _parser;
			}
			set
			{
				if(_parser != value)
				{
					_parser = value;
					RaisePropertyChanged("Parser");
				}
			}
		}

		/// <summary>
		/// Indica se foi ou não configurado.
		/// </summary>
		public bool IsConfigured
		{
			get
			{
				return _isConfigured;
			}
			set
			{
				if(_isConfigured != value)
				{
					_isConfigured = value;
					RaisePropertyChanged("IsConfigured");
				}
			}
		}

		/// <summary>
		/// Identifica se o item é somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return Options.HasFlag(InputRulesOptions.ReadOnly) || Owner.IsReadOnly;
			}
			set
			{
				if((!Options.HasFlag(InputRulesOptions.ReadOnly)) && value)
				{
					Options |= InputRulesOptions.ReadOnly;
				}
				else if(Options.HasFlag(InputRulesOptions.ReadOnly) && (!value))
				{
					Options &= ~InputRulesOptions.ReadOnly;
				}
				RaisePropertyChanged("IsReadOnly", "Options");
			}
		}

		/// <summary>
		/// Indica se o item está habilitado.
		/// </summary>
		public bool IsEnabled
		{
			get
			{
				return !this.Options.HasFlag(InputRulesOptions.Disable);
			}
			set
			{
				if((Options.HasFlag(InputRulesOptions.Disable)) && value)
				{
					Options &= ~InputRulesOptions.Disable;
				}
				else if((!Options.HasFlag(InputRulesOptions.Disable)) && (!value))
				{
					Options |= InputRulesOptions.Disable;
				}
				RaisePropertyChanged("IsEnabled", "Options");
			}
		}

		/// <summary>
		/// Indica se a lista de valores do item deve ser ordenada.
		/// </summary>
		public bool IsSorted
		{
			get
			{
				return Options.HasFlag(InputRulesOptions.Sorted);
			}
			set
			{
				if((!Options.HasFlag(InputRulesOptions.Sorted)) && value)
				{
					Options |= InputRulesOptions.Sorted;
				}
				else if(Options.HasFlag(InputRulesOptions.Sorted) && (!value))
				{
					Options &= ~InputRulesOptions.Sorted;
				}
				RaisePropertyChanged("IsSorted", "Options");
			}
		}

		/// <summary>
		/// Indica se o item tem preenchimento válido obrigatório.
		/// </summary>
		public bool IsRequired
		{
			get
			{
				return Options.HasFlag(InputRulesOptions.Required);
			}
			set
			{
				if((!Options.HasFlag(InputRulesOptions.Required)) && value)
				{
					Options |= InputRulesOptions.Required;
				}
				else if(Options.HasFlag(InputRulesOptions.Required) && (!value))
				{
					Options &= ~InputRulesOptions.Required;
				}
				RaisePropertyChanged("IsRequired", "Options");
			}
		}

		/// <summary>
		/// Indica se o item é obrigatório.
		/// </summary>
		public bool IsNecessary
		{
			get
			{
				return Options.HasFlag(InputRulesOptions.Necessary);
			}
			set
			{
				if((!Options.HasFlag(InputRulesOptions.Necessary)) && value)
				{
					Options |= InputRulesOptions.Necessary;
				}
				else if(Options.HasFlag(InputRulesOptions.Necessary) && (!value))
				{
					Options &= ~InputRulesOptions.Necessary;
				}
				RaisePropertyChanged("IsNecessary", "Options");
			}
		}

		/// <summary>
		/// Indica que o item deve estar oculto.
		/// </summary>
		public bool IsHidden
		{
			get
			{
				return Options.HasFlag(InputRulesOptions.Hidden);
			}
			set
			{
				if((!Options.HasFlag(InputRulesOptions.Hidden)) && value)
				{
					Options |= InputRulesOptions.Hidden;
				}
				else if(Options.HasFlag(InputRulesOptions.Hidden) && (!value))
				{
					Options &= ~InputRulesOptions.Hidden;
				}
				RaisePropertyChanged("IsHidden", "Options");
			}
		}

		/// <summary>
		/// Customização.
		/// </summary>
		public IInputValidateCustomization Customization
		{
			get
			{
				return _customization;
			}
			set
			{
				if(_customization != value)
				{
					_customization = value;
					RaisePropertyChanged("Customization");
				}
			}
		}

		/// <summary>
		/// Opções de visibilidade do item.
		/// </summary>
		public StatebleItemVisibility Visibility
		{
			get
			{
				return (Options & InputRulesOptions.Hidden) != InputRulesOptions.Hidden ? StatebleItemVisibility.Visible : StatebleItemVisibility.Hidden;
			}
			set
			{
				if(Options.HasFlag(InputRulesOptions.Hidden) && (value == StatebleItemVisibility.Hidden))
				{
					Options &= ~InputRulesOptions.Hidden;
				}
				else if((!Options.HasFlag(InputRulesOptions.Hidden)) && (value != StatebleItemVisibility.Visible))
				{
					Options |= InputRulesOptions.Hidden;
				}
				RaisePropertyChanged("Visibility", "Options");
			}
		}

		/// <summary>
		/// Dono da propriedade.
		/// </summary>
		public IStateble Owner
		{
			get
			{
				return _owner;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="owner">Dono da propriedade.</param>
		/// <param name="propertyName">Nome da propriedade</param>
		public BusinessEntityPropertyState(IStateble owner, string propertyName)
		{
			owner.Require("owner").NotNull();
			propertyName.Require("propertyName").NotNull().NotEmpty();
			_charCase = CharacterCase.Undefined;
			_owner = owner;
			_propertyName = propertyName;
			_owner.PropertyChanged += OwnerPropertyChanged;
			_otherAttributes = new Dictionary<string, object>();
			_isConfigured = false;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~BusinessEntityPropertyState()
		{
			Dispose(false);
		}

		/// <summary>
		/// Método acionado quando uma propriedade for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OwnerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "IsReadOnly")
				RaisePropertyChanged("IsReadOnly");
		}

		/// <summary>
		/// Notifica uma alteração na lista de validação
		/// </summary>
		/// <param name="sender">Lista</param>
		/// <param name="e">Argumentos</param>
		private void ChangeValidatorList(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Owner.NotifyStatePropertyChanged(string.Format("{0}.Validators", PropertyName));
		}

		/// <summary>
		/// Verifica se o item não tem validações.
		/// </summary>
		/// <returns></returns>
		public bool IsEmpty()
		{
			return ((_parser == null) && (_identifier == null) && (_range == null) && (_values == null) && (_validChars == null) && (_mask == null) && (_length == null) && (_defaultValue == null) && (_checkDigits == null) && (_indexedValues == null) && (_validation == null) && (_label == null));
		}

		/// <summary>
		/// Copia as informações de outra instância.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public IStatebleItem CopyFrom(IStatebleItem source)
		{
			if(source == null)
			{
				return this;
			}
			Identifier = source.Identifier;
			IsConfigured = source.IsConfigured;
			ReloadSettings = source.ReloadSettings;
			CopyValue = source.CopyValue;
			Options = source.Options;
			if((CheckDigits == null) || (source.CheckDigits == null) || (CheckDigits.Base != source.CheckDigits.Base) || (CheckDigits.Digits != source.CheckDigits.Digits) || (CheckDigits.Limit != source.CheckDigits.Limit) || (CheckDigits.Start != source.CheckDigits.Start))
			{
				CheckDigits = source.CheckDigits;
			}
			if((Customization == null) || (!Customization.Equals(source.Customization)))
			{
				Customization = source.Customization;
			}
			if((DefaultValue == null) || (source.DefaultValue == null) || (!String.Equals(DefaultValue.DefaultValue, source.DefaultValue.DefaultValue)))
			{
				DefaultValue = source.DefaultValue;
			}
			if((IndexedValues == null) || (source.IndexedValues == null) || (!Enumerable.SequenceEqual(IndexedValues.Select(d => d.IndexDescription + "!" + d.IndexValue), source.IndexedValues.Select(d => d.IndexDescription + "!" + d.IndexValue))))
			{
				IndexedValues = source.IndexedValues;
			}
			if((Label == null) || (source.Label == null) || (!String.Equals(Label.Title, source.Label.Title)) || (!String.Equals(Label.Description, source.Label.Description)))
			{
				Label = source.Label;
			}
			if((Length == null) || (source.Length == null) || (Math.Abs(Length.MaxValue - source.Length.MaxValue) > 1E-3) || (Math.Abs(Length.MinValue - source.Length.MinValue) > 1E-3))
			{
				Length = source.Length;
			}
			if((Mask == null) || (source.Mask == null) || (!String.Equals(Mask.Mask, source.Mask.Mask)))
			{
				Mask = source.Mask;
			}
			if(!Object.Equals(Parser, source.Parser))
			{
				Parser = source.Parser;
			}
			if((Range == null) || (source.Range == null) || (!String.Equals(Range.FromValue, source.Range.FromValue)) || (!String.Equals(Range.ToValue, source.Range.ToValue)))
			{
				Range = source.Range;
			}
			var asEquatable = Validation as IEquatable<IValidator>;
			if((Validation == null) || (source.Validation == null) || (!String.Equals(Validation.DefaultMessageTemplate, source.Validation.DefaultMessageTemplate)) || (!String.Equals(Validation.FullName, source.Validation.FullName)) || (Validation.IsExclusiveInList != source.Validation.IsExclusiveInList) || (!String.Equals(Validation.ReturnedParameters, source.Validation.ReturnedParameters)) || (asEquatable == null) || (!asEquatable.Equals(source.Validation)))
			{
				Validation = source.Validation;
			}
			if((ValidChars == null) || (source.ValidChars == null) || (!String.Equals(ValidChars.ValidChars, source.ValidChars.ValidChars)))
			{
				ValidChars = source.ValidChars;
			}
			if((Values == null) || (source.Values == null) || (!Enumerable.SequenceEqual(Values.Select(v => v.Value), source.Values.Select(v => v.Value))))
			{
				Values = source.Values;
			}
			return this;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_otherAttributes.Clear();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Cria uma cópia do objeto (não copia membros, apenas referências).
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			var result = new BusinessEntityPropertyState(Owner, PropertyName);
			result._checkDigits = CheckDigits;
			result._copyValue = CopyValue;
			result._customization = Customization;
			result._defaultValue = DefaultValue;
			result._identifier = Identifier;
			result._indexedValues = IndexedValues;
			result._isConfigured = IsConfigured;
			result._label = Label;
			result._length = Length;
			result._mask = Mask;
			result._options = Options;
			result._otherAttributes = _otherAttributes;
			result._owner = Owner;
			result._parser = Parser;
			result._propertyName = PropertyName;
			result._range = Range;
			result._reloadSettings = ReloadSettings;
			result._validation = Validation;
			result._validChars = ValidChars;
			result._values = Values;
			result._charCase = CharCase;
			return result;
		}
	}
}
