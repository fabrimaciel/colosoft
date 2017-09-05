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

namespace Colosoft.Caching.Loaders
{
	/// <summary>
	/// Armazena os argumentos do evento acionado quando um entrada no cache 
	/// é inserida pelo Loader.
	/// </summary>
	public class InsertedCacheEntryEventArgs : EventArgs
	{
		/// <summary>
		/// Chave da entrada.
		/// </summary>
		public object Key
		{
			get;
			set;
		}

		/// <summary>
		/// Valor da entrada.
		/// </summary>
		public object Value
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public InsertedCacheEntryEventArgs(object key, object value)
		{
			this.Key = key;
			this.Value = value;
		}
	}
	/// <summary>
	/// Assinatura do método usado para notificar que um entrada
	/// foi inserida no cache pelo Loader.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void InsertedCacheEntryEventHandler (object sender, InsertedCacheEntryEventArgs e);
}
