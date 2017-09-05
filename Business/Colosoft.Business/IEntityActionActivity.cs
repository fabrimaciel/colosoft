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
	/// Representa o resultado do método acionado
	/// antes de salvar a entidade.
	/// </summary>
	public class BeforeEntityActionResult
	{
		/// <summary>
		/// Cancela a operação.
		/// </summary>
		public bool Cancel
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Assinatura das classes que tratam ações das entidades.
	/// </summary>
	public interface IEntityActionActivity
	{
		/// <summary>
		/// Método acionado antes de salva a entidade.
		/// </summary>
		/// <param name="entity"></param>
		BeforeEntityActionResult BeforeAction(IEntity entity);

		/// <summary>
		/// Método acionado depois de salvar a entidade.
		/// </summary>
		/// <param name="entity"></param>
		void AfterAction(IEntity entity);
	}
	/// <summary>
	/// Assinatura das classes que tratam ações das entidades.
	/// </summary>
	public interface IEntityActionActivity<TEntity> where TEntity : IEntity
	{
		/// <summary>
		/// Método acionado antes de salva a entidade.
		/// </summary>
		/// <param name="entity"></param>
		BeforeEntityActionResult BeforeAction(TEntity entity);

		/// <summary>
		/// Método acionado depois de salvar a entidade.
		/// </summary>
		/// <param name="entity"></param>
		void AfterAction(TEntity entity);
	}
}
