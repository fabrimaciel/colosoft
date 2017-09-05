namespace Colosoft.Security.Remote.Client.ProfileProviderServiceReference{
	using System.Runtime.Serialization;
	using System;
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	[System.ServiceModel.ServiceContractAttribute (ConfigurationName = "ProfileProviderServiceReference.IProfileProviderService")]
	internal interface IProfileProviderService	{
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IProfileProviderService/DeleteProfiles", ReplyAction = "http://tempuri.org/IProfileProviderService/DeleteProfilesResponse")]
		int DeleteProfiles (string[] a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IProfileProviderService/DeleteProfilesByProfileInfo", ReplyAction = "http://tempuri.org/IProfileProviderService/DeleteProfilesByProfileInfoResponse")]
		int DeleteProfilesByProfileInfo (Colosoft.Security.Remote.Client.ProfileProviderServiceReference.ProfileInfo[] a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IProfileProviderService/FindProfilesByUserName", ReplyAction = "http://tempuri.org/IProfileProviderService/FindProfilesByUserNameResponse")]
		Colosoft.Security.Remote.Client.ProfileProviderServiceReference.ProfileInfo[] FindProfilesByUserName (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IProfileProviderService/GetUserProfiles", ReplyAction = "http://tempuri.org/IProfileProviderService/GetUserProfilesResponse")]
		Colosoft.Security.Remote.Client.ProfileProviderServiceReference.ProfileInfo[] GetUserProfiles (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IProfileProviderService/GetProfile", ReplyAction = "http://tempuri.org/IProfileProviderService/GetProfileResponse")]
		Colosoft.Security.Remote.Client.ProfileProviderServiceReference.Profile GetProfile (Colosoft.Security.Remote.Client.ProfileProviderServiceReference.ProfileInfo a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IProfileProviderService/GetSource", ReplyAction = "http://tempuri.org/IProfileProviderService/GetSourceResponse")]
		Colosoft.Security.Remote.Client.ProfileProviderServiceReference.AuthenticationSource GetSource (int a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IProfileProviderService/SetProfilePropertyValue", ReplyAction = "http://tempuri.org/IProfileProviderService/SetProfilePropertyValueResponse")]
		void SetProfilePropertyValue (Colosoft.Security.Remote.Client.ProfileProviderServiceReference.ProfileInfo a, Colosoft.Security.Profile.ProfilePropertyDefinition b, string c);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IProfileProviderService/GetProfilePropertyDefinitions", ReplyAction = "http://tempuri.org/IProfileProviderService/GetProfilePropertyDefinitionsResponse")]
		Colosoft.Security.Profile.ProfilePropertyDefinition[] GetProfilePropertyDefinitions ();
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	internal interface IProfileProviderServiceChannel : Colosoft.Security.Remote.Client.ProfileProviderServiceReference.IProfileProviderService, System.ServiceModel.IClientChannel	{
	}
	[System.Diagnostics.DebuggerStepThroughAttribute ()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	internal partial class ProfileProviderServiceClient : System.ServiceModel.ClientBase<Colosoft.Security.Remote.Client.ProfileProviderServiceReference.IProfileProviderService>, Colosoft.Security.Remote.Client.ProfileProviderServiceReference.IProfileProviderService	{
		public ProfileProviderServiceClient ()		{
		}
		public ProfileProviderServiceClient (string a) : base (a)		{
		}
		public ProfileProviderServiceClient (string a, string b) : base (a, b)		{
		}
		public ProfileProviderServiceClient (string a, System.ServiceModel.EndpointAddress b) : base (a, b)		{
		}
		public ProfileProviderServiceClient (System.ServiceModel.Channels.Binding a, System.ServiceModel.EndpointAddress b) : base (a, b)		{
		}
		public int DeleteProfiles (string[] a)		{
			return base.Channel.DeleteProfiles (a);
		}
		public int DeleteProfilesByProfileInfo (Colosoft.Security.Remote.Client.ProfileProviderServiceReference.ProfileInfo[] a)		{
			return base.Channel.DeleteProfilesByProfileInfo (a);
		}
		public Colosoft.Security.Remote.Client.ProfileProviderServiceReference.ProfileInfo[] FindProfilesByUserName (string a)		{
			return base.Channel.FindProfilesByUserName (a);
		}
		public Colosoft.Security.Remote.Client.ProfileProviderServiceReference.ProfileInfo[] GetUserProfiles (string a)		{
			return base.Channel.GetUserProfiles (a);
		}
		public Colosoft.Security.Remote.Client.ProfileProviderServiceReference.Profile GetProfile (Colosoft.Security.Remote.Client.ProfileProviderServiceReference.ProfileInfo a)		{
			return base.Channel.GetProfile (a);
		}
		public Colosoft.Security.Remote.Client.ProfileProviderServiceReference.AuthenticationSource GetSource (int a)		{
			return base.Channel.GetSource (a);
		}
		public void SetProfilePropertyValue (Colosoft.Security.Remote.Client.ProfileProviderServiceReference.ProfileInfo a, Colosoft.Security.Profile.ProfilePropertyDefinition b, string c)		{
			base.Channel.SetProfilePropertyValue (a, b, c);
		}
		public Colosoft.Security.Profile.ProfilePropertyDefinition[] GetProfilePropertyDefinitions ()		{
			return base.Channel.GetProfilePropertyDefinitions ();
		}
	}
}
