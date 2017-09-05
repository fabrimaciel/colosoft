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

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Armazena as informações do callback com os ados
	/// da alteração de consulta.
	/// </summary>
	[Serializable]
	public class CQCallbackInfo
	{
		private string _activeQueryId;

		private List<string> _clientIds = new List<string>();

		/// <summary>
		/// Identificadores dos clientes.
		/// </summary>
		public List<string> ClientIds
		{
			get
			{
				return _clientIds;
			}
			set
			{
				_clientIds = value;
			}
		}

		/// <summary>
		/// Identificador da consulta ativa.
		/// </summary>
		public string CQId
		{
			get
			{
				return _activeQueryId;
			}
			set
			{
				_activeQueryId = value;
			}
		}

		/// <summary>
		/// Compara a instancia com o objeto informado.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			CQCallbackInfo info = obj as CQCallbackInfo;
			return (info != null && CQId == info.CQId);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (_activeQueryId ?? "").GetHashCode() ^ _clientIds.GetHashCode();
		}
	}
}
