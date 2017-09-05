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
	/// Assinatura de um parametro de relatório.
	/// </summary>
	public class ReportDefinitionParameter
	{
		/// <summary>
		/// Nome do parametro.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Valor do parametro.
		/// </summary>
		public string[] Values
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ReportDefinitionParameter()
		{
			this.Values = new string[0];
		}

		/// <summary>
		/// Cria o parametro com o nome e o valor equivalente.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		public ReportDefinitionParameter(string name, string value)
		{
			this.Name = name;
			if(value != null)
				this.Values = new string[] {
					value
				};
			else
				this.Values = new string[0];
		}

		/// <summary>
		/// Cria o parametro com o nome e os valores.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="values"></param>
		public ReportDefinitionParameter(string name, string[] values)
		{
			this.Name = name;
			this.Values = values ?? new string[0];
		}

		/// <summary>
		/// Recupera o texto que representa o parametro.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0} = {1}", Name, string.Join(";", Values));
		}
	}
}
