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

namespace Colosoft.Text.InterpreterExpression
{
	/// <summary>
	/// Representa os tokens de um container.
	/// </summary>
	public class ContainerChars : ICloneable
	{
		private readonly char _start;

		private readonly char _stop;

		/// <summary>
		/// Caracter de inicio do container.
		/// </summary>
		public char Start
		{
			get
			{
				return _start;
			}
		}

		/// <summary>
		/// Caracter de parada.
		/// </summary>
		public char Stop
		{
			get
			{
				return _stop;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="stop"></param>
		public ContainerChars(char start, char stop)
		{
			_start = start;
			_stop = stop;
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		public ContainerChars Clone()
		{
			return new ContainerChars(_start, _stop);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}
	}
}
