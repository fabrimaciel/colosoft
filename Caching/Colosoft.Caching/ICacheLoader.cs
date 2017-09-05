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

namespace Colosoft.Caching
{
	/// <summary>
	/// Assinatura da classe de inicialização do cache.
	/// </summary>
	public interface ICacheLoader : IDisposable
	{
		/// <summary>
		/// Evento acionado quando ocorre um erro no processamento da carga.
		/// </summary>
		event CacheErrorEventHandler LoadProcessingError;

		/// <summary>
		/// Inicializa a instancia com os parametros informados.
		/// </summary>
		/// <param name="parameters">Dicionário com os parametros de inicialização.</param>
		void Init(System.Collections.IDictionary parameters);

		/// <summary>
		/// Carrega o próximo item do cache.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		bool LoadNext(ref System.Collections.Specialized.OrderedDictionary data, ref object index);
	}
	/// <summary>
	/// Assinatura das classe de loader do cache que identifica ser possui 
	/// suporte para carga assincrona.
	/// </summary>
	public interface ICacheLoaderAsyncSupport : ICacheLoader
	{
		/// <summary>
		/// Identifica se possui suporte para carga assincrona.
		/// </summary>
		bool IsAsyncSupport
		{
			get;
		}
	}
}
