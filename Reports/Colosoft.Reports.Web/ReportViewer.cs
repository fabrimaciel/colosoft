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
using Colosoft;

namespace Colosoft.Reports.Web
{
	/// <summary>
	/// Assinatura do callback usado para recupera a Stream onde o relatório será renderizado.
	/// </summary>
	/// <param name="name">Nome o relatório</param>
	/// <param name="extension">Extensão.</param>
	/// <param name="encoding">Encoding.</param>
	/// <param name="mimeType">MIME Type</param>
	/// <param name="willSeek"></param>
	/// <returns></returns>
	public delegate System.IO.Stream CreateStreamCallback (string name, string extension, Encoding encoding, string mimeType, bool willSeek);
	/// <summary>
	/// Implementação do visualizador de relatórios.
	/// </summary>
	public class ReportViewer : Reports.UI.IReportViewer
	{
		private IReportDocument _document;

		/// <summary>
		/// Relação dos parametros compatíveis.
		/// </summary>
		private List<string> _compatiblesParameters = new List<string>();

		/// <summary>
		/// Evento acionado quando as origens de dados começam a ser atualizadas.
		/// </summary>
		event EventHandler Reports.UI.IReportViewer.BeginRefreshDataSources {
			add
			{
			}
			remove {
			}
		}

		/// <summary>
		/// Evento
		/// </summary>
		event EventHandler Reports.UI.IReportViewer.EndRefreshDataSources {
			add
			{
			}
			remove {
			}
		}

		/// <summary>
		/// Identifica se o visualizador está ocupado.
		/// </summary>
		public bool IsBusy
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Document associado.
		/// </summary>
		public IReportDocument Document
		{
			get
			{
				return _document;
			}
			set
			{
				_document = value;
			}
		}

		/// <summary>
		/// Atual página.
		/// </summary>
		public int CurrentPage
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		/// <summary>
		/// Método acionado quand for necessário processar um subrelatório.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalReportSubreportProcessing(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
		{
			var parameters = new ReportParameterInfoCollection();
			foreach (var i in e.Parameters)
				parameters.Add(new ReportParameterInfo {
					AllowBlank = i.AllowBlank,
					DataType = (ParameterDataType)(int)i.DataType,
					Nullable = i.Nullable,
					MultiValue = i.MultiValue,
					Name = i.Name,
					Values = i.Values
				});
			var args2 = new Colosoft.Reports.SubreportProcessingEventArgs(e.ReportPath, parameters, e.DataSourceNames.ToArray());
			Document.ProcessSubreport(args2);
			foreach (var i in args2.DataSources)
				e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource(i.Name, i.Value));
		}

		/// <summary>
		/// Método usado para criar o relatório local.
		/// </summary>
		/// <returns></returns>
		protected virtual Microsoft.Reporting.WebForms.LocalReport CreateLocalReport()
		{
			return new Microsoft.Reporting.WebForms.LocalReport();
		}

		/// <summary>
		/// Carrega a definição para o relatório.
		/// </summary>
		/// <param name="localReport">Relatório local para onde a definição será carregada.</param>
		/// <param name="report"></param>
		protected virtual void LoadDefinition(Microsoft.Reporting.WebForms.LocalReport localReport, System.IO.Stream report)
		{
			_compatiblesParameters.Clear();
			if(Document is IReportDefinitionContainer)
			{
				System.Xml.Linq.XElement root = null;
				root = System.Xml.Linq.XElement.Load(report, System.Xml.Linq.LoadOptions.None);
				var nameAttribute = root.Attribute("Name");
				if(nameAttribute != null && !string.IsNullOrEmpty(nameAttribute.Value) && Document.Title == null)
					Document.Title = nameAttribute.Value.GetFormatter();
				var reportDefinition = root.GetDefaultNamespace().NamespaceName;
				var reportParameters = root.Elements(System.Xml.Linq.XName.Get("ReportParameters", reportDefinition)).FirstOrDefault();
				var parameters = new List<string>();
				if(reportParameters != null)
					foreach (var parameter in reportParameters.Elements(System.Xml.Linq.XName.Get("ReportParameter", reportDefinition)))
					{
						var paramenterName = parameter.Attribute("Name");
						if(paramenterName != null)
							parameters.Add(paramenterName.Value);
					}
				_compatiblesParameters = parameters;
				using (var stream = new System.IO.MemoryStream())
				{
					root.Save(stream);
					stream.Seek(0, System.IO.SeekOrigin.Begin);
					localReport.LoadReportDefinition(stream);
				}
			}
			else
			{
				localReport.LoadReportDefinition(report);
			}
		}

