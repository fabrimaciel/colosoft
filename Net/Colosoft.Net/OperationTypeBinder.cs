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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Colosoft.Net.Json
{
	/// <summary>
	/// 
	/// </summary>
	public class OperationTypeBinder : SerializationBinder, ISerializationBinder
	{
		private readonly IServiceRegister serviceRegister;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceRegister"></param>
		public OperationTypeBinder(IServiceRegister serviceRegister)
		{
			this.serviceRegister = serviceRegister;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public override Type BindToType(string assemblyName, string typeName)
		{
			return serviceRegister.GetTypeByName(typeName, false);
		}

		#if (BEFORE_NET40)
		        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializedType"></param>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
#else
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serializedType"></param>
		/// <param name="assemblyName"></param>
		/// <param name="typeName"></param>
		public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
		#endif
		
		{
			assemblyName = null;
			typeName = serializedType.Name;
		}
	}
	/// <summary>
	/// 
	/// </summary>
	public interface ISerializationBinder
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		Type BindToType(string assemblyName, string typeName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serializedType"></param>
		/// <param name="assemblyName"></param>
		/// <param name="typeName"></param>
		void BindToName(Type serializedType, out string assemblyName, out string typeName);
	}
}
