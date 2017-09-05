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
	internal class KeyValuePairSerializationSurrogate<K, V> : ContextSensitiveSerializationSurrogate
	{
		public KeyValuePairSerializationSurrogate() : base(typeof(KeyValuePair<K, V>))
		{
		}

		public override object ReadDirect(CompactBinaryReader reader, object graph)
		{
			KeyValuePair<K, V> pair = (KeyValuePair<K, V>)graph;
			K key = reader.ReadObjectAs<K>();
			return new KeyValuePair<K, V>(key, reader.ReadObjectAs<V>());
		}

		public override void SkipDirect(CompactBinaryReader reader, object graph)
		{
			KeyValuePair<K, V> pair1 = (KeyValuePair<K, V>)graph;
			reader.SkipObjectAs<K>();
			reader.SkipObjectAs<V>();
		}

		public override void WriteDirect(CompactBinaryWriter writer, object graph)
		{
			KeyValuePair<K, V> pair = (KeyValuePair<K, V>)graph;
			writer.WriteObjectAs<K>(pair.Key);
			writer.WriteObjectAs<V>(pair.Value);
		}
	}
}
