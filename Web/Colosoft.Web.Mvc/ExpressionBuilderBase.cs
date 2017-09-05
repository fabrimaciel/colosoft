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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Implementação base do construtor de expressão.
	/// </summary>
	abstract class ExpressionBuilderBase
	{
		private readonly Type _itemType;

		private readonly ExpressionBuilderOptions _options;

		private System.Linq.Expressions.ParameterExpression _parameterExpression;

		/// <summary>
		/// Tipo do item.
		/// </summary>
		protected internal Type ItemType
		{
			get
			{
				return _itemType;
			}
		}

		/// <summary>
		/// Opções.
		/// </summary>
		public ExpressionBuilderOptions Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// Expressão do parâmetro.
		/// </summary>
		protected internal System.Linq.Expressions.ParameterExpression ParameterExpression
		{
			get
			{
				if(_parameterExpression == null)
					_parameterExpression = System.Linq.Expressions.Expression.Parameter(this.ItemType, "item");
				return _parameterExpression;
			}
			set
			{
				_parameterExpression = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="itemType">Tipo do item.</param>
		protected ExpressionBuilderBase(Type itemType)
		{
			_itemType = itemType;
			_options = new ExpressionBuilderOptions();
		}
	}
}
