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

namespace Colosoft.ServiceModel.Channels
{
	public enum WebSocketTransportUsage
	{
		WhenDuplex,
		Always,
		Never
	}
	public sealed class WebSocketTransportSettings : IEquatable<WebSocketTransportSettings>
	{
		public WebSocketTransportSettings()
		{
			throw new NotImplementedException("WebSocketTransportSettings");
		}

		public const string BinaryEncoderTransferModeHeader = null;

		public const string BinaryMessageReceivedAction = "http://schemas.microsoft.com/2011/02/websockets/onbinarymessage";

		public const string ConnectionOpenedAction = null;

		public const string SoapContentTypeHeader = null;

		public const string TextMessageReceivedAction = "http://schemas.microsoft.com/2011/02/websockets/ontextmessage";

		public bool CreateNotificationOnConnection
		{
			get;
			set;
		}

		public bool DisablePayloadMasking
		{
			get;
			set;
		}

		public TimeSpan KeepAliveInterval
		{
			get;
			set;
		}

		public int MaxPendingConnections
		{
			get;
			set;
		}

		public int ReceiveBufferSize
		{
			get;
			set;
		}

		public int SendBufferSize
		{
			get;
			set;
		}

		public string SubProtocol
		{
			get;
			set;
		}

		public WebSocketTransportUsage TransportUsage
		{
			get;
			set;
		}

		public bool Equals(WebSocketTransportSettings other)
		{
			return other.CreateNotificationOnConnection == CreateNotificationOnConnection && other.DisablePayloadMasking == DisablePayloadMasking && other.KeepAliveInterval == KeepAliveInterval && other.MaxPendingConnections == MaxPendingConnections && other.ReceiveBufferSize == ReceiveBufferSize && other.SendBufferSize == SendBufferSize && other.SubProtocol == SubProtocol && other.TransportUsage == TransportUsage;
		}
	}
}
