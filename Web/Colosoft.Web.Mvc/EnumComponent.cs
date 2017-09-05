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

namespace Colosoft.Web.Mvc.UI
{
	/// <summary>
	/// Representa um componente de um enum.
	/// </summary>
	public class EnumComponent : WidgetBase
	{
		private Type _enumType;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="enumType"></param>
		/// <param name="viewContext"></param>
		/// <param name="initializer"></param>
		public EnumComponent(Type enumType, ViewContext viewContext, Colosoft.Web.Mvc.Infrastructure.IJavaScriptInitializer initializer) : base(viewContext, initializer)
		{
			_enumType = enumType;
		}

		/// <summary>
		/// Recupera os valores do enum.
		/// </summary>
		/// <returns></returns>
		private IDictionary<string, object> GetEnumValues()
		{
			var names = Enum.GetNames(_enumType);
			var values = Enum.GetValues(_enumType);
			var result = new Dictionary<string, object>();
			for(var i = 0; i < names.Length; i++)
				result.Add(names[i], (int)values.GetValue(i));
			return result;
		}

		/// <summary>
		/// Escreve a inicialização javascript.
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteInitializationScript(System.IO.TextWriter writer)
		{
			if(string.IsNullOrEmpty(Name) || Name.Trim().Length == 0)
				writer.Write(new StringBuilder().Append("Object.freeze(").Append(Initializer.Serialize(GetEnumValues())).Append(")").ToString());
			else
				writer.Write(new StringBuilder().Append("var ").Append(Name).Append(" = Object.freeze(").Append(Initializer.Serialize(GetEnumValues())).Append(");").ToString());
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
