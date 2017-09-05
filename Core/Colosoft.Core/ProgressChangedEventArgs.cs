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

namespace Colosoft.Progress
{
	/// <summary>
	/// Representa os argumentos passados a cada alteração no progresso.
	/// </summary>
	public class ProgressChangedEventArgs : System.ComponentModel.ProgressChangedEventArgs
	{
		private long _total;

		private long _processed;

		/// <summary>
		/// Total de itens.
		/// </summary>
		public long Total
		{
			get
			{
				return _total;
			}
		}

		/// <summary>
		/// Itens processados.
		/// </summary>
		public long Processed
		{
			get
			{
				return _processed;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="total">Total de itens</param>
		/// <param name="processed">Itens processados</param>
		public ProgressChangedEventArgs(long total, long processed) : this(total, processed, null)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="total">Total de itens</param>
		/// <param name="processed">Itens processados</param>
		/// <param name="userState"></param>
		public ProgressChangedEventArgs(long total, long processed, object userState) : base(((int)(processed * 100f / (total == 0 ? 1 : total))), userState)
		{
			_total = total;
			_processed = processed;
		}
	}
}
