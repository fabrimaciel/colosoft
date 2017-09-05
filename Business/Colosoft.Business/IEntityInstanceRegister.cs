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

namespace Colosoft.Business
{
	/// <summary>
	/// Assinatura das classe usada pare recupera o Instance para uma entidade.
	/// </summary>
	public interface IEntityInstanceGetter
	{
		/// <summary>
		/// Identifica se a instância é valida.
		/// </summary>
		bool IsValid
		{
			get;
		}

		/// <summary>
		/// Recupera o instancia da entidade.
		/// </summary>
		/// <returns></returns>
		IEntity GetInstance();
	}
	/// <summary>
	/// Assinatura da classe de entidade que possui o suporte para registrar o instance.
	/// </summary>
	public interface IEntityInstanceRegister
	{
		/// <summary>
		/// Instancia do getter.
		/// </summary>
		IEntityInstanceGetter InstanceGetter
		{
			get;
		}

		/// <summary>
		/// Registra a instancia da entidade.
		/// </summary>
		/// <param name="instanceGetter">Instancia responsável por recuperar o Instance.</param>
		void Register(IEntityInstanceGetter instanceGetter);
	}
}
