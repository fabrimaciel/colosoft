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
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Colosoft.Kendo.Mvc.UI.Fluent
{
	class GridRowViewDataKey : IGridDataKey<DataRowView>
	{
		public GridRowViewDataKey(string memberName)
		{
			RouteKey = "id";
			Name = memberName;
		}

		public string Name
		{
			get;
			private set;
		}

		public string RouteKey
		{
			get;
			set;
		}

		public object GetValue(object dataItem)
		{
			return ((DataRowView)dataItem)[Name];
		}

		public string HiddenFieldHtml(System.Web.Mvc.HtmlHelper<DataRowView> htmlHelper)
		{
			return htmlHelper.Hidden(Name, null, new {
				id = ""
			}).ToString();
		}
	}
}
