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

namespace Colosoft.Caching.Policies
{
	/// <summary>
	/// Representa uma dica do tipo de contador.
	/// </summary>
	[Serializable]
	internal class CounterHint : EvictionHint, ICompactSerializable
	{
		protected short _count;

		/// <summary>
		/// Quantidade registrada para a instancia.
		/// </summary>
		public short Count
		{
			get
			{
				return _count;
			}
		}

		/// <summary>
		/// Verifica se a dica é variante.
		/// </summary>
		public override bool IsVariant
		{
			get
			{
				return _count < 0x7fff;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CounterHint()
		{
			_count = 1;
			base.HintType = EvictionHintType.CounterHint;
		}

		/// <summary>
		/// Inicializa a instancia já com uma quantidade para o contador.
		/// </summary>
		/// <param name="count"></param>
		public CounterHint(short count)
		{
			_count = 1;
			base.HintType = EvictionHintType.CounterHint;
			_count = count;
		}

		/// <summary>
		/// Atualiza o contador.
		/// </summary>
		/// <returns></returns>
		public override bool Update()
		{
			if(_count < 0x7fff)
			{
				_count = (short)(_count + 1);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Deserializa os dados da instancia.
		/// </summary>
		/// <param name="reader"></param>
		void ICompactSerializable.Deserialize(CompactReader reader)
		{
			base.Deserialize(reader);
			_count = reader.ReadInt16();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		void ICompactSerializable.Serialize(CompactWriter writer)
		{
			base.Serialize(writer);
			writer.Write(_count);
		}
	}
}
