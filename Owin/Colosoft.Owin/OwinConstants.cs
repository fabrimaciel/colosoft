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

namespace Colosoft.Owin.Types
{
	/// <summary>
	/// OwinConstants
	/// </summary>
	static class OwinConstants
	{
		public const string RequestScheme = "owin.RequestScheme";

		public const string RequestMethod = "owin.RequestMethod";

		public const string RequestPathBase = "owin.RequestPathBase";

		public const string RequestPath = "owin.RequestPath";

		public const string RequestQueryString = "owin.RequestQueryString";

		public const string RequestProtocol = "owin.RequestProtocol";

		public const string RequestHeaders = "owin.RequestHeaders";

		public const string RequestBody = "owin.RequestBody";

		public const string ResponseStatusCode = "owin.ResponseStatusCode";

		public const string ResponseReasonPhrase = "owin.ResponseReasonPhrase";

		public const string ResponseProtocol = "owin.ResponseProtocol";

		public const string ResponseHeaders = "owin.ResponseHeaders";

		public const string ResponseBody = "owin.ResponseBody";

		public const string CallCancelled = "owin.CallCancelled";

		public const string OwinVersion = "owin.Version";

		internal static class CommonKeys
		{
			public const string ClientCertificate = "ssl.ClientCertificate";

			public const string RemoteIpAddress = "server.RemoteIpAddress";

			public const string RemotePort = "server.RemotePort";

			public const string LocalIpAddress = "server.LocalIpAddress";

			public const string LocalPort = "server.LocalPort";

			public const string IsLocal = "server.IsLocal";

			public const string TraceOutput = "host.TraceOutput";

			public const string Addresses = "host.Addresses";

			public const string Capabilities = "server.Capabilities";

			public const string OnSendingHeaders = "server.OnSendingHeaders";
		}

		internal static class SendFiles
		{
			public const string Version = "sendfile.Version";

			public const string Support = "sendfile.Support";

			public const string Concurrency = "sendfile.Concurrency";

			public const string SendAsync = "sendfile.SendAsync";
		}

		internal static class Opaque
		{
			public const string Version = "opaque.Version";

			public const string Upgrade = "opaque.Upgrade";

			public const string Stream = "opaque.Stream";

			public const string CallCancelled = "opaque.CallCancelled";
		}

		internal static class WebSocket
		{
			public const string Version = "websocket.Version";

			public const string Accept = "websocket.Accept";

			public const string SubProtocol = "websocket.SubProtocol";

			public const string SendAsync = "websocket.SendAsync";

			public const string ReceiveAsync = "websocket.ReceiveAsync";

			public const string CloseAsync = "websocket.CloseAsync";

			public const string CallCancelled = "websocket.CallCancelled";

			public const string ClientCloseStatus = "websocket.ClientCloseStatus";

			public const string ClientCloseDescription = "websocket.ClientCloseDescription";
		}
	}
}
