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
	/// Classe que auxilia na expiração por dependencia.
	/// </summary>
	public class DependencyHelper
	{
		/// <summary>
		/// Adiciona uma nova dependencia para a dependencia informada.
		/// </summary>
		/// <param name="cacheDependency"></param>
		/// <param name="newDependency"></param>
		private static void AddToDependency(ref Dependencies.CacheDependency cacheDependency, Dependencies.CacheDependency newDependency)
		{
			if(newDependency != null)
			{
				if(cacheDependency == null)
					cacheDependency = new Dependencies.CacheDependency();
				cacheDependency.Dependencies.Add(newDependency);
			}
		}

		/// <summary>
		/// Recupera uma dependencia.
		/// </summary>
		/// <param name="hint"></param>
		/// <param name="absoluteExpiration"></param>
		/// <param name="slidingExpiration"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Dependencies.CacheDependency GetCacheDependency(ExpirationHint hint, out DateTime absoluteExpiration, out TimeSpan slidingExpiration)
		{
			Dependencies.CacheDependency cacheDependency = null;
			absoluteExpiration = DateTime.MaxValue;
			slidingExpiration = TimeSpan.Zero;
			if(hint != null)
			{
				if(hint is AggregateExpirationHint)
				{
					AggregateExpirationHint hint2 = (AggregateExpirationHint)hint;
					if((hint2.Hints != null) && (hint2.Hints.Length > 0))
						foreach (ExpirationHint hint3 in hint2.Hints)
							AddToDependency(ref cacheDependency, GetCacheDependency(hint3, out absoluteExpiration, out slidingExpiration));
					return cacheDependency;
				}
				if(hint is FixedExpiration)
				{
					absoluteExpiration = ((FixedExpiration)hint).AbsoluteTime;
					return cacheDependency;
				}
				if(hint is IdleExpiration)
				{
					slidingExpiration = ((IdleExpiration)hint).SlidingTime;
					return cacheDependency;
				}
				if(hint is KeyDependency)
				{
					var dependency2 = (KeyDependency)hint;
					AddToDependency(ref cacheDependency, new Dependencies.KeyDependency(dependency2.CacheKeys, new DateTime(dependency2.StartAfterTicks)));
					return cacheDependency;
				}
				if(hint is FileDependency)
				{
					FileDependency dependency3 = (FileDependency)hint;
					AddToDependency(ref cacheDependency, new Dependencies.FileDependency(dependency3.FileNames, new DateTime(dependency3.StartAfterTicks)));
					return cacheDependency;
				}
				return cacheDependency;
			}
			return cacheDependency;
		}

		/// <summary>
		/// Recupera o hint da dependencia informada.
		/// </summary>
		/// <param name="dependency"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static ExpirationHint GetExpirationHint(Dependencies.CacheDependency dependency)
		{
			var aggregateHint = new AggregateExpirationHint();
			return GetExpirationHint(dependency, aggregateHint);
		}

		/// <summary>
		/// Recupera o hint associado com a dependencia.
		/// </summary>
		/// <param name="cacheDependency"></param>
		/// <param name="aggregateHint"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private static ExpirationHint GetExpirationHint(Dependencies.CacheDependency cacheDependency, AggregateExpirationHint aggregateHint)
		{
			if(cacheDependency == null)
			{
				return null;
			}
			ExpirationHint expirationHint = null;
			if(cacheDependency is Dependencies.KeyDependency)
			{
				var dependency = cacheDependency as Dependencies.KeyDependency;
				expirationHint = new KeyDependency(dependency.CacheKeys, new DateTime(dependency.StartAfterTicks));
				aggregateHint.Add(expirationHint);
			}
			else if(cacheDependency is Dependencies.FileDependency)
			{
				var dependency2 = cacheDependency as Dependencies.FileDependency;
				expirationHint = new FileDependency(dependency2.fileNames, new DateTime(dependency2.StartAfterTicks));
				aggregateHint.Add(expirationHint);
			}
			if(aggregateHint.Hints.Length == 0)
				return null;
			if(aggregateHint.Hints.Length == 1)
				return aggregateHint.Hints[0];
			return aggregateHint;
		}

		/// <summary>
		/// Recupera o <see cref="ExpirationHint"/> baseado nas datas informadas.
		/// </summary>
		/// <param name="absoluteExpiration"></param>
		/// <param name="slidingExpiration"></param>
		/// <returns></returns>
		private static ExpirationHint GetExpirationHint(DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
			if(DateTime.MaxValue.Equals(absoluteExpiration) && TimeSpan.Zero.Equals(slidingExpiration))
				return null;
			if(DateTime.MaxValue.Equals(absoluteExpiration))
				return new IdleExpiration(slidingExpiration);
			absoluteExpiration = absoluteExpiration.ToUniversalTime();
			return new FixedExpiration(absoluteExpiration);
		}

		/// <summary>
		/// Recupera o <see cref="ExpirationHint"/> baseado na dependencia e nas datas informadas.
		/// </summary>
		/// <param name="dependency"></param>
		/// <param name="absoluteExpiration"></param>
		/// <param name="slidingExpiration"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static ExpirationHint GetExpirationHint(Dependencies.CacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
			ExpirationHint expirationHint = GetExpirationHint(absoluteExpiration, slidingExpiration);
			if(expirationHint == null)
				return GetExpirationHint(dependency);
			ExpirationHint eh = GetExpirationHint(dependency);
			if(eh == null)
				return expirationHint;
			AggregateExpirationHint hint3 = null;
			if(eh is AggregateExpirationHint)
			{
				hint3 = eh as AggregateExpirationHint;
				hint3.Add(expirationHint);
				return hint3;
			}
			hint3 = new AggregateExpirationHint();
			hint3.Add(expirationHint);
			hint3.Add(eh);
			return hint3;
		}
	}
}
