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
using System.Text;
using System.Web;
using System.Security.Permissions;
using System.Web.UI;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Representa as informações de um item basico.
	/// </summary>
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class BaseItem : IStateManager, INamingContainer
	{
		private bool _isTrackingViewState;

		private StateBag _statebag = new StateBag();

		/// <summary>
		/// Recupera os dados do ViewState.
		/// </summary>
		/// <param name="savedState"></param>
		protected virtual void LoadViewState(object savedState)
		{
			object[] objArray = (object[])savedState;
			if(objArray.Length != 1)
			{
				throw new ArgumentException("Invalid View State");
			}
			((IStateManager)this.ViewState).LoadViewState(objArray[0]);
		}

		/// <summary>
		/// Salva os dados do ViewState.
		/// </summary>
		/// <returns></returns>
		protected virtual object SaveViewState()
		{
			object[] objArray = new object[1];
			if(this.ViewState != null)
			{
				objArray[0] = ((IStateManager)this.ViewState).SaveViewState();
			}
			return objArray;
		}

		protected virtual void TrackViewState()
		{
			_isTrackingViewState = true;
			if(this.ViewState != null)
			{
				((IStateManager)this.ViewState).TrackViewState();
			}
		}

		/// <summary>
		/// Marca a instancia como suja.
		/// </summary>
		internal void SetDirty()
		{
			ViewState.SetDirty(true);
		}

		void IStateManager.LoadViewState(object state)
		{
			this.LoadViewState(state);
		}

		object IStateManager.SaveViewState()
		{
			return this.SaveViewState();
		}

		void IStateManager.TrackViewState()
		{
			this.TrackViewState();
		}

		protected virtual bool IsTrackingViewState
		{
			get
			{
				return _isTrackingViewState;
			}
		}

		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return this.IsTrackingViewState;
			}
		}

		protected StateBag ViewState
		{
			get
			{
				return this._statebag;
			}
		}
	}
}
