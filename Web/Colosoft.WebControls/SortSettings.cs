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
	/// <summary>
	/// Representa as configuração da ordenação do Grid.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public sealed class SortSettings : IStateManager
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

		public override string ToString()
		{
			return string.Empty;
		}

		[NotifyParentProperty(true), DefaultValue("The column (datafield) to be used as a default sorting column on initial page load."), Category("Behavior"), Description("")]
		public string InitialSortColumn
		{
			get
			{
				if(this.ViewState["InitialSortColumn"] != null)
				{
					return (string)this.ViewState["InitialSortColumn"];
				}
				return "";
			}
			set
			{
				this.ViewState["InitialSortColumn"] = value;
			}
		}

		[Description("The sort direction to be used on initial page load. Worksonly if InitialSortColumn is set. Asc (ascending) by default."), Category("Behavior"), NotifyParentProperty(true), DefaultValue(0)]
		public SortDirection InitialSortDirection
		{
			get
			{
				if(this.ViewState["InitialSortDirection"] != null)
				{
					return (SortDirection)this.ViewState["InitialSortDirection"];
				}
				return SortDirection.Asc;
			}
			set
			{
				this.ViewState["InitialSortDirection"] = value;
			}
		}

		[Description("Determines when the sorting action takes place. Default is ClickOnHeader"), NotifyParentProperty(true), DefaultValue(0), Category("Behavior")]
		public SortAction SortAction
		{
			get
			{
				object obj2 = this.ViewState["SortAction"];
				if(obj2 == null)
				{
					return SortAction.ClickOnHeader;
				}
				return (SortAction)obj2;
			}
			set
			{
				this.ViewState["SortAction"] = value;
			}
		}

		[Category("Behavior"), Description("The visual appearance of sorting icons in the column header. Default is Vertical."), DefaultValue(0), NotifyParentProperty(true)]
		public SortIconsPosition SortIconsPosition
		{
			get
			{
				object obj2 = this.ViewState["SortIconsPosition"];
				if(obj2 == null)
				{
					return SortIconsPosition.Vertical;
				}
				return (SortIconsPosition)obj2;
			}
			set
			{
				this.ViewState["SortIconsPosition"] = value;
			}
		}

		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return this._isTracking;
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
