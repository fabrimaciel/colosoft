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

namespace Colosoft.Net
{
	/// <summary>
	/// Agregador dos observadores do Uploader.
	/// </summary>
	public class AggregateUploaderObserver : AggregateObserver<IUploaderObserver>
	{
		/// <summary>
		/// Acionado quando o trabalho de atualização é iniciado.
		/// </summary>
		public void OnDoWork(object userState)
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnDoWork(userState);
		}

		/// <summary>
		/// Define o progresso do download de dados.
		/// </summary>
		/// <param name="e"></param>
		public void OnProgressChanged(Colosoft.Net.UploadProgressChangedEventArgs e)
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnProgressChanged(e);
		}

		/// <summary>
		/// Acionado quando o download é finalizado.
		/// </summary>
		/// <param name="e"></param>
		public void OnCompleted(Colosoft.Net.UploadCompletedEventArgs e)
		{
			lock (Observers)
				foreach (var i in Observers)
					i.OnCompleted(e);
		}
	}
}
