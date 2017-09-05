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

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Assinatura da função associada com um membro.
	/// </summary>
	public interface IMemberFunction : IFunctor
	{
		/// <summary>
		/// Recupera o armazenamento associado com o indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		IIndexStore GetStore(AttributeIndex index);

		/// <summary>
		/// Recupera os dados filtrando pelo
		/// </summary>
		/// <param name="store">Armazenamento.</param>
		/// <param name="values">Valores dos parametros.</param>
		/// <param name="comparisonType">Tipo de comparação.</param>
		/// <param name="key">Chave que será usada na comparação.</param>
		/// <returns></returns>
		System.Collections.ArrayList GetData(IIndexStore store, System.Collections.IDictionary values, ComparisonType comparisonType, object key);

		/// <summary>
		/// Nome do membro.
		/// </summary>
		string MemberName
		{
			get;
		}
	}
}
