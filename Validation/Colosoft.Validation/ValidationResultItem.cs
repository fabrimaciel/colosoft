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
	/// Representa um erro de validação
	/// </summary>
	[Serializable]
	public class ValidationResultItem
	{
		private string _key;

		private IMessageFormattable _message;

		private ValidationResultType _resultType;

		private object[] _parameters;

		/// <summary>
		/// Propriedade que gerou o erro
		/// </summary>
		public string Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}

		/// <summary>
		/// Mensagem de erro
		/// </summary>
		public IMessageFormattable Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
			}
		}

		/// <summary>
		/// Tipo de retorno
		/// </summary>
		public ValidationResultType ResultType
		{
			get
			{
				return _resultType;
			}
			set
			{
				_resultType = value;
			}
		}

		/// <summary>
		/// Lista de parâmetro do resultado.
		/// </summary>
		public object[] Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ValidationResultItem()
		{
		}

		/// <summary>
		/// Cria uma nova instancia com os valores iniciais.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="message"></param>
		/// <param name="resultType"></param>
		/// <param name="parameters"></param>
		public ValidationResultItem(string key, IMessageFormattable message, ValidationResultType resultType, params object[] parameters)
		{
			_key = key;
			_message = message;
			_resultType = resultType;
			_parameters = parameters ?? new object[0];
		}
	}
}
