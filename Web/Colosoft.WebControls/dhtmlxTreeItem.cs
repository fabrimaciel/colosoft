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
using System.Web.UI.WebControls;
using System.Xml;

namespace Colosoft.WebControls.dhtmlx
{
	public class dhtmlxTreeItem
	{
		private string _text;

		private string _id;

		private string _tooltip;

		private Image _nodeWithoutChildren;

		private Image _openedNode;

		private Image _closedNote;

		private System.Drawing.Color _colorNotSelectedItem;

		private System.Drawing.Color _colorSelectedItem;

		private bool _selected;

		private Style _style;

		private bool _opened;

		private string _scriptCall;

		private bool _checked;

		private int _imageHeight;

		private int _imageWidth;

		private int _topOffSet;

		private List<dhtmlxTreeItem> _items = new List<dhtmlxTreeItem>();

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		public string ID
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public string Tooltip
		{
			get
			{
				return _tooltip;
			}
			set
			{
				_tooltip = value;
			}
		}

		public Image NodeWithoutChildren
		{
			get
			{
				return _nodeWithoutChildren;
			}
			set
			{
				_nodeWithoutChildren = value;
			}
		}

		public Image OpenedNode
		{
			get
			{
				return _openedNode;
			}
			set
			{
				_openedNode = value;
			}
		}

		public Image ClosedNote
		{
			get
			{
				return _closedNote;
			}
			set
			{
				_closedNote = value;
			}
		}

		public System.Drawing.Color ColorNotSelectedItem
		{
			get
			{
				return _colorNotSelectedItem;
			}
			set
			{
				_colorNotSelectedItem = value;
			}
		}

		public System.Drawing.Color ColorSelectedItem
		{
			get
			{
				return _colorSelectedItem;
			}
			set
			{
				_colorSelectedItem = value;
			}
		}

		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected = value;
			}
		}

		public Style Style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
			}
		}

		public bool Opened
		{
			get
			{
				return _opened;
			}
			set
			{
				_opened = value;
			}
		}

		public string ScritpCall
		{
			get
			{
				return _scriptCall;
			}
			set
			{
				_scriptCall = value;
			}
		}

		public bool Checked
		{
			get
			{
				return _checked;
			}
			set
			{
				_checked = value;
			}
		}

		public int ImageHeight
		{
			get
			{
				return _imageHeight;
			}
			set
			{
				_imageHeight = value;
			}
		}

		public int ImageWidth
		{
			get
			{
				return _imageWidth;
			}
			set
			{
				_imageWidth = value;
			}
		}

		public int TopOffSet
		{
			get
			{
				return _topOffSet;
			}
			set
			{
				_topOffSet = value;
			}
		}

		public List<dhtmlxTreeItem> Items
		{
			get
			{
				return _items;
			}
		}

		public dhtmlxTreeItem()
		{
			_id = Guid.NewGuid().ToString();
		}

		public dhtmlxTreeItem(string text) : this()
		{
			_text = text;
		}

		public dhtmlxTreeItem(string ID, string text) : this(text)
		{
			_id = ID;
		}

		internal void LoadElement(XmlDocument doc, XmlElement element)
		{
			XmlElement elem = doc.CreateElement("item");
			LoadElement(elem, (_items.Count > 0));
			foreach (dhtmlxTreeItem item in _items)
			{
				item.ID = this.ID + "!" + item.ID;
				item.LoadElement(doc, elem);
			}
			element.AppendChild(elem);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		internal void LoadElement(XmlElement element, bool isChild)
		{
			if(_text != null && _text != "")
				element.SetAttribute("text", _text);
			element.SetAttribute("id", _id);
			element.SetAttribute("child", "1");
			if(_tooltip != null && _tooltip != "")
				element.SetAttribute("tooltip", _tooltip);
			if(_nodeWithoutChildren != null)
				element.SetAttribute("img0", _nodeWithoutChildren.Page.ResolveUrl(_nodeWithoutChildren.ImageUrl));
			if(_openedNode != null)
				element.SetAttribute("img1", _openedNode.Page.ResolveUrl(_openedNode.ImageUrl));
			if(_closedNote != null)
				element.SetAttribute("img2", _closedNote.Page.ResolveUrl(_closedNote.ImageUrl));
			if(_colorNotSelectedItem != System.Drawing.Color.Empty)
				element.SetAttribute("aCol", System.Drawing.ColorTranslator.ToHtml(_colorNotSelectedItem));
			if(_colorSelectedItem != System.Drawing.Color.Empty)
				element.SetAttribute("sCol", System.Drawing.ColorTranslator.ToHtml(_colorSelectedItem));
			if(_selected)
				element.SetAttribute("select", "1");
			if(_opened)
				element.SetAttribute("opened", "1");
			if(_scriptCall != null && _scriptCall != "")
				element.SetAttribute("call", _scriptCall);
			if(_checked)
				element.SetAttribute("checked", "1");
			if(isChild)
				element.SetAttribute("child", "1");
			if(_nodeWithoutChildren != null || _openedNode != null || _closedNote != null)
			{
				element.SetAttribute("imheight", _imageHeight.ToString());
				element.SetAttribute("imwidth", _imageWidth.ToString());
			}
			if(_topOffSet != 0)
				element.SetAttribute("topoffset", _topOffSet.ToString());
		}
	}
}
