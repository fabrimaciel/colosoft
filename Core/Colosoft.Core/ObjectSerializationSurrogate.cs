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
using System.Runtime.Serialization.Formatters.Binary;
using Colosoft.Serialization.IO;

namespace Colosoft.Serialization.Surrogates
{
	internal sealed class ObjectSerializationSurrogate : SerializationSurrogate
	{
		/// <summary>
		/// Instancia usada para serializar os dados de objetos.
		/// </summary>
		private static BinaryFormatter _formatter = new BinaryFormatter();

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="t"></param>
		public ObjectSerializationSurrogate(Type t) : base(t)
		{
		}

		/// <summary>
		/// Lê os dados e cria uma instancia preenchida.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public override object Read(CompactBinaryReader reader)
		{
			return _formatter.Deserialize(reader.BaseReader.BaseStream);
		}

		/// <summary>
		/// Salta os dados do leitor.
		/// </summary>
		/// <param name="reader"></param>
		public override void Skip(CompactBinaryReader reader)
		{
			Read(reader);
		}

		/// <summary>
		/// Serializa a instancia no escritor informado.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="graph"></param>
		public override void Write(CompactBinaryWriter writer, object graph)
		{
			_formatter.Serialize(writer.BaseWriter.BaseStream, graph);
		}
	}
}
