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
using Colosoft;

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Implementação base do manipulador de requisições.
	/// </summary>
	abstract class RequestHandlerBase
	{
		/// <summary>
		/// Versão do MVC.
		/// </summary>
		internal static readonly string MvcVersion = GetMvcVersionString();

		/// <summary>
		/// Nome do cabeçalho.
		/// </summary>
		public static readonly string MvcVersionHeaderName = "X-AspNetMvc-Version";

		/// <summary>
		/// Variáveis de ambiente.
		/// </summary>
		protected IDictionary<string, object> Environment
		{
			get;
			private set;
		}

		/// <summary>
		/// Rotas.
		/// </summary>
		protected System.Web.Routing.RouteCollection Routes
		{
			get
			{
				return System.Web.Routing.RouteTable.Routes;
			}
		}

		/// <summary>
		/// Caminho da requisição.
		/// </summary>
		protected string RequestPath
		{
			get
			{
				return (string)this.Environment["owin.RequestPath"];
			}
			set
			{
				this.Environment["owin.RequestPath"] = value;
			}
		}

		/// <summary>
		/// Identifica se é para desabilita a inclusão do cabeçalho MVC.
		/// </summary>
		public static bool DisableMvcResponseHeader
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="env"></param>
		public RequestHandlerBase(IDictionary<string, object> env)
		{
			Environment = env;
			InitResponseType();
		}

		/// <summary>
		/// Recupera o texto com a versão do MVC.
		/// </summary>
		/// <returns></returns>
		private static string GetMvcVersionString()
		{
			return new System.Reflection.AssemblyName(typeof(System.Web.Mvc.MvcHandler).Assembly.FullName).Version.ToString(2);
		}

		/// <summary>
		/// Inicializa o tipo de resposta.
		/// </summary>
		private void InitResponseType()
		{
			var header = (IDictionary<string, string[]>)this.Environment["owin.ResponseHeaders"];
			header.Add("Content-Type", new[] {
				"text/html"
			});
		}

		/// <summary>
		/// Processa a requisição.
		/// </summary>
		/// <returns></returns>
		public abstract Task<object> Handle();

		/// <summary>
		/// Adiciona para o cabeçalho a versão.
		/// </summary>
		/// <param name="httpContext"></param>
		protected internal virtual void AddVersionHeader(System.Web.HttpContextBase httpContext)
		{
			if(!DisableMvcResponseHeader)
				httpContext.Response.AppendHeader(MvcVersionHeaderName, MvcVersion);
		}

		/// <summary>
		/// Recupera o caminho da view.
		/// </summary>
		/// <param name="controller">Nome do controlador.</param>
		/// <param name="viewName"></param>
		/// <returns></returns>
		protected string GetViewPath(string controller, string viewName)
		{
			return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "View", controller, viewName + ".cshtml");
		}

		/// <summary>
		/// Registra a resposta.
		/// </summary>
		/// <param name="viewPath"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		protected async Task WriteResponse(string viewPath, object model)
		{
			if(!System.IO.File.Exists(viewPath))
				throw new Exception("View not found. Path: " + viewPath);
			using (var writer = new System.IO.StreamWriter((System.IO.Stream)this.Environment["owin.ResponseBody"]))
			{
				await writer.WriteAsync(RazorEngine.Razor.Parse(new System.IO.StreamReader(viewPath).ReadToEnd(), model));
			}
		}

		/// <summary>
		/// Recupera a rota pelo nome informado.
		/// </summary>
		/// <param name="httpContext">Contexto.</param>
		/// <returns></returns>
		protected System.Web.Routing.RouteData GetRoute(System.Web.HttpContextBase httpContext)
		{
			try
			{
				return Routes.GetRouteData(httpContext);
			}
			catch(Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Cria o controller.
		/// </summary>
		/// <param name="controllerType"></param>
		/// <returns></returns>
		protected System.Web.Mvc.IController CreateController(Type controllerType)
		{
			return (System.Web.Mvc.IController)Activator.CreateInstance(controllerType);
		}
	}
}
