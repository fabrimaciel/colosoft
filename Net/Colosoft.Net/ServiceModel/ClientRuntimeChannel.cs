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
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;

namespace Colosoft.ServiceModel
{
	#if DISABLE_REAL_PROXY
		// FIXME: This is a quick workaround for bug 571907
	public
#endif
	interface IInternalContextChannel
	{
		System.ServiceModel.Description.ContractDescription Contract
		{
			get;
		}

		object Process(System.Reflection.MethodBase method, string operationName, object[] parameters, System.ServiceModel.OperationContext context);

		IAsyncResult BeginProcess(System.Reflection.MethodBase method, string operationName, object[] parameters, AsyncCallback callback, object asyncState);

		object EndProcess(System.Reflection.MethodBase method, string operationName, object[] parameters, IAsyncResult result);
	}
	#if DISABLE_REAL_PROXY
		// FIXME: This is a quick workaround for bug 571907
	public
#endif
	class ClientRuntimeChannel : System.ServiceModel.Channels.CommunicationObject, System.ServiceModel.IClientChannel, IInternalContextChannel
	{
		System.ServiceModel.Dispatcher.ClientRuntime runtime;

		System.ServiceModel.EndpointAddress remote_address;

		System.ServiceModel.Description.ContractDescription contract;

		MessageVersion message_version;

		TimeSpan default_open_timeout, default_close_timeout;

		IChannel channel;

		IChannelFactory factory;

		TimeSpan? operation_timeout = null;

		ChannelFactory channel_factory;

		readonly ProcessDelegate _processDelegate;

		delegate object ProcessDelegate (System.Reflection.MethodBase method, string operationName, bool isAsync, ref object[] parameters, System.ServiceModel.OperationContext context);

		readonly RequestDelegate requestDelegate;

		delegate Message RequestDelegate (Message msg, TimeSpan timeout);

		readonly SendDelegate sendDelegate;

		delegate void SendDelegate (Message msg, TimeSpan timeout);

