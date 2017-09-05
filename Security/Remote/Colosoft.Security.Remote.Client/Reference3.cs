﻿namespace Colosoft.Security.Remote.Client.ServerHost{
	using System.Runtime.Serialization;
	using System;
	[System.Diagnostics.DebuggerStepThroughAttribute ()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.Runtime.Serialization", "4.0.0.0")]
	[System.Runtime.Serialization.DataContractAttribute (Name = "UserProviderInfo", Namespace = "http://schemas.datacontract.org/2004/07/Colosoft.Security.Remote.Server")]
	[System.SerializableAttribute ()]
	internal partial class UserProviderInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged	{
		[System.NonSerializedAttribute ()]
		private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private bool EnablePasswordResetField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private bool EnablePasswordRetrievalField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private int MaxInvalidPasswordAttemptsField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private int MinRequiredNonAlphanumericCharactersField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private int MinRequiredPasswordLengthField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private Colosoft.Security.Remote.Client.ServerHost.PasswordFormat PasswordFormatField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private string PasswordStrengthRegularExpressionField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private bool RequiresQuestionAndAnswerField;
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
		internal bool EnablePasswordReset {
			get {
				return this.EnablePasswordResetField;
			}
			set {
				if ((this.EnablePasswordResetField.Equals (value) != true)) {
					this.EnablePasswordResetField = value;
					this.RaisePropertyChanged ("EnablePasswordReset");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal bool EnablePasswordRetrieval {
			get {
				return this.EnablePasswordRetrievalField;
			}
			set {
				if ((this.EnablePasswordRetrievalField.Equals (value) != true)) {
					this.EnablePasswordRetrievalField = value;
					this.RaisePropertyChanged ("EnablePasswordRetrieval");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal int MaxInvalidPasswordAttempts {
			get {
				return this.MaxInvalidPasswordAttemptsField;
			}
			set {
				if ((this.MaxInvalidPasswordAttemptsField.Equals (value) != true)) {
					this.MaxInvalidPasswordAttemptsField = value;
					this.RaisePropertyChanged ("MaxInvalidPasswordAttempts");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal int MinRequiredNonAlphanumericCharacters {
			get {
				return this.MinRequiredNonAlphanumericCharactersField;
			}
			set {
				if ((this.MinRequiredNonAlphanumericCharactersField.Equals (value) != true)) {
					this.MinRequiredNonAlphanumericCharactersField = value;
					this.RaisePropertyChanged ("MinRequiredNonAlphanumericCharacters");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal int MinRequiredPasswordLength {
			get {
				return this.MinRequiredPasswordLengthField;
			}
			set {
				if ((this.MinRequiredPasswordLengthField.Equals (value) != true)) {
					this.MinRequiredPasswordLengthField = value;
					this.RaisePropertyChanged ("MinRequiredPasswordLength");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal Colosoft.Security.Remote.Client.ServerHost.PasswordFormat PasswordFormat {
			get {
				return this.PasswordFormatField;
			}
			set {
				if ((this.PasswordFormatField.Equals (value) != true)) {
					this.PasswordFormatField = value;
					this.RaisePropertyChanged ("PasswordFormat");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal string PasswordStrengthRegularExpression {
			get {
				return this.PasswordStrengthRegularExpressionField;
			}
			set {
				if ((object.ReferenceEquals (this.PasswordStrengthRegularExpressionField, value) != true)) {
					this.PasswordStrengthRegularExpressionField = value;
					this.RaisePropertyChanged ("PasswordStrengthRegularExpression");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal bool RequiresQuestionAndAnswer {
			get {
				return this.RequiresQuestionAndAnswerField;
			}
			set {
				if ((this.RequiresQuestionAndAnswerField.Equals (value) != true)) {
					this.RequiresQuestionAndAnswerField = value;
					this.RaisePropertyChanged ("RequiresQuestionAndAnswer");
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
	[System.Runtime.Serialization.DataContractAttribute (Name = "PasswordFormat", Namespace = "http://schemas.datacontract.org/2004/07/Colosoft.Security")]
	internal enum PasswordFormat : int
	{
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		Clear = 0,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		Hashed = 1,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		Encrypted = 2,
	}
	[System.Diagnostics.DebuggerStepThroughAttribute ()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.Runtime.Serialization", "4.0.0.0")]
	[System.Runtime.Serialization.DataContractAttribute (Name = "CreateUserResult", Namespace = "http://schemas.datacontract.org/2004/07/Colosoft.Security.Remote.Server")]
	[System.SerializableAttribute ()]
	internal partial class CreateUserResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged	{
		[System.NonSerializedAttribute ()]
		private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private Colosoft.Security.Remote.Client.ServerHost.UserCreateStatus StatusField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private Colosoft.Security.Remote.Client.ServerHost.User UserField;
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
		internal Colosoft.Security.Remote.Client.ServerHost.UserCreateStatus Status {
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
	[System.Runtime.Serialization.DataContractAttribute (Name = "User", Namespace = "http://schemas.datacontract.org/2004/07/Colosoft.Security.Remote.Server")]
	[System.SerializableAttribute ()]
	internal partial class User : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged	{
		[System.NonSerializedAttribute ()]
		private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private System.DateTimeOffset CreationDateField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private string EmailField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private string FullNameField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private string IdentityProviderField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private bool IgnoreCaptchaField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private bool IsActiveField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private bool IsApprovedField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private bool IsOnlineField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private System.DateTimeOffset LastActivityDateField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private System.DateTimeOffset LastLoginDateField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private System.DateTimeOffset LastPasswordChangedDateField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private string PasswordQuestionField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private string UserKeyField;
		[System.Runtime.Serialization.OptionalFieldAttribute ()]
		private string UserNameField;
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
		internal System.DateTimeOffset CreationDate {
			get {
				return this.CreationDateField;
			}
			set {
				if ((this.CreationDateField.Equals (value) != true)) {
					this.CreationDateField = value;
					this.RaisePropertyChanged ("CreationDate");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal string Email {
			get {
				return this.EmailField;
			}
			set {
				if ((object.ReferenceEquals (this.EmailField, value) != true)) {
					this.EmailField = value;
					this.RaisePropertyChanged ("Email");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal string FullName {
			get {
				return this.FullNameField;
			}
			set {
				if ((object.ReferenceEquals (this.FullNameField, value) != true)) {
					this.FullNameField = value;
					this.RaisePropertyChanged ("FullName");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal string IdentityProvider {
			get {
				return this.IdentityProviderField;
			}
			set {
				if ((object.ReferenceEquals (this.IdentityProviderField, value) != true)) {
					this.IdentityProviderField = value;
					this.RaisePropertyChanged ("IdentityProvider");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal bool IgnoreCaptcha {
			get {
				return this.IgnoreCaptchaField;
			}
			set {
				if ((this.IgnoreCaptchaField.Equals (value) != true)) {
					this.IgnoreCaptchaField = value;
					this.RaisePropertyChanged ("IgnoreCaptcha");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal bool IsActive {
			get {
				return this.IsActiveField;
			}
			set {
				if ((this.IsActiveField.Equals (value) != true)) {
					this.IsActiveField = value;
					this.RaisePropertyChanged ("IsActive");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal bool IsApproved {
			get {
				return this.IsApprovedField;
			}
			set {
				if ((this.IsApprovedField.Equals (value) != true)) {
					this.IsApprovedField = value;
					this.RaisePropertyChanged ("IsApproved");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal bool IsOnline {
			get {
				return this.IsOnlineField;
			}
			set {
				if ((this.IsOnlineField.Equals (value) != true)) {
					this.IsOnlineField = value;
					this.RaisePropertyChanged ("IsOnline");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal System.DateTimeOffset LastActivityDate {
			get {
				return this.LastActivityDateField;
			}
			set {
				if ((this.LastActivityDateField.Equals (value) != true)) {
					this.LastActivityDateField = value;
					this.RaisePropertyChanged ("LastActivityDate");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal System.DateTimeOffset LastLoginDate {
			get {
				return this.LastLoginDateField;
			}
			set {
				if ((this.LastLoginDateField.Equals (value) != true)) {
					this.LastLoginDateField = value;
					this.RaisePropertyChanged ("LastLoginDate");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal System.DateTimeOffset LastPasswordChangedDate {
			get {
				return this.LastPasswordChangedDateField;
			}
			set {
				if ((this.LastPasswordChangedDateField.Equals (value) != true)) {
					this.LastPasswordChangedDateField = value;
					this.RaisePropertyChanged ("LastPasswordChangedDate");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal string PasswordQuestion {
			get {
				return this.PasswordQuestionField;
			}
			set {
				if ((object.ReferenceEquals (this.PasswordQuestionField, value) != true)) {
					this.PasswordQuestionField = value;
					this.RaisePropertyChanged ("PasswordQuestion");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal string UserKey {
			get {
				return this.UserKeyField;
			}
			set {
				if ((object.ReferenceEquals (this.UserKeyField, value) != true)) {
					this.UserKeyField = value;
					this.RaisePropertyChanged ("UserKey");
				}
			}
		}
		[System.Runtime.Serialization.DataMemberAttribute ()]
		internal string UserName {
			get {
				return this.UserNameField;
			}
			set {
				if ((object.ReferenceEquals (this.UserNameField, value) != true)) {
					this.UserNameField = value;
					this.RaisePropertyChanged ("UserName");
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
	[System.Runtime.Serialization.DataContractAttribute (Name = "UserCreateStatus", Namespace = "http://schemas.datacontract.org/2004/07/Colosoft.Security")]
	internal enum UserCreateStatus : int
	{
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		Success = 0,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		InvalidUserName = 1,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		InvalidPassword = 2,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		InvalidQuestion = 3,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		InvalidAnswer = 4,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		InvalidEmail = 5,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		DuplicateUserName = 6,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		DuplicateEmail = 7,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		UserRejected = 8,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		InvalidProviderUserKey = 9,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		DuplicateProviderUserKey = 10,
		[System.Runtime.Serialization.EnumMemberAttribute ()]
		ProviderError = 11,
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	[System.ServiceModel.ServiceContractAttribute (ConfigurationName = "ServerHost.IUserProviderService")]
	internal interface IUserProviderService	{
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IUserProviderService/GetProviderInfo", ReplyAction = "http://tempuri.org/IUserProviderService/GetProviderInfoResponse")]
		Colosoft.Security.Remote.Client.ServerHost.UserProviderInfo GetProviderInfo ();
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IUserProviderService/CreateUser", ReplyAction = "http://tempuri.org/IUserProviderService/CreateUserResponse")]
		Colosoft.Security.Remote.Client.ServerHost.CreateUserResult CreateUser (string a, string b, string c, string d, string e, string f, bool g, string h, string i, bool j, SecurityParameter[] k);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IUserProviderService/ChangePasswordQuestionAndAnswer", ReplyAction = "http://tempuri.org/IUserProviderService/ChangePasswordQuestionAndAnswerResponse")]
		bool ChangePasswordQuestionAndAnswer (string a, string b, string c, string d);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IUserProviderService/ResetPassword", ReplyAction = "http://tempuri.org/IUserProviderService/ResetPasswordResponse")]
		string ResetPassword (string a, string b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IUserProviderService/GetUser", ReplyAction = "http://tempuri.org/IUserProviderService/GetUserResponse")]
		Colosoft.Security.Remote.Client.ServerHost.User GetUser (string a, bool b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IUserProviderService/GetUserByKey", ReplyAction = "http://tempuri.org/IUserProviderService/GetUserByKeyResponse")]
		Colosoft.Security.Remote.Client.ServerHost.User GetUserByKey (string a, bool b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IUserProviderService/GetUserByToken", ReplyAction = "http://tempuri.org/IUserProviderService/GetUserByTokenResponse")]
		Colosoft.Security.Remote.Client.ServerHost.User GetUserByToken (string a, bool b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IUserProviderService/UpdateUser", ReplyAction = "http://tempuri.org/IUserProviderService/UpdateUserResponse")]
		void UpdateUser (Colosoft.Security.Remote.Client.ServerHost.User a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IUserProviderService/GetNumberOfUsersOnline", ReplyAction = "http://tempuri.org/IUserProviderService/GetNumberOfUsersOnlineResponse")]
		int GetNumberOfUsersOnline ();
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IUserProviderService/GetIdentityProviders", ReplyAction = "http://tempuri.org/IUserProviderService/GetIdentityProvidersResponse")]
		string[] GetIdentityProviders ();
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	internal interface IUserProviderServiceChannel : Colosoft.Security.Remote.Client.ServerHost.IUserProviderService, System.ServiceModel.IClientChannel	{
	}
	[System.Diagnostics.DebuggerStepThroughAttribute ()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	internal partial class UserProviderServiceClient : System.ServiceModel.ClientBase<Colosoft.Security.Remote.Client.ServerHost.IUserProviderService>, Colosoft.Security.Remote.Client.ServerHost.IUserProviderService	{
		public UserProviderServiceClient ()		{
		}
		public UserProviderServiceClient (string a) : base (a)		{
		}
		public UserProviderServiceClient (string a, string b) : base (a, b)		{
		}
		public UserProviderServiceClient (string a, System.ServiceModel.EndpointAddress b) : base (a, b)		{
		}
		public UserProviderServiceClient (System.ServiceModel.Channels.Binding a, System.ServiceModel.EndpointAddress b) : base (a, b)		{
		}
		public Colosoft.Security.Remote.Client.ServerHost.UserProviderInfo GetProviderInfo ()		{
			return base.Channel.GetProviderInfo ();
		}
		public Colosoft.Security.Remote.Client.ServerHost.CreateUserResult CreateUser (string a, string b, string c, string d, string e, string f, bool g, string h, string i, bool j, SecurityParameter[] k)		{
			return base.Channel.CreateUser (a, b, c, d, e, f, g, h, i, j, k);
		}
		public bool ChangePasswordQuestionAndAnswer (string a, string b, string c, string d)		{
			return base.Channel.ChangePasswordQuestionAndAnswer (a, b, c, d);
		}
		public string ResetPassword (string a, string b)		{
			return base.Channel.ResetPassword (a, b);
		}
		public Colosoft.Security.Remote.Client.ServerHost.User GetUser (string a, bool b)		{
			return base.Channel.GetUser (a, b);
		}
		public Colosoft.Security.Remote.Client.ServerHost.User GetUserByKey (string a, bool b)		{
			return base.Channel.GetUserByKey (a, b);
		}
		public Colosoft.Security.Remote.Client.ServerHost.User GetUserByToken (string a, bool b)		{
			return base.Channel.GetUserByToken (a, b);
		}
		public void UpdateUser (Colosoft.Security.Remote.Client.ServerHost.User a)		{
			base.Channel.UpdateUser (a);
		}
		public int GetNumberOfUsersOnline ()		{
			return base.Channel.GetNumberOfUsersOnline ();
		}
		public string[] GetIdentityProviders ()		{
			return base.Channel.GetIdentityProviders ();
		}
	}
}
