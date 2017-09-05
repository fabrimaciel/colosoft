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

namespace Colosoft.WebControls.SiteMap
{
	public class SiteMapFile
	{
		private List<SiteMapItem> _items = new List<SiteMapItem>();

		/// <summary>
		/// Itens do mapa do site.
		/// </summary>
		public List<SiteMapItem> Items
		{
			get
			{
				return _items;
			}
		}

		/// <summary>
		/// Controi com base nos items o arquivo Sitemap.
		/// </summary>
		/// <param name="outStream"></param>
		public void Build(Stream outStream)
		{
			XmlDocument doc = new XmlDocument();
			XmlElement urlset = doc.CreateElement("urlset");
			urlset.SetAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
			foreach (SiteMapItem item in Items)
			{
				XmlElement xItem = doc.CreateElement("url");
				XmlElement loc = doc.CreateElement("loc");
				loc.InnerText = item.Localization;
				xItem.AppendChild(loc);
				if(item.LastModification != null)
				{
					XmlElement lastmod = doc.CreateElement("lastmod");
					lastmod.InnerText = item.LastModification.GetValueOrDefault().ToString("yyyy-MM-dd");
					xItem.AppendChild(lastmod);
				}
				if(item.ChangeFrequently != null)
				{
					XmlElement changefreq = doc.CreateElement("changefreq");
					changefreq.InnerText = item.ChangeFrequently.ToString().ToLower();
					xItem.AppendChild(changefreq);
				}
				XmlElement priority = doc.CreateElement("priority");
				priority.InnerText = item.Priority.ToString("0.0");
				xItem.AppendChild(priority);
				urlset.AppendChild(xItem);
			}
			doc.AppendChild(urlset);
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Reset();
			settings.Encoding = Encoding.GetEncoding("iso-8859-1");
			using (XmlWriter writer = XmlWriter.Create(outStream, settings))
				doc.Save(writer);
		}
	}
}
