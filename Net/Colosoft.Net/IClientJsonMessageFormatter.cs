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

using System.ServiceModel.Dispatcher;

namespace Colosoft.Net.Json.Formatters
{
	/// <summary>
	/// 
	/// </summary>
	public interface IClientJsonMessageFormatter : IClientMessageFormatter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		byte[] EncodeParameters(object[] parameters);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="body"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		object DecodeReply(byte[] body, object[] parameters);
	}
}
