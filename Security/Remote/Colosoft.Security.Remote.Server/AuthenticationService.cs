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
using Colosoft.Security;
using System.ServiceModel;

namespace Colosoft.Security.Remote.Server
{
	/// <summary>
	/// Implementação da classe do serviço de autenticação
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
	public class AuthenticationService : IAuthenticationService
	{
		/// <summary>
		/// Objeto auxiliar para lock
		/// </summary>
		private object _lockObject = new object();

		private RemoteServerConfigurationSection _configurationSection;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public AuthenticationService()
		{
			_configurationSection = System.Configuration.ConfigurationManager.GetSection("colosoft.security.remote.server") as RemoteServerConfigurationSection;
			if(_configurationSection == null)
				throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Colosoft.Security.Remote.Server.Properties.Resources.InvalidOperationException_ServerConfigurationNotFound).Format());
		}

		/// <summary>
		/// Valida o usuário no provedor de identidade
		/// </summary>
		/// <param name="userName">Nome de usuário</param>
		/// <param name="password">Senha</param>
		/// <param name="servicesContext">Nome do contexto de serviços que será usado na autenticação.</param>
		/// <param name="parameters">Demais informações necessárias</param>  
		/// <returns></returns>
		public ValidateUserResultWrapper ValidateUser(string userName, string password, string servicesContext, SecurityParameter[] parameters)
		{
			IValidateUserResult result = null;
			try
			{
				result = Membership.ValidateUser(userName, password, parameters);
			}
			catch(Exception ex)
			{
				result = new ValidateUserResult {
					Message = ex.Message,
					Status = AuthenticationStatus.ErrorInValidate
				};
			}
			LogRequest(userName, result.Status);
			Colosoft.Net.ServiceAddress userProviderServiceAddress = null;
			Colosoft.Net.ServiceAddress profileProviderServiceAddress = null;
			Colosoft.Net.ServiceAddress serviceAddressProviderServiceAddress = null;
			if(result.Status == AuthenticationStatus.PasswordWarning || result.Status == AuthenticationStatus.Success)
			{
				userProviderServiceAddress = GetServiceAddress(_configurationSection.UserProviderServiceName, servicesContext);
				profileProviderServiceAddress = GetServiceAddress(_configurationSection.ProfileProviderServiceName, servicesContext);
				serviceAddressProviderServiceAddress = GetServiceAddress(_configurationSection.ServiceAddressProviderServiceName, servicesContext);
			}
			return new ValidateUserResultWrapper(result, userProviderServiceAddress, profileProviderServiceAddress, serviceAddressProviderServiceAddress);
		}

		/// <summary>
		/// Valida os dados do token.
		/// </summary>
		/// <param name="token">Token que será validado.</param>
		/// <param name="servicesContext">Nome do contexto de serviços que será usado na autenticação.</param>
		/// <returns></returns>
		public ValidateUserResultWrapper ValidateToken(string token, string servicesContext)
		{
			var result = Membership.ValidateToken(token);
			Colosoft.Net.ServiceAddress userProviderServiceAddress = null;
			Colosoft.Net.ServiceAddress profileProviderServiceAddress = null;
			Colosoft.Net.ServiceAddress serviceAddressProviderServiceAddress = null;
			if(result.Status == AuthenticationStatus.PasswordWarning || result.Status == AuthenticationStatus.Success)
			{
				userProviderServiceAddress = GetServiceAddress(_configurationSection.UserProviderServiceName, servicesContext);
				profileProviderServiceAddress = GetServiceAddress(_configurationSection.ProfileProviderServiceName, servicesContext);
				serviceAddressProviderServiceAddress = GetServiceAddress(_configurationSection.ServiceAddressProviderServiceName, servicesContext);
			}
			return new ValidateUserResultWrapper(result, userProviderServiceAddress, profileProviderServiceAddress, serviceAddressProviderServiceAddress);
		}

		/// <summary>
		/// Desloga o usuário do sistema
		/// </summary>
		/// <param name="token">Token do usuário</param>
		/// <returns>Sucesso da operação</returns>
		public bool LogOut(string token)
		{
			return Membership.LogOut(token);
		}

		/// <summary>
		/// Recupera o endereço do serviço.
		/// </summary>
		/// <param name="serviceName">Nome do serviço que será pesquisado.</param>
		/// <param name="servicesContext">Nome do contexto de serviços que será usado na autenticação.</param>
		/// <returns></returns>
		private static Net.ServiceAddress GetServiceAddress(string serviceName, string servicesContext)
		{
			Colosoft.Net.IServiceAddressProvider serviceAddressProvider = null;
			try
			{
				serviceAddressProvider = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Colosoft.Net.IServiceAddressProvider>();
			}
			catch(Exception ex)
			{
				var message = ResourceMessageFormatter.Create(() => Properties.Resources.InvalidOperationException_FailOnGetServiceAddressProvider);
				Log.Write(message, Logging.Category.Exception, Logging.Priority.High);
				throw new InvalidOperationException(message.Format(), ex);
			}
			var serviceAddresses = serviceAddressProvider.GetServiceAddresses(serviceName, servicesContext);
			if(serviceAddresses.Length == 0)
			{
				var message = ResourceMessageFormatter.Create(() => Properties.Resources.Exception_NotFoundAddressesForService, serviceName);
				Log.Write(message, Logging.Category.Warn, Logging.Priority.High);
				throw new Exception(message.Format());
			}
			return serviceAddresses.First();
		}

		/// <summary>
		/// Verifica se um token está ou não válido
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="token">token</param>
		/// <returns>Objeto com o resultado da consulta</returns>
		public Security.TokenConsultResult Check(string userName, string token)
		{
			return Tokens.Check(token);
		}

		/// <summary>
		/// Altera o password do usuário
		/// </summary>
		/// <param name="userName">usuário</param>
		/// <param name="oldPassword">senha atual</param>
		/// <param name="newPassword">nova senha</param>
		/// <param name="parameters">demais parametros</param>
		/// <returns>resultado da operação</returns>
		public ChangePasswordResult ChangePassword(string userName, string oldPassword, string newPassword, SecurityParameter[] parameters)
		{
			return Membership.ChangePassword(userName, oldPassword, newPassword, parameters);
		}

		/// <summary>
		/// Inicia o processo de redefinição de senha
		/// </summary>
		/// <param name="userName">Nome do usuário</param>
		/// <returns>Resultado do processo</returns>
		public ResetPasswordProcessResult RequestPasswordReset(string userName)
		{
			return Membership.RequestPasswordReset(userName);
		}

		/// <summary>
		/// Registra o log.
		/// </summary>
		/// <param name="userName">Nome do usuário</param>
		/// <param name="status">Status da autenticação</param>
		private void LogRequest(string userName, AuthenticationStatus status)
		{
			try
			{
				var logger = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetAllInstances<Colosoft.Security.Authentication.Log.ILog>().FirstOrDefault();
				if(logger != null)
				{
					string ip = (OperationContext.Current.IncomingMessageProperties[System.ServiceModel.Channels.RemoteEndpointMessageProperty.Name] as System.ServiceModel.Channels.RemoteEndpointMessageProperty).Address;
					logger.LogRequest(userName, ip, (byte)status);
				}
			}
			catch
			{
			}
		}
	}
}
