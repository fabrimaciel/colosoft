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
	/// Implementação da dica por tempo.
	/// </summary>
	[Serializable]
	internal class TimestampHint : EvictionHint, ICompactSerializable
	{
		protected DateTime _dt;

		/// <summary>
		/// Identifica se a instancia é variante.
		/// </summary>
		public override bool IsVariant
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// TimeStamp associado.
		/// </summary>
		public DateTime TimeStamp
		{
			get
			{
				return _dt;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public TimestampHint()
		{
			base.HintType = EvictionHintType.TimestampHint;
			_dt = DateTime.UtcNow;
		}

		/// <summary>
		/// Deserializa os dados da instancia.
		/// </summary>
		/// <param name="reader"></param>
		void ICompactSerializable.Deserialize(CompactReader reader)
		{
			base.Deserialize(reader);
			_dt = reader.ReadDateTime();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		void ICompactSerializable.Serialize(CompactWriter writer)
		{
			base.Serialize(writer);
			writer.Write(_dt);
		}

		/// <summary>
		/// Atualiza da instancia da dica.
		/// </summary>
		/// <returns></returns>
		public override bool Update()
		{
			_dt = DateTime.UtcNow;
			return true;
		}
	}
}
