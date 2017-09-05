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
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;

namespace Colosoft.Excel.ExcelXml.Extensions
{
	/// <summary>
	/// Static file helper methods
	/// </summary>
	public static class FileExtensions
	{
		/// <summary>
		/// Upload a file to FTP
		/// </summary>
		/// <param name="filePath">File to upload</param>
		/// <param name="remotePath">Remote path</param>
		/// <param name="logOn">User logOn</param>
		/// <param name="password">User password</param>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "FTP")]
		public static void UploadToFTP(string filePath, string remotePath, string logOn, string password)
		{
			using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				string url = Path.Combine(remotePath, Path.GetFileName(filePath));
				Uri uri = new Uri(url);
				FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(uri);
				ftp.Credentials = new NetworkCredential(logOn, password);
				ftp.KeepAlive = false;
				ftp.Method = WebRequestMethods.Ftp.UploadFile;
				ftp.UseBinary = true;
				ftp.ContentLength = fs.Length;
				ftp.Proxy = null;
				fs.Position = 0;
				int contentLen;
				int buffLength = 2048;
				byte[] buff = new byte[buffLength];
				using (Stream strm = ftp.GetRequestStream())
				{
					contentLen = fs.Read(buff, 0, buffLength);
					while (contentLen != 0)
					{
						strm.Write(buff, 0, contentLen);
						contentLen = fs.Read(buff, 0, buffLength);
					}
				}
			}
		}

		/// <summary>
		/// Merge two files into single file
		/// </summary>
		/// <param name="firstFile">First file</param>
		/// <param name="secondFile">Second file</param>
		/// <param name="mergedFile">Path to save the merged file to</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static bool MergeFiles(string firstFile, string secondFile, string mergedFile)
		{
			try
			{
				byte[] frstFile = File.ReadAllBytes(firstFile);
				byte[] sndFile = File.ReadAllBytes(secondFile);
				BinaryWriter output = new BinaryWriter(File.Open(mergedFile, FileMode.Create, FileAccess.Write));
				output.Write(frstFile);
				output.Write(sndFile);
				output.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Count the number of lines in a file
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <returns>Number of lines in a file or -1 on error</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static int NumberOfLines(string fileName)
		{
			try
			{
				TextReader trlines = new StreamReader(fileName);
				string data = trlines.ReadToEnd();
				int prvLen = data.Length;
				int noOfLines = (prvLen - data.Replace("\n", "").Length) + 1;
				trlines.Close();
				return noOfLines;
			}
			catch
			{
				return -1;
			}
		}

		/// <summary>
		/// Compare the last modified date stamp of two files
		/// </summary>
		/// <param name="firstFile">First file</param>
		/// <param name="secondFile">Second file</param>
		/// <returns>-1 if firstFile is newer, 0 if files are same and 1 if secondFile is newer.</returns>
		public static int CompareFile(string firstFile, string secondFile)
		{
			bool fe1 = File.Exists(firstFile);
			bool fe2 = File.Exists(secondFile);
			if(!fe1 && !fe2)
				return 0;
			else if(!fe1 && fe2)
				return 1;
			else if(fe1 && !fe2)
				return -1;
			DateTime ft1 = File.GetLastWriteTime(firstFile);
			DateTime ft2 = File.GetLastWriteTime(secondFile);
			if(ft1 == ft2)
				return 0;
			else if(ft1 < ft2)
				return 1;
			return -1;
		}

		/// <summary>
		/// Convert a path to relative path
		/// </summary>
		/// <param name="fromDirectory">Convert from path</param>
		/// <param name="toPath">To relative path</param>
		/// <returns>Relative path of the conversion path given</returns>
		public static string GetRelativePath(string fromDirectory, string toPath)
		{
			if(fromDirectory == null)
				throw new ArgumentNullException("fromDirectory");
			if(toPath == null)
				throw new ArgumentNullException("toPath");
			bool isRooted = Path.IsPathRooted(fromDirectory) && Path.IsPathRooted(toPath);
			if(isRooted)
			{
				bool isDifferentRoot = string.Compare(Path.GetPathRoot(fromDirectory), Path.GetPathRoot(toPath), true, CultureInfo.CurrentCulture) != 0;
				if(isDifferentRoot)
					return toPath;
			}
			StringCollection relativePath = new StringCollection();
			string[] fromDirectories = fromDirectory.Split(Path.DirectorySeparatorChar);
			string[] toDirectories = toPath.Split(Path.DirectorySeparatorChar);
			int length = Math.Min(fromDirectories.Length, toDirectories.Length);
			int lastCommonRoot = -1;
			for(int x = 0; x < length; x++)
			{
				if(string.Compare(fromDirectories[x], toDirectories[x], true, CultureInfo.CurrentCulture) != 0)
					break;
				lastCommonRoot = x;
			}
			if(lastCommonRoot == -1)
				return toPath;
			for(int x = lastCommonRoot + 1; x < fromDirectories.Length; x++)
				if(fromDirectories[x].Length > 0)
					relativePath.Add("..");
			for(int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
				relativePath.Add(toDirectories[x]);
			string[] relativeParts = new string[relativePath.Count];
			relativePath.CopyTo(relativeParts, 0);
			string newPath = string.Join(Path.DirectorySeparatorChar.ToString(), relativeParts);
			return newPath;
		}
	}
	/// <summary>
	/// Static folder helper methods
	/// </summary>
	public static class FolderExtensions
	{
		/// <summary>
		/// Returns all files in a directory and its subdirectories as a string array
		/// </summary>
		/// <param name="rootPath">Path of the directory</param>
		/// <returns>String array containing all the files</returns>
		public static string[] GetFileList(string rootPath)
		{
			return GetFileList(rootPath, null);
		}

		/// <summary>
		/// Returns all files in a directory and its subdirectories as a string array matching to a pattern
		/// </summary>
		/// <param name="rootPath">Path of the directory</param>
		/// <param name="pattern">Pattern to match</param>
		/// <returns>String array containing all the files</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static string[] GetFileList(string rootPath, string pattern)
		{
			if(rootPath.IsNullOrEmpty())
				pattern = "*.*";
			try
			{
				return Directory.GetFiles(rootPath, pattern, SearchOption.AllDirectories);
			}
			catch
			{
			}
			return null;
		}
	}
}
