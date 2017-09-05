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

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa a configuração do serviço do cache.
	/// </summary>
	public class ServiceConfiguration
	{
		private static int _evictionBulkRemoveDelay = 0;

		private static int _evictionBulkRemoveSize = 10;

		private static int _expirationBulkRemoveDelay = 0;

		private static int _expirationBulkRemoveSize = 10;

		/// <summary>
		/// Delay para a remoção de uma liberação.
		/// </summary>
		public static int EvictionBulkRemoveDelay
		{
			get
			{
				return _evictionBulkRemoveDelay;
			}
			set
			{
				_evictionBulkRemoveDelay = value;
			}
		}

		/// <summary>
		/// Tamanho para a remoção de uma liberação.
		/// </summary>
		public static int EvictionBulkRemoveSize
		{
			get
			{
				return _evictionBulkRemoveSize;
			}
			set
			{
				_evictionBulkRemoveSize = value;
			}
		}

		/// <summary>
		/// Delay para remoção de valume da expiração.
		/// </summary>
		public static int ExpirationBulkRemoveDelay
		{
			get
			{
				return _expirationBulkRemoveDelay;
			}
			set
			{
				_expirationBulkRemoveDelay = value;
			}
		}

		/// <summary>
		/// Tamanho que será usado na remoção da expiração.
		/// </summary>
		public static int ExpirationBulkRemoveSize
		{
			get
			{
				return _expirationBulkRemoveSize;
			}
			set
			{
				_expirationBulkRemoveSize = value;
			}
		}

		static ServiceConfiguration()
		{
			Load();
		}

		/// <summary>
		/// Carrega os dados da instancia.
		/// </summary>
		public static void Load()
		{
		}
	}
}
