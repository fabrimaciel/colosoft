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
	/// DateTimeModelBinder.
	/// </summary>
	public class DateTimeModelBinder : IModelBinder
	{
		/// <summary>
		/// Possíveis formatos par ao padrão ISO-8601
		/// </summary>
		private static readonly string[] ISO8601Formats =  {
			"yyyyMMddTHHmmsszzz",
			"yyyyMMddTHHmmsszz",
			"yyyyMMddTHHmmssZ",
			"yyyy-MM-ddTHH:mm:sszzz",
			"yyyy-MM-ddTHH:mm:sszz",
			"yyyy-MM-ddTHH:mm:ssZ",
			"yyyy-MM-ddTHH:mm:ss.fffZ",
			"yyyyMMddTHHmmzzz",
			"yyyyMMddTHHmmzz",
			"yyyyMMddTHHmmZ",
			"yyyy-MM-ddTHH:mmzzz",
			"yyyy-MM-ddTHH:mmzz",
			"yyyy-MM-ddTHH:mmZ",
			"yyyyMMddTHHzzz",
			"yyyyMMddTHHzz",
			"yyyyMMddTHHZ",
			"yyyy-MM-ddTHHzzz",
			"yyyy-MM-ddTHHzz",
			"yyyy-MM-ddTHHZ"
		};

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
			DateTime? resDateTime = null;
			if(valueResult == null)
				return null;
			if((bindingContext.ModelType == typeof(DateTime) || bindingContext.ModelType == typeof(DateTime?)))
			{
				if(!string.IsNullOrEmpty(valueResult.AttemptedValue) && valueResult.AttemptedValue.StartsWith("/Date("))
				{
					try
					{
						var epoch = long.Parse(valueResult.AttemptedValue.Substring("/Date(".Length, valueResult.AttemptedValue.Length - "/Date(".Length - 2));
						resDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddMilliseconds(epoch) + DateTimeOffset.Now.Offset;
					}
					catch(Exception ex)
					{
						if(bindingContext.ModelName != "Version")
							modelState.Errors.Add(ex);
						else
							throw;
					}
				}
				else if(bindingContext.ModelName != "Version")
				{
					DateTime resDateTime2;
					if(DateTime.TryParseExact(valueResult.AttemptedValue, ISO8601Formats, Culture ?? valueResult.Culture, DateTimeStyles.AssumeUniversal, out resDateTime2))
						resDateTime = resDateTime2;
					else
						try
						{
							resDateTime = DateTime.Parse(valueResult.AttemptedValue, Culture ?? valueResult.Culture, DateTimeStyles.AdjustToUniversal);
						}
						catch(Exception e)
						{
							modelState.Errors.Add(e);
						}
				}
				else
				{
					resDateTime = DateTime.Parse(valueResult.AttemptedValue, Culture ?? valueResult.Culture);
				}
			}
			bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
			return bindingContext.ModelType == typeof(DateTime) ? resDateTime.GetValueOrDefault() : resDateTime;
		}
	}
}
