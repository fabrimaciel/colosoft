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

namespace Colosoft.Business
{
	/// <summary>
	/// Método para notificação de alteração de evento
	/// </summary>
	/// <param name="properties">propriedades alteradas</param>
	public delegate void NotifyPropertyChangedHandler (params string[] properties);
	/// <summary>
	/// Contrato para tipos que manterão o estado de instâncias
	/// </summary>
	public class InstanceState : Colosoft.NotificationObject, Colosoft.Validation.IStateble, IDisposableState
	{
		private IEntity _entity;

		private string _entityTypeStartString;

		private System.ComponentModel.PropertyChangedEventHandler _propertyChangeNotify;

		private readonly object _propertyAttributesLock = new object();

		private Dictionary<string, IStatebleItem> _propertyAttributes;

		private IValidationManager _validationManager;

		/// <summary>
		/// Configurações da entidade.
		/// </summary>
		private IEnumerable<IPropertySettingsInfo> _entitySettings;

		private IEntityTypeManager _entityTypeManager;

		private Dictionary<string, Colosoft.Reflection.TypeName> _specializedList;

		private Dictionary<string, IStatebleItem> _cacheAttributes;

		private List<string> _clearList;

		private bool _isDisposed;

		private StatebleParameterCollection _parameters = new StatebleParameterCollection();

		/// <summary>
		/// Evento acionado quando o estado for alterado.
		/// </summary>
		public event StateChangedEventHandler StateChanged;

