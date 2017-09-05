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

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Assinatura das classes que fornecem os esquema para os tipos do sistema.
	/// </summary>
	public interface ITypeSchema
	{
		/// <summary>
		/// Recupera os metadados do tipo com o nome informado.
		/// </summary>
		/// <param name="fullName">Nome completo do tipo.</param>
		/// <returns>Instancia dos metadados do tipo.</returns>
		ITypeMetadata GetTypeMetadata(string fullName);

		/// <summary>
		/// Recupera os metadados do tipo com base no código informado.
		/// </summary>
		/// <param name="typeCode">Código do tipo.</param>
		/// <returns></returns>
		ITypeMetadata GetTypeMetadata(int typeCode);

		/// <summary>
		/// Recupera os metadados de uma propriedade pelo código informado.
		/// </summary>
		/// <param name="propertyCode">Código da propriedade.</param>
		/// <returns>Instancia dos metadados da propriedade.</returns>
		IPropertyMetadata GetPropertyMetadata(int propertyCode);

		/// <summary>
		/// Recupera os metadados de todos o tipos registrados
		/// </summary>
		/// <returns>Intâncias dos metadados dos tipos</returns>
		IEnumerable<ITypeMetadata> GetTypeMetadatas();

		/// <summary>
		/// Recupera os metadados dos tipos associados com o assembly informado.
		/// </summary>
		/// <param name="assemblyName">Nome do assembly onde os tipo estão inseridos.</param>
		/// <returns>Intâncias dos metadados dos tipos</returns>
		IEnumerable<ITypeMetadata> GetTypeMetadatas(string assemblyName);

		/// <summary>
		/// Recarrega informações.
		/// </summary>
		void Reload();
	}
}
