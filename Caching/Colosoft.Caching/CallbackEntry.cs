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
using Colosoft.Serialization;
using Colosoft.Serialization.IO;

namespace Colosoft.Caching
{
	/// <summary>
	/// Classe usada para monitora o acesso aos dados da entrada do cache.
	/// </summary>
	[Serializable]
	public class CallbackEntry : ICompactSerializable, ICloneable
	{
		private BitSet _flag;

		private ArrayList _itemRemovedListener;

		private ArrayList _itemUpdateListener;

		private object _onAsyncOperationCompleteCallback;

		private object _onWriteBehindOperationCompletedCallback;

		private object _value;

		/// <summary>
		/// Instancia do callback da 
		/// </summary>
		public object AsyncOperationCompleteCallback
		{
			get
			{
				return _onAsyncOperationCompleteCallback;
			}
			set
			{
				_onAsyncOperationCompleteCallback = value;
			}
		}

		/// <summary>
		/// Flags da entrada.
		/// </summary>
		public BitSet Flag
		{
			get
			{
				return _flag;
			}
			set
			{
				_flag = value;
			}
		}

		/// <summary>
		/// Callbacks.
		/// </summary>
		public ArrayList ItemRemoveCallbackListener
		{
			get
			{
				return _itemRemovedListener;
			}
		}

		/// <summary>
		/// Lista dos callbacks de atualização.
		/// </summary>
		public ArrayList ItemUpdateCallbackListener
		{
			get
			{
				return _itemUpdateListener;
			}
		}

		/// <summary>
		/// Dados do usuário.
		/// </summary>
		public Array UserData
		{
			get
			{
				Array data = null;
				if(_value != null)
					data = ((UserBinaryObject)_value).Data;
				return data;
			}
		}

