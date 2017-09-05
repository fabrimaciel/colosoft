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
	[TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public sealed class ToolBarSettings : IStateManager
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

		[Description("Gets or sets the visibility of the edit button (plus) in the grid toolbar."), NotifyParentProperty(true), DefaultValue(false), Category("Appearance")]
		public bool ShowAddButton
		{
			get
			{
				object obj2 = this.ViewState["ShowAddButton"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ShowAddButton"] = value;
			}
		}

		[NotifyParentProperty(true), Category("Appearance"), Description("Gets or sets the visibility of the delete button (recycle bin) in the grid toolbar."), DefaultValue(false)]
		public bool ShowDeleteButton
		{
			get
			{
				object obj2 = this.ViewState["ShowDeleteButton"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ShowDeleteButton"] = value;
			}
		}

		[Category("Appearance"), NotifyParentProperty(true), DefaultValue(false), Description("Gets or sets the visibility of the edit button (pencil) in the grid toolbar.")]
		public bool ShowEditButton
		{
			get
			{
				object obj2 = this.ViewState["ShowEditButton"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ShowEditButton"] = value;
			}
		}

		[DefaultValue(false), NotifyParentProperty(true), Description("Gets or sets the visibility of the search button (circle) in the grid toolbar."), Category("Appearance")]
		public bool ShowRefreshButton
		{
			get
			{
				object obj2 = this.ViewState["ShowRefreshButton"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ShowRefreshButton"] = value;
			}
		}

		[Category("Appearance"), Description("Gets or sets the visibility of the search button (magnifying glass) in the grid toolbar."), NotifyParentProperty(true), DefaultValue(false)]
		public bool ShowSearchButton
		{
			get
			{
				object obj2 = this.ViewState["ShowSearchButton"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ShowSearchButton"] = value;
			}
		}

		[NotifyParentProperty(true), DefaultValue(false), Category("Appearance"), Description("Determines if the grid search/filter toolbar should be shown on top of the grid.")]
		public bool ShowSearchToolBar
		{
			get
			{
				object obj2 = this.ViewState["ShowSearchToolBar"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ShowSearchToolBar"] = value;
			}
		}

		[NotifyParentProperty(true), DefaultValue(false), Category("Appearance"), Description("Gets or sets the visibility of the view row details button in the grid toolbar.")]
		public bool ShowViewRowDetailsButton
		{
			get
			{
				object obj2 = this.ViewState["ShowViewRowDetailsButton"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ShowViewRowDetailsButton"] = value;
			}
		}

		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return this._isTracking;
			}
		}

		[NotifyParentProperty(true), DefaultValue(0), Category("Appearance"), Description("Alignment of the toolbar - left, center, right.")]
		public ToolBarAlign ToolBarAlign
		{
			get
			{
				if(this.ViewState["ToolBarAlign"] != null)
				{
					return (ToolBarAlign)this.ViewState["ToolBarAlign"];
				}
				return ToolBarAlign.Left;
			}
			set
			{
				this.ViewState["ToolBarAlign"] = value;
			}
		}

		[Description("Top, Bottom, TopAndBottom"), DefaultValue(1), Category("Appearance"), NotifyParentProperty(true)]
		public ToolBarPosition ToolBarPosition
		{
			get
			{
				if(this.ViewState["ToolBarPosition"] != null)
				{
					return (ToolBarPosition)this.ViewState["ToolBarPosition"];
				}
				return ToolBarPosition.Bottom;
			}
			set
			{
				this.ViewState["ToolBarPosition"] = value;
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
