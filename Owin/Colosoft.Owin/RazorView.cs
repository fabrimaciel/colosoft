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
	/// Implementação da View do Razor.
	/// </summary>
	class RazorView : IView
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
		/// <param name="model"></param>
		/// <param name="engineService"></param>
		public RazorView(System.Web.Hosting.VirtualFile virtualFile, object model, RazorEngine.Templating.IRazorEngineService engineService)
		{
			_virtualFile = virtualFile;
			_model = model;
			_engineService = engineService;
		}

		/// <summary>
		/// Renderiza o conteúdo.
		/// </summary>
		/// <param name="viewContext"></param>
		/// <param name="writer"></param>
		public void Render(ViewContext viewContext, System.IO.TextWriter writer)
		{
			if(_virtualFile.Name.EndsWith(".cshtml", StringComparison.InvariantCultureIgnoreCase) || _virtualFile.Name.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase))
			{
				try
				{
					var cache = _virtualFile as ITemplateCacheSupport;
					RazorEngine.Templating.ITemplateKey templateKey = null;
					if(cache != null)
					{
						if(cache.TemplateKey == null)
						{
							templateKey = EngineService.GetKey(_virtualFile.VirtualPath.ToLower(), RazorEngine.Templating.ResolveType.Global);
							cache.Register(templateKey);
						}
						else
							templateKey = cache.TemplateKey;
					}
					else
						templateKey = EngineService.GetKey(_virtualFile.VirtualPath.ToLower(), RazorEngine.Templating.ResolveType.Global);
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
					RazorEngine.Templating.DynamicViewBag viewBag = new CustomDynamicViewBag(viewContext.ViewData) {
						ViewContext = viewContext
					};
					if(string.IsNullOrEmpty(viewContext.RequestContext.HttpContext.Response.ContentType))
					{
						string contentType = null;
						if(!string.IsNullOrEmpty(contentType))
						{
							var path = _virtualFile.VirtualPath;
							var index = path.LastIndexOf('.');
							if(index >= 0)
							{
								var extension = path.Substring(index);
								contentType = Colosoft.Web.ExtendedHtmlUtility.TranslateContentType(extension);
							}
						}
						viewContext.RequestContext.HttpContext.Response.ContentType = contentType;
					}
					EngineService.Run(templateKey, writer, modelType, _model, viewBag);
					writer.Flush();
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
		public void Render(ViewContext viewContext, System.IO.Stream outputStream)
		{
			using (var stream = _virtualFile.Open())
			{
				if(string.IsNullOrEmpty(viewContext.RequestContext.HttpContext.Response.ContentType))
				{
					string contentType = null;
					if(!string.IsNullOrEmpty(contentType))
					{
						var path = _virtualFile.VirtualPath;
						var index = path.LastIndexOf('.');
						if(index >= 0)
						{
							var extension = path.Substring(index);
							contentType = Colosoft.Web.ExtendedHtmlUtility.TranslateContentType(extension);
						}
					}
					viewContext.RequestContext.HttpContext.Response.ContentType = contentType;
				}
				var buffer = new byte[1024];
				var read = 0;
				while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
					outputStream.Write(buffer, 0, read);
				outputStream.Flush();
			}
		}

		/// <summary>
		/// Recupera o membro dinamico.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="memberName"></param>
		/// <returns></returns>
		internal static object GetDynamicMember(object obj, string memberName)
		{
			var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.None, memberName, obj.GetType(), new[] {
				Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags.None, null)
			});
			var callsite = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(binder);
			return callsite.Target(callsite, obj);
		}

		/// <summary>
		/// Converte o objeto para um Expando.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		internal static ExpandoObject ToExpando(object obj)
		{
			IDictionary<string, object> expandoObject = new ExpandoObject();
			var dic = new System.Web.Routing.RouteValueDictionary(obj);
			foreach (var o in dic)
			{
				expandoObject.Add(o.Key, o.Value == null || new[] {
					typeof(Enum),
					typeof(String),
					typeof(Char),
					typeof(Guid),
					typeof(Boolean),
					typeof(Byte),
					typeof(Int16),
					typeof(Int32),
					typeof(Int64),
					typeof(Single),
					typeof(Double),
					typeof(Decimal),
					typeof(SByte),
					typeof(UInt16),
					typeof(UInt32),
					typeof(UInt64),
					typeof(DateTime),
					typeof(DateTimeOffset),
					typeof(TimeSpan),
				}.Any(oo => oo.IsInstanceOfType(o.Value)) ? o.Value : ToExpando(o.Value));
			}
			return (ExpandoObject)expandoObject;
		}
	}
}
