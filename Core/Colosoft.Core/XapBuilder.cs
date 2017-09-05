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
using System.IO;
using Colosoft.IO.Compression;

namespace Colosoft.IO.Xap
{
	/// <summary>
	/// XAP file builder.
	/// </summary>
	public static class XapBuilder
	{
		/// <summary>
		/// Representa uma entrada do Xap.
		/// </summary>
		public sealed class XapEntry : IDisposable
		{
			private Lazy<System.IO.Stream> _stream;

			/// <summary>
			/// Nome da entrada.
			/// </summary>
			public string Name
			{
				get;
				private set;
			}

			/// <summary>
			/// Stream da entrada.
			/// </summary>
			public Stream Stream
			{
				get
				{
					return _stream.Value;
				}
			}

			/// <summary>
			/// Ultimo horário de escrita.
			/// </summary>
			public DateTime LastWriteTime
			{
				get;
				private set;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="name"></param>
			/// <param name="stream"></param>
			/// <param name="lastWriteTime"></param>
			public XapEntry(string name, Lazy<Stream> stream, DateTime lastWriteTime)
			{
				name.Require("name").NotNull().NotEmpty();
				stream.Require("stream").NotNull();
				this.Name = name;
				_stream = stream;
				this.LastWriteTime = lastWriteTime;
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				if(_stream != null && _stream.IsValueCreated)
				{
					_stream.Value.Dispose();
					_stream = null;
				}
			}
		}

		/// <summary>
		/// Gera o Xap para memória.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="directoryName"></param>
		/// <returns></returns>
		public static byte[] XapToMemory(IXapConfiguration configuration, string directoryName)
		{
			using (var ms = new MemoryStream())
			{
				XapFiles(configuration, new ZipArchive(ms, FileAccess.Write), directoryName);
				return ms.ToArray();
			}
		}

		/// <summary>
		/// Gera o Xap para memória.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="entries"></param>
		/// <returns></returns>
		public static byte[] XapToMemory(IXapConfiguration configuration, XapEntry[] entries)
		{
			using (var ms = new MemoryStream())
			{
				XapFiles(configuration, new ZipArchive(ms, FileAccess.Write), entries);
				return ms.ToArray();
			}
		}

		/// <summary>
		/// Gera o Xap para um arquivo para disco.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="directoryName"></param>
		/// <param name="xapfile"></param>
		public static void XapToDisk(IXapConfiguration configuration, string directoryName, string xapfile)
		{
			using (var archive = new ZipArchive(xapfile, FileAccess.Write))
				XapFiles(configuration, archive, directoryName);
		}

		/// <summary>
		/// Gera o Xap para um arquivo para disco.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="entries"></param>
		/// <param name="xapfile"></param>
		public static void XapToDisk(IXapConfiguration configuration, XapEntry[] entries, string xapfile)
		{
			using (var archive = new ZipArchive(xapfile, FileAccess.Write))
				XapFiles(configuration, archive, entries);
		}

		/// <summary>
		/// Adiciona os arquivos do diretório para o zip usando o padrão Xap.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="zip"></param>
		/// <param name="directoryName"></param>
		public static void XapFiles(IXapConfiguration configuration, ZipArchive zip, string directoryName)
		{
			ICollection<LanguageInfo> langs = FindSourceLanguages(configuration, directoryName);
			string manifestPath = Path.Combine(directoryName, "AppManifest.xaml");
			IList<Uri> assemblies;
			if(File.Exists(manifestPath))
				assemblies = GetManifestAssemblies(manifestPath);
			else
			{
				assemblies = GetLanguageAssemblies(configuration, langs);
				using (Stream appManifest = zip.Create("AppManifest.xaml"))
					configuration.ManifestTemplate.Generate(assemblies).Save(appManifest);
			}
			AddAssemblies(zip, directoryName, assemblies);
			GenerateLanguagesConfig(zip, langs);
			zip.CopyFromDirectory(directoryName, "");
			zip.Close();
		}

		/// <summary>
		/// Adiciona os arquivos do diretório para o zip usando o padrão Xap.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="zip"></param>
		/// <param name="entries"></param>
		public static void XapFiles(IXapConfiguration configuration, ZipArchive zip, XapEntry[] entries)
		{
			ICollection<LanguageInfo> langs = FindSourceLanguages(configuration, entries);
			IList<Uri> assemblies;
			assemblies = GetLanguageAssemblies(configuration, langs);
			using (Stream appManifest = zip.Create("AppManifest.xaml"))
				configuration.ManifestTemplate.Generate(assemblies).Save(appManifest);
			foreach (var entry in entries)
				zip.CopyFromStream(entry.Stream, entry.LastWriteTime, entry.Name);
			GenerateLanguagesConfig(zip, langs);
			zip.Close();
		}

		/// <summary>
		/// Recupera as uris dos assemblies do arquivo AppManifest.xaml.
		/// </summary>
		/// <param name="manifestPath"></param>
		/// <returns></returns>
		private static IList<Uri> GetManifestAssemblies(string manifestPath)
		{
			List<Uri> assemblies = new List<Uri>();
			var doc = new System.Xml.XmlDocument();
			doc.Load(manifestPath);
			foreach (System.Xml.XmlElement ap in doc.GetElementsByTagName("AssemblyPart"))
			{
				string src = ap.GetAttribute("Source");
				if(!string.IsNullOrEmpty(src))
					assemblies.Add(new Uri(src, UriKind.RelativeOrAbsolute));
			}
			return assemblies;
		}

