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

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Identifica a direção em que os dados devem ser tratados no GDA.
	/// </summary>
	public enum DirectionParameter : int
	{
		/// <summary>
		/// Identifica que o valor deverá apenas ser enviando para a base de dados.
		/// </summary>
		Output = 1,
		/// <summary>
		/// Identifica que o valor deverá apenas ser recuperado da base de dados.
		/// </summary>
		Input = 2,
		/// <summary>
		/// Identifica que o valor poderá ser enviado ou recuperado da base de dados.
		/// </summary>
		InputOutput = 3,
		/// <summary>
		/// O parametro é inserido apenas pelo comando insert, mas ele também pode ser considerado como um Input.
		/// </summary>
		OutputOnlyInsert = 4,
		/// <summary>
		/// O parametro é inserido apenas pelo comando insert
		/// </summary>
		OnlyInsert = 5,
		/// <summary>
		/// O parametro busca o valor se ele existir no resultado,
		/// e ele se comportar da mesma forma que o parametro Output.
		/// </summary>
		InputOptionalOutput = 6,
		/// <summary>
		/// O parametro busca o valor se ele existir no resultado.
		/// </summary>
		InputOptional = 7,
		/// <summary>
		/// O parametro busca o valor se ele existir no resultado, e ele se comportar da mesma forma que o
		/// parametro Output que é inserido apenas pelo comando insert.
		/// </summary>
		InputOptionalOutputOnlyInsert = 8
	}
}
