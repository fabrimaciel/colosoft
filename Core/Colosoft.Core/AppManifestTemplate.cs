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
using System.Linq;
using System.Text;

namespace Colosoft.IO.Xap
{
	/// <summary>
	/// Gera um AppManifest.xaml.
	/// </summary>
	public class AppManifestTemplate : Colosoft.IO.Xap.IAppManifestTemplate
	{
		/// <summary>
		/// Gera o manifest para o assembly informados.
		/// </summary>
		/// <param name="assemblySources"></param>
		/// <returns></returns>
		public System.Xml.XmlDocument Generate(IEnumerable<Uri> assemblySources)
		{
			var doc = new System.Xml.XmlDocument();
			using (var stream = typeof(AppManifestTemplate).Assembly.GetManifestResourceStream("Colosoft.IO.Xap.AppManifest.xaml"))
			{
				doc.Load(stream);
				var target = (System.Xml.XmlElement)doc.GetElementsByTagName("Deployment.Parts")[0];
				foreach (Uri source in assemblySources)
				{
					var ap = doc.CreateElement("AssemblyPart", target.NamespaceURI);
					string src = source.ToString();
					ap.SetAttribute("Source", src);
					target.AppendChild(ap);
				}
				return doc;
			}
		}
	}
}
