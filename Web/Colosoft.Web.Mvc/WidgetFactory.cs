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

using Colosoft.Web.Mvc.Infrastructure;
using Colosoft.Web.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Colosoft.Web.Mvc.UI
{
	/// <summary>
	/// Cria uma API fluent para o colosoft Widgets.
	/// </summary>
	public class WidgetFactory
	{
		/// <summary>
		/// Html helper associado.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public HtmlHelper HtmlHelper
		{
			get;
			set;
		}

		/// <summary>
		/// Inicializador Javascript.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IJavaScriptInitializer Initializer
		{
			get;
			private set;
		}

		/// <summary>
		/// Context de visualização.
		/// </summary>
		private ViewContext ViewContext
		{
			get
			{
				return HtmlHelper.ViewContext;
			}
		}

		/// <summary>
		/// Dados da view.
		/// </summary>
		private ViewDataDictionary ViewData
		{
			get
			{
				return HtmlHelper.ViewData;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="htmlHelper"></param>
		public WidgetFactory(HtmlHelper htmlHelper)
		{
			HtmlHelper = htmlHelper;
			Initializer = new JavaScriptInitializer();
		}

		/// <summary>
		/// Cria o builder de um enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public virtual EnumBuilder<T> Enum<T>()
		{
			return new EnumBuilder<T>(ViewContext, Initializer);
		}
	}
	/// <summary>
	/// Cria uma API fluent para o colosoft Widgets.
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public class WidgetFactory<TModel> : WidgetFactory
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="htmlHelper"></param>
		public WidgetFactory(HtmlHelper<TModel> htmlHelper) : base(htmlHelper)
		{
			this.HtmlHelper = htmlHelper;
		}
	}
}
