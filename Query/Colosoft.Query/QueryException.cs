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

namespace Colosoft.Query
{
	/// <summary>
	/// Representa uma Exception disparada pela parte de consulta.
	/// </summary>
	[Serializable]
	public class QueryException : Exception
	{
		/// <summary>
		/// Cria a instancia com a mensagem informada.
		/// </summary>
		/// <param name="message">Mensagem do erro.</param>
		public QueryException(string message) : base(message)
		{
		}

		/// <summary>
		/// Cria a instancia com a mensagem e o erro interno.
		/// </summary>
		/// <param name="message">Mensagem do erro.</param>
		/// <param name="innerException">Erro interno.</param>
		public QueryException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Construtor usado pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecuritySafeCritical]
		protected QueryException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Método usado pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecurityCritical]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
	/// <summary>
	/// Representa o erro de operação inválida.
	/// </summary>
	[Serializable]
	public class QueryInvalidOperationException : QueryException
	{
		/// <summary>
		/// Cria a instancia com a mensagem informada.
		/// </summary>
		/// <param name="message">Mensagem do erro.</param>
		public QueryInvalidOperationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Cria a instancia com a mensagem e o erro interno.
		/// </summary>
		/// <param name="message">Mensagem do erro.</param>
		/// <param name="innerException">Erro interno.</param>
		public QueryInvalidOperationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Construtor usado pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecuritySafeCritical]
		protected QueryInvalidOperationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Método usado pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecurityCritical]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
	/// <summary>
	/// Representa o erro na estratégio de vinculação de tipo.
	/// </summary>
	[Serializable]
	public class TypeBindStrategyException : QueryException
	{
		/// <summary>
		/// Cria a instancia com a mensagem informada.
		/// </summary>
		/// <param name="message">Mensagem do erro.</param>
		public TypeBindStrategyException(string message) : base(message)
		{
		}

		/// <summary>
		/// Cria a instancia com a mensagem e o erro interno.
		/// </summary>
		/// <param name="message">Mensagem do erro.</param>
		/// <param name="innerException">Erro interno.</param>
		public TypeBindStrategyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Construtor usado pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecuritySafeCritical]
		protected TypeBindStrategyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Método usado pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecurityCritical]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
	/// <summary>
	/// Representa um erro do parser
	/// </summary>
	[Serializable]
	public class ConditionalParserException : QueryException
	{
		/// <summary>
		/// Cria a instancia com a mensagem informada.
		/// </summary>
		/// <param name="message">Mensagem do erro.</param>
		public ConditionalParserException(string message) : base(message)
		{
		}

		/// <summary>
		/// Cria a instancia com a mensagem e o erro interno.
		/// </summary>
		/// <param name="message">Mensagem do erro.</param>
		/// <param name="innerException">Erro interno.</param>
		public ConditionalParserException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Construtor usado pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecuritySafeCritical]
		protected ConditionalParserException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Método usado pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecurityCritical]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
