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

namespace Colosoft.Net.Share
{
	/// <summary>
	/// Classe que gerencia o compartilhamento de rede.
	/// </summary>
	public class NetworkShareManager : IEnumerable<DirectoryShare>
	{
		private static NetworkShareManager _instance = new NetworkShareManager();

		private List<DirectoryShare> _shares = new List<DirectoryShare>();

		/// <summary>
		/// Instancia única do gerenciador.
		/// </summary>
		public static NetworkShareManager Instance
		{
			get
			{
				return _instance;
			}
		}

		/// <summary>
		/// Quantidade de compartilhamentos registrados no gerenciador.
		/// </summary>
		public int Count
		{
			get
			{
				return _shares.Count;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public NetworkShareManager()
		{
			_shares = new List<DirectoryShare>();
		}

		/// <summary>
		/// Carrega a configuração para o gerenciador.
		/// </summary>
		/// <param name="configuration"></param>
		public void LoadConfiguration(NetworkShareConfiguration configuration)
		{
			configuration.Require("configuration").NotNull();
			foreach (var i in configuration.GetDirectories())
				Add(i);
		}

		/// <summary>
		/// Adiciona o compartilhamento para o gerenciador.
		/// </summary>
		/// <param name="share"></param>
		public void Add(DirectoryShare share)
		{
			share.Require("share").NotNull();
			var index = _shares.BinarySearch(share, DirectoryShare.DirectoryNameComparer);
			if(index < 0)
				_shares.Insert(~index, share);
			else
				throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.Argument_AddingDuplicate).Format());
		}

		/// <summary>
		/// Remove o compartilhamento.
		/// </summary>
		/// <param name="share"></param>
		/// <returns></returns>
		public bool Remove(DirectoryShare share)
		{
			if(share == null)
				return false;
			return _shares.Remove(share);
		}

		/// <summary>
		/// Cria a impersonação para o diretório compartilhada.
		/// </summary>
		/// <param name="directoryShare">Caminho do diretório de compartilhamento.</param>
		/// <returns></returns>
		public IDisposable CreateImpersonator(string directoryShare)
		{
			if(directoryShare == null)
				return new ImpersonatorFake();
			DirectoryShare dir = null;
			string dirName = null;
			foreach (var i in _shares)
			{
				if(i.Directory.Length < directoryShare.Length && directoryShare.StartsWith(i.Directory, StringComparison.InvariantCultureIgnoreCase) && (dirName == null || StringComparer.InvariantCultureIgnoreCase.Compare(i.Directory.Length, dirName.Length) > 0))
				{
					dirName = i.Directory;
					dir = i;
				}
			}
			if(dir == null)
				return new ImpersonatorFake();
			return new Impersonator(dir.Identity.Username, dir.Identity.Domain, dir.Identity.Password, LogonType.LOGON32_LOGON_NEW_CREDENTIALS, LogonProvider.LOGON32_PROVIDER_DEFAULT);
		}

		/// <summary>
		/// Recupera o enumerador dos compartilhamentos.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<DirectoryShare> GetEnumerator()
		{
			return _shares.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador dos compartilhamentos.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _shares.GetEnumerator();
		}

		/// <summary>
		/// Implementação fake do Impersonator.
		/// </summary>
		class ImpersonatorFake : IDisposable
		{
			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
			}
		}
	}
}
