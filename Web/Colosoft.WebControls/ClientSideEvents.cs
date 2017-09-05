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
using System.ComponentModel;
using System.Web;
using System.Security.Permissions;

namespace Colosoft.WebControls.GridView
{
	[TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public sealed class ClientSideEvents : IStateManager
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

		[NotifyParentProperty(true), DefaultValue(""), Category("Appearance"), Description("PagerSettings_FirstPageImageUrl")]
		public string AfterAddDialogRowInserted
		{
			get
			{
				object obj2 = this.ViewState["AfterAddDialogRowInserted"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["AfterAddDialogRowInserted"] = value;
			}
		}

		[DefaultValue(""), NotifyParentProperty(true), Category("Appearance"), Description("PagerSettings_FirstPageImageUrl")]
		public string AfterAddDialogShown
		{
			get
			{
				object obj2 = this.ViewState["AfterAddDialogShown"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["AfterAddDialogShown"] = value;
			}
		}

		[NotifyParentProperty(true), Description("PagerSettings_FirstPageImageUrl"), DefaultValue(""), Category("Appearance")]
		public string AfterEditDialogRowInserted
		{
			get
			{
				object obj2 = this.ViewState["AfterEditDialogRowInserted"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["AfterEditDialogRowInserted"] = value;
			}
		}

		[DefaultValue(""), NotifyParentProperty(true), Description("PagerSettings_FirstPageImageUrl"), Category("Appearance")]
		public string AfterEditDialogShown
		{
			get
			{
				object obj2 = this.ViewState["AfterEditDialogShown"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["AfterEditDialogShown"] = value;
			}
		}

		[DefaultValue(""), Description("PagerSettings_FirstPageImageUrl"), NotifyParentProperty(true), Category("Appearance")]
		public string BeforeAjaxRequest
		{
			get
			{
				object obj2 = this.ViewState["BeforeAjaxRequest"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["BeforeAjaxRequest"] = value;
			}
		}

		[NotifyParentProperty(true), DefaultValue(""), Category("Appearance"), Description("PagerSettings_FirstPageImageUrl")]
		public string ColumnSort
		{
			get
			{
				object obj2 = this.ViewState["ColumnSort"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["ColumnSort"] = value;
			}
		}

		[NotifyParentProperty(true), DefaultValue(""), Category("Appearance"), Description("PagerSettings_FirstPageImageUrl")]
		public string GridInitialized
		{
			get
			{
				object obj2 = this.ViewState["GridInitialized"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["GridInitialized"] = value;
			}
		}

		[NotifyParentProperty(true), Description("PagerSettings_FirstPageImageUrl"), DefaultValue(""), Category("Appearance")]
		public string LoadDataError
		{
			get
			{
				object obj2 = this.ViewState["OnClientLoadDataError"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["OnClientLoadDataError"] = value;
			}
		}

		[NotifyParentProperty(true), DefaultValue(""), Category("Appearance"), Description("PagerSettings_FirstPageImageUrl")]
		public string RowDoubleClick
		{
			get
			{
				object obj2 = this.ViewState["RowDoubleClick"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["RowDoubleClick"] = value;
			}
		}

		[DefaultValue(""), NotifyParentProperty(true), Category("Appearance"), Description("PagerSettings_FirstPageImageUrl")]
		public string RowRightClick
		{
			get
			{
				object obj2 = this.ViewState["RowRightClick"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["RowRightClick"] = value;
			}
		}

		[Description("PagerSettings_FirstPageImageUrl"), NotifyParentProperty(true), DefaultValue(""), Category("Appearance")]
		public string RowSelect
		{
			get
			{
				object obj2 = this.ViewState["OnClientRowSelect"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["OnClientRowSelect"] = value;
			}
		}

		[DefaultValue(""), NotifyParentProperty(true), Description("PagerSettings_FirstPageImageUrl"), Category("Appearance")]
		public string ServerError
		{
			get
			{
				object obj2 = this.ViewState["OnClientServerError"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["OnClientServerError"] = value;
			}
		}

		[Category("Appearance"), Description("Called upon expanding the grid hierarchy (click on the plus sign)."), NotifyParentProperty(true), DefaultValue("")]
		public string SubGridRowExpanded
		{
			get
			{
				object obj2 = this.ViewState["SubGridRowExpanded"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["SubGridRowExpanded"] = value;
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
