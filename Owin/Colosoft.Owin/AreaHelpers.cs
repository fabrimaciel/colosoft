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
using System.Web.Mvc;
using System.Web.Routing;

namespace Colosoft.Owin.Razor
{
	static class AreaHelpers
	{
		/// <summary>
		/// Recupera o nome da area.
		/// </summary>
		/// <param name="route"></param>
		/// <returns></returns>
		public static string GetAreaName(RouteBase route)
		{
			IRouteWithArea area = route as IRouteWithArea;
			if(area != null)
			{
				return area.Area;
			}
			Route route2 = route as Route;
			if((route2 != null) && (route2.DataTokens != null))
			{
				return (route2.DataTokens["area"] as string);
			}
			return null;
		}

		/// <summary>
		/// Recupera o nome da area.
		/// </summary>
		/// <param name="routeData"></param>
		/// <returns></returns>
		public static string GetAreaName(RouteData routeData)
		{
			object obj2;
			if(routeData.DataTokens.TryGetValue("area", out obj2))
			{
				return (obj2 as string);
			}
			return GetAreaName(routeData.Route);
		}
	}
}
