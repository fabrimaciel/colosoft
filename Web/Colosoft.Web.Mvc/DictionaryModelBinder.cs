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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Implementação do Binder para dicionários.
	/// </summary>
	public class DictionaryModelBinder : System.Web.Mvc.DefaultModelBinder
	{
		/// <summary>
		/// Verifica se o tipo é um dicionário genérico.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool IsGenericDictionary(Type type)
		{
			return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<, >));
		}

		/// <summary>
		/// Adiciona os itens para o dicionário.
		/// </summary>
		/// <param name="dictionary"></param>
		/// <param name="dictionaryType">Tipo do dicionário.</param>
		/// <param name="modelName"></param>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		private void AddItemsToDictionary(IDictionary dictionary, Type dictionaryType, string modelName, ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			List<string> keys = new List<string>();
			var request = controllerContext.HttpContext.Request;
			keys.AddRange(((IDictionary<string, object>)controllerContext.RouteData.Values).Keys.Cast<string>());
			keys.AddRange(request.QueryString.Keys.Cast<string>());
			keys.AddRange(request.Form.Keys.Cast<string>());
			Type dictionaryValueType = dictionaryType.GetGenericArguments()[1];
			IModelBinder dictionaryValueBinder = Binders.GetBinder(dictionaryValueType);
			foreach (string key in keys)
			{
				string dictItemKey = null;
				string valueModelName = null;
				if(!key.Equals("area", StringComparison.InvariantCultureIgnoreCase) && !key.Equals("controller", StringComparison.InvariantCultureIgnoreCase) && !key.Equals("action", StringComparison.InvariantCultureIgnoreCase))
				{
					if(key.StartsWith(modelName + "[", StringComparison.InvariantCultureIgnoreCase))
					{
						int endIndex = key.IndexOf("]", modelName.Length + 1);
						if(endIndex != -1)
						{
							dictItemKey = key.Substring(modelName.Length + 1, endIndex - modelName.Length - 1);
							valueModelName = key.Substring(0, endIndex + 1);
						}
					}
					else
					{
						dictItemKey = valueModelName = key;
					}
					if(dictItemKey != null && valueModelName != null && !dictionary.Contains(dictItemKey))
					{
						object dictItemValue = dictionaryValueBinder.BindModel(controllerContext, new ModelBindingContext(bindingContext) {
							ModelName = valueModelName,
							ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, dictionaryValueType)
						});
						if(dictItemValue != null)
						{
							dictionary.Add(dictItemKey, dictItemValue);
						}
					}
				}
			}
		}

		/// <summary>
		/// Vincula o modelo de dados.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <returns></returns>
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			object result = null;
			Type modelType = bindingContext.ModelType;
			string modelName = bindingContext.ModelName;
			if(IsGenericDictionary(modelType))
			{
				IDictionary dictionary = (IDictionary)CreateModel(controllerContext, bindingContext, modelType);
				AddItemsToDictionary(dictionary, modelType, modelName, controllerContext, bindingContext);
				result = dictionary;
			}
			else
			{
				result = base.BindModel(controllerContext, bindingContext);
				var properties = modelType.GetProperties();
				foreach (var property in properties)
				{
					Type propertyType = property.PropertyType;
					if(IsGenericDictionary(propertyType))
					{
						var dictionary = (IDictionary)Activator.CreateInstance(propertyType);
						AddItemsToDictionary(dictionary, propertyType, modelName, controllerContext, bindingContext);
						property.SetValue(result, dictionary, null);
					}
				}
			}
			return result;
		}
	}
}
