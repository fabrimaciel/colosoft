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
using System.IO;
using Colosoft.Serialization.IO;
using Colosoft.Serialization.Surrogates;
using Colosoft;
using Colosoft.Runtime;

namespace Colosoft.Serialization.Formatters
{
	/// <summary>
	/// Classe responsável pelos método de serialização e deserialização compactos.
	/// </summary>
	public class CompactBinaryFormatter
	{
		/// <summary>
		/// Deserializa o objeto que possui seus dados contidos na stream informada.
		/// </summary>
		/// <param name="stream">Stream onde estão os dados serializados.</param>
		/// <param name="cacheContext">Nome do contexto.</param>
		/// <returns></returns>
		public static object Deserialize(Stream stream, string cacheContext)
		{
			using (var reader = new CompactBinaryReader(stream))
				return Deserialize(reader, cacheContext, false);
		}

		/// <summary>
		/// Deserializa o objeto a partir do leitor.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="cacheContext"></param>
		/// <param name="skip">Identifica se é para saltar a deserialização.</param>
		/// <returns></returns>
		internal static object Deserialize(CompactBinaryReader reader, string cacheContext, bool skip)
		{
			short handle = reader.ReadInt16();
			reader.Context.CacheContext = cacheContext;
			var surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForTypeHandle(handle, cacheContext);
			if(surrogateForTypeHandle == null)
				surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForSubTypeHandle(handle, reader.ReadInt16(), cacheContext);
			if(surrogateForTypeHandle == null)
				throw new CompactSerializationException(ResourceMessageFormatter.Create(() => Properties.Resources.CompactSerialization_TypeHandleNotRegistered, handle).Format());
			if(!skip)
				return surrogateForTypeHandle.Read(reader);
			surrogateForTypeHandle.Skip(reader);
			return null;
		}

		/// <summary>
		/// Deserializa os dados da stream em um objeto.
		/// </summary>
		/// <param name="stream">Stream onde os dados estão armazenados.</param>
		/// <param name="cacheContext"></param>
		/// <param name="closeStream">True caso a stream seja fechada quando a operação finalizar.</param>
		/// <returns></returns>
		public static object Deserialize(Stream stream, string cacheContext, bool closeStream)
		{
			CompactBinaryReader reader = new CompactBinaryReader(stream);
			object obj2 = Deserialize(reader, cacheContext, false);
			reader.Dispose(closeStream);
			return obj2;
		}

		/// <summary>
		/// Deserializa os dados da stream em um objeto.
		/// </summary>
		/// <param name="stream">Stream onde os dados estão armazenados.</param>
		/// <param name="cacheContext"></param>
		/// <param name="closeStream">True caso a stream seja fechada quando a operação finalizar.</param>
		/// <param name="memManager">Gerenciador de memória que será utilizado na operação.</param>
		/// <returns></returns>
		public static object Deserialize(Stream stream, string cacheContext, bool closeStream, MemoryManager memManager)
		{
			var reader = new CompactBinaryReader(stream);
			reader.Context.MemManager = memManager;
			object obj2 = Deserialize(reader, cacheContext, false);
			reader.Dispose(closeStream);
			return obj2;
		}

		/// <summary>
		/// Recupera um instancia a partir dos dados serializados no buffer informado.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		public static object FromByteBuffer(byte[] buffer, string cacheContext)
		{
			using (var stream = new MemoryStream(buffer))
				return Deserialize(stream, cacheContext);
		}

		/// <summary>
		/// Serializa a instancia no escritor.
		/// </summary>
		/// <param name="writer">Escritor onde os dados serão salvos.</param>
		/// <param name="graph">Instancia que será serializada.</param>
		/// <param name="cacheContext"></param>
		internal static void Serialize(CompactBinaryWriter writer, object graph, string cacheContext)
		{
			var surrogateForObject = TypeSurrogateSelector.GetSurrogateForObject(graph, cacheContext);
			writer.Context.CacheContext = cacheContext;
			writer.Write(surrogateForObject.TypeHandle);
			surrogateForObject.Write(writer, graph);
		}

		/// <summary>
		/// Serializa a instancia na stream.
		/// </summary>
		/// <param name="stream">Stream onde os dados serão salvos.</param>
		/// <param name="graph">Instancia que será serializada.</param>
		/// <param name="cacheContext"></param>
		public static void Serialize(Stream stream, object graph, string cacheContext)
		{
			using (var writer = new CompactBinaryWriter(stream))
				Serialize(writer, graph, cacheContext);
		}

		/// <summary>
		/// Serializa a instancia na stream.
		/// </summary>
		/// <param name="stream">Stream onde os dados serão salvos.</param>
		/// <param name="graph">Instancia que será serializada.</param>
		/// <param name="cacheContext"></param>
		/// <param name="closeStream">True caso deseja que a stream seja fechada no final a operação.</param>
		public static void Serialize(Stream stream, object graph, string cacheContext, bool closeStream)
		{
			CompactBinaryWriter writer = new CompactBinaryWriter(stream);
			Serialize(writer, graph, cacheContext);
			writer.Dispose(closeStream);
		}

		/// <summary>
		/// Serializa a instancia na stream.
		/// </summary>
		/// <param name="stream">Stream onde os dados serão salvos.</param>
		/// <param name="graph">Instancia que será serializada.</param>
		/// <param name="cacheContext"></param>
		/// <param name="closeStream">True caso deseja que a stream seja fechada no final a operação.</param>
		/// <param name="objManager"></param>
		public static void Serialize(Stream stream, object graph, string cacheContext, bool closeStream, MemoryManager objManager)
		{
			var writer = new CompactBinaryWriter(stream);
			writer.Context.MemManager = objManager;
			Serialize(writer, graph, cacheContext);
			writer.Dispose(closeStream);
		}

		/// <summary>
		/// Serializa a instancia informada e recupera um buffer com os dados.
		/// </summary>
		/// <param name="graph"></param>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		public static byte[] ToByteBuffer(object graph, string cacheContext)
		{
			using (var stream = new MemoryStream())
			{
				Serialize(stream, graph, cacheContext);
				return stream.ToArray();
			}
		}
	}
}
