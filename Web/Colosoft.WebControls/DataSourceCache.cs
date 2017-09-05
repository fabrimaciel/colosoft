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
using System.Web;
using System.ComponentModel;
using System.Web.UI;
using System.Web.Caching;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Representa o cache para um DataSource.
	/// </summary>
	internal class DataSourceCache : System.Web.UI.IStateManager
	{
		/// <summary>
		/// Constante para uma duração infinita.
		/// </summary>
		public const int Infinite = 0;

		private bool _tracking;

		private System.Web.UI.StateBag _viewState;

		/// <summary>
		/// Duração do cache.
		/// </summary>
		public virtual int Duration
		{
			get
			{
				object obj2 = this.ViewState["Duration"];
				if(obj2 != null)
					return (int)obj2;
				return 0;
			}
			set
			{
				if(value < 0)
					throw new ArgumentOutOfRangeException("value", "The duration must be non-negative.");
				this.ViewState["Duration"] = value;
			}
		}

		/// <summary>
		/// Identifica se o cache está habilitado.
		/// </summary>
		public virtual bool Enabled
		{
			get
			{
				object obj2 = this.ViewState["Enabled"];
				return ((obj2 != null) && ((bool)obj2));
			}
			set
			{
				this.ViewState["Enabled"] = value;
			}
		}

		/// <summary>
		/// Política de expiração do cache.
		/// </summary>
		public virtual System.Web.UI.DataSourceCacheExpiry ExpirationPolicy
		{
			get
			{
				object obj2 = this.ViewState["ExpirationPolicy"];
				if(obj2 != null)
					return (System.Web.UI.DataSourceCacheExpiry)obj2;
				return System.Web.UI.DataSourceCacheExpiry.Absolute;
			}
			set
			{
				if((value < DataSourceCacheExpiry.Absolute) || (value > DataSourceCacheExpiry.Sliding))
					throw new ArgumentOutOfRangeException("Invalid DataSourceCacheExpiry.");
				this.ViewState["ExpirationPolicy"] = value;
			}
		}

		/// <summary>
		/// Chave de dependencia associada.
		/// </summary>
		[DefaultValue(""), Description("DataSourceCache_KeyDependency"), NotifyParentProperty(true)]
		public virtual string KeyDependency
		{
			get
			{
				object obj2 = this.ViewState["KeyDependency"];
				if(obj2 != null)
				{
					return (string)obj2;
				}
				return string.Empty;
			}
			set
			{
				this.ViewState["KeyDependency"] = value;
			}
		}

		/// <summary>
		/// Estado da view.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected System.Web.UI.StateBag ViewState
		{
			get
			{
				if(_viewState == null)
				{
					_viewState = new System.Web.UI.StateBag();
					if(_tracking)
						((IStateManager)_viewState).TrackViewState();
				}
				return this._viewState;
			}
		}

		/// <summary>
		/// Invalidata a entrada com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		public void Invalidate(string key)
		{
			if(string.IsNullOrEmpty(key))
				throw new ArgumentNullException("key");
			if(!this.Enabled)
				throw new InvalidOperationException("Cannot perform operation when cache is not enabled.");
			HttpRuntime.Cache.Remove(key);
		}

		/// <summary>
		/// Carrega os dados do cache.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object LoadDataFromCache(string key)
		{
			if(string.IsNullOrEmpty(key))
				throw new ArgumentNullException("key");
			if(!this.Enabled)
				throw new InvalidOperationException("Cannot perform operation when cache is not enabled.");
			return HttpRuntime.Cache.Get(key);
		}

		/// <summary>
		/// Salva os dados para o cache.
		/// </summary>
		/// <param name="key">Chave da entrada do cache.</param>
		/// <param name="data">Dados que serão salvos.</param>
		public void SaveDataToCache(string key, object data)
		{
			this.SaveDataToCache(key, data, null);
		}

		/// <summary>
		/// Salva os dados para o cache.
		/// </summary>
		/// <param name="key">Chave da entrada do cache.</param>
		/// <param name="data">Dados que serão salvos.</param>
		/// <param name="dependency">Dependencia da entrada.</param>
		public void SaveDataToCache(string key, object data, CacheDependency dependency)
		{
			this.SaveDataToCacheInternal(key, data, dependency);
		}

		/// <summary>
		/// Carrega o estado da View.
		/// </summary>
		/// <param name="savedState"></param>
		protected virtual void LoadViewState(object savedState)
		{
			if(savedState != null)
				((System.Web.UI.IStateManager)this.ViewState).LoadViewState(savedState);
		}

		/// <summary>
		/// Salva os dados para o cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <param name="dependency"></param>
		protected virtual void SaveDataToCacheInternal(string key, object data, CacheDependency dependency)
		{
			if(string.IsNullOrEmpty(key))
				throw new ArgumentNullException("key");
			if(!this.Enabled)
				throw new InvalidOperationException("Cannot perform operation when cache is not enabled.");
			DateTime noAbsoluteExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
			TimeSpan noSlidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
			switch(this.ExpirationPolicy)
			{
			case System.Web.UI.DataSourceCacheExpiry.Absolute:
				noAbsoluteExpiration = DateTime.UtcNow.AddSeconds((this.Duration == 0) ? ((double)0x7fffffff) : ((double)this.Duration));
				break;
			case System.Web.UI.DataSourceCacheExpiry.Sliding:
				noSlidingExpiration = TimeSpan.FromSeconds((double)this.Duration);
				break;
			}
			var dependencies = new System.Web.Caching.AggregateCacheDependency();
			string[] cachekeys = null;
			if(this.KeyDependency.Length > 0)
			{
				cachekeys = new string[] {
					this.KeyDependency
				};
				dependencies.Add(new CacheDependency[] {
					new CacheDependency(null, cachekeys)
				});
			}
			if(dependency != null)
			{
				dependencies.Add(new CacheDependency[] {
					dependency
				});
			}
			HttpRuntime.Cache.Insert(key, data, dependencies, noAbsoluteExpiration, noSlidingExpiration);
		}

		/// <summary>
		/// Salva o estado.
		/// </summary>
		/// <returns></returns>
		protected virtual object SaveViewState()
		{
			if(this._viewState == null)
			{
				return null;
			}
			return ((System.Web.UI.IStateManager)this._viewState).SaveViewState();
		}

		/// <summary>
		/// Segue o estado da view.
		/// </summary>
		protected void TrackViewState()
		{
			_tracking = true;
			if(_viewState != null)
				((IStateManager)_viewState).TrackViewState();
		}

		/// <summary>
		/// Carrega os dados do estado.
		/// </summary>
		/// <param name="savedState"></param>
		void IStateManager.LoadViewState(object savedState)
		{
			this.LoadViewState(savedState);
		}

		/// <summary>
		/// Salva o estado.
		/// </summary>
		/// <returns></returns>
		object IStateManager.SaveViewState()
		{
			return this.SaveViewState();
		}

		/// <summary>
		/// Segue o estado da view.
		/// </summary>
		void IStateManager.TrackViewState()
		{
			this.TrackViewState();
		}

		/// <summary>
		/// Identifica se é para sergui o estado da view.
		/// </summary>
		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return _tracking;
			}
		}
	}
}
