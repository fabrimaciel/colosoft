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
using System.ComponentModel;
using System.Web;
using System.Security.Permissions;
using System.Web.UI;

namespace Colosoft.WebControls.GridView
{
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class NumberFormatter : ColumnFormatter, IStateManager
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

		[DefaultValue(-1), NotifyParentProperty(true), Category("Behavior"), Description("The number of decimal places. Defaults to localization settings.")]
		public int DecimalPlaces
		{
			get
			{
				if(this.ViewState["DecimalPlaces"] != null)
				{
					return (int)this.ViewState["DecimalPlaces"];
				}
				return -1;
			}
			set
			{
				this.ViewState["DecimalPlaces"] = value;
			}
		}

		[NotifyParentProperty(true), Category("Behavior"), Description("The decimal separator. Defaults to localization settings."), DefaultValue("")]
		public string DecimalSeparator
		{
			get
			{
				if(this.ViewState["DecimalSeparator"] != null)
				{
					return (string)this.ViewState["DecimalSeparator"];
				}
				return "";
			}
			set
			{
				this.ViewState["DecimalSeparator"] = value;
			}
		}

		[NotifyParentProperty(true), DefaultValue(""), Category("Behavior"), Description("The default value of the cell, if cell value is not specified.")]
		public string DefaultValue
		{
			get
			{
				if(this.ViewState["DefaultValue"] != null)
				{
					return (string)this.ViewState["DefaultValue"];
				}
				return "";
			}
			set
			{
				this.ViewState["DefaultValue"] = value;
			}
		}

		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return this._isTracking;
			}
		}

		[DefaultValue(""), Category("Behavior"), Description("The separator for thousands, defaults to localization values."), NotifyParentProperty(true)]
		public string ThousandsSeparator
		{
			get
			{
				if(this.ViewState["ThousandsSeparator"] != null)
				{
					return (string)this.ViewState["ThousandsSeparator"];
				}
				return "";
			}
			set
			{
				this.ViewState["ThousandsSeparator"] = value;
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
