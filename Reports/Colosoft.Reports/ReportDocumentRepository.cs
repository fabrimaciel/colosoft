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
	/// Assinatura de um repositório de documentos de relatório do sistema.
	/// </summary>
	public class ReportDocumentRepository
	{
		private Microsoft.Practices.ServiceLocation.IServiceLocator _serviceLocator;

		private static ReportDocumentRepository _current;

		private static object _objLock = new object();

		/// <summary>
		/// Instancia do localizador de serviço.
		/// </summary>
		public Microsoft.Practices.ServiceLocation.IServiceLocator ServiceLocator
		{
			get
			{
				if(_serviceLocator == null)
					_serviceLocator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current;
				return _serviceLocator;
			}
			set
			{
				_serviceLocator = value;
			}
		}

		/// <summary>
		/// Instancia corrente.
		/// </summary>
		public static ReportDocumentRepository Current
		{
			get
			{
				if(_current == null)
					lock (_objLock)
						if(_current == null)
							_current = new ReportDocumentRepository();
				return _current;
			}
			set
			{
				_current = value;
			}
		}

		/// <summary>
		/// Cria um documento de relatório pelo nome informado.
		/// </summary>
		/// <param name="name">Nome do relatório.</param>
		/// <returns></returns>
		public IReportDocument CreateReportDocument(string name)
		{
			return ServiceLocator.GetInstance<IReportDocument>(typeof(IReportDocument).FullName + "." + name);
		}
	}
}
