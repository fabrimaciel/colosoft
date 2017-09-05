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
using Colosoft.Serialization;
using System.Collections;
using Colosoft.Serialization.IO;

namespace Colosoft.Caching.Expiration
{
	/// <summary>
	/// Representa um agregador de <see cref="ExpirationHint"/>.
	/// </summary>
	[Serializable]
	public class AggregateExpirationHint : ExpirationHint, IExpirationEventSink, ICompactSerializable, IEnumerable<ExpirationHint>
	{
		private List<ExpirationHint> _hints;

		/// <summary>
		/// Chave do cache.
		/// </summary>
		public override string CacheKey
		{
			set
			{
				for(int i = 0; i < _hints.Count; i++)
					((ExpirationHint)_hints[i]).CacheKey = value;
			}
		}

		/// <summary>
		/// Recupera os hints que foram agregados.
		/// </summary>
		public ExpirationHint[] Hints
		{
			get
			{
				return (ExpirationHint[])_hints.ToArray<ExpirationHint>();
			}
		}

		/// <summary>
		/// Chave de ordenação.
		/// </summary>
		internal override int SortKey
		{
			get
			{
				ExpirationHint hint = _hints[0];
				for(int i = 0; i < _hints.Count; i++)
				{
					if(((IComparable)_hints[i]).CompareTo(hint) < 0)
						hint = (ExpirationHint)_hints[i];
				}
				return hint.SortKey;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public AggregateExpirationHint()
		{
			_hints = new List<ExpirationHint>();
			base.HintType = ExpirationHintType.AggregateExpirationHint;
		}

		/// <summary>
		/// Cria uma instancia agregando os hints.
		/// </summary>
		/// <param name="hints">Dicas que serão agregadas.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public AggregateExpirationHint(params ExpirationHint[] hints)
		{
			_hints = new List<ExpirationHint>();
			base.HintType = ExpirationHintType.AggregateExpirationHint;
			this.Initialize(hints);
		}

		/// <summary>
		/// Adiciona uma nova expiração para ser agregada.
		/// </summary>
		/// <param name="eh"></param>
		public void Add(ExpirationHint eh)
		{
			lock (this)
			{
				if(!eh.IsRoutable)
					this.SetBit(8);
				if(eh.IsVariant)
					this.SetBit(4);
				eh.SetExpirationEventSink(this);
				_hints.Add(eh);
			}
		}

		/// <summary>
		/// Verifica se já foi expirado.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal override bool CheckExpired(CacheRuntimeContext context)
		{
			if(base.HasExpired)
			{
				return true;
			}
			for(int i = 0; i < _hints.Count; i++)
			{
				if(((ExpirationHint)_hints[i]).CheckExpired(context))
				{
					base.NotifyExpiration(_hints[i], null);
					break;
				}
			}
			return base.HasExpired;
		}

		/// <summary>
		/// Determina a expiração.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal override bool DetermineExpiration(CacheRuntimeContext context)
		{
			if(base.HasExpired)
			{
				return true;
			}
			for(int i = 0; i < _hints.Count; i++)
			{
				if(((ExpirationHint)_hints[i]).DetermineExpiration(context))
				{
					base.NotifyExpiration(_hints[i], null);
					break;
				}
			}
			return base.HasExpired;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			for(int i = 0; i < _hints.Count; i++)
				((IDisposable)_hints[i]).Dispose();
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="hints"></param>
		protected void Initialize(ExpirationHint[] hints)
		{
			hints.Require("hints").NotNull();
			_hints.AddRange(hints);
			for(int i = 0; i < _hints.Count; i++)
			{
				if(!_hints[i].IsRoutable)
					SetBit(8);
				if(_hints[i].IsVariant)
					SetBit(4);
				_hints[i].SetExpirationEventSink(this);
			}
		}

		/// <summary>
		/// Reseta a instancia.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal override bool Reset(CacheRuntimeContext context)
		{
			bool flag = base.Reset(context);
			for(int i = 0; i < _hints.Count; i++)
			{
				if(_hints[i].Reset(context))
					flag = true;
			}
			return flag;
		}

		/// <summary>
		/// Reseta a variação.
		/// </summary>
		/// <param name="context"></param>
		internal override void ResetVariant(CacheRuntimeContext context)
		{
			for(int i = 0; i < _hints.Count; i++)
			{
				ExpirationHint hint = (ExpirationHint)_hints[i];
				if(hint.IsVariant)
					hint.Reset(context);
			}
		}

		/// <summary>
		/// Define o bit informado.
		/// </summary>
		/// <param name="bit"></param>
		/// <returns></returns>
		public override bool SetBit(int bit)
		{
			bool flag = false;
			if(bit == 2)
			{
				for(int i = 0; i < _hints.Count; i++)
				{
					flag = ((ExpirationHint)_hints[i]).SetBit(bit);
					if(!flag)
						return flag;
				}
			}
			return base.SetBit(bit);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str;
			str = (!(_hints[0] is IdleExpiration) && !(_hints[0] is FixedExpiration)) ? (str = "INNER\r\n") : string.Empty;
			for(int i = 0; i < _hints.Count; i++)
			{
				if((i >= 1) && (((_hints[i - 1] is KeyDependency) && (_hints[i] is KeyDependency)) || ((_hints[i - 1] is FileDependency) && (_hints[i] is FileDependency))))
					str = str + "INNER\r\n";
				if(_hints[i] is ExtensibleDependency)
					str = str + "EXTDEPENDENCY \"\r\n";
				else
					str = str + _hints[i].ToString();
			}
			return str;
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public override void Serialize(CompactWriter writer)
		{
			base.Serialize(writer);
			writer.Write(_hints.Count);
			for(int i = 0; i < _hints.Count; i++)
				writer.WriteObject(_hints[i]);
		}

		/// <summary>
		/// Deserializa os dados para a instancia.
		/// </summary>
		/// <param name="reader"></param>
		public override void Deserialize(CompactReader reader)
		{
			base.Deserialize(reader);
			int capacity = reader.ReadInt32();
			if(_hints == null)
				_hints = new List<ExpirationHint>(capacity);
			for(int i = 0; i < capacity; i++)
				_hints.Insert(i, (ExpirationHint)reader.ReadObject());
		}

		/// <summary>
		/// Método acionado quando um dependencia expirar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void DependentExpired(object sender, EventArgs e)
		{
			base.NotifyExpiration(sender, e);
		}

		/// <summary>
		/// Recupera o enumerado da instancia.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return _hints.GetEnumerator();
		}

		IEnumerator<ExpirationHint> IEnumerable<ExpirationHint>.GetEnumerator()
		{
			return _hints.GetEnumerator();
		}
	}
}
