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

namespace Colosoft.Cryptography
{
	/// <summary>
	/// Classe com método que auxiliam na utilização da criptografia RSA.
	/// </summary>
	public static class RSACryptoHelper
	{
		/// <summary>
		/// Criptografa o texto informado.
		/// </summary>
		/// <param name="inputString">Texto que será criptografado.</param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string EncryptString(string inputString, RSACryptografyKey key)
		{
			key.Require("key").NotNull();
			var xmlString = string.Format("<BitStrength>{0}</BitStrength><RSAKeyValue><Modulus>{1}</Modulus><Exponent>{2}</Exponent></RSAKeyValue>", key.BitStrength, key.RSAKeyValue != null ? key.RSAKeyValue.Modulus : null, key.RSAKeyValue != null ? key.RSAKeyValue.Exponent : null);
			return EncryptString(inputString, key.BitStrength, xmlString);
		}

		/// <summary>
		/// Criptografa o texto informado.
		/// </summary>
		/// <param name="inputString">Texto que será criptografado.</param>
		/// <param name="dwKeySize">Tamanho da chave.</param>
		/// <param name="xmlString">Xml contendo e chave privada para a criptografia.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static string EncryptString(string inputString, int dwKeySize, string xmlString)
		{
			var rsaCryptoServiceProvider = new System.Security.Cryptography.RSACryptoServiceProvider(dwKeySize);
			rsaCryptoServiceProvider.FromXmlString(xmlString);
			int keySize = dwKeySize / 8;
			byte[] bytes = Encoding.UTF32.GetBytes(inputString);
			int maxLength = keySize - 42;
			int dataLength = bytes.Length;
			int iterations = dataLength / maxLength;
			StringBuilder stringBuilder = new StringBuilder();
			for(int i = 0; i <= iterations; i++)
			{
				byte[] tempBytes = new byte[(dataLength - maxLength * i > maxLength) ? maxLength : dataLength - maxLength * i];
				Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0, tempBytes.Length);
				byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);
				Array.Reverse(encryptedBytes);
				stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Descriptografa o texto informado.
		/// </summary>
		/// <param name="inputString">Texto contem os dados criptografados.</param>
		/// <param name="key">Chavem que será usada.</param>
		public static string DecryptString(string inputString, RSACryptografyKey key)
		{
			key.Require("key").NotNull();
			RSACryptografyKey.RSAKey keyValue = key.RSAKeyValue;
			if(keyValue == null)
				keyValue = new RSACryptografyKey.RSAKey();
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(RSACryptografyKey.RSAKey));
			var sb = new StringBuilder();
			using (var writer = new System.IO.StringWriter(sb))
				serializer.Serialize(writer, keyValue);
			var xmlString = sb.ToString();
			return DecryptString(inputString, key.BitStrength, xmlString);
		}

		/// <summary>
		/// Descriptografa o texto informado.
		/// </summary>
		/// <param name="inputString">Texto contem os dados criptografados.</param>
		/// <param name="dwKeySize">Tamanho da chave.</param>
		/// <param name="xmlString">Xml contendo a chave pública.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static string DecryptString(string inputString, int dwKeySize, string xmlString)
		{
			var rsaCryptoServiceProvider = new System.Security.Cryptography.RSACryptoServiceProvider(dwKeySize);
			rsaCryptoServiceProvider.FromXmlString(xmlString);
			int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ? (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
			int iterations = inputString.Length / base64BlockSize;
			var arrayList = new System.Collections.ArrayList();
			for(int i = 0; i < iterations; i++)
			{
				byte[] encryptedBytes = Convert.FromBase64String(inputString.Substring(base64BlockSize * i, base64BlockSize));
				Array.Reverse(encryptedBytes);
				arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
			}
			return Encoding.UTF32.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
		}
	}
}
