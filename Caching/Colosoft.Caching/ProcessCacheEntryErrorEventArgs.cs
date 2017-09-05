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
using System.Collections;

namespace Colosoft.Caching.Loaders
{
	/// <summary>
	/// Armazena os argumento do erro ocorrido ao processa a entrada do cache.
	/// </summary>
	public class ProcessCacheEntryErrorEventArgs : CacheErrorEventArgs
	{
		/// <summary>
		/// Entrada que está sendo processada.
		/// </summary>
		public DictionaryEntry Entry
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="error"></param>
		public ProcessCacheEntryErrorEventArgs(DictionaryEntry entry, Exception error) : base(error)
		{
			this.Entry = entry;
		}
	}
	/// <summary>
	/// Assinatura dos evento acionado quando ocorre um erro ao processar um entrada do cache.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void ProcessCacheEntryErrorEventHandler (object sender, ProcessCacheEntryErrorEventArgs e);
}
