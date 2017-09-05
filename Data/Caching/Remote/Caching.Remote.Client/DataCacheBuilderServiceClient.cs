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
using Colosoft.Net;

namespace Colosoft.Data.Caching.Remote.Client
{
    /// <summary>
    /// Implementação do proxy para o serviço do builder do cache.
    /// </summary>
    class DataCacheBuilderServiceClient : System.ServiceModel.ClientBase<IDataCacheBuilderService>, IDataCacheBuilderService
    {
        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="endpointAddress"></param>
        public DataCacheBuilderServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress endpointAddress)
            : base(binding, endpointAddress)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executa a construção do cache para os tipos informados.
        /// </summary>
        /// <param name="types">Tipos no qual será construído o cache.</param>
        /// <returns></returns>
        public BuildExecutionResult ExecuteBuild(Colosoft.Reflection.TypeName[] types)
        {
            return base.Channel.ExecuteBuild(types);
        }

        /// <summary>
        /// Executa a construção do cache para todas as entidades registrada no sistema.
        /// </summary>
        /// <returns></returns>
        public BuildExecutionResult ExecuteBuildAll()
        {
            return base.Channel.ExecuteBuildAll();
        }

        /// <summary>
        /// Recupera as informações da execução.
        /// </summary>
        /// <param name="uid">Identificador da operação.</param>
        /// <returns></returns>
        public BuildExecutionResult GetExecution(Guid uid)
        {
            return base.Channel.GetExecution(uid);
        }

        /// <summary>
        /// Recupera a relação das execuções ativas.
        /// </summary>
        /// <returns></returns>        
        public Guid[] GetExecutions()
        {
            return base.Channel.GetExecutions();
        }

        /// <summary>
        /// Mata o processo de build do cache.
        /// </summary>
        /// <param name="uid">Identificador da operação.</param>
        /// <returns></returns>
        public BuildExecutionResult Kill(Guid uid)
        {
            return base.Channel.Kill(uid);
        }

        #endregion
    }
}
