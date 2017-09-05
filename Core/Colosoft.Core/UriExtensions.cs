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

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;

namespace System
{
	/// <summary>
	/// Classe com os métodos para auxiliar a manipulação de Uri.
	/// </summary>
	public static class UriExtensions
	{
		private static Dictionary<string, System.Resources.ResourceManager> _resources = new Dictionary<string, System.Resources.ResourceManager>();

		/// <summary>
		/// Indica que uma URI é uma pacote do tipo VirtualStorage.
		/// </summary>
		/// <param name="uri">URI a ser analisada.</param>
		/// <returns>Retorna verdadeiro se a URI é um pacote do tipo VirtualStorage</returns>
		public static bool IsPackVirtualStorage(this Uri uri)
		{
			return uri.IsAbsoluteUri && string.Compare(uri.Scheme, "pack", StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(GetPackageUri(uri).GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped), "virtualstorage:///", StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <summary>
		/// Indica que uma URI é um pacote do tipo aplicação.
		/// </summary>
		/// <param name="uri">URI a ser analisada.</param>
		/// <returns>Retorna verdadeiro se a URI é um pacote do tipo aplicação.</returns>
		public static bool IsPackApplication(this Uri uri)
		{
			return uri.IsAbsoluteUri && string.Compare(uri.Scheme, "pack", StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(GetPackageUri(uri).GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped), "application:///", StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <summary>
		/// Indica que uma URI é um pacote do tipo local de origem.
		/// </summary>
		/// <param name="uri">URI a ser analisada.</param>
		/// <returns>Retorna verdadeiro se a URI é um pacote do local de origem.</returns>
		public static bool IsPackSiteOfOrigin(this Uri uri)
		{
			return uri.IsAbsoluteUri && string.Compare(uri.Scheme, "pack", StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(GetPackageUri(uri).GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped), "siteoforigin:///", StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <summary>
		/// Indica que uma URI é um pacote do tipo Zip.
		/// </summary>
		/// <param name="uri">URI a ser analisada.</param>
		/// <returns>Retorna verdadeiro se a URI é um pacote do tipo Zip.</returns>
		public static bool IsPackZip(this Uri uri)
		{
			return uri.IsAbsoluteUri && string.Compare(uri.Scheme, "pack", StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(GetPackageUri(uri).GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped), 0, "zip:///", 0, "zip:///".Length, StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <summary>
		/// Obtém uma Stream do recurso a partir da sua URI.
		/// </summary>
		/// <param name="uri">URI do recurso.</param>
		/// <param name="assembly">Assembly padrão.</param>
		/// <returns>Stream do recurso.</returns>
		/// <example>
		/// File System: "file:///C:/Folder/Resource.bin"
		/// File in Site: "http://www.website.com/Resource.bin"
		/// File in FTP: "ftp://ftp.ftpserver.com/Resource.bin"
		/// Absolute Pack - Resource File: "pack://application:,,,/Folder/File.bin"
		/// Absolute Pack - Content File: "pack://application:,,,/Folder/File.bin"
		/// Absolute Pack - Site of Origin File: "pack://siteoforigin:,,,/Folder/File.bin"
		/// Absolute Pack - Zip File: "pack://zip:,,,C:\\Folder\\Package.package/Folder/File.bin"
		/// Relative - (Resource -> Pack Content -> Pack Resource -> Pack Site of Origin) Resource: "/Folder/Resource.bin"
		/// Relative - (Resource -> Pack Content -> Pack Resource -> Pack Site of Origin) Zip File: "pack://zip:,,,Folder\\Resource.package/Folder/File.bin"
		/// </example>
		public static Stream GetStream(this Uri uri, Assembly assembly = null)
		{
			if(!uri.IsAbsoluteUri)
			{
				string key;
				string name;
				string version;
				string partName;
				if(GetAssemblyNameAndPart(uri, out partName, out name, out version, out key))
				{
					if(!string.IsNullOrEmpty(name))
						try
						{
							assembly = GetLoadedAssembly(name, version, key);
						}
						catch(IOException)
						{
							assembly = null;
						}
					else if(assembly == null)
						assembly = Assembly.GetCallingAssembly();
					if(assembly != null)
					{
						Stream stream = GetResourceStream(partName, assembly);
						if(stream != null)
							return stream;
					}
				}
			}
			else if(uri.IsFile)
				return new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			else if(uri.IsPackVirtualStorage())
			{
				var localFile = Colosoft.IO.VirtualStorage.Storage.GetFilePath(uri);
				if(!string.IsNullOrEmpty(localFile))
					return new FileStream(localFile, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			var uriManager = Colosoft.UriManager.Current;
			if(uriManager != null)
			{
				if(!uri.IsAbsoluteUri || uri.IsPackApplication())
				{
					Stream stream = uriManager.GetApplicationStream(uri);
					if(stream != null)
						return stream;
				}
				if(!uri.IsAbsoluteUri || uri.IsPackSiteOfOrigin())
				{
					Stream stream = uriManager.GetRemoteStream(uri);
					if(stream != null)
						return stream;
				}
				if(uri.IsPackZip())
				{
					Stream stream = uriManager.GetPackZipStream(uri, assembly);
					if(stream != null)
						return stream;
				}
				return uriManager.GetStream(uri);
			}
			if(!uri.IsAbsoluteUri)
				return null;
			try
			{
				WebRequest request = WebRequest.Create(uri);
				WebResponse response = request.GetResponse();
				return response.GetResponseStream();
			}
			catch(NotSupportedException)
			{
				return null;
			}
			catch(NotImplementedException)
			{
				return null;
			}
		}

		/// <summary>
		/// Obtém o URI para um arquivo a partir do URI do recurso. Se necessário será criado um arquivo temporário.
		/// </summary>
		/// <param name="uri">URI do recurso.</param>
		/// <param name="assembly">Assembly padrão.</param>
		/// <returns>URI para o arquivo.</returns>
		public static Uri FileUri(this Uri uri, Assembly assembly = null)
		{
			if(uri.IsAbsoluteUri && uri.IsFile)
				return uri;
			if(uri.IsPackVirtualStorage())
			{
				var localFile = Colosoft.IO.VirtualStorage.Storage.GetFilePath(uri);
				if(!string.IsNullOrEmpty(localFile))
					return new Uri(localFile);
			}
			if(assembly == null && !uri.IsAbsoluteUri)
				assembly = Assembly.GetCallingAssembly();
			using (Stream stream = uri.GetStream(assembly))
			{
				var fileStream = stream as FileStream;
				if(fileStream != null)
					return new Uri(fileStream.Name);
				string path = Path.GetTempFileName();
				using (fileStream = new FileStream(path, FileMode.Create))
				{
					int count;
					byte[] buffer = new byte[1024 * 1024];
					while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
					{
						fileStream.Write(buffer, 0, count);
					}
					fileStream.Flush();
				}
				return new Uri(path);
			}
		}

		private static Uri GetPackageUri(Uri packUri)
		{
			string text = packUri.GetComponents(UriComponents.HostAndPort, UriFormat.UriEscaped);
			text = text.Replace(',', '/');
			Uri uri = new Uri(Uri.UnescapeDataString(text));
			return uri;
		}

		private static bool IsSameKeyToken(byte[] reqKeyToken, byte[] curKeyToken)
		{
			if(reqKeyToken == null || curKeyToken == null || reqKeyToken.Length != curKeyToken.Length)
				return false;
			for(int i = 0; i < reqKeyToken.Length; i++)
				if(reqKeyToken[i] != curKeyToken[i])
					return false;
			return true;
		}

		private static Assembly GetLoadedAssembly(string name, string version, string key)
		{
			Version asmVersion = null;
			if(!string.IsNullOrEmpty(version))
				asmVersion = new Version(version);
			byte[] keyToken = null;
			if(!string.IsNullOrEmpty(key))
			{
				int count = key.Length >> 1;
				keyToken = new byte[count];
				for(int i = 0; i < count; i++)
				{
					string byteString = key.Substring(i + i, 2);
					keyToken[i] = byte.Parse(byteString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
				}
			}
			int index;
			Assembly assembly = null;
			AssemblyName assemblyName;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for(index = assemblies.Length - 1; index >= 0; index--)
			{
				assembly = assemblies[index];
				assemblyName = assembly.GetName();
				if((string.Compare(assemblyName.Name, name, true, Colosoft.Globalization.Culture.InvariantEnglishUS) == 0) && (asmVersion == null || asmVersion.Equals(assemblyName.Version)) && (keyToken == null || IsSameKeyToken(keyToken, assemblyName.GetPublicKeyToken())))
					break;
			}
			if(index < 0)
			{
				assemblyName = new AssemblyName(name);
				assemblyName.CultureInfo = new CultureInfo(string.Empty);
				if(asmVersion != null)
					assemblyName.Version = asmVersion;
				if(keyToken != null)
					assemblyName.SetPublicKeyToken(keyToken);
				assembly = Assembly.Load(assemblyName);
			}
			return assembly;
		}

		private static bool GetAssemblyNameAndPart(Uri uri, out string partName, out string name, out string version, out string key)
		{
			string original = uri.ToString();
			int start = (original[0] == '/') ? 1 : 0;
			int end;
			partName = original.Substring(start);
			name = string.Empty;
			version = string.Empty;
			key = string.Empty;
			end = original.IndexOf('/', start);
			bool hasAssemblyInfo = false;
			string firstSegment = string.Empty;
			if(end > 0)
			{
				firstSegment = original.Substring(start, end - start);
				if(firstSegment.EndsWith(";component", StringComparison.OrdinalIgnoreCase))
					return false;
				if(hasAssemblyInfo = firstSegment.EndsWith(";", StringComparison.Ordinal))
					partName = original.Substring(end + 1);
			}
			if(hasAssemblyInfo)
			{
				string[] assemblyInfo = firstSegment.Split(';');
				int count = assemblyInfo.Length;
				if((count > 4) || (count < 2))
					throw new UriFormatException(Colosoft.Properties.Resources.WrongFirstSegment);
				name = Uri.UnescapeDataString(assemblyInfo[0]);
				for(int i = 1; i < count - 1; i++)
				{
					if(assemblyInfo[i].StartsWith("v", StringComparison.OrdinalIgnoreCase))
						if(string.IsNullOrEmpty(version))
							version = assemblyInfo[i].Substring(1);
						else
							throw new UriFormatException(Colosoft.Properties.Resources.WrongFirstSegment);
					else if(string.IsNullOrEmpty(key))
						key = assemblyInfo[i];
					else
						throw new UriFormatException(Colosoft.Properties.Resources.WrongFirstSegment);
				}
			}
			return true;
		}

		private static ResourceManager GetResourceManager(AssemblyName assemblyName, Assembly assembly)
		{
			ResourceManager result;
			string key = assemblyName.FullName;
			lock (_resources)
			{
				if(!_resources.TryGetValue(key, out result))
					_resources.Add(key, result = new ResourceManager(assemblyName.Name, assembly));
			}
			return result;
		}

		private static Stream GetResourceStream(string partName, Assembly assembly)
		{
			AssemblyName assemblyName = assembly.GetName();
			string baseName = assemblyName.Name;
			Stream stream = assembly.GetManifestResourceStream(baseName + '.' + partName);
			if(stream != null)
				return stream;
			string resourceName = partName;
			int index = partName.IndexOf('/');
			if(index != -1)
			{
				baseName += '.' + partName.Substring(0, index);
				resourceName = partName.Substring(index + 1);
				assemblyName.Name = baseName;
			}
			ResourceManager resourceManager = GetResourceManager(assemblyName, assembly);
			object resource = null;
			try
			{
				resource = resourceManager.GetObject(resourceName, CultureInfo.CurrentUICulture);
			}
			catch(MissingManifestResourceException)
			{
			}
			if(resource == null)
				return null;
			Type type = resource.GetType();
			TypeCode code = Type.GetTypeCode(type);
			if(code == TypeCode.Object)
			{
				if(type.IsArray && type.GetElementType() == typeof(byte))
					return new MemoryStream((byte[])resource);
				stream = resource as MemoryStream;
				if(stream != null)
					return stream;
				stream = new MemoryStream();
				try
				{
					BinaryFormatter formater = new BinaryFormatter();
					formater.Serialize(stream, resource);
					stream.Position = 0;
					return stream;
				}
				catch(Exception)
				{
					stream.Dispose();
					throw;
				}
			}
			if(code < TypeCode.Boolean)
				return null;
			stream = new MemoryStream();
			try
			{
				BinaryWriter writer = new BinaryWriter(stream);
				switch(code)
				{
				case TypeCode.Boolean:
					writer.Write((bool)resource);
					break;
				case TypeCode.Char:
					writer.Write((char)resource);
					break;
				case TypeCode.SByte:
					writer.Write((sbyte)resource);
					break;
				case TypeCode.Byte:
					writer.Write((byte)resource);
					break;
				case TypeCode.Int16:
					writer.Write((short)resource);
					break;
				case TypeCode.UInt16:
					writer.Write((ushort)resource);
					break;
				case TypeCode.Int32:
					writer.Write((int)resource);
					break;
				case TypeCode.UInt32:
					writer.Write((uint)resource);
					break;
				case TypeCode.Int64:
					writer.Write((long)resource);
					break;
				case TypeCode.UInt64:
					writer.Write((ulong)resource);
					break;
				case TypeCode.Single:
					writer.Write((float)resource);
					break;
				case TypeCode.Double:
					writer.Write((double)resource);
					break;
				case TypeCode.Decimal:
					writer.Write((decimal)resource);
					break;
				case TypeCode.DateTime:
					writer.Write(((DateTime)resource).ToBinary());
					break;
				case TypeCode.String:
					char[] chars = ((string)resource).ToCharArray();
					int len = chars.Length + chars.Length;
					byte[] bytes = new byte[len];
					Buffer.BlockCopy(chars, 0, bytes, 0, len);
					writer.Write(bytes);
					break;
				default:
					stream.Dispose();
					return null;
				}
				stream.Position = 0;
				return stream;
			}
			catch(Exception)
			{
				stream.Dispose();
				throw;
			}
		}
	}
}
