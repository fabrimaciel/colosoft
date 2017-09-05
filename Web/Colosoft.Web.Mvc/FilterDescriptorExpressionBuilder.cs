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
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Representa o construtor de expressão do descritor de filtro.
	/// </summary>
	class FilterDescriptorExpressionBuilder : FilterExpressionBuilder
	{
		private readonly FilterDescriptor _descriptor;

		/// <summary>
		/// Descritor do filtro.
		/// </summary>
		public FilterDescriptor FilterDescriptor
		{
			get
			{
				return this._descriptor;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parameterExpression"></param>
		/// <param name="descriptor"></param>
		public FilterDescriptorExpressionBuilder(ParameterExpression parameterExpression, FilterDescriptor descriptor) : base(parameterExpression)
		{
			_descriptor = descriptor;
		}

		/// <summary>
		/// Cria o corpo da expressão.
		/// </summary>
		/// <returns></returns>
		public override Expression CreateBodyExpression()
		{
			Expression memberExpression = this.CreateMemberExpression();
			Type targetType = memberExpression.Type;
			Expression valueExpression = CreateValueExpression(targetType, _descriptor.Value, System.Globalization.CultureInfo.InvariantCulture);
			bool flag = true;
			if(TypesAreDifferent(this._descriptor, memberExpression, valueExpression))
			{
				if(!TryConvertExpressionTypes(ref memberExpression, ref valueExpression))
				{
					flag = false;
				}
			}
			else if(memberExpression.Type.IsEnumType() || valueExpression.Type.IsEnumType())
			{
				if(!TryPromoteNullableEnums(ref memberExpression, ref valueExpression))
				{
					flag = false;
				}
			}
			else if((targetType.IsNullableType() && (memberExpression.Type != valueExpression.Type)) && !TryConvertNullableValue(memberExpression, ref valueExpression))
			{
				flag = false;
			}
			if(!flag)
			{
				throw new ArgumentException(string.Format("Operator '{0}' is incompatible with operand types '{1}' and '{2}'", _descriptor.Operator, memberExpression.Type.GetTypeName(), valueExpression.Type.GetTypeName()));
			}
			return this._descriptor.Operator.CreateExpression(memberExpression, valueExpression, base.Options.LiftMemberAccessToNull);
		}

		/// <summary>
		/// Cria a descrição do filtro.
		/// </summary>
		/// <returns></returns>
		public FilterDescription CreateFilterDescription()
		{
			return new PredicateFilterDescription(base.CreateFilterExpression().Compile());
		}

		/// <summary>
		/// Cria a expressão do membro.
		/// </summary>
		/// <returns></returns>
		protected virtual Expression CreateMemberExpression()
		{
			Type memberType = this.FilterDescriptor.MemberType;
			MemberAccessExpressionBuilderBase base2 = ExpressionBuilderFactory.MemberAccess(base.ParameterExpression.Type, memberType, this.FilterDescriptor.Member);
			base2.Options.CopyFrom(base.Options);
			base2.ParameterExpression = base.ParameterExpression;
			Expression expression = base2.CreateMemberAccessExpression();
			if((memberType != null) && (expression.Type.GetNonNullableType() != memberType.GetNonNullableType()))
			{
				expression = Expression.Convert(expression, memberType);
			}
			return expression;
		}

		/// <summary>
		/// Cria uma expressão constante do valor informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static Expression CreateConstantExpression(object value)
		{
			if(value == null)
			{
				return ExpressionConstants.NullLiteral;
			}
			return Expression.Constant(value);
		}

		/// <summary>
		/// Cria a expressão do valor informado.
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="value"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		private static Expression CreateValueExpression(Type targetType, object value, System.Globalization.CultureInfo culture)
		{
			if(((targetType != typeof(string)) && (!targetType.IsValueType || targetType.IsNullableType())) && (string.Compare(value as string, "null", StringComparison.OrdinalIgnoreCase) == 0))
				value = null;
			if(value != null)
			{
				Type nonNullableType = targetType.GetNonNullableType();
				if(value.GetType() != nonNullableType)
				{
					if(nonNullableType.IsEnum)
					{
						value = Enum.Parse(nonNullableType, value.ToString(), true);
					}
					else if(nonNullableType == typeof(Guid))
					{
						value = new Guid(value.ToString());
					}
					else if(value is IConvertible)
					{
						value = Convert.ChangeType(value, nonNullableType, culture);
					}
				}
			}
			return CreateConstantExpression(value);
		}

		/// <summary>
		/// Promove a expressão.
		/// </summary>
		/// <param name="expr"></param>
		/// <param name="type"></param>
		/// <param name="exact"></param>
		/// <returns></returns>
		private static Expression PromoteExpression(Expression expr, Type type, bool exact)
		{
			if(expr.Type == type)
				return expr;
			ConstantExpression expression = expr as ConstantExpression;
			if(((expression != null) && (expression == ExpressionConstants.NullLiteral)) && (!type.IsValueType || type.IsNullableType()))
				return Expression.Constant(null, type);
			if(!expr.Type.IsCompatibleWith(type))
				return null;
			if(!type.IsValueType && !exact)
				return expr;
			return Expression.Convert(expr, type);
		}

		/// <summary>
		/// Tenta converte os tipos de expressão.
		/// </summary>
		/// <param name="memberExpression"></param>
		/// <param name="valueExpression"></param>
		/// <returns></returns>
		private static bool TryConvertExpressionTypes(ref Expression memberExpression, ref Expression valueExpression)
		{
			if(memberExpression.Type != valueExpression.Type)
			{
				if(!memberExpression.Type.IsAssignableFrom(valueExpression.Type))
				{
					if(!valueExpression.Type.IsAssignableFrom(memberExpression.Type))
					{
						return false;
					}
					memberExpression = Expression.Convert(memberExpression, valueExpression.Type);
				}
				else
				{
					valueExpression = Expression.Convert(valueExpression, memberExpression.Type);
				}
			}
			return true;
		}

		/// <summary>
		/// Tenta converter para um valor nullable.
		/// </summary>
		/// <param name="memberExpression"></param>
		/// <param name="valueExpression"></param>
		/// <returns></returns>
		private static bool TryConvertNullableValue(Expression memberExpression, ref Expression valueExpression)
		{
			ConstantExpression expression = valueExpression as ConstantExpression;
			if(expression != null)
			{
				try
				{
					valueExpression = Expression.Constant(expression.Value, memberExpression.Type);
				}
				catch(ArgumentException)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Tenta promover enums para nullable.
		/// </summary>
		/// <param name="memberExpression"></param>
		/// <param name="valueExpression"></param>
		/// <returns></returns>
		private static bool TryPromoteNullableEnums(ref Expression memberExpression, ref Expression valueExpression)
		{
			if(memberExpression.Type != valueExpression.Type)
			{
				Expression expression = PromoteExpression(valueExpression, memberExpression.Type, true);
				if(expression == null)
				{
					expression = PromoteExpression(memberExpression, valueExpression.Type, true);
					if(expression == null)
					{
						return false;
					}
					memberExpression = expression;
				}
				else
				{
					valueExpression = expression;
				}
			}
			return true;
		}

		/// <summary>
		/// Verifica se os tipos são diferentes.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="memberExpression"></param>
		/// <param name="valueExpression"></param>
		/// <returns></returns>
		private static bool TypesAreDifferent(FilterDescriptor descriptor, Expression memberExpression, Expression valueExpression)
		{
			return ((((descriptor.Operator == FilterOperator.IsEqualTo) || (descriptor.Operator == FilterOperator.IsNotEqualTo)) && !memberExpression.Type.IsValueType) && !valueExpression.Type.IsValueType);
		}
	}
}
