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
	/// Assinatura do evento acionado quando é requisitado o processamento de um subrelatório.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void SubreportProcessingEventHandler (object sender, SubreportProcessingEventArgs e);
	/// <summary>
	/// Argumentos usado no evento acionado para processar os subrelatórios.
	/// </summary>
	public class SubreportProcessingEventArgs : EventArgs
	{
		private IList<string> _dataSourceNames;

		private ReportDataSourceCollection _dataSources;

		private ReportParameterInfoCollection _parameters;

		private string _reportPath;

		/// <summary>
		/// Nome das origem de dados.
		/// </summary>
		public IList<string> DataSourceNames
		{
			get
			{
				return _dataSourceNames;
			}
		}

		/// <summary>
		/// Origens de dados.
		/// </summary>
		public ReportDataSourceCollection DataSources
		{
			get
			{
				return _dataSources;
			}
		}

		/// <summary>
		/// Parametros associados.
		/// </summary>
		public ReportParameterInfoCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Caminho do relatório.
		/// </summary>
		public string ReportPath
		{
			get
			{
				return _reportPath;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="subreportName">Nome do sub relatório.</param>
		/// <param name="paramMetaData">Metadados dos parametros.</param>
		/// <param name="dataSetNames">Nomes das origem de dados.</param>
		public SubreportProcessingEventArgs(string subreportName, ReportParameterInfoCollection paramMetaData, string[] dataSetNames)
		{
			_dataSources = new ReportDataSourceCollection();
			_reportPath = subreportName;
			_parameters = paramMetaData;
			_dataSourceNames = new System.Collections.ObjectModel.ReadOnlyCollection<string>(dataSetNames);
		}
	}
}
