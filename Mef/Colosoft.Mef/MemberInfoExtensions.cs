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

namespace Colosoft.Mef
{
	/// <summary>
	/// 
	/// </summary>
	static class MemberInfoExtensions
	{
		/// <summary>
		/// Recuperae o cardinalidade do membro informado.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="allowDefault"></param>
		/// <returns></returns>
		public static System.ComponentModel.Composition.Primitives.ImportCardinality GetCardinality(this System.Reflection.MemberInfo member, bool allowDefault)
		{
			var propertyInfo = member as System.Reflection.PropertyInfo;
			if(propertyInfo != null)
			{
				if(propertyInfo.PropertyType.IsEnumerable())
					return System.ComponentModel.Composition.Primitives.ImportCardinality.ZeroOrMore;
				return (allowDefault) ? System.ComponentModel.Composition.Primitives.ImportCardinality.ZeroOrOne : System.ComponentModel.Composition.Primitives.ImportCardinality.ExactlyOne;
			}
			var fieldInfo = member as System.Reflection.FieldInfo;
			if(fieldInfo != null)
			{
				if(fieldInfo.FieldType.IsEnumerable())
					return System.ComponentModel.Composition.Primitives.ImportCardinality.ZeroOrMore;
				return (allowDefault) ? System.ComponentModel.Composition.Primitives.ImportCardinality.ZeroOrOne : System.ComponentModel.Composition.Primitives.ImportCardinality.ExactlyOne;
			}
			if(member is System.Reflection.ConstructorInfo)
				return System.ComponentModel.Composition.Primitives.ImportCardinality.ExactlyOne;
			throw new System.ComponentModel.Composition.ImportCardinalityMismatchException();
		}

		/// <summary>
		/// Cria um <see cref="ComposableMember"/> para uma instancia de <see cref="System.Reflection.MemberInfo"/>.
		/// </summary>
		/// <param name="member">Instancia do <see cref="System.Reflection.MemberInfo"/> para criar o <see cref="ComposableMember"/>.</param>
		/// <returns>A <see cref="ComposableMember"/> instance.</returns>
		public static ComposableMember ToComposableMember(this System.Reflection.MemberInfo member)
		{
			return ComposableMember.Create(member);
		}

		/// <summary>
		/// Cria um <see cref="ComposableMember"/> para uma instancia de <see cref="System.Reflection.ParameterInfo"/>.
		/// </summary>
		/// <param name="parameter">Instancia do <see cref="System.Reflection.ParameterInfo"/> para criar o <see cref="ComposableMember"/>.</param>
		/// <returns>A <see cref="ComposableMember"/> instance.</returns>
		public static ComposableMember ToComposableMember(this System.Reflection.ParameterInfo parameter)
		{
			return new ComposableParameter(parameter);
		}
	}
}
