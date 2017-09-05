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

namespace Colosoft.Web.Mvc.UI
{
	/// <summary>
	/// Métodos de extensão do Widget.
	/// </summary>
	public static class WidgetExtensions
	{
		/// <summary>
		/// Verifica se é um caracter válido.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		private static bool IsValidCharacter(char c)
		{
			if(c == '?' || c == '!' || c == '#' || c == '.' || c == '[' || c == ']')
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Substitui caracteres inválidos.
		/// </summary>
		/// <param name="part"></param>
		/// <param name="builder"></param>
		private static void ReplaceInvalidCharacters(string part, StringBuilder builder)
		{
			for(int i = 0; i < part.Length; i++)
			{
				char character = part[i];
				if(IsValidCharacter(character))
				{
					builder.Append(character);
				}
				else
				{
					builder.Append(HtmlHelper.IdAttributeDotReplacement);
				}
			}
		}

		/// <summary>
		/// Recupera o valor do Widget.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetValue<T>(this IWidget instance, T value)
		{
			return instance.GetValue<T>(instance.Name, value);
		}

		/// <summary>
		/// Recupera o valor do widget.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string GetValue<T>(this IWidget instance, string name, T value, string format = "{0}")
		{
			ModelState state;
			string formattedValue = string.Empty;
			ViewDataDictionary viewData = instance.ViewData;
			object valueFromViewData = !string.IsNullOrEmpty(name) ? viewData.Eval(name) : null;
			if(!string.IsNullOrEmpty(name) && viewData.ModelState.TryGetValue(name, out state) && (state.Value != null))
			{
				formattedValue = state.Value.AttemptedValue;
				if(viewData.ModelState.IsValidField(name))
				{
					formattedValue = string.Format(System.Globalization.CultureInfo.CurrentCulture, format, state.Value.ConvertTo(typeof(T), state.Value.Culture));
				}
			}
			else if(value != null)
			{
				formattedValue = string.Format(System.Globalization.CultureInfo.CurrentCulture, format, value);
			}
			else if(valueFromViewData != null && valueFromViewData.GetType().IsPredefinedType())
			{
				formattedValue = string.Format(System.Globalization.CultureInfo.CurrentCulture, format, valueFromViewData);
			}
			return formattedValue;
		}

		/// <summary>
		/// Verifica se é valido.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsValid(this IWidget instance)
		{
			if(instance.ViewContext != null)
			{
				return instance.ViewData.ModelState.IsValidField(instance.Name ?? string.Empty);
			}
			return true;
		}

		/// <summary>
		/// Recupera os atributos de validação.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IDictionary<string, object> GetUnobtrusiveValidationAttributes(this IWidget instance)
		{
			if(instance.ViewContext.UnobtrusiveJavaScriptEnabled && instance.ViewData.ModelMetadata != null)
			{
				var name = instance.Name;
				var htmlHelper = new HtmlHelper(instance.ViewContext, new WidgetViewDataContainer {
					ViewData = instance.ViewData
				});
				var metadata = instance.ModelMetadata ?? ModelMetadata.FromStringExpression(name, instance.ViewData);
				return htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);
			}
			return null;
		}

		/// <summary>
		/// SanitizeId.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static string SanitizeId(this IWidget instance, string id)
		{
			if(string.IsNullOrWhiteSpace(id))
			{
				return string.Empty;
			}
			StringBuilder builder = new StringBuilder(id.Length);
			int startSharpIndex = id.IndexOf("#");
			int endSharpIndex = id.LastIndexOf("#");
			if(endSharpIndex > startSharpIndex)
			{
				ReplaceInvalidCharacters(id.Substring(0, startSharpIndex), builder);
				builder.Append(id.Substring(startSharpIndex, endSharpIndex - startSharpIndex + 1));
				ReplaceInvalidCharacters(id.Substring(endSharpIndex + 1), builder);
			}
			else
			{
				ReplaceInvalidCharacters(id, builder);
			}
			return builder.ToString();
		}

		class WidgetViewDataContainer : IViewDataContainer
		{
			public ViewDataDictionary ViewData
			{
				get;
				set;
			}
		}
	}
}
