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
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Representa o construtor de expressão de acesso ao descritor de propriedade customizado.
	/// </summary>
	class CustomTypeDescriptorPropertyAccessExpressionBuilder : MemberAccessExpressionBuilderBase
	{
		/// <summary>
		/// Recupera o método para recupera a propridade.
		/// </summary>
		private static readonly System.Reflection.MethodInfo PropertyMethod = typeof(CustomTypeDescriptorExtensions).GetMethod("Property");

		private readonly Type _propertyType;

		/// <summary>
		/// Tipo da propriedade.
		/// </summary>
		public Type PropertyType
		{
			get
			{
				return _propertyType;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="elementType"></param>
		/// <param name="memberType"></param>
		/// <param name="memberName"></param>
		public CustomTypeDescriptorPropertyAccessExpressionBuilder(Type elementType, Type memberType, string memberName) : base(elementType, memberName)
		{
			if(!elementType.IsCompatibleWith(typeof(System.ComponentModel.ICustomTypeDescriptor)))
			{
				throw new ArgumentException(string.Format("ElementType: {0} did not implement {1}", elementType, typeof(System.ComponentModel.ICustomTypeDescriptor)), "elementType");
			}
			_propertyType = GetPropertyType(memberType);
		}

		/// <summary>
		/// Cria a expressão de acesso ao membro.
		/// </summary>
		/// <returns></returns>
		public override System.Linq.Expressions.Expression CreateMemberAccessExpression()
		{
			var expression = System.Linq.Expressions.Expression.Constant(base.MemberName);
			return System.Linq.Expressions.Expression.Call(PropertyMethod.MakeGenericMethod(new Type[] {
				this._propertyType
			}), base.ParameterExpression, expression);
		}

		/// <summary>
		/// Recupera o tipo da propriedade com base no tipo do membro.
		/// </summary>
		/// <param name="memberType"></param>
		/// <returns></returns>
		private Type GetPropertyType(Type memberType)
		{
			Type propertyTypeFromTypeDescriptorProvider = GetPropertyTypeFromTypeDescriptorProvider();
			if(propertyTypeFromTypeDescriptorProvider != null)
				memberType = propertyTypeFromTypeDescriptorProvider;
			if(memberType.IsValueType && !memberType.IsNullableType())
				return typeof(Nullable<>).MakeGenericType(new Type[] {
					memberType
				});
			return memberType;
		}

		/// <summary>
		/// Recupera o tipo da propriedade com base no provedor do descritor do tipo.
		/// </summary>
		/// <returns></returns>
		private Type GetPropertyTypeFromTypeDescriptorProvider()
		{
			var descriptor = System.ComponentModel.TypeDescriptor.GetProperties(base.ItemType)[base.MemberName];
			if(descriptor != null)
			{
				return descriptor.PropertyType;
			}
			return null;
		}
	}
}
