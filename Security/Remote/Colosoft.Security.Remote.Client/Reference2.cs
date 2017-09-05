namespace Colosoft.Security.Remote.Client.RoleProviderServiceReference{
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	[System.ServiceModel.ServiceContractAttribute (ConfigurationName = "RoleProviderServiceReference.IRoleProviderService")]
	internal interface IRoleProviderService	{
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/CreateRole", ReplyAction = "http://tempuri.org/IRoleProviderService/CreateRoleResponse")]
		void CreateRole (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/DeleteRole", ReplyAction = "http://tempuri.org/IRoleProviderService/DeleteRoleResponse")]
		bool DeleteRole (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/GetRolesForUser", ReplyAction = "http://tempuri.org/IRoleProviderService/GetRolesForUserResponse")]
		string[] GetRolesForUser (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/GetExclusiveRolesForUser", ReplyAction = "http://tempuri.org/IRoleProviderService/GetExclusiveRolesForUserResponse")]
		string[] GetExclusiveRolesForUser (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/GetUsersInRole", ReplyAction = "http://tempuri.org/IRoleProviderService/GetUsersInRoleResponse")]
		string[] GetUsersInRole (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/IsUserInRole", ReplyAction = "http://tempuri.org/IRoleProviderService/IsUserInRoleResponse")]
		bool IsUserInRole (string a, string b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/RoleExists", ReplyAction = "http://tempuri.org/IRoleProviderService/RoleExistsResponse")]
		bool RoleExists (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/AddUsersToRoles", ReplyAction = "http://tempuri.org/IRoleProviderService/AddUsersToRolesResponse")]
		void AddUsersToRoles (string[] a, string[] b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/FindUsersInRole", ReplyAction = "http://tempuri.org/IRoleProviderService/FindUsersInRoleResponse")]
		string[] FindUsersInRole (string a, string b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/RemoveUsersFromRoles", ReplyAction = "http://tempuri.org/IRoleProviderService/RemoveUsersFromRolesResponse")]
		void RemoveUsersFromRoles (string[] a, string[] b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/GetAllRoles", ReplyAction = "http://tempuri.org/IRoleProviderService/GetAllRolesResponse")]
		string[] GetAllRoles ();
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/CreateRoleGroup", ReplyAction = "http://tempuri.org/IRoleProviderService/CreateRoleGroupResponse")]
		void CreateRoleGroup (string a, string[] b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/DeleteRoleGroup", ReplyAction = "http://tempuri.org/IRoleProviderService/DeleteRoleGroupResponse")]
		void DeleteRoleGroup (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/ExistsRoleGroup", ReplyAction = "http://tempuri.org/IRoleProviderService/ExistsRoleGroupResponse")]
		bool ExistsRoleGroup (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/AddRolesToRoleGroup", ReplyAction = "http://tempuri.org/IRoleProviderService/AddRolesToRoleGroupResponse")]
		void AddRolesToRoleGroup (string a, string[] b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/RemoveRolesFromRoleGroup", ReplyAction = "http://tempuri.org/IRoleProviderService/RemoveRolesFromRoleGroupResponse")]
		void RemoveRolesFromRoleGroup (string[] a, string[] b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/GetRolesForRoleGroup", ReplyAction = "http://tempuri.org/IRoleProviderService/GetRolesForRoleGroupResponse")]
		string[] GetRolesForRoleGroup (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/RoleGroupExists", ReplyAction = "http://tempuri.org/IRoleProviderService/RoleGroupExistsResponse")]
		bool RoleGroupExists (string a);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/GetAllRoleGroups", ReplyAction = "http://tempuri.org/IRoleProviderService/GetAllRoleGroupsResponse")]
		string[] GetAllRoleGroups ();
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/AddUsersToRoleGroup", ReplyAction = "http://tempuri.org/IRoleProviderService/AddUsersToRoleGroupResponse")]
		void AddUsersToRoleGroup (string[] a, string[] b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/RemoveUsersFromRoleGroup", ReplyAction = "http://tempuri.org/IRoleProviderService/RemoveUsersFromRoleGroupResponse")]
		void RemoveUsersFromRoleGroup (string[] a, string[] b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/FindUsersInRoleGroup", ReplyAction = "http://tempuri.org/IRoleProviderService/FindUsersInRoleGroupResponse")]
		string[] FindUsersInRoleGroup (string a, string b);
		[System.ServiceModel.OperationContractAttribute (Action = "http://tempuri.org/IRoleProviderService/IsUserInRoleGroup", ReplyAction = "http://tempuri.org/IRoleProviderService/IsUserInRoleGroupResponse")]
		bool IsUserInRoleGroup (string a, string b);
	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	internal interface IRoleProviderServiceChannel : Colosoft.Security.Remote.Client.RoleProviderServiceReference.IRoleProviderService, System.ServiceModel.IClientChannel	{
	}
	[System.Diagnostics.DebuggerStepThroughAttribute ()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute ("System.ServiceModel", "4.0.0.0")]
	internal partial class RoleProviderServiceClient : System.ServiceModel.ClientBase<Colosoft.Security.Remote.Client.RoleProviderServiceReference.IRoleProviderService>, Colosoft.Security.Remote.Client.RoleProviderServiceReference.IRoleProviderService	{
		public RoleProviderServiceClient ()		{
		}
		public RoleProviderServiceClient (string a) : base (a)		{
		}
		public RoleProviderServiceClient (string a, string b) : base (a, b)		{
		}
		public RoleProviderServiceClient (string a, System.ServiceModel.EndpointAddress b) : base (a, b)		{
		}
		public RoleProviderServiceClient (System.ServiceModel.Channels.Binding a, System.ServiceModel.EndpointAddress b) : base (a, b)		{
		}
		public void CreateRole (string a)		{
			base.Channel.CreateRole (a);
		}
		public bool DeleteRole (string a)		{
			return base.Channel.DeleteRole (a);
		}
		public string[] GetRolesForUser (string a)		{
			return base.Channel.GetRolesForUser (a);
		}
		public string[] GetExclusiveRolesForUser (string a)		{
			return base.Channel.GetExclusiveRolesForUser (a);
		}
		public string[] GetUsersInRole (string a)		{
			return base.Channel.GetUsersInRole (a);
		}
		public bool IsUserInRole (string a, string b)		{
			return base.Channel.IsUserInRole (a, b);
		}
		public bool RoleExists (string a)		{
			return base.Channel.RoleExists (a);
		}
		public void AddUsersToRoles (string[] a, string[] b)		{
			base.Channel.AddUsersToRoles (a, b);
		}
		public string[] FindUsersInRole (string a, string b)		{
			return base.Channel.FindUsersInRole (a, b);
		}
		public void RemoveUsersFromRoles (string[] a, string[] b)		{
			base.Channel.RemoveUsersFromRoles (a, b);
		}
		public string[] GetAllRoles ()		{
			return base.Channel.GetAllRoles ();
		}
		public void CreateRoleGroup (string a, string[] b)		{
			base.Channel.CreateRoleGroup (a, b);
		}
		public void DeleteRoleGroup (string a)		{
			base.Channel.DeleteRoleGroup (a);
		}
		public bool ExistsRoleGroup (string a)		{
			return base.Channel.ExistsRoleGroup (a);
		}
		public void AddRolesToRoleGroup (string a, string[] b)		{
			base.Channel.AddRolesToRoleGroup (a, b);
		}
		public void RemoveRolesFromRoleGroup (string[] a, string[] b)		{
			base.Channel.RemoveRolesFromRoleGroup (a, b);
		}
		public string[] GetRolesForRoleGroup (string a)		{
			return base.Channel.GetRolesForRoleGroup (a);
		}
		public bool RoleGroupExists (string a)		{
			return base.Channel.RoleGroupExists (a);
		}
		public string[] GetAllRoleGroups ()		{
			return base.Channel.GetAllRoleGroups ();
		}
		public void AddUsersToRoleGroup (string[] a, string[] b)		{
			base.Channel.AddUsersToRoleGroup (a, b);
		}
		public void RemoveUsersFromRoleGroup (string[] a, string[] b)		{
			base.Channel.RemoveUsersFromRoleGroup (a, b);
		}
		public string[] FindUsersInRoleGroup (string a, string b)		{
			return base.Channel.FindUsersInRoleGroup (a, b);
		}
		public bool IsUserInRoleGroup (string a, string b)		{
			return base.Channel.IsUserInRoleGroup (a, b);
		}
	}
}
