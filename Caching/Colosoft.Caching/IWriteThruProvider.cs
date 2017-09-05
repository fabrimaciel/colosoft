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
using System.Collections;

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Assinatura de um provedor.
	/// </summary>
	public interface IWriteThruProvider
	{
		/// <summary>
		/// Adiciona as entradas para a origem.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="vals">Valores das entradas.</param>
		/// <returns></returns>
		Hashtable AddToSource(string[] keys, object[] vals);

		/// <summary>
		/// Adiciona uma entrada para a origem.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="value">Valor da entrada.</param>
		/// <returns></returns>
		bool AddToSource(string key, object value);

		/// <summary>
		/// Limpa os dados da origem.
		/// </summary>
		/// <returns></returns>
		bool Clear();

		/// <summary>
		/// Remove a entrada com a chave informada da origem.
		/// </summary>
		/// <param name="key">Chave de entrada que será removida.</param>
		/// <returns></returns>
		bool RemoveFromSource(string key);

		/// <summary>
		/// Remove as entradas com as chaves informadas.
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		Hashtable RemoveFromSource(string[] keys);

		/// <summary>
		/// Inicia o provedor.
		/// </summary>
		/// <param name="parameters">Parametros de configuração.</param>
		void Start(IDictionary parameters);

		/// <summary>
		/// Para o provedor.
		/// </summary>
		void Stop();

		/// <summary>
		/// Atualiza os dados na origem.
		/// </summary>
		/// <param name="key">Chave da entrada que será atualiza.</param>
		/// <param name="value">Valor da entrada.</param>
		/// <returns></returns>
		bool UpdateSource(string key, object value);

		/// <summary>
		/// Atualiza as entradas na origem.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="vals"></param>
		/// <returns></returns>
		Hashtable UpdateSource(string[] keys, object[] vals);
	}
}
