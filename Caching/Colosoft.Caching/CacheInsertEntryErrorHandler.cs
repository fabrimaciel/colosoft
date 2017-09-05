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
	/// Armazena os argumentos do erro ocorrido.
	/// </summary>
	public class CacheInsertEntryErrorEventArgs : CacheErrorEventArgs
	{
		private object _key;

		private object _value;

		/// <summary>
		/// Chave da entrada do cache.
		/// </summary>
		public object Key
		{
			get
			{
				return _key;
			}
		}

		/// <summary>
		/// Valor do entrada do cache.
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="error"></param>
		public CacheInsertEntryErrorEventArgs(object key, object value, Exception error) : base(error)
		{
			_key = key;
			_value = value;
		}
	}
	/// <summary>
	/// Assinatura do método acionado quando ocorreu um erro ao inserir uma entrada do cache.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <returns></returns>
	public delegate void CacheInsertEntryErrorHandler (object sender, CacheInsertEntryErrorEventArgs e);
}
