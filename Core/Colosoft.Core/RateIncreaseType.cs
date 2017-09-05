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

namespace Colosoft
{
	/// <summary>
	/// Enumerador com os tipos de incremento possíveis
	/// </summary>
	public enum RateIncreaseType : byte
	{
		/// <summary>
		/// Acréscimo de unidade
		/// </summary>
		Unit = 1,
		/// <summary>
		/// Acréscimo de valor
		/// </summary>
		Value = 2,
		/// <summary>
		/// Acréscimo de unidade por percentual
		/// </summary>
		UnitPercentage = 3,
		/// <summary>
		/// Acrécimo de valor por percentual
		/// </summary>
		ValuePercentage = 4,
		/// <summary>
		/// Sustituição do número de unidades
		/// </summary>
		UnitReplace = 5,
		/// <summary>
		/// Substituição do valor
		/// </summary>
		ValueReplace = 6
	}
}