		private static System.ServiceModel.Dispatcher.ClientRuntime CreateClientRuntime(ServiceEndpoint endpoint, object callbackDispatchRuntime)
		{
			return (System.ServiceModel.Dispatcher.ClientRuntime)typeof(ServiceEndpoint).GetMethod("CreateClientRuntime", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(endpoint, new object[] {
				callbackDispatchRuntime
			});
		}

		public ClientRuntimeChannel(ServiceEndpoint endpoint, ChannelFactory channelFactory, System.ServiceModel.EndpointAddress remoteAddress, Uri via) : this(CreateClientRuntime(endpoint, null), endpoint.Contract, channelFactory.DefaultOpenTimeout2, channelFactory.DefaultCloseTimeout2, null, channelFactory.OpenedChannelFactory, endpoint.Binding.MessageVersion, remoteAddress, via)
		{
			channel_factory = channelFactory;
		}

		public ClientRuntimeChannel(System.ServiceModel.Dispatcher.ClientRuntime runtime, ContractDescription contract, TimeSpan openTimeout, TimeSpan closeTimeout, IChannel contextChannel, IChannelFactory factory, MessageVersion messageVersion, System.ServiceModel.EndpointAddress remoteAddress, Uri via)
		{
			if(runtime == null)
				throw new ArgumentNullException("runtime");
			if(messageVersion == null)
				throw new ArgumentNullException("messageVersion");
			this.runtime = runtime;
			this.remote_address = remoteAddress;
			if(runtime.Via == null)
				runtime.Via = via ?? (remote_address != null ? remote_address.Uri : null);
			this.contract = contract;
			this.message_version = messageVersion;
			default_open_timeout = openTimeout;
			default_close_timeout = closeTimeout;
			_processDelegate = new ProcessDelegate(Process);
			requestDelegate = new RequestDelegate(Request);
			sendDelegate = new SendDelegate(Send);
			AllowInitializationUI = true;
			if(contextChannel != null)
				channel = contextChannel;
			else
			{
				var method = factory.GetType().GetMethod("CreateChannel", new Type[] {
					typeof(System.ServiceModel.EndpointAddress),
					typeof(Uri)
				});
				try
				{
					channel = (IChannel)method.Invoke(factory, new object[] {
						remote_address,
						Via
					});
					this.factory = factory;
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					if(ex.InnerException != null)
						throw ex.InnerException;
					else
						throw;
				}
			}
		}

		public System.ServiceModel.Description.ContractDescription Contract
		{
			get
			{
				return contract;
			}
		}

		public System.ServiceModel.Dispatcher.ClientRuntime Runtime
		{
			get
			{
				return runtime;
			}
		}

		IRequestChannel RequestChannel
		{
			get
			{
				return channel as IRequestChannel;
			}
		}

		IOutputChannel OutputChannel
		{
			get
			{
				return channel as IOutputChannel;
			}
		}

		internal IDuplexChannel DuplexChannel
		{
			get
			{
				return channel as IDuplexChannel;
			}
		}

		bool did_interactive_initialization;

		public bool AllowInitializationUI
		{
			get;
			set;
		}

		public bool DidInteractiveInitialization
		{
			get
			{
				return did_interactive_initialization;
			}
		}

		public Uri Via
		{
			get
			{
				return runtime.Via;
			}
		}

		class DelegatingWaitHandle : System.Threading.WaitHandle
		{
			public DelegatingWaitHandle(IAsyncResult[] results)
			{
				this.results = results;
			}

			IAsyncResult[] results;

			protected override void Dispose(bool disposing)
			{
				if(disposing)
					foreach (var r in results)
						r.AsyncWaitHandle.Close();
			}

			public override bool WaitOne()
			{
				foreach (var r in results)
					r.AsyncWaitHandle.WaitOne();
				return true;
			}

			public override bool WaitOne(int millisecondsTimeout)
			{
				return System.Threading.WaitHandle.WaitAll(ResultWaitHandles, millisecondsTimeout);
			}

			WaitHandle[] ResultWaitHandles
			{
				get
				{
					var arr = new WaitHandle[results.Length];
					for(int i = 0; i < arr.Length; i++)
						arr[i] = results[i].AsyncWaitHandle;
					return arr;
				}
			}

			public override bool WaitOne(int millisecondsTimeout, bool exitContext)
			{
				return WaitHandle.WaitAll(ResultWaitHandles, millisecondsTimeout, exitContext);
			}

			public override bool WaitOne(TimeSpan timeout, bool exitContext)
			{
				return WaitHandle.WaitAll(ResultWaitHandles, timeout, exitContext);
			}
		}

		class DisplayUIAsyncResult : IAsyncResult
		{
			public DisplayUIAsyncResult(IAsyncResult[] results)
			{
				this.results = results;
			}

			IAsyncResult[] results;

			internal IAsyncResult[] Results
			{
				get
				{
					return results;
				}
			}

			public object AsyncState
			{
				get
				{
					return null;
				}
			}

			WaitHandle wait_handle;

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					if(wait_handle == null)
						wait_handle = new DelegatingWaitHandle(results);
					return wait_handle;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					foreach (var r in results)
						if(!r.CompletedSynchronously)
							return false;
					return true;
				}
			}

			public bool IsCompleted
			{
				get
				{
					foreach (var r in results)
						if(!r.IsCompleted)
							return false;
					return true;
				}
			}
		}

		public IAsyncResult BeginDisplayInitializationUI(AsyncCallback callback, object state)
		{
			OnInitializationUI();
			IAsyncResult[] arr = new IAsyncResult[runtime.InteractiveChannelInitializers.Count];
			int i = 0;
			foreach (var init in runtime.InteractiveChannelInitializers)
				arr[i++] = init.BeginDisplayInitializationUI(this, callback, state);
			return new DisplayUIAsyncResult(arr);
		}

		public void EndDisplayInitializationUI(IAsyncResult result)
		{
			DisplayUIAsyncResult r = (DisplayUIAsyncResult)result;
			int i = 0;
			foreach (var init in runtime.InteractiveChannelInitializers)
				init.EndDisplayInitializationUI(r.Results[i++]);
			did_interactive_initialization = true;
		}

		public void DisplayInitializationUI()
		{
			OnInitializationUI();
			foreach (var init in runtime.InteractiveChannelInitializers)
				init.EndDisplayInitializationUI(init.BeginDisplayInitializationUI(this, null, null));
			did_interactive_initialization = true;
		}

		void OnInitializationUI()
		{
			if(!AllowInitializationUI && runtime.InteractiveChannelInitializers.Count > 0)
				throw new InvalidOperationException("AllowInitializationUI is set to false but the client runtime contains one or more InteractiveChannelInitializers.");
		}

		public void Dispose()
		{
			Close();
		}

		public event EventHandler<System.ServiceModel.UnknownMessageReceivedEventArgs> UnknownMessageReceived;