		/// <summary>
		/// Carrega a definição para o relatório.
		/// </summary>
		/// <param name="localReport"></param>
		protected virtual void LoadDefinition(Microsoft.Reporting.WebForms.LocalReport localReport)
		{
			if(Document is IReportDefinitionContainer)
				localReport.LoadReportDefinition(((IReportDefinitionContainer)Document).GetDefinition());
		}

		/// <summary>
		/// Carrega as definição do subrelatório.
		/// </summary>
		/// <param name="localReport"></param>
		/// <param name="subreport"></param>
		protected virtual void LoadSubreportDefinition(Microsoft.Reporting.WebForms.LocalReport localReport, ISubreportDefinition subreport)
		{
			localReport.LoadSubreportDefinition(subreport.Name, subreport.GetDefinition());
		}

		/// <summary>
		/// Prepara o relatório que será exportado.
		/// </summary>
		/// <param name="localReport">Instancia do relatório.</param>
		/// <param name="culture">Cultura que será usada.</param>
		/// <returns></returns>
		protected virtual void PrepareLocalReport(Microsoft.Reporting.WebForms.LocalReport localReport, System.Globalization.CultureInfo culture)
		{
			if(Document == null)
				throw new InvalidOperationException("Document is null");
			localReport.SubreportProcessing += LocalReportSubreportProcessing;
			LoadDefinition(localReport);
			if(Document is ISubreportDefinitionContainer)
				foreach (ISubreportDefinition subreport in (ISubreportDefinitionContainer)Document)
					LoadSubreportDefinition(localReport, subreport);
			foreach (var dataSource in Document.DataSources)
			{
				localReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource(dataSource.Name, dataSource.Value));
			}
			var reportParameters = new List<Microsoft.Reporting.WebForms.ReportParameter>();
			foreach (var parameter in Document.Parameters.Values)
			{
				if(_compatiblesParameters.Count > 0 && !_compatiblesParameters.Contains(parameter.Name))
					continue;
				if(parameter.Values != null)
				{
					var values = new string[parameter.Values.Length];
					for(var i = 0; i < parameter.Values.Length; i++)
						if(parameter.Values[i] != null)
						{
							if(!(parameter.Values[i] is string))
							{
								var converter = System.ComponentModel.TypeDescriptor.GetConverter(parameter.Values[i]);
								if(culture != null)
									values[i] = converter.ConvertToString(null, culture, parameter.Values[i]);
								else
									values[i] = converter.ConvertToString(parameter.Values[i]);
							}
							else
								values[i] = parameter.Values[i] as string;
						}
					reportParameters.Add(new Microsoft.Reporting.WebForms.ReportParameter(parameter.Name, values));
				}
				else
					reportParameters.Add(new Microsoft.Reporting.WebForms.ReportParameter(parameter.Name, (string)null));
			}
			localReport.SetParameters(reportParameters);
			if(Document.Title != null)
				localReport.DisplayName = Document.Title.FormatOrNull() ?? "";
		}

		/// <summary>
		/// Exporta o relatório no formato informado.
		/// </summary>
		/// <param name="exportType">Formato que será usado na exportação.</param>
		/// <param name="createStreamCallback">Callback usada na criação a Stream de exportação.</param>
		/// <param name="warnings">Warnings.</param>
		public virtual void Export(ExportType exportType, CreateStreamCallback createStreamCallback, out Warning[] warnings)
		{
			Export(exportType, createStreamCallback, out warnings, null);
		}

		/// <summary>
		/// Exporta o relatório no formato informado.
		/// </summary>
		/// <param name="exportType">Formato que será usado na exportação.</param>
		/// <param name="createStreamCallback">Callback usada na criação a Stream de exportação.</param>
		/// <param name="warnings">Warnings.</param>
		/// <param name="culture"></param>
		public virtual void Export(ExportType exportType, CreateStreamCallback createStreamCallback, out Warning[] warnings, System.Globalization.CultureInfo culture)
		{
			var localReport = CreateLocalReport();
			PrepareLocalReport(localReport, culture);
			Dictionary<string, object> obj = new Dictionary<string, object>();
			Microsoft.Reporting.WebForms.Warning[] warnings2 = null;
			localReport.Render(exportType.ToString(), null, new Microsoft.Reporting.WebForms.CreateStreamCallback((name, extension, encoding, mimeType, willSeek) => createStreamCallback(name, extension, encoding, mimeType, willSeek)), out warnings2);
			if(warnings2 != null)
			{
				warnings = warnings2.Select(f => new Warning {
					Code = f.Code,
					Message = f.Message,
					ObjectName = f.ObjectName,
					ObjectType = f.ObjectType,
					Severity = f.Severity == Microsoft.Reporting.WebForms.Severity.Error ? Severity.Error : Severity.Warning
				}).ToArray();
			}
			else
				warnings = null;
		}

		/// <summary>
		/// Exporta o relatório no formato informado.
		/// </summary>
		/// <param name="exportType">Formato que será usado na exportação.</param>
		/// <param name="fileNameExtension"></param>
		/// <param name="mimeType">MIME Type.</param>
		/// <param name="encoding">Encoding.</param>
		/// <param name="warnings">Warnings.</param>
		/// <returns></returns>
		public virtual System.IO.Stream Export(ExportType exportType, out string mimeType, out string encoding, out string fileNameExtension, out Warning[] warnings)
		{
			return Export(exportType, out mimeType, out encoding, out fileNameExtension, out warnings, null);
		}

		/// <summary>
		/// Exporta o relatório no formato informado.
		/// </summary>
		/// <param name="exportType">Formato que será usado na exportação.</param>
		/// <param name="fileNameExtension"></param>
		/// <param name="mimeType">MIME Type.</param>
		/// <param name="encoding">Encoding.</param>
		/// <param name="warnings">Warnings.</param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public virtual System.IO.Stream Export(ExportType exportType, out string mimeType, out string encoding, out string fileNameExtension, out Warning[] warnings, System.Globalization.CultureInfo culture)
		{
			var localReport = CreateLocalReport();
			PrepareLocalReport(localReport, culture);
			Dictionary<string, object> obj = new Dictionary<string, object>();
			Microsoft.Reporting.WebForms.Warning[] warnings2 = null;
			string[] streamids = null;
			fileNameExtension = string.Empty;
			byte[] byteViewer = null;
			byteViewer = localReport.Render(exportType.ToString(), null, out mimeType, out encoding, out fileNameExtension, out streamids, out warnings2);
			if(warnings2 != null)
			{
				warnings = warnings2.Select(f => new Warning {
					Code = f.Code,
					Message = f.Message,
					ObjectName = f.ObjectName,
					ObjectType = f.ObjectType,
					Severity = f.Severity == Microsoft.Reporting.WebForms.Severity.Error ? Severity.Error : Severity.Warning
				}).ToArray();
			}
			else
				warnings = null;
			return new System.IO.MemoryStream(byteViewer);
		}
	}
}
