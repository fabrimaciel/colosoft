namespace Colosoft.Security.Remote.Client.TokenProviderServiceReference{
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	[System.ServiceModel.ServiceContractAttribute (ConfigurationName = "TokenProviderServiceReference.ITokenProviderService")]
	internal interface ITokenProviderService	{
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/ITokenProviderService/Check", ReplyAction = "http://tempuri.org/ITokenProviderService/CheckResponse")]
		Colosoft.Security.TokenConsultResult Check (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/ITokenProviderService/Ping", ReplyAction = "http://tempuri.org/ITokenProviderService/PingResponse")]
		[System.ServiceModel.ServiceKnownTypeAttribute (typeof(Colosoft.Security.TokenConsultResult))]
		[System.ServiceModel.ServiceKnownTypeAttribute (typeof(Colosoft.Security.TokenPingResultStatus))]
		Colosoft.Security.TokenPingResult Ping (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/ITokenProviderService/MarkMessageAsRead", ReplyAction = "http://tempuri.org/ITokenProviderService/MarkMessageAsReadResponse")]
		void MarkMessageAsRead (System.Collections.Generic.IEnumerable<int> a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/ITokenProviderService/CloseUserTokens", ReplyAction = "http://tempuri.org/ITokenProviderService/CloseUserTokensResponse")]
		void CloseUserTokens (int a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/ITokenProviderService/CloseUserTokens2", ReplyAction = "http://tempuri.org/ITokenProviderService/CloseUserTokens2Response")]
		void CloseUserTokens2 (int a, string b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/ITokenProviderService/SetProfile", ReplyAction = "http://tempuri.org/ITokenProviderService/SetProfileResponse")]
		Colosoft.Security.TokenSetProfileResult SetProfile (string a, int b);
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	internal interface ITokenProviderServiceChannel : Colosoft.Security.Remote.Client.TokenProviderServiceReference.ITokenProviderService, System.ServiceModel.IClientChannel	{
	}
	[System.Diagnostics.DebuggerStepThroughAttribute ()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	internal partial class TokenProviderServiceClient : System.ServiceModel.ClientBase<Colosoft.Security.Remote.Client.TokenProviderServiceReference.ITokenProviderService>, Colosoft.Security.Remote.Client.TokenProviderServiceReference.ITokenProviderService	{
		public TokenProviderServiceClient ()		{
		}
		public TokenProviderServiceClient (string a) : base (a)		{
		}
		public TokenProviderServiceClient (string a, string b) : base (a, b)		{
		}
		public TokenProviderServiceClient (string a, System.ServiceModel.EndpointAddress b) : base (a, b)		{
		}
		public TokenProviderServiceClient (System.ServiceModel.Channels.Binding a, System.ServiceModel.EndpointAddress b) : base (a, b)		{
		}
		public Colosoft.Security.TokenConsultResult Check (string a)		{
			return base.Channel.Check (a);
		}
		public Colosoft.Security.TokenPingResult Ping (string a)		{
			return base.Channel.Ping (a);
		}
		public void MarkMessageAsRead (System.Collections.Generic.IEnumerable<int> a)		{
			base.Channel.MarkMessageAsRead (a);
		}
		public void CloseUserTokens (int a)		{
			base.Channel.CloseUserTokens (a);
		}
		public void CloseUserTokens2 (int a, string b)		{
			base.Channel.CloseUserTokens2 (a, b);
		}
		public Colosoft.Security.TokenSetProfileResult SetProfile (string a, int b)		{
			return base.Channel.SetProfile (a, b);
		}
	}
}
