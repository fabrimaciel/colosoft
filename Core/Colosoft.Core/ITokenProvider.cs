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
	/// Assinatura das classes que disponibilizam token.
	/// </summary>
	public interface ITokenProvider
	{
		/// <summary>
		/// Nome do provedor.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Cria um token com o tamanho padrão e com os caracteres padrão
		/// </summary>
		/// <returns>Token</returns>
		string Create();

		/// <summary>
		/// Cria um token com o tamanho padrão os caracteres informados
		/// </summary>
		/// <param name="validChars">Vetor com os caracteres válidos</param>
		/// <returns>Token</returns>
		string Create(char[] validChars);

		/// <summary>
		/// Cria um token do tamanho informado com os caracteres "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%¨*()"
		/// </summary>
		/// <param name="size">Tamanho do token</param>
		/// <returns>Token</returns>
		string Create(int size);

		/// <summary>
		/// Cria um token do tamanho informado com os caracteres informados
		/// </summary>
		/// <param name="size">Tamanho do token</param>
		/// <param name="validChars">Vetor com os caracteres válidos</param>
		/// <returns>Token</returns>
		string Create(int size, char[] validChars);

		/// <summary>
		/// Insere o controle do token
		/// </summary>
		/// <param name="token">token</param>
		/// <param name="userId">identificador do usuário</param>
		/// <returns>verdadeiro se a inserção foi bem sucedida</returns>
		bool Insert(string token, int userId);

		/// <summary>
		/// Verifica se um token está ou não válido
		/// </summary>
		/// <param name="token">token</param>
		/// <returns>Objeto com o resultado da consulta</returns>
		TokenConsultResult Check(string token);

		/// <summary>
		/// Define o perfil para o token.
		/// </summary>
		/// <param name="token">Token.</param>
		/// <param name="profileId">Identificador do perfil.</param>
		/// <returns></returns>
		TokenSetProfileResult SetProfile(string token, int profileId);

		/// <summary>
		/// Invalida o token
		/// </summary>
		/// <param name="token">token</param>
		/// <returns>verdadeiro se conseguiu invalidar</returns>
		bool Close(string token);

		/// <summary>
		/// Verifica se já existe um token aberto para o usuário em questão
		/// </summary>
		/// <param name="userId">Identificador do usuário</param>
		/// <returns>token</returns>
		string GetToken(int userId);

		/// <summary>
		/// Executa uma verificação do token no servidor.
		/// </summary>
		/// <param name="token">Token</param>
		/// <returns></returns>
		TokenPingResult Ping(string token);

		/// <summary>
		/// Marca as mensagens como lidas.
		/// </summary>
		/// <param name="dispatcherIds">Identificadores dos despachos.</param>
		void MarkMessageAsRead(IEnumerable<int> dispatcherIds);

		/// <summary>
		/// Fecha os tokens em aberto de um usuário.
		/// </summary>
		/// <param name="userId">Identificador do usuário</param>
		void CloseUserTokens(int userId);
	}
}
