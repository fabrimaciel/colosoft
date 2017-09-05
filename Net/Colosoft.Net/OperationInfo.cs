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
using Colosoft.Net.Json.Exceptions;

namespace Colosoft.Net.Json
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class OperationInfo : IOperationInfo
	{
		private readonly Type originalType;

		private readonly Type normalizedType;

		private readonly string action;

		private readonly IServiceRegister serviceRegister;

		private readonly OperationInfoType operationType;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="originalType"></param>
		/// <param name="operationType"></param>
		/// <param name="serviceRegister"></param>
		protected OperationInfo(string action, Type originalType, OperationInfoType operationType, IServiceRegister serviceRegister)
		{
			this.action = action;
			this.originalType = originalType;
			this.serviceRegister = serviceRegister;
			this.operationType = operationType;
			this.normalizedType = this.serviceRegister.TryToNormalize(originalType);
			string messageError;
			switch(operationType)
			{
			case OperationInfoType.Parameter:
				messageError = "The service is not able to use the given object parameter type for serializing / deserializing objects, in order to resolve this kind of problem, you must to use a serviceTypeRegister on *.config file";
				break;
			case OperationInfoType.Result:
				messageError = "The service is not able to use the given object return type for serializing / deserializing objects, in order to resolve this kind of problem, you must to use a serviceTypeRegister on *.config file";
				break;
			default:
				messageError = "Operation Type unknown.";
				break;
			}
			if(normalizedType == null && serviceRegister.CheckOperationTypes)
				throw new TypeUnresolvedException(messageError, originalType);
		}

		/// <summary>
		/// 
		/// </summary>
		public Type OrginalType
		{
			get
			{
				return this.originalType;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Type NormalizedType
		{
			get
			{
				return this.normalizedType;
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
		public OperationInfoType OperationType
		{
			get
			{
				return this.operationType;
			}
		}
	}
}
