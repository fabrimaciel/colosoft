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
	/// Assinatura de um registro de dados.
	/// </summary>
	public interface IDataRecord
	{
		/// <summary>
		/// Descritor do registro.
		/// </summary>
		Record.RecordDescriptor Descriptor
		{
			get;
		}

		/// <summary>
		/// Quantidade de campos no resultado.
		/// </summary>
		int FieldCount
		{
			get;
		}

		/// <summary>
		/// Recupera o valor da coluna na posição informada.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		object this[int i]
		{
			get;
		}

		/// <summary>
		/// Recupera um valor pelo nome da coluna.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		object this[string name]
		{
			get;
		}

		/// <summary>
		/// Recupera os valores da registro para o vetor informado.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		int GetValues(object[] values);
	}
}
