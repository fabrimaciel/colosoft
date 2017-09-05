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

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa um mapa de informações de tipos.
	/// </summary>
	public class TypeInfoMap
	{
		private Dictionary<int, Dictionary<string, object>> _map;

		private StringBuilder _protocolString;

		private int _typeHandle;

		private Dictionary<string, int> _typeToHandleMap;

		private object _objLock = new object();

		/// <summary>
		/// Evento acionado quando quando um novo manipulador for adicionado.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
		public event Action<int> HandleAdded;

		/// <summary>
		/// Cria uma instancia já com as classe indexadas.
		/// </summary>
		/// <param name="indexClasses"></param>
		public TypeInfoMap(Hashtable indexClasses)
		{
			this.CreateMap(indexClasses);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="protocolString"></param>
		public TypeInfoMap(string protocolString)
		{
			this.CreateMap(protocolString);
		}

		/// <summary>
		/// Cria um mapa.
		/// </summary>
		/// <param name="indexClasses">Nomes das classes indexadas.</param>
		private void CreateMap(Hashtable indexClasses)
		{
			_map = new Dictionary<int, Dictionary<string, object>>();
			_typeToHandleMap = new Dictionary<string, int>();
			IDictionaryEnumerator enumerator = indexClasses.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Hashtable hashtable = enumerator.Value as Hashtable;
				if(hashtable != null)
				{
					var typeMap = new Dictionary<string, object>();
					var attributes = new Dictionary<string, string>();
					var sequence = new List<string>();
					IDictionaryEnumerator enumerator2 = hashtable.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						Hashtable hashtable4 = enumerator2.Value as Hashtable;
						if(hashtable4 != null)
						{
							IDictionaryEnumerator enumerator3 = hashtable4.GetEnumerator();
							while (enumerator3.MoveNext())
							{
								Hashtable hashtable5 = enumerator3.Value as Hashtable;
								if(hashtable5 != null)
								{
									sequence.Add(hashtable5["id"] as string);
									attributes.Add(hashtable5["id"] as string, hashtable5["data-type"] as string);
								}
							}
						}
					}
					typeMap.Add("name", hashtable["name"] as string);
					typeMap.Add("attributes", attributes);
					typeMap.Add("sequence", sequence);
					_map.Add(_typeHandle, typeMap);
					_typeToHandleMap.Add(typeMap["name"] as string, _typeHandle);
					_typeHandle++;
				}
			}
		}

		/// <summary>
		/// Cria um mapa com base nos dados contidos no texto formatado informado.
		/// </summary>
		/// <param name="value"></param>
		private void CreateMap(string value)
		{
			int startIndex = 0;
			int index = value.IndexOf('"', startIndex + 1);
			int capacity = Convert.ToInt32(value.Substring(startIndex, index - startIndex));
			_map = new Dictionary<int, Dictionary<string, object>>(capacity);
			_typeToHandleMap = new Dictionary<string, int>(capacity);
			for(int i = 0; i < capacity; i++)
			{
				startIndex = index + 1;
				index = value.IndexOf('"', index + 1);
				int key = Convert.ToInt32(value.Substring(startIndex, index - startIndex));
				startIndex = index + 1;
				index = value.IndexOf('"', index + 1);
				string str = value.Substring(startIndex, index - startIndex);
				var hashtable = new Dictionary<string, object>();
				hashtable.Add("name", str);
				startIndex = index + 1;
				index = value.IndexOf('"', index + 1);
				int num6 = Convert.ToInt32(value.Substring(startIndex, index - startIndex));
				var list = new List<string>(num6);
				for(int j = 0; j < num6; j++)
				{
					startIndex = index + 1;
					index = value.IndexOf('"', index + 1);
					string str2 = value.Substring(startIndex, index - startIndex);
					list.Add(str2);
				}
				hashtable.Add("sequence", list);
				_map.Add(key, hashtable);
				_typeToHandleMap.Add(hashtable["name"] as string, key);
			}
		}

		/// <summary>
		/// Recupera o mapa para o tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private Dictionary<string, object> GetTypeMap(Type type)
		{
			var cacheSetted = type.GetCustomAttributes(typeof(CacheAttribute), true).Length > 0;
			var result = new Dictionary<string, object>();
			result.Add("name", type.FullName);
			var members = type.GetMembers(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
			var attributes = new Dictionary<string, string>();
			var sequence = new List<string>();
			foreach (var m in members)
			{
				if((m is System.Reflection.PropertyInfo || m is System.Reflection.FieldInfo) && (!cacheSetted || m.GetCustomAttributes(typeof(CacheIndexAttribute), true).Length > 0))
				{
					attributes.Add(m.Name, (m is System.Reflection.PropertyInfo ? ((System.Reflection.PropertyInfo)m).PropertyType : ((System.Reflection.FieldInfo)m).FieldType).FullName);
					sequence.Add(m.Name);
				}
			}
			result.Add("attributes", attributes);
			result.Add("sequence", sequence);
			return result;
		}

		/// <summary>
		/// Recupera o mapa para o nome do tipo informado.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private Dictionary<string, object> GetTypeMap(Colosoft.Reflection.TypeName typeName)
		{
			var result = new Dictionary<string, object>();
			result.Add("name", typeName.FullName);
			var attributes = new Dictionary<string, string>();
			var sequence = new List<string>();
			result.Add("attributes", attributes);
			result.Add("sequence", sequence);
			return result;
		}

		/// <summary>
		/// Método acionado quando um novo manipulador for adicionado.
		/// </summary>
		/// <param name="handleId"></param>
		protected void OnHandleAdded(int handleId)
		{
			if(HandleAdded != null)
				HandleAdded(handleId);
		}

		/// <summary>
		/// Recupera a lista os nomes dos atributos
		/// </summary>
		/// <param name="handleId"></param>
		/// <returns></returns>
		public List<string> GetAttribList(int handleId)
		{
			return (List<string>)_map[handleId]["sequence"];
		}

		/// <summary>
		/// Recupera os atributos associados com o identificador informado.
		/// </summary>
		/// <param name="handle"></param>
		/// <returns></returns>
		public Dictionary<string, string> GetAttributes(int handle)
		{
			return (Dictionary<string, string>)_map[handle]["attributes"];
		}

		/// <summary>
		/// Recupera os atributos associados com o tipo representado pelo nome informado.
		/// </summary>
		/// <param name="typeName">Nome do tipo que será pesquisado.</param>
		/// <returns></returns>
		public Dictionary<string, string> GetAttributes(string typeName)
		{
			int handleId = this.GetHandleId(typeName);
			if((handleId != -1) && _map.ContainsKey(handleId))
				return GetAttributes(handleId);
			return null;
		}

		/// <summary>
		/// Recupera o identificador do manipulador do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public int GetHandleId(Type type)
		{
			type.Require("type").NotNull();
			var typeFullname = type.FullName.Replace("+", ".");
			int handleId = 0;
			lock (_objLock)
				if(_typeToHandleMap.TryGetValue(typeFullname, out handleId))
					return handleId;
			if(type != null)
			{
				var typeMap = GetTypeMap(type);
				if(typeMap != null)
				{
					handleId = _typeHandle++;
					lock (_objLock)
					{
						_typeToHandleMap.Add(typeFullname, handleId);
						_map.Add(handleId, typeMap);
					}
					OnHandleAdded(handleId);
					return handleId;
				}
			}
			lock (_objLock)
				_typeToHandleMap.Add(typeFullname, -1);
			return -1;
		}

		/// <summary>
		/// Recupera o identificador do manipulador do tipo informado.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public int GetHandleId(Colosoft.Reflection.TypeName typeName)
		{
			string typeFullname = typeName.FullName;
			int handleId = 0;
			lock (_objLock)
				if(_typeToHandleMap.TryGetValue(typeFullname, out handleId))
					return handleId;
			if(typeName != null)
			{
				var typeMap = GetTypeMap(typeName);
				if(typeMap != null)
				{
					handleId = _typeHandle++;
					lock (_objLock)
					{
						_typeToHandleMap.Add(typeFullname, handleId);
						_map.Add(handleId, typeMap);
					}
					OnHandleAdded(handleId);
					return handleId;
				}
			}
			lock (_objLock)
				_typeToHandleMap.Add(typeFullname, -1);
			return -1;
		}

		/// <summary>
		/// Recupera o identificador do manipulador do tipo informado.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		protected int GetHandleId(string typeName)
		{
			if(_typeToHandleMap.ContainsKey(typeName))
				return (int)_typeToHandleMap[typeName];
			return -1;
		}

		/// <summary>
		/// Recupera o nome do tipo pelo identificador informado.
		/// </summary>
		/// <param name="handle"></param>
		/// <returns></returns>
		public string GetTypeName(int handle)
		{
			return (string)_map[handle]["name"];
		}

		/// <summary>
		/// Recupera a string do protocolo.
		/// </summary>
		/// <returns></returns>
		public string ToProtocolString()
		{
			_protocolString = new StringBuilder();
			_protocolString.Append(_map.Count).Append("\"");
			var enumerator = _map.GetEnumerator();
			while (enumerator.MoveNext())
			{
				_protocolString.Append(enumerator.Current.Key).Append("\"");
				var hashtable = enumerator.Current.Value;
				_protocolString.Append(hashtable["name"] as string).Append("\"");
				var list = (List<string>)hashtable["sequence"];
				_protocolString.Append(list.Count).Append("\"");
				for(int i = 0; i < list.Count; i++)
					_protocolString.Append(list[i] as string).Append("\"");
			}
			return _protocolString.ToString();
		}
	}
}
