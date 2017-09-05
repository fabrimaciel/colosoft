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
	/// Prioridades do expedidor.
	/// </summary>
	public enum DispatcherPriority
	{
		/// <summary>
		/// Prioridade de aplicação inativa.
		/// </summary>
		ApplicationIdle = 2,
		/// <summary>
		/// Prioridade de plano de fundo.
		/// </summary>
		Background = 4,
		/// <summary>
		/// Prioridade de contexto inativo.
		/// </summary>
		ContextIdle = 3,
		/// <summary>
		/// Prioridade de databinding.
		/// </summary>
		DataBind = 8,
		/// <summary>
		/// Prioridade de inativo.
		/// </summary>
		Inactive = 0,
		/// <summary>
		/// Prioridade de entrada.
		/// </summary>
		Input = 5,
		/// <summary>
		/// Inválida.
		/// </summary>
		Invalid = -1,
		/// <summary>
		/// Prioridade de carregado,
		/// </summary>
		Loaded = 6,
		/// <summary>
		/// Prioridade normal.
		/// </summary>
		Normal = 9,
		/// <summary>
		/// Prioridade de renderização.
		/// </summary>
		Render = 7,
		/// <summary>
		/// Prioridade de envio.
		/// </summary>
		Send = 10,
		/// <summary>
		/// Prioridade de sistema inativo.
		/// </summary>
		SystemIdle = 1
	}
}
