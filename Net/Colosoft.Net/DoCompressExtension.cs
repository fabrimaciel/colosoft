﻿/* 
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
using System.ServiceModel;
using System.Text;

namespace Colosoft.Net
{
	/// <summary>
	/// Implementação da extensão para compressão.
	/// </summary>
	public class DoCompressExtension : IExtension<OperationContext>
	{
		/// <summary>
		/// Anexa o contexto.
		/// </summary>
		/// <param name="owner"></param>
		public void Attach(OperationContext owner)
		{
		}

		/// <summary>
		/// Desanexa o contexto.
		/// </summary>
		/// <param name="owner"></param>
		public void Detach(OperationContext owner)
		{
		}
	}
	/// <summary>
	/// Implementação da extensão para compressão.
	/// </summary>
	public class DoCompressPlusExtension : IExtension<OperationContext>
	{
		/// <summary>
		/// Anexa o contexto.
		/// </summary>
		/// <param name="owner"></param>
		public void Attach(OperationContext owner)
		{
		}

		/// <summary>
		/// Desanexa o contexto.
		/// </summary>
		/// <param name="owner"></param>
		public void Detach(OperationContext owner)
		{
		}
	}
}
