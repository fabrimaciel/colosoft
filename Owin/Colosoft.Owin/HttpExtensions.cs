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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Owin.Server
{
	static class HttpExtensions
	{
		public static NameValueCollection GetNameValueCollection(this Microsoft.Owin.IFormCollection collection)
		{
			var result = new NameValueCollection();
			foreach (var i in collection)
				result.Add(i.Key, i.Value.FirstOrDefault());
			return result;
		}

		public static NameValueCollection GetNameValueCollection(this Microsoft.Owin.IReadableStringCollection collection)
		{
			var result = new NameValueCollection();
			foreach (var i in collection)
				result.Add(i.Key, i.Value.FirstOrDefault());
			return result;
		}

		public static System.Web.HttpCookieCollection GetCookieCollection(this Microsoft.Owin.RequestCookieCollection collection)
		{
			var result = new System.Web.HttpCookieCollection();
			foreach (var i in collection)
				result.Add(new System.Web.HttpCookie(i.Key, i.Value));
			return result;
		}

		public static NameValueCollection GetNameValueCollection(this Microsoft.Owin.IHeaderDictionary collection)
		{
			var result = new NameValueCollection();
			foreach (var i in collection)
				result.Add(i.Key, i.Value.FirstOrDefault());
			return result;
		}
	}
}
