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
using Colosoft.Query;

namespace Colosoft.Business
{
	/// <summary>
	/// Implementação da sessão da estratégia de vinculação do EntityDescriptor.
	/// </summary>
	class EntityDescriptorQueryResultBindStrategySession : Colosoft.Query.IQueryResultBindStrategySession
	{
		private EntityDescriptorQueryResultBindStrategy _bindStrategy;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="bindStrategy"></param>
		public EntityDescriptorQueryResultBindStrategySession(EntityDescriptorQueryResultBindStrategy bindStrategy)
		{
			_bindStrategy = bindStrategy;
		}

		/// <summary>
		/// Preenche os dados do objeto com o conteudo do registro informado.
		/// </summary>
		/// <param name="record"></param>
		/// <param name="mode"></param>
		/// <param name="instance"></param>
		/// <returns>Nomes das propriedade alteradas.</returns>
		public IEnumerable<string> Bind(IRecord record, BindStrategyMode mode, ref object instance)
		{
			return _bindStrategy.Bind(record, mode, ref instance);
		}
	}
}
