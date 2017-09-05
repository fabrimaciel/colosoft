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
using System.ComponentModel.Composition.Primitives;

namespace Colosoft.Mef
{
	/// <summary>
	/// Define um export requerido por uma instancia <see cref="ProviderComposablePart"/>.
	/// </summary>
	public class ProviderExportDefinition : ExportDefinition
	{
		private Func<System.Reflection.MemberInfo> _memberGetter;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="memberGetter">Referencia do método usado para recuperar o membro.</param>
		/// <param name="exportDescription"></param>
		/// <param name="creationPolicy">Política de criação.</param>
		public ProviderExportDefinition(Func<System.Reflection.MemberInfo> memberGetter, ExportDescription exportDescription, System.ComponentModel.Composition.CreationPolicy creationPolicy) : base(CompositionServices.GetContractNameFromExportDescription(memberGetter, exportDescription), CompositionServices.GetMetadataFromExportDescription(memberGetter, exportDescription, creationPolicy))
		{
			memberGetter.Require("member").NotNull();
			exportDescription.Require("exportDescription").NotNull();
			_memberGetter = memberGetter;
		}

		/// <summary>
		/// Instancia do membro associado.
		/// </summary>
		public System.Reflection.MemberInfo Member
		{
			get
			{
				return _memberGetter();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} (ContractName=\"{1}\")", Member != null ? Member.Name : "", ContractName);
		}
	}
}
