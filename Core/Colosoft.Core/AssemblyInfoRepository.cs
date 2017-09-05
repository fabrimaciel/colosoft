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

namespace Colosoft.Reflection.Local
{
	/// <summary>
	/// Assinatura de um reposiório com as informações dos assemblies.
	/// </summary>
	public class AssemblyInfoRepository : IAssemblyInfoRepository
	{
		private AssemblyAnalyzer _assemblyAnalyzer;

		private Dictionary<string, AssemblyInfoEntry> _assemblies;

		private string _assembliesDirectory;

		private object _lock = new object();

		private IAssemblyInfoRepositoryObserver _observer;

		private DateTime _manifestFileLastWriteTime = DateTime.MinValue;

		private bool _isLoaded;

		/// <summary>
		/// Evento 
		/// </summary>
		public event EventHandler Loaded;

		/// <summary>
		/// Identifica se a instancia foi carregada.
		/// </summary>
		public bool IsLoaded
		{
			get
			{
				return _isLoaded;
			}
		}

		/// <summary>
		/// Nome do arquivo de manifesto.
		/// </summary>
		protected string ManifestFileName
		{
			get
			{
				return System.IO.Path.Combine(_assembliesDirectory, "AssembliesManifest.xml");
			}
		}

		/// <summary>
		/// Quantidade de assemblies carregados no repositório.
		/// </summary>
		public int Count
		{
			get
			{
				CheckInitialize();
				return _assemblies.Count;
			}
		}

		/// <summary>
		/// Identifica se o repositório sofreu alguma alteração.
		/// </summary>
		public bool IsChanged
		{
			get
			{
				return IsManifestFileChanged;
			}
		}

