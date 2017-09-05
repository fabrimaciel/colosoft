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

namespace Colosoft.Web
{
	/// <summary>
	/// Classe com métodos de extensão para HttpRequest.
	/// </summary>
	public static class HttpRequestExtentions
	{
		/// <summary>
		/// Remove a validação de requisição.
		/// </summary>
		/// <param name="request"></param>
		public static void RemoveValidateRequest(this System.Web.HttpRequest request)
		{
			if(request == null)
				return;
			var flagsField = typeof(System.Web.HttpRequest).GetField("_flags", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			if(flagsField != null)
			{
				object flags = flagsField.GetValue(request);
				var clearMethod = flags.GetType().GetMethod("Clear", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				if(clearMethod != null)
					clearMethod.Invoke(flags, new object[] {
						2
					});
				flagsField.SetValue(request, flags);
			}
		}
	}
}
