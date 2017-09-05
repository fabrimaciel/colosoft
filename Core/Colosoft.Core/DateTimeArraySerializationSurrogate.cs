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
	/// Implementação do subtituto de serialização para um vetor de <see cref="DateTime"/>.
	/// </summary>
	internal sealed class DateTimeArraySerializationSurrogate : ContextSensitiveSerializationSurrogate
	{
		public DateTimeArraySerializationSurrogate() : base(typeof(DateTime[]))
		{
		}

		public override object Instantiate(CompactBinaryReader reader)
		{
			return new DateTime[reader.ReadInt32()];
		}

		public override object ReadDirect(CompactBinaryReader reader, object graph)
		{
			DateTime[] timeArray = (DateTime[])graph;
			for(int i = 0; i < timeArray.Length; i++)
				timeArray[i] = reader.ReadDateTime();
			return timeArray;
		}

		public override void SkipDirect(CompactBinaryReader reader, object graph)
		{
			DateTime[] timeArray = (DateTime[])graph;
			for(int i = 0; i < timeArray.Length; i++)
				reader.SkipDateTime();
		}

		public override void WriteDirect(CompactBinaryWriter writer, object graph)
		{
			DateTime[] timeArray = (DateTime[])graph;
			writer.Write(timeArray.Length);
			for(int i = 0; i < timeArray.Length; i++)
				writer.Write(timeArray[i]);
		}
	}
}
