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
using System.Xml;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Web.Services.Protocols;
using System.Security.Permissions;

namespace Colosoft.ServiceProcess
{
	/// <summary>
	/// Representa um servidor o serviço.
	/// </summary>
	[Serializable]
	public class ServiceException : ServerException, ISerializable
	{
		private int _errorCode;

		private int _eventId;

		private XmlQualifiedName _faultCode;

		private bool _logException;

		private EventLogEntryType _logLevel;

		private bool _reportException;

		/// <summary>
		/// Código do erro.
		/// </summary>
		public int ErrorCode
		{
			get
			{
				return _errorCode;
			}
			set
			{
				_errorCode = value;
			}
		}

		/// <summary>
		/// Identificador do evento.
		/// </summary>
		public int EventId
		{
			get
			{
				return _eventId;
			}
			set
			{
				_eventId = value;
			}
		}

		/// <summary>
		/// Código da falha.
		/// </summary>
		public XmlQualifiedName FaultCode
		{
			get
			{
				return _faultCode;
			}
			protected set
			{
				_faultCode = value;
			}
		}

		/// <summary>
		/// Identifica se é para salvar no log.
		/// </summary>
		public bool LogException
		{
			get
			{
				return _logException;
			}
			set
			{
				_logException = value;
			}
		}

		/// <summary>
		/// Nível do log.
		/// </summary>
		public EventLogEntryType LogLevel
		{
			get
			{
				return _logLevel;
			}
			set
			{
				_logLevel = value;
			}
		}

		/// <summary>
		/// Identifica se é para reportar o erro.
		/// </summary>
		public bool ReportException
		{
			get
			{
				return _reportException;
			}
			set
			{
				_reportException = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ServiceException()
		{
			_faultCode = Soap12FaultCodes.ReceiverFaultCode;
			_logLevel = EventLogEntryType.Warning;
			_eventId = 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="errorCode"></param>
		public ServiceException(int errorCode) : this(errorCode, false)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ServiceException(string message) : base(message)
		{
			_faultCode = Soap12FaultCodes.ReceiverFaultCode;
			_logLevel = EventLogEntryType.Warning;
			_eventId = 0;
		}

		/// <summary>
		/// Cria a instancia com o código do erro.
		/// </summary>
		/// <param name="errorCode"></param>
		/// <param name="logException"></param>
		public ServiceException(int errorCode, bool logException)
		{
			_faultCode = Soap12FaultCodes.ReceiverFaultCode;
			_logLevel = EventLogEntryType.Warning;
			_eventId = 0;
			this.ErrorCode = errorCode;
			this.LogException = logException;
		}

		/// <summary>
		/// Construtor de serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			_faultCode = Soap12FaultCodes.ReceiverFaultCode;
			_logLevel = EventLogEntryType.Warning;
			_eventId = 0;
			if(info == null)
				throw new ArgumentNullException("info");
			_logException = (bool)info.GetValue("m_logException", typeof(bool));
			_reportException = (bool)info.GetValue("m_reportException", typeof(bool));
			_errorCode = (int)info.GetValue("m_errorCode", typeof(int));
			_faultCode = (XmlQualifiedName)info.GetValue("m_faultCode", typeof(XmlQualifiedName));
			_logLevel = (EventLogEntryType)info.GetValue("m_logLevel", typeof(EventLogEntryType));
			_eventId = (int)info.GetValue("m_eventId", typeof(int));
		}

		/// <summary>
		/// Cria a instancia com a mensagem e o erro interno.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public ServiceException(string message, Exception innerException) : base(message, innerException)
		{
			_faultCode = Soap12FaultCodes.ReceiverFaultCode;
			_logLevel = EventLogEntryType.Warning;
			_eventId = 0;
		}

		/// <summary>
		/// Cria a instancia com a mensagem o e código do erro.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="errorCode"></param>
		public ServiceException(string message, int errorCode) : this(message, errorCode, false)
		{
		}

		/// <summary>
		/// Cria a instancia com a mensagem, o código do erro e a identificação se é para gerar um log.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="errorCode"></param>
		/// <param name="logException"></param>
		public ServiceException(string message, int errorCode, bool logException) : base(message)
		{
			_faultCode = Soap12FaultCodes.ReceiverFaultCode;
			_logLevel = EventLogEntryType.Warning;
			_eventId = 0;
			this.ErrorCode = errorCode;
			this.LogException = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="errorCode"></param>
		/// <param name="innerException"></param>
		public ServiceException(string message, int errorCode, Exception innerException) : base(message, innerException)
		{
			_faultCode = Soap12FaultCodes.ReceiverFaultCode;
			_logLevel = EventLogEntryType.Warning;
			_eventId = 0;
			this.ErrorCode = errorCode;
			this.LogException = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="errorCode"></param>
		/// <param name="logException"></param>
		/// <param name="innerException"></param>
		public ServiceException(string message, int errorCode, bool logException, Exception innerException) : base(message, innerException)
		{
			_faultCode = Soap12FaultCodes.ReceiverFaultCode;
			_logLevel = EventLogEntryType.Warning;
			_eventId = 0;
			this.ErrorCode = errorCode;
			this.LogException = logException;
		}

		/// <summary>
		/// Recupera as propriedades.
		/// </summary>
		/// <param name="properties"></param>
		protected virtual void GetExceptionProperties(ExceptionPropertyCollection properties)
		{
		}

		/// <summary>
		/// Recupera os dados para  serializaçaõ.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("m_logException", _logException);
			info.AddValue("m_reportException", _reportException);
			info.AddValue("m_errorCode", _errorCode);
			info.AddValue("m_faultCode", _faultCode);
			info.AddValue("m_logLevel", _logLevel);
			info.AddValue("m_eventId", _eventId);
			base.GetObjectData(info, context);
		}

		/// <summary>
		/// Recupera a <see cref="SoapException"/> equivalente.
		/// </summary>
		/// <returns></returns>
		internal virtual SoapException ToSoapException()
		{
			XmlNode detail = null;
			var properties = new ExceptionPropertyCollection();
			this.GetExceptionProperties(properties);
			if((base.InnerException != null) || (properties.Count > 0))
			{
				using (var writer = new System.IO.StringWriter(System.Globalization.CultureInfo.InvariantCulture))
				{
					XmlTextWriter writer2 = new XmlTextWriter(writer);
					writer2.WriteStartDocument();
					writer2.WriteStartElement(SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);
					writer2.WriteAttributeString("xmlns", "xsd", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema");
					writer2.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");
					if(base.InnerException != null)
					{
						writer2.WriteAttributeString("InnerExceptionType", base.InnerException.GetType().Name);
						writer2.WriteAttributeString("InnerExceptionMessage", base.InnerException.Message);
					}
					if(properties.Count > 0)
					{
					}
					writer2.WriteEndElement();
					writer2.WriteEndDocument();
					XmlDocument document = new XmlDocument();
					document.LoadXml(writer.ToString());
					detail = document.DocumentElement;
				}
			}
			return new SoapException(this.Message, this.FaultCode, string.Empty, string.Empty, detail, new SoapFaultSubCode(new XmlQualifiedName(base.GetType().Name)), null);
		}
	}
}
