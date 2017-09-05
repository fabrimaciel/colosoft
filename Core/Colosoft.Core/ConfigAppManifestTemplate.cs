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
	public class ConfigAppManifestTemplate : Colosoft.IO.Xap.IAppManifestTemplate
	{
		private string _template;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="templateFileName"></param>
		public ConfigAppManifestTemplate(string templateFileName)
		{
			_template = templateFileName;
		}

		/// <summary>
		/// Gera o manifest para o assembly informados.
		/// </summary>
		/// <param name="assemblySources"></param>
		/// <returns></returns>
		public System.Xml.XmlDocument Generate(IEnumerable<Uri> assemblySources)
		{
			var doc = new System.Xml.XmlDocument();
			doc.LoadXml(_template);
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
	/// <summary>
	/// Seção de configuração.
	/// </summary>
	public class AppManifestSection : System.Configuration.IConfigurationSectionHandler
	{
		/// <summary>
		/// Cria uma seção.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			if(((System.Xml.XmlElement)section).GetElementsByTagName("Deployment.Parts").Count != 1)
				throw new Exception("appManifestTemplate section requires exactly one Deployment.Parts element");
			return new ConfigAppManifestTemplate(section.InnerXml);
		}
	}
}
