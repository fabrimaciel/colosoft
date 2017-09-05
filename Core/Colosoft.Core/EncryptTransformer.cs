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
	/// Transformador para criptografia.
	/// </summary>
	class EncryptTransformer
	{
		private EncryptionAlgorithm _algorithm;

		private byte[] _encKey;

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
		/// Chave privada usada na criptografia.
		/// </summary>
		public byte[] Key
		{
			get
			{
				return _encKey;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="algorithm"></param>
		public EncryptTransformer(EncryptionAlgorithm algorithm)
		{
			_algorithm = algorithm;
		}

		/// <summary>
		/// Recupera o provedor do serviço de criptografia.
		/// </summary>
		/// <param name="bytesKey">Vetor com a chave que será usada pelo algoritmo.</param>
		/// <param name="algorithm">Algoritmo simetrico.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public System.Security.Cryptography.ICryptoTransform GetCryptoServiceProvider(byte[] bytesKey, out System.Security.Cryptography.SymmetricAlgorithm algorithm)
		{
			System.Security.Cryptography.DES des;
			switch(this._algorithm)
			{
			case EncryptionAlgorithm.Des:
				des = new DESCryptoServiceProvider();
				des.Mode = CipherMode.CBC;
				if(bytesKey != null)
				{
					des.Key = bytesKey;
					_encKey = des.Key;
					break;
				}
				_encKey = des.Key;
				break;
			case EncryptionAlgorithm.Rc2:
				RC2 rc = new RC2CryptoServiceProvider();
				rc.Mode = CipherMode.CBC;
				if(bytesKey != null)
				{
					rc.Key = bytesKey;
					_encKey = rc.Key;
				}
				else
					_encKey = rc.Key;
				if(_initVec == null)
					_initVec = rc.IV;
				else
					rc.IV = _initVec;
				algorithm = rc;
				return rc.CreateEncryptor();
			case EncryptionAlgorithm.Rijndael:
			{
				Rijndael rijndael = new RijndaelManaged();
				rijndael.Mode = System.Security.Cryptography.CipherMode.CBC;
				if(bytesKey != null)
				{
					rijndael.Key = bytesKey;
					_encKey = rijndael.Key;
				}
				else
					_encKey = rijndael.Key;
				if(_initVec == null)
					_initVec = rijndael.IV;
				else
					rijndael.IV = _initVec;
				algorithm = rijndael;
				return rijndael.CreateEncryptor();
			}
			case EncryptionAlgorithm.TripleDes:
			{
				TripleDES edes = new TripleDESCryptoServiceProvider();
				edes.Mode = CipherMode.CBC;
				if(bytesKey != null)
				{
					edes.Key = bytesKey;
					_encKey = edes.Key;
				}
				else
					_encKey = edes.Key;
				if(_initVec == null)
					_initVec = edes.IV;
				else
					edes.IV = _initVec;
				algorithm = edes;
				return edes.CreateEncryptor();
			}
			default:
				throw new CryptographicException("Algorithm '" + _algorithm + "' not supported.");
			}
			if(_initVec == null)
				_initVec = des.IV;
			else
				des.IV = _initVec;
			algorithm = des;
			return des.CreateEncryptor();
		}
	}
}
