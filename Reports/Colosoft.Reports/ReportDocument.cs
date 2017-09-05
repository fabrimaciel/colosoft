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
	/// Implementação padrão de uma documento de relatório.
	/// </summary>
	public class ReportDocument : NotificationObject, IReportDocument, IReportDefinitionContainer, IReportDataSourceParametersContainer
	{
		private string _name;

		private IMessageFormattable _title;

		private Func<System.IO.Stream> _reportDefinitionLoader;

		private List<IReportDataSource> _dataSources = new List<IReportDataSource>();

		private ReportParameterCollection _parameters = new ReportParameterCollection();

		private ReportDefinitionParameterCollection _definitionParameters = new ReportDefinitionParameterCollection();

		private ReportDataSourceParameterCollection _dataSourcesParameters = new ReportDataSourceParameterCollection();

		/// <summary>
		/// Nome do documento.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Título do documento.
		/// </summary>
		public virtual IMessageFormattable Title
		{
			get
			{
				return _title;
			}
			set
			{
				if(_title != value)
				{
					_title = value;
					RaisePropertyChanged("Title");
				}
			}
		}

		/// <summary>
		/// Relação das origens de dados do documento.
		/// </summary>
		public List<IReportDataSource> DataSources
		{
			get
			{
				return _dataSources;
			}
		}

		/// <summary>
		/// Relação dos paramtros do relatório.
		/// </summary>
		public ReportParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Parametros da definição.
		/// </summary>
		public ReportDefinitionParameterCollection DefinitionParameters
		{
			get
			{
				return _definitionParameters;
			}
		}

		/// <summary>
		/// Parametros das fontes de dados.
		/// </summary>
		public ReportDataSourceParameterCollection DataSourcesParameters
		{
			get
			{
				return _dataSourcesParameters;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do documento.</param>
		public ReportDocument(string name)
		{
			name.Require("name").NotNull().NotEmpty();
			_name = name;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="reportDefinitionLoader"></param>
		public ReportDocument(string name, Func<System.IO.Stream> reportDefinitionLoader) : this(name)
		{
			reportDefinitionLoader.Require("reportDefinitionLoader").NotNull();
			_reportDefinitionLoader = reportDefinitionLoader;
		}

		/// <summary>
		/// Processa o subrelatório.
		/// </summary>
		/// <param name="e"></param>
		public virtual void ProcessSubreport(SubreportProcessingEventArgs e)
		{
		}

		/// <summary>
		/// Recupera a definiação do relatório.
		/// </summary>
		/// <returns></returns>
		public virtual System.IO.Stream GetDefinition()
		{
			return _reportDefinitionLoader();
		}

		/// <summary>
		/// Atualiza as origens de dados.
		/// </summary>
		public virtual void RefreshDataSources()
		{
		}

		/// <summary>
		/// Atualiza o documento a partir do parametros dos DataSources.
		/// </summary>
		/// <param name="culture">Cultura que será utilizada para converter os parametros.</param>
		public virtual void RefreshDocumentFromParameters(System.Globalization.CultureInfo culture)
		{
			var properties = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			foreach (var prop in properties)
			{
				var parameterName = prop.GetCustomAttributes(typeof(ReportDataSourceParameterAttribute), true).Select(f => ((ReportDataSourceParameterAttribute)f).ParameterName).FirstOrDefault();
				if(parameterName != null && DataSourcesParameters.Contains(parameterName))
				{
					var parameterValue = DataSourcesParameters[parameterName];
					if(parameterValue != null)
					{
						if(parameterValue.GetType() != prop.PropertyType)
						{
							var propertyTypeConverter = System.ComponentModel.TypeDescriptor.GetConverter(prop.PropertyType);
							if(propertyTypeConverter.CanConvertFrom(parameterValue.GetType()))
							{
								try
								{
									parameterValue = propertyTypeConverter.ConvertFrom(null, culture, parameterValue);
								}
								catch(Exception ex)
								{
									throw new Exception(string.Format("Error on convert parameter {0} with value '{1}' to property {2} of type {3}", parameterName, parameterValue, prop.PropertyType.FullName, this.GetType().FullName), ex);
								}
							}
							else
								throw new Exception(string.Format("Invalid cast from parameter {0} with value '{1}' to property {2} of type {3}", parameterName, parameterValue, prop.PropertyType.FullName, this.GetType().FullName));
						}
						try
						{
							prop.SetValue(this, parameterValue, null);
						}
						catch(System.Reflection.TargetInvocationException ex)
						{
							if(ex.InnerException != null)
								throw ex.InnerException;
							throw;
						}
					}
				}
			}
		}
	}
}
