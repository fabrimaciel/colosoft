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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Colosoft.ServiceModel.Channels
{
	/// <summary>
	/// Implementação base do canal.
	/// </summary>
	public abstract class ChannelBase : CommunicationObject, ICommunicationObject, IChannel, IDefaultCommunicationTimeouts
	{
		private ChannelManagerBase manager;

		protected ChannelBase(ChannelManagerBase manager)
		{
			this.manager = manager;
		}

		protected override TimeSpan DefaultCloseTimeout
		{
			get
			{
				throw new NotImplementedException("Actually it should be overriden before being used.");
			}
		}

		protected override TimeSpan DefaultOpenTimeout
		{
			get
			{
				throw new NotImplementedException("Actually it should be overriden before being used.");
			}
		}

		protected internal TimeSpan DefaultReceiveTimeout
		{
			get
			{
				return manager.DefaultReceiveTimeout;
			}
		}

		protected internal TimeSpan DefaultSendTimeout
		{
			get
			{
				return manager.DefaultSendTimeout;
			}
		}

		protected ChannelManagerBase Manager
		{
			get
			{
				return manager;
			}
		}

		public virtual T GetProperty<T>() where T : class
		{
			return null;
		}

		protected override void OnClosed()
		{
			base.OnClosed();
		}

		TimeSpan IDefaultCommunicationTimeouts.CloseTimeout
		{
			get
			{
				return DefaultCloseTimeout;
			}
		}

		TimeSpan IDefaultCommunicationTimeouts.OpenTimeout
		{
			get
			{
				return DefaultOpenTimeout;
			}
		}

		TimeSpan IDefaultCommunicationTimeouts.ReceiveTimeout
		{
			get
			{
				return DefaultReceiveTimeout;
			}
		}

		TimeSpan IDefaultCommunicationTimeouts.SendTimeout
		{
			get
			{
				return DefaultSendTimeout;
			}
		}
	}
}