		/// <summary>
		/// Identifica se arquivo de manifesto foi alterado.
		/// </summary>
		public bool IsManifestFileChanged
		{
			get
			{
				var fileInfo = new System.IO.FileInfo(ManifestFileName);
				if(!fileInfo.Exists)
					return true;
				return fileInfo.LastWriteTime != _manifestFileLastWriteTime;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="assembliesDirectory">Diretório dos assemblies que será observado.</param>
		public AssemblyInfoRepository(string assembliesDirectory) : this(assembliesDirectory, null)
		{
		}

		/// <summary>
		/// Cria uma instancia já definindo o Worker background.
		/// </summary>
		/// <param name="assembliesDirectory"></param>
		/// <param name="observer"></param>
		public AssemblyInfoRepository(string assembliesDirectory, IAssemblyInfoRepositoryObserver observer)
		{
			assembliesDirectory.Require("assembliesDirectory").NotNull().NotEmpty();
			_assembliesDirectory = assembliesDirectory;
			_assemblyAnalyzer = new AssemblyAnalyzer();
			_observer = observer;
		}

		/// <summary>
		/// Verifica se o arquivo de manifesto existe.
		/// </summary>
		/// <returns></returns>
		private bool ManifestExists()
		{
			return System.IO.File.Exists(ManifestFileName);
		}

		/// <summary>
		/// Recupera as informações de assembly do arquivo de manifesto.
		/// </summary>
		/// <returns></returns>
		private AssemblyInfoEntry[] GetManifest()
		{
			var fileInfo = new System.IO.FileInfo(ManifestFileName);
			if(!fileInfo.Exists)
				return new AssemblyInfoEntry[0];
			try
			{
				using (var stream = fileInfo.OpenRead())
				{
					var serializer = new System.Xml.Serialization.XmlSerializer(typeof(AssemblyInfoEntry[]));
					var result = (AssemblyInfoEntry[])serializer.Deserialize(stream);
					_manifestFileLastWriteTime = fileInfo.LastWriteTime;
					return result;
				}
			}
			catch
			{
				return new AssemblyInfoEntry[0];
			}
		}

		/// <summary>
		/// Salva o arquivo de manifesto.
		/// </summary>
		private void SaveManifest()
		{
			var manifestFileName = ManifestFileName;
			using (var stream = System.IO.File.Create(manifestFileName))
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(typeof(AssemblyInfoEntry[]));
				serializer.Serialize(stream, _assemblies.Values.ToArray());
			}
		}

		/// <summary>
		/// Verifica se a instancia está inicializada, caso não, executa a inicialização.
		/// </summary>
		private void CheckInitialize()
		{
			if(_assemblies == null || IsManifestFileChanged || !ManifestExists())
				Refresh(false);
		}

		/// <summary>
		/// Converte para as informações do assembly.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private static AssemblyInfo ConvertAssemblyInfo(AsmData data, string fileName)
		{
			var info = new AssemblyInfo {
				Name = System.IO.Path.GetFileNameWithoutExtension(fileName),
				LastWriteTime = System.IO.File.GetLastWriteTime(fileName),
				References = data.References.Select(f => f.Name).ToArray()
			};
			return info;
		}

		/// <summary>
		/// Método acionado quando o repositório for carregado.
		/// </summary>
		protected virtual void OnLoaded()
		{
			_isLoaded = true;
			if(_observer != null)
				_observer.OnLoaded();
			if(Loaded != null)
				Loaded(this, EventArgs.Empty);
		}

		/// <summary>
		/// Método acioando quando os arquivos de assembly estão sendo carregados.
		/// </summary>
		protected virtual void OnLoadingAssemblyFiles()
		{
			if(_observer != null)
				_observer.OnLoadingAssemblyFiles();
		}

		/// <summary>
		/// Método acionad quando o progresso de analize dos assemblies é alterado. do repositório é alterado.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="percentage"></param>
		protected virtual void OnAnalysisAssemblyProgressChanged(IMessageFormattable message, int percentage)
		{
			if(_observer != null)
				_observer.OnAnalysisAssemblyProgressChanged(message, percentage);
		}

		/// <summary>
		/// Atualiza os dados do repositório.
		/// </summary>
		/// <param name="executeAnalyzer">Identifica se é para executar o analizador.</param>
		public void Refresh(bool executeAnalyzer)
		{
			var isInitializing = false;
			lock (_lock)
			{
				string[] originalFiles = null;
				List<string> files = null;
				OnLoadingAssemblyFiles();
				try
				{
					originalFiles = System.IO.Directory.GetFiles(_assembliesDirectory, "*.dll").Concat(System.IO.Directory.GetFiles(_assembliesDirectory, "*.exe")).Select(f => System.IO.Path.GetFileName(f)).ToArray();
					Array.Sort<string>(originalFiles, StringComparer.InvariantCulture);
					files = new List<string>(originalFiles);
				}
				catch
				{
					files = new List<string>();
				}
				bool manifestExists = ManifestExists();
				isInitializing = _assemblies == null || IsManifestFileChanged;
				if(isInitializing)
				{
					_assemblies = new Dictionary<string, AssemblyInfoEntry>();
					if(!manifestExists && !executeAnalyzer)
					{
						foreach (var file in originalFiles)
						{
							var entry = new AssemblyInfoEntry {
								FileName = file,
								Info = new AssemblyInfo {
									Name = System.IO.Path.GetFileNameWithoutExtension(file),
									References = new string[0],
									LastWriteTime = System.IO.File.GetLastWriteTime(System.IO.Path.Combine(_assembliesDirectory, file))
								}
							};
							_assemblies.Add(entry.Info.Name, entry);
						}
					}
					else
						foreach (var i in GetManifest())
							_assemblies.Add(i.Info.Name, i);
				}
				var removeNames = new Queue<string>();
				foreach (var i in _assemblies)
				{
					var index = files.BinarySearch(i.Value.FileName, StringComparer.InvariantCulture);
					if(index < 0)
						removeNames.Enqueue(i.Key);
					else
						files.RemoveAt(index);
				}
				while (removeNames.Count > 0)
					_assemblies.Remove(removeNames.Dequeue());
				var newInfos = new List<AssemblyInfoEntry>();
				if(manifestExists || executeAnalyzer)
				{
					foreach (var i in _assemblies)
					{
						try
						{
							var lastWriteTime = System.IO.File.GetLastWriteTime(System.IO.Path.Combine(_assembliesDirectory, i.Value.FileName));
							if(i.Value.Info.LastWriteTime != lastWriteTime)
							{
								removeNames.Enqueue(i.Key);
								if(executeAnalyzer)
									files.Add(i.Value.FileName);
								else
								{
									i.Value.Info.LastWriteTime = lastWriteTime;
									newInfos.Add(i.Value);
								}
							}
						}
						catch
						{
							if(executeAnalyzer)
								files.Add(i.Value.FileName);
							removeNames.Enqueue(i.Key);
						}
					}
					while (removeNames.Count > 0)
						_assemblies.Remove(removeNames.Dequeue());
				}
				string tempDirectory = null;
				if(files.Count > 0)
				{
					try
					{
						tempDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()));
						System.IO.Directory.CreateDirectory(tempDirectory);
						var copyFiles = new string[files.Count];
						for(var i = 0; i < files.Count; i++)
						{
							var destFileName = System.IO.Path.Combine(tempDirectory, files[i]);
							System.IO.File.Copy(System.IO.Path.Combine(_assembliesDirectory, files[i]), destFileName);
							copyFiles[i] = destFileName;
						}
						for(var i = 0; i < files.Count; i++)
						{
							OnAnalysisAssemblyProgressChanged(("Analyzing " + files[i] + " ...").GetFormatter(), ((100 * i) / files.Count));
							try
							{
								var data = _assemblyAnalyzer.AnalyzeRootAssembly(copyFiles[i], false);
								newInfos.Add(new AssemblyInfoEntry {
									FileName = files[i],
									Info = ConvertAssemblyInfo(data, System.IO.Path.Combine(_assembliesDirectory, files[i]))
								});
							}
							catch
							{
							}
						}
					}
					finally
					{
						if(tempDirectory != null)
						{
							var files2 = System.IO.Directory.GetFiles(tempDirectory);
							foreach (var i in files2)
								try
								{
									System.IO.File.Delete(i);
								}
								catch
								{
								}
							try
							{
								System.IO.Directory.Delete(tempDirectory, true);
							}
							catch
							{
							}
						}
					}
				}
				foreach (var i in newInfos)
					_assemblies.Add(i.Info.Name, i);
				if(newInfos.Count > 0)
					SaveManifest();
			}
			if(isInitializing)
				OnLoaded();
		}

