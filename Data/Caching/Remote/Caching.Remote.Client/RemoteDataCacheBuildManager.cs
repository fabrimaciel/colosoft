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

namespace Colosoft.Data.Caching.Remote.Client
{
    /// <summary>
    /// Implementação do gerenciador remoto de build do cache de dados.
    /// </summary>
    public class RemoteDataCacheBuildManager : IDataCacheBuildManager
    {
        #region Constants

        /// <summary>
        /// Nome da configuração do serviço.
        /// </summary>
        const string DataCacheBuilderClientConfigurationName = "DataCacheBuilderService";

        #endregion

        #region Local Variables

        private DataCacheBuildMonitor _monitor;
        private readonly string _clientUid = Guid.NewGuid().ToString();

        #endregion

        #region Properties

        /// <summary>
        /// Instancia do cliente do serviço.
        /// </summary>
        private DataCacheBuilderServiceClient Client
        {
            get
            {
                return Colosoft.Net.ServiceClientsManager.Current
                    .Get<DataCacheBuilderServiceClient>(_clientUid);
            }
        }

        /// <summary>
        /// Instancia do monitor associado.
        /// </summary>
        public IDataCacheBuildMonitor Monitor
        {
            get { return _monitor; }
        }

        #endregion

         #region Constructors

        /// <summary>
        /// Construtor padrão
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public RemoteDataCacheBuildManager()
        {
            _monitor = new DataCacheBuildMonitor(this);
            Colosoft.Net.ServicesConfiguration.Current.Updated += ServicesConfigurationUpdated;
            Colosoft.Net.ServiceClientsManager.Current.Register(_clientUid,
                () =>
                {
                    var serviceAddress = Colosoft.Net.ServicesConfiguration.Current[DataCacheBuilderClientConfigurationName];

                    if (serviceAddress == null)
                        throw new InvalidOperationException(string.Format("Address to service {0} undefined.", DataCacheBuilderClientConfigurationName));

                    var client = new DataCacheBuilderServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());
                    Colosoft.Net.SecurityTokenBehavior.Register(client.Endpoint);
                    return client;
                });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Método acionado quando o endereço do serviço for alterado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServicesConfigurationUpdated(object sender, Colosoft.Net.ServicesConfigurationActionEventArgs e)
        {
            if (e.ServiceName == DataCacheBuilderClientConfigurationName)
                Colosoft.Net.ServiceClientsManager.Current.Reset(_clientUid);
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
            try
            {
                return Client.ExecuteBuild(types);
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new DataCacheBuildOfflineException(ex);
            }
        }

        /// <summary>
        /// Executa a construção do cache para todas as entidades registrada no sistema.
        /// </summary>
        /// <returns></returns>
        public BuildExecutionResult ExecuteBuildAll()
        {
            try
            {
                return Client.ExecuteBuildAll();
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new DataCacheBuildOfflineException(ex);
            }
        }

        /// <summary>
        /// Recupera as informações da execução.
        /// </summary>
        /// <param name="uid">Identificador da operação.</param>
        /// <returns></returns>
        public BuildExecutionResult GetExecution(Guid uid)
        {
            try
            {
                return Client.GetExecution(uid);
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new DataCacheBuildOfflineException(ex);
            }
        }

        /// <summary>
        /// Recupera a relação das execuções ativas.
        /// </summary>
        /// <returns></returns>
        public Guid[] GetExecutions()
        {
            try
            {
                return Client.GetExecutions();
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new DataCacheBuildOfflineException(ex);
            }
        }

        /// <summary>
        /// Mata o processo de build do cache.
        /// </summary>
        /// <param name="uid">Identificador da operação.</param>
        /// <returns></returns>
        public BuildExecutionResult Kill(Guid uid)
        {
            try
            {
                return Client.Kill(uid);
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new DataCacheBuildOfflineException(ex);
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Libera a instancia.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            _monitor.Dispose();
        }

        /// <summary>
        /// Libera a instancia.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
