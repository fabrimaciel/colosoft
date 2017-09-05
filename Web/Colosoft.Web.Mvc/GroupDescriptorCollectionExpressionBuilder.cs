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
	/// Implementação do construtor de expressão da coleção de descritores de grupo.
	/// </summary>
	class GroupDescriptorCollectionExpressionBuilder : ExpressionBuilderBase
	{
		private readonly IEnumerable<GroupDescriptor> _groupDescriptors;

		private readonly IQueryable _notPagedData;

		private readonly IQueryable _queryable;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="groupDescriptors"></param>
		/// <param name="notPagedData"></param>
		public GroupDescriptorCollectionExpressionBuilder(IQueryable queryable, IEnumerable<GroupDescriptor> groupDescriptors, IQueryable notPagedData) : base(queryable.ElementType)
		{
			_queryable = queryable;
			_groupDescriptors = groupDescriptors;
			_notPagedData = notPagedData;
		}

		/// <summary>
		/// Cria a consulta.
		/// </summary>
		/// <returns></returns>
		public IQueryable CreateQuery()
		{
			GroupDescriptorExpressionBuilder childBuilder = null;
			foreach (GroupDescriptor descriptor in _groupDescriptors.Reverse<GroupDescriptor>())
			{
				childBuilder = new GroupDescriptorExpressionBuilder(_queryable, descriptor, childBuilder, this._notPagedData) {
					Options =  {
						LiftMemberAccessToNull = _queryable.Provider.IsLinqToObjectsProvider()
					}
				};
			}
			if(childBuilder != null)
			{
				return childBuilder.CreateQuery();
			}
			return _queryable;
		}
	}
}
