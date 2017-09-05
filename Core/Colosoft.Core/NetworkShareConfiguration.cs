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

namespace Colosoft.Net.Share
{
	/// <summary>
	/// Armazena as configurações do compartilhamento de rede.
	/// </summary>
	public class NetworkShareConfiguration
	{
		[NonSerialized]
		private Colosoft.Cryptography.RSACryptografyKey _privateKey;

		private DirectoryShareInfo[] _directories;

		/// <summary>
		/// Diretório de compartilhamento.
		/// </summary>
		public DirectoryShareInfo[] Directories
		{
			get
			{
				return _directories ?? new DirectoryShareInfo[0];
			}
			set
			{
				_directories = value;
			}
		}

		/// <summary>
		/// Chave usada 
		/// </summary>
		protected Colosoft.Cryptography.RSACryptografyKey PrivateKey
		{
			get
			{
				if(_privateKey == null)
					_privateKey = GetDefaultPrivateKey();
				return _privateKey;
			}
		}

		/// <summary>
		/// Decriptografa o texto com a chave da configuração.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private string Decrypt(string text)
		{
			if(!string.IsNullOrEmpty(text))
				return Colosoft.Cryptography.RSACryptoHelper.DecryptString(text.Trim(' ', '\r', '\n'), PrivateKey);
			return text;
		}

		/// <summary>
		/// Recupera a chave de criptografia padrão.
		/// </summary>
		/// <returns></returns>
		private static Colosoft.Cryptography.RSACryptografyKey GetDefaultPrivateKey()
		{
			string xmlString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><RSACryptografyKey>";
			using (var stream = typeof(NetworkShareConfiguration).Assembly.GetManifestResourceStream("Colosoft.Net.Share.PrivateKey.kez"))
			{
				var stringReader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8);
				xmlString += stringReader.ReadToEnd() + "</RSACryptografyKey>";
			}
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Colosoft.Cryptography.RSACryptografyKey));
			using (var reader = new System.IO.StringReader(xmlString))
				return (Colosoft.Cryptography.RSACryptografyKey)serializer.Deserialize(reader);
		}

		/// <summary>
		/// Configura a chave de criptografia privata que será usada para descriptografar os dados
		/// da configuração.
		/// </summary>
		/// <param name="privateKey"></param>
		public void ConfigurePrivateKey(Colosoft.Cryptography.RSACryptografyKey privateKey)
		{
			_privateKey = privateKey;
		}

		/// <summary>
		/// Carrega os dados da configuração.
		/// </summary>
		/// <param name="inputStream"></param>
		/// <returns></returns>
		public static NetworkShareConfiguration Open(System.IO.Stream inputStream)
		{
			inputStream.Require("inputStream").NotNull();
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(NetworkShareConfiguration));
			var configuration = (NetworkShareConfiguration)serializer.Deserialize(inputStream);
			return configuration;
		}

		/// <summary>
		/// Carrega o arquivo conténd as configurações.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static NetworkShareConfiguration Open(string fileName)
		{
			fileName.Require("fileName").NotEmpty().NotNull();
			using (var fs = System.IO.File.OpenRead(fileName))
				return Open(fs);
		}

		/// <summary>
		/// Recupera os diretórios de compartilhamento da configuração.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<DirectoryShare> GetDirectories()
		{
			foreach (var i in Directories)
				yield return new DirectoryShare(i.Directory, new NetworkShareIdentity(i.Domain, i.Username, i.UseCryptografy ? Decrypt(i.Password) : i.Password));
		}

		/// <summary>
		/// Armazena as informações do compartilhamento.
		/// </summary>
		[System.Xml.Serialization.XmlType("DirectoryShare")]
		public class DirectoryShareInfo
		{
			/// <summary>
			/// Nome do diretório de compartilhamento.
			/// </summary>
			[System.Xml.Serialization.XmlAttribute("directory")]
			public string Directory
			{
				get;
				set;
			}

			/// <summary>
			/// Domínio.
			/// </summary>
			[System.Xml.Serialization.XmlAttribute("domain")]
			public string Domain
			{
				get;
				set;
			}

			/// <summary>
			/// Usuário de acesso.
			/// </summary>
			[System.Xml.Serialization.XmlAttribute("username")]
			public string Username
			{
				get;
				set;
			}

			/// <summary>
			/// Identifica se é para usar criptografia na configuração.
			/// </summary>
			[System.Xml.Serialization.XmlAttribute("cryptografy")]
			public bool UseCryptografy
			{
				get;
				set;
			}

			/// <summary>
			/// Senha de acesso.
			/// </summary>
			public string Password
			{
				get;
				set;
			}
		}
	}
}
