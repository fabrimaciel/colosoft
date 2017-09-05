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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation
{
	static class FilterDescriptorExtensions
	{
		private static IEnumerable<IFilterDescriptor> GetChildDescriptors(IFilterDescriptor f)
		{
			if(f is CompositeFilterDescriptor)
			{
				return ((CompositeFilterDescriptor)f).FilterDescriptors;
			}
			return null;
		}

		internal static bool IsActive(this FilterDescriptor filter)
		{
			object obj2 = filter.Value;
			if(obj2 == null)
			{
				return false;
			}
			string str = obj2 as string;
			if(str != null)
			{
				return !string.IsNullOrEmpty(str);
			}
			return true;
		}

		internal static IEnumerable<FilterDescriptor> SelectMemberDescriptors(this IEnumerable<IFilterDescriptor> descriptors)
		{
			return descriptors.SelectRecursive<IFilterDescriptor>(f => GetChildDescriptors(f)).OfType<FilterDescriptor>();
		}

		internal static void SetMemberTypeFrom(this FilterDescriptor descriptor, object item)
		{
			if(descriptor.Member.HasValue())
			{
				descriptor.MemberType = BindingHelper.ExtractMemberTypeFromObject(item, descriptor.Member);
			}
		}

		internal static IEnumerable<IFilterDescriptor> SetMemberTypeFrom(this IEnumerable<IFilterDescriptor> descriptors, object item)
		{
			Action<FilterDescriptor> action = null;
			if(item != null)
			{
				if(action == null)
				{
					action = delegate(FilterDescriptor f) {
						f.SetMemberTypeFrom(item);
					};
				}
				descriptors.SelectMemberDescriptors().Each<FilterDescriptor>(action);
			}
			return descriptors;
		}
	}
}
