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
	/// Binder para recupera os dados da conexão.
	/// </summary>
	public abstract class CustomModelBinder : DefaultModelBinder
	{
		/// <summary>
		/// Cria um nome para a propriedade da model.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		protected static string CreatePropertyModelName(string prefix, string propertyName)
		{
			if(string.IsNullOrEmpty(prefix))
				return propertyName ?? String.Empty;
			else if(string.IsNullOrEmpty(propertyName))
				return prefix ?? string.Empty;
			else
				return prefix + "." + propertyName;
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
		protected object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.ICustomTypeDescriptor descriptor, string propertyName)
		{
			var propertyMetadata = bindingContext.PropertyMetadata[propertyName];
			var property = descriptor.GetProperties()[propertyName];
			var propertyBinder = this.Binders.GetBinder(property.PropertyType);
			var propertyBindingContext = new System.Web.Mvc.ModelBindingContext(bindingContext) {
				ModelMetadata = propertyMetadata,
				ModelName = CreatePropertyModelName(bindingContext.ModelName, propertyMetadata.PropertyName)
			};
			return propertyBinder.BindModel(controllerContext, propertyBindingContext);
		}
	}
}
