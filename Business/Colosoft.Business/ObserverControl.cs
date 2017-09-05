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
	/// Representa um controle de observadores de eventos
	/// </summary>
	public class ObserverControl
	{
		/// <summary>
		/// Lista das propriedades que foram alteradas
		/// </summary>
		private List<string> _changedProperties = new List<string>();

		/// <summary>
		/// Lista de observadores das alterações de propriedades
		/// </summary>
		private List<IEntityObserver> _observers = new List<IEntityObserver>();

		/// <summary>
		/// Indica se há ou não observadores de evento registrado
		/// </summary>
		public bool HasObserver
		{
			get
			{
				return _observers.Count > 0;
			}
		}

		/// <summary>
		/// Lista das propriedades que foram alteradas
		/// </summary>
		public List<string> ChangedProperties
		{
			get
			{
				return _changedProperties;
			}
		}

		/// <summary>
		/// Notifica os clientes de uma alteração de propriedade antes do evento acionado pelo .Net
		/// </summary>
		/// <param name="entity">IEntity entidade que foi alterada</param>
		public void StartFlush(IEntity entity)
		{
			var aux = entity as Entity;
			foreach (string propertyName in _changedProperties)
			{
				foreach (var observer in _observers)
					observer.OnPropertyChanged(entity, propertyName);
			}
		}

		/// <summary>
		/// Ocorre após notificar os observadores e os eventos do .Net
		/// </summary>
		/// <param name="entity">IEntity entidade que foi alterada</param>
		public void EndFlush(IEntity entity)
		{
			_changedProperties.Clear();
		}

		/// <summary>
		/// Registra uma alteração em uma propriedade
		/// </summary>
		/// <param name="propertyName">Nome da propriedade</param>
		public void RegisterNotify(string propertyName)
		{
			if(!_changedProperties.Contains(propertyName))
			{
				_changedProperties.Add(propertyName);
			}
		}

		/// <summary>
		/// Dispara o evento aos observadores
		/// </summary>
		/// <param name="entity">Entidade que gerou o evento</param>
		/// <param name="propertyName">Nome da propriedade que ocasionou o evento</param>
		public void Raise(Entity entity, string propertyName)
		{
			foreach (var observer in _observers)
				observer.OnPropertyChanged(entity, propertyName);
		}

		/// <summary>
		/// Adiciona um observador ao controle
		/// </summary>
		/// <param name="control">Controle base</param>
		/// <param name="newObserver">Novo observador</param>
		/// <returns>Controle base com observador anexado</returns>
		public static ObserverControl operator +(ObserverControl control, IEntityObserver newObserver)
		{
			if(control._observers == null)
			{
				control._observers = new List<IEntityObserver>();
			}
			control._observers.Add(newObserver);
			return control;
		}

		/// <summary>
		/// Remove um observador do controle
		/// </summary>
		/// <param name="control">Controle base</param>
		/// <param name="removeObserver">Observador a ser removido</param>
		/// <returns>Controle base sem o observador</returns>
		public static ObserverControl operator -(ObserverControl control, IEntityObserver removeObserver)
		{
			control._observers.Remove(removeObserver);
			return control;
		}
	}
}
