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

namespace Colosoft.Caching.Dependencies
{
	/// <summary>
	/// Representa uma dependência do cache.
	/// </summary>
	[Serializable]
	public class CacheDependency : IDisposable
	{
		private List<CacheDependency> _dependencies;

		private DateTime _startAfter;

		/// <summary>
		/// Subdependencias.
		/// </summary>
		public List<CacheDependency> Dependencies
		{
			get
			{
				return _dependencies;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CacheDependency()
		{
			_dependencies = new List<CacheDependency>();
		}

		/// <summary>
		/// Cria uma instancia já definindo uma dependencia extensivel.
		/// </summary>
		/// <param name="extensibleDependency"></param>
		public CacheDependency(ExtensibleDependency extensibleDependency)
		{
			_dependencies = new List<CacheDependency>();
			_dependencies.Add(extensibleDependency);
		}

		/// <summary>
		/// Cria uma dependencia com um arquivo.
		/// </summary>
		/// <param name="fileName">Nome do arquivo que a dependencia ira monitorar.</param>
		public CacheDependency(string fileName) : this(fileName, DateTime.Now)
		{
		}

		/// <summary>
		/// Cria uma dependencia com vários arquivos.
		/// </summary>
		/// <param name="fileNames">Nomes dos arquivos que a dependencia ira monitorar.</param>
		public CacheDependency(string[] fileNames) : this(fileNames, null, null, DateTime.Now)
		{
		}

		/// <summary>
		/// Cria uma instancia de dependencia para um arquivo.
		/// </summary>
		/// <param name="fileName">Nome do arquivo que a dependencia ira monitorar.</param>
		/// <param name="start">Data de início do monitoramenteo da dependencia.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public CacheDependency(string fileName, DateTime start)
		{
			fileName.Require("fileName").NotNull().NotEmpty();
			CacheDependency item = new FileDependency(fileName, start);
			if(_dependencies == null)
				_dependencies = new List<CacheDependency>();
			_dependencies.Add(item);
			_startAfter = start;
		}

		/// <summary>
		/// Cria uma instancia de dependencia para vários arquivos.
		/// </summary>
		/// <param name="fileNames">Nomes dos arquivos que a dependencia ira monitorar.</param>
		/// <param name="start">Data de início do monitoramenteo da dependencia.</param>
		public CacheDependency(string[] fileNames, DateTime start) : this(fileNames, null, null, start)
		{
		}

		/// <summary>
		/// Cria uma instancia de dependencia para vários arquivos e chaves de cache.
		/// </summary>
		/// <param name="fileNames"></param>
		/// <param name="cacheKeys"></param>
		public CacheDependency(string[] fileNames, string[] cacheKeys) : this(fileNames, cacheKeys, null, DateTime.Now)
		{
		}

		/// <summary>
		/// Cria uma instancia de dependencia para vários arquivos e chaves de cache.
		/// </summary>
		/// <param name="fileNames"></param>
		/// <param name="cacheKeys"></param>
		/// <param name="dependency"></param>
		public CacheDependency(string[] fileNames, string[] cacheKeys, CacheDependency dependency) : this(fileNames, cacheKeys, dependency, DateTime.Now)
		{
		}

		/// <summary>
		/// Cria uma instancia de dependencia para vários arquivos e chaves de cache.
		/// </summary>
		/// <param name="fileNames"></param>
		/// <param name="cacheKeys"></param>
		/// <param name="start"></param>
		public CacheDependency(string[] fileNames, string[] cacheKeys, DateTime start) : this(fileNames, cacheKeys, null, start)
		{
		}

		/// <summary>
		/// Cria uma instancia de dependencia para vários arquivos e chaves de cache.
		/// </summary>
		/// <param name="fileNames"></param>
		/// <param name="cacheKeys"></param>
		/// <param name="dependency"></param>
		/// <param name="start"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public CacheDependency(string[] fileNames, string[] cacheKeys, CacheDependency dependency, DateTime start)
		{
			fileNames.Require("fileNames").NotNull().NotEmptyCollection();
			cacheKeys.Require("cacheKeys").NotNull().NotEmptyCollection();
			foreach (string str in fileNames)
				str.Require("fileName").NotNull().NotEmpty();
			var item = new FileDependency(fileNames, start);
			foreach (string str2 in cacheKeys)
				str2.Require("cacheKey").NotNull().NotEmpty();
			var dependency3 = new KeyDependency(cacheKeys, start);
			if(dependency != null)
			{
				_dependencies = new List<CacheDependency>();
				_dependencies.Add(item);
				_dependencies.Add(dependency3);
				_dependencies.Add(dependency);
			}
			_startAfter = start;
		}

		/// <summary>
		/// Adiciona uma lista de depdencias para a instancia.
		/// </summary>
		/// <param name="dependencies"></param>
		protected void AddDependencies(params CacheDependency[] dependencies)
		{
			_dependencies.AddRange(dependencies);
		}

		/// <summary>
		/// Libera a instancia da dependencia.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
	}
}
