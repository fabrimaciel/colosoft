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

using Kendo.Mvc;
using Kendo.Mvc.Infrastructure;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Colosoft.Kendo.Mvc.UI
{
	/// <summary>
	/// Representa um componente da fonte de dados.
	/// </summary>
	public class DataSourceComponent : WidgetBase
	{
		private global::Kendo.Mvc.UI.DataSource _dataSource;

		/// <summary>
		/// Fonte de dados associada.
		/// </summary>
		public global::Kendo.Mvc.UI.DataSource DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				_dataSource = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="viewContext"></param>
		/// <param name="initializer"></param>
		public DataSourceComponent(global::Kendo.Mvc.UI.DataSource dataSource, ViewContext viewContext, IJavaScriptInitializer initializer) : base(viewContext, initializer)
		{
			_dataSource = dataSource;
		}

		/// <summary>
		/// Processa a fonte de dados.
		/// </summary>
		private void ProcessDataSource()
		{
			var binder = new DataSourceRequestModelBinder();
			var controller = ViewContext.Controller;
			var bindingContext = new ModelBindingContext() {
				ValueProvider = controller.ValueProvider
			};
			var request = (DataSourceRequest)binder.BindModel(controller.ControllerContext, bindingContext);
			DataSource.Process(request, false);
		}

		/// <summary>
		/// Escreve a inicialização javascript.
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteInitializationScript(System.IO.TextWriter writer)
		{
			if(string.IsNullOrEmpty(Name) || Name.Trim().Length == 0)
				writer.Write(new StringBuilder().Append("new kendo.data.DataSource").Append("(").Append(Initializer.Serialize(DataSource.ToJson())).Append(")").ToString());
			else
				writer.Write(new StringBuilder().Append("var ").Append(Name).Append(" = new kendo.data.DataSource").Append("(").Append(Initializer.Serialize(DataSource.ToJson())).Append(");").ToString());
		}

		/// <summary>
		/// Escreve o html da componente.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteHtml(System.Web.UI.HtmlTextWriter writer)
		{
			if(DataSource.Type != DataSourceType.Custom || DataSource.CustomType == "aspnetmvc-ajax")
			{
				ProcessDataSource();
			}
			VerifySettings();
			if(IsSelfInitialized)
			{
				if(HasDeferredInitialization)
					WriteDeferredScriptInitialization();
				else
					WriteInitializationScript(writer);
			}
		}

		/// <summary>
		/// Verifica as configurações.
		/// </summary>
		public override void VerifySettings()
		{
		}
	}
}
