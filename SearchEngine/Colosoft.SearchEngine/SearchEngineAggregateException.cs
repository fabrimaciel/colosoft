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
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Representa um agregador de exceções ocorridas no SearchEngine.
	/// </summary>
	[Serializable, System.Diagnostics.DebuggerDisplay("Count = {InnerExceptions.Count}")]
	public class SearchEngineAggregateException : Exception
	{
		private ReadOnlyCollection<Exception> _innerExceptions;

		/// <summary>
		/// Exceções internas.
		/// </summary>
		public ReadOnlyCollection<Exception> InnerExceptions
		{
			get
			{
				return _innerExceptions;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SearchEngineAggregateException() : base("One or more errors occurred.")
		{
			_innerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
		}

		/// <summary>
		/// Cria uma nova instancia com base nas exceções informadas.
		/// </summary>
		/// <param name="innerExceptions"></param>
		public SearchEngineAggregateException(IEnumerable<Exception> innerExceptions) : this("One or more errors occurred.", innerExceptions)
		{
		}

		/// <summary>
		/// Cria uma nova instancia com base nas exceções informadas.
		/// </summary>
		/// <param name="innerExceptions"></param>
		public SearchEngineAggregateException(params Exception[] innerExceptions) : this("One or more errors occurred.", innerExceptions)
		{
		}

		/// <summary>
		/// Cria uma instancia com base na mensagem informada.
		/// </summary>
		/// <param name="message"></param>
		public SearchEngineAggregateException(string message) : base(message)
		{
			_innerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
		}

		/// <summary>
		/// Construtor usado na serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecurityCritical]
		protected SearchEngineAggregateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			Exception[] list = info.GetValue("InnerExceptions", typeof(Exception[])) as Exception[];
			if(list == null)
				throw new SerializationException("The serialization stream contains no inner exceptions.");
			_innerExceptions = new ReadOnlyCollection<Exception>(list);
		}

		/// <summary>
		/// Cria uma instancia baseada na mensagem e nas exceções informadas.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerExceptions"></param>
		public SearchEngineAggregateException(string message, IEnumerable<Exception> innerExceptions) : this(message, (innerExceptions == null) ? null : ((IList<Exception>)new List<Exception>(innerExceptions)))
		{
		}

		/// <summary>
		/// Cria uma instancia baseada na mensagem e na exceção informada.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public SearchEngineAggregateException(string message, Exception innerException) : base(message, innerException)
		{
			if(innerException == null)
				throw new ArgumentNullException("innerException");
			_innerExceptions = new ReadOnlyCollection<Exception>(new Exception[] {
				innerException
			});
		}

		private SearchEngineAggregateException(string message, IList<Exception> innerExceptions) : base(message, ((innerExceptions != null) && (innerExceptions.Count > 0)) ? innerExceptions[0] : null)
		{
			if(innerExceptions == null)
				throw new ArgumentNullException("innerExceptions");
			Exception[] list = new Exception[innerExceptions.Count];
			for(int i = 0; i < list.Length; i++)
			{
				list[i] = innerExceptions[i];
				if(list[i] == null)
					throw new ArgumentException("An element of innerExceptions was null.");
			}
			_innerExceptions = new ReadOnlyCollection<Exception>(list);
		}

		/// <summary>
		/// Cria uma instancia baseada na mensagem e nas exceções informadas.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerExceptions"></param>
		public SearchEngineAggregateException(string message, params Exception[] innerExceptions) : this(message, (IList<Exception>)innerExceptions)
		{
		}

		/// <summary>
		/// Achata as exceções.
		/// </summary>
		/// <returns></returns>
		public SearchEngineAggregateException Flatten()
		{
			List<Exception> innerExceptions = new List<Exception>();
			List<SearchEngineAggregateException> list2 = new List<SearchEngineAggregateException>();
			list2.Add(this);
			int num = 0;
			while (list2.Count > num)
			{
				IList<Exception> list3 = list2[num++].InnerExceptions;
				for(int i = 0; i < list3.Count; i++)
				{
					Exception item = list3[i];
					if(item != null)
					{
						var exception2 = item as SearchEngineAggregateException;
						if(exception2 != null)
						{
							list2.Add(exception2);
						}
						else
						{
							innerExceptions.Add(item);
						}
					}
				}
			}
			return new SearchEngineAggregateException(this.Message, innerExceptions);
		}

		/// <summary>
		/// Recupera a exceção base.
		/// </summary>
		/// <returns></returns>
		public override Exception GetBaseException()
		{
			Exception innerException = this;
			for(var exception2 = this; (exception2 != null) && (exception2.InnerExceptions.Count == 1); exception2 = innerException as SearchEngineAggregateException)
				innerException = innerException.InnerException;
			return innerException;
		}

		/// <summary>
		/// Recupera os dados para a serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			base.GetObjectData(info, context);
			Exception[] array = new Exception[this._innerExceptions.Count];
			this._innerExceptions.CopyTo(array, 0);
			info.AddValue("InnerExceptions", array, typeof(Exception[]));
		}

		/// <summary>
		/// Trata as exceções obedecendo o predicador informado
		/// </summary>
		/// <param name="predicate"></param>
		public void Handle(Func<Exception, bool> predicate)
		{
			if(predicate == null)
				throw new ArgumentNullException("predicate");
			List<Exception> innerExceptions = null;
			for(int i = 0; i < _innerExceptions.Count; i++)
			{
				if(!predicate(_innerExceptions[i]))
				{
					if(innerExceptions == null)
						innerExceptions = new List<Exception>();
					innerExceptions.Add(_innerExceptions[i]);
				}
			}
			if(innerExceptions != null)
				throw new SearchEngineAggregateException(this.Message, innerExceptions);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = base.ToString();
			for(int i = 0; i < _innerExceptions.Count; i++)
				str = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{1}---> (Inner Exception #{2}) {3}{4}{5}", new object[] {
					str,
					Environment.NewLine,
					i,
					this._innerExceptions[i].ToString(),
					"<---",
					Environment.NewLine
				});
			return str;
		}
	}
}
