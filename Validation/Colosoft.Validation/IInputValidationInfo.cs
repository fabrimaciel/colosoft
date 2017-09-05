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
	/// Contrato para classes que armazenam informações de validação de entrada.
	/// </summary>
	public interface IInputValidationInfo
	{
		/// <summary>
		/// Lista de valores indexados.
		/// </summary>
		IEnumerable<IIndexedValue> IndexedValues
		{
			get;
			set;
		}

		/// <summary>
		/// Lista de valores.
		/// </summary>
		IEnumerable<IPropertyValue> Values
		{
			get;
			set;
		}

		/// <summary>
		/// Validação de caracteres válidos.
		/// </summary>
		IValidChars ValidChars
		{
			get;
			set;
		}

		/// <summary>
		/// Máscara aplicada à propriedade
		/// </summary>
		IMask Mask
		{
			get;
			set;
		}

		/// <summary>
		/// Validação de dígitos verificadores.
		/// </summary>
		ICheckDigits CheckDigits
		{
			get;
			set;
		}

		/// <summary>
		/// Validação de valor padrão.
		/// </summary>
		IDefaultValue DefaultValue
		{
			get;
			set;
		}

		/// <summary>
		/// Validação de tamanho.
		/// </summary>
		ILength Length
		{
			get;
			set;
		}

		/// <summary>
		/// Validação de faixa.
		/// </summary>
		IRange Range
		{
			get;
			set;
		}

		/// <summary>
		/// Validação de entrada com customização.
		/// </summary>
		IInputValidateCustomization Customization
		{
			get;
			set;
		}

		/// <summary>
		/// Caso de entrada de caracteres.
		/// </summary>
		CharacterCase CharCase
		{
			get;
			set;
		}
	}
}
