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
using Colosoft.Caching.Interop;

namespace Colosoft.Caching.Storage.Mmf
{
	/// <summary>
	/// Implementação da visão do arquivo.
	/// </summary>
	internal class MmfFileView
	{
		private IntPtr _mvPtr;

		private uint _viewLength;

		/// <summary>
		/// Tamanho da visão.
		/// </summary>
		public uint Length
		{
			get
			{
				return _viewLength;
			}
		}

		/// <summary>
		/// Ponteiro para a visão.
		/// </summary>
		public IntPtr ViewPtr
		{
			get
			{
				return _mvPtr;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="view">Ponteiro para a visão.</param>
		/// <param name="length"></param>
		public MmfFileView(IntPtr view, uint length)
		{
			if(view == IntPtr.Zero)
				throw new ArgumentOutOfRangeException("_mvPtr");
			_mvPtr = view;
			_viewLength = length;
		}

		/// <summary>
		/// Fecha a instancia.
		/// </summary>
		public void Close()
		{
			if(_mvPtr != IntPtr.Zero)
				SafeNativeMethods.UnmapViewOfFile(_mvPtr);
		}

		/// <summary>
		/// Copia a memória da visão.
		/// </summary>
		/// <param name="srcOffset">Offset da origem.</param>
		/// <param name="destOffset">Offset do destino.</param>
		/// <param name="count">Quantidade de bytes que serão copiados.</param>
		/// <returns></returns>
		public bool CopyMemory(int srcOffset, int destOffset, int count)
		{
			byte[] buffer = new byte[count];
			if(!this.Read(buffer, srcOffset, count))
				return false;
			if(!this.Write(buffer, destOffset, count))
				return false;
			return true;
		}

		/// <summary>
		/// Faz um flush dos dados.
		/// </summary>
		public void Flush()
		{
			if(_mvPtr != IntPtr.Zero)
				SafeNativeMethods.FlushViewOfFile(_mvPtr, _viewLength);
		}

		/// <summary>
		/// Realiza uma troca da memória.
		/// </summary>
		/// <param name="srcOffset">Offset da origem.</param>
		/// <param name="destOffset">Offset do destino.</param>
		/// <param name="srcLen">Tamanho que será lido a origem.</param>
		/// <param name="destLen">Tamanho que será salvo no destino.</param>
		/// <returns></returns>
		public bool SwapMemory(int srcOffset, int destOffset, int srcLen, int destLen)
		{
			byte[] buffer = new byte[srcLen];
			byte[] buffer2 = new byte[destLen];
			if(!this.Read(buffer, srcOffset, srcLen))
				return false;
			if(!this.Read(buffer2, destOffset, destLen))
				return false;
			this.Write(buffer2, srcOffset, destLen);
			this.Write(buffer, destOffset, srcLen);
			return true;
		}

		/// <summary>
		/// Lê um buffer de dados da instancia.
		/// </summary>
		/// <param name="offSet">Offset para a leitura.</param>
		/// <param name="count">Quantidade de dados que serão lidos.</param>
		/// <returns>Buffer com os dados lidos.</returns>
		public byte[] Read(int offSet, int count)
		{
			if((offSet < 0) || ((offSet + count) > _viewLength))
				return null;
			byte[] destination = new byte[count];
			Marshal.Copy(new IntPtr(_mvPtr.ToInt64() + offSet), destination, 0, count);
			return destination;
		}

		/// <summary>
		/// Lê os dados para o buffer informado.
		/// </summary>
		/// <param name="buffer">Buffer onde os dados serão salvos.</param>
		/// <param name="offSet">Offset para a leitura.</param>
		/// <param name="count">Quantidade de dados que serão lidos.</param>
		/// <returns>True se for lido com sucesso.</returns>
		public bool Read(byte[] buffer, int offSet, int count)
		{
			if((offSet < 0) || ((offSet + count) > _viewLength))
				return false;
			Marshal.Copy(new IntPtr(_mvPtr.ToInt64() + offSet), buffer, 0, count);
			return true;
		}

		/// <summary>
		/// Lê um byte.
		/// </summary>
		/// <param name="ofs">Offset para a leitura.</param>
		/// <returns></returns>
		public byte ReadByte(int ofs)
		{
			return Marshal.ReadByte(_mvPtr, ofs);
		}

		/// <summary>
		/// Lê um <see cref="Int16"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <returns></returns>
		public short ReadInt16(int ofs)
		{
			return Marshal.ReadInt16(_mvPtr, ofs);
		}

		/// <summary>
		/// Lê um <see cref="Int32"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <returns></returns>
		public int ReadInt32(int ofs)
		{
			return Marshal.ReadInt32(_mvPtr, ofs);
		}

		/// <summary>
		/// Lê um <see cref="Int64"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <returns></returns>
		public long ReadInt64(int ofs)
		{
			return Marshal.ReadInt64(_mvPtr, ofs);
		}

		/// <summary>
		/// Lê um <see cref="UInt16"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <returns></returns>
		public ushort ReadUInt16(int ofs)
		{
			return (ushort)Marshal.ReadInt16(_mvPtr, ofs);
		}

		/// <summary>
		/// Lê um <see cref="Int32"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <returns></returns>
		public uint ReadUInt32(int ofs)
		{
			return (uint)Marshal.ReadInt32(_mvPtr, ofs);
		}

		/// <summary>
		/// Lê um <see cref="UInt64"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <returns></returns>
		public ulong ReadUInt64(int ofs)
		{
			return (ulong)Marshal.ReadInt64(_mvPtr, ofs);
		}

		/// <summary>
		/// Escreve o buffer na instancia.
		/// </summary>
		/// <param name="buffer">Buffer com os dados que serão escritos.</param>
		/// <param name="offSet">Offset para recuperar os dados do buffer.</param>
		/// <returns></returns>
		public bool Write(byte[] buffer, int offSet)
		{
			return this.Write(buffer, offSet, buffer.Length);
		}

		/// <summary>
		/// Escreve o buffer na instancia.
		/// </summary>
		/// <param name="buffer">Buffer com os dados que serão escritos.</param>
		/// <param name="offSet">Offset para recuperar os dados do buffer.</param>
		/// <param name="count">Quantidade de dados que serão recuperados.</param>
		/// <returns></returns>
		public bool Write(byte[] buffer, int offSet, int count)
		{
			if((offSet < 0) || ((offSet + count) > _viewLength))
				return false;
			Marshal.Copy(buffer, 0, new IntPtr(_mvPtr.ToInt64() + offSet), count);
			return true;
		}

		/// <summary>
		/// Escreve o <see cref="Byte"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <param name="val">Valor que será escrito.</param>
		public void WriteByte(int ofs, byte val)
		{
			Marshal.WriteByte(_mvPtr, ofs, val);
		}

		/// <summary>
		/// Escreve o <see cref="Int16"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <param name="val">Valor que será escrito.</param>
		public void WriteInt16(int ofs, short val)
		{
			Marshal.WriteInt16(_mvPtr, ofs, val);
		}

		/// <summary>
		/// Escreve o <see cref="Int32"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <param name="val">Valor que será escrito.</param>
		public void WriteInt32(int ofs, int val)
		{
			Marshal.WriteInt32(_mvPtr, ofs, val);
		}

		/// <summary>
		/// Escreve o <see cref="Int64"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <param name="val">Valor que será escrito.</param>
		public void WriteInt64(int ofs, long val)
		{
			Marshal.WriteInt64(_mvPtr, ofs, val);
		}

		/// <summary>
		/// Escreve o <see cref="UInt16"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <param name="val">Valor que será escrito.</param>
		public void WriteUInt16(int ofs, ushort val)
		{
			Marshal.WriteInt16(_mvPtr, ofs, (short)val);
		}

		/// <summary>
		/// Escreve o <see cref="UInt32"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <param name="val">Valor que será escrito.</param>
		public void WriteUInt32(int ofs, uint val)
		{
			Marshal.WriteInt32(_mvPtr, ofs, (int)val);
		}

		/// <summary>
		/// Escreve o <see cref="UInt64"/>.
		/// </summary>
		/// <param name="ofs">Offset.</param>
		/// <param name="val">Valor que será escrito.</param>
		public void WriteUInt64(int ofs, ulong val)
		{
			Marshal.WriteInt64(_mvPtr, ofs, (long)val);
		}
	}
}
