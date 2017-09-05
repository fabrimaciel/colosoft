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

namespace Colosoft.Data.Caching.Local
{
	/// <summary>
	/// Implementação de um repositório local.
	/// </summary>
	public class DataEntriesRepository : IDataEntriesRepository
	{
		/// <summary>
		/// Gera o nome do arquivo associado com a entrada.
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		private string MakeEntryFileName(DataEntry entry)
		{
			return MakeEntryFileName(entry.TypeName);
		}

		/// <summary>
		/// Gera o nome do arquivo associado com a entrada.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private string MakeEntryFileName(Colosoft.Reflection.TypeName typeName)
		{
			return System.IO.Path.Combine(GetLocalPath(), GetMD5Hash(typeName.AssemblyQualifiedName) + ".cache");
		}

		/// <summary>
		/// Recupera o hash MD5 do texto informado.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		private static string GetMD5Hash(string input)
		{
			using (System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider())
			{
				byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
				bs = x.ComputeHash(bs);
				System.Text.StringBuilder s = new System.Text.StringBuilder();
				foreach (byte b in bs)
					s.Append(b.ToString("x2").ToLower());
				string password = s.ToString();
				return password;
			}
		}

		/// <summary>
		/// Verifica se o erro identifica que o arquivo está bloqueado.
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		private static bool IsFileLocked(Exception exception)
		{
			int errorCode = System.Runtime.InteropServices.Marshal.GetHRForException(exception) & ((1 << 16) - 1);
			return errorCode == 32 || errorCode == 33;
		}

		/// <summary>
		/// Recupera o caminho local onde os dados do repositório são armazenados.
		/// </summary>
		/// <returns></returns>
		public static string GetLocalPath()
		{
			return System.IO.Path.Combine(Colosoft.IO.IsolatedStorage.IsolatedStorage.AuthenticationContextDirectory, "Caching");
		}

		/// <summary>
		/// Recupera as versões das entradas carregadas no cache.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<DataEntryVersion> GetEntryVersions()
		{
			var directory = GetLocalPath();
			if(System.IO.Directory.Exists(directory))
			{
				foreach (var i in System.IO.Directory.GetFiles(directory, "*.cache"))
				{
					DataEntryVersion version = null;
					try
					{
						using (var stream = System.IO.File.OpenRead(i))
						{
							var reader = new Colosoft.Serialization.IO.CompactBinaryReader(stream, Colosoft.Text.Encoding.Default);
							version = new DataEntryVersion();
							((Colosoft.Serialization.ICompactSerializable)version).Deserialize(reader);
						}
					}
					catch
					{
						continue;
					}
					yield return version;
				}
			}
		}

		/// <summary>
		/// Recupera as entradas carregadas para o cache.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<DataEntry> GetEntries()
		{
			var directory = GetLocalPath();
			if(System.IO.Directory.Exists(directory))
			{
				foreach (var i in System.IO.Directory.GetFiles(directory, "*.cache"))
				{
					DataEntry entry = null;
					System.IO.Stream stream = null;
					try
					{
						stream = System.IO.File.OpenRead(i);
						var reader = new Colosoft.Serialization.IO.CompactBinaryReader(stream, Colosoft.Text.Encoding.Default);
						entry = new DataEntry();
						((Colosoft.Serialization.ICompactSerializable)entry).Deserialize(reader);
					}
					catch
					{
						if(stream != null)
							stream.Dispose();
						continue;
					}
					yield return entry;
				}
			}
		}

		/// <summary>
		/// Recupera as entradas carregadas para o cache.
		/// </summary>
		/// <param name="typeNames">Nomes do tipos da entradas que serão carregadas.</param>
		/// <returns></returns>
		public IEnumerable<DataEntry> GetEntries(Colosoft.Reflection.TypeName[] typeNames)
		{
			typeNames.Require("typeNames").NotNull();
			if(typeNames.Length == 0)
				yield break;
			var hashNames = typeNames.Select(f => MakeEntryFileName(f)).ToArray();
			var directory = GetLocalPath();
			if(System.IO.Directory.Exists(directory))
			{
				foreach (var i in hashNames.Where(f => System.IO.File.Exists(f)))
				{
					DataEntry entry = null;
					System.IO.Stream stream = null;
					try
					{
						using (stream = System.IO.File.OpenRead(i))
						{
							var reader = new Colosoft.Serialization.IO.CompactBinaryReader(stream, Colosoft.Text.Encoding.Default);
							entry = new DataEntry();
							((Colosoft.Serialization.ICompactSerializable)entry).Deserialize(reader);
						}
					}
					catch
					{
						continue;
					}
					yield return entry;
				}
			}
		}

		/// <summary>
		/// Insere um nova entrada no repositório.
		/// </summary>
		/// <param name="entry"></param>
		public bool Insert(DataEntry entry)
		{
			entry.Require("entry").NotNull();
			var directory = GetLocalPath();
			try
			{
				if(!System.IO.Directory.Exists(directory))
					System.IO.Directory.CreateDirectory(directory);
				using (var stream = System.IO.File.OpenWrite(MakeEntryFileName(entry)))
				{
					var writer = new Colosoft.Serialization.IO.CompactBinaryWriter(stream);
					((Colosoft.Serialization.ICompactSerializable)entry).Serialize(writer);
				}
				return true;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.Fail(ex.Message);
				return false;
			}
		}

		/// <summary>
		/// Insere uma nova entrada no respositório
		/// </summary>
		/// <param name="version">Versão da entrada.</param>
		/// <param name="stream">Stream com os dados.</param>
		/// <returns></returns>
		public bool Insert(DataEntryVersion version, System.IO.Stream stream)
		{
			version.Require("version").NotNull();
			stream.Require("stream").NotNull();
			var directory = GetLocalPath();
			try
			{
				if(!System.IO.Directory.Exists(directory))
					System.IO.Directory.CreateDirectory(directory);
				using (var outStream = System.IO.File.OpenWrite(MakeEntryFileName(version.TypeName)))
				{
					var buffer = new byte[1024];
					for(var read = 0; (read = stream.Read(buffer, 0, buffer.Length)) > 0;)
						outStream.Write(buffer, 0, read);
				}
				return true;
			}
			catch(Exception ex)
			{
				if(!IsFileLocked(ex))
					System.Diagnostics.Debug.Fail(ex.Message);
				return false;
			}
		}
	}
}
