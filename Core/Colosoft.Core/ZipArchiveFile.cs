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
	/// ZipArchiveFile represents one archiveFile in the ZipArchive.   It is analogous to the System.IO.DiffFile
	/// object for normal files.  
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public sealed class ZipArchiveFile
	{
		/// <summary>
		/// Truncates the archiveFile represented by the ZipArchiveFile to be empty and returns a Stream that can be used
		/// to write (binary) data into it.
		/// </summary>
		/// <returns>A Stream that can be written on. </returns>
		public Stream Create()
		{
			if(IsReadOnly)
				throw new ApplicationException("Archive is ReadOnly");
			if(uncompressedData != null && (uncompressedData.CanWrite || uncompressedData.CanRead))
				throw new ApplicationException("ZipArchiveFile already open.");
			compressedData = null;
			positionOfCompressedDataInArchive = 0;
			compressedLength = 0;
			uncompressedData = new RepairedMemoryStream(256);
			return uncompressedData;
		}

		/// <summary>
		/// Opens the archiveFile represented by the ZipArchiveFile and returns a stream that can use to read (binary) data.
		/// </summary>
		/// <returns>A Stream that can be read from.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public Stream OpenRead()
		{
			if(uncompressedData == null)
			{
				if(compressedData == null)
				{
					compressedData = new byte[compressedLength];
					archive.fromStream.Seek(positionOfCompressedDataInArchive, SeekOrigin.Begin);
					archive.fromStream.Read(compressedData, 0, compressedLength);
				}
				MemoryStream compressedReader = new MemoryStream(compressedData);
				if(compressionMethod == CompressionMethod.None)
					return compressedReader;
				else
					return new System.IO.Compression.DeflateStream(compressedReader, System.IO.Compression.CompressionMode.Decompress);
			}
			else
			{
				if(uncompressedData.CanWrite)
					throw new ApplicationException("ZipArchiveFile still open for writing.");
				return new MemoryStream(uncompressedData.GetBuffer(), 0, (int)uncompressedData.Length, false);
			}
		}

		/// <summary>
		/// Truncates the archiveFile represented by the ZipArchiveFile to be empty and returns a TextWriter that text
		/// can be written to (using the default encoding). 
		/// </summary>
		/// <returns>The TextWriter that text can be written to. </returns>
		public void MoveTo(string newArchivePath)
		{
			if(IsReadOnly)
				throw new ApplicationException("Archive is ReadOnly");
			archive.entries.Remove(name);
			name = newArchivePath;
			archive.entries[newArchivePath] = this;
		}

		/// <summary>
		/// Delete the archiveFile represented by the ZipArchiveFile.   The textStream can be in use without conflict.
		/// Deleting a textStream simply means it will not be persisted when ZipArchive.Close() is called.  
		/// </summary>
		public void Delete()
		{
			if(IsReadOnly)
				throw new ApplicationException("Archive is ReadOnly");
			archive.entries.Remove(name);
			name = null;
			archive = null;
			uncompressedData = null;
			compressedData = null;
		}

		/// <summary>
		/// The last time the archive was updated (Create() was called).   The copy operations transfer the
		/// LastWriteTime from the source to the target.  
		/// </summary>
		public DateTime LastWriteTime
		{
			get
			{
				return lastWriteTime;
			}
			set
			{
				if(IsReadOnly)
					throw new ApplicationException("Archive is ReadOnly");
				lastWriteTime = value;
			}
		}

		/// <summary>
		/// The length of the archive textStream in bytes. 
		/// </summary>
		public long Length
		{
			get
			{
				if(uncompressedData != null)
					return uncompressedData.Length;
				else
					return length;
			}
		}

		/// <summary>
		/// The name in the archive. 
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				MoveTo(value);
			}
		}

		/// <summary>
		/// The CRC32 checksum associated with the data.  Useful for quickly determining if the data has
		/// changed.  
		/// </summary>
		public uint CheckSum
		{
			get
			{
				if(crc32 == null)
				{
					System.Diagnostics.Debug.Assert(uncompressedData != null);
					crc32 = Crc32.Calculate(0, uncompressedData.GetBuffer(), 0, (int)uncompressedData.Length);
				}
				return crc32.Value;
			}
		}

		/// <summary>
		/// The archive assoated with the ZipArchiveFile. 
		/// </summary>
		internal ZipArchive Archive
		{
			get
			{
				return archive;
			}
		}

		/// <summary>
		/// Returns true if the textStream can's be written (the archive is read-only.  
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return archive.IsReadOnly;
			}
		}

		#if DEBUG
		        /// <summary>
        /// Recupera os dados com texto.
        /// </summary>
        public string DataAsText { get { return ReadAllText(); } }
