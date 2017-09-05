﻿/* 
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
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web;
using System.Drawing;
using System.Web.UI.Design;

namespace Colosoft.WebControls
{
	/// <summary>
	/// The PropertyGrid is a work written by leppie. http://propertygrid.codeplex.com/
	/// </summary>
	[ToolboxBitmap(typeof(PropertyGrid))]
	[ToolboxData("<{0}:PropertyGrid runat=server></{0}:PropertyGrid>")]
	[Description("The PropertyGrid is a work written by leppie. http://propertygrid.codeplex.com/")]
	[Designer(typeof(PropertyGrid.PropertyGridDesigner))]
	public sealed class PropertyGrid : Control, INamingContainer
	{
		[SupportsPreviewControl(true)]
		internal class PropertyGridDesigner : System.Web.UI.Design.ControlDesigner
		{
			public override string GetDesignTimeHtml()
			{
				PropertyGrid pg = this.Component as PropertyGrid;
				System.IO.StringWriter output = new System.IO.StringWriter();
				try
				{
					pg.OnInit(EventArgs.Empty);
					pg.OnLoad(EventArgs.Empty);
					pg.SelectedObject = pg;
					pg.OnPreRender(EventArgs.Empty);
					HtmlTextWriter w = new HtmlTextWriter(output);
					pg.Render(w);
				}
				catch(Exception ex)
				{
					output.Write(ex.ToString());
				}
				return output.ToString();
			}
		}

		[Browsable(false)]
		public override bool EnableViewState
		{
			get
			{
				return base.EnableViewState;
			}
			set
			{
				base.EnableViewState = false;
			}
		}

		bool xhtmlstrict = true;

		protected override void OnLoad(EventArgs e)
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
			base.OnLoad(e);
			if(!(Site != null && Site.DesignMode))
			{
				Skinny.Manager.Register(this);
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if(Visible || (Site != null && Site.DesignMode))
			{
				if(Site != null && Site.DesignMode)
				{
					writer.Write("<style type='text/css'>@import url('{0}');</style>", Res("Colosoft.WebControls.Resources.PropertyGrid.PropertyGrid.css"));
				}
				bool pad = false;
				if(!(Site != null && Site.DesignMode))
				{
					pad = (Page.Request.Browser.Browser == "IE");
				}
				else
				{
					pad = true;
				}
				writer.Write(@"<div id=""{0}""{1} class=""PG PG_{0}"">", ClientID, pad ? string.Empty : " style='padding-right:2px'");
				RenderChildren(writer);
				writer.Write("</div>");
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			if(!Skinny.Manager.IsCallBack && Visible)
			{
				if(Site != null && Site.DesignMode)
				{
				}
				else
				{
					if(!Page.ClientScript.IsClientScriptBlockRegistered(typeof(PropertyGrid), "PropertyGrid_style" + ClientID))
					{
						Page.ClientScript.RegisterClientScriptBlock(typeof(PropertyGrid), "PropertyGrid_style" + ClientID, GetCSS());
					}
					if(xhtmlstrict)
					{
						LiteralControl head = Page.Controls[0] as LiteralControl;
						if(head != null)
						{
							head.Text = head.Text.Replace("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" >", "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
							head.Text = head.Text.Replace("HEAD", "head");
						}
						registerStylesheet("Colosoft.WebControls.Resources.PropertyGrid.PropertyGrid.css");
					}
					Page.ClientScript.RegisterClientScriptResource(typeof(PropertyGrid), "Colosoft.WebControls.Resources.PropertyGrid.PropertyGrid.js");
					if(!Page.ClientScript.IsClientScriptBlockRegistered(typeof(PropertyGrid), ClientID + "_init"))
					{
						int lh = GetFontHeight(fontfamily, fontsize) + 5;
						Page.ClientScript.RegisterClientScriptBlock(typeof(PropertyGrid), ClientID + "_init", string.Format(@"
<script type=""text/javascript"">
jQuery(function(){{
	jQuery('#{0}').PropertyGrid({{
		selcolor:	'{1}',
		itembgcolor:'{2}',
		width:		'{3}',
		bgcolor:	'{4}',
		headerfgcolor:	'{5}',
		lineheight:	{6},
		fgcolor:	'{7}',
		family:		'{8}',
		fontsize:	'{9}',
		interval:	{10},
		path:		'{11}',
		
		UNFRESH_GIF:'{12}',
		REFRESH_GIF:'{13}',
		HELPOFF_GIF:'{14}',
		HELP_GIF:	'{15}',
		ON_GIF:		'{16}',
		OFF_GIF:	'{17}'}});
}});
</script>", ClientID, CSSColor(selcolor), CSSColor(itembgcolor), width, CSSColor(bgcolor), CSSColor(headercolor), lh, CSSColor(color), fontfamily, fontsize, interval, respath, Res("Colosoft.WebControls.Resources.PropertyGrid.unfresh.gif"), Res("Colosoft.WebControls.Resources.PropertyGrid.refresh.gif"), Res("Colosoft.WebControls.Resources.PropertyGrid.helpoff.gif"), Res("Colosoft.WebControls.Resources.PropertyGrid.help.gif"), Res("Colosoft.WebControls.Resources.PropertyGrid.on.gif"), Res("Colosoft.WebControls.Resources.PropertyGrid.off.gif")));
					}
				}
			}
			base.OnPreRender(e);
		}

		void registerStylesheet(string resourceKey)
		{
			HtmlLink _link = new HtmlLink();
			_link.Href = Page.ClientScript.GetWebResourceUrl(typeof(PropertyGrid), resourceKey);
			_link.Attributes["rel"] = "stylesheet";
			_link.Attributes["text"] = "text/css";
			try
			{
				Page.Header.Controls.Add(_link);
			}
			catch
			{
				Page.ClientScript.RegisterClientScriptBlock(typeof(PropertyGrid), resourceKey, string.Format("<link href=\"{0}\" rel=\"stylesheet\" text=\"text/css\" />", _link.Href));
			}
		}

		[Skinny.Method]
		public string[] GetValues()
		{
			string[] output = new string[proplist.Count];
			for(int i = 0; i < output.Length; i++)
			{
				output[i] = (proplist[i] as PropertyGridItem).PropertyValue;
			}
			return output;
		}

		[Skinny.Method]
		public string[] SetValue(string id, string val)
		{
			if(!ReadOnly)
			{
				PropertyGridItem pgi = properties[ClientID + "_" + id] as PropertyGridItem;
				pgi.PropertyValue = val;
			}
			return GetValues();
		}

		[Skinny.Method]
		public string[] GetDescription(string id)
		{
			PropertyGridItem pgi = properties[ClientID + "_" + id] as PropertyGridItem;
			PropertyDescriptor pd = pgi.Descriptor;
			string[] output = new string[] {
				pd.DisplayName + " : " + pd.PropertyType.Name,
				pd.Description
			};
			return output;
		}

		string fontfamily = "Verdana";

		FontUnit fontsize = new FontUnit("8pt");

		Color bgcolor = Color.Gainsboro;

		Color headercolor = Color.DimGray;

		Color color = Color.Black;

		Color itembgcolor = Color.White;

		Color selcolor = Color.LightSteelBlue;

		int width = 300;

		bool isreadonly = false;

		bool showhelp = false;

		int interval = 3000;

		string respath = "pg/";

		[DefaultValue(3000)]
		[Category("Behavior")]
		public int UpdateInterval
		{
			get
			{
				return interval;
			}
			set
			{
				interval = value;
			}
		}

		[DefaultValue(false)]
		[Category("Appearance")]
		public bool ShowHelp
		{
			get
			{
				return showhelp;
			}
			set
			{
				showhelp = value;
			}
		}

		[DefaultValue(false)]
		[Category("Behavior")]
		public bool ReadOnly
		{
			get
			{
				return isreadonly;
			}
			set
			{
				isreadonly = value;
			}
		}

		[DefaultValue(300)]
		[Category("Appearance")]
		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		[DefaultValue(typeof(Color), "Gainsboro")]
		[Category("Appearance")]
		[TypeConverter(typeof(WebColorConverter))]
		public Color BackgroundColor
		{
			get
			{
				return bgcolor;
			}
			set
			{
				bgcolor = value;
			}
		}

		[DefaultValue(typeof(Color), "DimGray")]
		[Category("Appearance")]
		[TypeConverter(typeof(WebColorConverter))]
		public Color HeaderForeColor
		{
			get
			{
				return headercolor;
			}
			set
			{
				headercolor = value;
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[Category("Appearance")]
		[TypeConverter(typeof(WebColorConverter))]
		public Color ForeColor
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		[DefaultValue(typeof(Color), "White")]
		[Category("Appearance")]
		[TypeConverter(typeof(WebColorConverter))]
		public Color ItemBackgroundColor
		{
			get
			{
				return itembgcolor;
			}
			set
			{
				itembgcolor = value;
			}
		}

		[DefaultValue(typeof(Color), "LightSteelBlue")]
		[Category("Appearance")]
		[TypeConverter(typeof(WebColorConverter))]
		public Color SelectionColor
		{
			get
			{
				return selcolor;
			}
			set
			{
				selcolor = value;
			}
		}

		[DefaultValue("Verdana")]
		[Category("Appearance")]
		[TypeConverter(typeof(FontConverter.FontNameConverter))]
		public string FontFamily
		{
			get
			{
				return fontfamily;
			}
			set
			{
				fontfamily = value;
			}
		}

		[DefaultValue(typeof(FontUnit), "8pt")]
		[Category("Appearance")]
		public FontUnit FontSize
		{
			get
			{
				return fontsize;
			}
			set
			{
				fontsize = value;
			}
		}

		static string CSSColor(Color c)
		{
			return string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
		}

		static int GetFontHeight(string family, FontUnit fontsize)
		{
			GraphicsUnit gu = GraphicsUnit.Point;
			float fv = 8;
			switch(fontsize.Unit.Type)
			{
			case UnitType.Cm:
				gu = GraphicsUnit.Millimeter;
				fv = (float)fontsize.Unit.Value * 10;
				break;
			case UnitType.Inch:
				gu = GraphicsUnit.Inch;
				fv = (float)fontsize.Unit.Value;
				break;
			case UnitType.Mm:
				gu = GraphicsUnit.Millimeter;
				fv = (float)fontsize.Unit.Value;
				break;
			case UnitType.Pica:
				gu = GraphicsUnit.Point;
				fv = (float)fontsize.Unit.Value * 12;
				break;
			case UnitType.Pixel:
				gu = GraphicsUnit.Pixel;
				fv = (float)fontsize.Unit.Value;
				break;
			case UnitType.Point:
				gu = GraphicsUnit.Point;
				fv = (float)fontsize.Unit.Value;
				break;
			case UnitType.Em:
			case UnitType.Ex:
			case UnitType.Percentage:
			default:
				break;
			}
			using (Font fnt = new Font(family, fv, gu))
			{
				int fh = fnt.Height;
				fh += (fh) % 2;
				return fh;
			}
		}

		string GetOperaCSS()
		{
			int fh = GetFontHeight(fontfamily, fontsize);
			int pgwidth = Width;
			string fontfam = fontfamily;
			string fntsize = fontsize.ToString();
			int widthinner = Width - 2;
			int lineheight = fh + 6;
			int padwidth = 18;
			int lineheightmarge = lineheight + 1;
			int widthlesspad = widthinner - padwidth;
			int halfwidth = widthlesspad / 2;
			int halfwidthless3 = halfwidth - 5;
			int inputlineheight = lineheight - 4;
			string bgcol = CSSColor(bgcolor);
			string hdcol = CSSColor(headercolor);
			string frcol = CSSColor(color);
			string itbgcol = CSSColor(itembgcolor);
			string selcol = CSSColor(selcolor);
			return string.Format(@"
<style type=""text/css"">
.PG_{16}
{{
  width:{0}px;
}}
.PG_{16} *
{{
  font-family:{1};
  font-size:{2};
  color:{13};
}}
.PGH_{16}, .PGF_{16}, .PGC_{16}, .PGF2_{16}
{{
  border-color: {12};
  background-color:{11};
}}
.PGC_{16} *
{{
  line-height:{4}px;
  height:{4}px;
}}
.PGC_{16} a, .PGC_OPEN_{16}, .PGC_CLOSED_{16}
{{
  width:{5}px;
}}
.PGC_HEAD_{16} span
{{
  color:{12};
}}
.PGI_NONE_{16}, .PGI_CLOSED_{16}, .PGI_OPEN_{16}
{{
  width:{5}px;
  height:{6}px;
}}
.PGI_NAME_{16}, .PGI_VALUE_{16}, .PGI_NAME_SUB_{16}
{{
  width:{8}px;
  background-color:{14};
}}
.PGI_VALUE_{16} a, .PGI_VALUE_{16} select
{{
  width:100%;
}}
.PGI_NAME_SUB_{16} span
{{
  margin-left:{5}px;
}}
.PGI_VALUE_{16} a:hover
{{
  background-color:{15};
}}
.PGI_VALUE_{16} input
{{
  width:{9}px;
  line-height:{10}px;
  height:{10}px;
}}
</style>", pgwidth, fontfam, fontsize, widthinner, lineheight, padwidth, lineheightmarge, widthlesspad, halfwidth, halfwidthless3, inputlineheight, bgcol, hdcol, frcol, itbgcol, selcol, ClientID);
		}

		string GetCSS()
		{
			if(Page.Request.Browser.Browser == "Opera")
			{
				return GetOperaCSS();
			}
			return string.Empty;
		}

		abstract class GridControl : Control
		{
			PropertyGrid parentgrid;

			public PropertyGrid ParentGrid
			{
				get
				{
					if(null == parentgrid)
					{
						Control p = Parent;
						while (!(p is PropertyGrid))
						{
							p = p.Parent;
						}
						parentgrid = (PropertyGrid)p;
					}
					return parentgrid;
				}
			}

			protected string Res(string resourceKey)
			{
				return ParentGrid.Res(resourceKey);
			}
		}

		class PropertyGridHeader : GridControl
		{
			protected override void Render(HtmlTextWriter writer)
			{
				ID = "active";
				writer.Write(@"
<div class=""PGH PGH_{0}"">
	<img class=""PGH_L"" src=""{1}"" title=""{6}""	alt=""LIVE""	style=""margin-left:2px"" />
	<img class=""PGH_L"" src=""{2}"" title=""{7}""		alt=""REFRESH""/>
	<img class=""PGH_R"" src=""{3}"" title=""{8}""	alt=""UP""	style=""margin-right:2px"" />
	<img class=""PGH_R"" src=""{4}"" title=""{9}""	alt=""DOWN""/>
	<img class=""PGH_R"" src=""{5}"" title=""{10}""	alt=""HELP""/>  
</div>", Parent.ClientID, Res("Colosoft.WebControls.Resources.PropertyGrid.off.gif"), Res("Colosoft.WebControls.Resources.PropertyGrid.refresh.gif"), Res("Colosoft.WebControls.Resources.PropertyGrid.expand.gif"), Res("Colosoft.WebControls.Resources.PropertyGrid.collapse.gif"), ParentGrid.ShowHelp ? Res("Colosoft.WebControls.Resources.PropertyGrid.help.gif") : Res("Colosoft.WebControls.Resources.PropertyGrid.helpoff.gif"), Properties.Resources.propertygrid_title_LiveMode, Properties.Resources.propertygrid_title_Refresh, Properties.Resources.propertygrid_title_ExpandAll, Properties.Resources.propertygrid_title_CollapseAll, Properties.Resources.propertygrid_title_ToggleHelp);
			}
		}

		class PropertyGridFooter : GridControl
		{
			protected override void OnLoad(EventArgs e)
			{
				base.OnLoad(e);
				ID = "foot";
			}

			protected override void Render(HtmlTextWriter writer)
			{
				writer.Write(@"<div class=""PGF PGF_{2}""{1}><span id=""{0}""></span></div>", ClientID, ParentGrid.ShowHelp ? string.Empty : "style='display:none'", ParentGrid.ClientID);
				writer.Write(@"
<div	class=""PGF2 PGF2_{1}"">
	<span	style=""float:right;margin-right:1px""><img
				id=""{1}_active""
				src='{3}'
				style=""display:none""
				title=""Busy...""
				alt=""BUSY""/>{0}</span>
	<span	style=""float:left;margin-left:2px""></span>
</div>", ParentGrid.ReadOnly ? string.Format(@"<img	src='{0}'
											title='Values cannot be modified'
											alt='LOCK' />", Res("Colosoft.WebControls.Resources.PropertyGrid.lock.gif")) : string.Empty, Parent.ClientID, ParentGrid.respath, Res("Colosoft.WebControls.Resources.PropertyGrid.active.gif"), Res("Colosoft.WebControls.Resources.PropertyGrid.info.gif"));
			}
		}

		class PropertyGridCategory : GridControl
		{
			string catname;

			public string CategoryName
			{
				get
				{
					return catname;
				}
				set
				{
					catname = value;
				}
			}

			protected override void Render(HtmlTextWriter writer)
			{
				writer.Write(@"
<div class=""PGC PGC_{2}"">
<div class=""PGC_OPEN PGC_OPEN_{2}""></div>
<div class=""PGC_HEAD PGC_HEAD_{2}""><span><a href='#cat{2}'>{0}</a></span></div>
<div id=""{1}"" class=""PGC_WRAP"">", CategoryName, ClientID, ParentGrid.ClientID);
				RenderChildren(writer);
				writer.Write(@"</div></div>");
			}

			protected override void OnLoad(EventArgs e)
			{
				base.OnLoad(e);
				ID = "cat" + ParentGrid.catcounter++;
			}
		}

		class PropertyGridSubItem : PropertyGridItem
		{
			PropertyGridItem parentitem;

			public PropertyGridSubItem(PropertyDescriptor pd, PropertyGridItem parentitem) : base(pd)
			{
				this.parentitem = parentitem;
			}

			public PropertyDescriptor ParentDescriptor
			{
				get
				{
					return parentitem.Descriptor;
				}
			}

			public override object SelectedObject
			{
				get
				{
					return parentitem.Descriptor.GetValue(base.SelectedObject);
				}
			}

			public PropertyGridItem ParentItem
			{
				get
				{
					return parentitem;
				}
			}

			protected override bool IsSubItem
			{
				get
				{
					return true;
				}
			}
		}

		class SubItems : Control
		{
			protected override void Render(HtmlTextWriter writer)
			{
				writer.Write(@"<div id=""{0}"" style=""display:none"">", Parent.ClientID);
				RenderChildren(writer);
				writer.Write("</div>");
			}
		}

		class PropertyGridItem : GridControl
		{
			readonly PropertyDescriptor propdesc;

			internal string controlid;

			public PropertyGridItem(PropertyDescriptor propdesc)
			{
				this.propdesc = propdesc;
			}

			protected bool HasSubItems
			{
				get
				{
					return subitems.Count > 0;
				}
			}

			protected bool IsParentItem
			{
				get
				{
					return !IsSubItem;
				}
			}

			protected virtual bool IsSubItem
			{
				get
				{
					return false;
				}
			}

			internal ArrayList subitems = new ArrayList();

			protected override void OnLoad(EventArgs e)
			{
				base.OnLoad(e);
				if(IsParentItem)
				{
					if(HasSubItems)
					{
						ID = "sub" + ParentGrid.subcounter++;
					}
				}
			}

			void RenderEditor(HtmlTextWriter writer)
			{
				if(propdesc.IsReadOnly || ParentGrid.ReadOnly)
				{
					writer.Write(@"<span title=""{1}""><span id=""{0}"" style=""color:gray"">{1}</span></span>", controlid, PropertyValue);
				}
				else
				{
					TypeConverter tc = propdesc.Converter;
					if(tc.GetStandardValuesSupported())
					{
						string pv = PropertyValue;
						writer.Write(@"<a href=""#""" + @" title=""Click to edit""><span id=""{0}"">{1}</span></a>", controlid, pv, ParentGrid.ClientID);
						writer.Write(@"<select style=""display:none"">", ParentGrid.ClientID);
						foreach (object si in tc.GetStandardValues())
						{
							string val = tc.ConvertToString(si);
							if(val == pv)
							{
								writer.Write(@"<option selected=""selected"">{0}</option>", val);
							}
							else
							{
								writer.Write(@"<option>{0}</option>", val);
							}
						}
						writer.Write("</select>");
					}
					else
					{
						if(tc.CanConvertFrom(typeof(string)))
						{
							writer.Write(@"<a href=""#""" + @" title=""Click to edit""><span id=""{0}"">{1}</span></a>", controlid, PropertyValue, ParentGrid.ClientID);
							writer.Write(@"<input style=""display:none"" type=""text"" />", ParentGrid.ClientID);
						}
						else
						{
							writer.Write(@"<span title=""{1}""><span id=""{0}"" style=""color:gray"">{1}</span></span>", controlid, PropertyValue);
						}
					}
				}
			}

			protected override void Render(HtmlTextWriter writer)
			{
				writer.Write(@"
<div class=""PGI PGI_{4}"">
<div class=""PGI_{1} PGI_{1}_{4}""{3}></div>
<div class=""PGI_NAME{2} PGI_NAME{2}_{4}""" + @" title=""Click for help""><span>{0}</span></div><div class=""PGI_VALUE PGI_VALUE_{4}"">", propdesc.DisplayName, !HasSubItems ? "NONE" : "CLOSED", IsSubItem ? "_SUB" : string.Empty, !HasSubItems ? string.Empty : string.Format(@" ", ClientID), ParentGrid.ClientID);
				RenderEditor(writer);
				writer.Write("</div></div>");
				if(IsParentItem)
				{
					RenderChildren(writer);
				}
			}

			public string Name
			{
				get
				{
					return propdesc.Name;
				}
			}

			public PropertyDescriptor Descriptor
			{
				get
				{
					return propdesc;
				}
			}

			public string PropertyValue
			{
				get
				{
					if(propdesc.Converter.CanConvertTo(typeof(string)))
					{
						return propdesc.Converter.ConvertToString(propdesc.GetValue(SelectedObject));
					}
					else
					{
						return propdesc.GetValue(SelectedObject).ToString();
					}
				}
				set
				{
					object so = SelectedObject;
					object val = propdesc.Converter.ConvertFromString(value);
					propdesc.SetValue(so, val);
					if(IsSubItem)
					{
						PropertyGridItem parent = ((PropertyGridSubItem)this).ParentItem;
						parent.Descriptor.SetValue(parent.SelectedObject, so);
					}
					else
					{
						ParentGrid.CreateGrid();
					}
				}
			}

			public virtual object SelectedObject
			{
				get
				{
					return ParentGrid.SelectedObject;
				}
			}
		}

		object selobj;

		[Browsable(false)]
		public object SelectedObject
		{
			get
			{
				return selobj;
			}
			set
			{
				if(value != selobj)
				{
					selobj = value;
					CreateGrid();
				}
			}
		}

		ArrayList proplist = new ArrayList();

		Hashtable properties = new Hashtable();

		ArrayList catlist = new ArrayList();

		int catcounter = 0;

		int subcounter = 0;

		int itemcounter = 0;

		void CreateGrid()
		{
			if(selobj == null)
			{
				return;
			}
			Controls.Clear();
			properties.Clear();
			proplist.Clear();
			itemcounter = catcounter = subcounter = 0;
			Controls.Add(new PropertyGridHeader());
			Hashtable cats = new Hashtable();
			foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(selobj))
			{
				if(!pd.IsBrowsable)
				{
					continue;
				}
				string cat = pd.Category;
				Hashtable mems = cats[cat] as Hashtable;
				if(mems == null)
				{
					cats[cat] = mems = new Hashtable();
				}
				try
				{
					PropertyGridItem pgi = new PropertyGridItem(pd);
					pgi.controlid = ClientID + "_" + itemcounter++;
					properties[pgi.controlid] = pgi;
					object o = selobj;
					object subo = null;
					try
					{
						subo = pd.GetValue(o);
					}
					catch
					{
					}
					if(pd.Converter.GetPropertiesSupported())
					{
						foreach (PropertyDescriptor spd in pd.Converter.GetProperties(subo))
						{
							if(spd.IsBrowsable)
							{
								PropertyGridItem pgsi = new PropertyGridSubItem(spd, pgi);
								pgsi.controlid = ClientID + "_" + itemcounter++;
								pgi.subitems.Add(pgsi);
								properties[pgsi.controlid] = pgsi;
							}
						}
					}
					mems.Add(pd.Name, pgi);
				}
				catch(Exception ex)
				{
					Page.Response.Write(ex);
				}
			}
			this.catlist.Clear();
			ArrayList catlist = new ArrayList(cats.Keys);
			catlist.Sort();
			System.Web.UI.HtmlControls.HtmlContainerControl cc = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
			cc.ID = "cats";
			Controls.Add(cc);
			foreach (string cat in catlist)
			{
				PropertyGridCategory pgc = new PropertyGridCategory();
				pgc.CategoryName = cat;
				this.catlist.Add(pgc);
				cc.Controls.Add(pgc);
				Hashtable i = cats[cat] as Hashtable;
				ArrayList il = new ArrayList(i.Keys);
				il.Sort();
				foreach (string pginame in il)
				{
					PropertyGridItem pgi = i[pginame] as PropertyGridItem;
					proplist.Add(pgi);
					pgc.Controls.Add(pgi);
					if(pgi.subitems.Count > 0)
					{
						SubItems si = new SubItems();
						pgi.Controls.Add(si);
						foreach (PropertyGridItem spgi in pgi.subitems)
						{
							si.Controls.Add(spgi);
							proplist.Add(spgi);
						}
					}
				}
			}
			Controls.Add(new PropertyGridFooter());
		}

		private string Res(string resourceKey)
		{
			return Page.ClientScript.GetWebResourceUrl(typeof(PropertyGrid), resourceKey);
		}
	}
}
