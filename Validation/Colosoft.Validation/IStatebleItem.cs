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
	/// Opções de visibilidade para itens com estado.
	/// </summary>
	public enum StatebleItemVisibility
	{
		/// <summary>
		/// Visível.
		/// </summary>
		Visible,
		/// <summary>
		/// Não exibe a propriedade, mas reserva o espaço no layout.
		/// </summary>
		Hidden,
		/// <summary>
		/// Não exibe a propriedade e não reserva o espaço no layout.
		/// </summary>
		Collapsed
	}
	/// <summary>
	/// Contrao para cada propriedade que controla estado
	/// </summary>
	public interface IStatebleItem : System.ComponentModel.INotifyPropertyChanged, IInputValidationInfo, ICloneable
	{
		/// <summary>
		/// Nome da propriedade
		/// </summary>
		string PropertyName
		{
			get;
		}

		/// <summary>
		/// Identificador da configuração.
		/// </summary>
		string Identifier
		{
			get;
			set;
		}

		/// <summary>
		/// Validações da propriedade
		/// </summary>
		IValidator Validation
		{
			get;
			set;
		}

		/// <summary>
		/// Labels da propriedade.
		/// </summary>
		IPropertyLabel Label
		{
			get;
			set;
		}

		/// <summary>
		/// Se o valor será copiado ao se copiar o ítem.
		/// </summary>
		bool CopyValue
		{
			get;
		}

		/// <summary>
		/// Indica que as propriedades devem ser recarregadas.
		/// </summary>
		bool ReloadSettings
		{
			get;
		}

		/// <summary>
		/// Opções de tela da propriedade.
		/// </summary>
		Colosoft.Validation.InputRulesOptions Options
		{
			get;
		}

		/// <summary>
		/// Conversor.
		/// </summary>
		IParse Parser
		{
			get;
		}

		/// <summary>
		/// Indica se foi ou não configurado.
		/// </summary>
		bool IsConfigured
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se o item é somente leitura.
		/// </summary>
		bool IsReadOnly
		{
			get;
			set;
		}

		/// <summary>
		/// Indica se o item está habilitado.
		/// </summary>
		bool IsEnabled
		{
			get;
			set;
		}

		/// <summary>
		/// Indica se a lista de valores do item deve ser ordenada.
		/// </summary>
		bool IsSorted
		{
			get;
			set;
		}

		/// <summary>
		/// Indica se o item tem preenchimento válido obrigatório.
		/// </summary>
		bool IsRequired
		{
			get;
			set;
		}

		/// <summary>
		/// Indica se o item é obrigatório.
		/// </summary>
		bool IsNecessary
		{
			get;
			set;
		}

		/// <summary>
		/// Indica que o item deve estar oculto.
		/// </summary>
		bool IsHidden
		{
			get;
			set;
		}

		/// <summary>
		/// Opções de visibilidade do item.
		/// </summary>
		StatebleItemVisibility Visibility
		{
			get;
			set;
		}

		/// <summary>
		/// Instancia do pai do item.
		/// </summary>
		IStateble Owner
		{
			get;
		}

		/// <summary>
		/// Verifica se o item não tem validações.
		/// </summary>
		/// <returns></returns>
		bool IsEmpty();

		/// <summary>
		/// Copia as informações de outra instância.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		IStatebleItem CopyFrom(IStatebleItem instance);
	}
}
