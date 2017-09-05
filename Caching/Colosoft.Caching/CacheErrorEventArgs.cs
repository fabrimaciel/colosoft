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

namespace Colosoft.Caching
{
	/// <summary>
	/// Armazena os argumento de um evento de error.
	/// </summary>
	public class CacheErrorEventArgs : EventArgs
	{
		private Exception _error;

		/// <summary>
		/// Instancia do erro relacionado.
		/// </summary>
		public virtual Exception Error
		{
			get
			{
				return _error;
			}
			protected set
			{
				_error = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="error"></param>
		public CacheErrorEventArgs(Exception error)
		{
			_error = error;
		}
	}
	/// <summary>
	/// Assinatura do evento acionado quando ocorrer algum erro no cache.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void CacheErrorEventHandler (object sender, CacheErrorEventArgs e);
}
