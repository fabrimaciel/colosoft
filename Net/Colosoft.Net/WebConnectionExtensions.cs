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
	static class WebConnectionExtensions
	{
		private static System.Reflection.FieldInfo _state;

		private static System.Reflection.MethodInfo _trySetBusy;

		private static System.Reflection.FieldInfo _status;

		private static System.Reflection.FieldInfo _initConn;

		private static System.Reflection.FieldInfo _queue;

		private static System.Reflection.FieldInfo _abortHandler;

		private static object GetState(this object connection)
		{
			return (_state ?? (_state = connection.GetType().GetField("state", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))).GetValue(connection);
		}

		private static bool TrySetBusy(this object state)
		{
			return (bool)(_trySetBusy ?? (_trySetBusy = state.GetType().GetMethod("TrySetBusy", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))).Invoke(state, null);
		}

		private static void SetStatus(this object connection, System.Net.WebExceptionStatus status)
		{
			(_status ?? (_status = connection.GetType().GetField("status", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))).SetValue(connection, status);
		}

		private static System.Threading.WaitCallback GetInitConn(this object connection)
		{
			return (System.Threading.WaitCallback)(_initConn ?? (_initConn = connection.GetType().GetField("initConn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))).GetValue(connection);
		}

		private static System.Collections.Queue GetQueue(this object connection)
		{
			return (System.Collections.Queue)(_queue ?? (_queue = connection.GetType().GetField("queue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))).GetValue(connection);
		}

		private static EventHandler GetAbortHandler(this object connection)
		{
			return (EventHandler)(_abortHandler ?? (_abortHandler = connection.GetType().GetField("abortHandler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))).GetValue(connection);
		}

		private static bool _isFirstSendRequest = true;

		/// <summary>
		/// Envia a requisição.
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		public static EventHandler SendRequest(object connection, System.Net.HttpWebRequest request)
		{
			if(request.IsAborted())
				return null;
			object obj2 = connection;
			lock (obj2)
			{
				if(connection.GetState().TrySetBusy())
				{
					connection.SetStatus(System.Net.WebExceptionStatus.Success);
					var initConn = connection.GetInitConn();
					System.Threading.Thread workerThread = null;
					workerThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(state =>  {
						System.Threading.Thread.CurrentThread.Name = _isFirstSendRequest ? "WCF FirstSend" : "WCF SendRequest";
						_isFirstSendRequest = false;
						try
						{
							initConn(state);
						}
						catch(Exception ex)
						{
							try
							{
								Console.WriteLine("WCF SendRequest ERROR " + ex.ToString());
							}
							catch
							{
							}
						}
					}));
					workerThread.Start(request);
					new System.Threading.Timer(o =>  {
						if(workerThread != null && workerThread.IsAlive)
							workerThread.Abort();
					}, null, request.ReadWriteTimeout + 1000, System.Threading.Timeout.Infinite);
				}
				else
				{
					object queue = connection.GetQueue();
					lock (queue)
					{
						connection.GetQueue().Enqueue(request);
					}
				}
			}
			return connection.GetAbortHandler();
		}
	}
}
