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
    /// Implementação da interface <see cref="IDataEntryDownloader"/>.
    /// </summary>
    public class RemoteDataEntryDownloader : IDataEntryDownloader
    {
        #region Constants

        /// <summary>
        /// Nome da configuração do serviço de autenticação.
        /// </summary>
        const string DataEntryDownloaderClientConfigurationName = "DataEntryDownloaderService";

        #endregion

        #region Local Variables

        private readonly string _clientUid = Guid.NewGuid().ToString();

        /// <summary>
        /// Versão que será recuperadas no download.
        /// </summary>
        private Queue<DataEntryVersion> _versions = new Queue<DataEntryVersion>();
        private System.Threading.Thread _workThread;
        private bool _isBusy = false;
        private bool _isCancelled = false;

        #endregion

        #region Events

        /// <summary>
        /// Evento disparado quando o progresso do download se altera.
        /// </summary>
        public event Colosoft.Net.DownloadProgressEventHandler ProgressChanged;

        /// <summary>
        /// Evento disparado quando o download for finalizado.
        /// </summary>
        public event Colosoft.Net.DownloadCompletedEventHandler DownloadCompleted;

        #endregion

        #region Properties

        /// <summary>
        /// Instancia do cliente do serviço.
        /// </summary>
        private DataEntryDownloaderServiceReference.DataEntryDownloaderServiceClient Client
        {
            get
            {
                return Colosoft.Net.ServiceClientsManager.Current
                    .Get<DataEntryDownloaderServiceReference.DataEntryDownloaderServiceClient>(_clientUid);
            }
        }
                
        /// <summary>
        /// Identifica se a instancia está ocupada realizando o donwload.
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public RemoteDataEntryDownloader()
        {
            Colosoft.Net.ServicesConfiguration.Current.Updated += ServicesConfigurationUpdated;
            Colosoft.Net.ServiceClientsManager.Current.Register(_clientUid,
                () =>
                {
                    var serviceAddress = Colosoft.Net.ServicesConfiguration.Current[DataEntryDownloaderClientConfigurationName];

                    if (serviceAddress == null)
                        throw new InvalidOperationException(string.Format("Address to service {0} undefined.", DataEntryDownloaderClientConfigurationName));

                    var client = new DataEntryDownloaderServiceReference.DataEntryDownloaderServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());
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
            if (e.ServiceName == DataEntryDownloaderClientConfigurationName)
                Colosoft.Net.ServiceClientsManager.Current.Reset(_clientUid);
        }

        /// <summary>
        /// Verifica se a instancia está ocupada, e caso
        /// esteja dispara uma exception.
        /// </summary>
        private void CheckIsBusy()
        {
            if (IsBusy)
                throw new DetailsInvalidOperationException(
                    ResourceMessageFormatter.Create(
                        () => Properties.Resources.RemoteDataEntryDownloader_DownloaderIsBusy));
        }

        /// <summary>
        /// Realiza o processamento.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void DoWork(object userState)
        {
            DataEntryPackage result = null;
            Exception lastException = null;
            try
            {
                var versions = _versions.ToArray();
                string name = null;
                System.IO.Stream outStream = null;
                System.IO.Stream fsStream = null;

                var totalBytesToReceive = Client.GetDataEntries(versions, out name, out outStream);

                try
                {
                    var buffer = new byte[1024];
                    var read = 0;

                    var tempFile = System.IO.Path.GetTempFileName();

                    fsStream = new DownloaderFileStream(this, totalBytesToReceive, tempFile, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);

                    while ((read = outStream.Read(buffer, 0, buffer.Length)) > 0)
                        fsStream.Write(buffer, 0, read);

                    fsStream.Position = 0;

                    result = new DataEntryPackage(fsStream);
                }
                catch
                {
                    if (fsStream != null)
                        fsStream.Dispose();
                    throw;
                }
                finally
                {
                    outStream.Dispose();
                }
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                if (!_isCancelled)
                    lastException = ex;
            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            _isBusy = false;

            try
            {
                // Aciona o evento informado que o download foi feito
                OnDownloadCompleted(lastException, _isCancelled, result);
            }
            finally
            {
                if (result != null)
                    result.Dispose();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Método acionado quando o download for completado.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="cancelled"></param>
        /// <param name="result"></param>
        protected void OnDownloadCompleted(Exception error, bool cancelled, DataEntryPackage result)
        {
            if (DownloadCompleted != null)
                DownloadCompleted(this, new DataEntryDownloadCompletedEventArgs(error, cancelled, null, result));
        }

        /// <summary>
        /// Método acionado quando o progresso do download for alterado.
        /// </summary>
        /// <param name="totalBytesToReceive"></param>
        /// <param name="bytesReceived"></param>
        protected void OnDownloadProgress(long totalBytesToReceive, long bytesReceived)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, new Colosoft.Net.DownloadProgressChangedEventArgs(bytesReceived, totalBytesToReceive));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adiciona a versão para realizar o download.
        /// </summary>
        /// <param name="version"></param>
        public void Add(DataEntryVersion version)
        {
            version.Require("version").NotNull();
            CheckIsBusy();

            lock (_versions)
                _versions.Enqueue(version);
        }

        /// <summary>
        /// Adiciona uma faixa de versões para fazer o download.
        /// </summary>
        /// <param name="versions"></param>
        public void AddRange(IEnumerable<DataEntryVersion> versions)
        {
            versions.Require("versions").NotNull();
            CheckIsBusy();

            lock (_versions)
                foreach (var i in versions.Where(f => f != null))
                    _versions.Enqueue(i);
        }

        /// <summary>
        /// Limpa as versões que foram registrada para ser feito o download.
        /// </summary>
        public void Clear()
        {
            CheckIsBusy();

            lock (_versions)
                _versions.Clear();
        }

        /// <summary>
        /// Executa o download em background.
        /// </summary>
        public void RunAsync(object userState)
        {
            CheckIsBusy();

            if (_versions.Count == 0)
            {
                OnDownloadCompleted(null, false, new DataEntryPackage(null));
                return;
            }

            _isCancelled = false;
            _isBusy = true;

            try
            {
                _workThread = new System.Threading.Thread(DoWork);
                _workThread.Start(userState);
            }
            catch (Exception)
            {
                _isBusy = false;
                throw;
            }
        }

        /// <summary>
        /// Cancela a execução assincrona do donwnload.
        /// </summary>
        public void CancelAsync()
        {
            if (IsBusy)
            {
                _isCancelled = true;
                _workThread.Abort();
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Libera a instancia.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Libera a instancia.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_workThread != null)
            {
                _workThread.Abort();
                _workThread = null;
            }

            Colosoft.Net.ServicesConfiguration.Current.Updated -= ServicesConfigurationUpdated;
            Colosoft.Net.ServiceClientsManager.Current.Remove(_clientUid);
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// Adaptação do <see cref="System.IO.FileStream"/> para da suporte aos recursos do RemoteApplicationDownloader.
        /// </summary>
        class DownloaderFileStream : System.IO.FileStream
        {
            #region Local Variables

            private RemoteDataEntryDownloader _downloader;
            private long _totalBytesToReceive = 0;

            #endregion

            #region Constructors

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="downloader"></param>
            /// <param name="totalBytesToReceive"></param>
            /// <param name="path"></param>
            /// <param name="fileMode"></param>
            /// <param name="fileAccess"></param>
            public DownloaderFileStream(RemoteDataEntryDownloader downloader, long totalBytesToReceive,
                                        string path, System.IO.FileMode fileMode, System.IO.FileAccess fileAccess)
                : base(path, fileMode, fileAccess)
            {
                _downloader = downloader;
                _totalBytesToReceive = totalBytesToReceive;
            }

            #endregion

            /// <summary>
            /// 
            /// </summary>
            /// <param name="array"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            public override void Write(byte[] array, int offset, int count)
            {
                base.Write(array, offset, count);
                _downloader.OnDownloadProgress(_totalBytesToReceive, count);
            }
        }

        #endregion
    }
}
