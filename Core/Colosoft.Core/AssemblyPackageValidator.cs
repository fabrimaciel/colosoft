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

namespace Colosoft.Reflection.Local
{
	/// <summary>
	/// Implementação do validador de pacotes para arquivos locais.
	/// </summary>
	public class AssemblyPackageValidator : IAssemblyPackageValidator
	{
		/// <summary>
		/// Repositório dos arquivos de assembly.
		/// </summary>
		private string[] _assemblyFilesDirectories;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="assemblyFilesDirectories">Diretórios onde estão os arquivos de assembly.</param>
		public AssemblyPackageValidator(string[] assemblyFilesDirectories)
		{
			assemblyFilesDirectories.Require("assemblyFilesDirectories").NotNull().NotEmptyCollection();
			foreach (var i in assemblyFilesDirectories)
				if(string.IsNullOrEmpty(i) || !System.IO.Directory.Exists(i))
					throw new InvalidOperationException(string.Format("Assembly files directory {0} not exists", i));
			_assemblyFilesDirectories = assemblyFilesDirectories;
		}

		/// <summary>
		/// Valida o pacote.
		/// </summary>
		/// <param name="assemblyPackage"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		private bool Validate(IAssemblyPackage assemblyPackage, IDictionary<string, FileInfo2> files)
		{
			if(assemblyPackage == null)
				return false;
			FileInfo2 info = null;
			foreach (var part in assemblyPackage)
				if(!files.TryGetValue(part.Source, out info) || info.LastWriteTime > assemblyPackage.CreateTime)
					return false;
			return true;
		}

		/// <summary>
		/// Recupera a relação dos arquivos do diretório do repositório.
		/// </summary>
		/// <returns></returns>
		private IDictionary<string, FileInfo2> GetDirectoryFiles()
		{
			var dic = new Dictionary<string, FileInfo2>(Text.StringEqualityComparer.GetComparer(StringComparison.InvariantCultureIgnoreCase));
			foreach (var directory in _assemblyFilesDirectories)
				foreach (var i in System.IO.Directory.GetFiles(directory))
				{
					var fileName = System.IO.Path.GetFileName(i);
					if(!dic.ContainsKey(fileName))
						dic.Add(fileName, new FileInfo2(i));
				}
			return dic;
		}

		/// <summary>
		/// Valida os pacote informados.
		/// </summary>
		/// <param name="assemblyPackages">Instancia dos pacotes que serão validados.</param>
		/// <returns></returns>
		public bool[] Validate(IAssemblyPackage[] assemblyPackages)
		{
			assemblyPackages.Require("assemblyPackages").NotNull();
			if(assemblyPackages.Length == 0)
				return new bool[0];
			var files = GetDirectoryFiles();
			var result = new bool[assemblyPackages.Length];
			for(int i = 0; i < result.Length; i++)
				result[i] = Validate(assemblyPackages[i], files);
			return result;
		}

		/// <summary>
		/// Armazena as informações do arquivo.
		/// </summary>
		class FileInfo2
		{
			private string _path;

			private DateTime? _lastWriteTime;

			/// <summary>
			/// Nome completo do arquivo.
			/// </summary>
			public string Path
			{
				get
				{
					return _path;
				}
			}

			/// <summary>
			/// Horário da ultima escrita do arquivo
			/// </summary>
			public DateTime? LastWriteTime
			{
				get
				{
					if(_lastWriteTime == null)
						_lastWriteTime = System.IO.File.GetLastWriteTime(_path);
					return _lastWriteTime;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="path">Caminho do arquivo.</param>
			public FileInfo2(string path)
			{
				_path = path;
			}
		}
	}
}
