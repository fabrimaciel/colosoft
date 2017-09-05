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
	/// Implementação base do vinculador para entidade.
	/// </summary>
	abstract class EntityModelBinderBase : DefaultModelBinder
	{
		private Colosoft.Business.IEntityTypeManager _entityTypeManager;

		/// <summary>
		/// Gerenciador de tipos das entidades.
		/// </summary>
		public Colosoft.Business.IEntityTypeManager EntityTypeManager
		{
			get
			{
				return _entityTypeManager;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entityTypeManager"></param>
		public EntityModelBinderBase(Colosoft.Business.IEntityTypeManager entityTypeManager)
		{
			entityTypeManager.Require("entityTypeManager").NotNull();
			_entityTypeManager = entityTypeManager;
		}

		/// <summary>
		/// Cria um nome para a propriedade da model.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		private static string CreatePropertyModelName(string prefix, string propertyName)
		{
			if(string.IsNullOrEmpty(prefix))
				return propertyName ?? String.Empty;
			else if(string.IsNullOrEmpty(propertyName))
				return prefix ?? string.Empty;
			else
				return prefix + "." + propertyName;
		}

		/// <summary>
		/// Vincula o valor da propriedade.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <param name="propertyDescriptor"></param>
		protected override void BindProperty(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor)
		{
			string prefix = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
			var clearProperties = controllerContext.Controller.ViewData.ContainsKey(EntityModelMetadataProvider.ClearPropertyNamesKey) ? controllerContext.Controller.ViewData[EntityModelMetadataProvider.ClearPropertyNamesKey] as IEnumerable<string> : null;
			var propertyPath = prefix;
			if(clearProperties != null)
			{
				var parts = prefix.Split('.');
				if(parts.Length > 1 && parts[parts.Length - 2].EndsWith("]"))
				{
					var index = 0;
					propertyPath = string.Join(".", parts.Skip(1).Select(f => (index = f.IndexOf('[')) >= 0 ? f.Substring(0, index) : f));
				}
				else if(parts.Length > 1)
					propertyPath = string.Join(".", parts.Skip(1));
			}
			if(!bindingContext.ValueProvider.ContainsPrefix(prefix) && clearProperties != null && clearProperties.Contains(propertyPath))
			{
				var propertyValue = propertyDescriptor.GetValue(bindingContext.Model) as System.Collections.IList;
				if(propertyValue != null)
					propertyValue.Clear();
				else if(!propertyDescriptor.IsReadOnly)
					propertyDescriptor.SetValue(bindingContext.Model, null);
			}
			base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
		}

		/// <summary>
		/// Recupera os valores das chaves associadas com a entidade.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <param name="entityType"></param>
		/// <returns></returns>
		public Colosoft.Query.RecordKey GetRecordKey(ControllerContext controllerContext, ModelBindingContext bindingContext, Type entityType)
		{
			var descriptor = GetTypeDescriptor(controllerContext, bindingContext);
			var loader = _entityTypeManager.GetLoader(entityType);
			return loader.GetRecordKey(propertyName => GetPropertyValue(controllerContext, bindingContext, descriptor, loader, propertyName));
		}

		/// <summary>
		/// Recupera os valores das chaves associadas com a entidade.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <param name="entityType"></param>
		/// <returns></returns>
		public Colosoft.Query.IRecord GetRecordOfKey(ControllerContext controllerContext, ModelBindingContext bindingContext, Type entityType)
		{
			var descriptor = GetTypeDescriptor(controllerContext, bindingContext);
			var loader = _entityTypeManager.GetLoader(entityType);
			return loader.GetRecordOfKey(propertyName => GetPropertyValue(controllerContext, bindingContext, descriptor, loader, propertyName));
		}

		/// <summary>
		/// Recupera o valor da propriedade.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <param name="descriptor"></param>
		/// <param name="loader"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		private object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.ICustomTypeDescriptor descriptor, Colosoft.Business.IEntityLoader loader, string propertyName)
		{
			if(propertyName == "Uid" && !string.IsNullOrEmpty(loader.UidPropertyName))
				propertyName = loader.UidPropertyName;
			var propertyMetadata = bindingContext.PropertyMetadata[propertyName];
			var property = descriptor.GetProperties()[propertyName];
			var propertyBinder = this.Binders.GetBinder(property.PropertyType);
			var propertyBindingContext = new System.Web.Mvc.ModelBindingContext(bindingContext) {
				ModelMetadata = propertyMetadata,
				ModelName = CreatePropertyModelName(bindingContext.ModelName, propertyMetadata.PropertyName)
			};
			var value = propertyBinder.BindModel(controllerContext, propertyBindingContext);
			if(value == null && propertyName == "Uid")
				value = GetPropertyValue(controllerContext, bindingContext, descriptor, loader, loader.UidPropertyName);
			return value;
		}
	}
}
