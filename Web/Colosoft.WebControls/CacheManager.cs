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
using System.Threading;

namespace Colosoft.WebControls.Route.Security
{
	public class CachingRulesEventArgs : EventArgs
	{
		public CachingRulesEventArgs()
		{
			this.ClearCache = true;
		}

		/// <summary>
		/// Identifica se o cache foi limpo
		/// </summary>
		public bool ClearCache
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Classe estática responsável por gerenciar o cache e que é compartilhada por toda a aplicação.
	/// Além de manter a coleção das políticas da aplicação, também possui um objeto do tipo ReaderWriterLockSlim
	/// (namespace System.Threading) que gerencia o acesso concorrente ao cache. 
	/// Esse tipo de objeto garante múltiplas threads para leitura ou o acesso exclusivo para escrita.
	/// </summary>
	public static class CacheManager
	{
		private static AuthorizationRuleCollection _rules;

		private static object objLock = new object();

		/// <summary>
		/// Armazenar em cache todas as políticas de acesso da aplicação corrente.
		/// </summary>
		public static AuthorizationRuleCollection Rules
		{
			get
			{
				lock (objLock)
					return _rules;
			}
			set
			{
				lock (objLock)
					_rules = value;
			}
		}
	}
}
