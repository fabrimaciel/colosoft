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
using Colosoft.Serialization;
using Colosoft.Serialization.IO;

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa o valor comprimido de uma entrada.
	/// </summary>
	public class CompressedValueEntry : ICompactSerializable
	{
		/// <summary>
		/// Flag associado com a entrada.
		/// </summary>
		public BitSet Flag;

		/// <summary>
		/// Valor da entrada.
		/// </summary>
		public object Value;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CompressedValueEntry()
		{
		}

		/// <summary>
		/// Cria a instancia com os valores iniciais.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="flag"></param>
		public CompressedValueEntry(object value, BitSet flag)
		{
			this.Value = value;
			this.Flag = flag;
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			this.Value = reader.ReadObject();
			this.Flag = reader.ReadObject() as BitSet;
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			writer.WriteObject(this.Value);
			writer.WriteObject(this.Flag);
		}
	}
}
