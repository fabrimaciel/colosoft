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
	/// Analizador de assembly.
	/// </summary>
	public class AssemblyAnalyzer : MarshalByRefObject
	{
		private readonly Dictionary<string, Dictionary<string, List<Redirection>>> _allVersionRedirections = new Dictionary<string, Dictionary<string, List<Redirection>>>();

		private static bool[,] _architectureCompatibilityMatrix = new bool[,] {
			{
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				true,
				true,
				true
			},
			{
				false,
				true,
				true,
				false,
				false
			},
			{
				false,
				true,
				false,
				true,
				false
			},
			{
				false,
				true,
				false,
				false,
				true
			}
		};

		private int _assembliesFinished;

		private readonly Dictionary<string, string> assemblyNameToPathMap = new Dictionary<string, string>();

		private System.ComponentModel.BackgroundWorker _bgWorker;

		/// <summary>
		/// Cache dos assemblies.
		/// </summary>
		private Dictionary<string, AsmData> _cache;

		/// <summary>
		/// Identifica se é para exibir um warnig quando ocorrer um dependencia circular.
		/// </summary>
		private bool _circularDependencyWarningShown;

		private System.Reflection.ProcessorArchitecture _currentLoadedArchitecture;

		private List<AsmData> _gac;

		private Stack<string> _parentsStack;

		private readonly Dictionary<string, PathRedirection> _probingPaths = new Dictionary<string, PathRedirection>();

		private int _totalReferencedAssemblies;

		private string _workingDir;

		/// <summary>
		/// Worker usado para processar a analize.
		/// </summary>
		public System.ComponentModel.BackgroundWorker BgWorker
		{
			get
			{
				return _bgWorker;
			}
			set
			{
				_bgWorker = value;
			}
		}

		/// <summary>
		/// Dados dos assemblies do GAC.
		/// </summary>
		public List<AsmData> Gac
		{
			get
			{
				return _gac;
			}
			set
			{
				_gac = value;
			}
		}

		/// <summary>
		/// Analiza o assembly informado.
		/// </summary>
		/// <param name="asmName"></param>
		/// <param name="parent"></param>
		/// <param name="domain"></param>
		/// <param name="throwWhenMissing"></param>
		private void AnalyzeAssembly(System.Reflection.AssemblyName asmName, AsmData parent, AppDomain domain, bool throwWhenMissing)
		{
			bool redirectionApplied = false;
			string additionalInfo = string.Empty;
			System.Reflection.AssemblyName analyzedAssembly = asmName;
			Dictionary<string, List<Redirection>> asmRedirections = this.GetRedirections(asmName);
			if(asmRedirections != null)
			{
				analyzedAssembly = Redirection.GetCorrectAssemblyName(asmName, asmRedirections);
				redirectionApplied = analyzedAssembly.Version != asmName.Version;
			}
			bool invalid = false;
			bool isInGac = false;
			AsmData gacAssemblyData = null;
			System.Reflection.Assembly asm = null;
			string file = analyzedAssembly.FullName;
			try
			{
				if(_gac != null)
					foreach (AsmData item in _gac)
					{
						if(item.AssemblyFullName.Contains(analyzedAssembly.FullName))
						{
							isInGac = true;
							gacAssemblyData = item;
							break;
						}
					}
			}
			catch(Exception)
			{
			}
			if(_cache.ContainsKey(analyzedAssembly.FullName) && !_parentsStack.Contains(analyzedAssembly.FullName))
			{
				AsmData cachedItem = _cache[analyzedAssembly.FullName];
				parent.References.Add(cachedItem);
				return;
			}
			string asmLocation = null;
			AsmData currentAsmData = null;
			bool gacAssemblySet = false;
			if(!isInGac)
			{
				string extAdd = "";
				if(file.LastIndexOf(", Version=") != -1)
					file = file.Substring(0, file.LastIndexOf(", Version="));
				if((System.IO.Path.GetExtension(file) != ".exe") && (System.IO.Path.GetExtension(file) != ".dll"))
					extAdd = ".dll";
				string tmpPath = this.FindPath(parent, file, extAdd, analyzedAssembly.FullName);
				if(System.IO.File.Exists(tmpPath))
					TryLoad(domain, ref additionalInfo, ref invalid, ref asm, tmpPath);
				asmLocation = tmpPath;
			}
			else
			{
				try
				{
					using (var stream = System.IO.File.OpenRead(gacAssemblyData.Path))
					{
						var raw = new byte[stream.Length];
						stream.Read(raw, 0, raw.Length);
						asm = domain.Load(raw);
					}
					asmLocation = gacAssemblyData.Path;
					if(!gacAssemblyData.AssemblyFullName.Contains(asm.FullName) && !asm.FullName.Contains(gacAssemblyData.AssemblyFullName))
					{
						currentAsmData = gacAssemblyData;
						gacAssemblySet = true;
						asm = null;
					}
				}
				catch(System.IO.FileNotFoundException ex)
				{
					additionalInfo = "File " + ex.FileName + " could not be found.";
				}
				catch(System.IO.FileLoadException ex)
				{
					additionalInfo = "File " + ex.FileName + " could not be loaded. " + ex.FusionLog;
				}
				catch(BadImageFormatException ex)
				{
					additionalInfo = "Bad image format. " + ex.ToString() + "\r\n" + ex.FusionLog;
				}
			}
			if(currentAsmData == null)
			{
				currentAsmData = new AsmData(analyzedAssembly.Name, (asm == null) ? "" : System.IO.Path.GetFullPath(asmLocation));
				currentAsmData.AssemblyFullName = analyzedAssembly.FullName;
				currentAsmData.Validity = AsmData.AsmValidity.Invalid;
				currentAsmData.InvalidAssemblyDetails = additionalInfo;
				currentAsmData.Architecture = this.GetArchitecture(currentAsmData.Path);
			}
			if((!gacAssemblySet && (asm != null)) && (analyzedAssembly.Version != asm.GetName().Version))
			{
				string message = string.Concat(new object[] {
					"Assembly was found with version ",
					asm.GetName().Version,
					" but parent references ",
					analyzedAssembly.Version
				});
				currentAsmData.AdditionalInfo = message;
				asm = null;
			}
			if((!gacAssemblySet && (asm != null)) && !invalid)
			{
				try
				{
					object[] attributes = asm.GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), false);
					if(attributes.Length > 0)
					{
						currentAsmData.AssemblyProductName = ((System.Reflection.AssemblyProductAttribute)attributes[0]).Product;
					}
				}
				catch(InvalidOperationException)
				{
					currentAsmData.AssemblyProductName = "Product name could not be read.";
				}
				catch(System.IO.FileNotFoundException)
				{
					currentAsmData.AssemblyProductName = "Product name could not be read. Assembly was loaded but later could not be found.";
				}
				catch(Exception ex)
				{
					currentAsmData.AssemblyProductName = "Product name could not be read. Error: " + ex.Message;
				}
			}
			parent.References.Add(currentAsmData);
			if(invalid)
			{
				currentAsmData.Validity = AsmData.AsmValidity.ReferencesOnly;
			}
			if(_parentsStack.Contains(analyzedAssembly.FullName))
			{
				string circ = this.GetParentsString();
				currentAsmData.Validity = AsmData.AsmValidity.CircularDependency;
				if(!_circularDependencyWarningShown)
					_circularDependencyWarningShown = true;
				return;
			}
			if(asm != null)
			{
				currentAsmData.Path = asmLocation;
				currentAsmData.AssemblyFullName = asm.FullName;
				if(!invalid)
				{
					currentAsmData.Validity = redirectionApplied ? AsmData.AsmValidity.Redirected : AsmData.AsmValidity.Valid;
					currentAsmData.OriginalVersion = redirectionApplied ? asmName.Version.ToString() : string.Empty;
				}
				if(((asm.CodeBase != null) && System.Runtime.InteropServices.RuntimeEnvironment.FromGlobalAccessCache(asm)) && (currentAsmData.AssemblyProductName == "Microsoft\x00ae .NET Framework"))
					return;
				if((currentAsmData.Validity != AsmData.AsmValidity.Invalid) && !this.ApplyArchitecture(currentAsmData.Architecture))
				{
					currentAsmData.Validity = AsmData.AsmValidity.ReferencesOnly;
					currentAsmData.AdditionalInfo = currentAsmData.AdditionalInfo + "\r\nProcessorArchitecture mismatch";
				}
				_parentsStack.Push(currentAsmData.AssemblyFullName);
				_cache.Add(analyzedAssembly.FullName, currentAsmData);
				foreach (System.Reflection.AssemblyName n in asm.GetReferencedAssemblies())
				{
					this.AnalyzeAssembly(n, currentAsmData, domain, throwWhenMissing);
				}
				_parentsStack.Pop();
				if(!System.IO.File.Exists(currentAsmData.Path))
					return;
			}
			if(throwWhenMissing && !gacAssemblySet)
				throw new Exception("returning from analysis");
		}

		/// <summary>
		/// Aplica a arquitetura.
		/// </summary>
		/// <param name="processorArchitecture"></param>
		/// <returns></returns>
		private bool ApplyArchitecture(System.Reflection.ProcessorArchitecture processorArchitecture)
		{
			if(((_currentLoadedArchitecture == System.Reflection.ProcessorArchitecture.Amd64) || (_currentLoadedArchitecture == System.Reflection.ProcessorArchitecture.IA64)) || (_currentLoadedArchitecture == System.Reflection.ProcessorArchitecture.X86))
				return this.IsCompatible(_currentLoadedArchitecture, processorArchitecture);
			if(_currentLoadedArchitecture != System.Reflection.ProcessorArchitecture.MSIL)
				return false;
			_currentLoadedArchitecture = processorArchitecture;
			if(processorArchitecture == System.Reflection.ProcessorArchitecture.None)
				return false;
			return true;
		}

		/// <summary>
		/// Verifica uma dependencia circular.
		/// </summary>
		/// <param name="fullName"></param>
		/// <returns></returns>
		private bool CheckCircularDependency(string fullName)
		{
			return _parentsStack.Contains(fullName);
		}

		/// <summary>
		/// Localiza o caminho do arquivo.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="file"></param>
		/// <param name="extAdd"></param>
		/// <param name="currentFullName"></param>
		/// <returns></returns>
		private string FindPath(AsmData parent, string file, string extAdd, string currentFullName)
		{
			string result;
			PathRedirection redirects;
			string parentDir = _workingDir;
			if((parent != null) && !string.IsNullOrEmpty(parent.Path))
				parentDir = System.IO.Path.GetDirectoryName(parent.Path);
			string tmpPath = file + extAdd;
			if(System.IO.File.Exists(tmpPath))
				return tmpPath;
			tmpPath = System.IO.Path.Combine(parentDir, file + extAdd);
			if(System.IO.File.Exists(tmpPath))
				return tmpPath;
			string ret = System.IO.Path.Combine(parentDir, file + extAdd);
			if(!_probingPaths.TryGetValue(parent.AssemblyFullName, out redirects))
				return ret;
			foreach (string currentDir in redirects.Directories)
			{
				string targetDir = currentDir;
				if(!System.IO.Path.IsPathRooted(currentDir))
					targetDir = System.IO.Path.Combine(parentDir, currentDir);
				if(System.IO.File.Exists(System.IO.Path.Combine(targetDir, file + extAdd)))
				{
					string targetPath = System.IO.Path.Combine(targetDir, file + extAdd);
					return targetPath;
				}
			}
			result = ret;
			return result;
		}

		/// <summary>
		/// Recupera a arquitetura do caminho informado.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private System.Reflection.ProcessorArchitecture GetArchitecture(string path)
		{
			if(System.IO.File.Exists(path))
			{
				try
				{
					var descriptor = System.Reflection.AssemblyName.GetAssemblyName(path);
					if(descriptor != null)
						return descriptor.ProcessorArchitecture;
				}
				catch(Exception)
				{
				}
			}
			return System.Reflection.ProcessorArchitecture.None;
		}

		/// <summary>
		/// Recupera a string dos pais.
		/// </summary>
		/// <returns></returns>
		private string GetParentsString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (string asm in _parentsStack)
				sb.Insert(0, asm + "\r\n");
			return sb.ToString();
		}

		/// <summary>
		/// Recupera o redirecionamento do assembly.
		/// </summary>
		/// <param name="asmName"></param>
		/// <returns></returns>
		private Dictionary<string, List<Redirection>> GetRedirections(System.Reflection.AssemblyName asmName)
		{
			string key;
			if(this.assemblyNameToPathMap.TryGetValue(asmName.Name, out key) && _allVersionRedirections.ContainsKey(key))
				return _allVersionRedirections[key];
			return null;
		}

		/// <summary>
		/// Verifica se as arquiteturas são compatíveis.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		private bool IsCompatible(System.Reflection.ProcessorArchitecture parent, System.Reflection.ProcessorArchitecture child)
		{
			return _architectureCompatibilityMatrix[(int)parent, (int)child];
		}

		/// <summary>
		/// Tenta carregar as informações.
		/// </summary>
		/// <param name="domain"></param>
		/// <param name="additionalInfo"></param>
		/// <param name="invalid"></param>
		/// <param name="asm"></param>
		/// <param name="tmpPath"></param>
		private static void TryLoad(AppDomain domain, ref string additionalInfo, ref bool invalid, ref System.Reflection.Assembly asm, string tmpPath)
		{
			byte[] raw = null;
			try
			{
				using (var stream = System.IO.File.OpenRead(tmpPath))
				{
					raw = new byte[stream.Length];
					stream.Read(raw, 0, raw.Length);
				}
			}
			catch(System.IO.FileLoadException ex)
			{
				invalid = true;
				additionalInfo = "File " + ex.FileName + " could not be loaded. " + ex.FusionLog;
				return;
			}
			catch(Exception ex)
			{
				invalid = true;
				additionalInfo = "Unexpected error. " + ex + "\r\n";
				return;
			}
			try
			{
				asm = domain.Load(raw);
			}
			catch(Exception)
			{
				invalid = true;
				try
				{
					asm = System.Reflection.Assembly.ReflectionOnlyLoad(raw);
				}
				catch(BadImageFormatException ex)
				{
					additionalInfo = "Bad image format. " + ex.ToString() + "\r\n" + ex.FusionLog;
				}
				catch(Exception ex)
				{
					additionalInfo = "Unexpected error. " + ex + "\r\n";
				}
			}
		}

		/// <summary>
		/// Analiza o assembly.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		public AsmData AnalyzeRootAssembly(string assemblyName)
		{
			return this.AnalyzeRootAssembly(assemblyName, false);
		}

		/// <summary>
		/// Analiza o assembly informado.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <param name="throwWhenMissing"></param>
		/// <returns></returns>
		public AsmData AnalyzeRootAssembly(string assemblyName, bool throwWhenMissing)
		{
			_cache = new Dictionary<string, AsmData>();
			_circularDependencyWarningShown = false;
			_parentsStack = new Stack<string>();
			_workingDir = Environment.CurrentDirectory;
			string fullPath = System.IO.Path.GetFullPath(assemblyName);
			AsmData ret = new AsmData(assemblyName, fullPath);
			var domain = AppDomain.CurrentDomain;
			try
			{
				if(!System.IO.File.Exists(assemblyName))
				{
					ret.Path = "";
					ret.Validity = AsmData.AsmValidity.Invalid;
					ret.AssemblyFullName = "";
					return ret;
				}
				System.Reflection.Assembly asm = null;
				try
				{
					using (var stream = System.IO.File.OpenRead(fullPath))
					{
						var raw = new byte[stream.Length];
						stream.Read(raw, 0, raw.Length);
						try
						{
							asm = domain.Load(raw);
							ret.Validity = AsmData.AsmValidity.Valid;
						}
						catch(Exception)
						{
							asm = System.Reflection.Assembly.ReflectionOnlyLoad(raw);
							ret.Validity = AsmData.AsmValidity.ReferencesOnly;
						}
					}
				}
				catch(Exception)
				{
					asm = null;
					ret.Validity = AsmData.AsmValidity.Invalid;
				}
				if(asm != null)
				{
					ret.AssemblyFullName = asm.FullName;
					ret.Path = fullPath;
					ret.Architecture = asm.GetName().ProcessorArchitecture;
					_currentLoadedArchitecture = ret.Architecture;
					string tempName = asm.GetName().Name;
					if(!this.assemblyNameToPathMap.ContainsKey(tempName))
						this.assemblyNameToPathMap.Add(tempName, asm.Location);
					string cfgFilePath = Redirection.FindConfigFile(ret.Path);
					if(!string.IsNullOrEmpty(cfgFilePath) && !_allVersionRedirections.ContainsKey(fullPath))
					{
						var versionRedirections = Redirection.GetVersionRedirections(cfgFilePath);
						PathRedirection pathRedirections = Redirection.GetPathRedirections(ret.AssemblyFullName, cfgFilePath);
						_allVersionRedirections.Add(fullPath, versionRedirections);
						_probingPaths.Add(ret.AssemblyFullName, pathRedirections);
					}
					var references = asm.GetReferencedAssemblies().ToList();
					var fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fullPath), string.Format("{0}.XmlSerializers{1}", System.IO.Path.GetFileNameWithoutExtension(fullPath), System.IO.Path.GetExtension(fullPath)));
					if(System.IO.File.Exists(fileName))
						references.Add(System.Reflection.AssemblyName.GetAssemblyName(fileName));
					_totalReferencedAssemblies = references.Count;
					_parentsStack.Push(ret.AssemblyFullName);
					foreach (System.Reflection.AssemblyName asmName in references)
					{
						this.AnalyzeAssembly(asmName, ret, domain, throwWhenMissing);
						_assembliesFinished++;
						if(_bgWorker != null)
							_bgWorker.ReportProgress((100 * _assembliesFinished) / _totalReferencedAssemblies);
					}
					_parentsStack.Pop();
				}
			}
			finally
			{
			}
			return ret;
		}

		/// <summary>
		/// Recupera os assemblies contidos no diretório informado.
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="recursive"></param>
		/// <param name="cancellationPendingCallback"></param>
		/// <param name="progressReportCallback"></param>
		/// <returns></returns>
		public static List<string> GetAssemblies(string directory, bool recursive, Func<bool> cancellationPendingCallback, Action<int, string> progressReportCallback)
		{
			if(progressReportCallback != null)
				progressReportCallback(0, "Reading directory " + directory);
			string[] files = System.IO.Directory.GetFiles(directory);
			List<string> assemblies = new List<string>();
			foreach (string file in files)
			{
				if((cancellationPendingCallback != null) && cancellationPendingCallback())
				{
					return assemblies;
				}
				try
				{
					if(IsAssembly(file))
					{
						assemblies.Add(file);
					}
				}
				catch(Exception)
				{
				}
			}
			if(recursive)
			{
				foreach (string dir in System.IO.Directory.GetDirectories(directory))
					assemblies.AddRange(GetAssemblies(dir, recursive, cancellationPendingCallback, progressReportCallback));
			}
			return assemblies;
		}

		/// <summary>
		/// Verifica se é um assembly válido.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		public bool IsValidAssembly(string path, out string error)
		{
			error = string.Empty;
			try
			{
				System.Reflection.Assembly asm = System.Reflection.Assembly.ReflectionOnlyLoadFrom(path);
				_totalReferencedAssemblies = asm.GetReferencedAssemblies().Length;
			}
			catch(System.IO.FileLoadException ex)
			{
				error = "This file could not be loaded. \r\nException details: " + ex.FusionLog;
			}
			catch(BadImageFormatException ex)
			{
				error = "This file is not a valid assembly or this assembly is built with later version of CLR\r\nPlease update your CLR to the latest version. \r\nException details: " + ex.FusionLog;
			}
			catch(System.Security.SecurityException ex)
			{
				error = "A security problem has occurred while loading the assembly.\r\nFailed permission: " + ex.FirstPermissionThatFailed;
			}
			catch(System.IO.PathTooLongException)
			{
				error = "Given path is too long.";
			}
			return (error == string.Empty);
		}

		/// <summary>
		/// Verifica se o arquivo informado é um assembly.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		public static bool IsAssembly(string fileName)
		{
			uint rva15value = 0;
			bool invalid = false;
			using (var fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				var reader = new System.IO.BinaryReader(fileStream);
				try
				{
					int dictionaryOffset;
					if(60 > fileStream.Length)
						return false;
					fileStream.Position = 60;
					uint headerOffset = reader.ReadUInt32();
					fileStream.Position = headerOffset + 0x18;
					if(fileStream.Position > fileStream.Length)
						return false;
					switch(reader.ReadUInt16())
					{
					case 0x10b:
						dictionaryOffset = 0x60;
						break;
					case 0x20b:
						dictionaryOffset = 0x70;
						break;
					default:
						invalid = true;
						dictionaryOffset = 0;
						break;
					}
					if(!invalid)
					{
						fileStream.Position = ((headerOffset + 0x18) + dictionaryOffset) + 0x70;
						rva15value = reader.ReadUInt32();
					}
				}
				finally
				{
					reader.Close();
				}
			}
			return ((rva15value != 0) && !invalid);
		}
	}
}
