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
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security.Principal;

namespace Colosoft.Security.Policy
{
	/// <summary>
	/// Politica de autorização do serviço.
	/// </summary>
	public class AuthorizationPolicy : IAuthorizationPolicy
	{
		private readonly ClaimSet _issuer;

		private readonly Guid _id;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public AuthorizationPolicy()
		{
			_id = Guid.NewGuid();
			_issuer = ClaimSet.System;
		}

		private IIdentity GetClientIdentity(EvaluationContext evaluationContext)
		{
			object obj;
			if(!evaluationContext.Properties.TryGetValue("Identities", out obj))
				throw new Exception("No Identity found");
			IList<IIdentity> identities = obj as IList<IIdentity>;
			if(identities == null || identities.Count == 0)
				throw new Exception("No Identity found");
			var identity = identities.Where(f => f is UserIdentity).FirstOrDefault();
			if(identity == null)
			{
				identity = new UserIdentity(identities[0]);
				identities.RemoveAt(0);
				identities.Insert(0, identity);
			}
			return identity;
		}

		/// <summary>
		/// Executa a politica.
		/// </summary>
		/// <param name="evaluationContext"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public bool Evaluate(EvaluationContext evaluationContext, ref object state)
		{
			var client = GetClientIdentity(evaluationContext);
			evaluationContext.Properties["Principal"] = new UserIdentity(client);
			return true;
		}

		/// <summary>
		/// Emissor da autorização.
		/// </summary>
		public System.IdentityModel.Claims.ClaimSet Issuer
		{
			get
			{
				return _issuer;
			}
		}

		/// <summary>
		/// Identificador da política.
		/// </summary>
		public string Id
		{
			get
			{
				return _id.ToString();
			}
		}
	}
}
