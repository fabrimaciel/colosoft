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
using System.Collections;
using Colosoft.Caching.Interop;

namespace Colosoft.Caching.Storage.Mmf
{
	/// <summary>
	/// Implementação do Memory Mapped File para armazenamento.
	/// </summary>
	internal class MmfStorage : IDisposable
	{
		private string _fileName;

		private uint _initialSizeMB = 0x20;

		private MmfFile _mmf;

		private uint _viewCount = 8;

		private ViewManager _viewManager;

		private uint _viewSize = 0x400000;

		/// <summary>
		/// Nome do arquivo.
		/// </summary>
		public string FileName
		{
			get
			{
				return _fileName;
			}
		}

		/// <summary>
		/// Identifica se é uma página.
		/// </summary>
		public bool IsPageFileStore
		{
			get
			{
				return _mmf.IsPageFile;
			}
		}

		/// <summary>
		/// Cresce a memória mapeada.
		/// </summary>
		/// <param name="numViewsToAdd">Quantidade de views que serão adicionadas.</param>
		private void GrowMemoryMappedStore(int numViewsToAdd)
		{
			try
			{
				ulong maxLength = _mmf.MaxLength + ((ulong)(numViewsToAdd * _viewSize));
				_mmf.SetMaxLength(maxLength);
				_viewManager.ExtendViewsBucket(numViewsToAdd);
			}
			catch(OutOfMemoryException)
			{
				throw;
			}
			catch(Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Fecha a memória mapeada.
		/// </summary>
		private void CloseMemoryMappedStore()
		{
			try
			{
				_viewManager.CloseAllViews();
				_mmf.Close();
			}
			catch(Exception exception)
			{
				Colosoft.Logging.Trace.Error("MmfStorage.CloseMemoryMappedStoreError:".GetFormatter(), exception.GetFormatter());
				throw;
			}
		}

		/// <summary>
		/// Adiciona os dados do item na memória.
		/// </summary>
		/// <param name="item">Buffer com os dados que serão adicionados.</param>
		/// <returns>Ponteiro dos dados adicionados.</returns>
		public MmfObjectPtr Add(byte[] item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			if(item.Length > _viewSize)
				throw new ArgumentException("item size is larger than view size");
			View matchingView = _viewManager.GetMatchingView((uint)item.Length);
			try
			{
				if(matchingView == null)
				{
					this.GrowMemoryMappedStore(1);
					matchingView = _viewManager.GetMatchingView((uint)item.Length);
				}
				if(matchingView != null)
				{
					MemArea area = matchingView.Allocate((uint)item.Length);
					if(area == null)
						return null;
					if(!area.SetMemContents(item))
					{
						matchingView.DeAllocate(area);
						return null;
					}
					return new MmfObjectPtr(matchingView, area);
				}
			}
			catch(OutOfMemoryException)
			{
				throw;
			}
			catch(Exception)
			{
				throw;
			}
			return null;
		}

		/// <summary>
		/// Limpa os dados da instancia.
		/// </summary>
		public void Clear()
		{
			_viewManager.ClearAllViews();
		}

		/// <summary>
		/// Recupera os dados associados com o ponteiro.
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public byte[] Get(MmfObjectPtr info)
		{
			info.Require("info").NotNull();
			if(!info.View.IsOpen)
				_viewManager.OpenView(info.View);
			byte[] memContents = info.Area.GetMemContents();
			info.View.Usage++;
			return memContents;
		}

		/// <summary>
		/// Recupera o enumerador para pecorrer os itens da instancia.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return new MmfStorageEnumerator(this);
		}

		/// <summary>
		/// Recupera o ponteiro para os dados armazenados.
		/// </summary>
		/// <param name="viewId">Identificador da View.</param>
		/// <param name="offset">Offset para recupera os dados.</param>
		/// <returns></returns>
		public MmfObjectPtr GetPtr(uint viewId, uint offset)
		{
			View viewByID = _viewManager.GetViewByID(viewId);
			if(viewByID != null)
			{
				try
				{
					return new MmfObjectPtr(viewByID, viewByID.AreaAtOffset(offset));
				}
				catch(Exception)
				{
				}
			}
			return null;
		}

		/// <summary>
		/// Insere os dados no armazenamento.
		/// </summary>
		/// <param name="info">Informações para onde os dados serão salvos.</param>
		/// <param name="item">Dados que serão salvos.</param>
		/// <returns></returns>
		public MmfObjectPtr Insert(MmfObjectPtr info, byte[] item)
		{
			info.Require("info").NotNull();
			item.Require("item").NotNull();
			if(!info.View.IsOpen)
				_viewManager.OpenView(info.View);
			if(info.Area.HasDataSpace((uint)item.Length))
			{
				info.Area.SetMemContents(item);
				return info;
			}
			info = this.Add(item);
			return info;
		}

		/// <summary>
		/// Abre o armazenamento da memória mapeada.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="viewCount"></param>
		/// <param name="viewSize"></param>
		/// <param name="initialSizeMB"></param>
		public void OpenMemoryMappedStore(string fileName, uint viewCount, uint viewSize, uint initialSizeMB)
		{
			try
			{
				_fileName = fileName;
				_viewCount = viewCount;
				_viewSize = SysUtil.AllignViewSize(viewSize);
				_initialSizeMB = initialSizeMB;
				_mmf = MmfFile.Create(_fileName, (ulong)(_initialSizeMB * 0x100000), false);
				_viewManager = new ViewManager(_mmf, _viewSize);
				_viewManager.CreateInitialViews(_viewCount);
			}
			catch(Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Recupera e remove dos dados do ponteiro informado.
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public byte[] Remove(MmfObjectPtr info)
		{
			info.Require("info").NotNull();
			if(!info.View.IsOpen)
				_viewManager.OpenView(info.View);
			byte[] memContents = info.Area.GetMemContents();
			info.View.DeAllocate(info.Area);
			return memContents;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder(0x400);
			builder.Append(_viewManager);
			return builder.ToString();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			CloseMemoryMappedStore();
		}

		/// <summary>
		/// Implementação do enumerador para o armazenamento.
		/// </summary>
		private class MmfStorageEnumerator : IEnumerator
		{
			private MemArea _area;

			private MmfStorage _storage;

			private View _view;

			/// <summary>
			/// Instancia ativa.
			/// </summary>
			object IEnumerator.Current
			{
				get
				{
					_storage._viewManager.OpenView(_view);
					return _area;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="storage"></param>
			public MmfStorageEnumerator(MmfStorage storage)
			{
				_storage = storage;
				_view = _storage._viewManager.GetViewByID(0);
				_area = null;
			}

			/// <summary>
			/// Move para o próximo item.
			/// </summary>
			/// <returns></returns>
			bool IEnumerator.MoveNext()
			{
				if(_view == null)
				{
					return false;
				}
				_storage._viewManager.OpenView(_view);
				if(_area == null)
					_area = _view.FirstArea();
				else
				{
					_area = _area.NextArea();
					if(_area == null)
					{
						_view = _storage._viewManager.GetViewByID(_view.ID + 1);
						if(_view != null)
						{
							_storage._viewManager.OpenView(_view);
							_area = _view.FirstArea();
						}
					}
				}
				return (_area != null);
			}

			/// <summary>
			/// Reseta o enumerado.
			/// </summary>
			void IEnumerator.Reset()
			{
				_view = _storage._viewManager.GetViewByID(0);
				_area = null;
			}
		}
	}
}
