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
using Colosoft.Serialization;
using Colosoft.Serialization.IO;

namespace Colosoft.Caching
{
	/// <summary>
	/// Informações de uma chamada assincrona.
	/// </summary>
	public class AsyncCallbackInfo : CallbackInfo
	{
		private int _requestId;

		/// <summary>
		/// Identificador da requisição.
		/// </summary>
		public int RequestID
		{
			get
			{
				return _requestId;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public AsyncCallbackInfo()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="requestId">Identificador da requisição.</param>
		/// <param name="clientId">Identificador do cliente.</param>
		/// <param name="asyncCallback">Objecto da chamada assincrona.</param>
		public AsyncCallbackInfo(int requestId, string clientId, object asyncCallback) : base(clientId, asyncCallback)
		{
			_requestId = requestId;
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public override void Deserialize(CompactReader reader)
		{
			base.Deserialize(reader);
			_requestId = reader.ReadInt32();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public override void Serialize(CompactWriter writer)
		{
			base.Serialize(writer);
			writer.Write(_requestId);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if(!(obj is CallbackInfo))
				return false;
			CallbackInfo info = obj as CallbackInfo;
			if(info.Client != base.Client)
				return false;
			if((info.Callback is short) && (base._theCallback is short))
			{
				if(((short)info.Callback) != ((short)base._theCallback))
					return false;
			}
			else if(info.Callback != base._theCallback)
				return false;
			return true;
		}

		/// <summary>
		/// Hashcode da instancia.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode() ^ _requestId.GetHashCode();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = (base._theClient != null) ? base._theClient : "NULL";
			string str2 = (base._theCallback != null) ? base._theCallback.ToString() : "NULL";
			return (str + ":" + str2);
		}
	}
}
