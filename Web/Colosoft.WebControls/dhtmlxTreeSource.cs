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
using System.Xml;
using System.IO;

namespace Colosoft.WebControls.dhtmlx
{
	/// <summary>
	/// Representa o método acionado quando for solicitado os itens relacionados ao
	/// item com o ID passado.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="id">Identificador do item pai.</param>
	/// <returns>Lista dos itens as serem carregados.</returns>
	public delegate IList<dhtmlxTreeItem> LoadingTreeItems (object sender, string id);
	public class dhtmlxTreeSource
	{
		private string _ID;

		private string _imagePath;

		private bool _enabledCheckBox;

		private bool _enabledDragAndDrop;

		/// <summary>
		/// Lista dos itens da Tree.
		/// </summary>
		private List<dhtmlxTreeItem> _items = new List<dhtmlxTreeItem>();

		public event LoadingTreeItems LoadingItems;

		public string ID
		{
			get
			{
				return _ID;
			}
			set
			{
				_ID = value;
			}
		}

		public string ImagePath
		{
			get
			{
				return _imagePath;
			}
			set
			{
				_imagePath = value;
			}
		}

		public bool EnabledCheckBox
		{
			get
			{
				return _enabledCheckBox;
			}
			set
			{
				_enabledCheckBox = value;
			}
		}

		public bool EnabledDragAndDrop
		{
			get
			{
				return _enabledDragAndDrop;
			}
			set
			{
				_enabledDragAndDrop = value;
			}
		}

		/// <summary>
		/// Lista dos itens da Tree.
		/// </summary>
		public List<dhtmlxTreeItem> Items
		{
			get
			{
				return _items;
			}
		}

		public dhtmlxTreeSource()
		{
			_ID = Guid.NewGuid().ToString();
		}

		public dhtmlxTreeSource(string ID)
		{
			_ID = ID;
		}

		/// <summary>
		/// Salva os dados da arvore no stream.
		/// </summary>
		/// <param name="outStream"></param>
		public void SaveSource(Stream outStream)
		{
			XmlDocument doc = new XmlDocument();
			XmlElement tree = doc.CreateElement("tree");
			tree.SetAttribute("id", _ID);
			foreach (dhtmlxTreeItem item in _items)
			{
				item.LoadElement(doc, tree);
			}
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Reset();
			settings.Encoding = Encoding.GetEncoding("iso-8859-1");
			XmlWriter writer = XmlWriter.Create(outStream, settings);
			doc.AppendChild(tree);
			doc.Save(writer);
		}

		public void SaveSource(Stream outStream, string node)
		{
			XmlDocument doc = new XmlDocument();
			XmlElement tree = doc.CreateElement("tree");
			tree.SetAttribute("id", node);
			IList<dhtmlxTreeItem> loadItens = null;
			if(LoadingItems != null)
			{
				loadItens = LoadingItems(this, node);
			}
			if(loadItens != null)
			{
				foreach (dhtmlxTreeItem item in loadItens)
				{
					item.LoadElement(doc, tree);
				}
			}
			doc.AppendChild(tree);
			doc.Save(outStream);
		}
	}
}
