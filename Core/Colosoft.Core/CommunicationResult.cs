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
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Colosoft.Runtime.Remoting
{
	/// <summary>
	/// Representa o resultado da comunicação.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct CommunicationResult
	{
		/// <summary>
		/// Identifica que a comunicação foi realizada com sucesso.
		/// </summary>
		public static readonly CommunicationResult Success;

		/// <summary>
		/// Idenificica que a comunicação foi fechada.
		/// </summary>
		public static readonly CommunicationResult Closed;

		/// <summary>
		/// Identifica que a comunicação está aberta.
		/// </summary>
		public static readonly CommunicationResult AlreadyOpen;

		/// <summary>
		/// Identifica que a comunicação foi aberta com falha.
		/// </summary>
		public static readonly CommunicationResult OpenFailed;

		/// <summary>
		/// Identifica que estorou o tempo de espera.
		/// </summary>
		public static readonly CommunicationResult Timeout;

		/// <summary>
		/// Identifica que houve uma falha no envio.
		/// </summary>
		public static readonly CommunicationResult SendFailed;

		/// <summary>
		/// Identifica que houve uma falha no recebimento.
		/// </summary>
		public static readonly CommunicationResult ReceiveFailed;

		/// <summary>
		/// Identifica que o endereço está sendo usado.
		/// </summary>
		public static readonly CommunicationResult AddressUsed;

		private static readonly Dictionary<int, string> _resultDictionary;

		private int _errorCode;

		private string _message;

		/// <summary>
		/// Código do erro.
		/// </summary>
		public int ErrorCode
		{
			get
			{
				return _errorCode;
			}
		}

		/// <summary>
		/// Cria a instancia informando o código do erro.
		/// </summary>
		/// <param name="errorCode"></param>
		public CommunicationResult(int errorCode)
		{
			_errorCode = errorCode;
			_message = null;
		}

		/// <summary>
		/// Cria a instancia informando o código do erro e a mensagem.
		/// </summary>
		/// <param name="errorCode"></param>
		/// <param name="message"></param>
		public CommunicationResult(int errorCode, string message)
		{
			_errorCode = errorCode;
			_message = message;
		}

		/// <summary>
		/// Construtor estático.
		/// </summary>
		static CommunicationResult()
		{
			Success = new CommunicationResult(0);
			Closed = new CommunicationResult(2);
			AlreadyOpen = new CommunicationResult(1);
			OpenFailed = new CommunicationResult(3);
			Timeout = new CommunicationResult(4);
			SendFailed = new CommunicationResult(5);
			ReceiveFailed = new CommunicationResult(6);
			AddressUsed = new CommunicationResult(0x2740);
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			dictionary.Add(4, Properties.Resources.CommunicationReader_FormatErrorTimeout);
			dictionary.Add(0x1c4, Properties.Resources.CommunicationReader_FormatErrorFtpDiskFull);
			dictionary.Add(0x228, Properties.Resources.CommunicationReader_FormatErrorFtpDiskFull);
			dictionary.Add(0x274c, Properties.Resources.CommunicationReader_FormatErrorTimeout);
			dictionary.Add(0x3f0, Properties.Resources.CommunicationReader_FormatErrorNoOpFailed);
			_resultDictionary = dictionary;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = null;
			if(_message != null)
				str = string.Format(CultureInfo.CurrentCulture, _message, ErrorCode);
			if(this.ErrorCode != 0)
			{
				if(_resultDictionary.ContainsKey(this.ErrorCode))
				{
					str = string.Format(CultureInfo.CurrentCulture, _resultDictionary[this.ErrorCode], new object[] {
						this.ErrorCode
					});
				}
				else
				{
					if((0x63 < this.ErrorCode) && (this.ErrorCode <= 0x3e7))
					{
						str = string.Format(CultureInfo.CurrentCulture, Properties.Resources.CommunicationReader_FormatErrorFtp, new object[] {
							this.ErrorCode
						});
					}
					else if(0x270f < this.ErrorCode)
					{
						str = string.Format(CultureInfo.CurrentCulture, Properties.Resources.CommunicationReader_FormatErrorSocket, new object[] {
							this.ErrorCode
						});
					}
				}
			}
			if(str == null)
				return string.Format(CultureInfo.CurrentCulture, Properties.Resources.CommunicationReader_FormatErrorCommunication, new object[] {
					this.ErrorCode
				});
			return str;
		}

		/// <summary>
		/// Recupera as informações para depuraçã.
		/// </summary>
		/// <returns></returns>
		public string GetDebugInfo()
		{
			try
			{
				StringBuilder builder = new StringBuilder(this.ToString());
				builder.Append(string.Format(CultureInfo.InvariantCulture, Properties.Resources.CommunicationReader_FormatErrorCode, new object[] {
					this._errorCode
				}));
				var trace = new System.Diagnostics.StackTrace(true);
				foreach (System.Diagnostics.StackFrame frame in trace.GetFrames())
				{
					builder.Append(string.Format(CultureInfo.InvariantCulture, Properties.Resources.CommunicationReader_FormatStackFrame, frame.GetMethod().ToString(), frame.GetFileName(), frame.GetFileLineNumber()));
				}
				return builder.ToString();
			}
			catch(ArgumentOutOfRangeException)
			{
			}
			catch(ArgumentNullException)
			{
			}
			catch(FormatException)
			{
			}
			return this.ToString();
		}

		/// <summary>
		/// Verifica se os dados são iguais aos da instancia informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Recupera o hashcode.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Sobrecarga do operador com o tipo Boolean.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public static implicit operator bool(CommunicationResult result)
		{
			return (result.ErrorCode == 0);
		}

		/// <summary>
		/// Sobrecarga do operador com o tipo Int32.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public static implicit operator int(CommunicationResult result)
		{
			return result.ErrorCode;
		}

		/// <summary>
		/// Sobrecarga do operador de comparação.
		/// </summary>
		/// <param name="result1"></param>
		/// <param name="result2"></param>
		/// <returns></returns>
		public static bool operator ==(CommunicationResult result1, CommunicationResult result2)
		{
			return (result1.ErrorCode == result2.ErrorCode);
		}

		/// <summary>
		/// Sobrecarga do operador de comparação.
		/// </summary>
		/// <param name="result1"></param>
		/// <param name="result2"></param>
		/// <returns></returns>
		public static bool operator !=(CommunicationResult result1, CommunicationResult result2)
		{
			return (result1.ErrorCode != result2.ErrorCode);
		}
	}
}
