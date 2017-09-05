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
using System.Text;

namespace Colosoft.WebControls.GridView
{
	/// <summary>
	/// Assinatura da classe que representa um formatador de valores das colunas.
	/// </summary>
	public interface IColumnValuesFormatter
	{
		/// <summary>
		/// Método usado para formatar o valor.
		/// </summary>
		/// <param name="instance">Instancia do objeto onde está contido a propriedade
		/// do valor que está sendo formatado.</param>
		/// <param name="dataField">Nome do campo de dados que está sendo formatado.</param>
		/// <param name="value">Valor que será formatado.</param>
		/// <returns>Valor formatado.</returns>
		string Format(object instance, string dataField, object value);

		/// <summary>
		/// Método usado para remover a formatação do valor.
		/// </summary>
		/// <param name="instanceType">Tipo da instancia do objeto onde está contido a propriedade
		/// do valor que está sendo formatado.</param>
		/// <param name="dataField">Nome do campo de dados que está sendo formatado.</param>
		/// <param name="value">Valor formatado.</param>
		/// <returns></returns>
		object Unformat(Type instanceType, string dataField, string value);
	}
}
