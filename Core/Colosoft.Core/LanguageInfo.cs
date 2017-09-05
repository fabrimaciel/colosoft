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
	/// Informações da linguagem.
	/// </summary>
	public class LanguageInfo
	{
		/// <summary>
		/// Extensões da linguagem.
		/// </summary>
		public string[] Extensions
		{
			get;
			private set;
		}

		/// <summary>
		/// Assemblies da linguagem.
		/// </summary>
		public string[] Assemblies
		{
			get;
			private set;
		}

		/// <summary>
		/// Contexto da linguagem.
		/// </summary>
		public string LanguageContext
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="extensions"></param>
		/// <param name="assemblies"></param>
		/// <param name="languageContext"></param>
		public LanguageInfo(string[] extensions, string[] assemblies, string languageContext)
		{
			Extensions = extensions;
			Assemblies = assemblies;
			LanguageContext = languageContext;
		}

		/// <summary>
		/// Recupera o nome do assembly.
		/// </summary>
		/// <returns></returns>
		public string GetContextAssemblyName()
		{
			return System.Reflection.AssemblyName.GetAssemblyName(Assemblies[0]).FullName;
		}

		/// <summary>
		/// Recupera a string com as extensões da linguagem.
		/// </summary>
		/// <returns></returns>
		public string GetExtensionsString()
		{
			StringBuilder str = new StringBuilder();
			foreach (string ext in Extensions)
			{
				if(str.Length > 0)
					str.Append(",");
				str.Append(ext + ",." + ext);
			}
			return str.ToString();
		}
	}
	/// <summary>
	/// Seção de configuração para a linguagem.
	/// </summary>
	public class LanguageSection : System.Configuration.IConfigurationSectionHandler
	{
		/// <summary>
		/// Cria a seção de configuração.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			Dictionary<string, LanguageInfo> languages = new Dictionary<string, LanguageInfo>();
			char[] splitChars = new char[] {
				' ',
				'\t',
				',',
				';',
				'\r',
				'\n'
			};
			foreach (System.Xml.XmlElement elem in ((System.Xml.XmlElement)section).GetElementsByTagName("Language"))
			{
				LanguageInfo info = new LanguageInfo(elem.GetAttribute("extensions").Split(splitChars, StringSplitOptions.RemoveEmptyEntries), elem.GetAttribute("assemblies").Split(splitChars, StringSplitOptions.RemoveEmptyEntries), elem.GetAttribute("languageContext"));
				foreach (string ext in info.Extensions)
				{
					languages["." + ext.ToLower()] = info;
				}
			}
			return languages;
		}
	}
}
