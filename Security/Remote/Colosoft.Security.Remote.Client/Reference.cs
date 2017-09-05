namespace Colosoft.Security.Remote.Client.AuthenticationHost{
	using System.Runtime.Serialization;
	using System;
	[System.Diagnostics.DebuggerStepThroughAttribute ()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.Runtime.Serialization", "4.0.0.0")]
	[System.Runtime.Serialization.DataContractAttribute (Name = "ValidateUserResult", Namespace = "http://schemas.datacontract.org/2004/07/Colosoft.Security.Remote.Server")]
	[System.SerializableAttribute ()]
	internal partial class ValidateUserResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged	{
		[System.NonSerializedAttribute ()]
		private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private Colosoft.Security.Remote.Client.AuthenticationHost.CaptchaInfo CaptchaField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private System.Nullable<System.DateTimeOffset> ExpireDateField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private string MessageField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private Colosoft.Security.Remote.Client.AuthenticationHost.AuthenticationStatus StatusField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private string TokenField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private Colosoft.Security.Remote.Client.ServerHost.User UserField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private Colosoft.Net.ServiceAddress UserProviderServiceAddressField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private Colosoft.Net.ServiceAddress ProfileProviderServiceAddressField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private Colosoft.Net.ServiceAddress ServiceAddressProviderServiceAddressField;
		[global::System.ComponentModel.BrowsableAttribute (false)]
		public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
			get {
				return this.extensionDataField;
			}
			set {
				this.extensionDataField = value;
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal Colosoft.Security.Remote.Client.AuthenticationHost.CaptchaInfo Captcha {
			get {
				return this.CaptchaField;
			}
			set {
				if ((object.ReferenceEquals (this.CaptchaField, value) != true)) {
					this.CaptchaField = value;
					this.RaisePropertyChanged ("Captcha");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal System.Nullable<System.DateTimeOffset> ExpireDate {
			get {
				return this.ExpireDateField;
			}
			set {
				if ((this.ExpireDateField.Equals (value) != true)) {
					this.ExpireDateField = value;
					this.RaisePropertyChanged ("ExpireDate");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal string Message {
			get {
				return this.MessageField;
			}
			set {
				if ((object.ReferenceEquals (this.MessageField, value) != true)) {
					this.MessageField = value;
					this.RaisePropertyChanged ("Message");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal Colosoft.Security.Remote.Client.AuthenticationHost.AuthenticationStatus Status {
			get {
				return this.StatusField;
			}
			set {
				if ((this.StatusField.Equals (value) != true)) {
					this.StatusField = value;
					this.RaisePropertyChanged ("Status");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal string Token {
			get {
				return this.TokenField;
			}
			set {
				if ((object.ReferenceEquals (this.TokenField, value) != true)) {
					this.TokenField = value;
					this.RaisePropertyChanged ("Token");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal Colosoft.Security.Remote.Client.ServerHost.User User {
			get {
				return this.UserField;
			}
			set {
				if ((object.ReferenceEquals (this.UserField, value) != true)) {
					this.UserField = value;
					this.RaisePropertyChanged ("User");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal Colosoft.Net.ServiceAddress UserProviderServiceAddress {
			get {
				return UserProviderServiceAddressField;
			}
			set {
				if ((object.ReferenceEquals (UserProviderServiceAddressField, value) != true)) {
					this.UserProviderServiceAddressField = value;
					this.RaisePropertyChanged ("UserProviderServiceAddress");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal Colosoft.Net.ServiceAddress ProfileProviderServiceAddress {
			get {
				return ProfileProviderServiceAddressField;
			}
			set {
				if ((object.ReferenceEquals (ProfileProviderServiceAddressField, value) != true)) {
					this.ProfileProviderServiceAddressField = value;
					this.RaisePropertyChanged ("ProfileProviderServiceAddress");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal Colosoft.Net.ServiceAddress ServiceAddressProviderServiceAddress {
			get {
				return ServiceAddressProviderServiceAddressField;
			}
			set {
				if ((object.ReferenceEquals (ServiceAddressProviderServiceAddressField, value) != true)) {
					this.ServiceAddressProviderServiceAddressField = value;
					this.RaisePropertyChanged ("ServiceAddressProviderServiceAddress");
				}
			}
		}
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged (string a)		{
			System.ComponentModel.PropertyChangedEventHandler b = this.PropertyChanged;
			if ((b != null)) {
				b (this, new System.ComponentModel.PropertyChangedEventArgs (a));
			}
		}
	}
	[System.Diagnostics.DebuggerStepThroughAttribute ()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.Runtime.Serialization", "4.0.0.0")]
	[System.Runtime.Serialization.DataContractAttribute (Name = "CaptchaInfo", Namespace = "http://schemas.datacontract.org/2004/07/Colosoft.Security.CaptchaSupport")]
	[System.SerializableAttribute ()]
	internal partial class CaptchaInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged	{
		[System.NonSerializedAttribute ()]
		private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private byte[] ImageField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private System.Guid UidField;
		[global::System.ComponentModel.BrowsableAttribute (false)]
		public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
			get {
				return this.extensionDataField;
			}
			set {
				this.extensionDataField = value;
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal byte[] Image {
			get {
				return this.ImageField;
			}
			set {
				if ((object.ReferenceEquals (this.ImageField, value) != true)) {
					this.ImageField = value;
					this.RaisePropertyChanged ("Image");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal System.Guid Uid {
			get {
				return this.UidField;
			}
			set {
				if ((this.UidField.Equals (value) != true)) {
					this.UidField = value;
					this.RaisePropertyChanged ("Uid");
				}
			}
		}
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged (string a)		{
			System.ComponentModel.PropertyChangedEventHandler b = this.PropertyChanged;
			if ((b != null)) {
				b (this, new System.ComponentModel.PropertyChangedEventArgs (a));
			}
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.Runtime.Serialization", "4.0.0.0")]
	[System.Runtime.Serialization.DataContractAttribute (Name = "AuthenticationStatus", Namespace = "http://schemas.datacontract.org/2004/07/Colosoft.Security")]
	internal enum AuthenticationStatus : int
	{
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		Success = 0,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		InvalidUserNameOrPassword = 1,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		InvalidDomain = 2,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		PasswordExpired = 3,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		PasswordWarning = 4,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		ErrorInValidate = 5,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		CaptchaRequired = 6,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		InvalidCaptcha = 7,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		ErrorTokenControl = 8,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		DuplicateToken = 9,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		ErrorInCommunication = 10,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		UnknownError = 11,
	}
	[System.Diagnostics.DebuggerStepThroughAttribute ()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.Runtime.Serialization", "4.0.0.0")]
	[System.Runtime.Serialization.DataContractAttribute (Name = "ChangePasswordResult", Namespace = "http://schemas.datacontract.org/2004/07/Colosoft.Security")]
	[System.SerializableAttribute ()]
	internal partial class ChangePasswordResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged	{
		[System.NonSerializedAttribute ()]
		private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private string MessageField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private ChangePasswordStatus StatusField;
		[global::System.ComponentModel.BrowsableAttribute (false)]
		public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
			get {
				return this.extensionDataField;
			}
			set {
				this.extensionDataField = value;
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal string Message {
			get {
				return this.MessageField;
			}
			set {
				if ((object.ReferenceEquals (this.MessageField, value) != true)) {
					this.MessageField = value;
					this.RaisePropertyChanged ("Message");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal ChangePasswordStatus Status {
			get {
				return this.StatusField;
			}
			set {
				if ((this.StatusField.Equals (value) != true)) {
					this.StatusField = value;
					this.RaisePropertyChanged ("Status");
				}
			}
		}
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged (string a)		{
			System.ComponentModel.PropertyChangedEventHandler b = this.PropertyChanged;
			if ((b != null)) {
				b (this, new System.ComponentModel.PropertyChangedEventArgs (a));
			}
		}
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	[System.ServiceModel.ServiceContractAttribute (ConfigurationName = "AuthenticationHost.IAuthenticationService")]
	internal interface IAuthenticationService	{
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IAuthenticationService/ValidateUser", ReplyAction = "http://tempuri.org/IAuthenticationService/ValidateUserResponse")]
		Colosoft.Security.Remote.Client.AuthenticationHost.ValidateUserResult ValidateUser (string a, string b, string c, SecurityParameter[] d);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IAuthenticationService/ValidateToken", ReplyAction = "http://tempuri.org/IAuthenticationService/ValidateTokenResponse")]
		Colosoft.Security.Remote.Client.AuthenticationHost.ValidateUserResult ValidateToken (string a, string b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IAuthenticationService/LogOut", ReplyAction = "http://tempuri.org/IAuthenticationService/LogOutResponse")]
		bool LogOut (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IAuthenticationService/Check", ReplyAction = "http://tempuri.org/IAuthenticationService/CheckResponse")]
		TokenConsultResult Check (string a, string b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IAuthenticationService/ChangePassword", ReplyAction = "http://tempuri.org/IAuthenticationService/ChangePasswordResponse")]
		Colosoft.Security.Remote.Client.AuthenticationHost.ChangePasswordResult ChangePassword (string a, string b, string c, SecurityParameter[] d);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IAuthenticationService/RequestPasswordReset", ReplyAction = "http://tempuri.org/IAuthenticationService/RequestPasswordResetResponse")]
		Colosoft.Security.ResetPasswordProcessResult RequestPasswordReset (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IAuthenticationService/ResetPassword", ReplyAction = "http://tempuri.org/IAuthenticationService/ResetPasswordResponse")]
		Colosoft.Security.Remote.Client.AuthenticationHost.ChangePasswordResult ResetPassword (string a, string b, string c, params SecurityParameter[] d);
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	internal interface IAuthenticationServiceChannel : Colosoft.Security.Remote.Client.AuthenticationHost.IAuthenticationService, System.ServiceModel.IClientChannel	{
	}
	[System.Diagnostics.DebuggerStepThroughAttribute ()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	internal partial class AuthenticationServiceClient : System.ServiceModel.ClientBase<Colosoft.Security.Remote.Client.AuthenticationHost.IAuthenticationService>, Colosoft.Security.Remote.Client.AuthenticationHost.IAuthenticationService	{
		public AuthenticationServiceClient ()		{
		}
		public AuthenticationServiceClient (string a) : base (a)		{
		}
		public AuthenticationServiceClient (string a, string b) : base (a, b)		{
		}
		public AuthenticationServiceClient (string a, System.ServiceModel.EndpointAddress b) : base (a, b)		{
		}
		public AuthenticationServiceClient (System.ServiceModel.Channels.Binding a, System.ServiceModel.EndpointAddress b) : base (a, b)		{
		}
		public Colosoft.Security.Remote.Client.AuthenticationHost.ValidateUserResult ValidateUser (string a, string b, string c, SecurityParameter[] d)		{
			return base.Channel.ValidateUser (a, b, c, d);
		}
		public Colosoft.Security.Remote.Client.AuthenticationHost.ValidateUserResult ValidateToken (string a, string b)		{
			return base.Channel.ValidateToken (a, b);
		}
		public bool LogOut (string a)		{
			return base.Channel.LogOut (a);
		}
		public TokenConsultResult Check (string a, string b)		{
			return base.Channel.Check (a, b);
		}
		public Colosoft.Security.Remote.Client.AuthenticationHost.ChangePasswordResult ChangePassword (string a, string b, string c, SecurityParameter[] d)		{
			return base.Channel.ChangePassword (a, b, c, d);
		}
		public Colosoft.Security.ResetPasswordProcessResult RequestPasswordReset (string a)		{
			return base.Channel.RequestPasswordReset (a);
		}
		public Colosoft.Security.Remote.Client.AuthenticationHost.ChangePasswordResult ResetPassword (string a, string b, string c, params SecurityParameter[] d)		{
			return base.Channel.ResetPassword (a, b, c, d);
		}
	}
}
