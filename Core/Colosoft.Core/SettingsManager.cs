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

namespace Colosoft.Configuration
{
	/// <summary>
	/// Classe reposável por gerenciar as configuração do sistema.
	/// </summary>
	public static class SettingsManager
	{
		private static object objLock = new object();

		/// <summary>
		/// Dicionário onde os repositórios são armazenados.
		/// </summary>
		private static Dictionary<string, ISettingsRepository> _repositories = new Dictionary<string, ISettingsRepository>();

		/// <summary>
		/// Adiciona um repositório no gerenciador.
		/// </summary>
		/// <param name="repository"></param>
		public static void AddRepository(ISettingsRepository repository)
		{
			if(repository == null)
				throw new ArgumentNullException("repository");
			lock (objLock)
			{
				if(_repositories.ContainsKey(repository.Name))
					_repositories.Remove(repository.Name);
				_repositories.Add(repository.Name, repository);
			}
		}

		/// <summary>
		/// Recupera o repositório pelo nome.
		/// </summary>
		/// <param name="name">Nome do repositório.</param>
		/// <returns></returns>
		public static ISettingsRepository GetRepository(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			ISettingsRepository repository = null;
			lock (objLock)
			{
				if(_repositories.TryGetValue(name, out repository))
					return repository;
			}
			return null;
		}

		/// <summary>
		/// Recupera uma instancia de configuração contida no caminho informado.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		public static T Get<T>(SettingsScope scope, Uri location)
		{
			if(location == null)
				throw new ArgumentNullException("location");
			var repository = GetRepository(location.Scheme);
			if(repository == null)
				return default(T);
			return repository.Get<T>(scope, location);
		}

		/// <summary>
		/// Define o valor de uma variável textual da configuração.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="location"></param>
		/// <param name="value"></param>
		public static void Set(SettingsScope scope, Uri location, object value)
		{
			if(location == null)
				throw new ArgumentNullException("location");
			var repository = GetRepository(location.Scheme);
			if(repository == null)
				throw new InvalidOperationException(string.Format(Properties.Resources.InvalidOperation_SettingsManager_RepositoryNotFound, location.Scheme));
			repository.Set(scope, location, value);
		}

		/// <summary>
		/// Recupera uma stream contida no caminho informado.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="location">Localização da stream na configuração.</param>
		/// <returns></returns>
		public static System.IO.Stream GetStream(SettingsScope scope, Uri location)
		{
			var repository = GetRepository(location.Scheme);
			if(repository == null)
				throw new InvalidOperationException(string.Format(Properties.Resources.InvalidOperation_SettingsManager_RepositoryNotFound, location.Scheme));
			return repository.GetStream(scope, location);
		}

		/// <summary>
		/// Salva a stream na configuração.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="location"></param>
		/// <param name="stream"></param>
		public static void SaveStream(SettingsScope scope, Uri location, System.IO.Stream stream)
		{
			var repository = GetRepository(location.Scheme);
			if(repository == null)
				throw new InvalidOperationException(string.Format(Properties.Resources.InvalidOperation_SettingsManager_RepositoryNotFound, location.Scheme));
			repository.SaveStream(scope, location, stream);
		}
	}
}
