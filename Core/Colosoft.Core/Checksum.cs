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

namespace Colosoft.Util
{
	/// <summary>
	/// Checksum
	/// </summary>
	public class Checksum
	{
		private static bool _md5ProviderDisabled;

		/// <summary>
		/// Recupera o provedor MD5.
		/// </summary>
		/// <returns></returns>
		private static System.Security.Cryptography.MD5 GetMD5Provider()
		{
			System.Security.Cryptography.MD5 md = null;
			if(!_md5ProviderDisabled)
			{
				try
				{
					md = new System.Security.Cryptography.MD5CryptoServiceProvider();
				}
				catch(InvalidOperationException)
				{
					_md5ProviderDisabled = true;
				}
			}
			return md;
		}

		/// <summary>
		/// Calcula o Hash MD5 da stream informada.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static byte[] CalculateMD5(System.IO.Stream stream)
		{
			System.Security.Cryptography.MD5 md = GetMD5Provider();
			if(md == null)
				return new byte[0];
			using (md)
				return md.ComputeHash(stream);
		}

		/// <summary>
		/// Calcula o Hash
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset">Offset que será usado no buffer.</param>
		/// <param name="count">Quantidade de bytes que serão lidos do buffer.</param>
		/// <returns></returns>
		public static byte[] CalculateMD5(byte[] buffer, int offset, int count)
		{
			System.Security.Cryptography.MD5 md = GetMD5Provider();
			if(md == null)
				return new byte[0];
			using (md)
				return md.ComputeHash(buffer, offset, count);
		}

		/// <summary>
		/// Calcula o Hash MD5 do arquivo informado.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static byte[] CalculateMD5(string fileName)
		{
			if(_md5ProviderDisabled)
			{
				return new byte[0];
			}
			using (var stream = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
				return CalculateMD5(stream);
		}
	}
}
