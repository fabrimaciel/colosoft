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
using System.Text;
using Colosoft.Configuration.Attributes;
using Colosoft.Configuration;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;

namespace Colosoft.WebControls.Route
{
	class RouteSettings
	{
		/// <summary>
		/// Identifica que os dados de configuração já foram carregados.
		/// </summary>
		private static bool loadDataConfig = false;

		private static object syncObject = new object();

		/// <summary>
		/// Lista das informações de roteamento.
		/// </summary>
		private static List<RouteInfo> _routesInfo = new List<RouteInfo>();

		/// <summary>
		/// Rotas de navegação.
		/// </summary>
		private static Dictionary<string, RouteInfoNavigate> _routesNavigate = new Dictionary<string, RouteInfoNavigate>();

		/// <summary>
		/// Lista das rotas de navegação complexas.
		/// </summary>
		private static Dictionary<string, RouteInfoNavigate> _complexRoutesNavigate = new Dictionary<string, RouteInfoNavigate>();

		/// <summary>
		/// Lista das ações de processamento das rotas.
		/// </summary>
		private static Dictionary<string, RouteAction> _routeActions = new Dictionary<string, RouteAction>();

		/// <summary>
		/// Lista das rotas que devem ser ignoradas pelo sistema de roteamento.
		/// </summary>
		private static List<string> _ignoreRoutes = new List<string>();

		/// <summary>
		/// Quantidade máxima de itens que deverão ser armazenados no cache.
		/// </summary>
		private static int _cacheMaximumItems = 64;

		/// <summary>
		/// Quantidade máxima de itens que deverão ser armazenados no cache.
		/// </summary>
		public static int CacheMaximumItems
		{
			get
			{
				return RouteSettings._cacheMaximumItems;
			}
			set
			{
				RouteSettings._cacheMaximumItems = value;
			}
		}

		/// <summary>
		/// Lista das informações de roteamento.
		/// </summary>
		public static List<RouteInfo> RoutesInfo
		{
			get
			{
				return _routesInfo;
			}
		}

		/// <summary>
		/// Rotas de navegação.
		/// </summary>
		public static Dictionary<string, RouteInfoNavigate> RoutesNavigate
		{
			get
			{
				return _routesNavigate;
			}
		}

		/// <summary>
		/// Rotas de navegação complexas.
		/// </summary>
		public static Dictionary<string, RouteInfoNavigate> ComplexRoutesNavigate
		{
			get
			{
				return _complexRoutesNavigate;
			}
		}

		/// <summary>
		/// Lista das ações de processamento das rotas.
		/// </summary>
		public static Dictionary<string, RouteAction> RouteActions
		{
			get
			{
				return _routeActions;
			}
		}

		/// <summary>
		/// Lista das rotas que devem ser ignoradas pelo sistema de roteamento.
		/// </summary>
		public static List<string> IgnoreRoutes
		{
			get
			{
				return _ignoreRoutes;
			}
		}

		/// <summary>
		/// Recupera a ação com de rota com base no nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static RouteAction GetAction(string name)
		{
			RouteAction action = null;
			_routeActions.TryGetValue(name, out action);
			return action;
		}

		/// <summary>
		/// Adiciona uma rota que deve ser ignorada pelo sistema de roteamento.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="location"></param>
		[Configuration("Route" + "/Ignore", ConfigKeyPresence.Optional)]
		public static void AddIgnoreRoute(string location)
		{
			if(!_ignoreRoutes.Exists(f => string.Compare(f, location, true) == 0))
				_ignoreRoutes.Add(location);
		}

