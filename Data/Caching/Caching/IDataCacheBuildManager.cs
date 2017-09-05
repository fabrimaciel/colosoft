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

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Representa um erro do build do cache de dados.
	/// </summary>
	[Serializable]
	public class DataCacheBuildException : Exception
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public DataCacheBuildException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Construtor usado pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecuritySafeCritical]
		protected DataCacheBuildException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
	}
	/// <summary>
	/// Representa o erro ocorrido quando serviço de build está offline.
	/// </summary>
	[Serializable]
	public class DataCacheBuildOfflineException : DataCacheBuildException
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="innerException"></param>
		public DataCacheBuildOfflineException(Exception innerException) : base(ResourceMessageFormatter.Create(() => Properties.Resources.DataCacheBuildOfflineException_Message).Format(), innerException)
		{
		}

		/// <summary>
		/// Construtor usado pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecuritySafeCritical]
		protected DataCacheBuildOfflineException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
	}
	/// <summary>
	/// Assinatura do gerenciador de build do cache de dados.
	/// </summary>
	public interface IDataCacheBuildManager : IDisposable
	{
		/// <summary>
		/// Instancia do monitor associado.
		/// </summary>
		IDataCacheBuildMonitor Monitor
		{
			get;
		}

		/// <summary>
		/// Executa a construção do cache para os tipos informados.
		/// </summary>
		/// <param name="types">Tipos no qual será construído o cache.</param>
		/// <returns></returns>
		BuildExecutionResult ExecuteBuild(Colosoft.Reflection.TypeName[] types);

		/// <summary>
		/// Executa a construção do cache para todas as entidades registrada no sistema.
		/// </summary>
		/// <returns></returns>
		BuildExecutionResult ExecuteBuildAll();

		/// <summary>
		/// Recupera as informações da execução.
		/// </summary>
		/// <param name="uid">Identificador da operação.</param>
		/// <returns></returns>
		BuildExecutionResult GetExecution(Guid uid);

		/// <summary>
		/// Recupera a relação das execuções ativas.
		/// </summary>
		/// <returns></returns>
		Guid[] GetExecutions();

		/// <summary>
		/// Mata o processo de build do cache.
		/// </summary>
		/// <param name="uid">Identificador da operação.</param>
		/// <returns></returns>
		BuildExecutionResult Kill(Guid uid);
	}
}
