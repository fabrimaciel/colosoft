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
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.UI
{
	/// <summary>
	/// Implementação do ModelBinder para a requisição da fonte de dados.
	/// </summary>
	public class DataSourceRequestModelBinder : IModelBinder
	{
		/// <summary>
		/// Préfixo.
		/// </summary>
		public string Prefix
		{
			get;
			set;
		}

		/// <summary>
		/// Vincula os dados do modelo.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <returns></returns>
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			string str;
			int num;
			var request = new DataSourceRequest();
			if(this.TryGetValue<string>(bindingContext, GridUrlParameters.Sort, out str))
				request.Sorts = GridDescriptorSerializer.Deserialize<SortDescriptor>(str);
			if(this.TryGetValue<int>(bindingContext, GridUrlParameters.Page, out num))
				request.Page = num;
			if(this.TryGetValue<int>(bindingContext, GridUrlParameters.PageSize, out num))
				request.PageSize = num;
			if(this.TryGetValue<string>(bindingContext, GridUrlParameters.Filter, out str))
				request.Filters = Infrastructure.FilterDescriptorFactory.Create(str);
			if(this.TryGetValue<string>(bindingContext, GridUrlParameters.Group, out str))
				request.Groups = GridDescriptorSerializer.Deserialize<GroupDescriptor>(str);
			if(this.TryGetValue<string>(bindingContext, GridUrlParameters.Aggregates, out str))
				request.Aggregates = GridDescriptorSerializer.Deserialize<AggregateDescriptor>(str);
			return request;
		}

		/// <summary>
		/// Tenta recupera o valor associado com a chave informada.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="bindingContext"></param>
		/// <param name="key"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		private bool TryGetValue<T>(ModelBindingContext bindingContext, string key, out T result)
		{
			if(Prefix.HasValue())
				key = this.Prefix + "-" + key;
			var result2 = bindingContext.ValueProvider.GetValue(key);
			if(result2 == null)
			{
				result = default(T);
				return false;
			}
			result = (T)result2.ConvertTo(typeof(T));
			return true;
		}
	}
}