		/// <summary>
		/// Adiciona as informacoes do roteamento no sistema.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="location"></param>
		/// <param name="destinationPage"></param>
		/// <param name="path"></param>
		/// <param name="compiled"></param>
		[Configuration("Route" + "/Info", ConfigKeyPresence.Mandatory)]
		public static void AddRouteInfo(string name, string location, string destinationPage, string path, string compiled, string actionName, string isDirectory, string isSecurePage, string forceNoSecurePage)
		{
			if(string.IsNullOrEmpty(path))
				path = "*";
			else
				path = path.Replace(".", "").Replace(" ", "");
			bool isCompiled = true;
			if(!bool.TryParse(compiled, out isCompiled))
				isCompiled = true;
			bool isDirectory1 = false;
			if(!bool.TryParse(isDirectory, out isDirectory1))
				isDirectory1 = false;
			bool isSecurePage1 = false;
			if(!bool.TryParse(isSecurePage, out isSecurePage1))
				isSecurePage1 = false;
			bool forceNoSecurePage1 = false;
			if(!bool.TryParse(forceNoSecurePage, out forceNoSecurePage1))
				forceNoSecurePage1 = false;
			RouteInfo info = new RouteInfo(name, location, destinationPage, path, isCompiled, actionName, isDirectory1, isSecurePage1, forceNoSecurePage1);
			string loc = "";
			if(!string.IsNullOrEmpty(info.Location) && info.Location[0] == '~')
				loc = info.Location.Substring(1);
			else
				loc = info.Location;
			if(!string.IsNullOrEmpty(loc) && loc[0] == '/')
				loc = loc.Substring(1);
			loc = loc.ToLower();
			var complexParts = Regex.Matches(loc, "{(?<part>[0-9]*?)}", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
			if(complexParts.Count > 0)
			{
				var complexRegex = loc;
				if(complexParts.Count == 1 && complexParts[0].Index == 0 && complexParts[0].Length == complexRegex.Length)
					complexRegex = "((?<complex" + complexParts[0].Groups["part"].Value + ">[\\s\\S]*?)/)|(?<complex" + complexParts[0].Groups["part"].Value + ">[\\s\\S]*)";
				else
				{
					foreach (Match i in complexParts)
					{
						if((i.Index + i.Length == loc.Length))
							complexRegex = complexRegex.Replace(i.Value, "(((?<complex" + i.Groups["part"].Value + ">[\\s\\S]*?)/)|(?<complex" + i.Groups["part"].Value + ">[\\s\\S]*))");
						else
							complexRegex = complexRegex.Replace(i.Value, "(?<complex" + i.Groups["part"].Value + ">[\\s\\S]*?)");
					}
				}
				if(!_complexRoutesNavigate.ContainsKey(complexRegex))
				{
					RouteInfoNavigate rNavAux = new RouteInfoNavigate(loc, null, info);
					rNavAux.ComplexPartsCount = complexParts.Count;
					rNavAux.ComplexRegex = complexRegex;
					_complexRoutesNavigate.Add(complexRegex, rNavAux);
				}
			}
			else if(!_routesNavigate.ContainsKey(loc))
			{
				RouteInfoNavigate rNavAux = new RouteInfoNavigate(loc, null, info);
				_routesNavigate.Add(loc, rNavAux);
			}
		}

		/// <summary>
		/// Adiciona uma nova ação que irá controlar as rotas do sistema.
		/// </summary>
		/// <param name="name">Nome da ação.</param>
		/// <param name="classType">Tipo da classe que representa a ação.</param>
		[Configuration("Route" + "/Action", ConfigKeyPresence.Optional)]
		public static void AddRouteAction(string name, string classType)
		{
			Type cType = Type.GetType(classType);
			if(cType == null)
				throw new Exception(string.Format("RouteAction\r\nType {0} not found", classType));
			RouteAction action = null;
			try
			{
				action = (RouteAction)Activator.CreateInstance(cType);
			}
			catch(Exception ex)
			{
				throw new Exception("Fail on create RouteAction by type " + classType, ex);
			}
			lock (_routeActions)
			{
				if(_routeActions.ContainsKey(name))
					_routeActions[name] = action;
				else
					_routeActions.Add(name, action);
			}
		}

		/// <summary>
		/// Registra as informações do cache.
		/// </summary>
		/// <param name="maximumItems"></param>
		[Configuration("Route" + "/Info", ConfigKeyPresence.Mandatory)]
		public static void Cache(int maximumItems)
		{
			_cacheMaximumItems = maximumItems;
		}

		/// <summary>
		/// Carraga os dados do roteamento.
		/// </summary>
		/// <returns>True se os dados foram recarregados.</returns>
		public static bool LoadConfiguration()
		{
			bool needLoad = false;
			if(!loadDataConfig)
			{
				loadDataConfig = true;
				needLoad = true;
				Configurator.AddSectionHandler("Colosoft.Route");
				Configurator.AddFileHandler("Route.config");
			}
			else if(Configurator.RefreshHandlers(f => f is RouteSectionHandler || f is Configuration.Handlers.FileConfigHandler))
			{
				needLoad = true;
			}
			if(needLoad)
			{
				lock (syncObject)
				{
					_routesInfo.Clear();
					_routesNavigate.Clear();
					_complexRoutesNavigate.Clear();
					try
					{
						Configurator.Configure(typeof(RouteSettings));
						return true;
					}
					catch(Exception ex)
					{
						if(ex is TargetInvocationException)
							ex = ex.InnerException;
						throw new Colosoft.Configuration.Exceptions.LoadConfigurationException(ex);
					}
				}
			}
			return false;
		}
	}
}
