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
using System.Web;
using System.Web.Mvc;
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.UI
{
	/// <summary>
	/// View component Builder base class.
	/// </summary>
	public abstract class WidgetBuilderBase<TViewComponent, TBuilder> : IHtmlString where TViewComponent : WidgetBase where TBuilder : WidgetBuilderBase<TViewComponent, TBuilder>
	{
		private TViewComponent component;

		/// <summary>
		/// Initializes a new instance of the <see cref="WidgetBuilderBase&lt;TViewComponent, TBuilder&gt;"/> class.
		/// </summary>
		/// <param name="component">The component.</param>
		public WidgetBuilderBase(TViewComponent component)
		{
			this.component = component;
		}

		/// <summary>
		/// Gets the view component.
		/// </summary>
		/// <value>The component.</value>
		protected internal TViewComponent Component
		{
			get
			{
				return component;
			}
			set
			{
				component = value;
			}
		}

		/// <summary>
		/// Sobrecarga de operação implicita.
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static implicit operator TViewComponent(WidgetBuilderBase<TViewComponent, TBuilder> builder)
		{
			return builder.ToComponent();
		}

		/// <summary>
		/// Returns the internal view component.
		/// </summary>
		/// <returns></returns>
		public TViewComponent ToComponent()
		{
			return Component;
		}

		/// <summary>
		/// Sets the name of the component.
		/// </summary>
		/// <param name="componentName">The name.</param>
		/// <returns></returns>
		public virtual TBuilder Name(string componentName)
		{
			Component.Name = componentName;
			return this as TBuilder;
		}

		/// <summary>
		/// Suppress initialization script rendering. Note that this options should be used in conjunction with WidgetFactory.DeferredScripts
		/// </summary>        
		/// <returns></returns>
		public virtual TBuilder Deferred(bool deferred = true)
		{
			Component.HasDeferredInitialization = deferred;
			return this as TBuilder;
		}

		/// <summary>
		/// Carrega os metadados do modelo.
		/// </summary>
		/// <param name="modelMetadata"></param>
		/// <returns></returns>
		public TBuilder ModelMetadata(ModelMetadata modelMetadata)
		{
			Component.ModelMetadata = modelMetadata;
			return this as TBuilder;
		}

		/// <summary>
		/// Sets the HTML attributes.
		/// </summary>
		/// <param name="attributes">The HTML attributes.</param>
		/// <returns></returns>
		public virtual TBuilder HtmlAttributes(object attributes)
		{
			return HtmlAttributes(attributes.ToDictionary());
		}

		/// <summary>
		/// Sets the HTML attributes.
		/// </summary>
		/// <param name="attributes">The HTML attributes.</param>
		/// <returns></returns>
		public virtual TBuilder HtmlAttributes(IDictionary<string, object> attributes)
		{
			Component.HtmlAttributes.Clear();
			Component.HtmlAttributes.Merge(attributes);
			return this as TBuilder;
		}

		/// <summary>
		/// Renders the component.
		/// </summary>
		public virtual void Render()
		{
			Component.Render();
		}

		/// <summary>
		/// Recupera o html.
		/// </summary>
		/// <returns></returns>
		public virtual string ToHtmlString()
		{
			return ToComponent().ToHtmlString();
		}

		/// <summary>
		/// Recupera o template do cliente.
		/// </summary>
		/// <returns></returns>
		public virtual MvcHtmlString ToClientTemplate()
		{
			return ToComponent().ToClientTemplate();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToHtmlString();
		}
	}
}
