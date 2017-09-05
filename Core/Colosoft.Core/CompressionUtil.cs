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
using System.IO.Compression;
using System.IO;

namespace Colosoft.IO.Compression
{
	/// <summary>
	/// Classe que auxilia na compressão de dados.
	/// </summary>
	public class CompressionUtil
	{
		/// <summary>
		/// Compacta os dados informados.
		/// </summary>
		/// <param name="value">Buffer dos dados que serão compactados.</param>
		/// <param name="flag">Flag que será preenchido durante a compactação.</param>
		/// <returns>Dados compactados.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static byte[] Compress(byte[] value, ref BitSet flag)
		{
			byte[] buffer;
			try
			{
				var stream2 = new MemoryStream();
				using (var stream = new GZipStream(stream2, CompressionMode.Compress, true))
				{
					stream.Write(value, 0, value.Length);
					if(flag == null)
						flag = new BitSet();
					flag.SetBit(2);
					buffer = stream2.ToArray();
				}
			}
			catch(Exception)
			{
				flag.UnsetBit(2);
				buffer = value;
			}
			return buffer;
		}

		/// <summary>
		/// Compacta os dados informados.
		/// </summary>
		/// <param name="value">Buffer com os dados que serão compactados.</param>
		/// <param name="flag"></param>
		/// <param name="threshold">Limite dodo tamanho dos dados que serão compactados.</param>
		/// <returns>Dados compactados.</returns>
		public static byte[] Compress(byte[] value, ref BitSet flag, long threshold)
		{
			if(value == null)
				return value;
			if(flag == null)
				return value;
			if(value.Length <= threshold)
				return value;
			return Compress(value, ref flag);
		}

		/// <summary>
		/// Compacta os dados informados.
		/// </summary>
		/// <param name="value">Buffer dos dados que serão compactados.</param>
		/// <param name="offset">Offset do buffer informado.</param>
		/// <param name="count">Quantidade de dados quer serão lidos do buffer.</param>
		/// <returns>Dados compactados.</returns>
		public static byte[] Compress(byte[] value, int offset, int count)
		{
			MemoryStream stream = null;
			byte[] buffer;
			try
			{
				stream = new MemoryStream();
				buffer = stream.ToArray();
			}
			catch(Exception)
			{
				throw;
			}
			finally
			{
				if(stream != null)
					stream.Dispose();
			}
			return buffer;
		}

		/// <summary>
		/// Descompacta os dados informados.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static byte[] Decompress(byte[] value)
		{
			int num;
			using (var stream = new GZipStream(new MemoryStream(value), CompressionMode.Decompress))
			{
				using (var stream2 = new MemoryStream())
				{
					byte[] buffer = new byte[0x800];
					while ((num = stream.Read(buffer, 0, buffer.Length)) > 0)
						stream2.Write(buffer, 0, num);
					return stream2.ToArray();
				}
			}
		}

		/// <summary>
		/// Descompacta os dados informados.
		/// </summary>
		/// <param name="value">Buffer onde os dados estão registrados.</param>
		/// <param name="flag">Flags que foram usados na compactação.</param>
		/// <returns>Dados descompactados.</returns>
		public static byte[] Decompress(byte[] value, BitSet flag)
		{
			if(value != null)
			{
				if(flag == null)
					return value;
				if(flag.IsBitSet(2))
					return Decompress(value);
			}
			return value;
		}

		/// <summary>
		/// Descompacta os dados informados.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static byte[] Decompress(byte[] value, int offset, int count)
		{
			MemoryStream stream = null;
			byte[] buffer;
			try
			{
				stream = new MemoryStream();
				buffer = stream.ToArray();
			}
			catch(Exception)
			{
				throw;
			}
			finally
			{
				if(stream != null)
					stream.Dispose();
			}
			return buffer;
		}
	}
}
