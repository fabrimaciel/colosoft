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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Colosoft.Caching.Exceptions
{
	/// <summary>
	/// Representa um erro de falha na operação.
	/// </summary>
	[Serializable]
	public class OperationFailedException : CacheException
	{
		private bool _isTracable;

		/// <summary>
		/// Identifica se o erro é rastreável.
		/// </summary>
		public bool IsTracable
		{
			get
			{
				return _isTracable;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public OperationFailedException()
		{
			_isTracable = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public OperationFailedException(string message) : base(message)
		{
			_isTracable = true;
		}

		/// <summary>
		/// Construtor usado na serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected OperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			_isTracable = Convert.ToBoolean(info.GetString("_isTracable"));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="isTracable"></param>
		public OperationFailedException(string message, bool isTracable) : base(message)
		{
			_isTracable = isTracable;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public OperationFailedException(string message, Exception inner) : base(message, inner)
		{
			_isTracable = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="inner"></param>
		/// <param name="isTracable"></param>
		public OperationFailedException(string reason, Exception inner, bool isTracable) : base(reason, inner)
		{
			_isTracable = true;
			_isTracable = isTracable;
		}

		/// <summary>
		/// Recupera os dados serializados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("_isTracable", _isTracable);
		}
	}
}
