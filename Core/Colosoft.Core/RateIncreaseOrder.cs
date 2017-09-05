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
	/// Ordem em que o incremento será aplicado
	/// </summary>
	public enum RateIncreaseOrder : byte
	{
		/// <summary>
		/// Antes de se posicionar as unidades(só aceita incrementos que afetam as unidades)
		/// </summary>
		BeforeUnits = 1,
		/// <summary>
		/// Antes de fazer o cálculo do preço
		/// </summary>
		BeforeRate = 2,
		/// <summary>
		/// No final do cálculo(só aceita incrementos que afetem valor)
		/// </summary>
		AfterRate = 3
	}
}
