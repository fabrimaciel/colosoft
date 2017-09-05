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
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Description;
using Newtonsoft.Json;
using Colosoft.Net.Json.Configuration;
using Colosoft.Net.Json.Formatters;
using System.ServiceModel.Dispatcher;
using Colosoft.Net.Json;

namespace Colosoft.Net.Json
{
	/// <summary>
	/// Class WebHttpJsonNetBehavior.
	/// </summary>
	public class WebHttpJsonNetBehavior : WebHttpJsonBehavior
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WebHttpJsonNetBehavior"/> class.
		/// </summary>
		public WebHttpJsonNetBehavior() : this(new List<Type>(), true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebHttpJsonNetBehavior"/> class.
		/// </summary>
		/// <param name="knownTypes">The known types.</param>
		/// <param name="enableUriTemplate">if set to <c>true</c> [enable URI template].</param>
		public WebHttpJsonNetBehavior(IEnumerable<Type> knownTypes, bool enableUriTemplate = true) : base(knownTypes, enableUriTemplate)
		{
			SerializerSettings serializerInfo = this.ConfigRegister.SerializerConfig;
			CustomContractResolver resolver = new CustomContractResolver(true, false, this.ConfigRegister.TryToNormalize) {
				DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
			};
			this.Serializer = new JsonSerializer {
				NullValueHandling = NullValueHandling.Ignore,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				TypeNameHandling = TypeNameHandling.None,
				ContractResolver = resolver
			};
			if(!serializerInfo.OnlyPublicConstructor)
				Serializer.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
			if(serializerInfo.EnablePolymorphicMembers)
			{
				Serializer.Binder = new OperationTypeBinder(this.ConfigRegister);
				Serializer.TypeNameHandling = TypeNameHandling.Objects;
			}
		}

		/// <summary>
		/// Makes the dispatch message formatter.
		/// </summary>
		/// <param name="operationDescription">The operation description.</param>
		/// <param name="endpoint">The endpoint.</param>
		/// <returns>IDispatchJsonMessageFormatter.</returns>
		public override IDispatchJsonMessageFormatter MakeDispatchMessageFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			return new DispatchJsonNetMessageFormatter(operationDescription, this.Serializer, this.ConfigRegister);
		}

		/// <summary>
		/// Makes the client message formatter.
		/// </summary>
		/// <param name="operationDescription">The operation description.</param>
		/// <param name="endpoint">The endpoint.</param>
		/// <returns>IClientJsonMessageFormatter.</returns>
		public override IClientJsonMessageFormatter MakeClientMessageFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			return new ClientJsonNetMessageFormatter(operationDescription, endpoint, this.Serializer, this.ConfigRegister);
		}

		/// <summary>
		/// Gets the query string converter.
		/// </summary>
		/// <param name="operationDescription">The service operation.</param>
		/// <returns>A <see cref="T:System.ServiceModel.Dispatcher.QueryStringConverter" /> instance.</returns>
		protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
		{
			return new QueryStringJsonConverter(this.Serializer, this.ConfigRegister);
		}

		/// <summary>
		/// Gets the serializer.
		/// </summary>
		/// <value>The serializer.</value>
		public JsonSerializer Serializer
		{
			get;
			private set;
		}
	}
}
