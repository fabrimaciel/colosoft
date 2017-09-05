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

namespace Colosoft.IO.VirtualStorage
{
	/// <summary>
	/// Assinatura da classe responsável pela sincronização do
	/// armazenamento virtual.
	/// </summary>
	public interface IStorageSync
	{
		/// <summary>
		/// Inicializa o processo de sincronização de forma assincrona.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult BeginSync(AsyncCallback callback, object state);

		/// <summary>
		/// Finaliza o processo de sincronização de forma assincrona.
		/// </summary>
		/// <param name="ar"></param>
		VirtualStorageSyncResult EndSync(IAsyncResult ar);

		/// <summary>
		/// Sincroniza dos arquivos do diretório virtual.
		/// </summary>
		VirtualStorageSyncResult Sync();
	}
	/// <summary>
	/// Armazena os dados de uma falha do processo de sincronização.
	/// </summary>
	public class VirtualStorageSyncFailure
	{
		/// <summary>
		/// Mensagem da falha ocorrida.
		/// </summary>
		public IMessageFormattable Message
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Armazena o resultado do processo de sincronização.
	/// </summary>
	public class VirtualStorageSyncResult
	{
		private VirtualStorageSyncFailure[] _failures = new VirtualStorageSyncFailure[0];

		/// <summary>
		/// Identifica se a sincronização foi realizada com sucesso.
		/// </summary>
		public bool Success
		{
			get
			{
				return Failures.Length == 0;
			}
		}

		/// <summary>
		/// Relação das falhas do resultado.
		/// </summary>
		public VirtualStorageSyncFailure[] Failures
		{
			get
			{
				return _failures;
			}
			set
			{
				_failures = value ?? new VirtualStorageSyncFailure[0];
			}
		}
	}
}
