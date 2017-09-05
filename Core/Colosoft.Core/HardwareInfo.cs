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

namespace Colosoft.Licensing
{
	/// <summary>
	/// Classe usada para carrega as informações do hardware.
	/// </summary>
	class HardwareInfo
	{
		/// <summary>
		/// Get volume serial number of drive C
		/// </summary>
		/// <returns></returns>
		private static string GetDiskVolumeSerialNumber()
		{
			try
			{
				var _disk = new System.Management.ManagementObject(@"Win32_LogicalDisk.deviceid=""c:""");
				_disk.Get();
				return _disk["VolumeSerialNumber"].ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Get CPU ID
		/// </summary>
		/// <returns></returns>
		private static string GetProcessorId()
		{
			try
			{
				var _mbs = new System.Management.ManagementObjectSearcher("Select ProcessorId From Win32_processor");
				var _mbsList = _mbs.Get();
				string _id = string.Empty;
				foreach (var _mo in _mbsList)
				{
					_id = _mo["ProcessorId"].ToString();
					break;
				}
				return _id;
			}
			catch
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Get motherboard serial number
		/// </summary>
		/// <returns></returns>
		private static string GetMotherboardID()
		{
			try
			{
				var _mbs = new System.Management.ManagementObjectSearcher("Select SerialNumber From Win32_BaseBoard");
				var _mbsList = _mbs.Get();
				string _id = string.Empty;
				foreach (var _mo in _mbsList)
				{
					_id = _mo["SerialNumber"].ToString();
					break;
				}
				return _id;
			}
			catch
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Quebra as partes.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="partLength"></param>
		/// <returns></returns>
		private static IEnumerable<string> SplitInParts(string input, int partLength)
		{
			if(input == null)
				throw new ArgumentNullException("input");
			if(partLength <= 0)
				throw new ArgumentException("Part length has to be positive.", "partLength");
			for(int i = 0; i < input.Length; i += partLength)
				yield return input.Substring(i, Math.Min(partLength, input.Length - i));
		}

		/// <summary>
		/// Combine CPU ID, Disk C Volume Serial Number and Motherboard Serial Number as device Id
		/// </summary>
		/// <returns></returns>
		public static string GenerateUID(string appName)
		{
			string _id = string.Concat(appName, GetProcessorId(), GetMotherboardID(), GetDiskVolumeSerialNumber());
			byte[] _byteIds = Encoding.UTF8.GetBytes(_id);
			var _md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] _checksum = _md5.ComputeHash(_byteIds);
			string _part1Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 0));
			string _part2Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 4));
			string _part3Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 8));
			string _part4Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 12));
			return string.Format("{0}-{1}-{2}-{3}", _part1Id, _part2Id, _part3Id, _part4Id);
		}

		/// <summary>
		/// Recupera os bytes do identificador único.
		/// </summary>
		/// <param name="UID"></param>
		/// <returns></returns>
		public static byte[] GetUIDInBytes(string UID)
		{
			string[] _ids = UID.Split('-');
			if(_ids.Length != 4)
				throw new ArgumentException("Wrong UID");
			byte[] _value = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(BASE36.Decode(_ids[0])), 0, _value, 0, 8);
			Buffer.BlockCopy(BitConverter.GetBytes(BASE36.Decode(_ids[1])), 0, _value, 8, 8);
			Buffer.BlockCopy(BitConverter.GetBytes(BASE36.Decode(_ids[2])), 0, _value, 16, 8);
			Buffer.BlockCopy(BitConverter.GetBytes(BASE36.Decode(_ids[3])), 0, _value, 24, 8);
			return _value;
		}

		/// <summary>
		/// Recupera o formato do identificador único.
		/// </summary>
		/// <param name="UID"></param>
		/// <returns></returns>
		public static bool ValidateUIDFormat(string UID)
		{
			if(!string.IsNullOrWhiteSpace(UID))
			{
				string[] _ids = UID.Split('-');
				return (_ids.Length == 4);
			}
			else
			{
				return false;
			}
		}
	}
}
