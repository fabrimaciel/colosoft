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
	/// Implementação do substituto de serialização para um vetor.
	/// </summary>
	public sealed class ArraySerializationSurrogate : ContextSensitiveSerializationSurrogate
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="t"></param>
		public ArraySerializationSurrogate(Type t) : base(t)
		{
		}

		/// <summary>
		/// Cria uma instancia.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public override object Instantiate(CompactBinaryReader reader)
		{
			int length = reader.ReadInt32();
			return Array.CreateInstance(base.ActualType.GetElementType(), length);
		}

		/// <summary>
		/// Lê os dados para a instancia diretamente.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="graph"></param>
		/// <returns></returns>
		public override object ReadDirect(CompactBinaryReader reader, object graph)
		{
			Array array = (Array)graph;
			for(int i = 0; i < array.Length; i++)
				array.SetValue(reader.ReadObject(), i);
			return array;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="graph"></param>
		public override void SkipDirect(CompactBinaryReader reader, object graph)
		{
			Array array = (Array)graph;
			for(int i = 0; i < array.Length; i++)
				reader.SkipObject();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="graph"></param>
		public override void WriteDirect(CompactBinaryWriter writer, object graph)
		{
			Array array = (Array)graph;
			writer.Write(array.Length);
			for(int i = 0; i < array.Length; i++)
				writer.WriteObject(array.GetValue(i));
		}
	}
}
