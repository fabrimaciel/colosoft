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

namespace Colosoft
{
	/// <summary>
	/// Assinatura de uma <see cref="Exception"/> com mensagem formatável.
	/// </summary>
	public interface IDetailsException
	{
		/// <summary>
		/// Mensagem formatável.
		/// </summary>
		IMessageFormattable MessageFormattable
		{
			get;
		}
	}
	/// <summary>
	/// Representa uma <see cref="Exception"/> detalhada.
	/// </summary>
	[Serializable]
	public class DetailsException : Exception, IDetailsException
	{
		private IMessageFormattable _messageFormattable;

		/// <summary>
		/// Mensagem formatável.
		/// </summary>
		public IMessageFormattable MessageFormattable
		{
			get
			{
				return _messageFormattable;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="message">Instancia da mensagem formatável.</param>
		public DetailsException(IMessageFormattable message) : base(message != null ? message.Format() : "")
		{
			_messageFormattable = message;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="message">Instancia da mensagem formatável.</param>
		/// <param name="innerException"></param>
		public DetailsException(IMessageFormattable message, Exception innerException) : base(message != null ? message.Format() : "", innerException)
		{
			_messageFormattable = message;
		}

		/// <summary>
		/// Construtor de deserialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected DetailsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
			_messageFormattable = info.GetValue("MessageFormattable", typeof(IMessageFormattable)) as IMessageFormattable;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("MessageFormattable", _messageFormattable);
		}
	}
	/// <summary>
	/// Implementação de um <see cref="InvalidOperationException"/> com detalhes.
	/// </summary>
	[Serializable]
	public class DetailsInvalidOperationException : InvalidOperationException, IDetailsException
	{
		private IMessageFormattable _messageFormattable;

		/// <summary>
		/// Mensagem formatável.
		/// </summary>
		public IMessageFormattable MessageFormattable
		{
			get
			{
				return _messageFormattable;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="message">Instancia da mensagem formatável.</param>
		public DetailsInvalidOperationException(IMessageFormattable message) : base(message != null ? message.Format() : "")
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="message">Instancia da mensagem formatável.</param>
		/// <param name="innerException"></param>
		public DetailsInvalidOperationException(IMessageFormattable message, Exception innerException) : base(message != null ? message.Format() : "", innerException)
		{
			_messageFormattable = message;
		}

		/// <summary>
		/// Construtor de deserialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected DetailsInvalidOperationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
			_messageFormattable = info.GetValue("MessageFormattable", typeof(IMessageFormattable)) as IMessageFormattable;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("MessageFormattable", _messageFormattable);
		}
	}
}
