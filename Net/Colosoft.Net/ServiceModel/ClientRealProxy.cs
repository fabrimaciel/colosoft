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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Proxies;
using System.Text;

namespace Colosoft.ServiceModel
{
	class ClientRealProxy : RealProxy, IRemotingTypeInfo
	{
		public ClientRealProxy(Type type, IInternalContextChannel channel, bool isDuplex) : base(type)
		{
			this.channel = channel;
			this.isDuplex = isDuplex;
		}

		bool isDuplex;

		IInternalContextChannel channel;

		Dictionary<object, object[]> saved_params = new Dictionary<object, object[]>();

		System.Threading.ManualResetEvent wait = new System.Threading.ManualResetEvent(false);

		public virtual string TypeName
		{
			get;
			set;
		}

		static bool CanCastTo<T>(Type type)
		{
			return typeof(T) == type || typeof(T).GetInterfaces().Contains(type);
		}

		public virtual bool CanCastTo(Type t, object o)
		{
			if(CanCastTo<System.ServiceModel.IClientChannel>(t))
				return true;
			#if !NET_2_1
			if(isDuplex && CanCastTo<System.ServiceModel.IDuplexContextChannel>(t))
				return true;
			#endif
			return false;
		}

		public override System.Runtime.Remoting.Messaging.IMessage Invoke(System.Runtime.Remoting.Messaging.IMessage inputMessage)
		{
			try
			{
				return DoInvoke(inputMessage);
			}
			catch(System.Reflection.TargetInvocationException ex)
			{
				if(ex.InnerException != null)
					throw ex.InnerException;
				throw;
			}
		}

		System.Runtime.Remoting.Messaging.IMessage DoInvoke(System.Runtime.Remoting.Messaging.IMessage inputMessage)
		{
			var inmsg = (System.Runtime.Remoting.Messaging.IMethodCallMessage)inputMessage;
			var od = channel.Contract.Operations.FirstOrDefault(o => inmsg.MethodBase.Equals(o.SyncMethod) || inmsg.MethodBase.Equals(o.BeginMethod) || inmsg.MethodBase.Equals(o.EndMethod));
			if(od == null)
			{
				var ret = inmsg.MethodBase.Invoke(channel, inmsg.InArgs);
				return new System.Runtime.Remoting.Messaging.ReturnMessage(ret, null, 0, null, inmsg);
			}
			else
			{
				object[] pl;
				System.Reflection.MethodBase method = null;
				List<object> outArgs = null;
				object ret;
				if(inmsg.MethodBase.Equals(od.SyncMethod))
				{
					pl = new object[inmsg.MethodBase.GetParameters().Length];
					Array.Copy(inmsg.Args, pl, inmsg.ArgCount);
					ret = channel.Process(inmsg.MethodBase, od.Name, pl, System.ServiceModel.OperationContext.Current);
					method = od.SyncMethod;
				}
				else if(inmsg.MethodBase.Equals(od.BeginMethod))
				{
					pl = new object[inmsg.ArgCount - 2];
					Array.Copy(inmsg.Args, 0, pl, 0, pl.Length);
					ret = channel.BeginProcess(inmsg.MethodBase, od.Name, pl, (AsyncCallback)inmsg.Args[inmsg.ArgCount - 2], inmsg.Args[inmsg.ArgCount - 1]);
					saved_params[ret] = pl;
					wait.Set();
				}
				else
				{
					var result = (IAsyncResult)inmsg.InArgs[0];
					wait.WaitOne();
					pl = saved_params[result];
					wait.Reset();
					saved_params.Remove(result);
					ret = channel.EndProcess(inmsg.MethodBase, od.Name, pl, result);
					method = od.BeginMethod;
				}
				if(method != null && method.GetParameters().Any(pi => pi.IsOut || pi.ParameterType.IsByRef))
					return new System.Runtime.Remoting.Messaging.ReturnMessage(ret, pl, pl.Length, null, inmsg);
				else
					return new System.Runtime.Remoting.Messaging.ReturnMessage(ret, outArgs != null ? outArgs.ToArray() : null, outArgs != null ? outArgs.Count : 0, null, inmsg);
			}
		}
	}
}
