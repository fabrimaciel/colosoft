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

namespace Colosoft.Web.Mvc.Extensions
{
	/// <summary>
	/// Classe com método de extensão para trabalhar com ModelState
	/// </summary>
	public static class ModelStateExtensions
	{
		/// <summary>
		/// Recupera a mensagem de error.
		/// </summary>
		/// <param name="error"></param>
		/// <param name="modelState"></param>
		/// <returns></returns>
		private static string GetErrorMessage(ModelError error, ModelState modelState)
		{
			if(error.ErrorMessage.HasValue())
			{
				return error.ErrorMessage;
			}
			if(modelState.Value == null)
			{
				return error.ErrorMessage;
			}
			return string.Format("ValueNotValidForProperty {0}", modelState.Value.AttemptedValue);
		}

		/// <summary>
		/// Serializa os erros.
		/// </summary>
		/// <param name="modelState"></param>
		/// <returns></returns>
		public static object SerializeErrors(this ModelStateDictionary modelState)
		{
			return (from entry in modelState
			where entry.Value.Errors.Any<ModelError>()
			select entry).ToDictionary<KeyValuePair<string, ModelState>, string, Dictionary<string, object>>(entry => entry.Key, entry => SerializeModelState(entry.Value));
		}

		/// <summary>
		/// Serializa o estado do modelo.
		/// </summary>
		/// <param name="modelState"></param>
		/// <returns></returns>
		private static Dictionary<string, object> SerializeModelState(ModelState modelState)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["errors"] = (from error in modelState.Errors
			select GetErrorMessage(error, modelState)).ToArray<string>();
			return dictionary;
		}

		/// <summary>
		/// Converte para um resultado da fonte de dados.
		/// </summary>
		/// <param name="modelState"></param>
		/// <returns></returns>
		public static object ToDataSourceResult(this ModelStateDictionary modelState)
		{
			if(!modelState.IsValid)
			{
				return new {
					Errors = modelState.SerializeErrors()
				};
			}
			return new object();
		}
	}
}
