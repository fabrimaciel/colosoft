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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Colosoft.ServiceModel.Channels
{
	class TextMessageEncoder : MessageEncoder
	{
		Encoding encoding;

		MessageVersion version;

		public TextMessageEncoder(MessageVersion version, Encoding encoding)
		{
			this.version = version;
			this.encoding = encoding;
		}

		internal Encoding Encoding
		{
			get
			{
				return encoding;
			}
		}

		public override string ContentType
		{
			get
			{
				return String.Concat(MediaType, "; charset=", encoding.WebName);
			}
		}

		public override string MediaType
		{
			get
			{
				return version.Envelope == EnvelopeVersion.Soap12 ? "application/soap+xml" : "text/xml";
			}
		}

		public override MessageVersion MessageVersion
		{
			get
			{
				return version;
			}
		}

		public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
		{
			if(bufferManager == null)
				throw new ArgumentNullException("bufferManager");
			var settings = new System.Xml.XmlReaderSettings();
			settings.CheckCharacters = false;
			var ret = Message.CreateMessage(System.Xml.XmlDictionaryReader.CreateDictionaryReader(System.Xml.XmlReader.Create(new System.IO.StreamReader(new System.IO.MemoryStream(buffer.Array, buffer.Offset, buffer.Count), encoding), settings)), int.MaxValue, version);
			FillActionContentType(ret, contentType);
			return ret;
		}

		public override Message ReadMessage(System.IO.Stream stream, int maxSizeOfHeaders, string contentType)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			var settings = new System.Xml.XmlReaderSettings();
			settings.CheckCharacters = false;
			var ret = Message.CreateMessage(System.Xml.XmlDictionaryReader.CreateDictionaryReader(System.Xml.XmlReader.Create(new System.IO.StreamReader(stream, encoding), settings)), maxSizeOfHeaders, version);
			ret.Properties.Encoder = this;
			FillActionContentType(ret, contentType);
			return ret;
		}

		void FillActionContentType(Message msg, string contentType)
		{
			if(contentType.StartsWith("application/soap+xml", StringComparison.Ordinal))
			{
				var ct = new System.Net.Mime.ContentType(contentType);
				if(ct.Parameters.ContainsKey("action"))
					msg.Headers.Action = ct.Parameters["action"];
			}
		}

		public override void WriteMessage(Message message, System.IO.Stream stream)
		{
			if(message == null)
				throw new ArgumentNullException("message");
			if(stream == null)
				throw new ArgumentNullException("stream");
			VerifyMessageVersion(message);
			var s = new System.Xml.XmlWriterSettings();
			s.Encoding = encoding;
			s.CheckCharacters = false;
			using (var w = System.Xml.XmlWriter.Create(stream, s))
			{
				message.WriteMessage(System.Xml.XmlDictionaryWriter.CreateDictionaryWriter(w));
			}
		}

		public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
		{
			VerifyMessageVersion(message);
			ArraySegment<byte> seg = new ArraySegment<byte>(bufferManager.TakeBuffer(maxMessageSize), messageOffset, maxMessageSize);
			var s = new System.Xml.XmlWriterSettings();
			s.Encoding = encoding;
			s.CheckCharacters = false;
			using (var w = System.Xml.XmlWriter.Create(new System.IO.MemoryStream(seg.Array, seg.Offset, seg.Count), s))
			{
				message.WriteMessage(System.Xml.XmlDictionaryWriter.CreateDictionaryWriter(w));
			}
			return seg;
		}

		internal void VerifyMessageVersion(Message message)
		{
			if(!message.Version.Equals(MessageVersion))
				throw new ProtocolException(String.Format("Message version mismatch. Expected {0} but was {1}.", MessageVersion, message.Version));
		}
	}
}
