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

namespace Colosoft.Web.Mvc.Extensions
{
	public static class ExpressionExtensions
	{
		public static bool IsBindable(this LambdaExpression expression)
		{
			ExpressionType nodeType = expression.Body.NodeType;
			if((nodeType != ExpressionType.MemberAccess) && (nodeType != ExpressionType.Parameter))
			{
				return false;
			}
			return true;
		}

		public static string MemberWithoutInstance(this LambdaExpression expression)
		{
			return System.Web.Mvc.ExpressionHelper.GetExpressionText(expression);
		}

		public static MemberExpression ToMemberExpression(this LambdaExpression expression)
		{
			MemberExpression body = expression.Body as MemberExpression;
			if(body == null)
			{
				UnaryExpression expression3 = expression.Body as UnaryExpression;
				if(expression3 != null)
				{
					body = expression3.Operand as MemberExpression;
				}
			}
			return body;
		}
	}
}
