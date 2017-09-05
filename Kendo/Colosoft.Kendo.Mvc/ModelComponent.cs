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

using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Colosoft.Kendo.Mvc.UI
{
	/// <summary>
	/// Representa um componente de uma model.
	/// </summary>
	public class ModelComponent : WidgetBase
	{
		private CustomModelDescriptor _modelDescriptor;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="modelDescriptor"></param>
		/// <param name="viewContext"></param>
		/// <param name="initializer"></param>
		public ModelComponent(CustomModelDescriptor modelDescriptor, ViewContext viewContext, global::Kendo.Mvc.Infrastructure.IJavaScriptInitializer initializer) : base(viewContext, initializer)
		{
			_modelDescriptor = modelDescriptor;
		}

		/// <summary>
		/// Escreve a inicialização javascript.
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteInitializationScript(System.IO.TextWriter writer)
		{
			if(string.IsNullOrEmpty(Name) || Name.Trim().Length == 0)
				writer.Write(new StringBuilder().Append("new kendo.data.Model.define").Append("(").Append(Initializer.Serialize(_modelDescriptor.ToJson())).Append(")").ToString());
			else
				writer.Write(new StringBuilder().Append("var ").Append(Name).Append(" = new kendo.data.Model.define").Append("(").Append(Initializer.Serialize(_modelDescriptor.ToJson())).Append(");").ToString());
		}

		/// <summary>
		/// Escreve o html da componente.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteHtml(System.Web.UI.HtmlTextWriter writer)
		{
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
