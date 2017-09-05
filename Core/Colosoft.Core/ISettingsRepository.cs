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
	/// Representa um repositório que armazena os dados de configuração.
	/// </summary>
	public interface ISettingsRepository
	{
		/// <summary>
		/// Nome do respositório.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Recupera uma instancia de configuração contida no caminho informado.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		T Get<T>(SettingsScope scope, Uri location);

		/// <summary>
		/// Define o valor de uma variável textual da configuração.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="location"></param>
		/// <param name="value"></param>
		void Set(SettingsScope scope, Uri location, object value);

		/// <summary>
		/// Recupera uma stream contida no caminho informado.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="location">Localização da stream na configuração.</param>
		/// <returns></returns>
		System.IO.Stream GetStream(SettingsScope scope, Uri location);

		/// <summary>
		/// Salva a stream na configuração.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="location"></param>
		/// <param name="stream"></param>
		void SaveStream(SettingsScope scope, Uri location, System.IO.Stream stream);
	}
}
