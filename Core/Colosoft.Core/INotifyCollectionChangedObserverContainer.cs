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

namespace Colosoft.Collections
{
	/// <summary>
	/// Possíveis escopos de vida o observer.
	/// </summary>
	public enum NotifyCollectionChangedObserverLiveScope
	{
		/// <summary>
		/// Idenfinido.
		/// </summary>
		None,
		/// <summary>
		/// Identifica que o observer irá sobreviver
		/// durante o tempo de vida da coleção.
		/// </summary>
		Instance
	}
	/// <summary>
	/// Assinatura do container 
	/// de observer que tratam as alterações feitas nas coleções.
	/// </summary>
	public interface INotifyCollectionChangedObserverContainer
	{
		/// <summary>
		/// Adiciona o observer para a instancia.
		/// </summary>
		/// <param name="observer"></param>
		/// <param name="liveScope">Escopo do observer.</param>
		void AddObserver(INotifyCollectionChangedObserver observer, NotifyCollectionChangedObserverLiveScope liveScope);

		/// <summary>
		/// Remove o observer da instancia.
		/// </summary>
		/// <param name="observer"></param>
		void RemoveObserver(INotifyCollectionChangedObserver observer);
	}
}
