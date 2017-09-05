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
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Colosoft.Owin.Razor
{
	/// <summary>
	/// Implementação do motor de view para Razor.
	/// </summary>
	class RazorViewEngine : VirtualPathProviderViewEngine
	{
		private RazorEngine.Templating.IRazorEngineService _engineService;

		private IDictionary<string, IView> _partialViews = new Dictionary<string, IView>(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// Instancia do serviço do motor o Razor.
		/// </summary>
		public RazorEngine.Templating.IRazorEngineService EngineService
		{
			get
			{
				return _engineService;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// <param name="templateServiceConfiguration">Configuração do serviço</param>
		/// </summary>
		public RazorViewEngine(RazorEngine.Configuration.ITemplateServiceConfiguration templateServiceConfiguration)
		{
			templateServiceConfiguration.Require("templateServiceConfiguration").NotNull();
			base.AreaViewLocationFormats = new string[] {
				"~/Areas/{2}/Views/{1}/{0}.cshtml",
				"~/Areas/{2}/Views/{1}/{0}.vbhtml",
				"~/Areas/{2}/Views/Shared/{0}.cshtml",
				"~/Areas/{2}/Views/Shared/{0}.vbhtml"
			};
			base.AreaMasterLocationFormats = new string[] {
				"~/Areas/{2}/Views/{1}/{0}.cshtml",
				"~/Areas/{2}/Views/{1}/{0}.vbhtml",
				"~/Areas/{2}/Views/Shared/{0}.cshtml",
				"~/Areas/{2}/Views/Shared/{0}.vbhtml"
			};
			base.AreaPartialViewLocationFormats = new string[] {
				"~/Areas/{2}/Views/{1}/{0}.cshtml",
				"~/Areas/{2}/Views/{1}/{0}.vbhtml",
				"~/Areas/{2}/Views/Shared/{0}.cshtml",
				"~/Areas/{2}/Views/Shared/{0}.vbhtml"
			};
			base.ViewLocationFormats = new string[] {
				"~/Views/{1}/{0}.cshtml",
				"~/Views/{1}/{0}.vbhtml",
				"~/Views/Shared/{0}.cshtml",
				"~/Views/Shared/{0}.vbhtml"
			};
			base.MasterLocationFormats = new string[] {
				"~/Views/{1}/{0}.cshtml",
				"~/Views/{1}/{0}.vbhtml",
				"~/Views/Shared/{0}.cshtml",
				"~/Views/Shared/{0}.vbhtml"
			};
			base.PartialViewLocationFormats = new string[] {
				"~/Views/{1}/{0}.cshtml",
				"~/Views/{1}/{0}.vbhtml",
				"~/Views/Shared/{0}.cshtml",
				"~/Views/Shared/{0}.vbhtml"
			};
			base.FileExtensions = new string[] {
				"cshtml",
				"vbhtml"
			};
			_engineService = CreateRazorEngineService(templateServiceConfiguration);
		}

		/// <summary>
		/// Recupera a configuração padrão.
		/// </summary>
		/// <param name="templateGetKey">Referencia do método para recupera a chave do modelo.</param>
		/// <returns></returns>
		public static RazorEngine.Configuration.ITemplateServiceConfiguration GetDefaultConfiguration(TemplateGetKeyHandle templateGetKey)
		{
			templateGetKey.Require("templateGetKey").NotNull();
			var configuration = new RazorEngine.Configuration.TemplateServiceConfiguration {
				TemplateManager = new RazorTemplateManager(templateGetKey),
				CachingProvider = new RazorEngine.Templating.InvalidatingCachingProvider(),
				Activator = new RazorActivator(),
				BaseTemplateType = typeof(RazorBaseTemplate<>),
				EncodedStringFactory = new RazorEngine.Text.RawStringFactory(),
				Debug = true
			};
			configuration.Namespaces.Add("System.Web.Helpers");
			configuration.Namespaces.Add("System.Web.Mvc");
			return configuration;
		}

		/// <summary>
		/// Recupera a chave do modelo.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="resolveType"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		private RazorEngine.Templating.ITemplateKey GetKey(string name, RazorEngine.Templating.ResolveType resolveType = 0, RazorEngine.Templating.ITemplateKey context = null)
		{
			return EngineService.GetKey(name, resolveType, context);
		}

		/// <summary>
		/// Recupera a instancia do serviço de template.
		/// </summary>
		/// <returns></returns>
		private RazorEngine.Templating.IRazorEngineService CreateRazorEngineService(RazorEngine.Configuration.ITemplateServiceConfiguration configuration)
		{
			return RazorEngine.Templating.RazorEngineService.Create(configuration);
		}

		/// <summary>
		/// Localiza a view.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="viewName"></param>
		/// <param name="masterName"></param>
		/// <param name="useCache"></param>
		/// <returns></returns>
		public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			return base.FindView(controllerContext, viewName, masterName, useCache);
		}

		/// <summary>
		/// Cria a parte de uma view.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="partialPath"></param>
		/// <returns></returns>
		protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
		{
			IView result = null;
			var exists = false;
			lock (_partialViews)
				exists = _partialViews.TryGetValue(partialPath, out result);
			if(exists)
				return result;
			var file = VirtualPathProvider.GetFile(partialPath);
			IView result2 = null;
			if(file != null)
				result2 = new RazorPartialView(file, _engineService);
			lock (_partialViews)
				if(!_partialViews.TryGetValue(partialPath, out result))
					_partialViews.Add(partialPath, result = result2);
			return result;
		}

		/// <summary>
		/// Cria a view.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="viewPath"></param>
		/// <param name="masterPath"></param>
		/// <returns></returns>
		protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
		{
			var file = VirtualPathProvider.GetFile(viewPath);
			if(file != null)
				return new RazorView(file, null, EngineService);
			return null;
		}
	}
}
