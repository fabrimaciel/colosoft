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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation
{
	/// <summary>
	/// Representa a descrição do filtro.
	/// </summary>
	public abstract class FilterDescription : FilterDescriptorBase
	{
		/// <summary>
		/// Identifica se a descrição está ativa.
		/// </summary>
		public virtual bool IsActive
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected FilterDescription()
		{
		}

		/// <summary>
		/// Cria a expressão do filtro.
		/// </summary>
		/// <param name="parameterExpression"></param>
		/// <returns></returns>
		protected override System.Linq.Expressions.Expression CreateFilterExpression(System.Linq.Expressions.ParameterExpression parameterExpression)
		{
			var builder = new Expressions.FilterDescriptionExpressionBuilder(parameterExpression, this);
			return builder.CreateBodyExpression();
		}

		/// <summary>
		/// Verifica se o item informado satisfaz o filtro.
		/// </summary>
		/// <param name="dataItem"></param>
		/// <returns></returns>
		public abstract bool SatisfiesFilter(object dataItem);
	}
}
