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

namespace Colosoft.Lock
{
	/// <summary>
	/// Tipos de retorno de um processo de locar ou desalocar um objeto.
	/// </summary>
	public enum LockProcessResultType
	{
		/// <summary>
		/// Sucesso.
		/// </summary>
		Success,
		/// <summary>
		/// Objeto com versão inválida.
		/// </summary>
		InvalidVersion,
		/// <summary>
		/// Loque inexistente.
		/// </summary>
		LackingLock,
		/// <summary>
		/// Token inválido.
		/// </summary>
		InvalidToken,
		/// <summary>
		/// Instância já bloqueada.
		/// </summary>
		InstanceLocked,
		/// <summary>
		/// Erro.
		/// </summary>
		Error
	}
	/// <summary>
	/// Representa o resultado de um processo de bloqueio.
	/// </summary>
	public class LockProcessResult
	{
		/// <summary>
		/// Tipo do retorno.
		/// </summary>
		public LockProcessResultType ProcessResult
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem retornada.
		/// </summary>
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Lista de identificadores de lock afetadas pelo comando.
		/// </summary>
		public LockSession Session
		{
			get;
			set;
		}
	}
}
