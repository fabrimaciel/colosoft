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

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Implementação responsável por criar os controllers.
	/// </summary>
	class ControllerFactory : System.Web.Mvc.DefaultControllerFactory
	{
		private System.Web.Mvc.ControllerBuilder _controllerBuilder;

		private IBuildManager _buildManager;

		private ControllerTypeCache _instanceControllerTypeCache;

		private static ControllerTypeCache _staticControllerTypeCache = new ControllerTypeCache();

		/// <summary>
		/// ControllerBuilder.
		/// </summary>
		internal System.Web.Mvc.ControllerBuilder ControllerBuilder
		{
			get
			{
				return (_controllerBuilder ?? System.Web.Mvc.ControllerBuilder.Current);
			}
			set
			{
				_controllerBuilder = value;
			}
		}

		internal IBuildManager BuildManager
		{
			get
			{
				if(_buildManager == null)
					_buildManager = new BuildManagerWrapper();
				return _buildManager;
			}
			set
			{
				_buildManager = value;
			}
		}

		internal ControllerTypeCache ControllerTypeCache
		{
			get
			{
				return _instanceControllerTypeCache ?? _staticControllerTypeCache;
			}
			set
			{
				_instanceControllerTypeCache = value;
			}
		}

		/// <summary>
		/// Recupera o tipo do controller com base nos namespaces.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="controllerName"></param>
		/// <param name="namespaces"></param>
		/// <returns></returns>
		private Type GetControllerTypeWithinNamespaces(System.Web.Routing.RouteBase route, string controllerName, HashSet<string> namespaces)
		{
			ControllerTypeCache.EnsureInitialized(BuildManager);
			ICollection<Type> matchingTypes = ControllerTypeCache.GetControllerTypes(controllerName, namespaces);
			switch(matchingTypes.Count)
			{
			case 0:
				return null;
			case 1:
				return matchingTypes.First();
			default:
				throw CreateAmbiguousControllerException(route, controllerName, matchingTypes);
			}
		}

		/// <summary>
		/// Cria o erro de controller ambiguo.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="controllerName"></param>
		/// <param name="matchingTypes"></param>
		/// <returns></returns>
		private static InvalidOperationException CreateAmbiguousControllerException(System.Web.Routing.RouteBase route, string controllerName, ICollection<Type> matchingTypes)
		{
			StringBuilder typeList = new StringBuilder();
			foreach (Type matchedType in matchingTypes)
			{
				typeList.AppendLine();
				typeList.Append(matchedType.FullName);
			}
			string errorText;
			var castRoute = route as System.Web.Routing.Route;
			if(castRoute != null)
			{
				errorText = string.Format("Controller '{0}' ambiguous with route url '{1}': {2}", controllerName, castRoute.Url, typeList);
			}
			else
			{
				errorText = string.Format("Controller '{0}' ambiguous: {1}", controllerName, typeList);
			}
			return new InvalidOperationException(errorText);
		}

		/// <summary>
		/// Cria uma instancia do controller.
		/// </summary>
		/// <param name="requestContext"></param>
		/// <param name="controllerName"></param>
		/// <returns></returns>
		public override System.Web.Mvc.IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
		{
			if(requestContext == null)
			{
				throw new ArgumentNullException("requestContext");
			}
			if(string.IsNullOrEmpty(controllerName))
				throw new ArgumentException("controllerName");
			Type controllerType = GetControllerType(requestContext, controllerName);
			if(controllerType != null)
				return (System.Web.Mvc.IController)Activator.CreateInstance(controllerType);
			else
				return new NotFoundController();
		}

		/// <summary>
		/// Recupera o tipo do controller.
		/// </summary>
		/// <param name="requestContext"></param>
		/// <param name="controllerName"></param>
		/// <returns></returns>
		protected override Type GetControllerType(System.Web.Routing.RequestContext requestContext, string controllerName)
		{
			object obj2;
			Type type;
			if(requestContext == null)
				throw new ArgumentNullException("requestContext");
			if(string.IsNullOrEmpty(controllerName) && ((requestContext.RouteData == null)))
				throw new ArgumentNullException("controllerName");
			var routeData = requestContext.RouteData;
			if(routeData.DataTokens.TryGetValue("Namespaces", out obj2))
			{
				IEnumerable<string> source = obj2 as IEnumerable<string>;
				if((source != null) && source.Any())
				{
					HashSet<string> namespaces = new HashSet<string>(source, StringComparer.OrdinalIgnoreCase);
					type = this.GetControllerTypeWithinNamespaces(routeData.Route, controllerName, namespaces);
					if(type == null)
					{
						bool flag = false;
						if(flag.Equals(routeData.DataTokens["UseNamespaceFallback"]))
							return type;
					}
					else
						return type;
				}
			}
			if(ControllerBuilder.DefaultNamespaces.Count > 0)
			{
				HashSet<string> set2 = new HashSet<string>(this.ControllerBuilder.DefaultNamespaces, StringComparer.OrdinalIgnoreCase);
				type = this.GetControllerTypeWithinNamespaces(routeData.Route, controllerName, set2);
				if(type != null)
					return type;
			}
			return this.GetControllerTypeWithinNamespaces(routeData.Route, controllerName, null);
		}

		/// <summary>
		/// Implementação do controller não encontrado.
		/// </summary>
		class NotFoundController : System.Web.Mvc.IController
		{
			public void Execute(System.Web.Routing.RequestContext requestContext)
			{
				var response = requestContext.HttpContext.Response;
				response.StatusCode = 404;
				response.Write("Page not found");
			}
		}
	}
}
