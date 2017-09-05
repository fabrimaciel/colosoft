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
using Colosoft.Serialization.IO;

namespace Colosoft.Caching.Expiration
{
	/// <summary>
	/// Possíveis tipos de expiração do cache.
	/// </summary>
	public enum ExpirationHintType
	{
		/// <summary>
		/// Agregação.
		/// </summary>
		AggregateExpirationHint = 13,
		/// <summary>
		/// Dependencia.
		/// </summary>
		DependencyHint = 15,
		/// <summary>
		/// Dependecia extensível.
		/// </summary>
		ExtensibleDependency = 0x11,
		/// <summary>
		/// Arquivo de dependencia.
		/// </summary>
		FileDependency = 5,
		/// <summary>
		/// Expiração fixa.
		/// </summary>
		FixedExpiration = 1,
		/// <summary>
		/// 
		/// </summary>
		FixedIdleExpiration = 4,
		/// <summary>
		/// 
		/// </summary>
		IdleExpiration = 12,
		/// <summary>
		/// Chave de dependencia.
		/// </summary>
		KeyDependency = 6,
		/// <summary>
		/// Nó de expiração.
		/// </summary>
		NodeExpiration = 7,
		/// <summary>
		/// NULL
		/// </summary>
		NULL = -1,
		/// <summary>
		/// Pai.
		/// </summary>
		Parent = 0,
	}
	/// <summary>
	/// Classe que gerencia a expiração.
	/// </summary>
	[Serializable]
	public abstract class ExpirationHint : IComparable, IDisposable, ICompactSerializable
	{
		/// <summary>
		/// Flag que identifica que foi liberado.
		/// </summary>
		public const int DISPOSED = 0x10;

		/// <summary>
		/// Flag que identifica que foi expirado
		/// </summary>
		public const int EXPIRED = 1;

		/// <summary>
		/// Flag que identifica que é variante.
		/// </summary>
		public const int IS_VARIANT = 4;

		/// <summary>
		/// Flag que identifica que é necessário ressincronizar.
		/// </summary>
		public const int NEEDS_RESYNC = 2;

		/// <summary>
		/// Flag que identifica que é roteável.
		/// </summary>
		public const int ROUTABLE = 8;

		private int _bits;

		/// <summary>
		/// Tipo do hint
		/// </summary>
		public ExpirationHintType HintType = ExpirationHintType.Parent;

		private IExpirationEventSink _objNotify;

		/// <summary>
		/// Chave do cache.
		/// </summary>
		public virtual string CacheKey
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se já expirou.
		/// </summary>
		public bool HasExpired
		{
			get
			{
				return this.IsBitSet(EXPIRED);
			}
		}

