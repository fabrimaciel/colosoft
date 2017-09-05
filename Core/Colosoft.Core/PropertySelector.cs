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

namespace Colosoft
{
	/// <summary>
	/// Classe responsável pela seleção de propriedades do tipo informado.
	/// </summary>
	public static class PropertySelector<T>
	{
		/// <summary>
		/// Recupera as propriedade da expressão informada.
		/// </summary>
		/// <param name="propertiesSelector">Expressão que seleciona as propriedades.</param>
		/// <returns>Informações das propriedades selecionadas.</returns>
		public static IEnumerable<System.Reflection.PropertyInfo> GetProperties(params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
		{
			if(propertiesSelector != null)
			{
				foreach (var i in propertiesSelector)
				{
					if(i == null)
						continue;
					var property = i.GetMember() as System.Reflection.PropertyInfo;
					if(property != null)
						yield return property;
				}
			}
		}

		/// <summary>
		/// Recupera os nomes das propriedades da expressão informada.
		/// </summary>
		/// <param name="propertiesSelector">Expressão que seleciona as propriedades.</param>
		/// <returns></returns>
		public static IEnumerable<string> GetPropertyNames(params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
		{
			if(propertiesSelector != null)
			{
				foreach (var i in propertiesSelector)
				{
					if(i == null)
						continue;
					var property = i.GetMember() as System.Reflection.PropertyInfo;
					if(property != null)
						yield return property.Name;
				}
			}
		}

		/// <summary>
		/// Recupera o nome da propriedade da expressão.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static string ExtractPropertyName(System.Linq.Expressions.Expression<Func<T>> propertyExpression)
		{
			if(propertyExpression == null)
				throw new ArgumentNullException("propertyExpression");
			var memberExpression = propertyExpression.Body as System.Linq.Expressions.MemberExpression;
			if(memberExpression == null)
				throw new ArgumentException(Colosoft.Properties.Resources.PropertySelector_NotMemberAccessExpression_Exception, "propertyExpression");
			var property = memberExpression.Member as System.Reflection.PropertyInfo;
			if(property == null)
				throw new ArgumentException(Colosoft.Properties.Resources.PropertySelector_ExpressionNotProperty_Exception, "propertyExpression");
			var getMethod = property.GetGetMethod(true);
			if(getMethod.IsStatic)
				throw new ArgumentException(Colosoft.Properties.Resources.PropertySelector_StaticExpression_Exception, "propertyExpression");
			return memberExpression.Member.Name;
		}
	}
}
