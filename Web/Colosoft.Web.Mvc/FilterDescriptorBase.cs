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

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Representa a base do descritor de filtro.
	/// </summary>
	public class FilterDescriptorBase : JsonObject, IFilterDescriptor
	{
		private Infrastructure.Implementation.Expressions.ExpressionBuilderOptions _options;

		/// <summary>
		/// Opções do construtor de expressão.
		/// </summary>
		internal Infrastructure.Implementation.Expressions.ExpressionBuilderOptions ExpressionBuilderOptions
		{
			get
			{
				if(_options == null)
					_options = new Infrastructure.Implementation.Expressions.ExpressionBuilderOptions();
				return this._options;
			}
		}

		/// <summary>
		/// Cria a expressão do filtro.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public virtual System.Linq.Expressions.Expression CreateFilterExpression(System.Linq.Expressions.Expression instance)
		{
			var parameterExpression = instance as System.Linq.Expressions.ParameterExpression;
			if(parameterExpression == null)
				throw new ArgumentException("Parameter should be of type ParameterExpression", "instance");
			return this.CreateFilterExpression(parameterExpression);
		}

		/// <summary>
		/// Cria a expressão do filtro.
		/// </summary>
		/// <param name="parameterExpression"></param>
		/// <returns></returns>
		protected virtual System.Linq.Expressions.Expression CreateFilterExpression(System.Linq.Expressions.ParameterExpression parameterExpression)
		{
			return parameterExpression;
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="json"></param>
		protected override void Serialize(IDictionary<string, object> json)
		{
		}
	}
}
