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
using System.Text;
using System.Web;
using System.Security.Permissions;
using System.Collections;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Assinatura do evento acionado quando ocorrer a alteração
	/// de uma situação do DataSource.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void VirtualObjectDataSourceStatusEventHandler (object sender, VirtualObjectDataSourceStatusEventArgs e);
	/// <summary>
	/// Assinatura usada para permitir a alteração do valor de retorno.
	/// </summary>
	public interface IVirtualObjectDataSourceStatusEventArgsExtended
	{
		/// <summary>
		/// Altera o valor do retorno.
		/// </summary>
		/// <param name="value"></param>
		void ChangeReturnValue(object value);
	}
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class VirtualObjectDataSourceStatusEventArgs : EventArgs, IVirtualObjectDataSourceStatusEventArgsExtended
	{
		private int _affectedRows;

		private Exception _exception;

		private bool _exceptionHandled;

		private IDictionary _outputParameters;

		private object _returnValue;

		/// <summary>
		/// Cria a instancia do o resultado e os parametros de saída.
		/// </summary>
		/// <param name="returnValue"></param>
		/// <param name="outputParameters"></param>
		public VirtualObjectDataSourceStatusEventArgs(object returnValue, IDictionary outputParameters) : this(returnValue, outputParameters, null)
		{
		}

		/// <summary>
		/// Cria a instancia com os dados do erro ocorrido.
		/// </summary>
		/// <param name="returnValue"></param>
		/// <param name="outputParameters"></param>
		/// <param name="exception"></param>
		public VirtualObjectDataSourceStatusEventArgs(object returnValue, IDictionary outputParameters, Exception exception)
		{
			_affectedRows = -1;
			_returnValue = returnValue;
			_outputParameters = outputParameters;
			_exception = exception;
		}

		/// <summary>
		/// Quantidade de linhas afetadas.
		/// </summary>
		public int AffectedRows
		{
			get
			{
				return _affectedRows;
			}
			set
			{
				_affectedRows = value;
			}
		}

		/// <summary>
		/// Erro associado.
		/// </summary>
		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}

		/// <summary>
		/// Identifica se o erro foi tratado.
		/// </summary>
		public bool ExceptionHandled
		{
			get
			{
				return _exceptionHandled;
			}
			set
			{
				_exceptionHandled = value;
			}
		}

		/// <summary>
		/// Parametros de saída.
		/// </summary>
		public IDictionary OutputParameters
		{
			get
			{
				return _outputParameters;
			}
		}

		/// <summary>
		/// Valor de retorno.
		/// </summary>
		public object ReturnValue
		{
			get
			{
				return _returnValue;
			}
		}

		/// <summary>
		/// Altera o valor do retorno.
		/// </summary>
		/// <param name="value"></param>
		void IVirtualObjectDataSourceStatusEventArgsExtended.ChangeReturnValue(object value)
		{
			_returnValue = value;
		}
	}
}
