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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.UI
{
	/// <summary>
	/// Implementação base se um Widget.
	/// </summary>
	public abstract class WidgetBase : IWidget, IScriptableComponent
	{
		internal static readonly string DeferredScriptsKey = "$DeferredScriptsKey$";

		private static readonly Regex UnicodeEntityExpression = new Regex(@"\\+u(\d+)\\*#(\d+;)", RegexOptions.Compiled);

		private string _name;

		/// <summary>
		/// Inicializador javascript.
		/// </summary>
		public IJavaScriptInitializer Initializer
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se está auto inicializada.
		/// </summary>
		public bool IsSelfInitialized
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se está em um template de cliente.
		/// </summary>
		public bool IsInClientTemplate
		{
			get;
			private set;
		}

		/// <summary>
		/// Identifica se a initialização foi deferida.
		/// </summary>
		public bool HasDeferredInitialization
		{
			get;
			set;
		}

		/// <summary>
		/// Seletor.
		/// </summary>
		public string Selector
		{
			get
			{
				return (IsInClientTemplate ? "\\#" : "#") + Id;
			}
		}

		/// <summary>
		/// Gets the client events of the grid.
		/// </summary>
		/// <value>The client events.</value>
		public IDictionary<string, object> Events
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public string Id
		{
			get
			{
				return this.SanitizeId(HtmlAttributes.ContainsKey("id") ? (string)HtmlAttributes["id"] : Name);
			}
		}

		/// <summary>
		/// Metadados da model.
		/// </summary>
		public ModelMetadata ModelMetadata
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the HTML attributes.
		/// </summary>
		/// <value>The HTML attributes.</value>
		public IDictionary<string, object> HtmlAttributes
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the view context to rendering a view.
		/// </summary>
		/// <value>The view context.</value>
		public ViewContext ViewContext
		{
			get;
			private set;
		}

		/// <summary>
		/// Dados da view.
		/// </summary>
		public ViewDataDictionary ViewData
		{
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WidgetBase"/> class.
		/// </summary>
		/// <param name="viewContext">The view context.</param>
		/// <param name="viewData"></param>
		protected WidgetBase(ViewContext viewContext, ViewDataDictionary viewData = null)
		{
			ViewContext = viewContext;
			ViewData = viewData ?? viewContext.ViewData;
			HtmlAttributes = new RouteValueDictionary();
			IsSelfInitialized = true;
			Events = new Dictionary<string, object>();
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="viewContext"></param>
		/// <param name="initializer"></param>
		/// <param name="viewData"></param>
		protected WidgetBase(ViewContext viewContext, IJavaScriptInitializer initializer, ViewDataDictionary viewData = null) : this(viewContext, viewData)
		{
			Initializer = initializer;
		}

		/// <summary>
		/// Renders the component.
		/// </summary>
		public void Render()
		{
			using (var textWriter = new System.Web.UI.HtmlTextWriter(ViewContext.Writer))
			{
				WriteHtml(textWriter);
			}
		}

		/// <summary>
		/// Writes the initialization script.
		/// </summary>
		/// <param name="writer">The writer.</param>
		public virtual void WriteInitializationScript(System.IO.TextWriter writer)
		{
		}

		/// <summary>
		/// Verifica as configurações.
		/// </summary>
		public virtual void VerifySettings()
		{
			if(string.IsNullOrEmpty(Name))
				throw new InvalidOperationException(Colosoft.Web.Mvc.Properties.Resources.NameCannotBeBlank);
			if(!Name.Contains("<#=") && Name.IndexOf(" ") != -1)
			{
				throw new InvalidOperationException(Colosoft.Web.Mvc.Properties.Resources.NameCannotContainSpaces);
			}
			this.ThrowIfClassIsPresent("k-" + GetType().Name.ToLowerInvariant() + "-rtl", Colosoft.Web.Mvc.Properties.Resources.Rtl);
		}

		/// <summary>
		/// Converte para string com o HTML.
		/// </summary>
		/// <returns></returns>
		public string ToHtmlString()
		{
			using (var output = new System.IO.StringWriter())
			{
				WriteHtml(new System.Web.UI.HtmlTextWriter(output));
				return output.ToString();
			}
		}

		/// <summary>
		/// Converte para o template do cliente.
		/// </summary>
		/// <returns></returns>
		public MvcHtmlString ToClientTemplate()
		{
			IsInClientTemplate = true;
			var html = ToHtmlString().Replace("</script>", "<\\/script>");
			if(System.Web.Util.HttpEncoder.Current != null && System.Web.Util.HttpEncoder.Current.GetType().ToString().Contains("AntiXssEncoder"))
			{
				html = Regex.Replace(html, "\\u0026", "&", RegexOptions.IgnoreCase);
				html = Regex.Replace(html, "%23", "#", RegexOptions.IgnoreCase);
				html = Regex.Replace(html, "%3D", "=", RegexOptions.IgnoreCase);
				html = Regex.Replace(html, "&#32;", " ", RegexOptions.IgnoreCase);
				html = Regex.Replace(html, @"\\u0026#32;", " ", RegexOptions.IgnoreCase);
			}
			html = UnicodeEntityExpression.Replace(html, m =>  {
				return System.Web.HttpUtility.HtmlDecode(Regex.Unescape(@"\u" + m.Groups[1].Value + "#" + m.Groups[2].Value));
			});
			html = System.Web.HttpUtility.HtmlDecode(html);
			return MvcHtmlString.Create(html);
		}

		/// <summary>
		/// Writes the HTML.
		/// </summary>
		protected virtual void WriteHtml(System.Web.UI.HtmlTextWriter writer)
		{
			VerifySettings();
			if(IsSelfInitialized)
			{
				if(HasDeferredInitialization)
				{
					WriteDeferredScriptInitialization();
				}
				else
				{
					writer.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Script);
					WriteInitializationScript(writer);
					writer.RenderEndTag();
				}
			}
		}

		/// <summary>
		/// Escreve o script de inicialização.
		/// </summary>
		protected virtual void WriteDeferredScriptInitialization()
		{
			var scripts = new System.IO.StringWriter();
			WriteInitializationScript(scripts);
			AppendScriptToContext(scripts.ToString());
		}

		/// <summary>
		/// Anexa o script para o contexto.
		/// </summary>
		/// <param name="script"></param>
		private void AppendScriptToContext(string script)
		{
			var items = ViewContext.HttpContext.Items;
			var scripts = new System.Collections.Specialized.OrderedDictionary();
			if(items.Contains(DeferredScriptsKey))
				scripts = (System.Collections.Specialized.OrderedDictionary)items[DeferredScriptsKey];
			else
				items[DeferredScriptsKey] = scripts;
			scripts[Name] = script;
		}
	}
}
