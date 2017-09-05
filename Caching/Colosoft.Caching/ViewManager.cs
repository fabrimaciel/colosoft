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
	/// Representa o gerenciador das visões da memória.
	/// </summary>
	internal class ViewManager
	{
		private uint _maxViews;

		private MmfFile _mmf;

		private ArrayList _viewsClosed;

		private uint _viewSize;

		private ArrayList _viewsOpen;

		private readonly uint RESERVED = SysUtil.AllignViewSize(0);

		/// <summary>
		/// Quantidade de views do gerenciador.
		/// </summary>
		public int ViewCount
		{
			get
			{
				return (_viewsClosed.Count + _viewsOpen.Count);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="mmf"></param>
		/// <param name="viewSize">Tamanho da visualização.</param>
		public ViewManager(MmfFile mmf, uint viewSize)
		{
			_mmf = mmf;
			_viewSize = viewSize;
			_viewsOpen = new ArrayList();
			_viewsClosed = new ArrayList();
		}

		/// <summary>
		/// Recupera uma view livre.
		/// </summary>
		/// <returns></returns>
		private View GetFreeView()
		{
			if(_viewsClosed.Count > 0)
				return (View)_viewsClosed[0];
			return null;
		}

		/// <summary>
		/// Recupera a última view usada e que está livre.
		/// </summary>
		/// <returns></returns>
		private View GetLeastUsedFreeView()
		{
			View view = null;
			for(int i = 0; i < _viewsClosed.Count; i++)
			{
				View view2 = (View)_viewsClosed[i];
				if(view == null)
					view = view2;
				else if(view2.Usage < view.Usage)
					view = view2;
			}
			return view;
		}

		/// <summary>
		/// Recupera a última view aberta e usada.
		/// </summary>
		/// <returns></returns>
		private View GetLeastUsedOpenView()
		{
			View view = null;
			for(int i = 0; i < _viewsOpen.Count; i++)
			{
				View view2 = (View)_viewsOpen[i];
				if(view == null)
					view = view2;
				else if(view2.Usage < view.Usage)
					view = view2;
			}
			return view;
		}

		/// <summary>
		/// Limpa todas as visualizações.
		/// </summary>
		public void ClearAllViews()
		{
			this.CloseAllViews();
			for(int i = _viewsClosed.Count - 1; i >= 0; i--)
			{
				View view = this.OpenView((View)_viewsClosed[i]);
				view.Format();
				if(i >= _maxViews)
					this.CloseView(view);
			}
		}

		/// <summary>
		/// Fecha todas as Views.
		/// </summary>
		public void CloseAllViews()
		{
			for(int i = _viewsOpen.Count - 1; i >= 0; i--)
				this.CloseView((View)_viewsOpen[i]);
			_viewsClosed.Sort(new ViewIDComparer());
		}

		/// <summary>
		/// Fecha a instancia da view informada.
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public View CloseView(View view)
		{
			View view2;
			if((view == null) || !view.IsOpen)
				return view;
			try
			{
				view.Close();
				_viewsOpen.Remove(view);
				_viewsClosed.Add(view);
				view2 = view;
			}
			catch(Exception)
			{
				throw;
			}
			return view2;
		}

		/// <summary>
		/// Cria as Views iniciais.
		/// </summary>
		/// <param name="initial">Quantidade inicial.</param>
		public void CreateInitialViews(uint initial)
		{
			_maxViews = initial;
			ulong maxLength = _mmf.MaxLength;
			int num2 = (int)(maxLength / ((ulong)_viewSize));
			if((maxLength % ((ulong)_viewSize)) > 0)
			{
				num2++;
				_mmf.SetMaxLength((ulong)(num2 * _viewSize));
			}
			for(uint i = 0; i < num2; i += 1)
			{
				View view = new View(_mmf, i, _viewSize);
				this.OpenView(view);
				this.CloseView(view);
			}
			this.ReOpenViews();
		}

		/// <summary>
		/// Extende a cesta das views.
		/// </summary>
		/// <param name="numViews">Número de views que serão incrementadas.</param>
		public void ExtendViewsBucket(int numViews)
		{
			if(numViews >= 1)
			{
				uint num = (uint)(_viewsClosed.Count + _viewsOpen.Count);
				try
				{
					for(uint i = 0; i < numViews; i += 1)
					{
						View view = new View(_mmf, num + i, _viewSize);
						this.OpenView(view);
					}
				}
				catch(Exception exception)
				{
					Colosoft.Logging.Trace.Error("MmfStorage.ExtendViewsBucketError:".GetFormatter(), exception.GetFormatter());
				}
			}
		}

		/// <summary>
		/// Recupera a view com o espaço requerido.
		/// </summary>
		/// <param name="memRequirements">Espaço de memória requirido.</param>
		/// <returns></returns>
		public View GetMatchingView(uint memRequirements)
		{
			for(int i = 0; i < _viewsOpen.Count; i++)
			{
				View view = (View)_viewsOpen[i];
				if(view.FreeSpace >= memRequirements)
					return view;
			}
			for(int j = 0; j < _viewsClosed.Count; j++)
			{
				View view2 = (View)_viewsClosed[j];
				if(view2.FreeSpace >= memRequirements)
					return this.OpenView(view2);
			}
			return null;
		}

		/// <summary>
		/// Recupera a instancia da view pelo identificador informado.
		/// </summary>
		/// <param name="viewID">Identificador da view.</param>
		/// <returns></returns>
		public View GetViewByID(uint viewID)
		{
			for(int i = 0; i < _viewsOpen.Count; i++)
			{
				View view = (View)_viewsOpen[i];
				if(view.ID == viewID)
					return view;
			}
			for(int j = 0; j < _viewsClosed.Count; j++)
			{
				View view2 = (View)_viewsClosed[j];
				if(view2.ID == viewID)
				{
					return view2;
				}
			}
			return null;
		}

		/// <summary>
		/// Abre a instancia da view informada.
		/// </summary>
		/// <param name="view">Instancia que será aberta.</param>
		/// <returns>Instancia que foi aberta.</returns>
		public View OpenView(View view)
		{
			View view3;
			if((view == null) || view.IsOpen)
				return view;
			try
			{
				bool flag = _viewsOpen.Count >= _maxViews;
				do
				{
					if(flag)
					{
						View leastUsedOpenView = this.GetLeastUsedOpenView();
						this.CloseView(leastUsedOpenView);
					}
					try
					{
						view.Open();
					}
					catch(OutOfMemoryException)
					{
					}
					flag = !view.IsOpen;
				}
				while (flag && (_viewsOpen.Count > 0));
				if(flag)
					return null;
				if(!view.HasValidHeader)
					view.Format();
				_viewsClosed.Remove(view);
				_viewsOpen.Add(view);
				view3 = view;
			}
			catch(Exception)
			{
				throw;
			}
			return view3;
		}

		/// <summary>
		/// Reabre todas as views.
		/// </summary>
		public void ReOpenViews()
		{
			_viewsClosed.Sort(new ViewIDComparer());
			for(uint i = 0; (i < _maxViews) && (i < _viewsClosed.Count); i += 1)
				this.OpenView((View)_viewsClosed[0]);
		}

		/// <summary>
		/// Faz um Shrink na cesta de views.
		/// </summary>
		/// <param name="numViews">Número de views que será reduzido.</param>
		public void ShrinkViewsBucket(int numViews)
		{
			if(numViews >= 1)
			{
				int count = _viewsClosed.Count;
				int num2 = _viewsOpen.Count;
				this.CloseAllViews();
				for(uint i = 0; i < numViews; i += 1)
				{
					_viewsClosed.RemoveAt(_viewsClosed.Count - 1);
					if(_viewsClosed.Count < 2)
						break;
				}
				_viewsClosed.TrimToSize();
				this.ReOpenViews();
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder(0x400);
			builder.Append("Views Open:\r\n");
			for(int i = 0; i < _viewsOpen.Count; i++)
				builder.Append(_viewsOpen[i]);
			builder.Append("Views Closed:\r\n");
			for(int j = 0; j < _viewsClosed.Count; j++)
				builder.Append(_viewsClosed[j]);
			return builder.ToString();
		}

		/// <summary>
		/// Implementação do comparador do identificador da view.
		/// </summary>
		private class ViewIDComparer : IComparer
		{
			/// <summary>
			/// Método de comparação.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			int IComparer.Compare(object x, object y)
			{
				return ((View)x).ID.CompareTo(((View)y).ID);
			}
		}
	}
}
