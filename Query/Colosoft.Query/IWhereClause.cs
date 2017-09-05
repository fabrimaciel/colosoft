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

namespace Colosoft.Query
{
	/// <summary>
	/// Assinatura da clausa  where.
	/// </summary>
	public interface IWhereClause
	{
		/// <summary>
		/// Container da clausula.
		/// </summary>
		ConditionalContainer Container
		{
			get;
		}

		/// <summary>
		/// Adiciona um parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		/// <returns></returns>
		IWhereClause Add(string name, object value);

		/// <summary>
		/// Adiciona um parametro.
		/// </summary>
		/// <param name="parameter">Instancia do parametro.</param>
		/// <returns></returns>
		IWhereClause Add(QueryParameter parameter);

		/// <summary>
		/// Adiciona uma condição do tipo AND.
		/// </summary>
		/// <param name="expression">Expressão.</param>
		/// <returns></returns>
		IWhereClause And(string expression);

		/// <summary>
		/// Adiciona uma condição do tipo AND.
		/// </summary>
		/// <param name="conditional">Condicional.</param>
		/// <returns></returns>
		IWhereClause And(Conditional conditional);

		/// <summary>
		/// Adiciona um container de condição do tipo AND.
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		IWhereClause And(ConditionalContainer container);

		/// <summary>
		/// Adiciona uma condição do tipo OR.
		/// </summary>
		/// <param name="expression">Expressõa.</param>
		/// <returns></returns>
		IWhereClause Or(string expression);

		/// <summary>
		/// Adiciona uma condição do tipo OR.
		/// </summary>
		/// <param name="conditional">Condicional.</param>
		/// <returns></returns>
		IWhereClause Or(Conditional conditional);

		/// <summary>
		/// Adiciona um container de condição do tipo OR.
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		IWhereClause Or(ConditionalContainer container);

		/// <summary>
		/// Adiciona a condição inicial. Essa operação limpa todas a outras condições já existentes.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		IWhereClause Start(string expression);

		/// <summary>
		/// Adiciona a condição inicial. Essa operação limpa todas a outras condições já existentes.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		IWhereClause Start(Conditional conditional);

		/// <summary>
		/// Adiciona o container condição inicial. Essa operação limpa todas a outras condições já existentes.
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		IWhereClause Start(ConditionalContainer container);
	}
}
