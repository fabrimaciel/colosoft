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

namespace Colosoft.Mef
{
	/// <summary>
	/// Representa o erro ocorrido ao carregar o parametro
	/// do import de um classe.
	/// </summary>
	[Serializable]
	public class ComposableMemberParameterException : Exception
	{
		/// <summary>
		/// Membro da composição.
		/// </summary>
		internal ComposableMember Member
		{
			get;
			private set;
		}

		/// <summary>
		/// Instancia do parametro.
		/// </summary>
		internal ProviderParameterImportDefinition Parameter
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ComposableMemberParameterException()
		{
		}

		/// <summary>
		/// Construtro padrão.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="parameter"></param>
		/// <param name="innerException"></param>
		internal ComposableMemberParameterException(ComposableMember member, ProviderParameterImportDefinition parameter, Exception innerException) : base(ResourceMessageFormatter.Create(() => Properties.Resources.ComposableMemberParameterExcepton_Message, parameter != null ? parameter.Parameter.Name : null, parameter != null ? parameter.Parameter.ParameterType.FullName : null, member != null ? member.DeclaringType.FullName : null).Format(), innerException)
		{
			Member = member;
			Parameter = parameter;
		}

		/// <summary>
		/// Recupera os dados da serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
