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
	/// Assinatura do gerador de chave de registor.
	/// </summary>
	public interface IRecordKeyGenerator
	{
		/// <summary>
		/// Recupera a chave simples que representa o registro.
		/// </summary>
		/// <param name="record">Instancia do registro.</param>
		/// <returns></returns>
		string GetSimpleKey(Query.IRecord record);

		/// <summary>
		/// Recupera a chave com base nos dados do registro informado.
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		string GetKey(Colosoft.Query.IRecord record);

		/// <summary>
		/// Recupera uma chave simples a partir de uma chave completa.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		string GetSimpleKeyFromFullKey(string key);
	}
	/// <summary>
	/// Assinatura das classe responsável por criar
	/// a chave para um registro.
	/// </summary>
	public interface IRecordKeyFactory
	{
		/// <summary>
		/// Cria o gerador associado com o tipo informado.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		IRecordKeyGenerator CreateGenerator(Colosoft.Reflection.TypeName typeName);

		/// <summary>
		/// Recupera os nomes dos campos que fazem parte da chave do registro.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		IEnumerable<string> GetKeyFields(Colosoft.Reflection.TypeName typeName);

		/// <summary>
		/// Cria uma chave para o registro.
		/// </summary>
		/// <param name="typeName">Nome do tipo que representa os dados contidos no registro.</param>
		/// <param name="record">Instancia do registro com os dados.</param>
		/// <returns>Chave que representa o registro.</returns>
		RecordKey Create(Colosoft.Reflection.TypeName typeName, IRecord record);
	}
}
