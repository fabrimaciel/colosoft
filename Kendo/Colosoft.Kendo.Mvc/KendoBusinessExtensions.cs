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

namespace Colosoft
{
	/// <summary>
	/// Classe com métodos de extensão para regras de negócio.
	/// </summary>
	public static class KendoBusinessExtensions
	{
		/// <summary>
		/// Converte o resultado da operação de salvar em um resultado do DataSource.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public static System.Web.Mvc.JsonResult ToDataSourceResult(this Colosoft.Business.SaveResult result)
		{
			var dataSourceResult = new global::Kendo.Mvc.UI.DataSourceResult();
			if(!result)
				dataSourceResult.Errors = result.Message.Format();
			return new System.Web.Mvc.JsonResult {
				Data = dataSourceResult,
				JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.DenyGet
			};
		}

		/// <summary>
		/// Converte o resultado da operação de apagar em um resultado do DataSource.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public static System.Web.Mvc.JsonResult ToDataSourceResult(this Colosoft.Business.DeleteResult result)
		{
			var dataSourceResult = new global::Kendo.Mvc.UI.DataSourceResult();
			if(!result)
				dataSourceResult.Errors = result.Message.Format();
			return new System.Web.Mvc.JsonResult {
				Data = dataSourceResult,
				JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.DenyGet
			};
		}

		/// <summary>
		/// Converte o resultado da operação em um resultado do DataSource.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public static System.Web.Mvc.JsonResult ToDataSourceResult(this Colosoft.Business.OperationResult result)
		{
			var dataSourceResult = new global::Kendo.Mvc.UI.DataSourceResult();
			if(!result)
				dataSourceResult.Errors = result.Message.Format();
			return new System.Web.Mvc.JsonResult {
				Data = dataSourceResult,
				JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.DenyGet
			};
		}
	}
}
