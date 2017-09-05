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
using System.Runtime.InteropServices;

namespace Colosoft.Caching.Interop
{
	/// <summary>
	/// Possíveis tipos de acesso para arquivo.
	/// </summary>
	[Flags]
	internal enum Win32FileAccess : uint
	{
		GENERIC_ALL = 0x10000000,
		GENERIC_EXECUTE = 0x20000000,
		GENERIC_READ = 0x80000000,
		GENERIC_WRITE = 0x40000000
	}
	/// <summary>
	/// Possíveis proteções para memória.
	/// </summary>
	[Flags]
	internal enum MemoryProtection : uint
	{
		PageNoAccess = 1,
		PageReadOnly = 2,
		PageReadWrite = 4,
		PageWriteCopy = 8,
		SecCommit = 0x8000000,
		SecImage = 0x1000000,
		SecNoCache = 0x10000000,
		SecReserve = 0x4000000
	}
	/// <summary>
	/// Possíveis atributos para arquivos.
	/// </summary>
	[Flags]
	internal enum Win32FileAttributes : uint
	{
		FILE_ATTRIBUTE_ARCHIVE = 0x20,
		FILE_ATTRIBUTE_COMPRESSED = 0x800,
		FILE_ATTRIBUTE_DEVICE = 0x40,
		FILE_ATTRIBUTE_DIRECTORY = 0x10,
		FILE_ATTRIBUTE_ENCRYPTED = 0x4000,
		FILE_ATTRIBUTE_HIDDEN = 2,
		FILE_ATTRIBUTE_NORMAL = 0x80,
		FILE_ATTRIBUTE_NOTCONTENTINDEXED = 0x2000,
		FILE_ATTRIBUTE_OFFLINE = 0x1000,
		FILE_ATTRIBUTE_READONLY = 1,
		FILE_ATTRIBUTE_REPARSE_POINT = 0x400,
		FILE_ATTRIBUTE_SPARSE_FILE = 0x200,
		FILE_ATTRIBUTE_SYSTEM = 4,
		FILE_ATTRIBUTE_TEMPORARY = 0x100
	}
	/// <summary>
	/// Possíveis mapas de acesso.
	/// </summary>
	[Flags]
	internal enum Win32FileMapAccess : uint
	{
		FILE_MAP_ALL_ACCESS = 0xf001f,
		FILE_MAP_COPY = 1,
		FILE_MAP_READ = 4,
		FILE_MAP_WRITE = 2
	}
	[Flags]
	internal enum Win32FileMode : uint
	{
		CREATE_ALWAYS = 2,
		CREATE_NEW = 1,
		OPEN_ALWAYS = 4,
		OPEN_EXISTING = 3,
		TRUNCATE_EXISTING = 5
	}
	[Flags]
	internal enum Win32FileShare : uint
	{
		FILE_SHARE_DELETE = 4,
		FILE_SHARE_READ = 1,
		FILE_SHARE_WRITE = 2
	}
	/// <summary>
	/// Armazena as informações do sistema.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct SYSTEM_INFO
	{
		/// <summary>
		/// 
		/// </summary>
		public uint dwOemId;

		/// <summary>
		/// 
		/// </summary>
		public uint dwPageSize;

		/// <summary>
		/// 
		/// </summary>
		public uint lpMinimumApplicationAddress;

		/// <summary>
		/// 
		/// </summary>
		public uint lpMaximumApplicationAddress;

		/// <summary>
		/// 
		/// </summary>
		public uint dwActiveProcessorMask;

		/// <summary>
		/// 
		/// </summary>
		public uint dwNumberOfProcessors;

		/// <summary>
		/// 
		/// </summary>
		public uint dwProcessorType;

		/// <summary>
		/// 
		/// </summary>
		public uint dwAllocationGranularity;

		/// <summary>
		/// 
		/// </summary>
		public ushort dwProcessorLevel;

		/// <summary>
		/// 
		/// </summary>
		public ushort dwProcessorRevision;
	}
	/// <summary>
	/// Classe para auxiliar na manipulação de Memory Mapped file.
	/// </summary> 
	internal static class SafeNativeMethods
	{
		/// <summary>
		/// Forma uma memsagem.
		/// </summary>
		/// <param name="dwFlags"></param>
		/// <param name="lpSource"></param>
		/// <param name="dwMessageId"></param>
		/// <param name="dwLanguageId"></param>
		/// <param name="lpBuffer"></param>
		/// <param name="nSize"></param>
		/// <param name="Arguments"></param>
		/// <returns></returns>
		[DllImport("kernel32", ThrowOnUnmappableChar = true, BestFitMapping = false)]
		public static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, StringBuilder lpBuffer, uint nSize, IntPtr Arguments);

		/// <summary>
		/// Recupera o código do ultimo erro.
		/// </summary>
		/// <returns></returns>
		[DllImport("kernel32")]
		public static extern uint GetLastError();

		/// <summary>
		/// Recupera as informações do sistema.
		/// </summary>
		/// <param name="pSI"></param>
		[DllImport("kernel32")]
		public static extern void GetSystemInfo(ref SYSTEM_INFO pSI);

		[DllImport("kernel32", SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
		public static extern bool CloseHandle(IntPtr hFile);

		[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
		public static extern IntPtr CreateFile(string lpFileName, Win32FileAccess dwDesiredAccess, Win32FileShare dwShareMode, IntPtr lpSecurityAttributes, Win32FileMode dwCreationDisposition, Win32FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

		[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
		public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, MemoryProtection flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "1"), DllImport("kernel32", ThrowOnUnmappableChar = true, BestFitMapping = false)]
		public static extern bool FlushViewOfFile(IntPtr lpBaseAddress, uint dwNumberOfBytesToFlush);

		[DllImport("Kernel32.dll", SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
		public static extern int GetFileSize(IntPtr hFile, out uint lpFileSizeHigh);

		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern bool GetFileSizeEx(IntPtr hFile, out ulong lpFileSize);

		public static string GetWin32ErrorMessage(uint error)
		{
			StringBuilder lpBuffer = new StringBuilder(0x400);
			uint num = FormatMessage(0x1000, IntPtr.Zero, error, 0, lpBuffer, 0x400, IntPtr.Zero);
			return lpBuffer.ToString(0, (int)num);
		}

		public static Win32FileMapAccess GetWin32FileMapAccess(MemoryProtection protection)
		{
			MemoryProtection protection2 = protection;
			if(protection2 != MemoryProtection.PageReadOnly)
			{
				if(protection2 == MemoryProtection.PageWriteCopy)
				{
					return Win32FileMapAccess.FILE_MAP_WRITE;
				}
				return Win32FileMapAccess.FILE_MAP_ALL_ACCESS;
			}
			return Win32FileMapAccess.FILE_MAP_READ;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "4"), DllImport("kernel32", ThrowOnUnmappableChar = true, BestFitMapping = false)]
		public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, Win32FileMapAccess dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

		[DllImport("kernel32", ThrowOnUnmappableChar = true, BestFitMapping = false)]
		public static extern IntPtr OpenFileMapping(Win32FileMapAccess dwDesiredAccess, bool isInheritHandle, string lpName);

		[DllImport("kernel32", ThrowOnUnmappableChar = true, BestFitMapping = false)]
		public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
	}
}
