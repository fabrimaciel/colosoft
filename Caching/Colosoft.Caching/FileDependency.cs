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
using Colosoft.Serialization.IO;

namespace Colosoft.Caching.Expiration
{
	/// <summary>
	/// Representa uma dica de expiração para a dependecia de um arquivo.
	/// </summary>
	[Serializable]
	public class FileDependency : DependencyHint
	{
		private bool[] _fileExists;

		private string[] _fileName;

		private bool[] _isDir;

		private DateTime[] _lastWriteTime;

		private long _startAfterTicks;

		/// <summary>
		/// Nomes dos arquivos associados.
		/// </summary>
		public string[] FileNames
		{
			get
			{
				return _fileName;
			}
		}

		/// <summary>
		/// Identifica se a instancia foi alterada.
		/// </summary>
		public override bool HasChanged
		{
			get
			{
				return FileExpired();
			}
		}

		/// <summary>
		/// Ticks.
		/// </summary>
		internal long StartAfterTicks
		{
			get
			{
				return _startAfterTicks;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public FileDependency()
		{
			base.HintType = ExpirationHintType.FileDependency;
		}

		/// <summary>
		/// Cria uma instancia já informado o nome do arquivo será monitorado.
		/// </summary>
		/// <param name="fileName">Nome do arquivo que será monitorado.</param>
		public FileDependency(string fileName) : this(fileName, DateTime.Now)
		{
			base.HintType = ExpirationHintType.FileDependency;
		}

		/// <summary>
		/// Cria uma instancia já informando os arquivos que serão monitorados.
		/// </summary>
		/// <param name="fileName"></param>
		public FileDependency(string[] fileName) : this(fileName, DateTime.Now)
		{
			base.HintType = ExpirationHintType.FileDependency;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="startAfter"></param>
		public FileDependency(string fileName, DateTime startAfter) : this(new string[] {
			fileName
		}, startAfter)
		{
			base.HintType = ExpirationHintType.FileDependency;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="startAfter"></param>
		public FileDependency(string[] fileName, DateTime startAfter) : base(startAfter)
		{
			base.HintType = ExpirationHintType.FileDependency;
			_fileName = fileName;
			_startAfterTicks = startAfter.Ticks;
		}

		/// <summary>
		/// Verifica se o arquivo expirou.
		/// </summary>
		/// <returns></returns>
		private bool FileExpired()
		{
			int length = _fileName.Length;
			bool flag = false;
			bool exists = false;
			for(int i = 0; i < length; i++)
			{
				DateTime lastWriteTime;
				var info = new System.IO.FileInfo(_fileName[i]);
				flag = (info.Attributes & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory;
				if(flag)
				{
					var info2 = new System.IO.DirectoryInfo(_fileName[i]);
					exists = info2.Exists;
					lastWriteTime = info2.LastWriteTime;
				}
				else
				{
					exists = info.Exists;
					lastWriteTime = info.LastWriteTime;
				}
				if(_fileExists[i])
				{
					if(!exists)
						return true;
					if(flag != _isDir[i])
						return true;
					if(lastWriteTime != _lastWriteTime[i])
						return true;
				}
				else if(exists)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="fileName">Nomes dos arquivos associados com a instancia.</param>
		/// <returns></returns>
		private bool Initialize(string[] fileName)
		{
			int length = fileName.Length;
			_fileExists = new bool[length];
			_isDir = new bool[length];
			_lastWriteTime = new DateTime[length];
			_fileName = new string[length];
			for(int i = 0; i < length; i++)
			{
				var info = new System.IO.FileInfo(fileName[i]);
				_fileName[i] = fileName[i];
				_isDir[i] = (info.Attributes & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory;
				if(_isDir[i])
				{
					var info2 = new System.IO.DirectoryInfo(fileName[i]);
					_fileExists[i] = info2.Exists;
					_lastWriteTime[i] = info2.LastWriteTime;
				}
				else
				{
					_fileExists[i] = info.Exists;
					_lastWriteTime[i] = info.LastWriteTime;
				}
			}
			return true;
		}

		/// <summary>
		/// Reseta a instancia no contexto informado.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal override bool Reset(CacheRuntimeContext context)
		{
			base.Reset(context);
			return this.Initialize(this.FileNames);
		}

		/// <summary>
		/// Deserializa os dados para a instancia.
		/// </summary>
		/// <param name="reader"></param>
		public override void Deserialize(CompactReader reader)
		{
			base.Deserialize(reader);
			_fileExists = (bool[])reader.ReadObject();
			_isDir = (bool[])reader.ReadObject();
			_fileName = (string[])reader.ReadObject();
			_lastWriteTime = (DateTime[])reader.ReadObject();
			_startAfterTicks = reader.ReadInt64();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public override void Serialize(CompactWriter writer)
		{
			base.Serialize(writer);
			writer.WriteObject(_fileExists);
			writer.WriteObject(_isDir);
			writer.WriteObject(_fileName);
			writer.WriteObject(_lastWriteTime);
			writer.Write(_startAfterTicks);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = "FILEDEPENDENCY \"";
			for(int i = 0; i < _fileName.Length; i++)
				str = str + _fileName[i] + "\"";
			object obj2 = str;
			return string.Concat(new object[] {
				obj2,
				"STARTAFTER\"",
				_startAfterTicks,
				"\"\r\n"
			});
		}
	}
}
