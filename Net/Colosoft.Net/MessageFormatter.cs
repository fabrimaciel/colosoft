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
using System.Reflection;
using Colosoft.Net.Json.Exceptions;

namespace Colosoft.Net.Json.Formatters
{
	/// <summary>
	/// 
	/// </summary>
	public class MessageFormatter : IMessageFormatter
	{
		private readonly string action;

		private readonly List<OperationParameter> operationParameters;

		private readonly OperationResult operationResult;

		private readonly IServiceRegister serviceRegister;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceOperationInfo"></param>
		/// <param name="serviceRegister"></param>
		protected MessageFormatter(ServiceOperation serviceOperationInfo, IServiceRegister serviceRegister) : this(serviceOperationInfo.Action, serviceOperationInfo.Parameters, serviceOperationInfo.ReturnType, serviceRegister)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="parameters"></param>
		/// <param name="returnType"></param>
		/// <param name="serviceRegister"></param>
		protected MessageFormatter(string action, IEnumerable<ParameterInfo> parameters, Type returnType, IServiceRegister serviceRegister)
		{
			try
			{
				this.serviceRegister = serviceRegister;
				this.action = action;
				this.operationParameters = new List<OperationParameter>(parameters.Select(n => new OperationParameter(n.Name, action, n.ParameterType, serviceRegister)));
				this.operationResult = new OperationResult(action, returnType, serviceRegister);
			}
			catch(Exception ex)
			{
				throw new ServiceOperationException("The service operation cannot be invoked because It uses an invalid object type, see innerException for details.", action, ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string Action
		{
			get
			{
				return this.action;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public OperationResult OperationResult
		{
			get
			{
				return this.operationResult;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<OperationParameter> OperationParameters
		{
			get
			{
				return this.operationParameters;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public IServiceRegister ServiceRegister
		{
			get
			{
				return this.serviceRegister;
			}
		}
	}
}
