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
	/// Classe que auxilia na localização dos redirecionamentos
	/// do framework.
	/// </summary>
	internal class FrameworkRedirectionsScanner : MarshalByRefObject
	{
		/// <summary>
		/// Recupera os redirecionamentos do Framework.
		/// </summary>
		/// <param name="assembliesInGac"></param>
		/// <param name="progressDialog"></param>
		/// <returns></returns>
		public Dictionary<string, List<Redirection>> GetFrameworkRedirections(List<AsmData> assembliesInGac, IAssemblyAnalyzerObserver progressDialog)
		{
			Dictionary<string, List<Redirection>> redirections = new Dictionary<string, List<Redirection>>();
			try
			{
				progressDialog.ReportProgress(0, "Checking .NET Framework libraries...".GetFormatter());
				int assembliesCount = 0;
				var upgrades = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework\Policy\Upgrades");
				if(upgrades == null)
					return redirections;
				var bindingRedirects = new List<BindingRedirect>();
				foreach (string targetVersion in upgrades.GetValueNames())
				{
					string sourceVersion = upgrades.GetValue(targetVersion) as string;
					BindingRedirect redirect = new BindingRedirect();
					redirect.NewVersion = new Version(targetVersion);
					if(sourceVersion.Contains("-"))
					{
						string[] versions = sourceVersion.Split(new char[] {
							'-'
						});
						redirect.OldVersionMin = new Version(versions[0]);
						redirect.OldVersionMax = new Version(versions[1]);
					}
					else
					{
						redirect.OldVersionMax = new Version(sourceVersion);
						redirect.OldVersionMin = new Version(sourceVersion);
					}
					bindingRedirects.Add(redirect);
				}
				upgrades.Close();
				foreach (AsmData assemblyDescription in assembliesInGac)
				{
					System.Reflection.Assembly asm = null;
					try
					{
						asm = System.Reflection.Assembly.Load(assemblyDescription.Name);
					}
					catch(Exception)
					{
						continue;
					}
					var assemblyName = asm.GetName(false);
					if(!redirections.ContainsKey(assemblyName.Name))
					{
						object[] attributes = null;
						try
						{
							attributes = asm.GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), false);
						}
						catch(Exception)
						{
						}
						if((attributes != null) && (attributes.Length > 0))
						{
							var productAttribute = attributes[0] as System.Reflection.AssemblyProductAttribute;
							if((productAttribute != null) && (productAttribute.Product == "Microsoft\x00ae .NET Framework"))
							{
								foreach (BindingRedirect bindingRedirect in bindingRedirects)
								{
									Redirection redirection = new Redirection();
									redirection.AssemblyIdentity = assemblyName;
									redirection.BindingRedirection = bindingRedirect;
									if(assemblyName.Version <= redirection.BindingRedirection.NewVersion)
										redirection.BindingRedirection.NewVersion = assemblyName.Version;
									if(redirections.ContainsKey(redirection.AssemblyIdentity.Name))
										redirections[redirection.AssemblyIdentity.Name].Add(redirection);
									else
									{
										var aux = new List<Redirection>();
										aux.Add(redirection);
										redirections.Add(redirection.AssemblyIdentity.Name, aux);
									}
								}
							}
						}
						assembliesCount++;
						progressDialog.ReportProgress((int)((100.0 * assembliesCount) / ((double)assembliesInGac.Count)), "Checking .NET Framework libraries...".GetFormatter());
						if(progressDialog.CancellationPending)
						{
							redirections.Clear();
							return redirections;
						}
					}
				}
			}
			catch(Exception)
			{
			}
			return redirections;
		}
	}
}
