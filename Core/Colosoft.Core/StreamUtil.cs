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

namespace Colosoft.IO
{
	/// <summary>
	/// Representa o callback para a escrite dos bytes.
	/// </summary>
	/// <param name="bytesWritten"></param>
	public delegate void CopyStreamCallback (long bytesWritten);
	/// <summary>
	/// Classe com métodos auxiliares para stream.
	/// </summary>
	public static class StreamUtil
	{
		/// <summary>
		/// Copia a stream da origem para o destino usando o buffer informado.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="dest"></param>
		/// <param name="buffer"></param>
		/// <param name="progressCallback"></param>
		/// <returns></returns>
		public static long CopyStream(System.IO.Stream source, System.IO.Stream dest, byte[] buffer, CopyStreamCallback progressCallback)
		{
			System.Diagnostics.Stopwatch stopwatch = null;
			int num2;
			if(progressCallback != null)
			{
				stopwatch = new System.Diagnostics.Stopwatch();
				stopwatch.Start();
			}
			long bytesWritten = 0;
			long elapsedMilliseconds = 0;
			while ((num2 = source.Read(buffer, 0, buffer.Length)) > 0)
			{
				dest.Write(buffer, 0, num2);
				bytesWritten += num2;
				if((progressCallback != null) && (stopwatch.ElapsedMilliseconds > (elapsedMilliseconds + 0x3e8)))
				{
					progressCallback(bytesWritten);
					elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				}
			}
			if((progressCallback != null) && (elapsedMilliseconds != 0))
				progressCallback(bytesWritten);
			return bytesWritten;
		}
	}
}
