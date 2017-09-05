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

namespace Colosoft.Caching.Storage.Mmf
{
	/// <summary>
	/// Representa um area de memória.
	/// </summary>
	internal class MemArea
	{
		private Header _hdr;

		private uint _offset;

		private View _view;

		public static readonly uint SPLIT_THRESHOLD = ((uint)(Header.Size + 8));

		/// <summary>
		/// Cabeçalho da instancia.
		/// </summary>
		internal Header AreaHeader
		{
			get
			{
				return _hdr;
			}
		}

		/// <summary>
		/// Capacidade a area.
		/// </summary>
		public uint Capacity
		{
			get
			{
				return _hdr.Capacity;
			}
			set
			{
				_hdr.Capacity = value;
				this.RawView.WriteUInt32(((int)_offset) + 1, _hdr.Capacity);
			}
		}

		/// <summary>
		/// Offset dos dados.
		/// </summary>
		internal uint DataOffset
		{
			get
			{
				return (_offset + ((uint)Header.Size));
			}
		}

		/// <summary>
		/// Identifica se possui uma próxima area.
		/// </summary>
		public bool HasNext
		{
			get
			{
				return _hdr.HasNext;
			}
		}

		/// <summary>
		/// Identifica se possui uma area anterior.
		/// </summary>
		public bool HasPrevious
		{
			get
			{
				return _hdr.HasPrevious;
			}
		}

		/// <summary>
		/// Identifica se a area está livre.
		/// </summary>
		public bool IsFree
		{
			get
			{
				return _hdr.IsFree;
			}
			set
			{
				_hdr.Status = value ? ((byte)0) : ((byte)1);
				this.RawView.WriteByte((int)_offset, _hdr.Status);
			}
		}

		/// <summary>
		/// Offset.
		/// </summary>
		internal uint Offset
		{
			get
			{
				return _offset;
			}
		}

		/// <summary>
		/// Offset do próximo item.
		/// </summary>
		internal uint OffsetNext
		{
			get
			{
				return _hdr.OffsetNext;
			}
			set
			{
				_hdr.OffsetNext = value;
				this.RawView.WriteUInt32(((int)_offset) + 5, _hdr.OffsetNext);
			}
		}

		/// <summary>
		/// Offset do item anterior.
		/// </summary>
		internal uint OffsetPrev
		{
			get
			{
				return _hdr.OffsetPrev;
			}
			set
			{
				_hdr.OffsetPrev = value;
				this.RawView.WriteUInt32(((int)_offset) + 9, _hdr.OffsetPrev);
			}
		}

		/// <summary>
		/// Visão da Raw.
		/// </summary>
		internal MmfFileView RawView
		{
			get
			{
				return _view.MmfView;
			}
		}

		/// <summary>
		/// Tamanho total da area.
		/// </summary>
		public uint TotalLength
		{
			get
			{
				return (this.Capacity + ((uint)Header.Size));
			}
		}

