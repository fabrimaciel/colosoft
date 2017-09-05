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
	/// Implementação do substituto de serialização de enums.
	/// </summary>
	internal sealed class EnumSerializationSurrogate : SerializationSurrogate
	{
		public EnumSerializationSurrogate(Type enm) : base(enm)
		{
		}

		public override object Read(CompactBinaryReader reader)
		{
			short handle = reader.ReadInt16();
			var surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForTypeHandle(handle, reader.Context.CacheContext);
			if(surrogateForTypeHandle == null)
				surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForSubTypeHandle(handle, reader.ReadInt16(), reader.Context.CacheContext);
			return Enum.ToObject(base.ActualType, surrogateForTypeHandle.Read(reader));
		}

		public override void Skip(CompactBinaryReader reader)
		{
			short handle = reader.ReadInt16();
			var surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForTypeHandle(handle, reader.Context.CacheContext);
			if(surrogateForTypeHandle == null)
				surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForSubTypeHandle(handle, reader.ReadInt16(), reader.Context.CacheContext);
			surrogateForTypeHandle.Skip(reader);
		}

		public override void Write(CompactBinaryWriter writer, object graph)
		{
			var surrogateForType = TypeSurrogateSelector.GetSurrogateForType(Enum.GetUnderlyingType(base.ActualType), writer.Context.CacheContext);
			writer.Write(surrogateForType.TypeHandle);
			surrogateForType.Write(writer, graph);
		}
	}
}
