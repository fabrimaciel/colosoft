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
using System.Threading.Tasks;

namespace Colosoft.Owin.Server.Middlewares
{
	/// <summary>
	/// Implementação do middleware sem corpo.
	/// </summary>
	class NoBodyMiddleware
	{
		private readonly Func<IDictionary<string, object>, Task> _next;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="next"></param>
		public NoBodyMiddleware(Func<IDictionary<string, object>, Task> next)
		{
			_next = next;
		}

		/// <summary>
		/// Invoke.
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		public async Task Invoke(IDictionary<string, object> environment)
		{
			await _next(environment);
			var request = new Microsoft.Owin.OwinRequest(environment);
			var response = new Microsoft.Owin.OwinResponse(environment);
			if((response.StatusCode >= 100 && response.StatusCode <= 199) || response.StatusCode == 204 || response.StatusCode == 304 || request.Method.Equals("HEAD", StringComparison.OrdinalIgnoreCase))
			{
				response.Body.Close();
				response.Body = System.IO.Stream.Null;
			}
		}
	}
}
