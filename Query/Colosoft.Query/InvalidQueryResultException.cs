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
	/// Representa o erro de um resultado de consulta inválido.
	/// </summary>
	[Serializable]
	public class InvalidQueryResultException : QueryException
	{
		private ValidationQueryResult.ValidationError _error;

		/// <summary>
		/// Erro da validação.
		/// </summary>
		public ValidationQueryResult.ValidationError Error
		{
			get
			{
				return _error;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="result"></param>
		public InvalidQueryResultException(ValidationQueryResult result) : base(FormatMessage(result))
		{
			_error = result.Error;
		}

		/// <summary>
		/// Construtor usado pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecuritySafeCritical]
		protected InvalidQueryResultException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
			info.AddValue("Error", (int)_error);
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
			_error = (ValidationQueryResult.ValidationError)info.GetInt32("Error");
		}

		/// <summary>
		/// Formata mensagem do erro.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private static string FormatMessage(ValidationQueryResult result)
		{
			return string.Format("({0}) {1}", result.Error, result.Message.Format());
		}
	}
}
