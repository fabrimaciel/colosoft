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
	internal sealed class NullableArraySerializationSurrogate<T> : SerializationSurrogate where T : struct
	{
		public NullableArraySerializationSurrogate() : base(typeof(T?[]))
		{
		}

		public override object Read(CompactBinaryReader reader)
		{
			short handle = reader.ReadInt16();
			var surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForTypeHandle(handle, null);
			if(surrogateForTypeHandle == null)
				surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForSubTypeHandle(handle, reader.ReadInt16(), reader.Context.CacheContext);
			T?[] nullableArray = new T?[reader.ReadInt32()];
			while (true)
			{
				int index = reader.ReadInt32();
				if(index < 0)
					return nullableArray;
				nullableArray[index] = new T?((T)surrogateForTypeHandle.Read(reader));
			}
		}

		public override void Skip(CompactBinaryReader reader)
		{
			short handle = reader.ReadInt16();
			var surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForTypeHandle(handle, null);
			if(surrogateForTypeHandle == null)
				surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForSubTypeHandle(handle, reader.ReadInt16(), reader.Context.CacheContext);
			int num2 = reader.ReadInt32();
			while (reader.ReadInt32() >= 0)
				surrogateForTypeHandle.Skip(reader);
		}

		public override void Write(CompactBinaryWriter writer, object graph)
		{
			ISerializationSurrogate surrogateForType = TypeSurrogateSelector.GetSurrogateForType(typeof(T), null);
			T?[] nullableArray = (T?[])graph;
			writer.Write(surrogateForType.TypeHandle);
			writer.Write(nullableArray.Length);
			for(int i = 0; i < nullableArray.Length; i++)
			{
				if(nullableArray[i].HasValue)
				{
					writer.Write(i);
					surrogateForType.Write(writer, nullableArray[i].Value);
				}
			}
			writer.Write(-1);
		}
	}
}
