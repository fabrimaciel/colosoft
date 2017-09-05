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

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Vinculador usado para coleções de entidades.
	/// </summary>
	class EntityCollectionModelBinder : System.Web.Mvc.DefaultModelBinder
	{
		/// <summary>
		/// Verifica se pode realizar a validação na requisição.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <returns></returns>
		private static bool ShouldPerformRequestValidation(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
		{
			return ((((controllerContext == null) || (controllerContext.Controller == null)) || ((bindingContext == null) || (bindingContext.ModelMetadata == null))) || (controllerContext.Controller.ValidateRequest && bindingContext.ModelMetadata.RequestValidationEnabled));
		}

		/// <summary>
		/// Realiza a vinculação com a model complexa
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <returns></returns>
		private object BindComplexModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
		{
			Func<object> func2 = null;
			object model = bindingContext.ModelMetadata.Model;
			Type modelType = bindingContext.ModelType;
			if((model == null) && modelType.IsArray)
			{
				var elementType = modelType.GetElementType();
				var type3 = typeof(List<>).MakeGenericType(new Type[] {
					elementType
				});
				object collection = this.CreateModel(controllerContext, bindingContext, type3);
				var context = new System.Web.Mvc.ModelBindingContext {
					ModelMetadata = System.Web.Mvc.ModelMetadataProviders.Current.GetMetadataForType(() => collection, type3),
					ModelName = bindingContext.ModelName,
					ModelState = bindingContext.ModelState,
					PropertyFilter = bindingContext.PropertyFilter,
					ValueProvider = bindingContext.ValueProvider
				};
				if(bindingContext.ModelMetadata.AdditionalValues.ContainsKey(EntityModelMetadataProvider.ClearPropertyNamesKey))
					context.ModelMetadata.AdditionalValues.Add(EntityModelMetadataProvider.ClearPropertyNamesKey, bindingContext.ModelMetadata.AdditionalValues[EntityModelMetadataProvider.ClearPropertyNamesKey]);
				var list = (System.Collections.IList)this.UpdateCollection(controllerContext, context, elementType);
				if(list == null)
					return null;
				var array = Array.CreateInstance(elementType, list.Count);
				list.CopyTo(array, 0);
				return array;
			}
			if(model == null)
				model = this.CreateModel(controllerContext, bindingContext, modelType);
			var type7 = TypeHelpers.ExtractGenericInterface(modelType, typeof(IEnumerable<>));
			if(type7 != null)
			{
				Type type8 = type7.GetGenericArguments()[0];
				if(typeof(ICollection<>).MakeGenericType(new Type[] {
					type8
				}).IsInstanceOfType(model))
				{
					var context6 = new System.Web.Mvc.ModelBindingContext();
					if(func2 == null)
						func2 = () => model;
					context6.ModelMetadata = System.Web.Mvc.ModelMetadataProviders.Current.GetMetadataForType(func2, modelType);
					context6.ModelName = bindingContext.ModelName;
					context6.ModelState = bindingContext.ModelState;
					context6.PropertyFilter = bindingContext.PropertyFilter;
					context6.ValueProvider = bindingContext.ValueProvider;
					if(bindingContext.ModelMetadata.AdditionalValues.ContainsKey(EntityModelMetadataProvider.ClearPropertyNamesKey))
						context6.ModelMetadata.AdditionalValues.Add(EntityModelMetadataProvider.ClearPropertyNamesKey, bindingContext.ModelMetadata.AdditionalValues[EntityModelMetadataProvider.ClearPropertyNamesKey]);
					var context5 = context6;
					return this.UpdateCollection(controllerContext, context5, type8);
				}
			}
			this.BindComplexElementalModel(controllerContext, bindingContext, model);
			return model;
		}

		/// <summary>
		/// Cria um contexto de vinculação para um elemento complexo do model.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		private System.Web.Mvc.ModelBindingContext CreateComplexElementalModelBindingContext(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext, object model)
		{
			var bindAttr = (System.Web.Mvc.BindAttribute)this.GetTypeDescriptor(controllerContext, bindingContext).GetAttributes()[typeof(System.Web.Mvc.BindAttribute)];
			Predicate<string> predicate = (bindAttr != null) ? propertyName => (bindAttr.IsPropertyAllowed(propertyName) && bindingContext.PropertyFilter(propertyName)) : bindingContext.PropertyFilter;
			return new System.Web.Mvc.ModelBindingContext {
				ModelMetadata = System.Web.Mvc.ModelMetadataProviders.Current.GetMetadataForType(() => model, bindingContext.ModelType),
				ModelName = bindingContext.ModelName,
				ModelState = bindingContext.ModelState,
				PropertyFilter = predicate,
				ValueProvider = bindingContext.ValueProvider
			};
		}

		/// <summary>
		/// Vincula um elemento complexo para a model.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <param name="model"></param>
		private void BindComplexElementalModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext, object model)
		{
			var context = this.CreateComplexElementalModelBindingContext(controllerContext, bindingContext, model);
			if(this.OnModelUpdating(controllerContext, context))
			{
				this.BindProperties(controllerContext, context);
				this.OnModelUpdated(controllerContext, context);
			}
		}

		/// <summary>
		/// Vincula as propriedades.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		private void BindProperties(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
		{
			var modelProperties = this.GetModelProperties(controllerContext, bindingContext);
			Predicate<string> propertyFilter = bindingContext.PropertyFilter;
			for(int i = 0; i < modelProperties.Count; i++)
			{
				var property = modelProperties[i];
				if(ShouldUpdateProperty(property, propertyFilter))
					this.BindProperty(controllerContext, bindingContext, property);
			}
		}

		/// <summary>
		/// Verifica se pode atualizar a propriedade.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="propertyFilter"></param>
		/// <returns></returns>
		private static bool ShouldUpdateProperty(System.ComponentModel.PropertyDescriptor property, Predicate<string> propertyFilter)
		{
			if(property.IsReadOnly && !CanUpdateReadonlyTypedReference(property.PropertyType))
				return false;
			if(!propertyFilter(property.Name))
				return false;
			return true;
		}

		/// <summary>
		/// Verifica se pode atualizar a referencia do tipo somente leitura.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool CanUpdateReadonlyTypedReference(Type type)
		{
			if(type.IsValueType)
				return false;
			if(type.IsArray)
				return false;
			if(type == typeof(string))
				return false;
			return true;
		}

		/// <summary>
		/// Recupera o enumerable de indices baseados em zero.
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<string> GetZeroBasedIndexes()
		{
			int i = 0;
			while (true)
			{
				yield return i.ToString(System.Globalization.CultureInfo.InvariantCulture);
				i++;
			}
		}

		/// <summary>
		/// Recupera os indices
		/// </summary>
		/// <param name="bindingContext"></param>
		/// <param name="stopOnIndexNotFound"></param>
		/// <param name="indexes"></param>
		private static void GetIndexes(System.Web.Mvc.ModelBindingContext bindingContext, out bool stopOnIndexNotFound, out IEnumerable<string> indexes)
		{
			string key = CreateSubPropertyName(bindingContext.ModelName, "index");
			var result = bindingContext.ValueProvider.GetValue(key);
			if(result != null)
			{
				string[] strArray = result.ConvertTo(typeof(string[])) as string[];
				if(strArray != null)
				{
					stopOnIndexNotFound = false;
					indexes = strArray;
					return;
				}
			}
			stopOnIndexNotFound = true;
			indexes = GetZeroBasedIndexes();
		}

		/// <summary>
		/// Recupera o descrição do recurso que define que um valor é requirido.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <returns></returns>
		private static string GetValueRequiredResource(System.Web.Mvc.ControllerContext controllerContext)
		{
			return "PropertyValueRequired";
		}

		/// <summary>
		/// Adiciona uma mensagem de valor requerido para o estado da model.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="modelState"></param>
		/// <param name="modelStateKey"></param>
		/// <param name="elementType"></param>
		/// <param name="value"></param>
		private static void AddValueRequiredMessageToModelState(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelStateDictionary modelState, string modelStateKey, Type elementType, object value)
		{
			if(((value == null) && !TypeHelpers.TypeAllowsNullValue(elementType)) && modelState.IsValidField(modelStateKey))
				modelState.AddModelError(modelStateKey, GetValueRequiredResource(controllerContext));
		}

		/// <summary>
		/// Realiza a vinculação dos dados com o modelo.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <returns></returns>
		public override object BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
		{
			System.Runtime.CompilerServices.RuntimeHelpers.EnsureSufficientExecutionStack();
			if(bindingContext == null)
				throw new ArgumentNullException("bindingContext");
			if(!string.IsNullOrEmpty(bindingContext.ModelName) && !bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName))
			{
				if(!bindingContext.FallbackToEmptyPrefix)
					return null;
				var context = new System.Web.Mvc.ModelBindingContext {
					ModelMetadata = bindingContext.ModelMetadata,
					ModelState = bindingContext.ModelState,
					PropertyFilter = bindingContext.PropertyFilter,
					ValueProvider = bindingContext.ValueProvider
				};
				if(bindingContext.ModelMetadata.AdditionalValues.ContainsKey(EntityModelMetadataProvider.ClearPropertyNamesKey))
					context.ModelMetadata.AdditionalValues.Add(EntityModelMetadataProvider.ClearPropertyNamesKey, bindingContext.ModelMetadata.AdditionalValues[EntityModelMetadataProvider.ClearPropertyNamesKey]);
				bindingContext = context;
			}
			if(!bindingContext.ModelMetadata.IsComplexType)
				return null;
			return this.BindComplexModel(controllerContext, bindingContext);
		}

		/// <summary>
		/// Atualiza a coleção.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <param name="elementType">Tipo do elemento da coleção.</param>
		/// <returns></returns>
		public object UpdateCollection(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext, Type elementType)
		{
			bool flag;
			IEnumerable<string> enumerable;
			GetIndexes(bindingContext, out flag, out enumerable);
			var binder = this.Binders.GetBinder(elementType);
			var newContents = new List<Colosoft.Business.IEntity>();
			var entityList = (System.Collections.IList)bindingContext.Model;
			foreach (string str in enumerable)
			{
				string prefix = CreateSubIndexName(bindingContext.ModelName, str);
				if(!bindingContext.ValueProvider.ContainsPrefix(prefix))
				{
					if(!flag)
					{
						continue;
					}
					break;
				}
				var context = new System.Web.Mvc.ModelBindingContext {
					ModelMetadata = System.Web.Mvc.ModelMetadataProviders.Current.GetMetadataForType(null, elementType),
					ModelName = prefix,
					ModelState = bindingContext.ModelState,
					PropertyFilter = bindingContext.PropertyFilter,
					ValueProvider = bindingContext.ValueProvider
				};
				if(bindingContext.ModelMetadata.AdditionalValues.ContainsKey(EntityModelMetadataProvider.ClearPropertyNamesKey))
					context.ModelMetadata.AdditionalValues.Add(EntityModelMetadataProvider.ClearPropertyNamesKey, bindingContext.ModelMetadata.AdditionalValues[EntityModelMetadataProvider.ClearPropertyNamesKey]);
				var entityFound = false;
				if(binder is EntityModelBinderBase)
				{
					var entityBinder = (EntityModelBinderBase)binder;
					var recordKey = entityBinder.GetRecordKey(controllerContext, context, elementType);
					foreach (Colosoft.Business.IEntity i in entityList)
						if(i.Equals(recordKey, Query.RecordKeyComparisonType.Key))
						{
							entityFound = true;
							context.ModelMetadata.Model = i;
							break;
						}
				}
				var entity = (Colosoft.Business.IEntity)binder.BindModel(controllerContext, context);
				AddValueRequiredMessageToModelState(controllerContext, bindingContext.ModelState, prefix, elementType, entity);
				if(!entityFound)
					entityList.Add(entity);
				newContents.Add(entity);
			}
			for(var i = 0; i < entityList.Count; i++)
			{
				if(!newContents.Contains(entityList[i]))
				{
					var count = entityList.Count;
					entityList.RemoveAt(i);
					if(entityList.Count < count)
						i--;
				}
			}
			return entityList;
		}
	}
}
