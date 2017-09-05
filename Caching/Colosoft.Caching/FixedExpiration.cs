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

namespace Colosoft.Caching.Expiration
{
	/// <summary>
	/// Representa uma data de expiração fixada.
	/// </summary>
	[Serializable]
	public class FixedExpiration : ExpirationHint, ICompactSerializable
	{
		private int _absoluteTime;

		private int _milliseconds;

		/// <summary>
		/// Data absoluta associada.
		/// </summary>
		public DateTime AbsoluteTime
		{
			get
			{
				DateTime dateTime = CachingUtils.GetDateTime(_absoluteTime);
				return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, _milliseconds, DateTimeKind.Unspecified);
			}
		}

		/// <summary>
		/// Chave de ordenação.
		/// </summary>
		internal override int SortKey
		{
			get
			{
				return _absoluteTime;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public FixedExpiration()
		{
			base.HintType = ExpirationHintType.FixedExpiration;
		}

		/// <summary>
		/// Cria uma instancia com a data absoluta informada.
		/// </summary>
		/// <param name="absoluteTime"></param>
		public FixedExpiration(DateTime absoluteTime)
		{
			base.HintType = ExpirationHintType.FixedExpiration;
			_absoluteTime = CachingUtils.DiffSeconds(absoluteTime);
			_milliseconds = CachingUtils.DiffMilliseconds(absoluteTime);
		}

		/// <summary>
		/// Determina a data de expiração.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal override bool DetermineExpiration(CacheRuntimeContext context)
		{
			if(base.HasExpired)
				return true;
			if(_absoluteTime < CachingUtils.DiffSeconds(DateTime.Now))
				base.NotifyExpiration(this, null);
			return base.HasExpired;
		}

		/// <summary>
		/// Texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Empty;
		}

		/// <summary>
		/// Deserializa os dados para a instancia.
		/// </summary>
		/// <param name="reader"></param>
		public override void Deserialize(CompactReader reader)
		{
			base.Deserialize(reader);
			_absoluteTime = reader.ReadInt32();
			_milliseconds = reader.ReadInt32();
		}

		/// <summary>
		/// Serialiaz os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public override void Serialize(CompactWriter writer)
		{
			base.Serialize(writer);
			writer.Write(_absoluteTime);
			writer.Write(_milliseconds);
		}
	}
}
