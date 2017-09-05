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
	/// Classe usada para criptografar dados.
	/// </summary>
	public class Encryptor
	{
		private byte[] _encKey;

		private byte[] _initVec;

		private EncryptTransformer _transformer;

		/// <summary>
		/// Vetor inicial.
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
		/// Chave privada usada na criptografia.
		/// </summary>
		public byte[] Key
		{
			get
			{
				return this._encKey;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="algorithm"></param>
		public Encryptor(EncryptionAlgorithm algorithm)
		{
			_transformer = new EncryptTransformer(algorithm);
		}

		/// <summary>
		/// Criptografa os dados 
		/// </summary>
		/// <param name="bytesData"></param>
		/// <param name="bytesKey"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public byte[] Encrypt(byte[] bytesData, byte[] bytesKey)
		{
			var stream = new System.IO.MemoryStream();
			_transformer.IV = this._initVec;
			SymmetricAlgorithm algorithm = null;
			var cryptoServiceProvider = this._transformer.GetCryptoServiceProvider(bytesKey, out algorithm);
			var stream2 = new CryptoStream(stream, cryptoServiceProvider, CryptoStreamMode.Write);
			stream2.Write(bytesData, 0, bytesData.Length);
			_encKey = _transformer.Key;
			_initVec = _transformer.IV;
			stream2.FlushFinalBlock();
			stream2.Close();
			return stream.ToArray();
		}

		/// <summary>
		/// Criptografa os dados 
		/// </summary>
		/// <param name="inStream">Stream com os dados que serão criptografados.</param>
		/// <param name="outStream">Stream onde serão salvos os dados criptografados.</param>
		/// <param name="transform">Provedor usado para criptografar os dados</param>
		public void Encrypt(System.IO.Stream inStream, System.IO.Stream outStream, ICryptoTransform transform)
		{
			CryptoStream stream = new CryptoStream(outStream, transform, CryptoStreamMode.Write);
			byte[] buffer = new byte[0x400];
			int count = 0;
			do
			{
				count = inStream.Read(buffer, 0, buffer.Length);
				stream.Write(buffer, 0, count);
			}
			while (count > 0);
			_encKey = _transformer.Key;
			_initVec = _transformer.IV;
			stream.FlushFinalBlock();
		}

		/// <summary>
		/// Criptografa os dados 
		/// </summary>
		/// <param name="inStream">Stream com os dados que serão criptografados.</param>
		/// <param name="outStream">Stream onde serão salvos os dados criptografados.</param>
		/// <param name="bytesKey">chave.</param>
		public void Encrypt(System.IO.Stream inStream, System.IO.Stream outStream, byte[] bytesKey)
		{
			SymmetricAlgorithm algoritm = null;
			this.Encrypt(inStream, outStream, this.GetCryptoProvider(bytesKey, out algoritm));
		}

		/// <summary>
		/// Recupera o provedor para a criptografia.
		/// </summary>
		/// <param name="bytesKey"></param>
		/// <param name="algoritm"></param>
		/// <returns></returns>
		public ICryptoTransform GetCryptoProvider(byte[] bytesKey, out SymmetricAlgorithm algoritm)
		{
			return _transformer.GetCryptoServiceProvider(bytesKey, out algoritm);
		}
	}
}
