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
	/// Implementação de uma coleção de parametro do relatório.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2229:ImplementSerializationConstructors"), Serializable]
	public class ReportParameterCollection : Dictionary<string, ReportParameter>
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ReportParameterCollection()
		{
		}

		/// <summary>
		/// Cria a instancia com a relação informada.
		/// </summary>
		/// <param name="parameters"></param>
		public ReportParameterCollection(IEnumerable<ReportParameter> parameters)
		{
			foreach (var p in parameters)
				this.Add(p.Name, p);
		}

		/// <summary>
		/// Registra um parametro para  a coleção.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
		public void Add(string name, object value)
		{
			base.Add(name, new ReportParameter(name, value));
		}

		/// <summary>
		/// Adiciona um parametro para a coleção.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="values">Valores que serão atribuídos ao parâmetro.</param>
		public void Add(string name, object[] values)
		{
			base.Add(name, new ReportParameter(name, values));
		}

		/// <summary>
		/// Adiciona um parâmetro para a coleção.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="values">Valores do parâmetro.</param>
		public void Add(string name, string[] values)
		{
			base.Add(name, new ReportParameter(name, values != null ? values.Select(f => (object)f).ToArray() : null));
		}
	}
}
