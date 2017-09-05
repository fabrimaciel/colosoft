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

namespace Colosoft.Threading
{
	/// <summary>
	/// Assinatura do gerenciador dos despatchers do sistema.
	/// </summary>
	public interface IDispatcherManager
	{
		/// <summary>
		/// Recupera o <see cref="IDispatcher"/> da thread informada.
		/// </summary>
		/// <param name="thread">Thread de onde será obtido do dispatcher.</param>
		/// <returns></returns>
		IDispatcher FromThread(System.Threading.Thread thread);

		/// <summary>
		/// Processa todo as mensagens de tela que estão na fila de mensagens.
		/// </summary>
		void DoEvents();
	}
}
