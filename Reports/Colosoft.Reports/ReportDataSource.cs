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
	/// Implementação de uma origem de dados para o relatório.
	/// </summary>
	public class ReportDataSource : IReportDataSource
	{
		private string _name;

		private object _value;

		/// <summary>
		/// Nome da origem dos dados.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Valor da origem.
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Cria a origem com o nome informado.
		/// </summary>
		/// <param name="name">Nome da origem de dados.</param>
		public ReportDataSource(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Cria a origem de dados com um enumerable de itens.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="dataSourceValue"></param>
		public ReportDataSource(string name, System.Collections.IEnumerable dataSourceValue) : this(name)
		{
			_value = dataSourceValue;
		}
	}
}
