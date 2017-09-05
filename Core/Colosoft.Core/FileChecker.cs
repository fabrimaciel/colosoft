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
	/// Classe usadao para realizar as verificações dos arquivos.
	/// </summary>
	public static class FileChecker
	{
		/// <summary>
		/// Verifica a assinatura do arquivo.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="signatureSize"></param>
		/// <param name="expectedSignature"></param>
		/// <returns></returns>
		public static bool CheckSignature(string filePath, int signatureSize, string expectedSignature)
		{
			if(String.IsNullOrEmpty(filePath))
				throw new ArgumentException("Must specify a filepath");
			if(String.IsNullOrEmpty(expectedSignature))
				throw new ArgumentException("Must specify a value for the expected file signature");
			using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
			{
				if(fs.Length < signatureSize)
					return false;
				byte[] signature = new byte[signatureSize];
				int bytesRequired = signatureSize;
				int index = 0;
				while (bytesRequired > 0)
				{
					int bytesRead = fs.Read(signature, index, bytesRequired);
					bytesRequired -= bytesRead;
					index += bytesRead;
				}
				string actualSignature = BitConverter.ToString(signature);
				if(actualSignature == expectedSignature)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Verifica se o arquivo informado é um zip.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static bool IsPKZip(string filePath)
		{
			return CheckSignature(filePath, 4, "50-4B-03-04");
		}

		/// <summary>
		/// Verifica se o arquivo informado é um GZip.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static bool IsGZip(string filePath)
		{
			return CheckSignature(filePath, 3, "1F-8B-08");
		}
	}
}
