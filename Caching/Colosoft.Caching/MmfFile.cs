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
using Colosoft.Caching.Interop;
using System.IO;

namespace Colosoft.Caching.Storage.Mmf
{
	internal class MmfFile : IDisposable
	{
		private IntPtr _hFile;

		private IntPtr _mmHandle;

		private ulong _mmLength;

		private MemoryProtection _protection;

		private static readonly IntPtr INVALID_HANDLE = new IntPtr(-1);

		private string objectName = Guid.NewGuid().ToString();

		private MmfFile(IntPtr hFile, ulong maxLength, MemoryProtection protection)
		{
			if(hFile == IntPtr.Zero)
			{
				throw new ArgumentOutOfRangeException("hFile");
			}
			if((protection < MemoryProtection.PageNoAccess) || (protection > MemoryProtection.SecReserve))
			{
				throw new ArgumentOutOfRangeException("protection");
			}
			_hFile = hFile;
			_mmLength = maxLength;
			_protection = protection;
		}

		~MmfFile()
		{
			try
			{
				Dispose();
			}
			catch
			{
			}
		}

		public void Close()
		{
			this.CloseMapHandle();
			if(_hFile != IntPtr.Zero)
			{
				SafeNativeMethods.CloseHandle(_hFile);
			}
			GC.SuppressFinalize(this);
		}

		public void CloseMapHandle()
		{
			if(_mmHandle != IntPtr.Zero)
			{
				SafeNativeMethods.CloseHandle(_mmHandle);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static MmfFile Create(IntPtr hFile, ulong maxLength)
		{
			if(maxLength < 0)
				throw new ArgumentOutOfRangeException("maxLength");
			MmfFile file = new MmfFile(hFile, maxLength, MemoryProtection.PageReadWrite);
			if(!file.IsPageFile)
			{
				file.SetMaxLength(Math.Max(maxLength, file.FileSize));
				return file;
			}
			file.SetMaxLength(maxLength);
			return file;
		}

		public static MmfFile Create(string name, bool resetContents)
		{
			return Create(name, 0, resetContents);
		}

		public static MmfFile Create(string name, ulong maxLength, bool resetContents)
		{
			if(name == null)
			{
				return Create(INVALID_HANDLE, maxLength);
			}
			IntPtr hFile = SafeNativeMethods.CreateFile(name, Win32FileAccess.GENERIC_ALL, Win32FileShare.FILE_SHARE_READ, IntPtr.Zero, resetContents ? Win32FileMode.CREATE_ALWAYS : Win32FileMode.OPEN_ALWAYS, Win32FileAttributes.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
			if(hFile == IntPtr.Zero)
			{
				throw new IOException(SafeNativeMethods.GetWin32ErrorMessage(Interop.SafeNativeMethods.GetLastError()));
			}
			if((maxLength <= 0) && !SafeNativeMethods.GetFileSizeEx(hFile, out maxLength))
			{
				throw new IOException(SafeNativeMethods.GetWin32ErrorMessage(Interop.SafeNativeMethods.GetLastError()));
			}
			return Create(hFile, maxLength);
		}

		public void Dispose()
		{
			this.Close();
		}

		public int GetMaxViews(uint viewSize)
		{
			ulong maxLength = this.MaxLength;
			int num2 = (int)(maxLength / ((ulong)viewSize));
			if((maxLength % ((ulong)viewSize)) > 0)
			{
				num2++;
			}
			return num2;
		}

		public MmfFileView MapView(ulong offSet, uint count)
		{
			if(offSet < 0)
			{
				throw new ArgumentOutOfRangeException("offSet");
			}
			if(count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			IntPtr view = SafeNativeMethods.MapViewOfFile(_mmHandle, SafeNativeMethods.GetWin32FileMapAccess(_protection), (uint)(offSet >> 0x20), ((uint)offSet) & uint.MaxValue, count);
			if(!(view == IntPtr.Zero))
			{
				return new MmfFileView(view, count);
			}
			uint lastError = Interop.SafeNativeMethods.GetLastError();
			if(lastError == 8)
			{
				throw new OutOfMemoryException();
			}
			throw new IOException(SafeNativeMethods.GetWin32ErrorMessage(lastError));
		}

		public void SetMaxLength(ulong maxLength)
		{
			this.CloseMapHandle();
			_mmLength = maxLength;
			_mmHandle = SafeNativeMethods.CreateFileMapping(_hFile, IntPtr.Zero, _protection, (uint)(_mmLength >> 0x20), ((uint)_mmLength) & uint.MaxValue, this.objectName);
			if(_mmHandle == IntPtr.Zero)
			{
				uint lastError = Interop.SafeNativeMethods.GetLastError();
				switch(lastError)
				{
				case 8:
					throw new OutOfMemoryException();
				case 0x70:
					throw new OutOfMemoryException();
				case 0x5af:
					throw new OutOfMemoryException("Limited Virtual Memory. Your system has no paging file, or the paging file is too small.");
				}
				throw new IOException(SafeNativeMethods.GetWin32ErrorMessage(lastError));
			}
		}

		public void UnMapView(MmfFileView view)
		{
			SafeNativeMethods.UnmapViewOfFile(view.ViewPtr);
		}

		public ulong FileSize
		{
			get
			{
				ulong num;
				if(!SafeNativeMethods.GetFileSizeEx(_hFile, out num))
				{
					throw new IOException(SafeNativeMethods.GetWin32ErrorMessage(Interop.SafeNativeMethods.GetLastError()));
				}
				return num;
			}
		}

		public bool IsPageFile
		{
			get
			{
				return (_hFile == INVALID_HANDLE);
			}
		}

		public ulong MaxLength
		{
			get
			{
				return _mmLength;
			}
		}
	}
}
