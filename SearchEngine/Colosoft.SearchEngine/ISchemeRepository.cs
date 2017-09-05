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

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Interface que implementada estabelece métodos para a leitura da estrutura
	/// </summary>
	public interface ISchemeRepository
	{
		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Retorna a lista com os schemas dos campos
		/// </summary>
		/// <returns>Lista de schemas de campos</returns>
		IList<SchemeField> GetFieldSchemas();

		/// <summary>
		/// Retorna a lista com os schemas dos campos
		/// </summary>
		/// <returns>Lista de schemas de indice</returns>
		SchemeIndex GetSchemaIndex(string name);

		/// <summary>
		/// Retorna  o schema dos campos
		/// </summary>
		/// <param name="name"></param>
		/// <returns>Lista de schemas de campos</returns>
		SchemeField GetSchemaField(string name);

		/// <summary>
		/// Recupera o canal pelo identificador informado.
		/// </summary>
		/// <param name="channelId">Identificador do canal.</param>
		/// <returns></returns>
		Channel GetChannel(byte channelId);

		/// <summary>
		/// Carrega as informações de canais do aplicativo
		/// </summary>
		/// <returns></returns>
		ICollection<Channel> GetChannels();
	}
}
