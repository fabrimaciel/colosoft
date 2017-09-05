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
using Colosoft.Serialization.Formatters;
using Colosoft.Serialization.IO;

namespace Colosoft.Caching.Storage
{
	/// <summary>
	/// Representa um item armazenado.
	/// </summary>
	[Serializable]
	internal class StoreItem : ICompactSerializable
	{
		/// <summary>
		/// Chave do item.
		/// </summary>
		public object Key;

		/// <summary>
		/// Valor do item.
		/// </summary>
		public object Value;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public StoreItem()
		{
		}

		/// <summary>
		/// Cria uma instancia já informado a chave e valor.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		public StoreItem(object key, object val)
		{
			this.Key = key;
			this.Value = val;
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		void ICompactSerializable.Deserialize(CompactReader reader)
		{
			this.Key = reader.ReadObject();
			this.Value = reader.ReadObject();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		void ICompactSerializable.Serialize(CompactWriter writer)
		{
			writer.WriteObject(this.Key);
			writer.WriteObject(this.Value);
		}

		/// <summary>
		/// Recupera os dados do buffer informado.
		/// </summary>
		/// <param name="buffer">Buffer onde os dados estão.</param>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		public static StoreItem FromBinary(byte[] buffer, string cacheContext)
		{
			return (StoreItem)CompactBinaryFormatter.FromByteBuffer(buffer, cacheContext);
		}

		/// <summary>
		/// Salva a chave o valor informados em um buffer.
		/// </summary>
		/// <param name="key">Chave do item.</param>
		/// <param name="val">Valor do item.</param>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		public static byte[] ToBinary(object key, object val, string cacheContext)
		{
			StoreItem graph = new StoreItem(key, val);
			return CompactBinaryFormatter.ToByteBuffer(graph, cacheContext);
		}
	}
}
