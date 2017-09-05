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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Descritor de filtro composto.
	/// </summary>
	public class CompositeFilterDescriptor : FilterDescriptorBase
	{
		private Infrastructure.Implementation.FilterDescriptorCollection _filterDescriptors;

		/// <summary>
		/// Cria a expressão para o filtro.
		/// </summary>
		/// <param name="parameterExpression"></param>
		/// <returns></returns>
		protected override Expression CreateFilterExpression(ParameterExpression parameterExpression)
		{
			var builder = new Infrastructure.Implementation.Expressions.FilterDescriptorCollectionExpressionBuilder(parameterExpression, this.FilterDescriptors, this.LogicalOperator);
			builder.Options.CopyFrom(base.ExpressionBuilderOptions);
			return builder.CreateBodyExpression();
		}

		/// <summary>
		/// Serializa os dados para o dicionário informado.
		/// </summary>
		/// <param name="json"></param>
		protected override void Serialize(IDictionary<string, object> json)
		{
			base.Serialize(json);
			json["logic"] = this.LogicalOperator.ToString().ToLowerInvariant();
			if(this.FilterDescriptors.Any<IFilterDescriptor>())
			{
				json["filters"] = this.FilterDescriptors.OfType<JsonObject>().ToJson();
			}
		}

		/// <summary>
		/// Define os descritores.
		/// </summary>
		/// <param name="value"></param>
		private void SetFilterDescriptors(Infrastructure.Implementation.FilterDescriptorCollection value)
		{
			_filterDescriptors = value;
		}

		/// <summary>
		/// Descritores.
		/// </summary>
		public Infrastructure.Implementation.FilterDescriptorCollection FilterDescriptors
		{
			get
			{
				if(_filterDescriptors == null)
					SetFilterDescriptors(new Infrastructure.Implementation.FilterDescriptorCollection());
				return _filterDescriptors;
			}
			set
			{
				if(_filterDescriptors != value)
				{
					SetFilterDescriptors(value);
				}
			}
		}

		public FilterCompositionLogicalOperator LogicalOperator
		{
			get;
			set;
		}
	}
}