		/// <summary>
		/// Parametros do controle de estado.
		/// </summary>
		public StatebleParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Identifica se a entidade está em estado de somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return !_entity.CanEdit;
			}
		}

		/// <summary>
		/// Recupera a string de inicio para o tipo da entidade.
		/// </summary>
		private string EntityTypeStartString
		{
			get
			{
				if(_entityTypeStartString == null)
				{
					Type entityType = _entity.GetType();
					_entityTypeStartString = string.Format("{0},{1}", entityType.GetAssemblyName(), entityType.FullName);
				}
				return _entityTypeStartString;
			}
		}

		/// <summary>
		/// Tipo base do item.
		/// </summary>
		public Reflection.TypeName Type
		{
			get
			{
				return Reflection.TypeName.Get(_entity.GetType());
			}
		}

		/// <summary>
		/// Dicionário com a relação dos attributos das propriedades do tipo associado com o estado.
		/// </summary>
		internal Dictionary<string, IStatebleItem> PropertyAttributes
		{
			get
			{
				return _propertyAttributes;
			}
		}

		/// <summary>
		/// Retorna a propriedade desejada
		/// </summary>
		/// <param name="propertyName">Nome da propriedade</param>
		/// <returns>Estado da propriedade</returns>
		public Colosoft.Validation.IStatebleItem this[string propertyName]
		{
			get
			{
				try
				{
					if(_cacheAttributes == null)
						_cacheAttributes = new Dictionary<string, IStatebleItem>();
					var containsProperty = false;
					var renew = false;
					lock (_cacheAttributes)
					{
						containsProperty = _cacheAttributes.ContainsKey(propertyName);
						renew = (!_clearList.IsNullOrEmpty()) && _clearList.Contains(propertyName);
						if(renew)
							_clearList.Remove(propertyName);
					}
					if((!containsProperty) || renew)
					{
						var item = GetProperty(EntityTypeStartString, propertyName, Colosoft.Globalization.Culture.SystemCulture);
						var original = item;
						lock (_cacheAttributes)
						{
							if(renew && containsProperty)
							{
								item.PropertyChanged -= StateItemChanged;
								item = _cacheAttributes[propertyName];
							}
							else if(!containsProperty)
							{
								if(!_cacheAttributes.ContainsKey(propertyName))
									_cacheAttributes.Add(propertyName, item);
							}
							else
							{
								item.PropertyChanged -= StateItemChanged;
								item = _cacheAttributes[propertyName];
							}
						}
						if(renew && containsProperty)
						{
							item.CopyFrom(original);
						}
						return item;
					}
					else
						lock (_cacheAttributes)
							return _cacheAttributes[propertyName];
				}
				catch(System.Collections.Generic.KeyNotFoundException ex)
				{
					throw new System.Collections.Generic.KeyNotFoundException(string.Format(Properties.Resources.Exception_KeyNotFound, propertyName), ex);
				}
			}
		}

		/// <summary>
		/// Construtor padrão da classe
		/// </summary>
		/// <param name="entity">Entidade associada.</param>
		/// <param name="entityTypeManager">Gerenciador dos tipos de entidades.</param>
		/// <param name="validationManager">Instancia do gerenciador de validações.</param>
		/// <param name="propertyChangeNotify">Método que receberá notificação de evento</param>
		/// <param name="culture"></param>
		public InstanceState(IEntity entity, IEntityTypeManager entityTypeManager, IValidationManager validationManager, System.ComponentModel.PropertyChangedEventHandler propertyChangeNotify, System.Globalization.CultureInfo culture)
		{
			entity.Require("entity").NotNull();
			validationManager.Require("validationManager").NotNull();
			propertyChangeNotify.Require("propertyChangeNotify").NotNull();
			_specializedList = new Dictionary<string, Colosoft.Reflection.TypeName>();
			_entityTypeManager = entityTypeManager;
			_validationManager = validationManager;
			_entity = entity;
			_propertyChangeNotify = propertyChangeNotify;
			_propertyAttributes = new Dictionary<string, IStatebleItem>();
			LoadTypeSettings(culture);
			_entity.PropertyChanged += EntityPropertyChanged;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~InstanceState()
		{
			Dispose(false);
		}

		/// <summary>
		/// Método acionado quando qualquer propriedade da entidade for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EntityPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "IsReadOnly" || e.PropertyName == "IsLockedToEdit" || e.PropertyName == "CanEdit")
				RaisePropertyChanged("IsReadOnly");
		}

		/// <summary>
		/// Recupera os dados da propriedade.
		/// </summary>
		/// <param name="startString"></param>
		/// <param name="propertyName"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		private IStatebleItem GetProperty(string startString, string propertyName, System.Globalization.CultureInfo culture)
		{
			string findPropertyName = string.Format("{0}.{1}", startString, propertyName);
			string findPropertyNameGeneral = string.Format("{0}.*.{1}", startString, propertyName);
			string identifierFound = null;
			if(_specializedList != null)
				foreach (string identifier in _specializedList.Keys.OrderByDescending(f => f.Split('.').Length))
				{
					if(findPropertyName.StartsWith(identifier))
					{
						identifierFound = identifier;
						findPropertyName = findPropertyName.Replace(identifier, identifier + GetSpecializationString(identifier.Replace(startString, ""), _specializedList[identifier]) + ".");
					}
				}
			if(PropertyAttributes != null && PropertyAttributes.ContainsKey(findPropertyName))
			{
				var statebleItem = (IStatebleItem)PropertyAttributes[findPropertyName].Clone();
				statebleItem.PropertyChanged += StateItemChanged;
				if(statebleItem != null && !statebleItem.IsConfigured && PropertyAttributes.ContainsKey(findPropertyNameGeneral) && PropertyAttributes[findPropertyNameGeneral].IsConfigured)
				{
					var result = (IStatebleItem)PropertyAttributes[findPropertyNameGeneral].Clone();
					result.PropertyChanged += StateItemChanged;
					return result;
				}
				return statebleItem;
			}
			else
			{
				var propertyState = CreatePropertyState(propertyName, identifierFound, culture);
				lock (_propertyAttributesLock)
					if(PropertyAttributes != null && !PropertyAttributes.ContainsKey(findPropertyName))
						PropertyAttributes.Add(findPropertyName, propertyState);
				return propertyState;
			}
		}

		/// <summary>
		/// Recupera a string da especialização do tipo informado.
		/// </summary>
		/// <param name="specialization"></param>
		/// <param name="pTypeName"></param>
		/// <returns></returns>
		private string GetSpecializationString(string specialization, Colosoft.Reflection.TypeName pTypeName)
		{
			if(!InstanceStateManager.ContainsSpecializationError(pTypeName))
			{
				string[] path = specialization.Split('.').Where(f => (!String.IsNullOrEmpty(f))).ToArray();
				object obj = _entity;
				for(int index = 0; index < path.Length - 1; index++)
				{
					if(obj != null)
						obj = obj.GetType().GetProperty(path[index]).GetValue(obj, null);
				}
				return _validationManager.LoadSpecilization(pTypeName).GetInstanceSpecilizationString(obj);
			}
			return null;
		}

		/// <summary>
		/// Método acionado quando um item de estado for alterado.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StateItemChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnChanged((IStatebleItem)sender, e.PropertyName);
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Validation.IStatebleItem> GetEnumerator()
		{
			return PropertyAttributes.Values.GetEnumerator();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return PropertyAttributes.Values.GetEnumerator();
		}

		/// <summary>
		/// Notifica que o estado foi alterado.
		/// </summary>
		protected void OnChanged(IStatebleItem obj, string stateName)
		{
			if(StateChanged != null)
				StateChanged(this, new StateChangedEventArgs(obj.PropertyName, stateName));
		}

		/// <summary>
		/// Notifica que uma propriedade do estado foi alterada.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade.</param>
		public void NotifyStatePropertyChanged(string propertyName)
		{
			if(_propertyChangeNotify != null)
				_propertyChangeNotify(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Recarrega as definições de validação de propriedades para a instância.
		/// </summary>
		public void ReloadTypeSettings()
		{
		}

		/// <summary>
		///  Carrega as configurações do tipo.
		/// </summary>
		/// <param name="culture"></param>
		private void LoadTypeSettings(System.Globalization.CultureInfo culture)
		{
			var entityTypeName = Colosoft.Reflection.TypeName.Get(_entity.GetType());
			if(_entitySettings == null)
				_entitySettings = _validationManager.LoadSettings(entityTypeName);
			string startString = EntityTypeStartString;
			var allIdentifiers = GetIdentifierSpecialize(entityTypeName, startString, startString, String.Empty);
			foreach (string identifier in allIdentifiers.Keys)
			{
				IPropertySettingsInfo currentPropertSettings = _entitySettings.Where(f => f.Identifier == identifier).FirstOrDefault();
				if(currentPropertSettings != null)
				{
					IStatebleItem stateble = CreatePropertyStateByIdentifier(allIdentifiers[identifier], identifier, culture);
					if(stateble != null)
					{
						stateble.IsConfigured = true;
						_propertyAttributes.Add(identifier, stateble);
					}
				}
				else
				{
					string[] propertyPath = allIdentifiers[identifier].Split('.');
					var currentObjectTypeName = Colosoft.Reflection.TypeName.Get(this.GetType());
					bool findPath = true;
					IEntityPropertyInfo info = null;
					for(int index = 0; index < propertyPath.Length - 1; index++)
					{
						string currentProperty = propertyPath[index];
						info = _validationManager.LoadTypeProperty(currentObjectTypeName, currentProperty);
						if(info == null)
						{
							findPath = false;
							break;
						}
						else
							currentObjectTypeName = info.PropertyType;
					}
					if((findPath) && (info != null) && (info.IsInstance))
					{
						IPropertySettingsInfo propertyInfo = _validationManager.LoadSettings(currentObjectTypeName).Where(f => f.Identifier.EndsWith(propertyPath[propertyPath.Length - 1])).FirstOrDefault();
						if(propertyInfo != null)
						{
							var propertyStateble = CreatePropertyState(allIdentifiers[identifier], propertyInfo, identifier, culture);
							propertyStateble.IsConfigured = true;
							_propertyAttributes.Add(identifier, propertyStateble);
						}
						else
						{
							var propertyState = CreatePropertyState(allIdentifiers[identifier], identifier, culture);
							_propertyAttributes.Add(identifier, propertyState);
						}
					}
					else
					{
						var propertyState = CreatePropertyState(allIdentifiers[identifier], identifier, culture);
						_propertyAttributes.Add(identifier, propertyState);
					}
				}
			}
		}

		/// <summary>
		/// Cria o estado da propriedade.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="identifier"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		private IStatebleItem CreatePropertyState(string propertyName, string identifier, System.Globalization.CultureInfo culture)
		{
			return CreatePropertyState(propertyName, new PropertySettingsInfo(identifier), identifier, culture);
		}

		/// <summary>
		/// Cria o estado da propriedade.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propertyInfo"></param>
		/// <param name="identifier"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		private IStatebleItem CreatePropertyState(string propertyName, IPropertySettingsInfo propertyInfo, string identifier, System.Globalization.CultureInfo culture)
		{
			IStatebleItem propertyStateble = _validationManager.CreatePropertyState(this, propertyName, propertyInfo, _entity.UIContext, culture);
			if(propertyStateble != null)
				propertyStateble.PropertyChanged += StateItemChanged;
			return propertyStateble;
		}

		/// <summary>
		/// Recupera os identificadores da especialização.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade.</param>
		/// <param name="startString"></param>
		/// <param name="clearPath">Caminho usado para limpeza na comparação</param>
		/// <param name="startPropertyName">Nome da propriedade de inicio para recuperar</param>
		/// <param name="nestedTypes">Relação dos tipos aninhadas encontrados.</param>
		/// <returns></returns>
		private Dictionary<string, string> GetIdentifierSpecialize(Colosoft.Reflection.TypeName entityTypeName, string startString, string clearPath, string startPropertyName, List<Colosoft.Reflection.TypeName> nestedTypes = null)
		{
			var result = new Dictionary<string, string>();
			if(!InstanceStateManager.ContainsSpecializationError(entityTypeName))
			{
				if(!startString.EndsWith("."))
					startString = startString + ".";
				if(!clearPath.EndsWith("."))
					clearPath = clearPath + ".";
				if(nestedTypes == null)
					nestedTypes = new List<Colosoft.Reflection.TypeName>();
				nestedTypes.Add(entityTypeName);
				Colosoft.Validation.IEntitySpecialization specialization = null;
				try
				{
					specialization = _validationManager.LoadSpecilization(entityTypeName);
				}
				catch(Exception ex)
				{
					InstanceStateManager.RegisterSpecializationError(entityTypeName, null, ex);
				}
				if(specialization != null)
				{
					if(!_specializedList.ContainsKey(clearPath))
						_specializedList.Add(clearPath, entityTypeName);
					var existsAllSpecialization = false;
					string[] specializations = null;
					try
					{
						specializations = specialization.GetSpecializations().ToArray();
					}
					catch(Exception ex)
					{
						InstanceStateManager.RegisterSpecializationError(entityTypeName, null, ex);
						specializations = new string[0];
					}
					foreach (string currentSpecilizeString in specializations)
					{
						if(currentSpecilizeString == "*" && existsAllSpecialization)
							existsAllSpecialization = true;
						PutsPropertiesIdentifier(entityTypeName, nestedTypes, result, startString, clearPath, startPropertyName, currentSpecilizeString);
					}
					if(!existsAllSpecialization)
						PutsPropertiesIdentifier(entityTypeName, nestedTypes, result, startString, clearPath, startPropertyName, "*");
				}
				else
				{
					PutsPropertiesIdentifier(entityTypeName, nestedTypes, result, startString, clearPath, startPropertyName, string.Empty);
				}
				nestedTypes.Remove(entityTypeName);
			}
			return result;
		}

		/// <summary>
		/// Adiciona os identificadores das propriedades.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade.</param>
		/// <param name="nestedTypes">Rleção dos tipos aninhados.</param>
		/// <param name="listToAppend">Lista onde será adicionada a relação do caminho com a especiação para o caminho da propriedade</param>
		/// <param name="startString">Texto com o inicio do caminho para alcançar a propriedade.</param>
		/// <param name="clearPath"></param>
		/// <param name="propertyPath">Caminho para alcançar a propriedade.</param>
		/// <param name="currentSpecilizeString">Texto que representa a especialização.</param>
		private void PutsPropertiesIdentifier(Colosoft.Reflection.TypeName entityTypeName, List<Colosoft.Reflection.TypeName> nestedTypes, Dictionary<string, string> listToAppend, string startString, string clearPath, string propertyPath, string currentSpecilizeString)
		{
			IEnumerable<IEntityPropertyInfo> properties = _validationManager.LoadTypeProperties(entityTypeName);
			if(properties != null)
			{
				foreach (IEntityPropertyInfo property in properties)
				{
					string newPropertyPath = string.IsNullOrEmpty(propertyPath) ? property.FullName : string.Format("{0}.{1}", propertyPath, property.FullName);
					if(!string.IsNullOrEmpty(currentSpecilizeString))
						listToAppend.Add(string.Format("{0}{1}.{2}", startString, currentSpecilizeString, property.FullName), newPropertyPath);
					else
						listToAppend.Add((startString + property.FullName), newPropertyPath);
					if(property.IsInstance && !nestedTypes.Contains(property.PropertyType, Colosoft.Reflection.TypeName.TypeNameFullNameComparer.Instance))
					{
						var aux = GetIdentifierSpecialize(property.PropertyType, string.Format("{0}{1}.{2}.", startString, currentSpecilizeString, property.FullName), string.Format("{0}{1}.", clearPath, property.FullName), newPropertyPath, nestedTypes);
						foreach (var kp in aux)
							listToAppend.Add(kp.Key, kp.Value);
					}
				}
			}
		}

		/// <summary>
		/// Cria o estado da propriedade pelo identificador.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade.</param>
		/// <param name="identifier"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		private IStatebleItem CreatePropertyStateByIdentifier(string propertyName, string identifier, System.Globalization.CultureInfo culture)
		{
			if(_entitySettings == null)
				return null;
			IPropertySettingsInfo propertyInfo = _entitySettings.Where(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Identifier, identifier)).FirstOrDefault();
			if(propertyInfo != null)
			{
				var propertyState = _validationManager.CreatePropertyState(this, propertyName, propertyInfo, _entity.UIContext, culture);
				propertyState.PropertyChanged += StateItemChanged;
				return propertyState;
			}
			else
				return null;
		}

		/// <summary>
		/// Reinicia o cache em caso afirmativo.
		/// </summary>
		public void ClearStateCache()
		{
			if(_cacheAttributes != null)
			{
				lock (_cacheAttributes)
				{
					if(_clearList == null)
						_clearList = new List<string>();
					else
						_clearList.Clear();
					foreach (var pair in _cacheAttributes)
						_clearList.Add(pair.Key);
				}
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_isDisposed = true;
			_entity.PropertyChanged -= EntityPropertyChanged;
			if(StateChanged != null)
				foreach (var call in StateChanged.GetInvocationList())
					StateChanged -= (StateChangedEventHandler)call;
			if(_propertyAttributes != null)
			{
				foreach (var propAttr in _propertyAttributes)
				{
					if(propAttr.Value != null)
						propAttr.Value.PropertyChanged -= StateItemChanged;
					if(propAttr.Value is IDisposable)
						((IDisposable)propAttr.Value).Dispose();
				}
				_propertyAttributes.Clear();
			}
			_propertyAttributes = null;
			if(_entitySettings != null && _entitySettings is System.Collections.IList)
				((System.Collections.IList)_entitySettings).Clear();
			_entitySettings = null;
			if(_specializedList != null)
				_specializedList.Clear();
			_specializedList = null;
			if(_cacheAttributes != null)
			{
				foreach (var cacheAttr in _cacheAttributes)
				{
					if(cacheAttr.Value != null)
						cacheAttr.Value.PropertyChanged -= StateItemChanged;
					if(cacheAttr.Value is IDisposable)
						((IDisposable)cacheAttr.Value).Dispose();
				}
				_cacheAttributes.Clear();
			}
			_cacheAttributes = null;
			if(_clearList != null)
			{
				_clearList.Clear();
				_clearList = null;
			}
		}

		/// <summary>
		/// Remove as referências.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Armazena as informações da configuração da propriedade.
		/// </summary>
		class PropertySettingsInfo : IPropertySettingsInfo
		{
			private string _identifier;

			/// <summary>
			/// Indica se a propriedade será ou não copiada ao copiar o objeto.
			/// </summary>
			public bool CopyValue
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Identificador da propriedade.
			/// </summary>
			public string Identifier
			{
				get
				{
					return _identifier;
				}
			}

			/// <summary>
			/// Identificador do grupo de regras de entrada.
			/// </summary>
			public int? InputRulesGroupId
			{
				get
				{
					return null;
				}
			}

			/// <summary>
			/// Identifica se recarrega as configurações.
			/// </summary>
			public bool ReloadSettings
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Identificador da validação associada.
			/// </summary>
			public int? ValidationId
			{
				get
				{
					return null;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="identifier"></param>
			public PropertySettingsInfo(string identifier)
			{
				_identifier = identifier;
			}
		}

		/// <summary>
		/// Identifica se a instancia já foi liberada.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return _isDisposed;
			}
		}
	}
}
