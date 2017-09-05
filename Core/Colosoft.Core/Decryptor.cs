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
	/// Classe responsável pela descriptografia.
	/// </summary>
	/// <example>
	/// try 
	/// { 
	///     Decryptor dec = new Decryptor(algorithm); 
	///     dec.IV = IV; 
	///     byte[] plainText = dec.Decrypt(cipherText, key); 
	///     Console.WriteLine(" Plain text: " + Encoding.ASCII.GetString(plainText)); 
	/// } 
	/// catch(Exception ex) 
	/// { 
	///     Console.WriteLine("Exception decrypting. " + ex.Message); 
	///     return; 
	/// } 
	/// </example>
	public class Decryptor
	{
		private byte[] _initVec;

		private DecryptTransformer _transformer;

		/// <summary>
		/// Vetor inicia.
		/// </summary>
		public byte[] IV
		{
			get
			{
				return _initVec;
			}
			set
			{
				_initVec = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="algorithm">Algoritmo de criptografia que sera utilizado</param>
		public Decryptor(EncryptionAlgorithm algorithm)
		{
			_transformer = new DecryptTransformer(algorithm);
		}

		/// <summary>
		/// Descriptografa os dados no vetor informado usando a chave fornecida.
		/// </summary>
		/// <param name="bytesData">Vetor dos dados que serão processados.</param>
		/// <param name="bytesKey">Chave que será usada.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public byte[] Decrypt(byte[] bytesData, byte[] bytesKey)
		{
			var stream = new System.IO.MemoryStream();
			_transformer.IV = _initVec;
			var cryptoServiceProvider = _transformer.GetCryptoServiceProvider(bytesKey);
			var stream2 = new System.Security.Cryptography.CryptoStream(stream, cryptoServiceProvider, System.Security.Cryptography.CryptoStreamMode.Write);
			stream2.Write(bytesData, 0, bytesData.Length);
			stream2.FlushFinalBlock();
			stream2.Close();
			return stream.ToArray();
		}

		/// <summary>
		/// Descriptografa os dados da stream de entrada e escreve na stream de saída.
		/// </summary>
		/// <param name="inStream">Stream de entrada.</param>
		/// <param name="outStream">Stream de saída.</param>
		/// <param name="bytesKey"></param>
		public void Decrypt(System.IO.Stream inStream, System.IO.Stream outStream, byte[] bytesKey)
		{
			_transformer.IV = _initVec;
			var cryptoServiceProvider = this._transformer.GetCryptoServiceProvider(bytesKey);
			var stream = new System.Security.Cryptography.CryptoStream(outStream, cryptoServiceProvider, System.Security.Cryptography.CryptoStreamMode.Write);
			byte[] buffer = new byte[0x400];
			int count = 0;
			do
			{
				count = inStream.Read(buffer, 0, buffer.Length);
				stream.Write(buffer, 0, count);
			}
			while (count > 0);
			stream.FlushFinalBlock();
		}
	}
}
