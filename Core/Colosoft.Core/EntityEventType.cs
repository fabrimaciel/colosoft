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

namespace Colosoft.Domain
{
	/// <summary>
	/// Possíveis tipos de eventos das entidades.
	/// </summary>
	public enum EntityEventType
	{
		/// <summary>
		/// Representa o evento acionado quando a entidade for inicializada.
		/// </summary>
		Initialized = 1,
		/// <summary>
		/// Representa o evento acionado quando uma propriedade da entidade
		/// estiver sendo alterada.
		/// </summary>
		PropertyChanging,
		/// <summary>
		/// Representa o evento acionado quando uma propriedade da entidade
		/// for alterada.
		/// </summary>
		PropertyChanged,
		/// <summary>
		/// Representa o evento acionado quando a entidade estiver sendo validada.
		/// </summary>
		Validating,
		/// <summary>
		/// Representa o evento acionado quando a entidade for validada.
		/// </summary>
		Validated,
		/// <summary>
		/// Representa o evento acionado quando a entidade estiver sendo salva.
		/// </summary>
		Saving,
		/// <summary>
		/// Representa o evento acionado quando a entidade for salva.
		/// </summary>
		Saved,
		/// <summary>
		/// Representa o evento acionado quando a entidade estiver sendo apagada.
		/// </summary>
		Deleting,
		/// <summary>
		/// Representa o evento acionado quando a entidade for salva.
		/// </summary>
		Deleted
	}
}
