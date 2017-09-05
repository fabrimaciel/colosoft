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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Armazena os dados do redirecionamento de uma assembly.
	/// </summary>
	[Serializable]
	public class Redirection
	{
		private System.Reflection.AssemblyName _assemblyIdentity;

		private BindingRedirect _bindingRedirection;

		/// <summary>
		/// Identidade do assembly.
		/// </summary>
		public System.Reflection.AssemblyName AssemblyIdentity
		{
			get
			{
				return _assemblyIdentity;
			}
			set
			{
				_assemblyIdentity = value;
			}
		}

		/// <summary>
		/// Vinculação do redirecionamento.
		/// </summary>
		public BindingRedirect BindingRedirection
		{
			get
			{
				return _bindingRedirection;
			}
			set
			{
				_bindingRedirection = value;
			}
		}

		/// <summary>
		/// Recupera o caminho do arquivo de configuração.
		/// </summary>
		/// <param name="appFilePath"></param>
		/// <returns></returns>
		public static string FindConfigFile(string appFilePath)
		{
			System.IO.Path.GetFileName(appFilePath);
			System.IO.Path.GetDirectoryName(appFilePath);
			string configName = string.Format(@"{0}\{1}.config", System.IO.Path.GetDirectoryName(appFilePath), System.IO.Path.GetFileName(appFilePath)).Trim(new char[] {
				'\\'
			});
			if(System.IO.File.Exists(configName))
				return configName;
			return null;
		}

		/// <summary>
		/// Recupera o nome correto do assembly.
		/// </summary>
		/// <param name="original"></param>
		/// <param name="dic"></param>
		/// <returns></returns>
		public static System.Reflection.AssemblyName GetCorrectAssemblyName(System.Reflection.AssemblyName original, Dictionary<string, List<Redirection>> dic)
		{
			if(dic.ContainsKey(original.Name))
			{
				foreach (Redirection redirection in dic[original.Name])
				{
					if(redirection.BindingRedirection == null)
						System.Diagnostics.Trace.WriteLine("Redirection data is invalid: " + redirection.AssemblyIdentity);
					else
					{
						Version redirectVersionMin = redirection.BindingRedirection.OldVersionMin;
						Version redirectVersionMax = redirection.BindingRedirection.OldVersionMax;
						if((original.Version >= redirectVersionMin) && (original.Version <= redirectVersionMax))
						{
							var name = new System.Reflection.AssemblyName(original.FullName);
							name.Version = redirection.BindingRedirection.NewVersion;
							name.ProcessorArchitecture = original.ProcessorArchitecture;
							name.SetPublicKeyToken(original.GetPublicKeyToken());
							return name;
						}
					}
				}
			}
			return original;
		}

		/// <summary>
		/// Recupera os redirecionamentos do framework.
		/// </summary>
		/// <param name="assembliesInGac"></param>
		/// <param name="observer"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
		public static Dictionary<string, List<Redirection>> GetFrameworkRedirections(List<AsmData> assembliesInGac, IAssemblyAnalyzerObserver observer)
		{
			string friendlyName = System.Threading.Thread.GetDomain().FriendlyName;
			string exeAssembly = System.Reflection.Assembly.GetEntryAssembly().FullName;
			AppDomainSetup setup = new AppDomainSetup();
			setup.ApplicationBase = Environment.CurrentDirectory;
			setup.DisallowBindingRedirects = false;
			setup.DisallowCodeDownload = true;
			setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			AppDomain redirectionsScanDomain = AppDomain.CreateDomain("Framework Redirections");
			redirectionsScanDomain.UnhandledException += new UnhandledExceptionEventHandler((sender, e) => ("An unhandled exception occured while parsing .NET Framework assemblies. We are sorry for any inconvenience caused.\r\nError message: " + ((Exception)e.ExceptionObject).Message).GetFormatter());
			FrameworkRedirectionsScanner scanner1 = (FrameworkRedirectionsScanner)redirectionsScanDomain.CreateInstanceAndUnwrap(exeAssembly, typeof(FrameworkRedirectionsScanner).FullName);
			AppDomain.Unload(redirectionsScanDomain);
			return new Dictionary<string, List<Redirection>>();
		}

		/// <summary>
		/// Recupera o caminho do redirecionamento.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <param name="configFile"></param>
		/// <returns></returns>
		public static PathRedirection GetPathRedirections(string assemblyName, string configFile)
		{
			if(!System.IO.File.Exists(configFile))
				return new PathRedirection {
					AssemblyName = assemblyName
				};
			var redirect = new PathRedirection {
				AssemblyName = assemblyName
			};
			var config = new System.Xml.XmlDocument();
			config.Load(configFile);
			var nsmgr = new System.Xml.XmlNamespaceManager(config.NameTable);
			nsmgr.AddNamespace("x", "urn:schemas-microsoft-com:asm.v1");
			var probingNode = config.CreateNavigator().SelectSingleNode("/configuration/runtime/x:assemblyBinding/x:probing", nsmgr);
			if(probingNode != null)
			{
				string privatePath = probingNode.GetAttribute("privatePath", string.Empty);
				if(string.IsNullOrEmpty(privatePath))
					return redirect;
				foreach (string p in privatePath.Split(new char[] {
					';'
				}))
					redirect.Directories.Add(System.IO.Path.GetFullPath(p));
			}
			return redirect;
		}

		/// <summary>
		/// Recupera os redirecionamentos das versões.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static Dictionary<string, List<Redirection>> GetVersionRedirections(string fileName)
		{
			var ret = new Dictionary<string, List<Redirection>>();
			var config = new System.Xml.XmlDocument();
			config.Load(fileName);
			foreach (System.Xml.XmlNode dependentAssemblyTag in config.GetElementsByTagName("dependentAssembly"))
			{
				if(((dependentAssemblyTag.ParentNode.Name == "assemblyBinding") && (dependentAssemblyTag.ParentNode.ParentNode != null)) && (dependentAssemblyTag.ParentNode.ParentNode.Name == "runtime"))
				{
					Redirection red = new Redirection();
					foreach (System.Xml.XmlNode node in dependentAssemblyTag.ChildNodes)
					{
						if(node.Name == "assemblyIdentity")
						{
							var name = new System.Reflection.AssemblyName(node.Attributes["name"].Value);
							if(node.Attributes["processorArchitecture"] != null)
								name.ProcessorArchitecture = (System.Reflection.ProcessorArchitecture)Enum.Parse(typeof(System.Reflection.ProcessorArchitecture), node.Attributes["processorArchitecture"].Value, true);
							red.AssemblyIdentity = name;
							continue;
						}
						if(node.Name == "bindingRedirect")
						{
							BindingRedirect redirect = new BindingRedirect();
							if(node.Attributes["oldVersion"] != null)
							{
								System.Xml.XmlAttribute attr = node.Attributes["oldVersion"];
								if(attr.Value.Contains("-"))
								{
									string[] versions = attr.Value.Split(new char[] {
										'-'
									});
									redirect.OldVersionMin = new Version(versions[0]);
									redirect.OldVersionMax = new Version(versions[1]);
								}
								else
								{
									redirect.OldVersionMax = new Version(attr.Value);
									redirect.OldVersionMin = new Version(attr.Value);
								}
							}
							if(node.Attributes["newVersion"] != null)
							{
								redirect.NewVersion = new Version(node.Attributes["newVersion"].Value);
							}
							red.BindingRedirection = redirect;
						}
					}
					if(ret.ContainsKey(red.AssemblyIdentity.Name))
					{
						ret[red.AssemblyIdentity.Name].Add(red);
					}
					else
					{
						var aux = new List<Redirection>();
						aux.Add(red);
						ret.Add(red.AssemblyIdentity.Name, aux);
					}
				}
			}
			if(ret.Count > 0)
			{
				return ret;
			}
			return null;
		}

		private class NamespaceResolver : System.Xml.IXmlNamespaceResolver
		{
			public IDictionary<string, string> GetNamespacesInScope(System.Xml.XmlNamespaceScope scope)
			{
				return null;
			}

			public string LookupNamespace(string prefix)
			{
				return null;
			}

			public string LookupPrefix(string namespaceName)
			{
				return null;
			}
		}
	}
}
