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
using System.Security.Cryptography;

namespace Colosoft.Cryptography
{
	/// <summary>
	/// Utilitário para critografia.
	/// </summary>
	public class EncryptionUtil
	{
		private static DESCryptoServiceProvider s_des;

		private static string s_iv = "%1Az=-qT";

		private static string s_key = "ahRA6q96";

		/// <summary>
		/// Converte o texto para um vetor de bytes.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static byte[] ConvertStringToByteArray(string s)
		{
			return new ASCIIEncoding().GetBytes(s);
		}

		/// <summary>
		/// Descriptogra o buffer informado para um texto.
		/// </summary>
		/// <param name="cypherText"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		public static string Decrypt(byte[] cypherText)
		{
			try
			{
				s_des = new DESCryptoServiceProvider();
				s_des.Key = ConvertStringToByteArray(s_key);
				s_des.IV = ConvertStringToByteArray(s_iv);
				using (var stream = new System.IO.MemoryStream(cypherText))
				{
					var stream2 = new CryptoStream(stream, s_des.CreateDecryptor(), CryptoStreamMode.Read);
					var reader = new System.IO.StreamReader(stream2);
					string str = reader.ReadLine();
					reader.Close();
					stream2.Close();
					return str;
				}
			}
			catch(Exception)
			{
			}
			return null;
		}

		/// <summary>
		/// Criptogra o texto informado em um vetor de bytes.
		/// </summary>
		/// <param name="plainText"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		public static byte[] Encrypt(string plainText)
		{
			try
			{
				s_des = new DESCryptoServiceProvider();
				int blockSize = s_des.BlockSize;
				int keySize = s_des.KeySize;
				s_des.Key = ConvertStringToByteArray(s_key);
				s_des.IV = ConvertStringToByteArray(s_iv);
				using (var stream = new System.IO.MemoryStream())
				{
					var stream2 = new CryptoStream(stream, s_des.CreateEncryptor(), CryptoStreamMode.Write);
					var writer = new System.IO.StreamWriter(stream2);
					writer.WriteLine(plainText);
					writer.Close();
					stream2.Close();
					byte[] buffer = stream.ToArray();
					return buffer;
				}
			}
			catch(Exception)
			{
			}
			return null;
		}
	}
}
