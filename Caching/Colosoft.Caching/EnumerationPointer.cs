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

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Representa um ponteiro para uma enumeração.
	/// </summary>
	public class EnumerationPointer : ICompactSerializable
	{
		/// <summary>
		/// Identificador do pedaço.
		/// </summary>
		public int ChunkId
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se já foi finalizado.
		/// </summary>
		public bool HasFinished
		{
			get
			{
				return (ChunkId == -1);
			}
		}

		/// <summary>
		/// Identificador do ponteiro.
		/// </summary>
		public string Id
		{
			get;
			private set;
		}

		/// <summary>
		/// Verifica se é uma instancia liberavel.
		/// </summary>
		public bool IsDisposable
		{
			get;
			set;
		}

		/// <summary>
		/// Verifiac se é um grupo de ponteiro.
		/// </summary>
		public virtual bool IsGroupPointer
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Identifica se o socket do servidor foi liberado.
		/// </summary>
		public bool IsSocketServerDispose
		{
			get;
			set;
		}

		/// <summary>
		/// Endereço IP do nó.
		/// </summary>
		public string NodeIpAddress
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public EnumerationPointer()
		{
			Id = Guid.NewGuid().ToString();
			ChunkId = -1;
			NodeIpAddress = string.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id">Identifiador do ponteiro.</param>
		/// <param name="chunkId">Identificador do pedaço.</param>
		public EnumerationPointer(string id, int chunkId)
		{
			Id = Guid.NewGuid().ToString();
			ChunkId = -1;
			NodeIpAddress = string.Empty;
			Id = id;
			ChunkId = chunkId;
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			Id = reader.ReadString();
			ChunkId = reader.ReadInt32();
			IsDisposable = reader.ReadBoolean();
			NodeIpAddress = reader.ReadString();
			IsSocketServerDispose = reader.ReadBoolean();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			writer.Write(Id);
			writer.Write(ChunkId);
			writer.Write(IsDisposable);
			writer.Write(NodeIpAddress);
			writer.Write(IsSocketServerDispose);
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			bool flag = false;
			if(obj is EnumerationPointer)
			{
				EnumerationPointer pointer = obj as EnumerationPointer;
				flag = Id.Equals(pointer.Id);
			}
			return flag;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if(Id != null)
				return this.Id.GetHashCode();
			return base.GetHashCode();
		}

		/// <summary>
		/// Reseta.
		/// </summary>
		public void Reset()
		{
			this.ChunkId = -1;
		}
	}
}
