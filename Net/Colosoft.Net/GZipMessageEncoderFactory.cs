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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Colosoft.Net
{
	/// <summary>
	/// Classe usada para criar um encoder customizados (GZipMessageEncoder).
	/// </summary>
	class GZipMessageEncoderFactory : MessageEncoderFactory
	{
		private MessageEncoder _encoder;

		private bool _isClient;

		/// <summary>
		/// Recupera o encoder.
		/// </summary>
		public override MessageEncoder Encoder
		{
			get
			{
				return _encoder;
			}
		}

		/// <summary>
		/// Versão da mensagem.
		/// </summary>
		public override MessageVersion MessageVersion
		{
			get
			{
				return _encoder.MessageVersion;
			}
		}

		/// <summary>
		/// O encoder GZip é um adaptador de outro encoder.
		/// Sendo assim é necessário informar a factory do encoder adaptador.
		/// </summary>
		/// <param name="messageEncoderFactory"></param>
		/// <param name="isClient">Identifica se está vinculado ao um cliente.</param>
		public GZipMessageEncoderFactory(MessageEncoderFactory messageEncoderFactory, bool isClient)
		{
			if(messageEncoderFactory == null)
				throw new ArgumentNullException("messageEncoderFactory", "A valid message encoder factory must be passed to the GZipEncoder");
			_encoder = new GZipMessageEncoder(messageEncoderFactory.Encoder, isClient);
			_isClient = isClient;
		}

		/// <summary>
		/// Implementação do encoder de mensagem para o GZip.
		/// </summary>
		class GZipMessageEncoder : MessageEncoder
		{
			private static string GZipContentType = "application/x-gzip";

			private bool _isClient;

			/// <summary>
			/// Esse membro armazena o encoder interno.
			/// </summary>
			private MessageEncoder _innerEncoder;

			/// <summary>
			/// Tipo de conteúdo.
			/// </summary>
			public override string ContentType
			{
				get
				{
					var contentType = _innerEncoder.ContentType;
					if(!string.IsNullOrEmpty(contentType) && (_isClient || (OperationContext.Current != null && OperationContext.Current.Extensions.OfType<DoCompressPlusExtension>().Any())))
					{
						var index = contentType.IndexOf(';');
						if(index >= 0)
							contentType = contentType.Substring(0, index) + "+gzip" + contentType.Substring(index);
					}
					return contentType;
				}
			}

			/// <summary>
			/// Tipo de mídia.
			/// </summary>
			public override string MediaType
			{
				get
				{
					var contentType = _innerEncoder.ContentType;
					if(!string.IsNullOrEmpty(contentType) && (_isClient || (OperationContext.Current != null && !OperationContext.Current.Extensions.OfType<DoCompressPlusExtension>().Any())))
					{
						var index = contentType.IndexOf(';');
						if(index >= 0)
							contentType = contentType.Substring(0, index) + "+gzip" + contentType.Substring(index);
					}
					return contentType;
				}
			}

			/// <summary>
			/// Versão da mensagem SOAP.
			/// </summary>
			public override MessageVersion MessageVersion
			{
				get
				{
					return _innerEncoder.MessageVersion;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="messageEncoder"></param>
			/// <param name="isClient"></param>
			public GZipMessageEncoder(MessageEncoder messageEncoder, bool isClient) : base()
			{
				if(messageEncoder == null)
					throw new ArgumentNullException("messageEncoder", "A valid message encoder must be passed to the GZipEncoder");
				_innerEncoder = messageEncoder;
				_isClient = isClient;
			}

			/// <summary>
			/// Método que comprime o vetor de bytes.
			/// </summary>
			/// <param name="buffer">Buffer com os dados que serão comprimidos.</param>
			/// <param name="bufferManager">Gerenciador do buffer.</param>
			/// <param name="messageOffset">Offset da mensagem.</param>
			/// <returns></returns>
			private static ArraySegment<byte> CompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager, int messageOffset)
			{
				MemoryStream memoryStream = new MemoryStream();
				using (GZipStream gzStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
					gzStream.Write(buffer.Array, buffer.Offset, buffer.Count);
				byte[] compressedBytes = memoryStream.ToArray();
				int totalLength = messageOffset + compressedBytes.Length;
				byte[] bufferedBytes = bufferManager.TakeBuffer(totalLength);
				Array.Copy(compressedBytes, 0, bufferedBytes, messageOffset, compressedBytes.Length);
				bufferManager.ReturnBuffer(buffer.Array);
				ArraySegment<byte> byteArray = new ArraySegment<byte>(bufferedBytes, messageOffset, bufferedBytes.Length - messageOffset);
				return byteArray;
			}

			/// <summary>
			/// Método que auxilia a descompactar o vetor de bytes.
			/// </summary>
			/// <param name="buffer"></param>
			/// <param name="bufferManager"></param>
			/// <returns></returns>
			private static ArraySegment<byte> DecompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager)
			{
				MemoryStream memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count);
				MemoryStream decompressedStream = new MemoryStream();
				int totalRead = 0;
				int blockSize = 1024;
				byte[] tempBuffer = bufferManager.TakeBuffer(blockSize);
				using (GZipStream gzStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					while (true)
					{
						int bytesRead = 0;
						try
						{
							bytesRead = gzStream.Read(tempBuffer, 0, blockSize);
						}
						catch(InvalidDataException)
						{
							if(totalRead == 0)
							{
								decompressedStream.Dispose();
								memoryStream.Position = 0;
								decompressedStream = memoryStream;
								totalRead = (int)memoryStream.Length;
								break;
							}
							else
								throw;
						}
						if(bytesRead == 0)
							break;
						decompressedStream.Write(tempBuffer, 0, bytesRead);
						totalRead += bytesRead;
					}
				}
				bufferManager.ReturnBuffer(tempBuffer);
				byte[] decompressedBytes = decompressedStream.ToArray();
				byte[] bufferManagerBuffer = bufferManager.TakeBuffer(decompressedBytes.Length + buffer.Offset);
				Array.Copy(buffer.Array, 0, bufferManagerBuffer, 0, buffer.Offset);
				Array.Copy(decompressedBytes, 0, bufferManagerBuffer, buffer.Offset, decompressedBytes.Length);
				ArraySegment<byte> byteArray = new ArraySegment<byte>(bufferManagerBuffer, buffer.Offset, decompressedBytes.Length);
				bufferManager.ReturnBuffer(buffer.Array);
				return byteArray;
			}

			/// <summary>
			/// Verifica se o tipo de conteúdo é suportado.
			/// </summary>
			/// <param name="contentType"></param>
			/// <returns></returns>
			public override bool IsContentTypeSupported(string contentType)
			{
				if(contentType == GZipContentType || contentType == _innerEncoder.ContentType || (!string.IsNullOrEmpty(contentType) && contentType.Contains("+gzip")))
					return true;
				return base.IsContentTypeSupported(contentType);
			}

			/// <summary>
			/// Um dos dois pontos de entrada no encoder. 
			/// Chamado pelo WCF para decodificar um vetor de bytes em uma mensagem.
			/// </summary>
			/// <param name="buffer"></param>
			/// <param name="bufferManager"></param>
			/// <param name="contentType"></param>
			/// <returns></returns>
			public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
			{
				if(!string.IsNullOrEmpty(contentType) && contentType.Contains("+gzip"))
				{
					////Decompress the buffer
					ArraySegment<byte> decompressedBuffer = DecompressBuffer(buffer, bufferManager);
					////Use the inner encoder to decode the decompressed buffer
					Message returnMessage = _innerEncoder.ReadMessage(decompressedBuffer, bufferManager);
					returnMessage.Properties.Encoder = this;
					return returnMessage;
				}
				return _innerEncoder.ReadMessage(buffer, bufferManager, contentType);
			}

			/// <summary>
			/// Um dos dois pontos de entrada do encoder.
			/// Chamado pelo WCF para codificar a mensagem em um vetor de bytes.
			/// </summary>
			/// <param name="message"></param>
			/// <param name="maxMessageSize"></param>
			/// <param name="bufferManager"></param>
			/// <param name="messageOffset"></param>
			/// <returns></returns>
			public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
			{
				////Use the inner encoder to encode a Message into a buffered byte array
				////Compress the resulting byte array
				if(_isClient || (OperationContext.Current != null && (OperationContext.Current.Extensions.OfType<DoCompressExtension>().Any() || OperationContext.Current.Extensions.OfType<DoCompressPlusExtension>().Any())))
				{
					var buffer = _innerEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
					return CompressBuffer(buffer, bufferManager, messageOffset);
				}
				else
					return _innerEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
			}

			/// <summary>
			/// Um dos dois pontos de entrada no encoder. 
			/// Chamado pelo WCF para decodificar um vetor de bytes em uma mensagem.
			/// </summary>
			/// <param name="stream"></param>
			/// <param name="maxSizeOfHeaders"></param>
			/// <param name="contentType"></param>
			/// <returns></returns>
			public override Message ReadMessage(System.IO.Stream stream, int maxSizeOfHeaders, string contentType)
			{
				if(!string.IsNullOrEmpty(contentType) && contentType.Contains("+gzip"))
				{
					////Pass false for the "leaveOpen" parameter to the GZipStream constructor.
					////This will ensure that the inner stream gets closed when the message gets closed, which
					////will ensure that resources are available for reuse/release.
					var gzStream = new GZipStream(stream, CompressionMode.Decompress, false);
					return _innerEncoder.ReadMessage(gzStream, maxSizeOfHeaders);
				}
				return _innerEncoder.ReadMessage(stream, maxSizeOfHeaders, contentType);
			}

			/// <summary>
			/// Um dos dois pontos de entrada do encoder.
			/// Chamado pelo WCF para codificar a mensagem em um vetor de bytes.
			/// </summary>
			/// <param name="message"></param>
			/// <param name="stream"></param>
			public override void WriteMessage(Message message, System.IO.Stream stream)
			{
				//// innerEncoder.WriteMessage(message, gzStream) depends on that it can flush data by flushing 
				//// the stream passed in, but the implementation of GZipStream.Flush will not flush underlying
				//// stream, so we need to flush here.
				if(_isClient || (OperationContext.Current != null && (OperationContext.Current.Extensions.OfType<DoCompressExtension>().Any() || OperationContext.Current.Extensions.OfType<DoCompressPlusExtension>().Any())))
				{
					using (var gzStream = new GZipStream(stream, CompressionMode.Compress, true))
						_innerEncoder.WriteMessage(message, gzStream);
					stream.Flush();
				}
				else
					_innerEncoder.WriteMessage(message, stream);
			}
		}
	}
}
