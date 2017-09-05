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
	/// Implementação de uma lista de validações.
	/// </summary>
	public class ValidationList : Colosoft.Collections.BaseObservableCollection<IValidator>
	{
		/// <summary>
		/// Construto da classe
		/// </summary>
		public ValidationList() : base()
		{
		}

		/// <summary>
		/// Cosntrutor da classe com inicialização de um IList que será observado
		/// </summary>
		/// <param name="list">IList a observar</param>
		public ValidationList(IList<IValidator> list) : base(list)
		{
			CheckExclusive(list);
		}

		/// <summary>
		/// Cosntrutor da classe com inicialização de um IEnumerable que será observado
		/// </summary>
		/// <param name="collection">Enumerador</param>
		public ValidationList(IEnumerable<IValidator> collection) : base(collection)
		{
			CheckExclusive(collection.ToList());
		}

		/// <summary>
		/// Define o item para a posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void SetItem(int index, IValidator item)
		{
			CheckExclusive(item);
			base.SetItem(index, item);
		}

		/// <summary>
		/// Insere um ítem no índice especificado
		/// </summary>
		/// <param name="index">Índice</param>
		/// <param name="item">Ítem</param>
		protected override void InsertItem(int index, IValidator item)
		{
			CheckExclusive(item);
			base.Insert(index, item);
		}

		/// <summary>
		/// Remove todas as validações de um determinado tipo da lista
		/// </summary>
		/// <param name="T">Tipo a ser removido</param>
		/// <returns>Sucesso da operação</returns>
		public bool RemoveByType(Type T)
		{
			bool result = true;
			IEnumerable<IValidator> valitorToRemove = this.Where(f => f.GetType() == T);
			foreach (IValidator validator in valitorToRemove)
				result &= this.Remove(validator);
			return result;
		}

		/// <summary>
		/// Remove todas as validações de um determinado nome
		/// </summary>
		/// <param name="validatorName">Nome da validação a ser removida</param>
		/// <returns>Sucesso da operação</returns>
		public bool RemoveByName(string validatorName)
		{
			bool result = true;
			IEnumerable<IValidator> valitorToRemove = this.Where(f => f.FullName == validatorName);
			foreach (IValidator validator in valitorToRemove)
				result &= this.Remove(validator);
			return result;
		}

		/// <summary>
		/// Verifica se já existe uma validação com o mesmo nome na lista
		/// </summary>
		/// <param name="validatorName">Nome da validação</param>
		/// <returns>Verdadeiro se existir</returns>
		public bool ContainsName(string validatorName)
		{
			return (this.Where(f => f.FullName == validatorName).Count() > 0);
		}

		/// <summary>
		/// Verifica se já existe uma validação com o mesmo tipo na lista
		/// </summary>
		/// <param name="T">Tipo a ser verificado</param>
		/// <returns>Verdadeiro se existir</returns>
		public bool ContainsType(Type T)
		{
			return (this.Where(f => f.GetType() == T).Count() > 0);
		}

		/// <summary>
		/// Checa a exclusividade da validação a ser inserida na lista
		/// </summary>
		/// <param name="item">Validação a ser inserida</param>
		private void CheckExclusive(IValidator item)
		{
			if(item.IsExclusiveInList)
				this.RemoveByType(item.GetType());
		}

		/// <summary>
		/// Checa a exclusividade da validação na lista
		/// </summary>
		/// <param name="list">Lista a ser validada</param>
		private void CheckExclusive(IList<IValidator> list)
		{
			for(int index = 0; index < list.Count; index++)
			{
				if(list[index].IsExclusiveInList)
				{
					for(int index2 = index + 1; index2 < list.Count; index2++)
					{
						if(list[index].GetType() == list[index2].GetType())
							throw new Exception(Properties.Resources.Error_ExclusiveValidatioTypeDuplicatedInList);
					}
				}
			}
		}
	}
}
