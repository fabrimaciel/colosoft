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

namespace Colosoft.Query.Database.Generic
{
	/// <summary>
	/// Implementação que representa uma transação da StoredProcedure do GDA.
	/// </summary>
	public class GDAStoredProcedureTransaction : IStoredProcedureTransaction
	{
		private GDA.GDASession _session;

		private string _providerName;

		/// <summary>
		/// Instancia da sessão associada.
		/// </summary>
		public GDA.GDASession Session
		{
			get
			{
				return _session;
			}
		}

		/// <summary>
		/// Nome do provedor.
		/// </summary>
		public string ProviderName
		{
			get
			{
				return _providerName;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="session">Instancia da sessão associada.</param>
		/// <param name="providerName">Nome do provedor associado.</param>
		public GDAStoredProcedureTransaction(GDA.GDASession session, string providerName)
		{
			session.Require("session").NotNull();
			_session = session;
			_providerName = providerName;
		}
	}
}
