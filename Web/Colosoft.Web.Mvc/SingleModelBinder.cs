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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// SingleModelBinder
	/// </summary>
	public class SingleModelBinder : IModelBinder
	{
		private CultureInfo _culture;

		/// <summary>
		/// Cultura associada que será usada.
		/// </summary>
		public CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = value;
			}
		}

		/// <summary>
		/// Vincula o valor.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <returns></returns>
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			var modelState = new ModelState {
				Value = valueResult
			};
			if(valueResult == null)
				return null;
			float? result = null;
			if((bindingContext.ModelType == typeof(float) || bindingContext.ModelType == typeof(float?)))
			{
				var result2 = 0f;
				if(!string.IsNullOrEmpty(valueResult.AttemptedValue) && float.TryParse(valueResult.AttemptedValue, NumberStyles.AllowDecimalPoint, Culture, out result2))
					result = result2;
			}
			bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
			return bindingContext.ModelType == typeof(float) ? result.GetValueOrDefault() : result;
		}
	}
}
