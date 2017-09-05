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
	/// Assinatura das classes que armazena os metadados de um tipo do sistema.
	/// </summary>
	public interface ITypeMetadata : IEnumerable<IPropertyMetadata>
	{
		/// <summary>
		/// Código do tipo.
		/// </summary>
		int TypeCode
		{
			get;
		}

		/// <summary>
		/// Nome do tipo.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Espaço de nome onde o tipo está inserido.
		/// </summary>
		string Namespace
		{
			get;
		}

		/// <summary>
		/// Nome completo do tipo.
		/// </summary>
		string FullName
		{
			get;
		}

		/// <summary>
		/// Nome do assembly onde o tipo está inserido.
		/// </summary>
		string Assembly
		{
			get;
		}

		/// <summary>
		/// Nome da tabela associada.
		/// </summary>
		TableName TableName
		{
			get;
		}

		/// <summary>
		/// Recupera os dados da propriedade pelo nome informado.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade que será recuperada.</param>
		/// <returns></returns>
		IPropertyMetadata this[string propertyName]
		{
			get;
		}

		/// <summary>
		/// Recupera a quantidade de propriedades da tabela.
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Define se o tipo pode ser persistido em cache.
		/// </summary>
		bool IsCache
		{
			get;
		}

		/// <summary>
		/// Define se a coluna é versionada ou não.
		/// </summary>
		bool IsVersioned
		{
			get;
		}

		/// <summary>
		/// Recupera os metadados da propriedade com base no código informado.
		/// </summary>
		/// <param name="propertyCode"></param>
		/// <returns></returns>
		IPropertyMetadata GetProperty(int propertyCode);

		/// <summary>
		/// Recupera as propriedades chave do tipo.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IPropertyMetadata> GetKeyProperties();

		/// <summary>
		/// Recupera todas as propriedades que são recuperadas após a consulta.
		/// </summary>
		/// <returns>Propriedades voláteis</returns>
		IEnumerable<IPropertyMetadata> GetVolatileProperties();

		/// <summary>
		/// Tenta recupera os metadados da propriedade pelo nome informado.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade que será pesquisada.</param>
		/// <param name="propertyMetadata">Metadados da propriedade encontrada.</param>
		/// <returns></returns>
		bool TryGet(string propertyName, out IPropertyMetadata propertyMetadata);
	}
}
