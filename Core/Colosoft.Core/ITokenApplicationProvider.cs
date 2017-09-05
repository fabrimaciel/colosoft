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

namespace Colosoft.Security
{
	/// <summary>
	/// Assinatura da classe que gerencia o token por aplicação.
	/// </summary>
	public interface ITokenApplicationProvider
	{
		/// <summary>
		/// Insere o controle do token
		/// </summary>
		/// <param name="token">token</param>
		/// <param name="userId">identificador do usuário</param>
		/// <param name="applicationName">Nome da aplicação associada com o token.</param>
		/// <returns>verdadeiro se a inserção foi bem sucedida</returns>
		bool Insert(string token, int userId, string applicationName);

		/// <summary>
		/// Verifica se já existe um token aberto para o usuário em questão
		/// </summary>
		/// <param name="userId">Identificador do usuário</param>
		/// <param name="applicationName">Nome da aplicação no qual o token deve estar associado.</param>
		/// <returns>token</returns>
		string GetToken(int userId, string applicationName);

		/// <summary>
		/// Fecha os tokens em aberto de um usuário.
		/// </summary>
		/// <param name="userId">Identificador do usuário</param>
		/// <param name="applicationName">Nome da aplicação.</param>
		void CloseUserTokens(int userId, string applicationName);
	}
}
