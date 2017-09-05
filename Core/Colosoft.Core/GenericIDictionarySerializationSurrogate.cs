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
using System.Collections;

namespace Colosoft.Serialization.Surrogates
{
	/// <summary>
	/// Implementação do substituto de serialização para <see cref="IDictionary"/>.
	/// </summary>
	internal sealed class GenericIDictionarySerializationSurrogate : ContextSensitiveSerializationSurrogate
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="type"></param>
		public GenericIDictionarySerializationSurrogate(Type type) : base(type)
		{
		}

		public override object Instantiate(CompactBinaryReader reader)
		{
			int num = reader.ReadInt32();
			Type[] types = new Type[num];
			for(int i = 0; i < num; i++)
			{
				string typeName = reader.ReadString();
				types[i] = Type.GetType(typeName);
			}
			return SurrogateHelper.CreateGenericType("System.Collections.Generic.Dictionary", types);
		}

		public override object ReadDirect(CompactBinaryReader reader, object graph)
		{
			int num = reader.ReadInt32();
			IDictionary dictionary = graph as IDictionary;
			for(int i = 0; i < num; i++)
			{
				object key = reader.ReadObject();
				object obj3 = reader.ReadObject();
				dictionary.Add(key, obj3);
			}
			return graph;
		}

		public override void SkipDirect(CompactBinaryReader reader, object graph)
		{
			int num = reader.ReadInt32();
			for(int i = 0; i < num; i++)
			{
				reader.SkipObject();
				reader.SkipObject();
			}
		}

		public override void WriteDirect(CompactBinaryWriter writer, object graph)
		{
			Type[] genericArguments = graph.GetType().GetGenericArguments();
			writer.Write(genericArguments.Length);
			for(int i = 0; i < genericArguments.Length; i++)
				writer.Write(genericArguments[i].AssemblyQualifiedName);
			IDictionary dictionary = (IDictionary)graph;
			writer.Write(dictionary.Count);
			IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				writer.WriteObject(enumerator.Key);
				writer.WriteObject(enumerator.Value);
			}
		}
	}
}
