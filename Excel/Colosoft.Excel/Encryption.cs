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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;

namespace Colosoft.Excel.ExcelXml.Extensions
{
	/// <summary>
	/// Encryption and decryption methods based on a password key.
	/// </summary>
	public static class EncryptDecrypt
	{
		/// <summary>
		/// Encrypt a byte array into a byte array using a key and an IV
		/// </summary>
		/// <param name="clearData">Byte array to encrypt</param>
		/// <param name="key">Key</param>
		/// <param name="IV">IV</param>
		/// <returns>Encrypted byte array</returns>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IV")]
		public static byte[] Encrypt(byte[] clearData, byte[] key, byte[] IV)
		{
			MemoryStream ms = new MemoryStream();
			Rijndael alg = Rijndael.Create();
			alg.Key = key;
			alg.IV = IV;
			CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
			cs.Write(clearData, 0, clearData.Length);
			cs.Close();
			byte[] encryptedData = ms.ToArray();
			return encryptedData;
		}

		/// <summary>
		/// Encrypt a string into a string using a password
		/// </summary>
		/// <param name="clearText">String to encrypt</param>
		/// <param name="password">Password</param>
		/// <returns>Encrypted string</returns>
		public static string Encrypt(string clearText, string password)
		{
			byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] {
				0x49,
				0x76,
				0x61,
				0x6e,
				0x20,
				0x4d,
				0x65,
				0x64,
				0x76,
				0x65,
				0x64,
				0x65,
				0x76
			});
			byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
			return Convert.ToBase64String(encryptedData);
		}

		/// <summary>
		/// Encrypt bytes into bytes using a password
		/// </summary>
		/// <param name="clearData">Byte array to encrypt</param>
		/// <param name="password">Password</param>
		/// <returns>Encrypted byte array</returns>
		public static byte[] Encrypt(byte[] clearData, string password)
		{
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] {
				0x49,
				0x76,
				0x61,
				0x6e,
				0x20,
				0x4d,
				0x65,
				0x64,
				0x76,
				0x65,
				0x64,
				0x65,
				0x76
			});
			return Encrypt(clearData, pdb.GetBytes(32), pdb.GetBytes(16));
		}

		/// <summary>
		/// Encrypt a file into another file using a password
		/// </summary>
		/// <param name="fileIn">File to be encrypted</param>
		/// <param name="fileOut">Filename where encrypted data will be saved</param>
		/// <param name="password">Password</param>
		public static void Encrypt(string fileIn, string fileOut, string password)
		{
			FileStream fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
			FileStream fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write);
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] {
				0x49,
				0x76,
				0x61,
				0x6e,
				0x20,
				0x4d,
				0x65,
				0x64,
				0x76,
				0x65,
				0x64,
				0x65,
				0x76
			});
			Rijndael alg = Rijndael.Create();
			alg.Key = pdb.GetBytes(32);
			alg.IV = pdb.GetBytes(16);
			CryptoStream cs = new CryptoStream(fsOut, alg.CreateEncryptor(), CryptoStreamMode.Write);
			int bufferLen = 4096;
			byte[] buffer = new byte[bufferLen];
			int bytesRead;
			do
			{
				bytesRead = fsIn.Read(buffer, 0, bufferLen);
				cs.Write(buffer, 0, bytesRead);
			}
			while (bytesRead != 0);
			cs.Close();
			fsIn.Close();
		}

		/// <summary>
		/// Decrypt a byte array into a byte array using a key and an IV
		/// </summary>
		/// <param name="cipherData">Byte array to decrypt</param>
		/// <param name="key">Password Key</param>
		/// <param name="IV">IV</param>
		/// <returns>Returns decrypted byte array</returns>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IV")]
		public static byte[] Decrypt(byte[] cipherData, byte[] key, byte[] IV)
		{
			MemoryStream ms = new MemoryStream();
			Rijndael alg = Rijndael.Create();
			alg.Key = key;
			alg.IV = IV;
			CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
			cs.Write(cipherData, 0, cipherData.Length);
			cs.Close();
			byte[] decryptedData = ms.ToArray();
			return decryptedData;
		}

		/// <summary>
		/// Decrypt a string into a string using a password
		/// </summary>
		/// <param name="cipherText">String to decrypt</param>
		/// <param name="password">password</param>
		/// <returns>Decrypted string</returns>
		public static string Decrypt(string cipherText, string password)
		{
			byte[] cipherBytes = Convert.FromBase64String(cipherText);
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] {
				0x49,
				0x76,
				0x61,
				0x6e,
				0x20,
				0x4d,
				0x65,
				0x64,
				0x76,
				0x65,
				0x64,
				0x65,
				0x76
			});
			byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
			return System.Text.Encoding.Unicode.GetString(decryptedData);
		}

		/// <summary>
		/// Decrypt bytes into bytes using a password
		/// </summary>
		/// <param name="cipherData">Byte array to decrypt</param>
		/// <param name="password">password</param>
		/// <returns>Returns decrypted byte array</returns>
		public static byte[] Decrypt(byte[] cipherData, string password)
		{
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] {
				0x49,
				0x76,
				0x61,
				0x6e,
				0x20,
				0x4d,
				0x65,
				0x64,
				0x76,
				0x65,
				0x64,
				0x65,
				0x76
			});
			return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16));
		}

		/// <summary>
		/// Decrypt a file into another file using a password
		/// </summary>
		/// <param name="fileIn">File to decrypt</param>
		/// <param name="fileOut">Filename where decrypted data will be saved</param>
		/// <param name="password">password</param>
		public static void Decrypt(string fileIn, string fileOut, string password)
		{
			FileStream fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
			FileStream fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write);
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] {
				0x49,
				0x76,
				0x61,
				0x6e,
				0x20,
				0x4d,
				0x65,
				0x64,
				0x76,
				0x65,
				0x64,
				0x65,
				0x76
			});
			Rijndael alg = Rijndael.Create();
			alg.Key = pdb.GetBytes(32);
			alg.IV = pdb.GetBytes(16);
			CryptoStream cs = new CryptoStream(fsOut, alg.CreateDecryptor(), CryptoStreamMode.Write);
			int bufferLen = 4096;
			byte[] buffer = new byte[bufferLen];
			int bytesRead;
			do
			{
				bytesRead = fsIn.Read(buffer, 0, bufferLen);
				cs.Write(buffer, 0, bytesRead);
			}
			while (bytesRead != 0);
			cs.Close();
			fsIn.Close();
		}
	}
}
