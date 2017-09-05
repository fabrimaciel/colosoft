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
using System.IO;
using System.Linq;
using System.Text;
using Colosoft.Net.Json.Exceptions;

namespace Colosoft.Net.Json.Configuration
{
	/// <summary>
	/// 
	/// </summary>
	public class ResolverType : ConfigServiceElement
	{
		private bool wasResolved;

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("serviceType", IsRequired = true)]
		public ServiceType ServiceType
		{
			get
			{
				return (ServiceType)this["serviceType"];
			}
			set
			{
				this["serviceType"] = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("binderType", IsRequired = true)]
		public ServiceType BinderType
		{
			get
			{
				return (ServiceType)this["binderType"];
			}
			set
			{
				this["binderType"] = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void PostDeserialize()
		{
			base.PostDeserialize();
			if(this.ServiceType.Name == "*")
				throw new ResolverTypeException("The ServiceType property of ResolverType must contain a valid CLR type name to be resolved by binder type associated.");
			if(this.BinderType.Name == "*")
				throw new ResolverTypeException("The BinderType property of ResolverType must contain a valid CLR type name to resolve the service type associated.");
			this.wasResolved = false;
		}

		/// <summary>
		/// 
		/// </summary>
		public override object Key
		{
			get
			{
				return this.ServiceType;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool WasResolved
		{
			get
			{
				return this.wasResolved;
			}
			internal set
			{
				this.wasResolved = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("serviceType: {0}, binderType: {1}", this.ServiceType, this.BinderType);
		}
	}
}
