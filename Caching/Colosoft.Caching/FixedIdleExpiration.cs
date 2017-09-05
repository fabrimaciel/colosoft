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

namespace Colosoft.Caching.Expiration
{
	/// <summary>
	/// Representa uma expiração fixada e por inatividade.
	/// </summary>
	[Serializable]
	public class FixedIdleExpiration : AggregateExpirationHint
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public FixedIdleExpiration()
		{
			base.HintType = ExpirationHintType.FixedIdleExpiration;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="idleTime"></param>
		/// <param name="absoluteTime"></param>
		public FixedIdleExpiration(TimeSpan idleTime, DateTime absoluteTime) : base(new ExpirationHint[] {
			new FixedExpiration(absoluteTime),
			new IdleExpiration(idleTime)
		})
		{
			base.HintType = ExpirationHintType.FixedIdleExpiration;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Empty;
		}
	}
}
