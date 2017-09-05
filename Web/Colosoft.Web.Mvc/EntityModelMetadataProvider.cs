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
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Implementação do provedor de metadados para modelos de entidade.
	/// </summary>
	public class EntityModelMetadataProvider : DataAnnotationsModelMetadataProvider
	{
		/// <summary>
		/// Chave os nomes das propriedades de coleção.
		/// </summary>
		public const string ClearPropertyNamesKey = "ENTITY.CLEAR_PROPERTY_NAMES";

		class MetadataStateble : Colosoft.Validation.IStateble
		{
			private Validation.StatebleParameterCollection _parameters = new Validation.StatebleParameterCollection();

			/// <summary>
			/// Evento acionado quando uma propriedade for alterada.
			/// </summary>
			public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

			/// <summary>
			/// Evento acionado quando o estado for alterado.
			/// </summary>
			public event Validation.StateChangedEventHandler StateChanged;

			/// <summary>
			/// Tipo.
			/// </summary>
			public Reflection.TypeName Type
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			/// <summary>
			/// Identifica se a instancia é somente leitura.
			/// </summary>
			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Parametros.
			/// </summary>
			public Validation.StatebleParameterCollection Parameters
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			/// <summary>
			/// Recupera o item pelo nome da propriedade.
			/// </summary>
			/// <param name="propertyName"></param>
			/// <returns></returns>
			public Validation.IStatebleItem this[string propertyName]
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			/// <summary>
			/// Limpa o cache do estado.
			/// </summary>
			public void ClearStateCache()
			{
			}

			/// <summary>
			/// Notifica que a propriedade foi alteada.
			/// </summary>
			/// <param name="propertyName"></param>
			public void NotifyStatePropertyChanged(string propertyName)
			{
				if(StateChanged != null)
					StateChanged(this, new Validation.StateChangedEventArgs(propertyName, ""));
			}

			/// <summary>
			/// Recarrega as configurações do tipo.
			/// </summary>
			public void ReloadTypeSettings()
			{
			}

			/// <summary>
			/// Notifica que a propriedade foi alterada.
			/// </summary>
			/// <param name="propertyName"></param>
			protected void RaisePropertyChange(string propertyName)
			{
				if(PropertyChanged != null)
					PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}

			/// <summary>
			/// Recupera o enumerador dos itens.
			/// </summary>
			/// <returns></returns>
			public IEnumerator<Validation.IStatebleItem> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
			}
		}

		/// <summary>
		/// Recupera os metadados para a propriedade.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="modelAccessor"></param>
		/// <param name="containerType"></param>
		/// <param name="propertyDescriptor"></param>
		/// <param name="propertySettings"></param>
		/// <returns></returns>
		private ModelMetadata GetMetadataForProperty(MetadataStateble owner, Func<object> modelAccessor, Type containerType, System.ComponentModel.PropertyDescriptor propertyDescriptor, Colosoft.Validation.IPropertySettingsInfo propertySettings)
		{
			IEnumerable<Attribute> attributes = this.FilterAttributes(containerType, propertyDescriptor, propertyDescriptor.Attributes.Cast<Attribute>());
			ModelMetadata result = CreateMetadata(owner, attributes, containerType, modelAccessor, propertyDescriptor.PropertyType, propertyDescriptor.Name, propertySettings);
			return result;
		}

		/// <summary>
		/// Cria o metadado para o tipo informado.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="attributes">Atributos da propriedade.</param>
		/// <param name="containerType">Tipo do container.</param>
		/// <param name="modelAccessor"></param>
		/// <param name="modelType">Tipo da model.</param>
		/// <param name="propertyName">Nome da propriedade.</param>
		/// <param name="propertySettings"></param>
		/// <returns></returns>
		private ModelMetadata CreateMetadata(MetadataStateble owner, IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName, Colosoft.Validation.IPropertySettingsInfo propertySettings)
		{
			var validationManager = Colosoft.Validation.ValidationManager.Instance;
			var stateItem = validationManager.CreatePropertyState(owner, propertyName, propertySettings, null, System.Globalization.CultureInfo.CurrentUICulture);
			var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
			metadata.AdditionalValues.Add("StateItem", stateItem);
			metadata.IsRequired = stateItem.IsRequired;
			metadata.IsReadOnly = stateItem.IsReadOnly;
			metadata.DisplayName = stateItem.Label != null ? stateItem.Label.Title.FormatOrNull() : null;
			metadata.Description = stateItem.Label != null ? stateItem.Label.Description.FormatOrNull() : null;
			return metadata;
		}

		/// <summary>
		/// Recupera os metadados das propriedades da entidade.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="containerType"></param>
		/// <returns></returns>
		protected virtual IEnumerable<ModelMetadata> GetMetadataForEntityProperties(object container, Type containerType)
		{
			var descriptor = GetTypeDescriptor(containerType);
			var properties = new List<ModelMetadata>();
			var owner = new MetadataStateble();
			var validationManager = Colosoft.Validation.ValidationManager.Instance;
			var settings = validationManager.LoadSettings(Colosoft.Reflection.TypeName.Get(containerType));
			foreach (System.ComponentModel.PropertyDescriptor property in descriptor.GetProperties())
			{
				if(property.Name == "RowVersion")
				{
				}
				else if(property.ComponentType != containerType && (property.ComponentType == typeof(Colosoft.Business.Entity) || property.ComponentType.BaseType == typeof(Colosoft.Business.Entity)))
				{
					continue;
				}
				var current = property;
				var modelAccessor = (container == null) ? null : new Func<object>(() => current.GetValue(container));
				properties.Add(GetMetadataForProperty(owner, modelAccessor, containerType, current, settings.FirstOrDefault(f => f.Identifier == current.Name)));
			}
			return properties;
		}

		/// <summary>
		/// Recupera as propriedades para o container informado.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="containerType"></param>
		/// <returns></returns>
		public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
		{
			if(typeof(Colosoft.Business.Entity).IsAssignableFrom(containerType))
				return GetMetadataForEntityProperties(containerType, containerType);
			return base.GetMetadataForProperties(container, containerType);
		}

		/// <summary>
		/// Verifica se o tipo implementa o tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="assignableType"></param>
		/// <returns></returns>
		private static Boolean IsAssignableTo(Type type, Type assignableType)
		{
			return assignableType.IsAssignableFrom(type);
		}

		/// <summary>
		/// Verifica se o tipo implementa o tipo informado.
		/// </summary>
		/// <typeparam name="TAssignable"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		private static Boolean IsAssignableTo<TAssignable>(Type type)
		{
			return IsAssignableTo(type, typeof(TAssignable));
		}
	}
}
