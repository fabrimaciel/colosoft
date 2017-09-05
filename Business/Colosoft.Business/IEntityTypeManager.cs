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

namespace Colosoft.Business
{
	/// <summary>
	/// Assinatura da classe responsável por gerenciar os tipos de entidades.
	/// </summary>
	public interface IEntityTypeManager
	{
		/// <summary>
		/// Tenta recupera os dados da versão do tipo da entidade com base
		/// no tipo informado.
		/// </summary>
		/// <param name="typeName">Tipo que será pesquisado.</param>
		/// <returns>Versão do tipo da entidade correspondente ou null.</returns>
		BusinessEntityTypeVersion GetTypeVersion(Colosoft.Reflection.TypeName typeName);

		/// <summary>
		/// Tenta recupera as propriedade da versão do tipo da entidade com base no tipo informado.
		/// </summary>
		/// <param name="typeName">Tipo da versão da entidade.</param>
		/// <param name="uiContext">Nome do contexto visual usado para filtrar os dados.</param>
		/// <returns></returns>
		IEnumerable<BusinessEntityTypeVersionProperty> GetTypeProperties(Colosoft.Reflection.TypeName typeName, string uiContext = null);

		/// <summary>
		/// Verifica se o tipo da entidade possui um identificador unico.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool HasUid(Type type);

		/// <summary>
		/// Verifica se o tipo informado possui chaves.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool HasKeys(Type type);

		/// <summary>
		/// Gera um novo identificador unico para uma instancia do tipo informado.
		/// </summary>
		/// <param name="type">Tipo da entidade de dados.</param>
		/// <returns></returns>
		int GenerateInstanceUid(Type type);

		/// <summary>
		/// Recupera o loader para o tipo informado.
		/// </summary>
		/// <param name="type">Tipo da entidade de dados.</param>
		/// <returns></returns>
		IEntityLoader GetLoader(Type type);
	}
}
