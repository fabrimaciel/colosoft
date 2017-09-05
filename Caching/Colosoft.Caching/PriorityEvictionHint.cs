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
	/// Implementação para o <see cref="EvictionHint"/> sobre prioridade.
	/// </summary>
	[Serializable]
	public class PriorityEvictionHint : EvictionHint, ICompactSerializable
	{
		private CacheItemPriority _priority;

		/// <summary>
		/// Identifica se a instancia é variante.
		/// </summary>
		public override bool IsVariant
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Prioridade associada.
		/// </summary>
		public CacheItemPriority Priority
		{
			get
			{
				return _priority;
			}
			set
			{
				_priority = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public PriorityEvictionHint()
		{
			base.HintType = EvictionHintType.PriorityEvictionHint;
			_priority = CacheItemPriority.Normal;
		}

		/// <summary>
		/// Cria uma nova instancia já definindo a prioridade.
		/// </summary>
		/// <param name="priority">Prioridade inicial para a instancia.</param>
		public PriorityEvictionHint(CacheItemPriority priority)
		{
			base.HintType = EvictionHintType.PriorityEvictionHint;
			_priority = priority;
		}

		/// <summary>
		/// Atualiza a instancia.
		/// </summary>
		/// <returns></returns>
		public override bool Update()
		{
			return false;
		}

		void ICompactSerializable.Deserialize(CompactReader reader)
		{
			base.Deserialize(reader);
			_priority = (CacheItemPriority)Enum.ToObject(typeof(CacheItemPriority), reader.ReadInt16());
		}

		void ICompactSerializable.Serialize(CompactWriter writer)
		{
			base.Serialize(writer);
			writer.Write(Convert.ToInt16(_priority));
		}
	}
}
