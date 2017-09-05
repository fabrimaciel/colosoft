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
	/// Representa uma expiração por inatividade.
	/// </summary>
	[Serializable]
	public class IdleExpiration : ExpirationHint, ICompactSerializable
	{
		private int _idleTimeToLive;

		private int _lastTimeStamp;

		/// <summary>
		/// Data do ultimo acesso.
		/// </summary>
		public int LastAccessTime
		{
			get
			{
				return _lastTimeStamp;
			}
		}

		/// <summary>
		/// Data de corte.
		/// </summary>
		public TimeSpan SlidingTime
		{
			get
			{
				return new TimeSpan(0, 0, _idleTimeToLive);
			}
		}

		/// <summary>
		/// Chave de ordenação.
		/// </summary>
		internal override int SortKey
		{
			get
			{
				return (_lastTimeStamp + _idleTimeToLive);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public IdleExpiration()
		{
			base.HintType = ExpirationHintType.IdleExpiration;
		}

		/// <summary>
		/// Cria uma instancia com o tempo de vida inicial.
		/// </summary>
		/// <param name="idleTTL"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public IdleExpiration(TimeSpan idleTTL)
		{
			this.SetBit(4);
			_idleTimeToLive = (int)idleTTL.TotalSeconds;
			_lastTimeStamp = CachingUtils.DiffSeconds(DateTime.Now);
			base.HintType = ExpirationHintType.IdleExpiration;
		}

		/// <summary>
		/// Determina a expiração.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal override bool DetermineExpiration(CacheRuntimeContext context)
		{
			if(base.HasExpired)
				return true;
			if(this.SortKey.CompareTo(CachingUtils.DiffSeconds(DateTime.Now)) < 0)
				base.NotifyExpiration(this, null);
			return base.HasExpired;
		}

		/// <summary>
		/// Reseta a instancia.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal override bool Reset(CacheRuntimeContext context)
		{
			_lastTimeStamp = CachingUtils.DiffSeconds(DateTime.Now);
			return base.Reset(context);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Empty;
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public override void Serialize(CompactWriter writer)
		{
			base.Serialize(writer);
			writer.Write(_idleTimeToLive);
			writer.Write(_lastTimeStamp);
		}

		/// <summary>
		/// Deserializa os dados da instancia.
		/// </summary>
		/// <param name="reader"></param>
		public override void Deserialize(CompactReader reader)
		{
			base.Deserialize(reader);
			_idleTimeToLive = reader.ReadInt32();
			_lastTimeStamp = reader.ReadInt32();
		}
	}
}
