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

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Armazena o resultado da recarga dos dados para o cache.
	/// </summary>
	public class RealoadDataCacheResult
	{
		/// <summary>
		/// Armazena os dados de uma erro do resultado.
		/// </summary>
		public class Error
		{
			/// <summary>
			/// Tipo no qual ocorreu o error.
			/// </summary>
			public Colosoft.Reflection.TypeName Type
			{
				get;
				set;
			}

			/// <summary>
			/// Erro ocorrido.
			/// </summary>
			public Exception Exception
			{
				get;
				set;
			}
		}

		/// <summary>
		/// Identifica se a recarga foi feita com sucesso.
		/// </summary>
		public bool Success
		{
			get;
			set;
		}

		/// <summary>
		/// Relação dos erros ocorridos.
		/// </summary>
		public Error[] Errors
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Assinatura da classe que gerencia o cache de dados do sistema.
	/// </summary>
	public interface IDataCacheManager : System.Collections.Generic.IEnumerable<Colosoft.Reflection.TypeName>, Colosoft.Caching.ICacheProvider
	{
		/// <summary>
		/// Evento acionado quando cache for completamenta carregado.
		/// </summary>
		event EventHandler Loaded;

		/// <summary>
		/// Evento acionado quando ocorre um erro na carga do cache.
		/// </summary>
		event Colosoft.Caching.CacheErrorEventHandler LoadError;

		/// <summary>
		/// Evento acionado quando ocorreu um erro no processamento da carga do cache.
		/// </summary>
		event Colosoft.Caching.CacheErrorEventHandler LoadProcessingError;

		/// <summary>
		/// Evento acionado quando ocorrer um erro ao inserir uma entrada no cache.
		/// </summary>
		event Colosoft.Caching.CacheInsertEntryErrorHandler InsertEntryError;

		/// <summary>
		/// Configura o cache para trabalhar com 
		/// os arquivos de dados que são armazenados
		/// local.
		/// </summary>
		void ConfigureLocalCache();

		/// <summary>
		/// Configura o cache para trabalhar os
		/// dados carregados diretamento do servidor.
		/// </summary>
		void ConfigureServerCache();

		/// <summary>
		/// Registra o nome do tipo que será carregado.
		/// </summary>
		/// <param name="typeName">Nome do tipo que será carregado.</param>
		/// <returns></returns>
		IDataCacheManager Register(Colosoft.Reflection.TypeName typeName);

		/// <summary>
		/// Registra o tipo que será carregado.
		/// </summary>
		/// <param name="type">Tipo que será carregado.</param>
		/// <returns></returns>
		IDataCacheManager Register(Type type);

		/// <summary>
		/// Registra o tipo que será carregado.
		/// </summary>
		/// <typeparam name="T">Tipo que será carregado.</typeparam>
		/// <returns></returns>
		IDataCacheManager Register<T>();

		/// <summary>
		/// Remove o registro do tipo para ser carregado para o cache.
		/// </summary>
		/// <param name="typeName">Nome do tipo que será removido.</param>
		/// <returns></returns>
		IDataCacheManager Unregister(Colosoft.Reflection.TypeName typeName);

		/// <summary>
		/// Recarrega os dados dos tipos informados para o cache.
		/// </summary>
		/// <param name="typeNames"></param>
		/// <returns></returns>
		RealoadDataCacheResult Reload(Colosoft.Reflection.TypeName[] typeNames);

		/// <summary>
		/// Inicializa o processo assincrono que recarrega os dados para o cache.
		/// </summary>
		/// <param name="typeNames">Nome dos tipos que serão processados.</param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult BeginReload(Colosoft.Reflection.TypeName[] typeNames, AsyncCallback callback, object state);

		/// <summary>
		/// Recupera o resulta da execução assincrona da recarga dos dados para o cache.
		/// </summary>
		/// <param name="ar"></param>
		/// <rereturns></rereturns>
		RealoadDataCacheResult EndReload(IAsyncResult ar);
	}
}