		/// <summary>
		/// Identifica se já foi liberado.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return this.IsBitSet(DISPOSED);
			}
		}

		/// <summary>
		/// Identifica se já foi roteado.
		/// </summary>
		public bool IsRoutable
		{
			get
			{
				return this.IsBitSet(ROUTABLE);
			}
		}

		/// <summary>
		/// Identifica se é variaten.
		/// </summary>
		public bool IsVariant
		{
			get
			{
				return this.IsBitSet(IS_VARIANT);
			}
		}

		/// <summary>
		/// Identifica se precisa resincronizar.
		/// </summary>
		public bool NeedsReSync
		{
			get
			{
				return this.IsBitSet(NEEDS_RESYNC);
			}
		}

		/// <summary>
		/// Chave de ordenação.
		/// </summary>
		internal abstract int SortKey
		{
			get;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected ExpirationHint()
		{
		}

		/// <summary>
		/// Verifica se já foi expierado no contexto informado.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal virtual bool CheckExpired(CacheRuntimeContext context)
		{
			return false;
		}

		/// <summary>
		/// Determina a expiração.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal virtual bool DetermineExpiration(CacheRuntimeContext context)
		{
			return this.HasExpired;
		}

		/// <summary>
		/// Verifica se o bit informado está marcado na instancia.
		/// </summary>
		/// <param name="bit"></param>
		/// <returns></returns>
		public bool IsBitSet(int bit)
		{
			return (_bits & bit) != 0;
		}

		/// <summary>
		/// Método acionado para notificar a expiração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void NotifyExpiration(object sender, EventArgs e)
		{
			if(this.SetBit(1))
			{
				IExpirationEventSink sink = _objNotify;
				if(sink != null)
					sink.DependentExpired(sender, e);
			}
		}

		/// <summary>
		/// Lê uma dica de expiração.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static ExpirationHint ReadExpHint(CompactReader reader)
		{
			ExpirationHintType parent = ExpirationHintType.Parent;
			parent = (ExpirationHintType)reader.ReadInt16();
			switch(parent)
			{
			case ExpirationHintType.NULL:
				return null;
			case ExpirationHintType.Parent:
				return (ExpirationHint)reader.ReadObject();
			case ExpirationHintType.FixedExpiration:
			{
				FixedExpiration expiration = new FixedExpiration();
				expiration.Deserialize(reader);
				return expiration;
			}
			case ExpirationHintType.FixedIdleExpiration:
			{
				FixedIdleExpiration expiration4 = new FixedIdleExpiration();
				expiration4.Deserialize(reader);
				return expiration4;
			}
			case ExpirationHintType.FileDependency:
			{
				FileDependency dependency = new FileDependency();
				dependency.Deserialize(reader);
				return dependency;
			}
			case ExpirationHintType.KeyDependency:
			{
				KeyDependency dependency2 = new KeyDependency();
				dependency2.Deserialize(reader);
				return dependency2;
			}
			case ExpirationHintType.IdleExpiration:
			{
				IdleExpiration expiration5 = new IdleExpiration();
				expiration5.Deserialize(reader);
				return expiration5;
			}
			case ExpirationHintType.AggregateExpirationHint:
			{
				AggregateExpirationHint hint2 = new AggregateExpirationHint();
				hint2.Deserialize(reader);
				return hint2;
			}
			case ExpirationHintType.ExtensibleDependency:
			{
				ExtensibleDependency dependency8 = new ExtensibleDependency();
				return (ExtensibleDependency)reader.ReadObject();
			}
			}
			return null;
		}

		/// <summary>
		/// Salva a dia no escritor informado.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="expHint"></param>
		public static void WriteExpHint(CompactWriter writer, ExpirationHint expHint)
		{
			if(expHint == null)
				writer.Write((short)(-1));
			else
			{
				writer.Write((short)expHint.HintType);
				if(expHint.HintType == ExpirationHintType.ExtensibleDependency)
					writer.WriteObject(expHint);
				else
					expHint.Serialize(writer);
			}
		}

		/// <summary>
		/// Reinicializa a instancia da dica.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal virtual bool ReInitializeHint(CacheRuntimeContext context)
		{
			return false;
		}

		/// <summary>
		/// Reseta a instancia.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal virtual bool Reset(CacheRuntimeContext context)
		{
			_bits &= -2;
			return true;
		}

		/// <summary>
		/// Resenta a variação.
		/// </summary>
		/// <param name="context"></param>
		internal virtual void ResetVariant(CacheRuntimeContext context)
		{
			if(this.IsVariant)
				this.Reset(context);
		}

		/// <summary>
		/// Define o bit para instancia.
		/// </summary>
		/// <param name="bit"></param>
		/// <returns></returns>
		public virtual bool SetBit(int bit)
		{
			int num;
			do
			{
				num = _bits;
				if((num & bit) != 0)
					return false;
			}
			while (System.Threading.Interlocked.CompareExchange(ref _bits, num | bit, num) != num);
			return true;
		}

		/// <summary>
		/// Define o <see cref="IExpirationEventSink"/> para a instancia.
		/// </summary>
		/// <param name="objNotify"></param>
		protected internal void SetExpirationEventSink(IExpirationEventSink objNotify)
		{
			_objNotify = objNotify;
		}

		/// <summary>
		/// Compara a instancia com o objeto informado.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if(obj is ExpirationHint)
				return this.SortKey.CompareTo(((ExpirationHint)obj).SortKey);
			return 1;
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public virtual void Serialize(CompactWriter writer)
		{
			writer.WriteObject(HintType);
			writer.Write(_bits);
		}

		/// <summary>
		/// Deserializa os dados para a instancia.
		/// </summary>
		/// <param name="reader"></param>
		public virtual void Deserialize(CompactReader reader)
		{
			HintType = (ExpirationHintType)reader.ReadObject();
			_bits = reader.ReadInt32();
		}

		/// <summary>
		/// Libera a instancia
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		public void Dispose()
		{
			SetBit(16);
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
