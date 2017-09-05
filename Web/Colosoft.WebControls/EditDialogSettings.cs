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
using System.Web.UI;
using System.Web;
using System.Security.Permissions;

namespace Colosoft.WebControls.GridView
{
	[TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public sealed class EditDialogSettings : IStateManager
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

		[Category("Appearance"), NotifyParentProperty(true), Description("The text label of the 'Cancel' button in search dialog. Defaults to current localization settings, setting it will override localization default."), DefaultValue("")]
		public string CancelText
		{
			get
			{
				object obj2 = this.ViewState["CancelText"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["CancelText"] = value;
			}
		}

		[NotifyParentProperty(true), DefaultValue(""), Category("Appearance"), Description("The caption of the dialog. Defaults to localization settings.")]
		public string Caption
		{
			get
			{
				object obj2 = this.ViewState["Caption"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["Caption"] = value;
			}
		}

		[NotifyParentProperty(true), DefaultValue(false), Category("Appearance"), Description("Determines whether the edit dialog will be closed automatically after editing has been done or it will stay open. Default is false.")]
		public bool CloseAfterEditing
		{
			get
			{
				object obj2 = this.ViewState["CloseAfterEditing"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["CloseAfterEditing"] = value;
			}
		}

		[Description("Determines if the dialog window should be draggable. Default is true."), NotifyParentProperty(true), DefaultValue(true), Category("Appearance")]
		public bool Draggable
		{
			get
			{
				object obj2 = this.ViewState["Draggable"];
				return ((obj2 == null) || ((bool)obj2));
			}
			set
			{
				this.ViewState["Draggable"] = value;
			}
		}

		[NotifyParentProperty(true), Description("The height of the edit dialog. Default is 300. Accepts only integer numbers."), DefaultValue(300), Category("Appearance")]
		public int Height
		{
			get
			{
				object obj2 = this.ViewState["Height"];
				if(obj2 == null)
				{
					return 300;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["Height"] = value;
			}
		}

		[DefaultValue(0), Category("Appearance"), NotifyParentProperty(true), Description("The left (X) offset of the dialog window, relative to the grid upper left corner. Accepts negative values. ")]
		public int LeftOffset
		{
			get
			{
				object obj2 = this.ViewState["LeftOffset"];
				if(obj2 == null)
				{
					return 0;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["LeftOffset"] = value;
			}
		}

		[DefaultValue(""), Category("Appearance"), Description("The text message the will be displayed while server is updated with new edit values. Defaults to current localization settings, setting it will override localization default."), NotifyParentProperty(true)]
		public string LoadingMessageText
		{
			get
			{
				object obj2 = this.ViewState["LoadingMessageText"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["LoadingMessageText"] = value;
			}
		}

		[DefaultValue(false), NotifyParentProperty(true), Description("Determines if the dialog should be modal or not. Default is false."), Category("Appearance")]
		public bool Modal
		{
			get
			{
				object obj2 = this.ViewState["Modal"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["Modal"] = value;
			}
		}

		[DefaultValue(true), Description("Determines if the grid will auto-reload its contents after editing. True by default."), NotifyParentProperty(true), Category("Appearance")]
		public bool ReloadAfterSubmit
		{
			get
			{
				object obj2 = this.ViewState["ReloadAfterSubmit"];
				return ((obj2 == null) || ((bool)obj2));
			}
			set
			{
				this.ViewState["ReloadAfterSubmit"] = value;
			}
		}

		[NotifyParentProperty(true), Description("Determines if the dialog window should be resizable. Default is true."), DefaultValue(true), Category("Appearance")]
		public bool Resizable
		{
			get
			{
				object obj2 = this.ViewState["Resizable"];
				return ((obj2 == null) || ((bool)obj2));
			}
			set
			{
				this.ViewState["Resizable"] = value;
			}
		}

		[Description("The text label of the 'Submit' button in search dialog. Defaults to current localization settings, setting it will override localization default."), NotifyParentProperty(true), DefaultValue(""), Category("Appearance")]
		public string SubmitText
		{
			get
			{
				object obj2 = this.ViewState["SubmitText"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["SubmitText"] = value;
			}
		}

		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return this._isTracking;
			}
		}

		[NotifyParentProperty(true), DefaultValue(0), Category("Appearance"), Description("The top (Y) offset of the dialog window, relative to the grid upper left corner. Accepts negative values. ")]
		public int TopOffset
		{
			get
			{
				object obj2 = this.ViewState["TopOffset"];
				if(obj2 == null)
				{
					return 0;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["TopOffset"] = value;
			}
		}

		private StateBag ViewState
		{
			get
			{
				return this._viewState;
			}
		}

		[Description("The width of the edit dialog. Default is 300. Accepts only integer numbers."), DefaultValue(300), Category("Appearance"), NotifyParentProperty(true)]
		public int Width
		{
			get
			{
				object obj2 = this.ViewState["Width"];
				if(obj2 == null)
				{
					return 300;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["Width"] = value;
			}
		}
	}
}
