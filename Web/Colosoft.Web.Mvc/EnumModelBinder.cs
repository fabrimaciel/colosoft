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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Implementação do provedor de ModelBinder para enums.
	/// </summary>
	public class EnumModelBinderProvider : IModelBinderProvider
	{
		private readonly EnumModelBinder _enumModelBinder;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public EnumModelBinderProvider()
		{
			_enumModelBinder = new EnumModelBinder();
		}

		/// <summary>
		/// Recupera o ModelBinder do tipo informado.
		/// </summary>
		/// <param name="modelType"></param>
		/// <returns></returns>
		public IModelBinder GetBinder(Type modelType)
		{
			if(modelType.IsEnum)
				return _enumModelBinder;
			return null;
		}
	}
	/// <summary>
	/// Implementação do binder para dar suporte a enums.
	/// </summary>
	class EnumModelBinder : System.Web.Mvc.IModelBinder
	{
		/// <summary>
		/// Recupera o valor da vinculação.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <returns></returns>
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			ModelState modelState = new ModelState {
				Value = valueResult
			};
			object actualValue = null;
			try
			{
				return Enum.ToObject(Type.GetType(bindingContext.ModelType.AssemblyQualifiedName), Convert.ToInt32(valueResult.AttemptedValue));
			}
			catch(FormatException e)
			{
				modelState.Errors.Add(e);
			}
			bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
			return actualValue;
		}
	}
}
