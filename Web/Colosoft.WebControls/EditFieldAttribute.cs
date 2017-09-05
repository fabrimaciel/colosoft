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
using System.Web.UI;
using System.Web;
using System.Security.Permissions;
using System.ComponentModel;

namespace Colosoft.WebControls.GridView
{
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class EditFieldAttribute : IStateManager
	{
		private bool _isTracking;

		private StateBag _viewState = new StateBag();

		void IStateManager.LoadViewState(object state)
		{
			if(state != null)
			{
				((IStateManager)ViewState).LoadViewState(state);
			}
		}

		object IStateManager.SaveViewState()
		{
			return ((IStateManager)ViewState).SaveViewState();
		}

		void IStateManager.TrackViewState()
		{
			this._isTracking = true;
			((IStateManager)ViewState).TrackViewState();
		}

		/// <summary>
		/// Nome do atributo do campo.
		/// </summary>
		[DefaultValue(""), NotifyParentProperty(true), Category("Behavior"), Description("The name of the HTML attribute")]
		public string Name
		{
			get
			{
				if(ViewState["Name"] != null)
					return (string)ViewState["Name"];
				return "";
			}
			set
			{
				ViewState["Name"] = value;
			}
		}

		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return _isTracking;
			}
		}

		/// <summary>
		/// Valor do atributo.
		/// </summary>
		[DefaultValue(""), Description(""), NotifyParentProperty(true), Category("Behavior")]
		public string Value
		{
			get
			{
				if(ViewState["Value"] != null)
					return (string)ViewState["Value"];
				return "";
			}
			set
			{
				ViewState["Value"] = value;
			}
		}

		private StateBag ViewState
		{
			get
			{
				return _viewState;
			}
		}
	}
}
