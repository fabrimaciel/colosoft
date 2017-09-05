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
using System.Text.RegularExpressions;

namespace Colosoft.WebControls.Route
{
	/// <summary>
	/// Armazena o resultado de carga do rota.
	/// </summary>
	public class LoadRouteResult
	{
		/// <summary>
		/// Armazena a localização da rota formatada com base os valores dos grupos.
		/// </summary>
		private string _formattedLocation;

		/// <summary>
		/// Caminho virtual que o resultado representa.
		/// </summary>
		public string VirtualPath
		{
			get;
			private set;
		}

		/// <summary>
		/// Informações da rota.
		/// </summary>
		public RouteInfoNavigate Navigate
		{
			get;
			private set;
		}

		/// <summary>
		/// Grupos de valores que foram extraída da rota complexa.
		/// </summary>
		public List<KeyValuePair<int, string>> ComplexValues
		{
			get;
			private set;
		}

		/// <summary>
		/// Armazena a localização da rota formatada com base os valores dos grupos.
		/// </summary>
		public string FormattedLocation
		{
			get
			{
				if(_formattedLocation == null)
				{
					_formattedLocation = Navigate.Info.Location;
					if(ComplexValues != null)
					{
						foreach (var i in ComplexValues)
							_formattedLocation = _formattedLocation.Replace("{" + i.Key + "}", i.Value);
					}
				}
				return _formattedLocation;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="virtualPath">Caminho virtual que o resultado representa.</param>
		/// <param name="info"></param>
		/// <param name="complexValues"></param>
		public LoadRouteResult(string virtualPath, RouteInfoNavigate info, List<KeyValuePair<int, string>> complexValues)
		{
			VirtualPath = virtualPath;
			Navigate = info;
			ComplexValues = complexValues;
		}
	}
	/// <summary>
	/// Gerencia o cache dos roteamentos.
	/// </summary>
	public class RouteCache
	{
		/// <summary>
		/// Matriz que identifica a quantidade de acessos ao roteamento. 
		/// </summary>
		private static int[] _accessRoute;

		/// <summary>
		/// Rotas já carregadas no cache.
		/// </summary>
		private static LoadRouteResult[] _routes;

		/// <summary>
		/// Recupera e define a instacia para a rota com base no caminho virtual.
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		public static LoadRouteResult GetItem(string virtualPath)
		{
			if(RouteSettings.LoadConfiguration() || _accessRoute == null)
			{
				_accessRoute = new int[RouteSettings.CacheMaximumItems];
				_routes = new LoadRouteResult[RouteSettings.CacheMaximumItems];
			}
			int pos = -1;
			for(int i = 0; i < _routes.Length; i++)
				if(pos < 0 && _routes[i] != null && _routes[i].VirtualPath == virtualPath)
				{
					_accessRoute[i]++;
					pos = i;
				}
				else
				{
					if(_accessRoute[i] > int.MinValue)
						_accessRoute[i]--;
				}
			if(pos >= 0)
				return _routes[pos];
			pos = 0;
			for(int i = 1; i < _accessRoute.Length; i++)
				if(_accessRoute[i] < _accessRoute[pos])
				{
					pos = i;
				}
			if(_routes[pos] != null)
			{
				GC.ReRegisterForFinalize(_routes[pos]);
			}
			_accessRoute[pos] = 10;
			_routes[pos] = LoadRoute(virtualPath);
			return _routes[pos];
		}

		/// <summary>
		/// Carrega uma informação de route com base no virtual path informado.
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		private static LoadRouteResult LoadRoute(string virtualPath)
		{
			try
			{
				string loc = "";
				if(!string.IsNullOrEmpty(virtualPath) && virtualPath[0] == '~')
					loc = virtualPath.Substring(1);
				else
					loc = virtualPath;
				if(!string.IsNullOrEmpty(loc) && loc[0] == '/')
					loc = loc.Substring(1);
				loc = loc.ToLower();
				if(RouteSettings.IgnoreRoutes.Count > 0 && RouteSettings.IgnoreRoutes.Exists(f => f.Length <= loc.Length && loc.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) == 0))
					return null;
				RouteInfoNavigate routeNavInfo = null;
				string findKey = null;
				foreach (var i in RouteSettings.RoutesNavigate.Keys)
				{
					if(loc.Length >= i.Length && loc.IndexOf(i, 0, i.Length, StringComparison.InvariantCultureIgnoreCase) == 0 && (findKey == null || i.Length > findKey.Length))
						findKey = i;
				}
				if(!string.IsNullOrEmpty(findKey))
					routeNavInfo = (RouteInfoNavigate)RouteSettings.RoutesNavigate[findKey];
				Match findMatch = null;
				RouteInfoNavigate findComplexNav = null;
				lock (RouteSettings.ComplexRoutesNavigate)
					foreach (KeyValuePair<string, RouteInfoNavigate> i in RouteSettings.ComplexRoutesNavigate)
					{
						var m = Regex.Match(loc, i.Key, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
						if(m.Success && m.Index == 0)
						{
							findComplexNav = i.Value;
							findMatch = m;
						}
					}
				if(routeNavInfo != null && findComplexNav != null)
				{
					var complexUrl = findComplexNav.Info.Location;
					var routeLocation = routeNavInfo.Info.Location;
					for(int i = 0; i < findComplexNav.ComplexPartsCount; i++)
						complexUrl = complexUrl.Replace("{" + i + "}", findMatch.Groups["complex" + i].Value);
					if(routeLocation.Length > 0 && routeLocation[routeLocation.Length - 1] != '/')
						routeLocation += '/';
					if(complexUrl.Length > 0 && complexUrl[complexUrl.Length - 1] != '/')
						complexUrl += '/';
					if(complexUrl.Length < routeNavInfo.Info.Location.Length || string.Compare(complexUrl, routeLocation, true) == 0)
						return new LoadRouteResult(virtualPath, routeNavInfo, null);
				}
				if(findComplexNav != null)
				{
					var groupValues = new List<KeyValuePair<int, string>>();
					if(findMatch != null)
						for(int i = 0; i < findComplexNav.ComplexPartsCount; i++)
							groupValues.Add(new KeyValuePair<int, string>(i, findMatch.Groups["complex" + i].Value));
					return new LoadRouteResult(virtualPath, findComplexNav, groupValues);
				}
				if(routeNavInfo != null)
					return new LoadRouteResult(virtualPath, routeNavInfo, null);
			}
			catch(Exception ex1)
			{
				throw new Exception(string.Format("Fail on process route to \"{0}\".", virtualPath), ex1);
			}
			return null;
		}
	}
}