		/// <summary>
		/// Gera o AppManifest.xaml.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="dir"></param>
		/// <returns></returns>
		internal static System.Xml.XmlDocument GenerateManifest(IXapConfiguration configuration, string dir)
		{
			return configuration.ManifestTemplate.Generate(GetLanguageAssemblies(configuration, FindSourceLanguages(configuration, dir)));
		}

		/// <summary>
		/// Recupera a lista de linguagens de assemblies que serão automaticamente adicionadas para o XAP.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="langs"></param>
		/// <returns></returns>
		private static IList<Uri> GetLanguageAssemblies(IXapConfiguration configuration, IEnumerable<LanguageInfo> langs)
		{
			List<Uri> assemblies = new List<Uri>();
			foreach (LanguageInfo lang in langs)
				foreach (string asm in lang.Assemblies)
					assemblies.Add(GetUri(configuration, asm));
			return assemblies;
		}

		/// <summary>
		/// Anexa o uriPrefix da configuração, a menos que o caminho seja absoluto. 
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		private static Uri GetUri(IXapConfiguration configuration, string path)
		{
			Uri uri = new Uri(path, UriKind.RelativeOrAbsolute);
			string prefix = configuration.UrlPrefix;
			if(prefix != "" && !IsPathRooted(uri))
				uri = new Uri(prefix + path, UriKind.RelativeOrAbsolute);
			return uri;
		}

		/// <summary>
		/// Verifica se a uri é absoluta ou inicia com '/'
		/// (i.e. absolute uri or absolute path)
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		private static bool IsPathRooted(Uri uri)
		{
			return uri.IsAbsoluteUri || uri.OriginalString.StartsWith("/");
		}

		/// <summary>
		/// Adiciona os assemblies com o caminho relativo para deintro do arquivo XAP.
		/// </summary>
		/// <param name="zip"></param>
		/// <param name="directoryName"></param>
		/// <param name="assemblyLocations"></param>
		private static void AddAssemblies(ZipArchive zip, string directoryName, IList<Uri> assemblyLocations)
		{
			foreach (Uri uri in assemblyLocations)
			{
				if(IsPathRooted(uri))
					continue;
				string targetPath = uri.OriginalString;
				string localPath = Path.Combine(directoryName, targetPath);
				if(!File.Exists(localPath))
					throw new ApplicationException("Could not find assembly: " + uri);
				zip.CopyFromFile(localPath, targetPath);
				string pdbPath = Path.ChangeExtension(localPath, ".pdb");
				string pdbTarget = Path.ChangeExtension(targetPath, ".pdb");
				if(File.Exists(pdbPath))
					zip.CopyFromFile(pdbPath, pdbTarget);
			}
		}

		/// <summary>
		/// Gera o arquivo languages.config
		/// </summary>
		/// <param name="zip"></param>
		/// <param name="langs"></param>
		private static void GenerateLanguagesConfig(ZipArchive zip, ICollection<LanguageInfo> langs)
		{
			bool needLangConfig = false;
			foreach (LanguageInfo lang in langs)
			{
				if(lang.LanguageContext != "")
				{
					needLangConfig = true;
					break;
				}
			}
			if(needLangConfig)
			{
				Stream outStream = zip.Create("languages.config");
				StreamWriter writer = new StreamWriter(outStream);
				writer.WriteLine("<Languages>");
				foreach (LanguageInfo lang in langs)
				{
					writer.WriteLine("  <Language languageContext=\"{0}\"", lang.LanguageContext);
					writer.WriteLine("            assembly=\"{0}\"", lang.GetContextAssemblyName());
					writer.WriteLine("            extensions=\"{0}\" />", lang.GetExtensionsString());
				}
				writer.WriteLine("</Languages>");
				writer.Close();
			}
		}

		/// <summary>
		/// Pesquisa o diretório das aplicações para encontrar todos os arquivo compatíveis com as linguagens configuradas.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="directoryName"></param>
		/// <returns></returns>
		internal static ICollection<LanguageInfo> FindSourceLanguages(IXapConfiguration configuration, string directoryName)
		{
			Dictionary<LanguageInfo, bool> result = new Dictionary<LanguageInfo, bool>();
			foreach (string file in Directory.GetFiles(directoryName, "*", SearchOption.AllDirectories))
			{
				string ext = Path.GetExtension(file);
				LanguageInfo lang;
				if(configuration.Languages.TryGetValue(ext.ToLower(), out lang))
					result[lang] = true;
			}
			return result.Keys;
		}

		/// <summary>
		/// Pesquisa o diretório das aplicações para encontrar todos os arquivo compatíveis com as linguagens configuradas.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="entries"></param>
		/// <returns></returns>
		internal static ICollection<LanguageInfo> FindSourceLanguages(IXapConfiguration configuration, XapEntry[] entries)
		{
			Dictionary<LanguageInfo, bool> result = new Dictionary<LanguageInfo, bool>();
			foreach (string file in entries.Select(f => f.Name))
			{
				string ext = Path.GetExtension(file);
				LanguageInfo lang;
				if(configuration.Languages.TryGetValue(ext.ToLower(), out lang))
					result[lang] = true;
			}
			return result.Keys;
		}
	}
}
