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

namespace Colosoft.Security.Remote.Client
{
    /// <summary>
    /// Argumentos do evento acionado quando o cliente o provedor de usuário
    /// remoto for configurado.
    /// </summary>
    public class ConfigureRemoteClientEventArgs : EventArgs
    {
        #region Local Variables

        private System.ServiceModel.ICommunicationObject _communicationObject;
        private System.ServiceModel.Description.ServiceEndpoint _endpoint;

        #endregion

        #region Properties

        /// <summary>
        /// Instancia do objeto de comunicação.
        /// </summary>
        public System.ServiceModel.ICommunicationObject CommunicationObject
        {
            get { return _communicationObject; }
        }

        /// <summary>
        /// Endereço do endpoint.
        /// </summary>
        public System.ServiceModel.Description.ServiceEndpoint Endpoint
        {
            get { return _endpoint; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="communicationObject"></param>
        /// <param name="endpoint"></param>
        public ConfigureRemoteClientEventArgs(
            System.ServiceModel.ICommunicationObject communicationObject,
            System.ServiceModel.Description.ServiceEndpoint endpoint)
        {
            _communicationObject = communicationObject;
            _endpoint = endpoint;
        }

        #endregion
    }

    /// <summary>
    /// Representa o evento acionado quando o cliente o provedor de usuário
    /// remoto for configurado.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ConfigureRemoteClientEventHandler(object sender, ConfigureRemoteClientEventArgs e);
}