		/// <summary>
		/// Visão.
		/// </summary>
		internal View View
		{
			get
			{
				return _view;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="memBase">Visão que será associada com a area.</param>
		public MemArea(View memBase) : this(memBase, 0)
		{
		}

		public MemArea(View memBase, uint offSet)
		{
			_offset = offSet;
			_view = memBase;
			this.ReadHeader();
		}

		/// <summary>
		/// Verifica se as areas informadas são adjacentes.
		/// </summary>
		/// <param name="area1"></param>
		/// <param name="area2"></param>
		/// <returns></returns>
		private static bool AreAdjacent(MemArea area1, MemArea area2)
		{
			if(area1.OffsetNext != area2.Offset)
				return (area2.OffsetNext == area1.Offset);
			return true;
		}

		/// <summary>
		/// Lê o dados dao cabeçalho.
		/// </summary>
		private void ReadHeader()
		{
			_hdr.RawRead(this.RawView, (int)_offset);
		}

		/// <summary>
		/// Define a próxima area.
		/// </summary>
		/// <param name="arena"></param>
		/// <param name="arenaNext"></param>
		/// <returns></returns>
		private static MemArea SetNextArea(MemArea arena, MemArea arenaNext)
		{
			if((arenaNext == null) || (arenaNext != arena))
			{
				arena.OffsetNext = (arenaNext == null) ? 0 : arenaNext.Offset;
				if(arenaNext != null)
					arenaNext.OffsetPrev = arena.Offset;
			}
			return arena;
		}

		/// <summary>
		/// Define a area anterior.
		/// </summary>
		/// <param name="arena"></param>
		/// <param name="arenaPrev"></param>
		/// <returns></returns>
		private static MemArea SetPreviousArena(MemArea arena, MemArea arenaPrev)
		{
			if((arenaPrev == null) || (arenaPrev != arena))
			{
				arena.OffsetPrev = (arenaPrev == null) ? 0 : arenaPrev.Offset;
				if(arenaPrev != null)
					arenaPrev.OffsetNext = arena.Offset;
			}
			return arena;
		}

		/// <summary>
		/// Adquire uma area adjacente.
		/// </summary>
		/// <param name="area"></param>
		/// <returns></returns>
		internal static MemArea CoalesceAdjacent(MemArea area)
		{
			if(area.IsFree)
			{
				MemArea arenaCopy = area.NextArea();
				int num = 0;
				if(arenaCopy != null)
				{
					uint num2 = 0;
					while ((arenaCopy != null) && arenaCopy.IsFree)
					{
						num++;
						num2 += arenaCopy.TotalLength;
						arenaCopy = arenaCopy.NextArea();
					}
					if(num2 > 0)
					{
						area.Capacity += num2;
						uint capacity = area.Capacity;
						area = SetNextArea(area, GetActualArea(arenaCopy));
						area.Capacity = capacity;
					}
				}
				if(area.HasPrevious)
				{
					uint num4 = 0;
					MemArea arena3 = area;
					MemArea arena4 = null;
					while (true)
					{
						arena4 = arena3.PreviousArea();
						if((arena4 == null) || !arena4.IsFree)
						{
							break;
						}
						num4 += arena4.TotalLength;
						arena3 = arena4;
						num++;
					}
					if(num4 > 0)
					{
						area.Capacity += num4;
						uint num5 = area.Capacity;
						area.NextArea();
						area = SetNextArea(arena3, GetActualArea(arenaCopy));
						area.Capacity = num5;
					}
				}
				if(num > 0)
				{
					View view = area.View;
					view.MyFreeSpace += (uint)(num * Header.Size);
				}
			}
			return area;
		}

		/// <summary>
		/// Recupera a <see cref="MemArea"/> associado com os dados armazenados na <see cref="MemArea"/> informada.
		/// </summary>
		/// <param name="areaCopy"><see cref="MemArea"/> com os dados do item de armazenamento.</param>
		/// <returns></returns>
		internal static MemArea GetActualArea(MemArea areaCopy)
		{
			if(areaCopy == null)
				return null;
			if(!areaCopy.IsFree)
			{
				StoreItem item = StoreItem.FromBinary(areaCopy.GetMemContents(), areaCopy.View.ParentStorageProvider.CacheContext);
				return areaCopy.View.ParentStorageProvider.GetMemArea(item.Key);
			}
			return areaCopy;
		}

		/// <summary>
		/// Recupera o buffer do conteúdo da area.
		/// </summary>
		/// <returns></returns>
		public byte[] GetMemContents()
		{
			Content content = new Content();
			content.RawRead(this.RawView, (int)this.DataOffset);
			return content.Data;
		}

		/// <summary>
		/// Verifica se possui espaço para o tamanho informado.
		/// </summary>
		/// <param name="len">Tamanho que serão verificado.</param>
		/// <returns></returns>
		public bool HasDataSpace(uint len)
		{
			return (this.Capacity >= Content.RequiredSpace(len));
		}

		/// <summary>
		/// Recupera a próxima <see cref="MemArea"/> se existir.
		/// </summary>
		/// <returns></returns>
		public MemArea NextArea()
		{
			if(this.HasNext)
				return new MemArea(_view, this.OffsetNext);
			return null;
		}

		/// <summary>
		/// Recupera a <see cref="MemArea"/> anterior se existir.
		/// </summary>
		/// <returns></returns>
		public MemArea PreviousArea()
		{
			if(this.HasPrevious)
				return new MemArea(_view, this.OffsetPrev);
			return null;
		}

		/// <summary>
		/// Define o buffer com o conteúdo.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool SetMemContents(byte[] value)
		{
			int num = (int)(this.Capacity - Content.RequiredSpace((uint)value.Length));
			if(num < 0)
				return false;
			Content content = new Content();
			content.Data = value;
			content.RawWrite(this.RawView, (int)this.DataOffset);
			if(num > SPLIT_THRESHOLD)
			{
				MemArea arena = SplitArea(this, Content.RequiredSpace((uint)value.Length));
				if(arena != this)
					arena.IsFree = true;
			}
			return true;
		}

		/// <summary>
		/// Divida a <see cref="MemArea"/> em partes com o tamanhon informado.
		/// </summary>
		/// <param name="area"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		internal static MemArea SplitArea(MemArea area, uint size)
		{
			uint num = size + ((uint)Header.Size);
			if(area.OffsetNext != 0)
				area.Capacity = area.OffsetNext - (area.Offset + ((uint)Header.Size));
			if(num > area.Capacity)
				return null;
			uint num2 = (area.Capacity - size) - ((uint)Header.Size);
			if(num2 < SPLIT_THRESHOLD)
			{
				View view1 = area.View;
				view1.MyFreeSpace -= area.Capacity + ((uint)Header.Size);
				return area;
			}
			area.Capacity = size;
			MemArea arena2 = area.View.AreaAtOffset(area.Offset + area.TotalLength);
			arena2.Capacity = num2;
			View view = area.View;
			view.MyFreeSpace -= num;
			MemArea arenaCopy = area.NextArea();
			SetNextArea(arena2, GetActualArea(arenaCopy));
			SetNextArea(area, arena2);
			return arena2;
		}

		/// <summary>
		/// Realiza a troca a <see cref="MemArea"/> adjacente.
		/// </summary>
		/// <param name="arena1"></param>
		/// <param name="arena2"></param>
		internal static void SwapAdjacent(MemArea arena1, MemArea arena2)
		{
			arena1 = GetActualArea(arena1);
			arena2 = GetActualArea(arena2);
			if(AreAdjacent(arena1, arena2))
			{
				MemArea arena = arena1;
				MemArea arena3 = arena2;
				if(arena1.Offset > arena2.Offset)
				{
					arena = arena2;
					arena3 = arena1;
				}
				Header arenaHeader = arena.AreaHeader;
				Header header2 = arena3.AreaHeader;
				MemArea arenaCopy = arena3.NextArea();
				MemArea actualArena = arena.PreviousArea();
				arenaCopy = GetActualArea(arenaCopy);
				actualArena = GetActualArea(actualArena);
				int count = arena.IsFree ? Header.Size : ((int)arena.TotalLength);
				int num2 = arena3.IsFree ? Header.Size : ((int)arena3.TotalLength);
				byte[] buffer = arena.RawView.Read((int)arena.Offset, count);
				byte[] buffer2 = arena3.RawView.Read((int)arena3.Offset, num2);
				arena3._offset = arena.Offset + arena3.TotalLength;
				arena.RawView.Write(buffer2, (int)arena.Offset, buffer2.Length);
				arena3.RawView.Write(buffer, (int)arena3.Offset, buffer.Length);
				arena.IsFree = header2.IsFree;
				arena.Capacity = header2.Capacity;
				arena.OffsetNext = arena3.Offset;
				arena.OffsetPrev = arenaHeader.OffsetPrev;
				SetPreviousArena(arena, actualArena);
				arena3.IsFree = arenaHeader.IsFree;
				arena3.Capacity = arenaHeader.Capacity;
				arena3.OffsetNext = header2.OffsetNext;
				arena3.OffsetPrev = arena.Offset;
				SetNextArea(arena3, arenaCopy);
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(" - Arena, ").Append(this.IsFree ? "-" : "X").Append(", Cap.=").Append(this.Capacity).Append(", Off.=").Append(this.Offset).Append(", N=").Append(this.OffsetNext).Append(", P=").Append(this.OffsetPrev);
			return builder.ToString();
		}

		/// <summary>
		/// Representa o conteúdo dos dados.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Content
		{
			public byte[] Data;

			public static uint RequiredSpace(uint dataLen)
			{
				return (dataLen + 4);
			}

			public void RawRead(MmfFileView raw, int offset)
			{
				int count = raw.ReadInt32(offset);
				this.Data = new byte[count];
				raw.Read(this.Data, offset + 4, count);
			}

			public void RawWrite(MmfFileView raw, int offset)
			{
				raw.WriteInt32(offset, this.Data.Length);
				raw.Write(this.Data, offset + 4, this.Data.Length);
			}
		}

		/// <summary>
		/// Representa um cabeçalho dos dados.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Header
		{
			public const byte FREE = 0;

			public const byte USED = 1;

			public byte Status;

			public uint Capacity;

			public uint OffsetNext;

			public uint OffsetPrev;

			/// <summary>
			/// Verifica se está livre.
			/// </summary>
			public bool IsFree
			{
				get
				{
					return (this.Status == FREE);
				}
			}

			/// <summary>
			/// Verifica se existe algum item a frente.
			/// </summary>
			public bool HasNext
			{
				get
				{
					return (this.OffsetNext > 0);
				}
			}

			/// <summary>
			/// Verifica se existe algum item para atras
			/// </summary>
			public bool HasPrevious
			{
				get
				{
					return (this.OffsetPrev > 0);
				}
			}

			/// <summary>
			/// Tamanho do cabeçalho.
			/// </summary>
			public static int Size
			{
				get
				{
					return Marshal.SizeOf(typeof(MemArea.Header));
				}
			}

			/// <summary>
			/// Lê os dados da view informada.
			/// </summary>
			/// <param name="raw"></param>
			/// <param name="offset"></param>
			public void RawRead(MmfFileView raw, int offset)
			{
				this.Status = raw.ReadByte(offset);
				offset++;
				this.Capacity = raw.ReadUInt32(offset);
				offset += 4;
				this.OffsetNext = raw.ReadUInt32(offset);
				offset += 4;
				this.OffsetPrev = raw.ReadUInt32(offset);
			}

			/// <summary>
			/// Escreve os dados na view informada.
			/// </summary>
			/// <param name="raw"></param>
			/// <param name="offset"></param>
			public void RawWrite(MmfFileView raw, int offset)
			{
				raw.WriteByte(offset, this.Status);
				offset++;
				raw.WriteUInt32(offset, this.Capacity);
				offset += 4;
				raw.WriteUInt32(offset, this.OffsetNext);
				offset += 4;
				raw.WriteUInt32(offset, this.OffsetPrev);
			}
		}
	}
}
