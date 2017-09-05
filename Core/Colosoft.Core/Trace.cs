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

namespace Colosoft.Diagnostics
{
	/// <summary>
	/// Classe reponsável por realiza o trace do sistema.
	/// </summary>
	public static class Trace
	{
		private static bool _isEnabled = false;

		/// <summary>
		/// Identifia se o 
		/// </summary>
		public static bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				_isEnabled = value;
			}
		}

		/// <summary>
		/// Cria uma operação para ser monitorada.
		/// </summary>
		/// <param name="name">Nome da operação que será monitorada.</param>
		/// <param name="args">Argumentos que serão utilizados na formação do nome da operação</param>
		/// <returns></returns>
		public static OperationScope CreateOperation(string name, params object[] args)
		{
			return new OperationScope(args != null ? string.Format(name, args) : name);
		}

		/// <summary>
		/// Escreve uma linha para o trace.
		/// </summary>
		/// <param name="value"></param>
		public static void WriteLine(object value)
		{
			if(IsEnabled)
				System.Diagnostics.Trace.WriteLine(string.Format("[Message: {0}]", value));
		}

		/// <summary>
		/// Representa o escopo de uma operação.
		/// </summary>
		public class OperationScope : IDisposable
		{
			private System.Diagnostics.Stopwatch _stopwatch;

			private string _name;

			/// <summary>
			/// Recupera o tempo total passado desde o inicio da operação.
			/// </summary>
			public TimeSpan Elapsed
			{
				get
				{
					return _stopwatch.Elapsed;
				}
			}

			/// <summary>
			/// Recupera o tempo total, em milisegundos, desde o início da operação.
			/// </summary>
			public long ElapsedMilliseconds
			{
				get
				{
					return _stopwatch.ElapsedMilliseconds;
				}
			}

			/// <summary>
			/// Nome da operação.
			/// </summary>
			public string Name
			{
				get
				{
					return _name;
				}
			}

			/// <summary>
			/// Identifica se a operação está em execução.
			/// </summary>
			public bool IsRunning
			{
				get
				{
					return _stopwatch.IsRunning;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="name">Nome da operação.</param>
			internal OperationScope(string name)
			{
				_name = name;
				_stopwatch = new System.Diagnostics.Stopwatch();
				_stopwatch.Start();
			}

			/// <summary>
			/// Destrutor.
			/// </summary>
			~OperationScope()
			{
				Dispose(false);
			}

			/// <summary>
			/// Para a operação.
			/// </summary>
			public void Stop()
			{
				if(_stopwatch != null && _stopwatch.IsRunning)
				{
					_stopwatch.Stop();
					System.Diagnostics.Trace.WriteLine(ToString());
				}
			}

			/// <summary>
			/// Recupera o texto que representa a instancia.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format("[Operation: {0}, Elapsed: {1}ms]", _name, _stopwatch.ElapsedMilliseconds);
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			/// <param name="disposing"></param>
			protected virtual void Dispose(bool disposing)
			{
				Stop();
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				Dispose(true);
			}
		}
	}
}