		/// <summary>
		/// Recupera as informações do assembly pelo nome informado.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <param name="assemblyInfo"></param>
		/// <param name="exception">Error ocorrido.</param>
		/// <returns>True se as informações do assembly forem recuperadas com sucesso.</returns>
		public bool TryGet(string assemblyName, out AssemblyInfo assemblyInfo, out Exception exception)
		{
			try
			{
				CheckInitialize();
				AssemblyInfoEntry entry = null;
				if(_assemblies.TryGetValue(assemblyName, out entry))
				{
					assemblyInfo = entry.Info;
					exception = null;
					return true;
				}
				assemblyInfo = null;
				exception = null;
				return false;
			}
			catch(Exception ex)
			{
				assemblyInfo = null;
				exception = ex;
				return false;
			}
		}

		/// <summary>
		/// Verifica se no repositório existe as informações do assembly informado.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		public bool Contains(string assemblyName)
		{
			CheckInitialize();
			return _assemblies.ContainsKey(assemblyName);
		}

		/// <summary>
		/// Recupera o enumerador da instancia.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<AssemblyInfo> GetEnumerator()
		{
			CheckInitialize();
			return _assemblies.Values.Select(f => f.Info).ToList().GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador da instancia.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			CheckInitialize();
			return _assemblies.Values.Select(f => f.Info).ToList().GetEnumerator();
		}
	}
	/// <summary>
	/// Armazena os dados da entrada.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class AssemblyInfoEntry : System.Xml.Serialization.IXmlSerializable
	{
		/// <summary>
		/// Informações do assembly.
		/// </summary>
		public AssemblyInfo Info
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do arquivo.
		/// </summary>
		public string FileName
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			ReflectionNamespace.ResolveReflectionSchema(xs);
			return new System.Xml.XmlQualifiedName("AssemblyInfoEntry", ReflectionNamespace.Data);
		}

		/// <summary>
		/// Recupera o esquema.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("FileName"))
				FileName = reader.ReadContentAsString();
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				var info = new AssemblyInfo();
				((System.Xml.Serialization.IXmlSerializable)info).ReadXml(reader);
				this.Info = info;
				reader.ReadEndElement();
			}
			else
			{
				this.Info = null;
				reader.Skip();
			}
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("FileName", FileName);
			writer.WriteStartElement("Info");
			if(this.Info != null)
				((System.Xml.Serialization.IXmlSerializable)this.Info).WriteXml(writer);
			writer.WriteEndElement();
		}
	}
}
