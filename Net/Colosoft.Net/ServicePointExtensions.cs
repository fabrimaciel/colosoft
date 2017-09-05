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

namespace Colosoft.Net.Mono
{
	static class ServicePointExtensions
	{
		private static System.Reflection.MethodInfo _getConnectionGroup;

		private static System.Reflection.MethodInfo _getConnection;

		private static System.Reflection.FieldInfo _currentConnections;

		private static System.Reflection.FieldInfo _idleTimer;

		private static System.Reflection.MethodInfo _idleTimerCallback;

		private static System.Reflection.FieldInfo _maxIdleTime;

		private static object GetConnectionGroup(this System.Net.ServicePoint servicePoint, string name)
		{
			return (_getConnectionGroup ?? (_getConnectionGroup = typeof(System.Net.ServicePoint).GetMethod("GetConnectionGroup", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).Invoke(servicePoint, new object[] {
				name
			});
		}

		private static object GetConnection(this object group, System.Net.HttpWebRequest request, out bool created)
		{
			created = true;
			return (_getConnection ?? (_getConnection = group.GetType().GetMethod("GetConnection", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))).Invoke(group, new object[] {
				request,
				created
			});
		}

		/// <summary>
		/// Recupera o atual número de conexões.
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <returns></returns>
		private static int GetCurrentConnections(this System.Net.ServicePoint servicePoint)
		{
			return (int)(_currentConnections ?? (_currentConnections = typeof(System.Net.ServicePoint).GetField("currentConnections", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))).GetValue(servicePoint);
		}

		/// <summary>
		/// Define o atual número de conexões.
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <param name="value"></param>
		private static void SetCurrentConnections(this System.Net.ServicePoint servicePoint, int value)
		{
			(_currentConnections ?? (_currentConnections = typeof(System.Net.ServicePoint).GetField("currentConnections", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))).SetValue(servicePoint, value);
		}

		/// <summary>
		/// Recupera o campo idleTimer.
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <returns></returns>
		private static System.Threading.Timer GetIdleTimer(this System.Net.ServicePoint servicePoint)
		{
			return (System.Threading.Timer)(_idleTimer ?? (_idleTimer = typeof(System.Net.ServicePoint).GetField("idleTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))).GetValue(servicePoint);
		}

		private static void SetIdleTimer(this System.Net.ServicePoint servicePoint, System.Threading.Timer value)
		{
			(_idleTimer ?? (_idleTimer = typeof(System.Net.ServicePoint).GetField("idleTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))).SetValue(servicePoint, value);
		}

		/// <summary>
		/// Recupera o callback usado para reciclar as conexões.
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <returns></returns>
		private static System.Threading.TimerCallback GetIdleTimerCallback(this System.Net.ServicePoint servicePoint)
		{
			return new System.Threading.TimerCallback(obj =>  {
				try
				{
					(_idleTimerCallback ?? (_idleTimerCallback = typeof(System.Net.ServicePoint).GetMethod("IdleTimerCallback", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))).Invoke(servicePoint, new object[] {
						obj
					});
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
			});
		}

		/// <summary>
		/// Recupera o tempo máximo para varredura.
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <returns></returns>
		private static int GetMaxIdleTime(this System.Net.ServicePoint servicePoint)
		{
			return (int)(_maxIdleTime ?? (_maxIdleTime = typeof(System.Net.ServicePoint).GetField("maxIdleTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))).GetValue(servicePoint);
		}

		/// <summary>
		/// Recupera os grupos de conexão do ServicePoint.
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <returns></returns>
		private static System.Collections.IDictionary GetGroups(this System.Net.ServicePoint servicePoint)
		{
			return (System.Collections.IDictionary)typeof(System.Net.ServicePoint).GetField("groups", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(servicePoint);
		}

		/// <summary>
		/// Recupera os grupos de conexão do ServicePoint.
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <param name="groups"></param>
		/// <returns></returns>
		private static void SetGroups(this System.Net.ServicePoint servicePoint, System.Collections.IDictionary groups)
		{
			typeof(System.Net.ServicePoint).GetField("groups", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(servicePoint, groups);
		}

		/// <summary>
		/// Recupera o tempo máximo para varredura.
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static void SetIdleSince(this System.Net.ServicePoint servicePoint, DateTime value)
		{
			typeof(System.Net.ServicePoint).GetField("idleSince", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(servicePoint, value);
		}

		/// <summary>
		/// Remove o grupo de conexão.
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <param name="group"></param>
		private static void RemoveConnectionGroup(this System.Net.ServicePoint servicePoint, object group)
		{
			typeof(System.Net.ServicePoint).GetMethod("RemoveConnectionGroup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(servicePoint, new object[] {
				group
			});
		}

		/// <summary>
		/// Verifica se possui grupos disponível para reciclar.
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <param name="outIdleSince"></param>
		/// <returns></returns>
		private static bool CheckAvailableForRecycling(this System.Net.ServicePoint servicePoint, out DateTime outIdleSince)
		{
			var groups = servicePoint.GetGroups();
			TimeSpan span;
			outIdleSince = DateTime.MinValue;
			System.Collections.ArrayList list = null;
			System.Collections.ArrayList list2 = null;
			object obj2 = servicePoint;
			lock (obj2)
			{
				if((groups == null) || (groups.Count == 0))
				{
					servicePoint.SetIdleSince(DateTime.MinValue);
					return true;
				}
				span = TimeSpan.FromMilliseconds((double)servicePoint.GetMaxIdleTime());
				list = new System.Collections.ArrayList(groups.Values);
			}
			foreach (var group in list)
			{
				if(group.TryRecycle(span, ref outIdleSince))
				{
					if(list2 == null)
						list2 = new System.Collections.ArrayList();
					list2.Add(group);
				}
			}
			object obj3 = servicePoint;
			lock (obj3)
			{
				servicePoint.SetIdleSince(outIdleSince);
				if((list2 != null) && (groups != null))
				{
					foreach (var group2 in list2)
					{
						if(groups.Contains(WebConnectionGroupExtensions.GetName(group2)))
							servicePoint.RemoveConnectionGroup(group2);
					}
				}
				if((groups != null) && (groups.Count == 0))
				{
					groups = null;
					servicePoint.SetGroups(null);
				}
				if(groups == null)
				{
					if(servicePoint.GetIdleTimer() != null)
					{
						servicePoint.GetIdleTimer().Dispose();
						servicePoint.SetIdleTimer(null);
					}
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Envia a requisição.
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <param name="request"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>
		public static EventHandler SendRequest(this System.Net.ServicePoint servicePoint, System.Net.HttpWebRequest request, string groupName)
		{
			object connection;
			object obj2 = servicePoint;
			lock (obj2)
			{
				bool flag2;
				connection = servicePoint.GetConnectionGroup(groupName).GetConnection(request, out flag2);
				if(flag2)
				{
					servicePoint.SetCurrentConnections(servicePoint.GetCurrentConnections() + 1);
					if(servicePoint.GetIdleTimer() == null)
					{
						var maxIdleTime = servicePoint.GetMaxIdleTime();
						servicePoint.SetIdleTimer(new System.Threading.Timer(new System.Threading.TimerCallback(servicePoint.GetIdleTimerCallback()), null, maxIdleTime, maxIdleTime));
					}
				}
			}
			return WebConnectionExtensions.SendRequest(connection, request);
		}
	}
}
