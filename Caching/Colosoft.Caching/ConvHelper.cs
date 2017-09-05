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
using Colosoft.Caching.Expiration;

namespace Colosoft.Caching.Util
{
	internal sealed class ConvHelper
	{
		public static readonly DateTime NoAbsoluteExpiration = DateTime.MaxValue;

		public static readonly TimeSpan NoSlidingExpiration = TimeSpan.Zero;

		public static ExpirationHint MakeExpirationHint(long ticks, bool isAbsolute)
		{
			if(ticks == 0)
			{
				return null;
			}
			if(!isAbsolute)
			{
				TimeSpan idleTTL = new TimeSpan(ticks);
				if(idleTTL.CompareTo(TimeSpan.Zero) < 0)
				{
					throw new ArgumentOutOfRangeException("slidingExpiration");
				}
				if(idleTTL.CompareTo((TimeSpan)(DateTime.Now.AddYears(1) - DateTime.Now)) >= 0)
				{
					throw new ArgumentOutOfRangeException("slidingExpiration");
				}
				return new IdleExpiration(idleTTL);
			}
			return new FixedExpiration(new DateTime(ticks));
		}

		public static ExpirationHint MakeFixedIdleExpirationHint(DateTime dt, TimeSpan ts)
		{
			return null;
		}
	}
}
