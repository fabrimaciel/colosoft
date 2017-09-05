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

namespace Colosoft.Reports
{
	/// <summary>
	/// Classe com os métodos de extensão usados para auxiliar na manipulação dos relatórios.
	/// </summary>
	public static class ReportsExtensions
	{
		/// <summary>
		/// Recupera a view do documento do relatório.
		/// </summary>
		/// <param name="reportDocument"></param>
		/// <returns></returns>
		public static Colosoft.Reports.UI.IReportViewer GetViewer(this IReportDocument reportDocument)
		{
			var serviceLocator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current;
			UI.IReportViewer reportViewer = null;
			try
			{
				reportViewer = serviceLocator.GetInstance<Colosoft.Reports.UI.IReportViewer>();
			}
			catch(Exception)
			{
				Notifications.Notification.Dispatch(ResourceMessageFormatter.Create(() => Properties.Resources.Extensions_GetReportViewerViewModelError), "Error".GetFormatter(), Notifications.MessageResultOption.OK, Notifications.NotificationType.Error, Notifications.MessageResult.OK);
				return null;
			}
			reportViewer.Document = reportDocument;
			return reportViewer;
		}
	}
}
