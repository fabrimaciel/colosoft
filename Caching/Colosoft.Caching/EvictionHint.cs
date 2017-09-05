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
	/// Possíveis tipos de dicas de liberação.
	/// </summary>
	public enum EvictionHintType
	{
		/// <summary>
		/// Despejo por contador.
		/// </summary>
		CounterHint = 1,
		/// <summary>
		/// None.
		/// </summary>
		NULL = -1,
		/// <summary>
		/// Associado ao pai.
		/// </summary>
		Parent = 0,
		/// <summary>
		/// Despejo por prioridade.
		/// </summary>
		PriorityEvictionHint = 3,
		/// <summary>
		/// Despejo por tempo de vida.
		/// </summary>
		TimestampHint = 2
	}
	/// <summary>
	/// Representa uma dica para uma política de liberação.
	/// </summary>
	[Serializable]
	public abstract class EvictionHint : ICompactSerializable
	{
		private EvictionHintType _hintType;

		/// <summary>
		/// Tipo da dica.
		/// </summary>
		public EvictionHintType HintType
		{
			get
			{
				return _hintType;
			}
			protected set
			{
				_hintType = value;
			}
		}

		/// <summary>
		/// Identifica se a dica é variante.
		/// </summary>
		public abstract bool IsVariant
		{
			get;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected EvictionHint()
		{
		}

		/// <summary>
		/// Atualiza a dica.
		/// </summary>
		/// <returns></returns>
		public abstract bool Update();

		/// <summary>
		/// Lê um dica de despejo que está serializada no leitor informado.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static EvictionHint ReadEvcHint(CompactReader reader)
		{
			EvictionHintType parent = EvictionHintType.Parent;
			parent = (EvictionHintType)reader.ReadInt16();
			switch(parent)
			{
			case EvictionHintType.NULL:
				return null;
			case EvictionHintType.Parent:
				return (EvictionHint)reader.ReadObject();
			case EvictionHintType.CounterHint:
				CounterHint hint2 = new CounterHint();
				((ICompactSerializable)hint2).Deserialize(reader);
				return hint2;
			case EvictionHintType.TimestampHint:
				TimestampHint hint4 = new TimestampHint();
				((ICompactSerializable)hint4).Deserialize(reader);
				return hint4;
			case EvictionHintType.PriorityEvictionHint:
				PriorityEvictionHint hint3 = new PriorityEvictionHint();
				((ICompactSerializable)hint3).Deserialize(reader);
				return hint3;
			}
			return null;
		}

		/// <summary>
		/// Salva uma dica de despejo no escritor informado.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="evcHint"></param>
		public static void WriteEvcHint(CompactWriter writer, EvictionHint evcHint)
		{
			if(evcHint == null)
				writer.Write((short)(-1));
			else
			{
				writer.Write((short)evcHint.HintType);
				evcHint.Serialize(writer);
			}
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer">Escritor onde os dados serão serializados.</param>
		public void Serialize(CompactWriter writer)
		{
			writer.Write((short)HintType);
		}

		/// <summary>
		/// Deserializa os dados para a instancia.
		/// </summary>
		/// <param name="reader">Leitor onde os dados estão serializados.</param>
		public void Deserialize(CompactReader reader)
		{
			HintType = (EvictionHintType)reader.ReadInt16();
		}
	}
}
