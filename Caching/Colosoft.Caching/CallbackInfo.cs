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
	/// Representa as informações de uma chamada de retorno.
	/// </summary>
	public class CallbackInfo : ICompactSerializable
	{
		/// <summary>
		/// Objeto do callback.
		/// </summary>
		protected object _theCallback;

		/// <summary>
		/// Nome do cliente que vai receber o callback.
		/// </summary>
		protected string _theClient;

		/// <summary>
		/// Objeto de callback.
		/// </summary>
		public object Callback
		{
			get
			{
				return _theCallback;
			}
			set
			{
				_theCallback = value;
			}
		}

		/// <summary>
		/// Nome do cliente.
		/// </summary>
		public string Client
		{
			get
			{
				return _theClient;
			}
			set
			{
				_theClient = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CallbackInfo()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="callback"></param>
		public CallbackInfo(string client, object callback)
		{
			_theClient = client;
			_theCallback = callback;
		}

		/// <summary>
		/// Compara a instancia com o objeto informado.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if(!(obj is CallbackInfo))
				return false;
			CallbackInfo info = obj as CallbackInfo;
			if(info.Client != _theClient)
				return false;
			if((info.Callback is short) && (_theCallback is short))
			{
				if(((short)info.Callback) != ((short)_theCallback))
					return false;
			}
			else if(info.Callback != _theCallback)
				return false;
			return true;
		}

		/// <summary>
		/// Recupera o hashcode da instancia.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (_theCallback != null ? _theCallback.GetHashCode() : 0) ^ (_theClient ?? "").GetHashCode();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = (_theClient != null) ? _theClient : "NULL";
			string str2 = (_theCallback != null) ? _theCallback.ToString() : "NULL";
			return (str + ":" + str2);
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public virtual void Serialize(CompactWriter writer)
		{
			writer.WriteObject(_theClient);
			writer.WriteObject(_theCallback);
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public virtual void Deserialize(CompactReader reader)
		{
			_theClient = (string)reader.ReadObject();
			_theCallback = reader.ReadObject();
		}
	}
}