		/// <summary>
		/// Valor da instancia.
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public object WriteBehindOperationCompletedCallback
		{
			get
			{
				return _onWriteBehindOperationCompletedCallback;
			}
			set
			{
				_onWriteBehindOperationCompletedCallback = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CallbackEntry()
		{
			_itemRemovedListener = ArrayList.Synchronized(new ArrayList(2));
			_itemUpdateListener = ArrayList.Synchronized(new ArrayList(2));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">Valor associado.</param>
		/// <param name="onCacheItemRemovedCallback">Callback de quando o item é removido.</param>
		/// <param name="onCacheItemUpdateCallback">Callback de quando o item é atualizado.</param>
		/// <param name="onAsyncOperationCompleteCallback">Callback de quando a operação é completada.</param>
		/// <param name="onWriteBehindOperationCompletedCallback">Callback de quando a operação de escrita é completada.</param>
		public CallbackEntry(object value, CallbackInfo onCacheItemRemovedCallback, CallbackInfo onCacheItemUpdateCallback, AsyncCallbackInfo onAsyncOperationCompleteCallback, AsyncCallbackInfo onWriteBehindOperationCompletedCallback)
		{
			_itemRemovedListener = ArrayList.Synchronized(new ArrayList(2));
			_itemUpdateListener = ArrayList.Synchronized(new ArrayList(2));
			_value = value;
			if(onCacheItemRemovedCallback != null)
				_itemRemovedListener.Add(onCacheItemRemovedCallback);
			if(onCacheItemUpdateCallback != null)
				_itemUpdateListener.Add(onCacheItemUpdateCallback);
			_onAsyncOperationCompleteCallback = onAsyncOperationCompleteCallback;
			_onWriteBehindOperationCompletedCallback = onWriteBehindOperationCompletedCallback;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clientId">Identificador do cliente.</param>
		/// <param name="requestId">Identificador da requisição.</param>
		/// <param name="value">Valor associado.</param>
		/// <param name="onCacheItemRemovedCallback">Identificador do callback de quando o item é removido.</param>
		/// <param name="onCacheItemUpdateCallback">Identificador do callback de quando o item é atualizado.</param>
		/// <param name="onAsyncOperationCompleteCallback">Identificador do callback de quando a operação é completada.</param>
		public CallbackEntry(string clientId, int requestId, object value, short onCacheItemRemovedCallback, short onCacheItemUpdateCallback, short onAsyncOperationCompleteCallback)
		{
			_itemRemovedListener = ArrayList.Synchronized(new ArrayList(2));
			_itemUpdateListener = ArrayList.Synchronized(new ArrayList(2));
			_value = value;
			if(onCacheItemUpdateCallback != -1)
				_itemUpdateListener.Add(new CallbackInfo(clientId, onCacheItemUpdateCallback));
			if(onCacheItemRemovedCallback != -1)
				_itemRemovedListener.Add(new CallbackInfo(clientId, onCacheItemRemovedCallback));
			if(onAsyncOperationCompleteCallback != -1)
				_onAsyncOperationCompleteCallback = new AsyncCallbackInfo(requestId, clientId, onAsyncOperationCompleteCallback);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clientid"></param>
		/// <param name="reqId"></param>
		/// <param name="value"></param>
		/// <param name="onCacheItemRemovedCallback"></param>
		/// <param name="onCacheItemUpdateCallback"></param>
		/// <param name="onAsyncOperationCompleteCallback"></param>
		/// <param name="onWriteBehindOperationCompletedCallback"></param>
		/// <param name="Flag"></param>
		public CallbackEntry(string clientid, int reqId, object value, short onCacheItemRemovedCallback, short onCacheItemUpdateCallback, short onAsyncOperationCompleteCallback, short onWriteBehindOperationCompletedCallback, BitSet Flag)
		{
			_itemRemovedListener = ArrayList.Synchronized(new ArrayList(2));
			_itemUpdateListener = ArrayList.Synchronized(new ArrayList(2));
			_value = value;
			_flag = Flag;
			if(onCacheItemUpdateCallback != -1)
				_itemUpdateListener.Add(new CallbackInfo(clientid, onCacheItemUpdateCallback));
			if(onCacheItemRemovedCallback != -1)
				_itemRemovedListener.Add(new CallbackInfo(clientid, onCacheItemRemovedCallback));
			if(onAsyncOperationCompleteCallback != -1)
				_onAsyncOperationCompleteCallback = new AsyncCallbackInfo(reqId, clientid, onAsyncOperationCompleteCallback);
			if(onWriteBehindOperationCompletedCallback != -1)
				_onWriteBehindOperationCompletedCallback = new AsyncCallbackInfo(reqId, clientid, onWriteBehindOperationCompletedCallback);
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			CallbackEntry entry = new CallbackEntry();
			entry._flag = _flag;
			entry._value = _value;
			entry._itemRemovedListener = _itemRemovedListener.Clone() as ArrayList;
			entry._itemUpdateListener = _itemUpdateListener.Clone() as ArrayList;
			entry._onAsyncOperationCompleteCallback = _onAsyncOperationCompleteCallback;
			entry._onWriteBehindOperationCompletedCallback = _onWriteBehindOperationCompletedCallback;
			return entry;
		}

		/// <summary>
		/// Adiciona para o callback de remoção.
		/// </summary>
		/// <param name="cbInfo"></param>
		public void AddItemRemoveCallback(CallbackInfo cbInfo)
		{
			if(_itemRemovedListener != null && !_itemRemovedListener.Contains(cbInfo))
				_itemRemovedListener.Add(cbInfo);
		}

		/// <summary>
		/// Adiciona para o callback de remoção.
		/// </summary>
		/// <param name="clientid"></param>
		/// <param name="callback"></param>
		public void AddItemRemoveCallback(string clientid, object callback)
		{
			this.AddItemRemoveCallback(new CallbackInfo(clientid, callback));
		}

		/// <summary>
		/// Adiciona para o callback de atualização.
		/// </summary>
		/// <param name="cbInfo"></param>
		public void AddItemUpdateCallback(CallbackInfo cbInfo)
		{
			if((_itemUpdateListener != null) && !_itemUpdateListener.Contains(cbInfo))
				_itemUpdateListener.Add(cbInfo);
		}

		/// <summary>
		/// Adiciona para o callback de atualização.
		/// </summary>
		/// <param name="clientid"></param>
		/// <param name="callback"></param>
		public void AddItemUpdateCallback(string clientid, object callback)
		{
			this.AddItemUpdateCallback(new CallbackInfo(clientid, callback));
		}

		/// <summary>
		/// Remove o item do callback de remoção.
		/// </summary>
		/// <param name="cbInfo"></param>
		public void RemoveItemRemoveCallback(CallbackInfo cbInfo)
		{
			if((_itemRemovedListener != null) && _itemRemovedListener.Contains(cbInfo))
				_itemRemovedListener.Remove(cbInfo);
		}

		/// <summary>
		/// Remove o item do callback de atualização.
		/// </summary>
		/// <param name="cbInfo"></param>
		public void RemoveItemUpdateCallback(CallbackInfo cbInfo)
		{
			if((_itemUpdateListener != null) && _itemUpdateListener.Contains(cbInfo))
				_itemUpdateListener.Remove(cbInfo);
		}

		/// <summary>
		/// Desserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			_value = reader.ReadObject();
			_flag = reader.ReadObject() as BitSet;
			ArrayList list = reader.ReadObject() as ArrayList;
			if(list != null)
				_itemUpdateListener = ArrayList.Synchronized(list);
			list = reader.ReadObject() as ArrayList;
			if(list != null)
				_itemRemovedListener = ArrayList.Synchronized(list);
			_onAsyncOperationCompleteCallback = reader.ReadObject();
			_onWriteBehindOperationCompletedCallback = reader.ReadObject();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			writer.WriteObject(_value);
			writer.WriteObject(_flag);
			lock (_itemUpdateListener.SyncRoot)
				writer.WriteObject(_itemUpdateListener);
			lock (_itemRemovedListener.SyncRoot)
				writer.WriteObject(_itemRemovedListener);
			writer.WriteObject(_onAsyncOperationCompleteCallback);
			writer.WriteObject(_onWriteBehindOperationCompletedCallback);
		}
	}
}
