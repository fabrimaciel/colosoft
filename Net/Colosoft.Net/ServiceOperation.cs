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
using System.ServiceModel.Description;
using System.Text;
using Colosoft.Net.Json.Exceptions;

namespace Colosoft.Net.Json
{
	/// <summary>
	/// 
	/// </summary>
	public class ServiceOperation
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="action"></param>
		public ServiceOperation(OperationDescription operation, string action)
		{
			if(action == null || action.Trim().Equals(string.Empty))
				throw new ArgumentException("The given action related to the given operation cannot be null or empty.", "action");
			if(operation == null)
				throw new ArgumentNullException("operation", "the given operation description cannot be null.");
			#if (BEFORE_NET45)
			            MethodInfo operationInfo = operation.SyncMethod;
#else
			MethodInfo operationInfo = operation.SyncMethod ?? operation.TaskMethod;
			#endif
			if(operationInfo == null)
				throw new NullReferenceException("The SynMethod property on operation cannot be null.");
			this.Action = action.Trim();
			this.Parameters = operationInfo.GetParameters();
			this.ReturnType = operationInfo.ReturnType;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Action
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		public ParameterInfo[] Parameters
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		public Type ReturnType
		{
			get;
			private set;
		}
	}
}
