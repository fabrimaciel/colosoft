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

namespace Colosoft
{
	/// <summary>
	/// Classe com métodos de extensão para tratar o HttpHandler.
	/// </summary>
	public static class HttpHandlerExtensions
	{
		/// <summary>
		/// Atualiza os parametro que devem ser aplicados ao handler.
		/// </summary>
		/// <param name="handler">Handle onde serão aplicados os dados.</param>
		/// <param name="request">Dados da requisição.</param>
		/// <param name="culture">Cultura que será utilizada.</param>
		public static void RefreshFromParameters(this System.Web.IHttpHandler handler, System.Web.HttpRequest request, System.Globalization.CultureInfo culture)
		{
			if(handler == null)
				return;
			request.Require("request").NotNull();
			var properties = handler.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			foreach (var prop in properties)
			{
				var parameter = prop.GetCustomAttributes(typeof(Colosoft.Web.QueryStringAttribute), true).Select(f => (Colosoft.Web.QueryStringAttribute)f).FirstOrDefault();
				if(parameter == null)
					continue;
				var parameterName = parameter.Name;
				object parameterValue = request[parameterName] ?? parameter.DefaultValue;
				if(parameterName != null && parameterValue != null)
				{
					if(parameterValue != null)
					{
						if(parameterValue.GetType() != prop.PropertyType)
						{
							var propertyTypeConverter = System.ComponentModel.TypeDescriptor.GetConverter(prop.PropertyType);
							if(propertyTypeConverter.CanConvertFrom(parameterValue.GetType()))
							{
								try
								{
									parameterValue = propertyTypeConverter.ConvertFrom(null, culture, parameterValue);
								}
								catch(Exception ex)
								{
									throw new Exception(string.Format("Error on convert parameter {0} with value '{1}' to property {2} of type {3}", parameterName, parameterValue, prop.PropertyType.FullName, handler.GetType().FullName), ex);
								}
							}
							else
								throw new Exception(string.Format("Invalid cast from parameter {0} with value '{1}' to property {2} of type {3}", parameterName, parameterValue, prop.PropertyType.FullName, handler.GetType().FullName));
						}
						try
						{
							prop.SetValue(handler, parameterValue, null);
						}
						catch(System.Reflection.TargetInvocationException ex)
						{
							if(ex.InnerException != null)
								throw ex.InnerException;
							throw;
						}
					}
				}
			}
		}
	}
}
