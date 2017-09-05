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
using System.IO;
using Colosoft.Serialization.Formatters;

namespace Colosoft.Caching.Storage.Util
{
	/// <summary>
	/// Representa um armazenamento para um sistema de arquivo.
	/// </summary>
	internal class FileSystemStorage : IDisposable
	{
		private string _dataFolder;

		private string _rootDir;

		/// <summary>
		/// Nome do diretório de dados.
		/// </summary>
		public string DataFolder
		{
			get
			{
				return _dataFolder;
			}
		}

		/// <summary>
		/// Nome do diretório principal.
		/// </summary>
		public string RootDir
		{
			get
			{
				return _rootDir;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public FileSystemStorage() : this(null, null)
		{
		}

		/// <summary>
		/// Cria a instancia já informando os parametros de configurção.
		/// </summary>
		/// <param name="rootDir">Caminho do diretório principal.</param>
		/// <param name="dataFolder">Caminho do diretório de dados.</param>
		public FileSystemStorage(string rootDir, string dataFolder)
		{
			if((rootDir == null) || (rootDir.Length < 1))
				rootDir = Path.GetTempPath();
			_rootDir = rootDir;
			_dataFolder = dataFolder;
			if(_dataFolder == null)
				_rootDir = Path.Combine(_rootDir, this.GetRandomFileName());
			else
			{
				if(Path.IsPathRooted(_dataFolder))
					_dataFolder = Path.GetDirectoryName(_dataFolder);
				_rootDir = Path.Combine(_rootDir, _dataFolder);
			}
			Directory.CreateDirectory(_rootDir);
			_rootDir = _rootDir + @"\";
		}

		/// <summary>
		/// Escreve o objeto para o arquivo.
		/// </summary>
		/// <param name="fileName">Nome do arquivo.</param>
		/// <param name="value">Instancia que será escrita.</param>
		/// <param name="serializationContext">Nome do contexto de serialização.</param>
		private void WriteObjectToFile(string fileName, object value, string serializationContext)
		{
			fileName = this.GetPathForFile(fileName);
			using (FileStream stream = new FileStream(fileName, FileMode.Create))
				CompactBinaryFormatter.Serialize(stream, value, serializationContext);
		}

		/// <summary>
		/// Lê o objeto do arquivo.
		/// </summary>
		/// <param name="fileName">Nome do arquivo.</param>
		/// <param name="serializationContext">Nome do contexto de serialização.</param>
		/// <returns></returns>
		private object ReadObjectFromFile(string fileName, string serializationContext)
		{
			fileName = this.GetPathForFile(fileName);
			if(!File.Exists(fileName))
				return null;
			using (FileStream stream = new FileStream(fileName, FileMode.Open))
				return CompactBinaryFormatter.Deserialize(stream, serializationContext);
		}

		/// <summary>
		/// Recupera nome unico para o arquivo.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private string GetUniqueNameForFile(object key)
		{
			return Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Recupera o caminho para o nome do arquivo.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private string GetPathForFile(string fileName)
		{
			fileName = _rootDir + fileName + ".fso";
			return fileName;
		}

		/// <summary>
		/// Adiciona uma instancia para o armazenamento.
		/// </summary>
		/// <param name="key">Chave que representa o valor que está sendo adicionado.</param>
		/// <param name="value">Valor que será adicionado.</param>
		/// <param name="serializationContext">Nome do contexto de serialização.</param>
		/// <returns></returns>
		public string Add(object key, object value, string serializationContext)
		{
			string uniqueNameForFile = this.GetUniqueNameForFile(key);
			this.WriteObjectToFile(uniqueNameForFile, value, serializationContext);
			return uniqueNameForFile;
		}

		/// <summary>
		/// Insere uma instancia para o armazenamento.
		/// </summary>
		/// <param name="key">Chave que representa o valor que está sendo adicionado.</param>
		/// <param name="value">Valor que será adicionado.</param>
		/// <param name="serializationContext">Nome do contexto de serialização.</param>
		/// <returns></returns>
		public string Insert(object key, object value, string serializationContext)
		{
			string fileName = null;
			if(key == null)
				fileName = this.Add(key, value, serializationContext);
			else
			{
				fileName = key.ToString();
				this.WriteObjectToFile(fileName, value, serializationContext);
			}
			return fileName;
		}

		/// <summary>
		/// Verifica se existe algum item com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Contains(object key)
		{
			if(key == null)
				return false;
			try
			{
				return File.Exists(key.ToString());
			}
			catch(Exception)
			{
				return false;
			}
		}

		public void Remove(object Key)
		{
			string pathForFile = this.GetPathForFile(Key.ToString());
			if(File.Exists(pathForFile))
				File.Delete(pathForFile);
		}

		/// <summary>
		/// Limpa os dados do armazenamento.
		/// </summary>
		public void Clear()
		{
			if(Directory.Exists(_rootDir))
			{
				Directory.Delete(_rootDir, true);
				Directory.CreateDirectory(_rootDir);
			}
		}

		/// <summary>
		/// Recupera o objeto pela chave informada.
		/// </summary>
		/// <param name="key">Chave que será usada na recuperação.</param>
		/// <param name="serializationContext"></param>
		/// <returns></returns>
		public object Get(object key, string serializationContext)
		{
			if(key == null)
				return null;
			return this.ReadObjectFromFile(key.ToString(), serializationContext);
		}

		/// <summary>
		/// Recupera um nome de arquivo aleatório.
		/// </summary>
		/// <returns></returns>
		public string GetRandomFileName()
		{
			return (Guid.NewGuid().ToString() + ".dir");
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			if(_dataFolder == null && Directory.Exists(_rootDir))
				Directory.Delete(_rootDir, true);
		}
	}
}