		public bool AllowOutputBatching
		{
			get;
			set;
		}

		public IInputSession InputSession
		{
			get
			{
				ISessionChannel<IInputSession> ch = RequestChannel as ISessionChannel<IInputSession>;
				ch = ch ?? OutputChannel as ISessionChannel<IInputSession>;
				if(ch != null)
					return ch.Session;
				var dch = OutputChannel as ISessionChannel<IDuplexSession>;
				return dch != null ? dch.Session : null;
			}
		}

		public System.ServiceModel.EndpointAddress LocalAddress
		{
			get
			{
				var dc = OperationChannel as IDuplexChannel;
				return dc != null ? dc.LocalAddress : null;
			}
		}

		public TimeSpan OperationTimeout
		{
			get
			{
				return this.operation_timeout ?? (channel_factory != null ? channel_factory.Endpoint.Binding.SendTimeout : DefaultCommunicationTimeouts.Instance.SendTimeout);
			}
			set
			{
				this.operation_timeout = value;
			}
		}

		public IOutputSession OutputSession
		{
			get
			{
				ISessionChannel<IOutputSession> ch = RequestChannel as ISessionChannel<IOutputSession>;
				ch = ch ?? OutputChannel as ISessionChannel<IOutputSession>;
				if(ch != null)
					return ch.Session;
				var dch = OutputChannel as ISessionChannel<IDuplexSession>;
				return dch != null ? dch.Session : null;
			}
		}

		public System.ServiceModel.EndpointAddress RemoteAddress
		{
			get
			{
				return RequestChannel != null ? RequestChannel.RemoteAddress : OutputChannel.RemoteAddress;
			}
		}

		public string SessionId
		{
			get
			{
				return OutputSession != null ? OutputSession.Id : InputSession != null ? InputSession.Id : null;
			}
		}

		protected override TimeSpan DefaultOpenTimeout
		{
			get
			{
				return default_open_timeout;
			}
		}

		protected override TimeSpan DefaultCloseTimeout
		{
			get
			{
				return default_close_timeout;
			}
		}

		protected override void OnAbort()
		{
			channel.Abort();
			if(factory != null)
				factory.Abort();
		}

		Action<TimeSpan> close_delegate;

		protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(close_delegate == null)
				close_delegate = new Action<TimeSpan>(OnClose);
			return close_delegate.BeginInvoke(timeout, callback, state);
		}

		protected override void OnEndClose(IAsyncResult result)
		{
			close_delegate.EndInvoke(result);
		}

		protected override void OnClose(TimeSpan timeout)
		{
			DateTime start = DateTime.Now;
			if(channel.State == System.ServiceModel.CommunicationState.Opened)
				channel.Close(timeout);
		}

		Action<TimeSpan> open_callback;

		protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(open_callback == null)
				open_callback = new Action<TimeSpan>(OnOpen);
			return open_callback.BeginInvoke(timeout, callback, state);
		}

		protected override void OnEndOpen(IAsyncResult result)
		{
			if(open_callback == null)
				throw new InvalidOperationException("Async open operation has not started");
			open_callback.EndInvoke(result);
		}

		protected override void OnOpen(TimeSpan timeout)
		{
			if(runtime.InteractiveChannelInitializers.Count > 0 && !DidInteractiveInitialization)
				throw new InvalidOperationException("The client runtime is assigned interactive channel initializers, and in such case DisplayInitializationUI must be called before the channel is opened.");
			if(channel.State == System.ServiceModel.CommunicationState.Created)
				channel.Open(timeout);
		}

		IChannel OperationChannel
		{
			get
			{
				return channel;
			}
		}

		public T GetProperty<T>() where T : class
		{
			if(typeof(T) == typeof(MessageVersion))
				return (T)(object)message_version;
			return OperationChannel.GetProperty<T>();
		}

		System.ServiceModel.IExtensionCollection<System.ServiceModel.IContextChannel> extensions;

		public System.ServiceModel.IExtensionCollection<System.ServiceModel.IContextChannel> Extensions
		{
			get
			{
				if(extensions == null)
					extensions = new System.ServiceModel.ExtensionCollection<System.ServiceModel.IContextChannel>(this);
				return extensions;
			}
		}

		public IAsyncResult BeginProcess(System.Reflection.MethodBase method, string operationName, object[] parameters, AsyncCallback callback, object asyncState)
		{
			var p = parameters;
			var retval = _processDelegate.BeginInvoke(method, operationName, true, ref p, System.ServiceModel.OperationContext.Current, callback, asyncState);
			if(p != parameters)
				throw new InvalidOperationException();
			return retval;
		}

		public object EndProcess(System.Reflection.MethodBase method, string operationName, object[] parameters, IAsyncResult result)
		{
			if(result == null)
				throw new ArgumentNullException("result");
			if(parameters == null)
				throw new ArgumentNullException("parameters");
			object[] p = parameters;
			var retval = _processDelegate.EndInvoke(ref p, result);
			if(p == parameters)
				return retval;
			Array.Copy(p, parameters, p.Length);
			return retval;
		}

		public object Process(System.Reflection.MethodBase method, string operationName, object[] parameters, System.ServiceModel.OperationContext context)
		{
			var p = parameters;
			var retval = Process(method, operationName, false, ref p, context);
			if(p != parameters)
				throw new InvalidOperationException();
			return retval;
		}

		object Process(System.Reflection.MethodBase method, string operationName, bool isAsync, ref object[] parameters, System.ServiceModel.OperationContext context)
		{
			var previousContext = System.ServiceModel.OperationContext.Current;
			try
			{
				System.ServiceModel.OperationContext.Current = context;
				return DoProcess(method, operationName, isAsync, ref parameters, context);
			}
			catch(Exception ex)
			{
				throw;
			}
			finally
			{
				System.ServiceModel.OperationContext.Current = previousContext;
			}
		}

		object DoProcess(System.Reflection.MethodBase method, string operationName, bool isAsync, ref object[] parameters, System.ServiceModel.OperationContext context)
		{
			if(AllowInitializationUI)
				DisplayInitializationUI();
			OperationDescription od = SelectOperation(method, operationName, parameters);
			if(State != System.ServiceModel.CommunicationState.Opened)
				Open();
			if(!od.IsOneWay)
				return Request(od, isAsync, ref parameters, context);
			else
			{
				Output(od, parameters, context);
				return null;
			}
		}

		OperationDescription SelectOperation(System.Reflection.MethodBase method, string operationName, object[] parameters)
		{
			string operation;
			if(Runtime.OperationSelector != null)
				operation = Runtime.OperationSelector.SelectOperation(method, parameters);
			else
				operation = operationName;
			OperationDescription od = contract.Operations.Find(operation);
			if(od == null)
				throw new Exception(String.Format("OperationDescription for operation '{0}' was not found in its internally-generated contract.", operation));
			return od;
		}

		void Output(OperationDescription od, object[] parameters, System.ServiceModel.OperationContext context)
		{
			System.ServiceModel.Dispatcher.ClientOperation op = runtime.Operations[od.Name];
			Send(CreateRequest(op, parameters, context), OperationTimeout);
		}

		private static System.Runtime.Serialization.DataContractSerializer GetSerializer(System.ServiceModel.Dispatcher.FaultContractInfo fci)
		{
			return (System.Runtime.Serialization.DataContractSerializer)typeof(System.ServiceModel.Dispatcher.FaultContractInfo).GetProperty("Serializer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(fci, null);
		}

		object Request(OperationDescription od, bool isAsync, ref object[] parameters, System.ServiceModel.OperationContext context)
		{
			System.ServiceModel.Dispatcher.ClientOperation op = runtime.Operations[od.Name];
			object[] inspections = new object[runtime.MessageInspectors.Count];
			Message req = CreateRequest(op, parameters, context);
			for(int i = 0; i < inspections.Length; i++)
				inspections[i] = runtime.MessageInspectors[i].BeforeSendRequest(ref req, this);
			Message res = Request(req, OperationTimeout);
			if(res.IsFault)
			{
				var resb = res.CreateBufferedCopy(runtime.MaxFaultSize);
				MessageFault fault = MessageFault.CreateFault(resb.CreateMessage(), runtime.MaxFaultSize);
				var conv = OperationChannel.GetProperty<FaultConverter>() ?? FaultConverter.GetDefaultFaultConverter(res.Version);
				Exception ex;
				if(!conv.TryCreateException(resb.CreateMessage(), fault, out ex))
				{
					if(fault.HasDetail)
					{
						Type detailType = typeof(System.ServiceModel.ExceptionDetail);
						var freader = fault.GetReaderAtDetailContents();
						System.Runtime.Serialization.DataContractSerializer ds = null;
						foreach (var fci in op.FaultContractInfos)
							if(res.Headers.Action == fci.Action || GetSerializer(fci).IsStartObject(freader))
							{
								detailType = fci.Detail;
								ds = GetSerializer(fci);
								break;
							}
						if(ds == null)
							ds = new System.Runtime.Serialization.DataContractSerializer(detailType);
						var detail = ds.ReadObject(freader);
						ex = (Exception)Activator.CreateInstance(typeof(System.ServiceModel.FaultException<>).MakeGenericType(detailType), new object[] {
							detail,
							fault.Reason,
							fault.Code,
							res.Headers.Action
						});
					}
					if(ex == null)
						ex = new System.ServiceModel.FaultException(fault);
				}
				throw ex;
			}
			for(int i = 0; i < inspections.Length; i++)
				runtime.MessageInspectors[i].AfterReceiveReply(ref res, inspections[i]);
			if(!op.DeserializeReply)
				return res;
			if(isAsync && od.EndMethod != null)
			{
				var endParams = od.EndMethod.GetParameters();
				parameters = new object[endParams.Length - 1];
			}
			return op.Formatter.DeserializeReply(res, parameters);
		}

		internal Message Request(Message msg, TimeSpan timeout)
		{
			if(RequestChannel != null)
				return RequestChannel.Request(msg, timeout);
			else
				return RequestCorrelated(msg, timeout, OutputChannel);
		}

		internal virtual Message RequestCorrelated(Message msg, TimeSpan timeout, IOutputChannel channel)
		{
			DateTime startTime = DateTime.Now;
			OutputChannel.Send(msg, timeout);
			return ((IDuplexChannel)channel).Receive(timeout - (DateTime.Now - startTime));
		}

		internal IAsyncResult BeginRequest(Message msg, TimeSpan timeout, AsyncCallback callback, object state)
		{
			return requestDelegate.BeginInvoke(msg, timeout, callback, state);
		}

		internal Message EndRequest(IAsyncResult result)
		{
			return requestDelegate.EndInvoke(result);
		}

		internal void Send(Message msg, TimeSpan timeout)
		{
			if(OutputChannel != null)
				OutputChannel.Send(msg, timeout);
			else
				RequestChannel.Request(msg, timeout);
		}

		internal IAsyncResult BeginSend(Message msg, TimeSpan timeout, AsyncCallback callback, object state)
		{
			return sendDelegate.BeginInvoke(msg, timeout, callback, state);
		}

		internal void EndSend(IAsyncResult result)
		{
			sendDelegate.EndInvoke(result);
		}

		Message CreateRequest(System.ServiceModel.Dispatcher.ClientOperation op, object[] parameters, System.ServiceModel.OperationContext context)
		{
			MessageVersion version = message_version;
			if(version == null)
				version = MessageVersion.Default;
			Message msg;
			if(op.SerializeRequest)
				msg = op.Formatter.SerializeRequest(version, parameters);
			else
			{
				if(parameters.Length != 1)
					throw new ArgumentException(String.Format("Argument parameters does not match the expected input. It should contain only a Message, but has {0} parameters", parameters.Length));
				if(!(parameters[0] is Message))
					throw new ArgumentException(String.Format("Argument should be only a Message, but has {0}", parameters[0] != null ? parameters[0].GetType().FullName : "null"));
				msg = (Message)parameters[0];
			}
			context = context ?? System.ServiceModel.OperationContext.Current;
			if(context != null)
			{
				foreach (var mh in context.OutgoingMessageHeaders)
				{
					int x = msg.Headers.FindHeader(mh.Name, mh.Namespace, mh.Actor);
					if(x >= 0)
						msg.Headers.RemoveAt(x);
					msg.Headers.Add((MessageHeader)mh);
				}
				msg.Properties.CopyProperties(context.OutgoingMessageProperties);
			}
			msg.Properties.AllowOutputBatching = AllowOutputBatching;
			if(msg.Version.Addressing.Equals(AddressingVersion.WSAddressing10))
			{
				if(msg.Headers.MessageId == null)
					msg.Headers.MessageId = new System.Xml.UniqueId();
				if(msg.Headers.ReplyTo == null)
					msg.Headers.ReplyTo = new System.ServiceModel.EndpointAddress(Constants.WsaAnonymousUri);
				if(msg.Headers.To == null && RemoteAddress != null)
					msg.Headers.To = RemoteAddress.Uri;
			}
			return msg;
		}
	}
}
