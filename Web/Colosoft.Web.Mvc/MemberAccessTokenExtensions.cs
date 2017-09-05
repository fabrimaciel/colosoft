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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Classe com métodos de extensão para MemberAccessToken.
	/// </summary>
	static class MemberAccessTokenExtensions
	{
		/// <summary>
		/// Cria a expressão de acesso para o membro.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static Expression CreateMemberAccessExpression(this IMemberAccessToken token, Expression instance)
		{
			System.Reflection.MemberInfo memberInfoForType = token.GetMemberInfoForType(instance.Type);
			if(memberInfoForType == null)
			{
				throw new ArgumentException(FormatInvalidTokenErrorMessage(token, instance.Type));
			}
			IndexerToken indexerToken = token as IndexerToken;
			if(indexerToken != null)
			{
				IEnumerable<Expression> indexerArguments = indexerToken.GetIndexerArguments();
				return Expression.Call(instance, (System.Reflection.MethodInfo)memberInfoForType, indexerArguments);
			}
			return Expression.MakeMemberAccess(instance, memberInfoForType);
		}

		/// <summary>
		/// Formata a mensagem de error do token inválido.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private static string FormatInvalidTokenErrorMessage(IMemberAccessToken token, Type type)
		{
			string propertyName;
			string str2;
			PropertyToken token2 = token as PropertyToken;
			if(token2 != null)
			{
				str2 = "property or field";
				propertyName = token2.PropertyName;
			}
			else
			{
				str2 = "indexer with arguments";
				IEnumerable<string> source = from a in ((IndexerToken)token).Arguments
				where a != null
				select a.ToString();
				propertyName = string.Join(",", source.ToArray<string>());
			}
			return string.Format("Invalid {0} - '{1}' for type: {2}", str2, propertyName, type.GetTypeName());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="indexerToken"></param>
		/// <returns></returns>
		private static IEnumerable<Expression> GetIndexerArguments(this IndexerToken indexerToken)
		{
			return (from a in indexerToken.Arguments
			select Expression.Constant(a));
		}

		/// <summary>
		/// Recupera as informações do membro para o tipo informado.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		private static MemberInfo GetMemberInfoForType(this IMemberAccessToken token, Type targetType)
		{
			PropertyToken token2 = token as PropertyToken;
			if(token2 != null)
			{
				return GetMemberInfoFromPropertyToken(token2, targetType);
			}
			IndexerToken token3 = token as IndexerToken;
			if(token3 == null)
			{
				throw new InvalidOperationException(token.GetType().GetTypeName() + " is not supported");
			}
			return GetMemberInfoFromIndexerToken(token3, targetType);
		}

		/// <summary>
		/// Recupera as informações do membro a partir do token do indexador.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		private static MemberInfo GetMemberInfoFromIndexerToken(IndexerToken token, Type targetType)
		{
			PropertyInfo indexerPropertyInfo = targetType.GetIndexerPropertyInfo((from a in token.Arguments
			select a.GetType()).ToArray<Type>());
			if(indexerPropertyInfo != null)
			{
				return indexerPropertyInfo.GetGetMethod();
			}
			return null;
		}

		/// <summary>
		/// Recupera as informações do membro a partir do token da propriedade.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		private static MemberInfo GetMemberInfoFromPropertyToken(PropertyToken token, Type targetType)
		{
			return targetType.FindPropertyOrField(token.PropertyName);
		}
	}
}
