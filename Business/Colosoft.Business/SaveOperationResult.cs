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
	/// Assinatura do método usado para salvar os dados de uma operação.
	/// </summary>
	/// <param name="session"></param>
	/// <returns></returns>
	public delegate SaveResult SaveOperationHandler (Colosoft.Data.IPersistenceSession session);
	/// <summary>
	/// Implementação do resultado da uma operação de save.
	/// </summary>
	public class SaveOperationResult : OperationResult
	{
		private SaveOperationHandler _save;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="success">Identifica se a operação foi executa com sucesso.</param>
		/// <param name="message">Mensagem da operação.</param>
		/// <param name="save">Comando que será usado para execução.</param>
		public SaveOperationResult(bool success, IMessageFormattable message, SaveOperationHandler save = null) : base(success, message)
		{
			_save = save;
		}

		/// <summary>
		/// Salva os dados do resultad.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public SaveResult Save(Colosoft.Data.IPersistenceSession session)
		{
			if(_save == null)
				return new SaveResult(true, null);
			return _save(session);
		}
	}
}
