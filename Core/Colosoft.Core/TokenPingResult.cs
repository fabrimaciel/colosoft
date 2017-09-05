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
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Colosoft.Security
{
	/// <summary>
	/// Resultados possíveis de um ping.
	/// </summary>
	public enum TokenPingResultStatus
	{
		/// <summary>
		/// Sucesso.
		/// </summary>
		Success,
		/// <summary>
		/// Ocorreu algum erro durante o processo.
		/// </summary>
		Error,
		/// <summary>
		/// O token é inválido.
		/// </summary>
		InvalidToken
	}
	/// <summary>
	/// Armazen as informações do erro.
	/// </summary>
	[Serializable]
	[XmlSchemaProvider("GetMySchema")]
	public sealed class TokenPingResultErrorInfo : ISerializable, IXmlSerializable
	{
		/// <summary>
		/// Nome do tipo do erro.
		/// </summary>
		public string Type
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem do erro.
		/// </summary>
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Pilha de rastreamento do erro.
		/// </summary>
		public string StackTrace
		{
			get;
			set;
		}

		/// <summary>
		/// Instancia do erro interno.
		/// </summary>
		public TokenPingResultErrorInfo Inner
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public TokenPingResultErrorInfo()
		{
		}

		/// <summary>
		/// Constrói a instancia com o erro ocorrido.
		/// </summary>
		/// <param name="exception"></param>
		public TokenPingResultErrorInfo(Exception exception)
		{
			exception.Require("exception").NotNull();
			Type = exception.GetType().FullName;
			Message = exception.Message;
			StackTrace = exception.StackTrace;
			if(exception.InnerException != null)
				Inner = new TokenPingResultErrorInfo(exception.InnerException);
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private TokenPingResultErrorInfo(SerializationInfo info, StreamingContext context)
		{
			Type = info.GetString("Type");
			Message = info.GetString("Message");
			StackTrace = info.GetString("StackTrace");
			Inner = (TokenPingResultErrorInfo)info.GetValue("Inner", typeof(TokenPingResultErrorInfo));
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Type", Type);
			info.AddValue("Message", Message);
			info.AddValue("StackTrace", StackTrace);
			info.AddValue("Inner", Inner, typeof(TokenPingResultErrorInfo));
		}

		/// <summary>
		/// Recupera o esquema que representa o tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveSecuritySchema();
			return new System.Xml.XmlQualifiedName("TokenPingResultErrorInfo", SecurityNamespaces.Security);
		}

		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê os dados serializados.
		/// </summary>
		/// <param name="reader"></param>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			Type = reader.GetAttribute("Type");
			reader.MoveToElement();
			reader.ReadStartElement();
			Message = reader.ReadElementString("Message", SecurityNamespaces.Security);
			StackTrace = reader.ReadElementString("StackTrace", SecurityNamespaces.Security);
			if(!reader.IsEmptyElement && reader.LocalName == "Inner")
			{
				var inner = new TokenPingResultErrorInfo();
				((System.Xml.Serialization.IXmlSerializable)inner).ReadXml(reader);
				Inner = inner;
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		/// <summary>
		/// Serializa os dados para o Xml.
		/// </summary>
		/// <param name="writer"></param>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Type", Type);
			writer.WriteElementString("Message", SecurityNamespaces.Security, Message);
			writer.WriteElementString("StackTrace", SecurityNamespaces.Security, StackTrace);
			writer.WriteStartElement("Inner", SecurityNamespaces.Security);
			if(Inner != null)
				((IXmlSerializable)Inner).WriteXml(writer);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine("[Type: " + Type + "]").AppendLine("[Message: " + Message + "]").AppendLine("[StackTrace: ").AppendLine(StackTrace).AppendLine("]");
			if(Inner != null)
				sb.AppendLine("[Inner : ").AppendLine(Inner.ToString()).AppendLine("]");
			return sb.ToString();
		}
	}
	/// <summary>
	/// Representa uma mensagem do resultado do ping.
	/// </summary>
	[Serializable]
	[XmlSchemaProvider("GetMySchema")]
	public sealed class TokenPingResultMessage : ISerializable, IXmlSerializable
	{
		private IMessageFormattable _message;

		private TokenPingResultErrorInfo _error;

		/// <summary>
		/// Mensagem.
		/// </summary>
		public IMessageFormattable Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
			}
		}

		/// <summary>
		/// Erro caso tenha ocorrido.
		/// </summary>
		public TokenPingResultErrorInfo Error
		{
			get
			{
				return _error;
			}
			set
			{
				_error = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public TokenPingResultMessage()
		{
		}

		/// <summary>
		/// Cria a instancia com a mensagem informada.
		/// </summary>
		/// <param name="message"></param>
		public TokenPingResultMessage(IMessageFormattable message)
		{
			_message = message;
		}

		/// <summary>
		/// Cria a instancia com o erro ocorrido.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		public TokenPingResultMessage(IMessageFormattable message, Exception exception)
		{
			_message = message;
			if(exception != null)
				_error = new TokenPingResultErrorInfo(exception);
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private TokenPingResultMessage(SerializationInfo info, StreamingContext context)
		{
			Message = info.GetString("Message").GetFormatter();
			Error = (TokenPingResultErrorInfo)info.GetValue("Error", typeof(TokenPingResultErrorInfo));
		}

		/// <summary>
		/// Conversor implicito para recupera a mensagem de um erro ocorrido.
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public static implicit operator TokenPingResultMessage(Exception exception)
		{
			if(exception == null)
				return null;
			return new TokenPingResultMessage(exception.Message.GetFormatter(), exception);
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Message", (Message ?? MessageFormattable.Empty).Format(), typeof(string));
			info.AddValue("Error", Error, typeof(TokenPingResultErrorInfo));
		}

		/// <summary>
		/// Recupera o esquema que representa o tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveSecuritySchema();
			return new System.Xml.XmlQualifiedName("TokenPingResultMessage", SecurityNamespaces.Security);
		}

		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê os dados serializados.
		/// </summary>
		/// <param name="reader"></param>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			Message = reader.ReadElementString("Message", SecurityNamespaces.Security).GetFormatter();
			if(!reader.IsEmptyElement && reader.LocalName == "Error")
			{
				var inner = new TokenPingResultErrorInfo();
				((System.Xml.Serialization.IXmlSerializable)inner).ReadXml(reader);
				Error = inner;
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		/// <summary>
		/// Serializa os dados para o Xml.
		/// </summary>
		/// <param name="writer"></param>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteElementString("Message", SecurityNamespaces.Security, (Message ?? MessageFormattable.Empty).Format());
			writer.WriteStartElement("Error", SecurityNamespaces.Security);
			if(Error != null)
				((IXmlSerializable)Error).WriteXml(writer);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine("[TokenPingResultMessage]");
			if(Message != null)
				sb.AppendLine("[Message: " + Message.Format() + "]");
			if(Error != null)
				sb.AppendLine("[Error: ").AppendLine(Error.ToString()).AppendLine("]");
			return sb.ToString();
		}
	}
	/// <summary>
	/// Representa o retorno do processo de ping do token.
	/// </summary>
	[Serializable]
	[XmlSchemaProvider("GetMySchema")]
	public sealed class TokenPingResult : ISerializable, IXmlSerializable
	{
		private DateTimeOffset _serverTime;

		private TokenPingResultStatus _status;

		private TokenPingResultMessage _message;

		private IEnumerable<IPingMessage> _notifications;

		/// <summary>
		/// Horário do servidor.
		/// </summary>
		public DateTimeOffset ServerTime
		{
			get
			{
				return _serverTime;
			}
			set
			{
				_serverTime = value;
			}
		}

		/// <summary>
		/// Status da requisição de verificação.
		/// </summary>
		public TokenPingResultStatus Status
		{
			get
			{
				return _status;
			}
			set
			{
				_status = value;
			}
		}

		/// <summary>
		/// Mensagem retornada pelo processo de verificação.
		/// </summary>
		public TokenPingResultMessage Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
			}
		}

		/// <summary>
		/// Notificações do usuário.
		/// </summary>
		public IEnumerable<IPingMessage> Notifications
		{
			get
			{
				return _notifications;
			}
			set
			{
				_notifications = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public TokenPingResult()
		{
			Status = TokenPingResultStatus.Success;
			Message = new TokenPingResultMessage(String.Empty.GetFormatter());
			Notifications = new List<IPingMessage>();
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private TokenPingResult(SerializationInfo info, StreamingContext context)
		{
			Message = (TokenPingResultMessage)info.GetValue("Message", typeof(TokenPingResultMessage));
			ServerTime = new DateTimeOffset(info.GetDateTime("ServerTimer"));
			Notifications = new List<IPingMessage>();
			TokenPingResultStatus status = TokenPingResultStatus.Success;
			if(Enum.TryParse<TokenPingResultStatus>(info.GetString("Status"), out status))
				Status = status;
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Message", Message, typeof(TokenPingResultMessage));
			info.AddValue("ServerTime", ServerTime.UtcDateTime, typeof(DateTime));
			info.AddValue("Status", Status.ToString(), typeof(string));
		}

		/// <summary>
		/// Recupera o esquema que representa o tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveSecuritySchema();
			return new System.Xml.XmlQualifiedName("TokenPingResult", SecurityNamespaces.Security);
		}

		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê os dados serializados.
		/// </summary>
		/// <param name="reader"></param>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			if(!reader.IsEmptyElement && reader.LocalName == "Message")
			{
				var message = new TokenPingResultMessage();
				((IXmlSerializable)message).ReadXml(reader);
				Message = message;
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			var serverTimeValue = reader.ReadElementString("ServerTime", SecurityNamespaces.Security);
			DateTimeOffset serverTime = DateTimeOffset.Now;
			if(DateTimeOffset.TryParse(serverTimeValue, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out serverTime))
				this.ServerTime = serverTime;
			var status = reader.ReadElementString("Status", SecurityNamespaces.Security);
			TokenPingResultStatus tokenStatus = TokenPingResultStatus.Success;
			if(Enum.TryParse<TokenPingResultStatus>(status, out tokenStatus))
				Status = tokenStatus;
			if(!reader.IsEmptyElement && reader.LocalName == "Notifications")
			{
				var notifications = new List<IPingMessage>();
				reader.ReadStartElement("Notifications", SecurityNamespaces.Security);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var join = new PingMessage();
					((System.Xml.Serialization.IXmlSerializable)join).ReadXml(reader);
					notifications.Add(join);
				}
				Notifications = notifications;
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			reader.ReadEndElement();
		}

		/// <summary>
		/// Serializa os dados xml.
		/// </summary>
		/// <param name="writer"></param>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			PingMessage[] messages = null;
			try
			{
				if(Notifications != null)
					messages = Notifications.Where(f => f != null).Select(f => new PingMessage(f)).ToArray();
				else
					messages = new PingMessage[0];
			}
			catch(Exception ex)
			{
				Message = ex;
				Status = TokenPingResultStatus.Error;
			}
			writer.WriteStartElement("Message", SecurityNamespaces.Security);
			if(Message != null)
				((IXmlSerializable)Message).WriteXml(writer);
			writer.WriteEndElement();
			writer.WriteElementString("ServerTime", SecurityNamespaces.Security, ServerTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString("Status", SecurityNamespaces.Security, Status.ToString());
			writer.WriteStartElement("Notifications", SecurityNamespaces.Security);
			if(Notifications != null)
			{
				foreach (var item in messages)
				{
					writer.WriteStartElement("PingMessage", SecurityNamespaces.Security);
					item.WriteXml(writer);
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Implemeñtação da mensagem do ping.
		/// </summary>
		class PingMessage : IPingMessage, IXmlSerializable
		{
			public int DispatcherId
			{
				get;
				set;
			}

			public string SenderName
			{
				get;
				set;
			}

			public IMessageFormattable Title
			{
				get;
				set;
			}

			public IMessageFormattable Body
			{
				get;
				set;
			}

			public string Link
			{
				get;
				set;
			}

			/// <summary>
			/// Construtor
			/// </summary>
			/// <param name="pingMessage"></param>
			public PingMessage(IPingMessage pingMessage)
			{
				this.SenderName = pingMessage.SenderName;
				this.Title = pingMessage.Title;
				this.Body = pingMessage.Body;
				this.Link = pingMessage.Link;
				this.DispatcherId = pingMessage.DispatcherId;
			}

			/// <summary>
			/// Construtor
			/// </summary>
			public PingMessage()
			{
			}

			/// <summary>
			/// Recupera o esquema que representa o tipo.
			/// </summary>
			/// <param name="xs"></param>
			/// <returns></returns>
			public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
			{
				xs.ResolveSecuritySchema();
				return new System.Xml.XmlQualifiedName("PingMessage", SecurityNamespaces.Security);
			}

			/// <summary>
			/// Lê os dados serializados.
			/// </summary>
			/// <param name="reader"></param>
			public void ReadXml(System.Xml.XmlReader reader)
			{
				reader.ReadStartElement();
				DispatcherId = reader.ReadElementContentAsInt("DispatcherId", SecurityNamespaces.Security);
				SenderName = reader.ReadElementString("SenderName", SecurityNamespaces.Security);
				Title = reader.ReadElementString("Title", SecurityNamespaces.Security).GetFormatter();
				Body = reader.ReadElementString("Body", SecurityNamespaces.Security).GetFormatter();
				Link = reader.ReadElementString("Link", SecurityNamespaces.Security);
				reader.ReadEndElement();
			}

			public System.Xml.Schema.XmlSchema GetSchema()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Serializa os dados.
			/// </summary>
			/// <param name="writer"></param>
			public void WriteXml(System.Xml.XmlWriter writer)
			{
				writer.WriteElementString("DispatcherId", SecurityNamespaces.Security, DispatcherId.ToString());
				writer.WriteElementString("SenderName", SecurityNamespaces.Security, SenderName);
				writer.WriteElementString("Title", SecurityNamespaces.Security, (Title ?? MessageFormattable.Empty).Format());
				writer.WriteElementString("Body", SecurityNamespaces.Security, (Body ?? MessageFormattable.Empty).Format());
				writer.WriteElementString("Link", SecurityNamespaces.Security, Link);
			}
		}
	}
}
