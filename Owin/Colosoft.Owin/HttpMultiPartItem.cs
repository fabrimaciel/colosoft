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

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Representa um item do MultiPart.
	/// </summary>
	class HttpMultiPartItem
	{
		/// <summary>
		/// Cabeçalhos.
		/// </summary>
		public Dictionary<string, string> Headers
		{
			get;
			private set;
		}

		/// <summary>
		/// Valor.
		/// </summary>
		public string Value
		{
			get;
			private set;
		}

		/// <summary>
		/// Stream associado.
		/// </summary>
		public System.IO.Stream Stream
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtro padrão.
		/// </summary>
		/// <param name="headers"></param>
		/// <param name="value"></param>
		/// <param name="stream"></param>
		public HttpMultiPartItem(Dictionary<string, string> headers, string value, System.IO.Stream stream)
		{
			if(headers == null)
				throw new ArgumentNullException("headers");
			Headers = headers;
			Value = value;
			Stream = stream;
		}
	}
}
