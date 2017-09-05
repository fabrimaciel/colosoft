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

namespace Colosoft.Validation
{
	/// <summary>
	/// REepresenta o retorno de uma regra de entrada customizada.
	/// </summary>
	public class InputValidateCheckResult
	{
		/// <summary>
		/// Construtor padrão quando houver sucesso.
		/// </summary>
		public InputValidateCheckResult()
		{
			Success = true;
			Message = String.Empty;
		}

		/// <summary>
		/// Contrutor para informar erro com a mensagem do erro.
		/// </summary>
		/// <param name="errorMessage"></param>
		public InputValidateCheckResult(string errorMessage)
		{
			Message = errorMessage;
		}

		/// <summary>
		/// Indica que houve sucesso na validação.
		/// </summary>
		public bool Success
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem a ser apresentada.
		/// </summary>
		public string Message
		{
			get;
			set;
		}
	}
}
