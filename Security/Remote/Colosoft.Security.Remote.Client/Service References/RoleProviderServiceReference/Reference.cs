﻿/* 
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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Colosoft.Security.Remote.Client.RoleProviderServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="RoleProviderServiceReference.IRoleProviderService")]
    internal interface IRoleProviderService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/CreateRole", ReplyAction="http://tempuri.org/IRoleProviderService/CreateRoleResponse")]
        void CreateRole(string roleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/DeleteRole", ReplyAction="http://tempuri.org/IRoleProviderService/DeleteRoleResponse")]
        bool DeleteRole(string roleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/GetRolesForUser", ReplyAction="http://tempuri.org/IRoleProviderService/GetRolesForUserResponse")]
        string[] GetRolesForUser(string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/GetExclusiveRolesForUser", ReplyAction="http://tempuri.org/IRoleProviderService/GetExclusiveRolesForUserResponse")]
        string[] GetExclusiveRolesForUser(string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/GetUsersInRole", ReplyAction="http://tempuri.org/IRoleProviderService/GetUsersInRoleResponse")]
        string[] GetUsersInRole(string roleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/IsUserInRole", ReplyAction="http://tempuri.org/IRoleProviderService/IsUserInRoleResponse")]
        bool IsUserInRole(string username, string roleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/RoleExists", ReplyAction="http://tempuri.org/IRoleProviderService/RoleExistsResponse")]
        bool RoleExists(string roleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/AddUsersToRoles", ReplyAction="http://tempuri.org/IRoleProviderService/AddUsersToRolesResponse")]
        void AddUsersToRoles(string[] usernames, string[] roleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/FindUsersInRole", ReplyAction="http://tempuri.org/IRoleProviderService/FindUsersInRoleResponse")]
        string[] FindUsersInRole(string roleName, string usernameToMatch);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/RemoveUsersFromRoles", ReplyAction="http://tempuri.org/IRoleProviderService/RemoveUsersFromRolesResponse")]
        void RemoveUsersFromRoles(string[] usernames, string[] roleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/GetAllRoles", ReplyAction="http://tempuri.org/IRoleProviderService/GetAllRolesResponse")]
        string[] GetAllRoles();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/CreateRoleGroup", ReplyAction="http://tempuri.org/IRoleProviderService/CreateRoleGroupResponse")]
        void CreateRoleGroup(string roleGroupName, string[] roles);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/DeleteRoleGroup", ReplyAction="http://tempuri.org/IRoleProviderService/DeleteRoleGroupResponse")]
        void DeleteRoleGroup(string roleGroupName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/ExistsRoleGroup", ReplyAction="http://tempuri.org/IRoleProviderService/ExistsRoleGroupResponse")]
        bool ExistsRoleGroup(string roleGroupName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/AddRolesToRoleGroup", ReplyAction="http://tempuri.org/IRoleProviderService/AddRolesToRoleGroupResponse")]
        void AddRolesToRoleGroup(string roleGroupName, string[] roleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/RemoveRolesFromRoleGroup", ReplyAction="http://tempuri.org/IRoleProviderService/RemoveRolesFromRoleGroupResponse")]
        void RemoveRolesFromRoleGroup(string[] roleGroupName, string[] roleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/GetRolesForRoleGroup", ReplyAction="http://tempuri.org/IRoleProviderService/GetRolesForRoleGroupResponse")]
        string[] GetRolesForRoleGroup(string roleGroupName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/RoleGroupExists", ReplyAction="http://tempuri.org/IRoleProviderService/RoleGroupExistsResponse")]
        bool RoleGroupExists(string roleGroupName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/GetAllRoleGroups", ReplyAction="http://tempuri.org/IRoleProviderService/GetAllRoleGroupsResponse")]
        string[] GetAllRoleGroups();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/AddUsersToRoleGroup", ReplyAction="http://tempuri.org/IRoleProviderService/AddUsersToRoleGroupResponse")]
        void AddUsersToRoleGroup(string[] usernames, string[] roleGroupNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/RemoveUsersFromRoleGroup", ReplyAction="http://tempuri.org/IRoleProviderService/RemoveUsersFromRoleGroupResponse")]
        void RemoveUsersFromRoleGroup(string[] usernames, string[] roleGroupNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/FindUsersInRoleGroup", ReplyAction="http://tempuri.org/IRoleProviderService/FindUsersInRoleGroupResponse")]
        string[] FindUsersInRoleGroup(string roleGroupName, string usernameToMatch);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoleProviderService/IsUserInRoleGroup", ReplyAction="http://tempuri.org/IRoleProviderService/IsUserInRoleGroupResponse")]
        bool IsUserInRoleGroup(string username, string roleGroupName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal interface IRoleProviderServiceChannel : Colosoft.Security.Remote.Client.RoleProviderServiceReference.IRoleProviderService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal partial class RoleProviderServiceClient : System.ServiceModel.ClientBase<Colosoft.Security.Remote.Client.RoleProviderServiceReference.IRoleProviderService>, Colosoft.Security.Remote.Client.RoleProviderServiceReference.IRoleProviderService {
        
        public RoleProviderServiceClient() {
        }
        
        public RoleProviderServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public RoleProviderServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RoleProviderServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RoleProviderServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void CreateRole(string roleName) {
            base.Channel.CreateRole(roleName);
        }
        
        public bool DeleteRole(string roleName) {
            return base.Channel.DeleteRole(roleName);
        }
        
        public string[] GetRolesForUser(string username) {
            return base.Channel.GetRolesForUser(username);
        }
        
        public string[] GetExclusiveRolesForUser(string username) {
            return base.Channel.GetExclusiveRolesForUser(username);
        }
        
        public string[] GetUsersInRole(string roleName) {
            return base.Channel.GetUsersInRole(roleName);
        }
        
        public bool IsUserInRole(string username, string roleName) {
            return base.Channel.IsUserInRole(username, roleName);
        }
        
        public bool RoleExists(string roleName) {
            return base.Channel.RoleExists(roleName);
        }
        
        public void AddUsersToRoles(string[] usernames, string[] roleNames) {
            base.Channel.AddUsersToRoles(usernames, roleNames);
        }
        
        public string[] FindUsersInRole(string roleName, string usernameToMatch) {
            return base.Channel.FindUsersInRole(roleName, usernameToMatch);
        }
        
        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
            base.Channel.RemoveUsersFromRoles(usernames, roleNames);
        }
        
        public string[] GetAllRoles() {
            return base.Channel.GetAllRoles();
        }
        
        public void CreateRoleGroup(string roleGroupName, string[] roles) {
            base.Channel.CreateRoleGroup(roleGroupName, roles);
        }
        
        public void DeleteRoleGroup(string roleGroupName) {
            base.Channel.DeleteRoleGroup(roleGroupName);
        }
        
        public bool ExistsRoleGroup(string roleGroupName) {
            return base.Channel.ExistsRoleGroup(roleGroupName);
        }
        
        public void AddRolesToRoleGroup(string roleGroupName, string[] roleNames) {
            base.Channel.AddRolesToRoleGroup(roleGroupName, roleNames);
        }
        
        public void RemoveRolesFromRoleGroup(string[] roleGroupName, string[] roleNames) {
            base.Channel.RemoveRolesFromRoleGroup(roleGroupName, roleNames);
        }
        
        public string[] GetRolesForRoleGroup(string roleGroupName) {
            return base.Channel.GetRolesForRoleGroup(roleGroupName);
        }
        
        public bool RoleGroupExists(string roleGroupName) {
            return base.Channel.RoleGroupExists(roleGroupName);
        }
        
        public string[] GetAllRoleGroups() {
            return base.Channel.GetAllRoleGroups();
        }
        
        public void AddUsersToRoleGroup(string[] usernames, string[] roleGroupNames) {
            base.Channel.AddUsersToRoleGroup(usernames, roleGroupNames);
        }
        
        public void RemoveUsersFromRoleGroup(string[] usernames, string[] roleGroupNames) {
            base.Channel.RemoveUsersFromRoleGroup(usernames, roleGroupNames);
        }
        
        public string[] FindUsersInRoleGroup(string roleGroupName, string usernameToMatch) {
            return base.Channel.FindUsersInRoleGroup(roleGroupName, usernameToMatch);
        }
        
        public bool IsUserInRoleGroup(string username, string roleGroupName) {
            return base.Channel.IsUserInRoleGroup(username, roleGroupName);
        }
    }
}
