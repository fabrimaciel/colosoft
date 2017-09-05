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
	/// Representa uma visão da memória.
	/// </summary>
	internal class View
	{
		private ViewHeader _hdr;

		private MemArea _lastFreeArena;

		private MmfFile _mmf;

		private MmfStorageProvider _parentStorageProvider;

		private uint _size;

		private int _usage;

		private uint _vid;

		private MmfFileView _view;

		/// <summary>
		/// Quantidade de espaço livre.
		/// </summary>
		public uint FreeSpace
		{
			get
			{
				return _hdr.FreeSpace;
			}
			set
			{
				_hdr.FreeSpace = value;
				if(this.IsOpen)
				{
					_view.WriteUInt32(4, _hdr.FreeSpace);
				}
			}
		}

		/// <summary>
		/// Identifica se possui um cabeçalho válido.
		/// </summary>
		public bool HasValidHeader
		{
			get
			{
				return _hdr.IsValid;
			}
		}

		/// <summary>
		/// Identificador.
		/// </summary>
		public uint ID
		{
			get
			{
				return _vid;
			}
		}

		/// <summary>
		/// Identifica se a instancia está aberta.
		/// </summary>
		public bool IsOpen
		{
			get
			{
				return (_view != null);
			}
		}

		/// <summary>
		/// Instancia da última <see cref="MemArea"/> livre.
		/// </summary>
		public MemArea LastFreeArea
		{
			get
			{
				return _lastFreeArena;
			}
			set
			{
				value = _lastFreeArena;
			}
		}

		/// <summary>
		/// Espaço máximo livre.
		/// </summary>
		public uint MaxFreeSpace
		{
			get
			{
				return _hdr.MaxFreeSpace;
			}
			set
			{
				_hdr.MaxFreeSpace = value;
				if(this.IsOpen)
				{
					_view.WriteUInt32(8, _hdr.MaxFreeSpace);
				}
			}
		}

		/// <summary>
		/// <see cref="MmfFileView"/> associado.
		/// </summary>
		internal MmfFileView MmfView
		{
			get
			{
				return _view;
			}
		}

		/// <summary>
		/// Espaço livre.
		/// </summary>
		public uint MyFreeSpace
		{
			get
			{
				return _hdr.MyFreeSpace;
			}
			set
			{
				_hdr.MyFreeSpace = value;
			}
		}

		/// <summary>
		/// Provedor de armazenamento associado.
		/// </summary>
		internal MmfStorageProvider ParentStorageProvider
		{
			get
			{
				return _parentStorageProvider;
			}
			set
			{
				_parentStorageProvider = value;
			}
		}

		/// <summary>
		/// Assinatura da instancia.
		/// </summary>
		internal uint Signature
		{
			get
			{
				return _hdr.Signature;
			}
			set
			{
				_hdr.Signature = value;
				if(this.IsOpen)
				{
					_view.WriteUInt32(0, _hdr.Signature);
				}
			}
		}

		/// <summary>
		/// Tamanho dos dados.
		/// </summary>
		public uint Size
		{
			get
			{
				return _size;
			}
		}

		/// <summary>
		/// Quantidade de memória utilizada.
		/// </summary>
		public int Usage
		{
			get
			{
				return _usage;
			}
			set
			{
				_usage = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="mmf"></param>
		/// <param name="id"></param>
		/// <param name="size">Tamanho dos dados.</param>
		public View(MmfFile mmf, uint id, uint size)
		{
			_mmf = mmf;
			_vid = id;
			_size = size;
		}

		/// <summary>
		/// Lê o cabeçalho.
		/// </summary>
		private void ReadHeader()
		{
			if(this.IsOpen)
				_hdr.RawRead(_view, 0);
		}

		/// <summary>
		/// Procura pela primeira <see cref="MemArea"/> livre.
		/// </summary>
		/// <param name="memRequirements">Quantidade de memória requerida.</param>
		/// <returns><see cref="MemArea"/> disponível.</returns>
		private MemArea FindFirstFreeArena(uint memRequirements)
		{
			memRequirements += (uint)MemArea.Header.Size;
			MemArea arena = this.FirstArea();
			if(arena.OffsetNext != 0)
				arena.Capacity = arena.OffsetNext - (arena.Offset + ((uint)MemArea.Header.Size));
			if(((_lastFreeArena == null) || !_lastFreeArena.IsFree) || (_lastFreeArena.Capacity < memRequirements))
			{
				while (arena != null)
				{
					if(arena.IsFree && (arena.Capacity >= memRequirements))
						return arena;
					arena = arena.NextArea();
				}
				return null;
			}
			return _lastFreeArena;
		}

		/// <summary>
		/// Calcula a area da memória usada.
		/// </summary>
		private void CalculateAreaMemoryUsage()
		{
			MemArea arena = this.FirstArea();
			MemArea arena2 = this.FirstArea();
			uint capacity = 0;
			uint num2 = 0;
			while (arena != null)
			{
				try
				{
					if(arena.IsFree)
					{
						num2 += arena.Capacity;
						if(arena.Capacity >= capacity)
							capacity = arena.Capacity;
					}
					arena = arena.NextArea();
					if((arena != null) && (arena.OffsetNext == arena2.Offset))
						arena.OffsetNext = 0;
					continue;
				}
				catch(Exception exception)
				{
					throw new Exception(exception.Message);
				}
			}
			this.FreeSpace = num2;
			this.MaxFreeSpace = capacity;
			this.MyFreeSpace = this.FreeSpace + ((uint)MemArea.Header.Size);
		}

		/// <summary>
		/// Aloca uma <see cref="MemArea"/> com o tamanho informado.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public MemArea Allocate(uint size)
		{
			if(size > this.FreeSpace)
				return null;
			MemArea arena = null;
			size = MemArea.Content.RequiredSpace(size);
			arena = this.FindFirstFreeArena(size);
			if(arena == null)
			{
				this.CalculateAreaMemoryUsage();
				arena = this.DeFragment();
				if(arena == null)
					return arena;
			}
			MemArea arena2 = MemArea.SplitArea(arena, size);
			if(arena2 != null)
			{
				arena.IsFree = false;
				if(arena != arena2)
				{
					arena2.IsFree = true;
					_lastFreeArena = arena2;
				}
				this.FreeSpace = this.MyFreeSpace - ((uint)MemArea.Header.Size);
				this.Usage++;
			}
			return arena;
		}

		/// <summary>
		/// Recupera uma <see cref="MemArea"/> com o offset informado.
		/// </summary>
		/// <param name="offSet"></param>
		/// <returns></returns>
		public MemArea AreaAtOffset(uint offSet)
		{
			return new MemArea(this, offSet);
		}

		/// <summary>
		/// Fecha a <see cref="View"/>.
		/// </summary>
		public virtual int Close()
		{
			if(this.IsOpen)
			{
				_mmf.UnMapView(_view);
				_view = null;
				_usage = 0;
			}
			return 0;
		}

		/// <summary>
		/// Desaloca a <see cref="MemArea"/> informada.
		/// </summary>
		/// <param name="area"><see cref="MemArea"/> que será desalocada.</param>
		/// <returns></returns>
		public MemArea DeAllocate(MemArea area)
		{
			if(!area.IsFree)
			{
				uint capacity = area.Capacity;
				View view = area.View;
				view.MyFreeSpace += area.Capacity;
				area.IsFree = true;
				area = MemArea.CoalesceAdjacent(area);
				area.IsFree = true;
				this.FreeSpace = this.MyFreeSpace - ((uint)MemArea.Header.Size);
				this.Usage++;
			}
			return area;
		}

		/// <summary>
		/// Desfragmenta.
		/// </summary>
		/// <returns></returns>
		public MemArea DeFragment()
		{
			MemArea arena2;
			MemArea arena = this.FirstArea();
			arena2 = arena.NextArea();
			while (arena2 != null)
			{
				if(!arena.IsFree)
					arena = arena2;
				else if(arena2.IsFree)
					arena = MemArea.CoalesceAdjacent(arena2);
				else
				{
					MemArea.SwapAdjacent(arena2, arena);
					arena = arena2;
				}
				arena2 = arena.NextArea();
			}
			this.FreeSpace = arena.Capacity;
			this.MaxFreeSpace = arena.Capacity;
			this.CalculateAreaMemoryUsage();
			return arena;
		}

		/// <summary>
		/// Recupera a primeira <see cref="MemArea"/> da instancia.
		/// </summary>
		/// <returns></returns>
		public MemArea FirstArea()
		{
			return new MemArea(this, (uint)ViewHeader.Size);
		}

		/// <summary>
		/// Formata os dados da instancia.
		/// </summary>
		public void Format()
		{
			if(this.IsOpen)
			{
				int size = ViewHeader.Size;
				this.Signature = 0xface0ff;
				this.FreeSpace = this.MaxFreeSpace = _view.Length - ((uint)size);
				this.MyFreeSpace = this.FreeSpace;
				_parentStorageProvider = null;
				MemArea arena = this.FirstArea();
				arena.IsFree = true;
				arena.Capacity = (uint)((_view.Length - size) - MemArea.Header.Size);
				arena.OffsetNext = 0;
				arena.OffsetPrev = 0;
			}
		}

		/// <summary>
		/// Abrea a visão.
		/// </summary>
		public virtual int Open()
		{
			_view = _mmf.MapView((ulong)(_vid * _size), _size);
			if(_view != null)
				this.ReadHeader();
			return 0;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("View, ID=").Append(this.ID).Append(", maxFree=").Append(this.MaxFreeSpace).Append(", Free=").Append(this.FreeSpace).Append("\r\n");
			if(this.IsOpen)
			{
				for(MemArea arena = this.FirstArea(); arena != null; arena = arena.NextArea())
				{
					builder.Append(arena.ToString()).Append("\r\n");
				}
			}
			return builder.ToString();
		}

		/// <summary>
		/// Representa os dados do cabeçalho da <see cref="View"/>.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct ViewHeader
		{
			public const uint SIGNATURE = 0xface0ff;

			public uint Signature;

			public uint FreeSpace;

			public uint MaxFreeSpace;

			public uint MyFreeSpace;

			public bool IsValid
			{
				get
				{
					return (this.Signature == 0xface0ff);
				}
			}

			public static int Size
			{
				get
				{
					return Marshal.SizeOf(typeof(View.ViewHeader));
				}
			}

			public void RawRead(MmfFileView view, int offset)
			{
				this.Signature = view.ReadUInt32(offset);
				offset += 4;
				this.FreeSpace = view.ReadUInt32(offset);
				offset += 4;
				this.MaxFreeSpace = view.ReadUInt32(offset);
			}

			public void RawWrite(MmfFileView view, int offset)
			{
				view.WriteUInt32(offset, this.Signature);
				offset += 4;
				view.WriteUInt32(offset, this.FreeSpace);
				offset += 4;
				view.WriteUInt32(offset, this.MaxFreeSpace);
			}
		}
	}
}
