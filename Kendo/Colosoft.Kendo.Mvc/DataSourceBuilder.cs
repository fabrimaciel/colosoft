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

namespace Colosoft.Kendo.Mvc.UI.Fluent
{
	/// <summary>
	/// Defines the fluent interface for configuring the <see cref="DataSource"/> component.
	/// </summary>
	public class DataSourceBuilder<TModel> : WidgetBuilderBase<DataSourceComponent, DataSourceBuilder<TModel>> where TModel : class
	{
		/// <summary>
		/// Fonte de dados associada.
		/// </summary>
		protected readonly global::Kendo.Mvc.UI.DataSource dataSource;

		/// <summary>
		/// Gerador de Url.
		/// </summary>
		protected readonly IUrlGenerator urlGenerator;

		/// <summary>
		/// Contexto da view.
		/// </summary>
		protected readonly ViewContext viewContext;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="viewContext"></param>
		/// <param name="initializer"></param>
		/// <param name="urlGenerator"></param>
		public DataSourceBuilder(global::Kendo.Mvc.UI.DataSource dataSource, ViewContext viewContext, IJavaScriptInitializer initializer, IUrlGenerator urlGenerator) : base(new DataSourceComponent(dataSource, viewContext, initializer))
		{
			this.viewContext = viewContext;
			this.urlGenerator = urlGenerator;
			this.dataSource = dataSource;
		}

		/// <summary>
		/// Use it to configure Ajax binding.
		/// </summary>        
		public DataSourceBuilder<TModel> Ajax(Action<AjaxDataSourceBuilder<TModel>> configurator)
		{
			dataSource.Type = DataSourceType.Ajax;
			configurator(new AjaxDataSourceBuilder<TModel>(dataSource, viewContext, urlGenerator));
			return this;
		}

		/// <summary>
		/// Use it to configure Server binding.
		/// </summary>        
		public DataSourceBuilder<TModel> Server(Action<ServerDataSourceBuilder<TModel>> configurator)
		{
			dataSource.Type = DataSourceType.Server;
			configurator(new ServerDataSourceBuilder<TModel>(dataSource, viewContext, urlGenerator));
			return this;
		}

		#if !MVC3
		/// <summary>
		/// Use it to configure WebApi binding.
		/// </summary>
		public DataSourceBuilder<TModel> WebApi(Action<AjaxDataSourceBuilder<TModel>> configurator)
		{
			dataSource.Type = DataSourceType.WebApi;
			configurator(new AjaxDataSourceBuilder<TModel>(dataSource, viewContext, urlGenerator));
			return this;
		}

		#endif
		/// <summary>
		/// Use it to configure Custom binding.
		/// </summary>
		public DataSourceBuilder<TModel> Custom(Action<CustomDataSourceBuilder<TModel>> configurator)
		{
			dataSource.Type = DataSourceType.Custom;
			configurator(new CustomDataSourceBuilder<TModel>(dataSource, viewContext, urlGenerator));
			return this;
		}

		/// <summary>
		/// Use it to configure SignalR binding.
		/// </summary>
		public DataSourceBuilder<TModel> SignalR(Action<SignalRDataSourceBuilder<TModel>> configurator)
		{
			dataSource.Type = DataSourceType.Custom;
			configurator(new SignalRDataSourceBuilder<TModel>(dataSource));
			return this;
		}
	}
}
