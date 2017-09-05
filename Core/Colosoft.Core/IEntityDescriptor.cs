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

namespace Colosoft
{
	/// <summary>
	/// Interface de entidades simplificadas.
	/// </summary>
	public interface IEntityDescriptor : IStatusControl
	{
		/// <summary>
		/// Identificador da entidade.
		/// </summary>
		int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da entidade.
		/// </summary>
		string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Descrição da entidade.
		/// </summary>
		string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Configura o controle de status associado.
		/// </summary>
		/// <param name="isActive">Identifica se os dados estão ativos.</param>
		/// <param name="isExpired">Identifica se os dados foram expirados.</param>
		void ConfigureStatusControl(bool isActive, bool isExpired);
	}
	/// <summary>
	/// Assintura da classe de EntityDescriptor que armazena o estado
	/// os valores atribuídos a suas propriedades.
	/// </summary>
	public interface IEntityDescriptorStateble
	{
		/// <summary>
		/// Quantidade de campos registrados.
		/// </summary>
		int FieldsCount
		{
			get;
		}

		/// <summary>
		/// Recuperar o valor do campo informado.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		object this[string fieldName]
		{
			get;
		}

		/// <summary>
		/// Verifica se a instancia possui o campo com o nome  informado.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		bool Contains(string fieldName);

		/// <summary>
		/// Recupera os nomes dos campos associados.
		/// </summary>
		/// <returns></returns>
		string[] GetFields();

		/// <summary>
		/// Remove o campo associado com o nome informado.
		/// </summary>
		/// <param name="fieldName"></param>
		void Remove(string fieldName);

		/// <summary>
		/// Atualiza o valor do campo. Caso não existe é adicionado.
		/// </summary>
		/// <param name="fieldName">Nome do campo.</param>
		/// <param name="value">Valor que será atualizado.</param>
		void Update(string fieldName, object value);
	}
}
