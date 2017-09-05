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

namespace Colosoft.Security.Profile
{
	/// <summary>
	/// Classe que prover métoso que trabalham com os modos de pesquisa do perfil.
	/// </summary>
	public static class ProfileSearchModeProvider
	{
		/// <summary>
		/// Recupera a descrição dos modos de pesquias.
		/// </summary>
		/// <returns></returns>
		public static Dictionary<ProfileSearchMode, IMessageFormattable> GetDescriptions()
		{
			var result = new Dictionary<ProfileSearchMode, IMessageFormattable>();
			result.Add(ProfileSearchMode.All, ResourceMessageFormatter.Create(() => Properties.Resources.ProfileSearchMode_All));
			result.Add(ProfileSearchMode.Source, ResourceMessageFormatter.Create(() => Properties.Resources.ProfileSearchMode_Source));
			result.Add(ProfileSearchMode.Self, ResourceMessageFormatter.Create(() => Properties.Resources.ProfileSearchMode_Self));
			result.Add(ProfileSearchMode.Seller, ResourceMessageFormatter.Create(() => Properties.Resources.ProfileSearchMode_Seller));
			result.Add(ProfileSearchMode.Intermediate, ResourceMessageFormatter.Create(() => Properties.Resources.ProfileSearchMode_Intermediate));
			return result;
		}
	}
	/// <summary>
	/// Modos de pesquisa que podem ser associados com o perfil.
	/// </summary>
	public enum ProfileSearchMode : byte
	{
		/// <summary>
		/// Pesquisa todos os itens sem restrição.
		/// </summary>
		All = 1,
		/// <summary>
		/// Pesquisa somente os itens associados com a origem do perfil.
		/// </summary>
		Source,
		/// <summary>
		/// Pesquisa somente os itens cadastrados pelo usuário.
		/// </summary>
		Self,
		/// <summary>
		/// Pesquisa somente os itens associados com o vendedor do perfil.
		/// </summary>
		Seller,
		/// <summary>
		/// Pesquisa somente os itens associados com o intermediador do perfil.
		/// </summary>
		Intermediate
	}
}
