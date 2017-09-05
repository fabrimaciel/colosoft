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
	/// Transformador da descriptografia.
	/// </summary>
	class DecryptTransformer
	{
		private EncryptionAlgorithm _algorithm;

		private byte[] _initVec;

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
		/// Construtor padrão.
		/// </summary>
		/// <param name="algorithm"></param>
		public DecryptTransformer(EncryptionAlgorithm algorithm)
		{
			_algorithm = algorithm;
		}

		/// <summary>
		/// Recupera o provedor do serviço de criptografia.
		/// </summary>
		/// <param name="bytesKey"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public ICryptoTransform GetCryptoServiceProvider(byte[] bytesKey)
		{
			switch(_algorithm)
			{
			case EncryptionAlgorithm.Des:
				DES des = new DESCryptoServiceProvider();
				des.Mode = CipherMode.CBC;
				des.Key = bytesKey;
				des.IV = this._initVec;
				return des.CreateDecryptor();
			case EncryptionAlgorithm.Rc2:
				RC2 rc = new RC2CryptoServiceProvider();
				rc.Mode = CipherMode.CBC;
				return rc.CreateDecryptor(bytesKey, _initVec);
			case EncryptionAlgorithm.Rijndael:
				Rijndael rijndael = new RijndaelManaged();
				rijndael.Mode = CipherMode.CBC;
				return rijndael.CreateDecryptor(bytesKey, _initVec);
			case EncryptionAlgorithm.TripleDes:
				TripleDES edes = new TripleDESCryptoServiceProvider();
				edes.Mode = CipherMode.CBC;
				return edes.CreateDecryptor(bytesKey, _initVec);
			}
			throw new CryptographicException("Algorithm '" + _algorithm + "' not supported.");
		}
	}
}
