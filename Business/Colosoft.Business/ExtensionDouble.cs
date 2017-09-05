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

namespace Colosoft.Business
{
	/// <summary>
	/// Extensão para <c>double</c>.
	/// </summary>
	public static class ExtensionDouble
	{
		private static readonly double HPrecision;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		static ExtensionDouble()
		{
			try
			{
				HPrecision = Convert.ToDouble(Colosoft.Configuration.Configuration.Instance.ReadPath(@"Rates\MoneyPrecision"));
			}
			catch
			{
				HPrecision = 2.0;
			}
		}

		/// <summary>
		/// Verifica se os valores estão próximos.
		/// </summary>
		/// <param name="d1">Valor 1.</param>
		/// <param name="d2">Valor 2.</param>
		/// <returns>Verdadeiro se os valores estão próximos.</returns>
		public static bool IsClose(this double d1, double d2)
		{
			return d1 == d2 || Math.Abs(d1 - d2) <= HPrecision;
		}

		/// <summary>
		/// Verifica se os valores não estão próximos.
		/// </summary>
		/// <param name="d1">Valor 1.</param>
		/// <param name="d2">Valor 2.</param>
		/// <returns>Verdadeiro se os valores não estão próximos.</returns>
		public static bool IsNotClose(this double d1, double d2)
		{
			return !(d1 == d2 || Math.Abs(d1 - d2) <= HPrecision);
		}
	}
}
