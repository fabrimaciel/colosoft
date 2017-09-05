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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using System.Threading;

namespace Colosoft.Collections
{
	/// <summary>
	/// Armazena os argumentos de um evento de erro.
	/// </summary>
	public class AsyncVirtualListErrorEventArgs : EventArgs
	{
		private Exception _error;

		/// <summary>
		/// Instancia do erro associado.
		/// </summary>
		public virtual Exception Error
		{
			get
			{
				return _error;
			}
			protected set
			{
				_error = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="error"></param>
		public AsyncVirtualListErrorEventArgs(Exception error)
		{
			_error = error;
		}
	}
	/// <summary>
	/// Representa uma coleção de virtualização de carga assincrona.
	/// </summary>
	public class AsyncVirtualList<T> : VirtualList<DataWrapper<T>>
	{
		private bool _isLoading;

		private SynchronizationContext _synchronizationContext;

		private int _loadedItemsCount = 0;

		private bool _instanceInitialized;

		/// <summary>
		/// Evento acionado quando ocorre algum erro na lista assincrona.
		/// </summary>
		public event AsyncVirtualListErrorHandler AsyncVirtualListError;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="loader"></param>
		/// <param name="refObject"></param>
		/// <param name="pageSize"></param>
		public AsyncVirtualList(int pageSize, VirtualListLoaderHandler<T> loader, object refObject) : base(pageSize, ConverterLoader(loader), refObject)
		{
			_synchronizationContext = SynchronizationContext.Current;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="loader"></param>
		/// <param name="refObject"></param>
		/// <param name="pageSize"></param>
		public AsyncVirtualList(int pageSize, VirtualListLoader<T> loader, object refObject) : base(pageSize, ConverterLoader(loader), refObject)
		{
			_synchronizationContext = SynchronizationContext.Current;
		}

		/// <summary>
		/// Assinatura do evento acionado quando ocorre algum erro na lista assincrona.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void AsyncVirtualListErrorHandler (object sender, EventArgs e);

		/// <summary>
		/// Número de items carregados.
		/// </summary>
		public int LoadedItemsCount
		{
			get
			{
				return _loadedItemsCount;
			}
		}

		/// <summary>
		/// Representa se a coleção está carregando.
		/// </summary>
		public bool IsLoading
		{
			get
			{
				return _isLoading;
			}
			set
			{
				if(_isLoading != value)
				{
					_isLoading = value;
					RaisePropertyChanged("IsLoading");
				}
			}
		}

		/// <summary>
		/// Determina se a intancia foi inicializada.
		/// </summary>
		public override bool InstanceInitialized
		{
			get
			{
				return _instanceInitialized;
			}
			set
			{
				if(value != _instanceInitialized)
				{
					_instanceInitialized = value;
					RaisePropertyChanged("InstanceInitialized");
				}
			}
		}

		/// <summary>
		/// Quantidade de registros na coleção.
		/// </summary>
		public override int Count
		{
			get
			{
				if(!InstanceInitialized)
				{
					InstanceInitialized = true;
					var args = new VirtualListLoaderEventArgs(0, 0, true, ReferenceObject);
					if(!ThreadPool.QueueUserWorkItem(LoadCountWork, args))
						LoadCountWork(args);
				}
				return _count;
			}
		}

		/// <summary>
		/// Método acionado quando ocorre algum erro na lista assincrona.
		/// </summary>
		/// <param name="error"></param>
		protected virtual void OnAsyncVirtualListError(Exception error)
		{
			if(AsyncVirtualListError != null)
				AsyncVirtualListError(this, new AsyncVirtualListErrorEventArgs(error));
		}

		/// <summary>
		/// Cria a páginda de dados.
		/// </summary>
		/// <param name="sessionIndex"></param>
		/// <returns></returns>
		protected override IDataPage<DataWrapper<T>> CreateDataPage(int sessionIndex)
		{
			return new DataWrapperPage<T>(sessionIndex, PageSize);
		}

		/// <summary>
		/// Carrega a virtual list.
		/// </summary>
		/// <param name="startRow"></param>
		/// <param name="pageSize"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		protected override VirtualListLoaderResult<DataWrapper<T>> Load(int startRow, int pageSize, int index)
		{
			int indexSession = startRow / pageSize;
			var page = new DataWrapperPage<T>(indexSession, pageSize);
			Sessions[indexSession] = page;
			OnDataPageLoaded(new DataPageLoadedEventArgs<DataWrapper<T>>(page));
			IsLoading = true;
			var args = new VirtualListLoaderEventArgs(startRow, pageSize, !InstanceInitialized, ReferenceObject);
			var state = new object[] {
				args,
				index
			};
			if(!ThreadPool.QueueUserWorkItem(LoadPageWork, state))
				LoadPageWork(state);
			return null;
		}

		/// <summary>
		/// Recupera um item.
		/// </summary>
		/// <param name="index">Índice do item.</param>
		/// <returns></returns>
		protected internal override DataWrapper<T> GetItem(int index)
		{
			int indexSession = PageSize == 0 ? 0 : (int)Math.Floor(index / (double)PageSize);
			if(!InstanceInitialized)
			{
				InstanceInitialized = true;
				Load(indexSession * PageSize, PageSize, index);
			}
			else
			{
				if(index >= Count2)
					throw new ArgumentOutOfRangeException();
				if(Sessions[indexSession] == null)
				{
					Load(indexSession * PageSize, PageSize, index);
				}
			}
			return Sessions[indexSession][index - (indexSession * PageSize)];
		}

		/// <summary>
		/// Método usado para converter o loader.
		/// </summary>
		/// <param name="loader"></param>
		/// <returns></returns>
		private static VirtualListLoader<DataWrapper<T>> ConverterLoader(VirtualListLoaderHandler<T> loader)
		{
			return VirtualListLoader<DataWrapper<T>>.Create((sender, e) =>  {
				var result = loader(sender, e);
				if(result.Error != null)
					return new VirtualListLoaderResult<DataWrapper<T>>(result.Error);
				IEnumerable<DataWrapper<T>> items = result.Items != null ? result.Items.Select(f => new DataWrapper<T>(0, f)) : null;
				if(result.UpdateCount)
					return new VirtualListLoaderResult<DataWrapper<T>>(items, result.NewCount);
				return new VirtualListLoaderResult<DataWrapper<T>>(items);
			});
		}

		/// <summary>
		/// Método usado para converter o loader.
		/// </summary>
		/// <param name="loader"></param>
		/// <returns></returns>
		private static VirtualListLoader<DataWrapper<T>> ConverterLoader(VirtualListLoader<T> loader)
		{
			return VirtualListLoader<DataWrapper<T>>.Create((sender, e) =>  {
				var result = loader.Load((IObservableCollection)sender, e.StartRow, e.PageSize, e.NeedItemsCount, e.ReferenceObject);
				if(result.Error != null)
					return new VirtualListLoaderResult<DataWrapper<T>>(result.Error);
				IEnumerable<DataWrapper<T>> items = result.Items != null ? result.Items.Select(f => new DataWrapper<T>(0, f)) : null;
				if(result.UpdateCount)
					return new VirtualListLoaderResult<DataWrapper<T>>(items, result.NewCount);
				return new VirtualListLoaderResult<DataWrapper<T>>(items);
			});
		}

		/// <summary>
		/// Recupera a nova quantidade de itens da coleção.
		/// </summary>
		/// <param name="newCount"></param>
		private void TakeNewCount(int newCount)
		{
			if(newCount != this.Count)
				RaiseCollectionReset();
		}

		/// <summary>
		/// Método assíncrono que carrega um página.
		/// </summary>
		/// <param name="args"></param>
		private void LoadPageWork(object args)
		{
			var objArray = (object[])args;
			var loaderArgs = (VirtualListLoaderEventArgs)objArray[0];
			var index = (int)objArray[1];
			try
			{
				var result = Loader.Load(this, loaderArgs.StartRow, loaderArgs.PageSize, loaderArgs.NeedItemsCount, loaderArgs.ReferenceObject);
				_synchronizationContext.Send(LoadPageCompleted, new object[] {
					loaderArgs,
					result,
					index
				});
			}
			catch(Exception ex)
			{
				if(_synchronizationContext != null)
					_synchronizationContext.Send(LoadPageError, new object[] {
						loaderArgs,
						ex,
						index
					});
				OnAsyncVirtualListError(ex);
			}
		}

		/// <summary>
		/// Evento acionado quando ocorre um erro ao carregar a página de dados.
		/// </summary>
		/// <param name="args"></param>
		private void LoadPageError(object args)
		{
			var objArray = (object[])args;
			var loaderArgs = (VirtualListLoaderEventArgs)objArray[0];
			var result = (Exception)objArray[1];
			var index = (int)objArray[2];
			if(index >= Count2)
				throw new ArgumentOutOfRangeException();
			int sessionId = PageSize == 0 ? 0 : loaderArgs.StartRow / PageSize;
			Sessions[sessionId].NotifyError(result);
			IsLoading = false;
		}

		/// <summary>
		/// Método que completa a carga de um página.
		/// </summary>
		/// <param name="args"></param>
		private void LoadPageCompleted(object args)
		{
			var objArray = (object[])args;
			var loaderArgs = (VirtualListLoaderEventArgs)objArray[0];
			var result = (VirtualListLoaderResult<DataWrapper<T>>)objArray[1];
			var index = (int)objArray[2];
			ClearPage(result);
			if(index >= Count2)
				throw new ArgumentOutOfRangeException();
			int sessionId = PageSize == 0 ? 0 : loaderArgs.StartRow / PageSize;
			Sessions[sessionId].Populate(result.Items);
			IsLoading = false;
			_loadedItemsCount += Sessions[sessionId].Count;
			TakeNewCount(result.NewCount);
			RaisePropertyChanged("Count");
			RaisePropertyChanged("LoadedItemsCount");
		}

		/// <summary>
		/// Método assíncrono que faz a carga da contagem.
		/// </summary>
		/// <param name="args"></param>
		private void LoadCountWork(object args)
		{
			var loaderArgs = (VirtualListLoaderEventArgs)args;
			VirtualListLoaderResult<DataWrapper<T>> result = null;
			if(Loader != null)
				try
				{
					result = Loader.Load(this, loaderArgs.StartRow, loaderArgs.PageSize, loaderArgs.NeedItemsCount, loaderArgs.ReferenceObject);
				}
				catch(Exception ex)
				{
					result = new VirtualListLoaderResult<DataWrapper<T>>(ex);
				}
			else
				result = new VirtualListLoaderResult<DataWrapper<T>>(new DataWrapper<T>[0], 0);
			if(_synchronizationContext != null)
				_synchronizationContext.Send(LoadCountCompleted, new object[] {
					loaderArgs,
					result
				});
			if(result.Error != null)
				OnAsyncVirtualListError(result.Error);
		}

		/// <summary>
		/// Método que completa a carga da contagem.
		/// </summary>
		/// <param name="args"></param>
		private void LoadCountCompleted(object args)
		{
			var objArray = (object[])args;
			var loaderArgs = (VirtualListLoaderEventArgs)objArray[0];
			var result = (VirtualListLoaderResult<DataWrapper<T>>)objArray[1];
			ClearPage(result);
			IsLoading = false;
			RaiseCollectionReset();
			RaisePropertyChanged("Count");
		}

		/// <summary>
		/// Define a contagem inicial.
		/// </summary>
		/// <param name="count"></param>
		internal void SetInitialCount(int count)
		{
			var result = new VirtualListLoaderResult<DataWrapper<T>>(null, count);
			ClearPage(result);
			InstanceInitialized = true;
			RaiseCollectionReset();
			RaisePropertyChanged("Count");
		}
	}
}
