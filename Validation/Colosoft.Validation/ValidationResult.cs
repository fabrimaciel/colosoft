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
	/// Representa um resultado de validação
	/// </summary>
	public class ValidationResult
	{
		private bool _isValid = true;

		private List<ValidationResultItem> _items = new List<ValidationResultItem>();

		/// <summary>
		/// Indica se a validação foi bem sucedida
		/// </summary>
		public bool IsValid
		{
			get
			{
				return _isValid;
			}
			set
			{
				_isValid = value;
			}
		}

		/// <summary>
		/// Enumerador com os itens de resultado da validação
		/// </summary>
		public List<ValidationResultItem> Items
		{
			get
			{
				return _items;
			}
		}

		/// <summary>
		/// Mescla os dados do resultado.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public ValidationResult Merge(ValidationResult result)
		{
			if(result != null)
			{
				if(!result.IsValid)
					IsValid = false;
				Items.AddRange(result.Items);
			}
			return this;
		}

		/// <summary>
		/// Identifica que o resultado é inválido.
		/// </summary>
		/// <param name="items"></param>
		public void Invalid(params ValidationResultItem[] items)
		{
			_isValid = false;
			if(items != null)
				_items.AddRange(items);
		}

		/// <summary>
		/// Identifica que o resulta é inválido.
		/// </summary>
		/// <param name="resultType"></param>
		/// <param name="message"></param>
		public void Invalid(ValidationResultType resultType, IMessageFormattable message)
		{
			_isValid = false;
			_items.Add(new ValidationResultItem(null, message, resultType));
		}
	}
}
