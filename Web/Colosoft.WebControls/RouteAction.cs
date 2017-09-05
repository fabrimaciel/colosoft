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

namespace Colosoft.WebControls.Route
{
	/// <summary>
	/// Dados da resposta de uma ação do rota.
	/// </summary>
	public class RouteActionResponse
	{
		private bool _compiled = true;

		/// <summary>
		/// Caminho para onde a rota será redirecionada.
		/// </summary>
		public string RedirectVirtualPath
		{
			get;
			private set;
		}

		/// <summary>
		/// Identifica se o redirecionamento é compilado ou não.
		/// </summary>
		public bool Compiled
		{
			get
			{
				return _compiled;
			}
			private set
			{
				_compiled = value;
			}
		}

		/// <summary>
		/// Novo manipulador que será usado para a rota.
		/// </summary>
		public System.Web.IHttpHandler RouteHandler
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public RouteActionResponse()
		{
		}

		/// <summary>
		/// Resposta informanado o nome manipulador.
		/// </summary>
		/// <param name="routeHandler">Novo manipulador que será usado para a rota.</param>
		public RouteActionResponse(System.Web.IHttpHandler routeHandler)
		{
			RouteHandler = routeHandler;
		}

		/// <summary>
		/// Resposta com redirecionamento.
		/// </summary>
		/// <param name="redirectVirtualPath">Caminho para onde a rota será redirecionada.</param>
		/// <param name="compiled">Identifica se o redirecionamento é compilado ou não.</param>
		public RouteActionResponse(string redirectVirtualPath, bool compiled)
		{
			RedirectVirtualPath = redirectVirtualPath;
			_compiled = compiled;
		}
	}
	/// <summary>
	/// Dados da requição da ação da rota.
	/// </summary>
	public class RouteActionRequest
	{
		/// <summary>
		/// Caminho de destino da rota.
		/// </summary>
		public string DestinationVirtualPath
		{
			get;
			internal set;
		}

		/// <summary>
		/// QueryString original da requisição.
		/// </summary>
		public System.Collections.Specialized.NameValueCollection OriginalQueryString
		{
			get;
			internal set;
		}

		/// <summary>
		/// Parametros do roteamento.
		/// </summary>
		public System.Collections.Specialized.NameValueCollection RouteParameters
		{
			get;
			internal set;
		}

		/// <summary>
		/// Contexto da requisição.
		/// </summary>
		public System.Web.HttpContext Context
		{
			get;
			internal set;
		}
	}
	/// <summary>
	/// Dados da requição da ação da rota.
	/// </summary>
	public class RouteActionBeginRequest
	{
		/// <summary>
		/// Contexto da requisição.
		/// </summary>
		public System.Web.HttpContext Context
		{
			get;
			internal set;
		}
	}
	/// <summary>
	/// Dados da resposta da requisição.
	/// </summary>
	public class RouteActionBeginResponse
	{
		/// <summary>
		/// Aplica as alterações.
		/// </summary>
		public bool ApplyChanges
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica que a página exige segurança.
		/// </summary>
		public bool IsSecurePage
		{
			get;
			set;
		}

		/// <summary>
		/// Força a rota não atender páginas de segurança.
		/// </summary>
		public bool ForceNoSecurePage
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Classe responsável processamento da ação relacionada com a rota.
	/// </summary>
	public abstract class RouteAction
	{
		/// <summary>
		/// Método acionado quando a rota é inicialmente requisitada.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		internal RouteActionBeginResponse BeginRequest(RouteInfo route, RouteActionBeginRequest request)
		{
			return OnBeginRequest(route, request);
		}

		/// <summary>
		/// Método acionado para o pré-carregamento da rota.
		/// </summary>
		/// <param name="route"></param>
		/// <returns></returns>
		internal RouteActionResponse PreLoad(RouteInfo route, RouteActionRequest request)
		{
			return OnPreLoad(route, request);
		}

		/// <summary>
		/// Método acionado para o carregamento da rota.
		/// </summary>
		/// <param name="route"></param>
		internal void Load(RouteInfo route)
		{
			OnLoad(route);
		}

		/// <summary>
		/// Método acionado quando a rota é inicialmente requisitada.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		protected virtual RouteActionBeginResponse OnBeginRequest(RouteInfo route, RouteActionBeginRequest request)
		{
			return new RouteActionBeginResponse();
		}

		/// <summary>
		/// Método acionado no pré-processamento da rota.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="request">Dados que estão sendo requisitados.</param>
		/// <returns></returns>
		protected virtual RouteActionResponse OnPreLoad(RouteInfo route, RouteActionRequest request)
		{
			return new RouteActionResponse();
		}

		/// <summary>
		/// Método acionado quando a rota for carregada.
		/// </summary>
		/// <param name="route"></param>
		protected virtual void OnLoad(RouteInfo route)
		{
		}
	}
}
