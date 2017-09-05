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
	/// Assinatura das classes responsáveis por criar <see cref="IEntityDescriptor"/>.
	/// </summary>
	public interface IEntityDescriptorCreator : Colosoft.Query.IQueryResultObjectCreator
	{
		/// <summary>
		/// Cria o descritor da entidade.
		/// </summary>
		/// <returns></returns>
		IEntityDescriptor CreateEntityDescriptor();
	}
	/// <summary>
	/// Implementação básico do criador de <see cref="IEntityDescriptor"/>
	/// </summary>
	class EntityDescriptorCreator : IEntityDescriptorCreator, Query.IQueryResultObjectCreator
	{
		private IEntityLoader _loader;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="loader">Instancia do loader da entidade associada.</param>
		public EntityDescriptorCreator(IEntityLoader loader)
		{
			loader.Require("loader").NotNull();
			_loader = loader;
		}

		/// <summary>
		/// Cria uma nova instancia para o descriptor.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IEntityDescriptor CreateEntityDescriptor()
		{
			return _loader.CreateEntityDescriptor();
		}

		/// <summary>
		/// Cria uma nova instancia.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public object Create()
		{
			return _loader.CreateEntityDescriptor();
		}
	}
}
