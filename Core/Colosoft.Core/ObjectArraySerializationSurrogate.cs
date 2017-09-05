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
using Colosoft.Serialization.IO;

namespace Colosoft.Serialization.Surrogates
{
	/// <summary>
	/// Implementação do substituto de serialização para vetor de objetos.
	/// </summary>
	internal sealed class ObjectArraySerializationSurrogate : ContextSensitiveSerializationSurrogate
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ObjectArraySerializationSurrogate() : base(typeof(object[]))
		{
		}

		/// <summary>
		/// Cria a instancia do tipo associado.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public override object Instantiate(CompactBinaryReader reader)
		{
			return new object[reader.ReadInt32()];
		}

		/// <summary>
		/// Lê os dados diretamente.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="graph"></param>
		/// <returns></returns>
		public override object ReadDirect(CompactBinaryReader reader, object graph)
		{
			object[] objArray = (object[])graph;
			short handle = reader.ReadInt16();
			var surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForTypeHandle(handle, reader.CacheContext);
			if(surrogateForTypeHandle == null)
				surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForSubTypeHandle(handle, reader.ReadInt16(), reader.Context.CacheContext);
			object obj2 = Array.CreateInstance(surrogateForTypeHandle.ActualType, objArray.Length);
			for(int i = 0; i < objArray.Length; i++)
				((Array)obj2).SetValue(reader.ReadObject(), i);
			return obj2;
		}

		/// <summary>
		/// Salta os dados do leitor.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="graph"></param>
		public override void SkipDirect(CompactBinaryReader reader, object graph)
		{
			object[] objArray = (object[])graph;
			short handle = reader.ReadInt16();
			if(TypeSurrogateSelector.GetSurrogateForTypeHandle(handle, reader.CacheContext) == null)
				TypeSurrogateSelector.GetSurrogateForSubTypeHandle(handle, reader.ReadInt16(), reader.Context.CacheContext);
			for(int i = 0; i < objArray.Length; i++)
				reader.SkipObject();
		}

		/// <summary>
		/// Escreve os dados diretamente.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="graph"></param>
		public override void WriteDirect(CompactBinaryWriter writer, object graph)
		{
			object[] objArray = (object[])graph;
			writer.Write(objArray.Length);
			if(typeof(object[]).Equals(graph.GetType()))
			{
				ISerializationSurrogate surrogateForObject = TypeSurrogateSelector.GetSurrogateForObject(new object(), writer.CacheContext);
				writer.Write(surrogateForObject.TypeHandle);
			}
			else
			{
				object obj2 = null;
				for(int j = 0; j < objArray.Length; j++)
				{
					if(objArray[j] != null)
					{
						obj2 = objArray[j];
						break;
					}
				}
				ISerializationSurrogate surrogate = TypeSurrogateSelector.GetSurrogateForObject(obj2, writer.CacheContext);
				writer.Write(surrogate.TypeHandle);
				if(surrogate.SubTypeHandle > 0)
					writer.Write(surrogate.SubTypeHandle);
			}
			for(int i = 0; i < objArray.Length; i++)
				writer.WriteObject(objArray[i]);
		}
	}
}
