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

using Colosoft.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Validation.Configuration
{
	/// <summary>
	/// Representa a classe de configuração das validação.
	/// </summary>
	public class ValidationSettings : System.Xml.Serialization.IXmlSerializable
	{
		private IObservableCollection<EntityType> _entityTypes = new BaseObservableCollection<EntityType>();

		private IObservableCollection<PropertySettings> _propertySettings = new BaseObservableCollection<PropertySettings>();

		private IObservableCollection<InputRulesGroup> _inputRulesGroups = new BaseObservableCollection<InputRulesGroup>();

		private IObservableCollection<InputRules> _inputRules = new BaseObservableCollection<InputRules>();

		private IObservableCollection<InputValidate> _inputValidates = new BaseObservableCollection<InputValidate>();

		private IObservableCollection<ValidationType> _validationTypes = new BaseObservableCollection<ValidationType>();

		private IObservableCollection<Validation> _validations = new BaseObservableCollection<Validation>();

		private Dictionary<string, System.Collections.IList> _entityProperties = new Dictionary<string, System.Collections.IList>();

		/// <summary>
		/// Tipos das entidades.
		/// </summary>
		public IObservableCollection<EntityType> EntityTypes
		{
			get
			{
				return _entityTypes;
			}
		}

		/// <summary>
		/// Configurações das propriedades.
		/// </summary>
		public IObservableCollection<PropertySettings> PropertySettings
		{
			get
			{
				return _propertySettings;
			}
		}

		/// <summary>
		/// Grupos de regras de entrada.
		/// </summary>
		public IObservableCollection<InputRulesGroup> InputRulesGroups
		{
			get
			{
				return _inputRulesGroups;
			}
		}

		/// <summary>
		/// Versões ativas dos tipos de entidade.
		/// </summary>
		public IEnumerable<EntityTypeVersion> EntityTypeVersions
		{
			get
			{
				return EntityTypes.Select(f => f.CurrentVersion).Where(f => f != null);
			}
		}

		/// <summary>
		/// Regras de entrada.
		/// </summary>
		public IObservableCollection<InputRules> InputRules
		{
			get
			{
				return _inputRules;
			}
		}

		/// <summary>
		/// Validações de entrada.
		/// </summary>
		public IObservableCollection<InputValidate> InputValidates
		{
			get
			{
				return _inputValidates;
			}
		}

		/// <summary>
		/// Tipos de validação.
		/// </summary>
		public IObservableCollection<ValidationType> ValidationTypes
		{
			get
			{
				return _validationTypes;
			}
		}

		/// <summary>
		/// Validações.
		/// </summary>
		public IObservableCollection<Validation> Validations
		{
			get
			{
				return _validations;
			}
		}

		/// <summary>
		/// Carrega as informações das propriedades de um determinado tipo.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade.</param>
		/// <returns></returns>
		public IEnumerable<IEntityPropertyInfo> LoadTypeProperties(Reflection.TypeName entityTypeName)
		{
			var typeFullName = entityTypeName.FullName;
			var entityTypeNamespace = string.Join(".", entityTypeName.Namespace);
			System.Collections.IList properties = null;
			bool found = false;
			lock (_entityProperties)
				found = _entityProperties.TryGetValue(typeFullName, out properties);
			if(!found)
			{
				foreach (var version in EntityTypeVersions)
				{
					if(Reflection.TypeName.TypeNameEqualityComparer.Instance.Equals(version.Type, entityTypeName))
					{
						properties = (System.Collections.IList)version.Properties;
						lock (_entityProperties)
							if(!_entityProperties.ContainsKey(entityTypeName.FullName))
								_entityProperties.Add(entityTypeName.FullName, properties);
						break;
					}
				}
			}
			if(properties != null)
				for(var i = 0; i < properties.Count; i++)
					yield return (IEntityPropertyInfo)properties[i];
		}

		/// <summary>
		/// Recupera as configurações associadas com o tipo da entidade.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade que será usado para filtrar as configurações.</param>
		/// <returns></returns>
		public IEnumerable<IPropertySettingsInfo> GetTypeSettings(Reflection.TypeName entityTypeName)
		{
			var typeFullName = entityTypeName.FullName;
			var entityTypeNamespace = string.Join(".", entityTypeName.Namespace);
			foreach (var version in EntityTypeVersions)
			{
				if(Reflection.TypeName.TypeNameEqualityComparer.Instance.Equals(version.Type, entityTypeName))
				{
					foreach (EntityTypeVersionProperty property in version.Properties)
						yield return new PropertySettingsInfoWrapper(version, property, uid => _inputRulesGroups.Where(f => f.Uid == uid).Select(f => (int?)f.InputRulesGroupId).FirstOrDefault());
					yield break;
				}
			}
		}

		/// <summary>
		/// Recupera o identificador da customização associada com o tipo da entidade.
		/// </summary>
		/// <param name="entityTypeName">Tipo da entidade na qual a customização está associada.</param>
		/// <returns></returns>
		public int? GetEntitySpecializationId(Reflection.TypeName entityTypeName)
		{
			foreach (var typeVersion in EntityTypeVersions)
			{
				if(Reflection.TypeName.TypeNameEqualityComparer.Instance.Equals(typeVersion.Type, entityTypeName))
				{
					return typeVersion.CustomizationId;
				}
			}
			return null;
		}

		/// <summary>
		/// Carrega os labels de uma propriedade.
		/// </summary>
		/// <param name="typeName">Tipo</param>
		/// <param name="uiContext">Contexto com o usuário</param>
		/// <returns></returns>
		public IEnumerable<PropertyLabelInfo> GetLabels(Reflection.TypeName typeName, string uiContext)
		{
			var typeNamespace = string.Join(".", typeName.Namespace);
			foreach (var version in EntityTypeVersions)
			{
				if(Reflection.TypeName.TypeNameEqualityComparer.Instance.Equals(version.Type, typeName))
				{
					for(var j = 0; j < version.Properties.Count; j++)
					{
						var property = version.Properties[j];
						for(var x = 0; x < property.DefaultLabels.Count; x++)
						{
							var defaultLabel = property.DefaultLabels[x];
							if(string.IsNullOrEmpty(defaultLabel.UIContext) || (uiContext != null && defaultLabel.UIContext == uiContext))
							{
								yield return new PropertyLabelInfo {
									Label = defaultLabel,
									PropertyName = property.Name
								};
							}
						}
					}
					yield break;
				}
			}
		}

		/// <summary>
		/// Recupera a regra de entrada associada com o grupo e o contexto UI informados.
		/// </summary>
		/// <param name="inputRulesGroupId">Identificador do grupo de regras de entrada.</param>
		/// <param name="uiContext">Nome do context de interface com o usuário.</param>
		/// <returns></returns>
		public InputRules GetRules(int inputRulesGroupId, string uiContext)
		{
			var inputRulesGroup = InputRulesGroups.FirstOrDefault(f => f.InputRulesGroupId == inputRulesGroupId);
			if(inputRulesGroup == null)
				return null;
			foreach (var inputRulesGroupEntry in inputRulesGroup.InputRules)
			{
				if(string.IsNullOrEmpty(inputRulesGroupEntry.UIContext) || (!string.IsNullOrEmpty(uiContext) && inputRulesGroupEntry.UIContext == uiContext))
				{
					foreach (var inputRule in _inputRules)
					{
						if(inputRule.Uid == inputRulesGroupEntry.InputRulesUid)
							return inputRule;
					}
					return null;
				}
			}
			return null;
		}

		/// <summary>
		/// Recupera a validação de entrada pelo identificador informado.
		/// </summary>
		/// <param name="inputRulesGroupId">Identificador do grupo de regras de entrada.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <returns></returns>
		public InputValidate GetInputValidate(int inputRulesGroupId, string uiContext)
		{
			var inputRules = GetRules(inputRulesGroupId, uiContext);
			if(inputRules != null && inputRules.InputValidateUid.HasValue)
				return GetInputValidate(inputRules.InputValidateUid.Value);
			return null;
		}

		/// <summary>
		/// Recupera a validação de entrada pelo identificador informado.
		/// </summary>
		/// <param name="inputValidateId">Identificador da validação de entrada.</param>
		/// <returns></returns>
		public InputValidate GetInputValidate(int inputValidateId)
		{
			return InputValidates.FirstOrDefault(f => f.InputValidateId == inputValidateId);
		}

		/// <summary>
		/// Recupera a validação de entrada pelo identificador informado.
		/// </summary>
		/// <param name="inputValidateUid">Identificador da validação de entrada.</param>
		/// <returns></returns>
		public InputValidate GetInputValidate(Guid inputValidateUid)
		{
			return InputValidates.FirstOrDefault(f => f.Uid == inputValidateUid);
		}

		/// <summary>
		/// Recupera a validação associada com o identificador informado.
		/// </summary>
		/// <param name="validationId"></param>
		/// <returns></returns>
		public Validation GetValidation(int validationId)
		{
			return Validations.FirstOrDefault(f => f.ValidationId == validationId);
		}

		/// <summary>
		/// Recupera os dados do tipo de validação.
		/// </summary>
		/// <param name="validationTypeId">Identificador do tipo de validação.</param>
		/// <returns></returns>
		public ValidationType GetValidationType(int validationTypeId)
		{
			return ValidationTypes.FirstOrDefault(f => f.ValidationTypeId == validationTypeId);
		}

		/// <summary>
		/// Recupera os dados do tipo de validação.
		/// </summary>
		/// <param name="validationTypeUid">Identificador do tipo de validação.</param>
		/// <returns></returns>
		public ValidationType GetValidationType(Guid validationTypeUid)
		{
			return ValidationTypes.FirstOrDefault(f => f.Uid == validationTypeUid);
		}

		/// <summary>
		/// Abre a configuração que está contida na stream informada.
		/// </summary>
		/// <param name="inputStreams"></param>
		/// <returns></returns>
		public static ValidationSettings Open(IEnumerable<System.IO.Stream> inputStreams)
		{
			inputStreams.Require("inputStreams").NotNull();
			var settings = new ValidationSettings();
			foreach (var stream in inputStreams)
			{
				var reader = System.Xml.XmlReader.Create(stream, new System.Xml.XmlReaderSettings {
					IgnoreWhitespace = true,
					ConformanceLevel = System.Xml.ConformanceLevel.Auto
				});
				if(reader.Read())
					((System.Xml.Serialization.IXmlSerializable)settings).ReadXml(reader);
			}
			return settings;
		}

		/// <summary>
		/// Abre a configuração que está contida na stream informada.
		/// </summary>
		/// <param name="inputStream"></param>
		/// <returns></returns>
		public static ValidationSettings Open(System.IO.Stream inputStream)
		{
			inputStream.Require("inputStream").NotNull();
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ValidationSettings));
			return (ValidationSettings)serializer.Deserialize(inputStream);
		}

		/// <summary>
		/// Salva os dados da configuração.
		/// </summary>
		/// <param name="outputStream"></param>
		public void Save(System.IO.Stream outputStream)
		{
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ValidationSettings));
			serializer.Serialize(outputStream, this);
		}

		/// <summary>
		/// Recupera o esquema do tipo.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê os dados serializados.
		/// </summary>
		/// <param name="reader"></param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.IsEmptyElement)
					{
						reader.Skip();
						continue;
					}
					if(reader.LocalName == "EntityTypes")
					{
						reader.ReadStartElement();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if(reader.LocalName == "EntityType")
							{
								var type = new EntityType();
								((System.Xml.Serialization.IXmlSerializable)type).ReadXml(reader);
								EntityTypes.Add(type);
							}
							else
								reader.Skip();
						}
						reader.ReadEndElement();
					}
					else if(reader.LocalName == "PropertySettings")
					{
						reader.ReadStartElement();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if(reader.LocalName == "PropertySetting")
							{
								var settings = new PropertySettings();
								((System.Xml.Serialization.IXmlSerializable)settings).ReadXml(reader);
								PropertySettings.Add(settings);
							}
							else
								reader.Skip();
						}
						reader.ReadEndElement();
					}
					else if(reader.LocalName == "ValidationTypes")
					{
						reader.ReadStartElement();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if(reader.LocalName == "ValidationType")
							{
								var validationType = new ValidationType();
								((System.Xml.Serialization.IXmlSerializable)validationType).ReadXml(reader);
								ValidationTypes.Add(validationType);
							}
							else
								reader.Skip();
						}
						reader.ReadEndElement();
					}
					else if(reader.LocalName == "Validations")
					{
						reader.ReadStartElement();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if(reader.LocalName == "Validation")
							{
								var validation = new Validation();
								((System.Xml.Serialization.IXmlSerializable)validation).ReadXml(reader);
								Validations.Add(validation);
							}
							else
								reader.Skip();
						}
						reader.ReadEndElement();
					}
					else if(reader.LocalName == "InputRules")
					{
						reader.ReadStartElement();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if(reader.LocalName == "InputRules")
							{
								var inputRules = new InputRules();
								((System.Xml.Serialization.IXmlSerializable)inputRules).ReadXml(reader);
								InputRules.Add(inputRules);
							}
							else
								reader.Skip();
						}
						reader.ReadEndElement();
					}
					else if(reader.LocalName == "InputRulesGroups")
					{
						reader.ReadStartElement();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if(reader.LocalName == "InputRulesGroup")
							{
								var inputRulesGroup = new InputRulesGroup();
								((System.Xml.Serialization.IXmlSerializable)inputRulesGroup).ReadXml(reader);
								InputRulesGroups.Add(inputRulesGroup);
							}
							else
								reader.Skip();
						}
						reader.ReadEndElement();
					}
					else if(reader.LocalName == "InputValidates")
					{
						reader.ReadStartElement();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if(reader.LocalName == "InputValidate")
							{
								var validate = new InputValidate();
								((System.Xml.Serialization.IXmlSerializable)validate).ReadXml(reader);
								InputValidates.Add(validate);
							}
							else
								reader.Skip();
						}
						reader.ReadEndElement();
					}
					else
						reader.Skip();
				}
				reader.ReadEndElement();
			}
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("EntityTypes");
			foreach (System.Xml.Serialization.IXmlSerializable type in EntityTypes)
			{
				writer.WriteStartElement("EntityType");
				type.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("PropertySettings");
			foreach (System.Xml.Serialization.IXmlSerializable propertySetting in PropertySettings)
			{
				writer.WriteStartElement("PropertySetting");
				propertySetting.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("ValidationTypes");
			foreach (System.Xml.Serialization.IXmlSerializable i in ValidationTypes)
			{
				writer.WriteStartElement("ValidationType");
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("Validations");
			foreach (System.Xml.Serialization.IXmlSerializable i in Validations)
			{
				writer.WriteStartElement("Validation");
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("InputRules");
			foreach (System.Xml.Serialization.IXmlSerializable i in InputRules)
			{
				writer.WriteStartElement("InputRules");
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("InputRulesGroups");
			foreach (System.Xml.Serialization.IXmlSerializable i in InputRulesGroups)
			{
				writer.WriteStartElement("InputRulesGroup");
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("InputValidates");
			foreach (System.Xml.Serialization.IXmlSerializable i in InputValidates)
			{
				writer.WriteStartElement("InputValidate");
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		class PropertySettingsInfoWrapper : IPropertySettingsInfo
		{
			private EntityTypeVersion _version;

			private EntityTypeVersionProperty _property;

			private Func<Guid, int?> _inputRulesGroupIdGetter;

			public string Identifier
			{
				get
				{
					return string.Format("{0},{1}.{2}", _version.Assembly, _version.Type.FullName, _property.Name);
				}
			}

			public int? ValidationId
			{
				get
				{
					return null;
				}
			}

			public int? InputRulesGroupId
			{
				get
				{
					return _property.InputRulesGroupUid.HasValue ? _inputRulesGroupIdGetter(_property.InputRulesGroupUid.Value) : null;
				}
			}

			public bool ReloadSettings
			{
				get
				{
					return false;
				}
			}

			public bool CopyValue
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="version"></param>
			/// <param name="property"></param>
			/// <param name="inputRulesGroupIdGetter"></param>
			public PropertySettingsInfoWrapper(EntityTypeVersion version, EntityTypeVersionProperty property, Func<Guid, int?> inputRulesGroupIdGetter)
			{
				_version = version;
				_property = property;
				_inputRulesGroupIdGetter = inputRulesGroupIdGetter;
			}
		}
	}
}
