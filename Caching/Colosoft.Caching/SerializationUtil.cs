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
using System.Collections;
using Colosoft.Serialization.IO;
using System.IO;
using Colosoft.Serialization.Formatters;
using System.Reflection;
using Colosoft.Serialization;

namespace Colosoft.Caching.Util
{
	/// <summary>
	/// Classes que contém métodos que auxiliam na serialização dos dados.
	/// </summary>
	public class SerializationUtil
	{
		private static Hashtable _attributeOrder = new Hashtable();

		private static Hashtable _portibilaty = new Hashtable();

		private static Hashtable _subTypeHandle = new Hashtable();

		/// <summary>
		/// Verifica se as informações já estão registradas.
		/// </summary>
		/// <param name="portable"></param>
		/// <param name="cacheContext"></param>
		/// <param name="handle"></param>
		/// <param name="type"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private static bool CheckAlreadyRegistered(bool portable, string cacheContext, string handle, Type type, string typeName)
		{
			if(portable)
			{
				if((_subTypeHandle.Contains(cacheContext) && ((Hashtable)_subTypeHandle[cacheContext]).Contains(handle)) && ((Hashtable)((Hashtable)_subTypeHandle[cacheContext])[handle]).Contains(type))
					return true;
			}
			else if(_attributeOrder.Contains(cacheContext) && ((Hashtable)_attributeOrder[cacheContext]).Contains(typeName))
				return true;
			return false;
		}

		/// <summary>
		/// Realiza a deserialização compacta.
		/// </summary>
		/// <param name="buffer">Buffer com os dados.</param>
		/// <param name="cacheContext">Nome do contexto do cache.</param>
		/// <returns></returns>
		public static object CompactDeserialize(object buffer, string cacheContext)
		{
			object obj2 = buffer;
			if(((buffer != null) && (buffer is byte[])) && HasHCHeader((byte[])buffer))
			{
				using (var stream = new MemoryStream((byte[])buffer))
				{
					stream.Position += CachingHeader.Length;
					return CompactBinaryFormatter.Deserialize(stream, cacheContext);
				}
			}
			return obj2;
		}

		/// <summary>
		/// Realiza a serialização compacta.
		/// </summary>
		/// <param name="graph">Instancia qeue será serializada.</param>
		/// <param name="cacheContext">Contexto da cache.</param>
		/// <returns>Dados da instancia serializada.</returns>
		public static object CompactSerialize(object graph, string cacheContext)
		{
			if((graph != null) && (graph is ICompactSerializable))
			{
				using (var stream = new MemoryStream())
				{
					stream.Write(CachingHeader.Version, 0, CachingHeader.Length);
					CompactBinaryFormatter.Serialize(stream, graph, cacheContext);
					return stream.ToArray();
				}
			}
			return graph;
		}

		/// <summary>
		/// Verifica se no buffer informado existe o cabeçalho do cache.
		/// </summary>
		/// <param name="serializedBuffer"></param>
		/// <returns></returns>
		internal static bool HasHCHeader(byte[] serializedBuffer)
		{
			byte[] v = new byte[5];
			if(serializedBuffer.Length < CachingHeader.Length)
			{
				return false;
			}
			for(int i = 0; i < CachingHeader.Length; i++)
			{
				v[i] = serializedBuffer[i];
			}
			return CachingHeader.CompareTo(v);
		}

		/// <summary>
		/// Realiza a deserialização segura do objeto.
		/// </summary>
		/// <param name="serializedObject"></param>
		/// <param name="serializationContext"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static object SafeDeserialize(object serializedObject, string serializationContext, BitSet flag)
		{
			object obj2 = serializedObject;
			try
			{
				if(serializedObject is byte[])
				{
					return CompactBinaryFormatter.FromByteBuffer((byte[])serializedObject, serializationContext);
				}
				if(serializedObject is UserBinaryObject)
				{
					obj2 = CompactBinaryFormatter.FromByteBuffer(((UserBinaryObject)serializedObject).GetFullObject(), serializationContext);
				}
			}
			catch(Exception)
			{
				obj2 = serializedObject;
			}
			return obj2;
		}

		/// <summary>
		/// Realiza a serilização segura do objeto
		/// </summary>
		/// <param name="serializableObject"></param>
		/// <param name="serializationContext"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static object SafeSerialize(object serializableObject, string serializationContext, ref BitSet flag)
		{
			if(serializableObject != null)
				serializableObject = CompactBinaryFormatter.ToByteBuffer(serializableObject, serializationContext);
			return serializableObject;
		}
	}
}
