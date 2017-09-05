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
using System.ComponentModel;

namespace Colosoft.Validation
{
	/// <summary>
	/// Contrato para item de controle de estado
	/// </summary>
	public interface IStateble : IEnumerable<IStatebleItem>, System.ComponentModel.INotifyPropertyChanged, IDisposable
	{
		/// <summary>
		/// Evento acionado quando o estado for alterado.
		/// </summary>
		event StateChangedEventHandler StateChanged;

		/// <summary>
		/// Tipo base do tipo de controle de estado.
		/// </summary>
		Reflection.TypeName Type
		{
			get;
		}

		/// <summary>
		/// Identifica se está em estado de somente leitura.
		/// </summary>
		bool IsReadOnly
		{
			get;
		}

		/// <summary>
		/// Recupera o estado associado com a propriedade.0
		/// </summary>
		/// <param name="propertyName">Nome da propriedade na qual o estado está associado.</param>
		/// <returns></returns>
		IStatebleItem this[string propertyName]
		{
			get;
		}

		/// <summary>
		/// Parametros do controle de estado.
		/// </summary>
		StatebleParameterCollection Parameters
		{
			get;
		}

		/// <summary>
		/// Notifica que uma propriedade do estado foi alterada.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade.</param>
		void NotifyStatePropertyChanged(string propertyName);

		/// <summary>
		/// Recarrega as definições de validação de propriedades para a instância.
		/// </summary>
		void ReloadTypeSettings();

		/// <summary>
		/// Reinicia o cache em caso afirmativo.
		/// </summary>
		void ClearStateCache();
	}
}
