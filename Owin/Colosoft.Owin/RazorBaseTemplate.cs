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

namespace Colosoft.Owin.Razor
{
	/// <summary>
	/// RazorBaseTemplate.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RazorBaseTemplate<T> : RazorEngine.Templating.TemplateBase<T>, IRazorBaseTemplate
	{
		private System.Web.Hosting.VirtualPathProvider _pathProvider;

		private ViewContext _viewContext;

		/// <summary>
		/// Url.
		/// </summary>
		public UrlHelper Url
		{
			get;
			set;
		}

		/// <summary>
		/// Html.
		/// </summary>
		public HtmlHelper<object> Html
		{
			get;
			set;
		}

		/// <summary>
		/// Contexto associado.
		/// </summary>
		public System.Web.HttpContextBase Context
		{
			get
			{
				if(_viewContext != null)
					return _viewContext.HttpContext;
				return null;
			}
		}

		/// <summary>
		/// ViewContext.
		/// </summary>
		public ViewContext ViewContext
		{
			get
			{
				return _viewContext;
			}
		}

		/// <summary>
		/// Configura o template.
		/// </summary>
		/// <param name="pathProvider"></param>
		public void Configure(System.Web.Hosting.VirtualPathProvider pathProvider)
		{
			_pathProvider = pathProvider;
		}

		/// <summary>
		/// Resolve o layout.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected override RazorEngine.Templating.ITemplate ResolveLayout(string name)
		{
			var layout = base.ResolveLayout(name);
			if(layout is IRazorBaseTemplate)
			{
				var layout2 = (IRazorBaseTemplate)layout;
				layout2.Url = Url;
				layout2.Html = Html;
			}
			return layout;
		}

		/// <summary>
		/// Define os dados para template.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="viewbag"></param>
		public override void SetData(object model, RazorEngine.Templating.DynamicViewBag viewbag)
		{
			_viewContext = viewbag is CustomDynamicViewBag ? ((CustomDynamicViewBag)viewbag).ViewContext : null;
			if(_viewContext != null)
			{
				Url = new UrlHelper(_viewContext.RequestContext, System.Web.Routing.RouteTable.Routes);
				Html = new HtmlHelper<object>(_viewContext, new ViewDataContainer(_viewContext));
			}
			base.SetData(model, viewbag);
		}
	}
}