#endif
		/// <summary>
		///  A text summary of the archive textStream (its name and length).  
		/// </summary>
		public override string ToString()
		{
			return "ZipArchiveEntry " + Name + " length " + Length;
		}

		/// <summary>
		/// Truncate the archive textStream and return a StreamWrite sutable for writing text to the textStream. 
		/// </summary>
		/// <returns></returns>
		public StreamWriter CreateText()
		{
			return new StreamWriter(Create());
		}

		/// <summary>
		/// Opens the archiveFile represented by the ZipArchiveFile and returns a stream that can use to read text.
		/// </summary>
		/// <returns>A TextReader text can be read from.</returns>
		public StreamReader OpenText()
		{
			return new StreamReader(OpenRead());
		}

		/// <summary>
		/// Read all the text from the archiveFile represented by the ZipArchiveFile and return it as a string. 
		/// </summary>
		/// <returns>The string contained in the archiveFile</returns>
		public string ReadAllText()
		{
			TextReader reader = OpenText();
			string ret = reader.ReadToEnd();
			reader.Close();
			return ret;
		}

		/// <summary>
		/// Replaces the data in the archiveFile represented by the ZipArchiveFile with the text in 'data'
		/// </summary>
		/// <param name="data">The data to replace the archiveFile data with.</param>
		public void WriteAllText(string data)
		{
			TextWriter writer = CreateText();
			writer.Write(data);
			writer.Close();
		}

		/// <summary>
		/// Copy the data in from the 'this' ZipArchiveFile to the archive textStream named 'outputFilePath' in
		/// to the file system at 'outputFilePath' 
		/// </summary>
		/// <param name="outputFilePath"></param>
		/// <param name="canOverride">Identifica se o arquivo pode se sobrescrito.</param>
		public void CopyToFile(string outputFilePath, bool canOverride)
		{
			string outputDirectory = Path.GetDirectoryName(outputFilePath);
			if(outputDirectory.Length > 0)
				Directory.CreateDirectory(outputDirectory);
			if(!canOverride && System.IO.File.Exists(outputFilePath))
				return;
			using (Stream outFile = new FileStream(outputFilePath, FileMode.Create))
				using (Stream inFile = OpenRead())
					CopyStream(inFile, outFile);
			try
			{
				File.SetLastWriteTime(outputFilePath, LastWriteTime);
			}
			catch(IOException)
			{
				if(canOverride)
					throw;
			}
			catch(UnauthorizedAccessException)
			{
				if(canOverride)
					throw;
			}
		}

		/// <summary>
		/// Copy the data in archive textStream named 'inputFilePath' into the 'this' archive textStream.  (discarding
		/// what was there before). 
		/// </summary>
		public void CopyTo(string outputArchivePath)
		{
			using (Stream outFile = archive.Create(outputArchivePath))
				using (Stream inFile = OpenRead())
					CopyStream(inFile, outFile);
			archive[outputArchivePath].LastWriteTime = LastWriteTime;
		}

		private uint? crc32;

		private string name;

		private DateTime lastWriteTime;

		private long length;

		private int compressedLength;

		private CompressionMethod compressionMethod;

		private ZipArchive archive;

		private uint headerOffset;

		private MemoryStream uncompressedData;

		private byte[] compressedData;

		private long positionOfCompressedDataInArchive;

        internal const uint SignatureFileEntry = 0x04034b50;
        internal const uint SignatureArchiveDirectory = 0x02014b50;
        internal const uint SignatureArchiveDirectoryEnd = 0x06054b50;
        internal const ushort VersionNeededToExtract = 0x0100; // version 1.0, TODO
        internal const ushort MaximumVersionExtractable = 0x0100;
        internal const ushort VersionMadeBy = 0;               // MS-DOS, TODO: should this be NTFS?
        internal const ushort GeneralPurposeBitFlag = 0;       // TODO
        internal const ushort ExtraFieldLength = 0;
        internal const ushort FileCommentLength = 0;
        internal const ushort DiskNumberStart = 0;
        internal const ushort InternalFileAttributes = 0;      // binary file, TODO: support ASCII?
        internal const uint ExternalFileAttributes = 0;      // TODO: do we want to support?

        internal enum CompressionMethod : ushort
		{
			None = 0,
			Deflate = 8,
		};


		static char[] invalidPathChars = Path.GetInvalidPathChars();

		static internal int CopyStream(Stream fromStream, Stream toStream)
		{
			byte[] buffer = new byte[8192];
			int totalBytes = 0;
			for(; ;)
			{
				int count = fromStream.Read(buffer, 0, buffer.Length);
				if(count == 0)
					break;
				toStream.Write(buffer, 0, count);
				totalBytes += count;
			}
			return totalBytes;
		}

		static private DateTime DosTimeToDateTime(uint dateTime)
		{
			int dateTimeSigned = (int)dateTime;
			int year = 1980 + (dateTimeSigned >> 25);
			int month = (dateTimeSigned >> 21) & 0xF;
			int day = (dateTimeSigned >> 16) & 0x1F;
			int hour = (dateTimeSigned >> 11) & 0x1F;
			int minute = (dateTimeSigned >> 5) & 0x3F;
			int second = (dateTimeSigned & 0x001F) * 2;
			if(second >= 60)
				second = 0;
			DateTime ret = new DateTime();
			try
			{
				ret = new System.DateTime(year, month, day, hour, minute, second, 0);
			}
			catch
			{
			}
			return ret;
		}

		static private uint DateTimeToDosTime(DateTime dateTime)
		{
			int ret = ((dateTime.Year - 1980) & 0x7F);
			ret = (ret << 4) + dateTime.Month;
			ret = (ret << 5) + dateTime.Day;
			ret = (ret << 5) + dateTime.Hour;
			ret = (ret << 6) + dateTime.Minute;
			ret = (ret << 5) + (dateTime.Second / 2);
			return (uint)ret;
		}

		/// <summary>
		/// Used by ZipArchive to write the entry to the archive. 
		/// </summary>
		/// <param name="writer">The stream representing the archive to write the entry to.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal void WriteToStream(Stream writer)
		{
			System.Diagnostics.Debug.Assert(!IsReadOnly);
			System.Diagnostics.Debug.Assert(positionOfCompressedDataInArchive == 0);
			if(uncompressedData != null)
			{
				if(uncompressedData.CanWrite)
					throw new Exception("Unclosed writable handle to " + Name + " still exists at Save time");
				MemoryStream compressedDataStream = new RepairedMemoryStream((int)(uncompressedData.Length * 5 / 8));
				Stream compressor = new System.IO.Compression.DeflateStream(compressedDataStream, System.IO.Compression.CompressionMode.Compress);
				compressor.Write(uncompressedData.GetBuffer(), 0, (int)uncompressedData.Length);
				compressor.Close();
				compressionMethod = CompressionMethod.Deflate;
				compressedLength = (int)compressedDataStream.Length;
				compressedData = compressedDataStream.GetBuffer();
			}
			System.Diagnostics.Debug.Assert(compressedData != null);
			WriteZipFileHeader(writer);
			writer.Write(compressedData, 0, compressedLength);
		}

		private void WriteZipFileHeader(Stream writer)
		{
			byte[] fileNameBytes = Encoding.UTF8.GetBytes(name.Replace(Path.DirectorySeparatorChar, '/'));
			if((uint)length != length)
				throw new ApplicationException("File length too long.");
			headerOffset = (uint)writer.Position;
			ByteBuffer header = new ByteBuffer(30);
			header.WriteUInt32(0x04034b50);
			header.WriteUInt16(0x0100);
			header.WriteUInt16(0);
			header.WriteUInt16((ushort)compressionMethod);
			header.WriteUInt32(DateTimeToDosTime(lastWriteTime));
			header.WriteUInt32(CheckSum);
			header.WriteUInt32((uint)compressedLength);
			header.WriteUInt32((uint)Length);
			header.WriteUInt16((ushort)fileNameBytes.Length);
			header.WriteUInt16(0);
			header.WriteContentsTo(writer);
			writer.Write(fileNameBytes, 0, fileNameBytes.Length);
		}

		internal void WriteArchiveDirectoryEntryToStream(Stream writer)
		{
			byte[] fileNameBytes = Encoding.UTF8.GetBytes(name);
			ByteBuffer header = new ByteBuffer(46);
			header.WriteUInt32(0x02014b50);
			header.WriteUInt16(0);
			header.WriteUInt16(0x0100);
			header.WriteUInt16(0);
			header.WriteUInt16((ushort)compressionMethod);
			header.WriteUInt32(DateTimeToDosTime(lastWriteTime));
			header.WriteUInt32(CheckSum);
			header.WriteUInt32((uint)compressedLength);
			header.WriteUInt32((uint)Length);
			header.WriteUInt16((ushort)fileNameBytes.Length);
			header.WriteUInt16(0);
			header.WriteUInt16(0);
			header.WriteUInt16(0);
			header.WriteUInt16(0);
			header.WriteUInt32(0);
			header.WriteUInt32(headerOffset);
			header.WriteContentsTo(writer);
			writer.Write(fileNameBytes, 0, fileNameBytes.Length);
		}

		/// <summary>
		/// Create a new archive archiveFile with no data (empty).  It is expected that only ZipArchive methods will
		/// use this routine.  
		/// </summary>
		internal ZipArchiveFile(ZipArchive archive, string archiveName)
		{
			this.archive = archive;
			name = archiveName;
			if(name != null)
				archive.entries[name] = this;
			lastWriteTime = DateTime.Now;
		}

		/// <summary>
		/// Reads a single archiveFile from a Zip Archive.  Should only be used by ZipArchive.  
		/// </summary>
		/// <returns>A ZipArchiveFile representing the archiveFile read from the archive.</returns>
		internal static ZipArchiveFile Read(ZipArchive archive)
		{
			Stream reader = archive.fromStream;
			ByteBuffer header = new ByteBuffer(30);
			int count = header.ReadContentsFrom(reader);
			if(count == 0)
				return null;
			uint fileSignature = header.ReadUInt32();
			if(fileSignature != 0x04034b50)
			{
				if(fileSignature != 0x02014b50)
					throw new ApplicationException("Bad ZipFile Header");
				return null;
			}
			ushort versionNeededToExtract = header.ReadUInt16();
			if(versionNeededToExtract > 0x0100)
				throw new ApplicationException("Zip file requires unsupported features");
			header.SkipBytes(2);
			ZipArchiveFile newEntry = new ZipArchiveFile(archive, null);
			newEntry.compressionMethod = (CompressionMethod)header.ReadUInt16();
			newEntry.lastWriteTime = DosTimeToDateTime(header.ReadUInt32());
			newEntry.crc32 = header.ReadUInt32();
			newEntry.compressedLength = checked((int)header.ReadUInt32());
			newEntry.length = header.ReadUInt32();
			int fileNameLength = checked((int)header.ReadUInt16());
			byte[] fileNameBuffer = new byte[fileNameLength];
			int fileNameCount = reader.Read(fileNameBuffer, 0, fileNameLength);
			newEntry.name = Encoding.UTF8.GetString(fileNameBuffer).Replace('/', Path.DirectorySeparatorChar);
			archive.entries[newEntry.name] = newEntry;
			if(count != header.Length || fileNameCount != fileNameLength || fileNameLength == 0 || newEntry.LastWriteTime.Ticks == 0)
				throw new ApplicationException("Bad Zip File Header");
			if(newEntry.Name.IndexOfAny(invalidPathChars) >= 0)
				throw new ApplicationException("Invalid File Name");
			if(newEntry.compressionMethod != CompressionMethod.None && newEntry.compressionMethod != CompressionMethod.Deflate)
				throw new ApplicationException("Unsupported compression mode " + newEntry.compressionMethod);
			if(archive.IsReadOnly && reader.CanSeek)
			{
				newEntry.positionOfCompressedDataInArchive = archive.fromStream.Position;
				reader.Seek(newEntry.compressedLength, SeekOrigin.Current);
			}
			else
			{
				newEntry.compressedData = new byte[newEntry.compressedLength];
				reader.Read(newEntry.compressedData, 0, (int)newEntry.compressedLength);
			}
			#if DEBUG
			            newEntry.Validate();
#endif
			return newEntry;
		}

		internal void Validate()
		{
			Stream readStream = OpenRead();
			uint crc = 0;
			byte[] buffer = new byte[655536];
			for(; ;)
			{
				int count = readStream.Read(buffer, 0, buffer.Length);
				if(count == 0)
					break;
				crc = Crc32.Calculate(crc, buffer, 0, count);
			}
			readStream.Close();
			if(crc != CheckSum)
				throw new ApplicationException("Error: data checksum failed for " + Name);
		}
	}
}
