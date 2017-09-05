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

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Implementação do gerenciador indices do grupo.
	/// </summary>
	public class GroupIndexManager : IDisposable
	{
		private Hashtable _groups = new Hashtable();

		/// <summary>
		/// Lista dos dados dos grupos.
		/// </summary>
		public ArrayList DataGroupList
		{
			get
			{
				if(_groups != null)
				{
					lock (_groups.SyncRoot)
					{
						ArrayList list = new ArrayList();
						IDictionaryEnumerator enumerator = _groups.GetEnumerator();
						while (enumerator.MoveNext())
							list.Add(enumerator.Key);
						return list;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Rucupera os dados do grupo.
		/// </summary>
		/// <param name="group">Nome do grupo.</param>
		/// <returns></returns>
		private Hashtable GetGroup(string group)
		{
			if(group == null)
				return null;
			Hashtable hashtable = new Hashtable();
			lock (_groups.SyncRoot)
			{
				if(!_groups.Contains(group))
					return hashtable;
				Hashtable hashtable2 = (Hashtable)_groups[group];
				foreach (Hashtable hashtable3 in hashtable2.Values)
				{
					IDictionaryEnumerator enumerator = hashtable3.GetEnumerator();
					while (enumerator.MoveNext())
						hashtable.Add(enumerator.Key, enumerator.Value);
				}
			}
			return hashtable;
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
			this.Clear();
		}

		/// <summary>
		/// Adiciona uma chave para o grupo.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="grpInfo"></param>
		public void AddToGroup(object key, GroupInfo grpInfo)
		{
			if(grpInfo != null)
			{
				string group = grpInfo.Group;
				string subGroup = grpInfo.SubGroup;
				if(group != null)
				{
					Hashtable hashtable = null;
					lock (_groups.SyncRoot)
					{
						if(subGroup == null)
						{
							subGroup = "_DEFAULT_SUB_GRP_";
						}
						if(_groups.Contains(group))
						{
							Hashtable hashtable2 = (Hashtable)_groups[group];
							if(hashtable2.Contains(subGroup))
							{
								hashtable = (Hashtable)hashtable2[subGroup];
							}
							else
							{
								hashtable = new Hashtable();
								hashtable2[subGroup] = hashtable;
							}
						}
						else
						{
							Hashtable hashtable3 = new Hashtable();
							hashtable = new Hashtable();
							hashtable3[subGroup] = hashtable;
							_groups[group] = hashtable3;
						}
						hashtable.Add(key, string.Empty);
					}
				}
			}
		}

		/// <summary>
		/// Limpa os dados da instancia.
		/// </summary>
		public void Clear()
		{
			if(_groups != null)
			{
				lock (_groups.SyncRoot)
				{
					_groups.Clear();
				}
			}
		}

		/// <summary>
		/// Recupera os dados do grupo.
		/// </summary>
		/// <param name="group">Nome do grupo.</param>
		/// <param name="subGroup">Nome do subgrupo.</param>
		/// <returns></returns>
		public Hashtable GetGroup(string group, string subGroup)
		{
			if(group != null)
			{
				lock (_groups.SyncRoot)
				{
					if(_groups.Contains(group))
					{
						Hashtable hashtable = (Hashtable)_groups[group];
						return (Hashtable)hashtable.Clone();
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Recupera as chaves associadas com o grupo.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <returns></returns>
		public ArrayList GetGroupKeys(string group, string subGroup)
		{
			if(group == null)
				return null;
			ArrayList list = new ArrayList();
			lock (_groups.SyncRoot)
			{
				if(!_groups.Contains(group))
					return list;
				Hashtable hashtable = (Hashtable)_groups[group];
				if(subGroup != null)
				{
					if(hashtable.Contains(subGroup))
					{
						Hashtable hashtable2 = (Hashtable)hashtable[subGroup];
						list.AddRange(hashtable2.Keys);
					}
					return list;
				}
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					IDictionary dictionary = enumerator.Value as IDictionary;
					if(dictionary != null)
						list.AddRange(dictionary.Keys);
				}
			}
			return list;
		}

		/// <summary>
		/// Verifica se existe algum grupo com o nome informado.
		/// </summary>
		/// <param name="group">Nome do grupo que será verificado.</param>
		/// <returns></returns>
		public bool GroupExists(string group)
		{
			if(_groups == null)
				return false;
			return _groups.Contains(group);
		}

		/// <summary>
		/// Verifica se a chave informada existe no grupo e no subgrupo informados.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		/// <returns></returns>
		public bool KeyExists(object key, string group, string subGroup)
		{
			if(this.GroupExists(group))
			{
				Hashtable hashtable = (Hashtable)_groups[group];
				if(subGroup != null)
				{
					if(hashtable.Contains(subGroup))
					{
						Hashtable hashtable2 = (Hashtable)hashtable[subGroup];
						if(hashtable2.Contains(key))
							return true;
					}
					return false;
				}
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if(((Hashtable)enumerator.Value).Contains(key))
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Remove a entrada do grupo.
		/// </summary>
		/// <param name="key">Chave da entrada.</param>
		/// <param name="grpInfo">Informações do grupo.</param>
		public void RemoveFromGroup(object key, GroupInfo grpInfo)
		{
			if(grpInfo != null)
			{
				string group = grpInfo.Group;
				string subGroup = grpInfo.SubGroup;
				if(group != null)
				{
					Hashtable hashtable = null;
					if(subGroup == null)
					{
						subGroup = "_DEFAULT_SUB_GRP_";
					}
					lock (_groups.SyncRoot)
					{
						if(_groups.Contains(group))
						{
							Hashtable hashtable2 = (Hashtable)_groups[group];
							if(hashtable2.Contains(subGroup))
							{
								hashtable = (Hashtable)hashtable2[subGroup];
								if(hashtable.Contains(key))
								{
									hashtable.Remove(key);
									if(hashtable.Count == 0)
									{
										hashtable2.Remove(subGroup);
										if(hashtable2.Count == 0)
										{
											_groups.Remove(group);
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
