﻿/* 
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

namespace Colosoft.Net
{
	/// <summary>
	/// Representa os evento acionado quando o download dos dados for finalizado.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void DownloadCompletedEventHandler (object sender, DownloadCompletedEventArgs e);
	/// <summary>
	/// Prover dados para o evento <see cref="DownloadCompletedEventHandler"/>.
	/// </summary>
	public class DownloadCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
	{
		private System.IO.Stream _result;

		/// <summary>
		/// Dados do resultado do download.
		/// </summary>
		public System.IO.Stream Result
		{
			get
			{
				return _result;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="error"></param>
		/// <param name="cancelled"></param>
		/// <param name="userState"></param>
		/// <param name="result"></param>
		public DownloadCompletedEventArgs(Exception error, bool cancelled, object userState, System.IO.Stream result) : base(error, cancelled, userState)
		{
			_result = result;
		}
	}
}
