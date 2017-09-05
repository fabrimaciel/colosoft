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
using System.Threading;

namespace Colosoft.Threading
{
	/// <summary>
	/// Adaptação de uma thread.
	/// </summary>
	public class ThreadClass : IThreadRunnable
	{
		/// <summary>
		/// Instancia adaptada.
		/// </summary>
		private Thread _threadField;

		/// <summary>
		/// Identifica se a thread está ativa.
		/// </summary>
		public bool IsAlive
		{
			get
			{
				return _threadField.IsAlive;
			}
		}

		/// <summary>
		/// Identifica se a thread está executando em background.
		/// </summary>
		public bool IsBackground
		{
			get
			{
				return _threadField.IsBackground;
			}
			set
			{
				_threadField.IsBackground = value;
			}
		}

		/// <summary>
		/// Identifica se a thread foi interrompida.
		/// </summary>
		public bool IsInterrupted
		{
			get
			{
				return ((_threadField.ThreadState & ThreadState.WaitSleepJoin) == ThreadState.WaitSleepJoin);
			}
		}

		/// <summary>
		/// Nome da thread.
		/// </summary>
		public string Name
		{
			get
			{
				return _threadField.Name;
			}
			set
			{
				if(_threadField.Name == null)
					_threadField.Name = value;
			}
		}

		/// <summary>
		/// Prioridade de thread.
		/// </summary>
		public ThreadPriority Priority
		{
			get
			{
				return _threadField.Priority;
			}
			set
			{
				_threadField.Priority = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ThreadClass()
		{
			this._threadField = new Thread(new ThreadStart(this.Run));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Name"></param>
		public ThreadClass(string Name)
		{
			this._threadField = new Thread(new ThreadStart(this.Run));
			this.Name = Name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Start"></param>
		public ThreadClass(ThreadStart Start)
		{
			this._threadField = new Thread(Start);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Start"></param>
		/// <param name="Name"></param>
		public ThreadClass(ThreadStart Start, string Name)
		{
			this._threadField = new Thread(Start);
			this.Name = Name;
		}

		/// <summary>
		/// Aborta a execução.
		/// </summary>
		public void Abort()
		{
			this._threadField.Abort();
		}

		/// <summary>
		/// Aborta a exeução.
		/// </summary>
		/// <param name="stateInfo">Informações do estado.</param>
		public void Abort(object stateInfo)
		{
			lock (this)
			{
				this._threadField.Abort(stateInfo);
			}
		}

		/// <summary>
		/// Recupera a instancia da atual thread.
		/// </summary>
		/// <returns></returns>
		public static ThreadClass Current()
		{
			ThreadClass class2 = new ThreadClass();
			class2._threadField = Thread.CurrentThread;
			return class2;
		}

		/// <summary>
		/// Interronpe a execução.
		/// </summary>
		public virtual void Interrupt()
		{
			this._threadField.Interrupt();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Join()
		{
			this._threadField.Join();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="miliSeconds"></param>
		public void Join(long miliSeconds)
		{
			lock (this)
			{
				this._threadField.Join(new TimeSpan(miliSeconds * 0x2710));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="miliSeconds"></param>
		/// <param name="nanoSeconds"></param>
		public void Join(long miliSeconds, int nanoSeconds)
		{
			lock (this)
			{
				this._threadField.Join(new TimeSpan((miliSeconds * 0x2710) + (nanoSeconds * 100)));
			}
		}

		/// <summary>
		/// Método de execução da thread.
		/// </summary>
		public virtual void Run()
		{
		}

		/// <summary>
		/// Inicializa a execução.
		/// </summary>
		public virtual void Start()
		{
			this._threadField.Start();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ("Thread[" + this.Name + "," + this.Priority.ToString() + ",]");
		}
	}
}
