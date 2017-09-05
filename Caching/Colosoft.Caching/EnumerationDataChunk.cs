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

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Representa a enumeração de pedaços de dados.
	/// </summary>
	public class EnumerationDataChunk : ICompactSerializable
	{
		/// <summary>
		/// Lista dos dados.
		/// </summary>
		private List<string> _data;

		private EnumerationPointer _pointer;

		/// <summary>
		/// Dados.
		/// </summary>
		public List<string> Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
			}
		}

		/// <summary>
		/// Identifica se é o ultimo pedaço.
		/// </summary>
		public bool IsLastChunk
		{
			get
			{
				return _pointer.HasFinished;
			}
		}

		/// <summary>
		/// Ponteiro associado.
		/// </summary>
		public EnumerationPointer Pointer
		{
			get
			{
				return _pointer;
			}
			set
			{
				_pointer = value;
			}
		}

		/// <summary>
		/// Deserializa os dados da instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			_data = (List<string>)reader.ReadObject();
			_pointer = (EnumerationPointer)reader.ReadObject();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			writer.WriteObject(_data);
			writer.WriteObject(_pointer);
		}
	}
}
