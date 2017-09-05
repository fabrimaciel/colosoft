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

namespace Colosoft.IO.Compression
{
	/// <summary>
	/// ZipArchive represents a Zip Archive.  It uses the System.IO.File structure as its guide 
	/// 
	/// The largest structual difference between a ZipArchive and the textStream system is that the archive has no
	/// independent notion of a 'directory'.  Instead files know their complete path name.  For the most
	/// part this difference is hard to notice, but does have some ramifications.  For example there is no
	/// concept of the modification time for a directory.    
	/// 
	/// TODO: Opening a textStream for Read/Write without truncation. 
	/// TODO: Allowing different text encodings
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public sealed class ZipArchive : IDisposable
	{
		/// <summary>
		/// Openes an existing ZIP archive 'archivePath' for reading.  
		/// </summary>
		/// <param name="archivePath"></param>
		public ZipArchive(string archivePath) : this(archivePath, FileAccess.Read)
		{
		}

		/// <summary>
		/// Opens a ZIP archive, 'archivePath'  If 'access' is ReadWrite or Write then the target 
		/// does not need to exist, but will be created with the ZipArchive is closed.  
		/// 
		/// If 'access' is ReadWrite the target can exist, and that data is used to initially
		/// populate the archive.  Any modifications that were made will be updated when the
		/// Close() method is called (and not before).  
		/// 
		/// If 'access' is Write then the target is either created or truncated to 0 before 
		/// the archive is written (thus the original data in the archiveFile is ignored).  
		/// </summary>
		public ZipArchive(string archivePath, FileAccess access)
		{
			entries = new SortedDictionary<string, ZipArchiveFile>(StringComparer.OrdinalIgnoreCase);
			this.archivePath = archivePath;
			this.access = access;
			if(access == FileAccess.Read)
				fromStream = new FileStream(archivePath, FileMode.Open, access);
			else if(access == FileAccess.ReadWrite)
				fromStream = new FileStream(archivePath, FileMode.OpenOrCreate, access);
			if(fromStream != null)
				Read(fromStream);
		}

		/// <summary>
		/// Read an archive from an exiting stream or write a new archive into a stream
		/// </summary>
		public ZipArchive(Stream fromStream, FileAccess desiredAccess)
		{
			entries = new SortedDictionary<string, ZipArchiveFile>(StringComparer.OrdinalIgnoreCase);
			this.access = desiredAccess;
			this.fromStream = fromStream;
			if((desiredAccess & FileAccess.Read) != 0)
			{
				if(!fromStream.CanRead)
					throw new Exception("Error: Can't read from stream.");
				Read(fromStream);
			}
			else if((desiredAccess & FileAccess.Write) != 0)
			{
				if(!fromStream.CanWrite)
					throw new Exception("Error: Can't write to stream.");
			}
		}

		/// <summary>
		/// Enumerate the files in the archive (directories don't have an independent existance).
		/// </summary>
		public IEnumerable<ZipArchiveFile> Files
		{
			get
			{
				return entries.Values;
			}
		}

		/// <summary>
		/// Returns a subset of the files in the archive that are in the directory 'archivePath'.  If
		/// searchOptions is TopDirectoryOnly only files in the directory 'archivePath' are returns. 
		/// If searchOptions is AllDirectories then all files that are in subdiretories are also returned. 
		/// </summary>
		public IEnumerable<ZipArchiveFile> GetFilesInDirectory(string archivePath, SearchOption searchOptions)
		{
			foreach (ZipArchiveFile entry in entries.Values)
			{
				string name = entry.Name;
				if(name.StartsWith(archivePath, StringComparison.OrdinalIgnoreCase) && name.Length > archivePath.Length)
				{
					if(searchOptions == SearchOption.TopDirectoryOnly)
					{
						if(name.IndexOf(Path.DirectorySeparatorChar, archivePath.Length + 1) >= 0)
							continue;
					}
					yield return entry;
				}
			}
		}

		/// <summary>
		/// Fetch a archiveFile by name.  'archivePath' is the full path name of the archiveFile in the archive.  
		/// It returns null if the name does not exist (and e
		/// </summary>
		public ZipArchiveFile this[string archivePath]
		{
			get
			{
				ZipArchiveFile ret = null;
				entries.TryGetValue(archivePath, out ret);
				return ret;
			}
		}

		/// <summary>
		/// Open the archive textStream 'archivePath' for reading and returns the resulting Stream.
		/// KeyNotFoundException is thrown if 'archivePath' does not exist
		/// </summary>
		public Stream OpenRead(string archivePath)
		{
			return entries[archivePath].OpenRead();
		}

		/// <summary>
		/// Opens the archive textStream 'archivePath' for writing and returns the resulting Stream. If the textStream
		/// already exists, it is truncated to be an empty textStream.
		/// </summary>
		public Stream Create(string archivePath)
		{
			ZipArchiveFile newEntry;
			if(!entries.TryGetValue(archivePath, out newEntry))
				newEntry = new ZipArchiveFile(this, archivePath);
			return newEntry.Create();
		}

		/// <summary>
		/// Returns true if the archive can not be written to (it was opend with FileAccess.Read). 
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return access == FileAccess.Read;
			}
			set
			{
				if(fromStream != null)
				{
					if(value == true)
					{
						if(!fromStream.CanRead)
							throw new Exception("Can't read from stream");
						access = FileAccess.Read;
					}
					else
					{
						if(fromStream.CanWrite == false)
							throw new ArgumentException("Can't reset IsReadOnly on a ZipArchive whose stream is ReadOnly.");
						access = (fromStream.CanRead) ? FileAccess.ReadWrite : FileAccess.Write;
					}
				}
				else
				{
					access = value ? FileAccess.Read : FileAccess.ReadWrite;
				}
			}
		}

		/// <summary>
		/// Closes the archive.  Until this call is made any pending modifications to the archive are NOT
		/// made (the archive is unchanged).  
		/// </summary>
		public void Close()
		{
			Flush();
			fromStream.Close();
		}

		/// <summary>
		/// Libera o conteúdo do arquivo.
		/// </summary>
		public void Flush()
		{
			closeCalled = true;
			if(!IsReadOnly)
			{
				if(fromStream == null)
				{
					System.Diagnostics.Debug.Assert(archivePath != null);
					System.Diagnostics.Debug.Assert(access == FileAccess.Write);
					fromStream = new FileStream(archivePath, FileMode.Create);
				}
				fromStream.Position = 0;
				fromStream.SetLength(0);
				foreach (ZipArchiveFile entry in entries.Values)
				{
					entry.WriteToStream(fromStream);
				}
				WriteArchiveDirectoryToStream(fromStream);
			}
		}

		/// <summary>
		/// Remove all files from the archive. 
		/// </summary>
		public void Clear()
		{
			entries.Clear();
		}

		/// <summary>
		/// Count of total number of files (does not include directories) in the archive. 
		/// </summary>
		public int Count
		{
			get
			{
				return entries.Count;
			}
		}

		/// <summary>
		/// Returns true if 'archivePath' exists in the archive.  
		/// </summary>
		/// <returns></returns>
		public bool Exists(string archivePath)
		{
			return entries.ContainsKey(archivePath);
		}

		/// <summary>
		///  Renames sourceArchivePath to destinationArchivePath.  If destinationArchivePath exists it is
		///  discarded.  
		/// </summary>
		public void Move(string sourceArchivePath, string destinationArchivePath)
		{
			entries[sourceArchivePath].MoveTo(destinationArchivePath);
		}

		/// <summary>
		/// Delete 'archivePath'.  It returns true if successful.  If archivePath does not exist, it
		/// simply returns false (no exception is thrown).  The delete succeeds even if streams on the
		/// data exists (they continue to exist, but will not be persisted on Close()
		/// </summary>
		public bool Delete(string archivePath)
		{
			ZipArchiveFile entry;
			if(!entries.TryGetValue(archivePath, out entry))
				return false;
			entry.Delete();
			return true;
		}

		/// <summary>
		/// Copies the archive textStream 'sourceArchivePath' to the textStream system textStream 'targetFilePath'. 
		/// It will overwrite existing files, however a locked targetFilePath will cause an exception.  
		/// </summary>
		/// <param name="sourceArchivePath"></param>
		/// <param name="targetFilePath"></param>
		public void CopyToFile(string sourceArchivePath, string targetFilePath)
		{
			entries[sourceArchivePath].CopyToFile(targetFilePath, true);
		}

		/// <summary>
		/// Copies the archive textStream 'sourceArchivePath' to the textStream system textStream 'targetFilePath'. 
		/// It will overwrite existing files, however a locked targetFilePath will cause an exception.  
		/// </summary>
		/// <param name="sourceArchivePath"></param>
		/// <param name="targetFilePath"></param>
		/// <param name="canOverride"></param>
		public void CopyToFile(string sourceArchivePath, string targetFilePath, bool canOverride)
		{
			entries[sourceArchivePath].CopyToFile(targetFilePath, canOverride);
		}

		/// <summary>
		/// Copyies 'sourceFilePath from the textStream system to the archive as 'targetArchivePath'
		/// It will overwrite any existing textStream.
		/// </summary>
		public void CopyFromFile(string sourceFilePath, string targetArchivePath)
		{
			using (Stream inFile = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite))
				using (Stream outFile = Create(targetArchivePath))
					ZipArchiveFile.CopyStream(inFile, outFile);
			this[targetArchivePath].LastWriteTime = File.GetLastWriteTime(sourceFilePath);
		}

		/// <summary>
		/// Copyies 'sourceFilePath from the textStream system to the archive as 'targetArchivePath'
		/// It will overwrite any existing textStream.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="lastWriteTime"></param>
		/// <param name="targetArchivePath"></param>
		public void CopyFromStream(System.IO.Stream stream, DateTime lastWriteTime, string targetArchivePath)
		{
			using (Stream outFile = Create(targetArchivePath))
				ZipArchiveFile.CopyStream(stream, outFile);
			this[targetArchivePath].LastWriteTime = lastWriteTime;
		}

		/// <summary>
		/// Deletes all files in the directory (and subdirectories) of 'archivePath'.  
		/// </summary>
		public int DeleteDirectory(string archivePath)
		{
			int ret = 0;
			List<ZipArchiveFile> entriesToDelete = new List<ZipArchiveFile>(GetFilesInDirectory(archivePath, SearchOption.AllDirectories));
			foreach (ZipArchiveFile entry in entriesToDelete)
			{
				entry.Delete();
				ret++;
			}
			return ret;
		}

		/// <summary>
		/// Copies (recursively the files in archive directory to a textStream system directory.
		/// </summary>
		/// <param name="sourceArchiveDirectory">The name of the source directory in the archive</param>
		/// <param name="targetDirectory">The target directory in the textStream system to copy to. 
		/// If it is empty it represents all files in the archive. </param>
		public void CopyToDirectory(string sourceArchiveDirectory, string targetDirectory)
		{
			foreach (ZipArchiveFile entry in GetFilesInDirectory(sourceArchiveDirectory, SearchOption.AllDirectories))
			{
				string relativePath = GetRelativePath(entry.Name, sourceArchiveDirectory);
				entry.CopyToFile(Path.Combine(targetDirectory, relativePath), true);
			}
		}

		/// <summary>
		/// Copies (recursively the files in archive directory to a textStream system directory.
		/// </summary>
		/// <param name="sourceArchiveDirectory">The name of the source directory in the archive</param>
		/// <param name="targetDirectory">The target directory in the textStream system to copy to. 
		/// If it is empty it represents all files in the archive. </param>
		/// <param name="canOverride"></param>
		public void CopyToDirectory(string sourceArchiveDirectory, string targetDirectory, bool canOverride)
		{
			foreach (ZipArchiveFile entry in GetFilesInDirectory(sourceArchiveDirectory, SearchOption.AllDirectories))
			{
				string relativePath = GetRelativePath(entry.Name, sourceArchiveDirectory);
				entry.CopyToFile(Path.Combine(targetDirectory, relativePath), canOverride);
			}
		}

		/// <summary>
		/// Copies a directory recursively from the textStream system to the archive.  
		/// </summary>
		/// <param name="sourceDirectory">The direcotry in the textStream system to copy to the archive</param>
		/// <param name="targetArchiveDirectory">
		/// The directory in the archive to copy to.  An empty string means the top level of the archive</param>
		public void CopyFromDirectory(string sourceDirectory, string targetArchiveDirectory)
		{
			foreach (string path in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
			{
				string relativePath = GetRelativePath(path, sourceDirectory);
				CopyFromFile(path, Path.Combine(targetArchiveDirectory, relativePath));
			}
		}

		/// <summary>
		/// Open an existing textStream in the archive for reading as text and returns the resulting StreamReader.  
		/// </summary>
		public StreamReader OpenText(string archivePath)
		{
			return entries[archivePath].OpenText();
		}

		/// <summary>
		/// Opens a textStream in the archive for writing as a text textStream.  Returns the resulting TextWriter.  
		/// </summary>
		public TextWriter CreateText(string archivePath)
		{
			ZipArchiveFile newEntry;
			if(!entries.TryGetValue(archivePath, out newEntry))
				newEntry = new ZipArchiveFile(this, archivePath);
			return newEntry.CreateText();
		}

		/// <summary>
		/// Reads all the data in 'archivePath' as a text string and returns it. 
		/// </summary>
		public string ReadAllText(string archivePath)
		{
			return entries[archivePath].ReadAllText();
		}

		/// <summary>
		/// Overwrites the archive textStream 'archivePath' with the text in 'data'
		/// </summary>
		public void WriteAllText(string archivePath, string data)
		{
			ZipArchiveFile newEntry;
			if(!entries.TryGetValue(archivePath, out newEntry))
				newEntry = new ZipArchiveFile(this, archivePath);
			newEntry.WriteAllText(data);
		}

		/// <summary>
		/// Returns a string reprentation of the archive (its name if known, and count of files)
		/// Mostly useful in the debugger.  
		/// </summary>
		public override string ToString()
		{
			string name = archivePath;
			if(archivePath == null)
				name = "<fromStream>";
			return "ZipArchive " + name + " FileCount = " + entries.Count;
		}

		internal SortedDictionary<string, ZipArchiveFile> entries;

		internal Stream fromStream;

		private FileAccess access;

		private string archivePath;

		private bool closeCalled;

		/// <summary>
		/// Destrutor padrão.
		/// </summary>
		~ZipArchive()
		{
			System.Diagnostics.Debug.Assert(access == FileAccess.Read || closeCalled || entries.Count == 0, "Did not close a writable archive (use clear to abandon it)");
		}

		internal static string GetRelativePath(string fileName, string directory)
		{
			System.Diagnostics.Debug.Assert(fileName.StartsWith(directory), "directory not a prefix");
			int directoryEnd = directory.Length;
			if(directoryEnd == 0)
				return fileName;
			while (directoryEnd < fileName.Length && fileName[directoryEnd] == Path.DirectorySeparatorChar)
				directoryEnd++;
			string relativePath = fileName.Substring(directoryEnd);
			return relativePath;
		}

		private void Read(Stream archiveStream)
		{
			for(; ;)
			{
				ZipArchiveFile entry = ZipArchiveFile.Read(this);
				if(entry == null)
					break;
			}
		}

		private void WriteArchiveDirectoryToStream(Stream writer)
		{
			long startOfDirectory = writer.Position;
			foreach (ZipArchiveFile entry in entries.Values)
				entry.WriteArchiveDirectoryEntryToStream(writer);
			long endOfDirectory = writer.Position;
			ByteBuffer trailer = new ByteBuffer(22);
			trailer.WriteUInt32(ZipArchiveFile.SignatureArchiveDirectoryEnd);
			trailer.WriteUInt16(ZipArchiveFile.DiskNumberStart);
			trailer.WriteUInt16(ZipArchiveFile.DiskNumberStart);
			trailer.WriteUInt16((ushort)entries.Count);
			trailer.WriteUInt16((ushort)entries.Count);
			trailer.WriteUInt32((uint)(endOfDirectory - startOfDirectory));
			trailer.WriteUInt32((uint)startOfDirectory);
			trailer.WriteUInt16((ushort)ZipArchiveFile.FileCommentLength);
			trailer.WriteContentsTo(writer);
		}

		/// <summary>
		/// Dispose instance.
		/// </summary>
		public void Dispose()
		{
			if(!closeCalled)
				Close();
		}
	}
}
