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
	/// TimeSpanModelBinder.
	/// </summary>
	public class TimeSpanModelBinder : DefaultModelBinder
	{
		/// <summary>
		/// Vincula o valor.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <returns></returns>
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			ValueProviderResult valueResultDays = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Days");
			ValueProviderResult valueResultHours = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Hours");
			ValueProviderResult valueResultMinutes = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Minutes");
			ValueProviderResult valueResultSeconds = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Seconds");
			if(valueResultHours != null || valueResultMinutes != null | valueResultSeconds != null)
			{
				int days = 0;
				int hours = 0;
				int minutes = 0;
				int seconds = 0;
				TimeSpan? actualValue = null;
				var valueResult = new ValueProviderResult(null, string.Empty, System.Globalization.CultureInfo.CurrentCulture);
				var modelState = new ModelState {
					Value = valueResult
				};
				try
				{
					if(valueResultDays != null)
						days = int.Parse(valueResultDays.AttemptedValue);
					if(valueResultHours != null)
						hours = int.Parse(valueResultHours.AttemptedValue);
					if(valueResultMinutes != null)
						minutes = int.Parse(valueResultMinutes.AttemptedValue);
					if(valueResultSeconds != null)
						seconds = int.Parse(valueResultSeconds.AttemptedValue);
					actualValue = new TimeSpan(days, hours, minutes, seconds, 0);
				}
				catch(FormatException e)
				{
					modelState.Errors.Add(e);
				}
				bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
				return actualValue;
			}
			else
			{
				return base.BindModel(controllerContext, bindingContext);
			}
		}
	}
}
