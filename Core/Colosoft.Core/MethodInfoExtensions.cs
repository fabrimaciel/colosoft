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
using System.Diagnostics;
using System.Reflection;

namespace Colosoft.Reflection
{
	/// <summary>
	/// 
	/// </summary>
	public static class MethodInfoExtensions
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Type[] knownTypesWithReturn =  {
			typeof(Func<>),
			typeof(Func<, >),
			typeof(Func<, , >),
			typeof(Func<, , , >),
			typeof(Func<, , , , >)
		};

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Type[] knownTypesWithoutReturn =  {
			typeof(Action),
			typeof(Action<>),
			typeof(Action<, >),
			typeof(Action<, , >),
			typeof(Action<, , , >)
		};

		/// <summary>
		/// Cria um delegate para o <see cref="MethodInfo"/> informado.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static Delegate CreateDelegate(this MethodInfo method, object instance)
		{
			if(method.GetType().IsGenericType)
				throw new InvalidOperationException("Unable to create a delegate for a method with generic arguments.");
			List<ParameterInfo> parameters = method.GetParameters().ToList();
			if(parameters.Count > 4)
				throw new InvalidOperationException("Unable to create a delegate for a method with more than 4 parameters.");
			bool hasReturnValue = method.ReturnType != typeof(void);
			Type[] availableTypes = (hasReturnValue) ? knownTypesWithReturn : knownTypesWithoutReturn;
			Type delegateType = availableTypes[parameters.Count];
			List<Type> geneticArgumenTypes = new List<Type>();
			parameters.ForEach(info => geneticArgumenTypes.Add(info.ParameterType));
			if(hasReturnValue)
			{
				geneticArgumenTypes.Add(method.ReturnType);
			}
			Type resultingType = (delegateType.IsGenericType) ? delegateType.MakeGenericType(geneticArgumenTypes.ToArray()) : delegateType;
			Delegate methodWrapper = Delegate.CreateDelegate(resultingType, instance, method);
			return methodWrapper;
		}
	}
}
