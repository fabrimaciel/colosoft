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

namespace Colosoft.Query
{
	/// <summary>
	/// Interface para as classe que armazena um nome traduzido.
	/// </summary>
	public interface ITranslatedName : IComparable<ITranslatedName>
	{
	}
	/// <summary>
	/// Interface para traduzir os nomes e propriedadades dos objetos CLR para colunas e tabelas do banco de dados
	/// </summary>
	public interface IQueryTranslator
	{
		/// <summary>
		/// Pega nome da tabela
		/// </summary>
		/// <param name="entity">Entidade cuja qual se quer saber o nome da tabela</param>
		/// <returns>Nome da tabela</returns>
		ITranslatedName GetName(EntityInfo entity);

		/// <summary>
		/// Pega nome da tabela
		/// </summary>
		/// <param name="entity">Entidade cuja qual se quer saber o nome da tabela</param>
		/// <param name="ignoreTypeSchema">Identifica se é para ignorar o tipo do esquema.</param>
		/// <returns>Nome da tabela</returns>
		ITranslatedName GetName(EntityInfo entity, bool ignoreTypeSchema);

		/// <summary>
		/// Pega nome da coluna
		/// </summary>
		/// <param name="entity">Entidade cuja qual a coluna pertence</param>
		/// <param name="propertyName">Nome da propriedade que representa a coluna</param>
		/// <returns>Nome da coluna</returns>
		ITranslatedName GetName(EntityInfo entity, string propertyName);

		/// <summary>
		/// Pega nome da coluna
		/// </summary>
		/// <param name="entity">Entidade cuja qual a coluna pertence</param>
		/// <param name="propertyName">Nome da propriedade que representa a coluna</param>
		/// <param name="ignoreTypeSchema">Identifica se é para ignorar o esquema do tipo.</param>
		/// <returns>Nome da coluna</returns>
		ITranslatedName GetName(EntityInfo entity, string propertyName, bool ignoreTypeSchema);

		/// <summary>
		/// Tenta recupera o nome traduzido para a propriedade da entidade.
		/// </summary>
		/// <param name="entity">Informações da entidade.</param>
		/// <param name="propertyName">Nome da propriedade da entidade.</param>
		/// <param name="name">Nome encontrado.</param>
		/// <returns>True caseo tenha encontrada a tradução do nome.</returns>
		bool TryGetName(EntityInfo entity, string propertyName, out ITranslatedName name);

		/// <summary>
		/// Tenta recupera o nome traduzido para a propriedade da entidade.
		/// </summary>
		/// <param name="entity">Informações da entidade.</param>
		/// <param name="propertyName">Nome da propriedade da entidade.</param>
		/// <param name="ignoreTypeSchema">Identifica se é para ignorar o esquema do tipo.</param>
		/// <param name="name">Nome encontrado.</param>
		/// <returns>True caseo tenha encontrada a tradução do nome.</returns>
		bool TryGetName(EntityInfo entity, string propertyName, bool ignoreTypeSchema, out ITranslatedName name);

		/// <summary>
		/// Pega o nome da storedprocedure.
		/// </summary>
		/// <param name="storedProcedureName">Nome da stored procedure.</param>
		/// <returns></returns>
		ITranslatedName GetName(StoredProcedureName storedProcedureName);
	}
}
