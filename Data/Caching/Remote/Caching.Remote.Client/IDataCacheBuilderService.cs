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
using System.ServiceModel;

namespace Colosoft.Data.Caching.Remote.Client
{
    /// <summary>
    /// Assinatura do serviço do builder de cache.
    /// </summary>
    [ServiceContract(Name = "IDataCacheBuilderService", Namespace = "http://www.Colosoft.com.br/Caching/01")]
    public interface IDataCacheBuilderService
    {
        /// <summary>
        /// Executa a construção do cache para os tipos informados.
        /// </summary>
        /// <param name="types">Tipos no qual será construído o cache.</param>
        /// <returns></returns>
        [OperationContract]
        BuildExecutionResult ExecuteBuild(Colosoft.Reflection.TypeName[] types);

        /// <summary>
        /// Executa a construção do cache para todas as entidades registrada no sistema.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        BuildExecutionResult ExecuteBuildAll();

        /// <summary>
        /// Recupera as informações da execução.
        /// </summary>
        /// <param name="uid">Identificador da operação.</param>
        /// <returns></returns>
        [OperationContract]
        BuildExecutionResult GetExecution(Guid uid);

        /// <summary>
        /// Recupera a relação das execuções ativas.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Guid[] GetExecutions();

        /// <summary>
        /// Mata o processo de build do cache.
        /// </summary>
        /// <param name="uid">Identificador da operação.</param>
        /// <returns></returns>
        [OperationContract]
        BuildExecutionResult Kill(Guid uid);
    }
}
