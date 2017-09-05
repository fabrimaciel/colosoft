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
using Colosoft.Enums;

namespace Colosoft.Lock
{
	/// <summary>
	/// Enumerador com os tipos de lock possíveis.
	/// </summary>
	[Serializable]
	public enum LockType
	{
		/// <summary>
		/// Loca um objeto para usá-lo.
		/// </summary>
		ToUse = 0,
		/// <summary>
		/// Loca um objeto para editá-lo.
		/// </summary>
		ToEdit = 1
	}
	/// <summary>
	/// Contrato para classes que controlarão o processo de lock.
	/// </summary>
	public interface ILockProcess
	{
		/// <summary>
		/// Bloqueia a inatância.
		/// </summary>
		/// <param name="session">Sessão do lock</param>
		/// <param name="token">token que irá bloquear</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockableObject"></param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="lockGroup">Grupo do lock</param>
		/// <returns></returns>
		LockProcessResult Lock(LockSession session, string token, string hostName, Lockable lockableObject, int lockType, string lockGroup);

		/// <summary>
		/// Desbloqueia a inatância.
		/// </summary>
		/// <param name="token">token que irá desbloquear</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockableObject"></param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <returns></returns>
		LockProcessResult UnLock(string token, string hostName, Lockable lockableObject, int lockType);

		/// <summary>
		/// Verifica se uma instância está bloqueada.
		/// </summary>
		/// <param name="lockableObject"></param>
		/// <returns></returns>
		bool IsLocked(Lockable lockableObject);

		/// <summary>
		/// Mata os locksde uma sessão.
		/// </summary>
		/// <param name="session">Sessão do lock</param>
		/// <returns></returns>
		KillLockProcessResult KillLockSession(LockSession session);

		/// <summary>
		/// Indica se a instância está locada para o usuáqui.
		/// </summary>
		/// <param name="lockableObject"></param>
		/// <param name="tokenId"></param>
		/// <returns></returns>
		bool IsLockedToMe(Lockable lockableObject, string tokenId);

		/// <summary>
		/// Cria uma transação de lock.
		/// </summary>
		/// <param name="type">Tipo da transação de lock</param>
		/// <param name="token">Token que inicia a transação.</param>
		/// <returns></returns>
		string CreateTransaction(LockTransactionType type, string token);

		/// <summary>
		/// Registra a solicitação de bloqueio da inatância.
		/// </summary>
		/// <param name="session">Sessão do lock</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockableObject"></param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="transaction">Identificador únido da transação</param>
		/// <param name="lockGroup">Grupo do lock</param>
		/// <returns></returns>
		bool RegisterLock(string transaction, LockSession session, string hostName, Lockable lockableObject, int lockType, string lockGroup);

		/// <summary>
		/// Registra a solicitação de desbloqueio da inatância.
		/// </summary>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockableObject"></param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="transaction">Identificador únido da transação</param>
		/// <returns></returns>
		bool RegisterUnlock(string transaction, string hostName, Lockable lockableObject, int lockType);

		/// <summary>
		/// Processa uma transação do bloqueio/desbloqueio.
		/// </summary>
		/// <param name="transaction">Identificador únido da transação</param>
		/// <returns></returns>
		LockProcessResult ProcessTransaction(string transaction);

		/// <summary>
		/// Mata um grupo do locks.
		/// </summary>
		/// <param name="lockGroup"></param>
		void KillLockGroup(string lockGroup);
	}
}
