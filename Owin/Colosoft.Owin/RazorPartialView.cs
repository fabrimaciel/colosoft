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

namespace Colosoft.Owin.Razor
{
	/// <summary>
	/// Implementação da View parcia.
	/// </summary>
	class RazorPartialView : System.Web.Mvc.IView
	{
		private System.Web.Hosting.VirtualFile _virtualFile;

		private object _model;

		private RazorEngine.Templating.IRazorEngineService _engineService;

		/// <summary>
		/// Serviço de template.
		/// </summary>
		private RazorEngine.Templating.IRazorEngineService EngineService
		{
			get
			{
				return _engineService;
			}
		}

		/// <summary>
		/// Identifica se o arquivo associado é um arquivo que precisa ser renderizado pelo Razor.
		/// </summary>
		public bool IsRazorFile
		{
			get
			{
				return _virtualFile.Name.EndsWith(".cshtml", StringComparison.InvariantCultureIgnoreCase) || _virtualFile.Name.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="virtualFile"></param>
		/// <param name="engineService"></param>
		public RazorPartialView(System.Web.Hosting.VirtualFile virtualFile, RazorEngine.Templating.IRazorEngineService engineService)
		{
			_virtualFile = virtualFile;
			_engineService = engineService;
		}

		/// <summary>
		/// Renderiza o conteúdo.
		/// </summary>
		/// <param name="viewContext"></param>
		/// <param name="writer"></param>
		public void Render(System.Web.Mvc.ViewContext viewContext, System.IO.TextWriter writer)
		{
			if(IsRazorFile)
			{
				try
				{
					var cache = _virtualFile as ITemplateCacheSupport;
					RazorEngine.Templating.ITemplateKey templateKey = null;
					if(cache != null)
					{
						if(cache.TemplateKey == null)
						{
							templateKey = EngineService.GetKey(_virtualFile.VirtualPath.ToLower(), RazorEngine.Templating.ResolveType.Include);
							cache.Register(templateKey);
						}
						else
							templateKey = cache.TemplateKey;
					}
					else
						templateKey = EngineService.GetKey(_virtualFile.VirtualPath.ToLower(), RazorEngine.Templating.ResolveType.Include);
					RazorEngine.Templating.DynamicViewBag viewBag = new CustomDynamicViewBag(viewContext.ViewData) {
						ViewContext = viewContext
					};
					var modelType = _model != null ? _model.GetType() : null;
					if(!EngineService.IsTemplateCached(templateKey, modelType))
					{
						try
						{
							EngineService.Compile(templateKey, modelType);
						}
						catch(Exception ex)
						{
							viewContext.HttpContext.Response.StatusCode = 501;
							writer.Write(ex.ToString());
							return;
						}
					}
					EngineService.Run(templateKey, writer, modelType, null, viewBag);
					return;
				}
				catch(Exception ex)
				{
					viewContext.HttpContext.Response.StatusCode = 501;
					writer.Write(ex.ToString());
					return;
				}
			}
			using (var stream = _virtualFile.Open())
			{
				var reader = new System.IO.StreamReader(stream);
				writer.Write(reader.ReadToEnd());
				writer.Flush();
			}
		}

		/// <summary>
		/// Renderiza o conteúdo.
		/// </summary>
		/// <param name="viewContext"></param>
		/// <param name="outputStream"></param>
		public void Render(System.Web.Mvc.ViewContext viewContext, System.IO.Stream outputStream)
		{
			using (var stream = _virtualFile.Open())
			{
				var buffer = new byte[1024];
				var read = 0;
				while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
					outputStream.Write(buffer, 0, read);
				outputStream.Flush();
			}
		}
	}
}
