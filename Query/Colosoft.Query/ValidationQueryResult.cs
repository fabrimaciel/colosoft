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
	/// Armazena o resultado da validação do resultado de uma consulta.
	/// </summary>
	public class ValidationQueryResult
	{
		private IMessageFormattable _message;

		private ValidationError _error;

		/// <summary>
		/// Mensagem da validação.
		/// </summary>
		public IMessageFormattable Message
		{
			get
			{
				return _message;
			}
		}

		/// <summary>
		/// Erro associada com a validação.
		/// </summary>
		public ValidationError Error
		{
			get
			{
				return _error;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="error">Informações do erro ocorrido.</param>
		/// <param name="message">Mensagem da validação.</param>
		public ValidationQueryResult(ValidationError error, IMessageFormattable message)
		{
			_message = message;
			_error = error;
		}

		/// <summary>
		/// Converte implicitamente para um Boolean.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator bool(ValidationQueryResult value)
		{
			return (value.Error == ValidationError.None);
		}

		/// <summary>
		/// Possíveis erros da validação.
		/// </summary>
		public enum ValidationError
		{
			/// <summary>
			/// Identifica que não existe nenhum erro.
			/// </summary>
			None,
			/// <summary>
			/// Identifica que o campos do resultado são inválido.
			/// </summary>
			InvalidFields
		}
	}
}
