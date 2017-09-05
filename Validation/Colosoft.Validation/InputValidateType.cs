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

namespace Colosoft.Validation.Configuration
{
	/// <summary>
	/// Tipod de validação de entrada
	/// </summary>
	public enum InputValidateType : byte
	{
		/// <summary>
		/// Grupo de outras validações de entrada
		/// </summary>
		Group = 0,
		/// <summary>
		/// Valor inicial
		/// </summary>
		DefaultValue = 1,
		/// <summary>
		/// Valores indexados
		/// </summary>
		IndexedValues = 2,
		/// <summary>
		/// Valores
		/// </summary>
		Values = 3,
		/// <summary>
		/// Faixa de valores possíveis
		/// </summary>
		Range = 4,
		/// <summary>
		/// Tamanho do campo
		/// </summary>
		Length = 5,
		/// <summary>
		/// Caracteres válidos
		/// </summary>
		ValidChars = 6,
		/// <summary>
		/// Máscara
		/// </summary>
		Mask = 7,
		/// <summary>
		/// Digitos verificadores.
		/// </summary>
		CheckDigits = 8,
		/// <summary>
		/// Lista de algum tipo do sistema.
		/// </summary>
		TypeList = 9,
		/// <summary>
		/// Customização.
		/// </summary>
		Customization = 10,
		/// <summary>
		/// Conversão para maiúsculo.
		/// </summary>
		CharacterUpperCase = 11,
		/// <summary>
		/// Conversão para minúsculo.
		/// </summary>
		CharacterLowerCase = 12
	}
}
