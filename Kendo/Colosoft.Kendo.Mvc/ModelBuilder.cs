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
	/// Implementação do construtor de model.
	/// </summary>
	public class ModelBuilder<TModel> : WidgetBuilderBase<ModelComponent, ModelBuilder<TModel>> where TModel : class
	{
		/// <summary>
		/// Component.
		/// </summary>
		protected readonly CustomModelDescriptor modelDescriptor;

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
		/// <param name="modelDescriptor"></param>
		/// <param name="viewContext"></param>
		/// <param name="initializer"></param>
		/// <param name="urlGenerator"></param>
		public ModelBuilder(CustomModelDescriptor modelDescriptor, ViewContext viewContext, IJavaScriptInitializer initializer, IUrlGenerator urlGenerator) : base(new ModelComponent(modelDescriptor, viewContext, initializer))
		{
			this.viewContext = viewContext;
			this.urlGenerator = urlGenerator;
			this.modelDescriptor = modelDescriptor;
		}
	}
}
