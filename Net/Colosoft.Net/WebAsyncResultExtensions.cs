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

namespace Colosoft.Net.Mono
{
	static class WebAsyncResultExtensions
	{
		private static System.Reflection.MethodInfo _setCompleted;

		private static System.Reflection.MethodInfo _doCallback;

		public static void SetCompleted(this IAsyncResult result, bool synch, System.IO.Stream writeStream)
		{
			var method = (_setCompleted ?? (_setCompleted = typeof(System.Net.WebClient).Assembly.GetType("WebAsyncResult").GetMethod("SetCompleted", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, new[] {
				typeof(bool),
				typeof(System.IO.Stream)
			}, null)));
			if(method == null)
				throw new NotImplementedException("Method SetCompleted(bool, Stream) not found");
			method.Invoke(result, new object[] {
				synch,
				writeStream
			});
		}

		public static void DoCallback(this IAsyncResult result)
		{
			var method = (_doCallback ?? (_doCallback = typeof(System.Net.WebClient).Assembly.GetType("WebAsyncResult").GetMethod("DoCallback", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)));
			if(method == null)
				throw new NotImplementedException("Method DoCallback() not found");
			method.Invoke(result, null);
		}
	}
}
