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

using Colosoft.Net.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Colosoft.Net.Json
{
	/// <summary>
	/// 
	/// </summary>
	public class WebHttpUriJsonNetBehavior : WebHttpJsonNetBehavior
	{
		private WebHttpBehavior basicImplementor;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebHttpUriJsonNetBehavior"/> class.
		/// </summary>
		public WebHttpUriJsonNetBehavior() : this(new List<Type>())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebHttpUriJsonNetBehavior"/> class.
		/// </summary>
		/// <param name="knownTypes"></param>
		public WebHttpUriJsonNetBehavior(IEnumerable<Type> knownTypes) : base(knownTypes)
		{
			this.basicImplementor = new WebHttpBehavior();
		}

		/// <summary>
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected override IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			IDispatchMessageFormatter formatter = null;
			if(this.IsGetOperation(operationDescription))
			{
				var flags = BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod;
				var method = typeof(WebHttpBehavior).GetMethod("GetRequestDispatchFormatter", flags);
				return method.Invoke(this.basicImplementor, new object[] {
					operationDescription,
					endpoint
				}) as IDispatchMessageFormatter;
			}
			else
			{
				formatter = new NissingDispatchMessageFormatter();
			}
			return formatter;
		}

		/// <summary>
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected override IClientMessageFormatter GetRequestClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			IClientMessageFormatter formatter = null;
			if(this.IsGetOperation(operationDescription))
			{
				var flags = BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod;
				var method = typeof(WebHttpBehavior).GetMethod("GetRequestClientFormatter", flags);
				return method.Invoke(this.basicImplementor, new object[] {
					operationDescription,
					endpoint
				}) as IClientMessageFormatter;
			}
			else
			{
				formatter = new MissingClientMessageFormatter();
			}
			return formatter;
		}

		/// <summary>
		/// Gets the query string converter.
		/// </summary>
		/// <param name="operationDescription">The operation description.</param>
		/// <returns></returns>
		protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
		{
			return new QueryStringJsonConverter(this.Serializer, this.ConfigRegister);
		}
	}
	/// <summary>
	/// 
	/// </summary>
	public class NissingDispatchMessageFormatter : IDispatchMessageFormatter
	{
		/// <summary>
		/// Deserializes a message into an array of parameters.
		/// </summary>
		/// <param name="message">The incoming message.</param>
		/// <param name="parameters">The objects that are passed to the operation as parameters.</param>
		/// <exception cref="System.InvalidOperationException">The operation was invoked mustn't be invoked because It's not implements.</exception>
		public void DeserializeRequest(Message message, object[] parameters)
		{
			throw new InvalidOperationException("The operation was invoked mustn't be invoked because the given operation wasn't .");
		}

		/// <summary>
		/// Serializes a reply message from a specified message version, array of parameters, and a return value.
		/// </summary>
		/// <param name="messageVersion">The SOAP message version.</param>
		/// <param name="parameters">The out parameters.</param>
		/// <param name="result">The return value.</param>
		/// <returns>
		/// The serialized reply message.
		/// </returns>
		/// <exception cref="System.InvalidOperationException">The operation was invoked mustn't be invoked because It's not implements.</exception>
		public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
		{
			throw new InvalidOperationException("The operation was invoked mustn't be invoked because It's not implemented.");
		}
	}
	/// <summary>
	/// 
	/// </summary>
	public class MissingClientMessageFormatter : IClientMessageFormatter
	{
		/// <summary>
		/// Converts a message into a return value and out parameters that are passed back to the calling operation.
		/// </summary>
		/// <param name="message">The inbound message.</param>
		/// <param name="parameters">Any out values.</param>
		/// <returns>
		/// The return value of the operation.
		/// </returns>
		/// <exception cref="System.InvalidOperationException">The operation was invoked mustn't be invoked because It's not implements.</exception>
		public object DeserializeReply(Message message, object[] parameters)
		{
			throw new InvalidOperationException("The operation was invoked mustn't be invoked because It's not implemented.");
		}

		/// <summary>
		/// Converts an <see cref="T:System.Object" /> array into an outbound <see cref="T:System.ServiceModel.Channels.Message" />.
		/// </summary>
		/// <param name="messageVersion">The version of the SOAP message to use.</param>
		/// <param name="parameters">The parameters passed to the WCF client operation.</param>
		/// <returns>
		/// The SOAP message sent to the service operation.
		/// </returns>
		/// <exception cref="System.InvalidOperationException">The operation was invoked mustn't be invoked because It's not implements.</exception>
		public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
		{
			throw new InvalidOperationException("The operation was invoked mustn't be invoked because It's not implemented.");
		}
	}
}
