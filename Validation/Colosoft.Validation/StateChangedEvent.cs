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

namespace Colosoft.Validation
{
	/// <summary>
	/// Informações de chamada de código de tratamento de evento de alteração de estado de propriedade.
	/// </summary>
	public class StateChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Nome da propriedade cujo estado foi alterado.
		/// </summary>
		public string PropertyName
		{
			get;
			private set;
		}

		/// <summary>
		/// Nome do item do estado da propriedade que foi alterado.
		/// </summary>
		public string StateName
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor parametrizado
		/// </summary>
		/// <param name="property">Nome da propriedade afetada</param>
		/// <param name="state">Nome do estado alterado</param>
		public StateChangedEventArgs(string property, string state)
		{
			PropertyName = property;
			StateName = state;
		}
	}
	/// <summary>
	/// Chamada de código de tratamento de evento de alteração de estado de propriedade.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void StateChangedEventHandler (object sender, StateChangedEventArgs e);
}
