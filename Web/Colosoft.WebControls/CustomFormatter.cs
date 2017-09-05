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
using System.ComponentModel;

namespace Colosoft.WebControls.GridView
{
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class CustomFormatter : ColumnFormatter, IStateManager
	{
		private bool _isTracking;

		private StateBag _viewState = new StateBag();

		void IStateManager.LoadViewState(object state)
		{
			if(state != null)
			{
				((IStateManager)this.ViewState).LoadViewState(state);
			}
		}

		object IStateManager.SaveViewState()
		{
			return ((IStateManager)this.ViewState).SaveViewState();
		}

		void IStateManager.TrackViewState()
		{
			this._isTracking = true;
			((IStateManager)this.ViewState).TrackViewState();
		}

		[DefaultValue(""), NotifyParentProperty(true), Category("Behavior"), Description("The javascript function that will format the value. The first parameter is the cell value.")]
		public string FormatFunction
		{
			get
			{
				if(this.ViewState["FormatFunction"] != null)
				{
					return (string)this.ViewState["FormatFunction"];
				}
				return "";
			}
			set
			{
				this.ViewState["FormatFunction"] = value;
			}
		}

		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return this._isTracking;
			}
		}

		[NotifyParentProperty(true), Category("Behavior"), Description("The javascript function that will unformat the value upon editing. The first parameter is the formatted value."), DefaultValue("")]
		public string UnFormatFunction
		{
			get
			{
				if(this.ViewState["UnFormatFunction"] != null)
				{
					return (string)this.ViewState["UnFormatFunction"];
				}
				return "";
			}
			set
			{
				this.ViewState["UnFormatFunction"] = value;
			}
		}

		private StateBag ViewState
		{
			get
			{
				return this._viewState;
			}
		}
	}
}
