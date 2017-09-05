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

namespace Colosoft.Security.IdentityModel
{
	/// <summary>
	/// Validador de usuário usado pelos serviços.
	/// </summary>
	public class DefaultUserNameValidator : System.IdentityModel.Selectors.UserNamePasswordValidator
	{
		/// <summary>
		/// Valida os dados do usuário.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		public override void Validate(string userName, string password)
		{
			if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
				throw new ArgumentNullException();
			IValidateUserResult result = null;
			try
			{
				result = Membership.ValidateUser(userName, password);
			}
			catch(Exception ex)
			{
				var ex2 = ex;
				if(ex2 is System.Reflection.TargetInvocationException)
					ex2 = ex2.InnerException;
				Log.Write(ResourceMessageFormatter.Create(() => Colosoft.Properties.Resources.Exception_FailOnValidateUser), ex2, Logging.Priority.Medium);
				throw ex2;
			}
			if(result.Status != AuthenticationStatus.Success)
				throw new System.ServiceModel.FaultException("Unknown Username or Incorrect Password");
		}
	}
}
