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
	[TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public sealed class AddDialogSettings : IStateManager
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

		[Category("Appearance"), DefaultValue(""), NotifyParentProperty(true), Description("The text label of the 'Cancel' button in the dialog. Defaults to current localization settings, setting it will override localization default.")]
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

		[DefaultValue(""), Category("Appearance"), Description("The caption of the dialog. Defaults to localization settings."), NotifyParentProperty(true)]
		public string Caption
		{
			get
			{
				object obj2 = this.ViewState["CaptionWhenAdding"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["CaptionWhenAdding"] = value;
			}
		}

		[Category("Appearance"), DefaultValue(true), NotifyParentProperty(true), Description("Determines if the add dialog values should be cleared after adding. Default is true.")]
		public bool ClearAfterAdding
		{
			get
			{
				object obj2 = this.ViewState["ClearAfterAdding"];
				return ((obj2 == null) || ((bool)obj2));
			}
			set
			{
				this.ViewState["ClearAfterAdding"] = value;
			}
		}

		[NotifyParentProperty(true), DefaultValue(false), Category("Appearance"), Description("Determines if the dialog should be auto-closed after adding. Default is false.")]
		public bool CloseAfterAdding
		{
			get
			{
				object obj2 = this.ViewState["CloseAfterAdding"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["CloseAfterAdding"] = value;
			}
		}

		[NotifyParentProperty(true), Description("Determines if the dialog window should be draggable. Default is true."), DefaultValue(true), Category("Appearance")]
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

		[DefaultValue(300), Description("The height of the add dialog. Default is 300. Accepts only integer numbers."), Category("Appearance"), NotifyParentProperty(true)]
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

		[Category("Appearance"), Description("The left (X) offset of the dialog window, relative to the grid upper left corner. Accepts negative values. "), NotifyParentProperty(true), DefaultValue(0)]
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

		[NotifyParentProperty(true), DefaultValue(""), Category("Appearance"), Description("The text message the will be displayed while server is updated with new values. Defaults to current localization settings, setting it will override localization default.")]
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

		[NotifyParentProperty(true), DefaultValue(true), Category("Appearance"), Description("Determines if the grid should be reloaded immediately after adding the values. Default is true.")]
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

		[Description("Determines if the dialog window should be resizable. Default is true."), DefaultValue(true), Category("Appearance"), NotifyParentProperty(true)]
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

		[DefaultValue(""), NotifyParentProperty(true), Category("Appearance"), Description("The text label of the 'Submit' button in the dialog. Defaults to current localization settings, setting it will override localization default.")]
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

		[Description("The top (Y) offset of the dialog window, relative to the grid upper left corner. Accepts negative values. "), Category("Appearance"), NotifyParentProperty(true), DefaultValue(0)]
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

		[NotifyParentProperty(true), Description("The width of the add dialog. Default is 300. Accepts only integer numbers."), DefaultValue(300), Category("Appearance")]
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
