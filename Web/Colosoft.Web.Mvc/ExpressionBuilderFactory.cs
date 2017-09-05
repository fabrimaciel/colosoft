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
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Factory dos construtores de expressão.
	/// </summary>
	static class ExpressionBuilderFactory
	{
		/// <summary>
		/// Recupera o construtor da expressão de acesso ao membro.
		/// </summary>
		/// <param name="source">Consulta de origem.</param>
		/// <param name="memberType">Tipo do membro.</param>
		/// <param name="memberName">Nome do membro.</param>
		/// <returns></returns>
		public static MemberAccessExpressionBuilderBase MemberAccess(IQueryable source, Type memberType, string memberName)
		{
			MemberAccessExpressionBuilderBase base2 = MemberAccess(source.ElementType, memberType, memberName);
			base2.Options.LiftMemberAccessToNull = source.Provider.IsLinqToObjectsProvider();
			return base2;
		}

		/// <summary>
		/// Recupera o construtor de expressão para o acesso ao membro.
		/// </summary>
		/// <param name="elementType">Tipo do elemento.</param>
		/// <param name="memberName">Nome do membro.</param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		public static MemberAccessExpressionBuilderBase MemberAccess(Type elementType, string memberName, bool liftMemberAccess)
		{
			MemberAccessExpressionBuilderBase base2 = MemberAccess(elementType, (Type)null, memberName);
			base2.Options.LiftMemberAccessToNull = liftMemberAccess;
			return base2;
		}

		/// <summary>
		/// Recupera o construtor de expressão para o acesso ao membro.
		/// </summary>
		/// <param name="elementType"></param>
		/// <param name="memberType"></param>
		/// <param name="memberName"></param>
		/// <returns></returns>
		public static MemberAccessExpressionBuilderBase MemberAccess(Type elementType, Type memberType, string memberName)
		{
			memberType = memberType ?? typeof(object);
			if(elementType.IsCompatibleWith(typeof(System.Data.DataRow)))
			{
				return new DataRowFieldAccessExpressionBuilder(memberType, memberName);
			}
			if(elementType.IsCompatibleWith(typeof(System.ComponentModel.ICustomTypeDescriptor)))
			{
				return new CustomTypeDescriptorPropertyAccessExpressionBuilder(elementType, memberType, memberName);
			}
			if(elementType.IsCompatibleWith(typeof(System.Xml.XmlNode)))
			{
				return new XmlNodeChildElementAccessExpressionBuilder(memberName);
			}
			if(!(elementType == typeof(object)) && !elementType.IsCompatibleWith(typeof(System.Dynamic.IDynamicMetaObjectProvider)))
			{
				return new PropertyAccessExpressionBuilder(elementType, memberName);
			}
			return new DynamicPropertyAccessExpressionBuilder(elementType, memberName);
		}

		/// <summary>
		/// Recuper ao construtor de expressão para acessar o membro.
		/// </summary>
		/// <param name="elementType"></param>
		/// <param name="memberType"></param>
		/// <param name="memberName"></param>
		/// <param name="liftMemberAccess"></param>
		/// <returns></returns>
		public static MemberAccessExpressionBuilderBase MemberAccess(Type elementType, Type memberType, string memberName, bool liftMemberAccess)
		{
			MemberAccessExpressionBuilderBase base2 = MemberAccess(elementType, memberType, memberName);
			base2.Options.LiftMemberAccessToNull = liftMemberAccess;
			return base2;
		}
	}
}
