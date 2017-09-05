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

namespace Colosoft.Caching.Configuration.Dom
{
	/// <summary>
	/// Armazena os dados da configuração do loader do cache.
	/// </summary>
	[Serializable]
	public class CacheLoader : ICloneable
	{
		private bool _enabled;

		private Parameter[] _parameters;

		private ProviderAssembly _provider;

		private int _retries;

		private int _retryInterval;

		/// <summary>
		/// Identifica se o loader está habilitado.
		/// </summary>
		[ConfigurationAttribute("enabled")]
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}

		/// <summary>
		/// Parametros que serão repassados para o loader.
		/// </summary>
		[ConfigurationSection("param")]
		public Parameter[] Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}

		/// <summary>
		/// Provedor do assembly.
		/// </summary>
		[ConfigurationSection("provider")]
		public ProviderAssembly Provider
		{
			get
			{
				return _provider;
			}
			set
			{
				_provider = value;
			}
		}

		/// <summary>
		/// Número de tentativas para a carga.
		/// </summary>
		[ConfigurationAttribute("retries")]
		public int Retries
		{
			get
			{
				return _retries;
			}
			set
			{
				_retries = value;
			}
		}

		/// <summary>
		/// Intervalo em segundos para tentar carregar o cache.
		/// </summary>
		[ConfigurationAttribute("retry-interval")]
		public int RetryInterval
		{
			get
			{
				return _retryInterval;
			}
			set
			{
				_retryInterval = value;
			}
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			CacheLoader loader = new CacheLoader();
			loader._enabled = _enabled;
			loader._retries = _retries;
			loader._retryInterval = _retryInterval;
			loader._provider = (_provider != null) ? ((ProviderAssembly)_provider.Clone()) : null;
			loader._parameters = (_parameters != null) ? ((Parameter[])_parameters.Clone()) : null;
			return loader;
		}
	}
}
