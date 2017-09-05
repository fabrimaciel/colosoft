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
using System.Collections;

namespace Colosoft.Text
{
	/// <summary>
	/// Classe usada na compara das mensagens formatáveis.
	/// </summary>
	public abstract class MessageFormattableComparer : IComparer, IEqualityComparer, IComparer<IMessageFormattable>, IEqualityComparer<IMessageFormattable>
	{
		private static MessageFormattableComparer _invariantCulture;

		private static MessageFormattableComparer _invariantCultureIgnoreCase;

		private static MessageFormattableComparer _ordinal;

		private static MessageFormattableComparer _ordinalIgnoreCase;

		/// <summary>
		/// Comparador da cultura atual.
		/// </summary>
		public static MessageFormattableComparer CurrentCulture
		{
			get
			{
				return new CultureAwareComparer(Globalization.Culture.SystemCulture, false);
			}
		}

		/// <summary>
		/// Comparador da cultura atual, com ignore case.
		/// </summary>
		public static MessageFormattableComparer CurrentCultureIgnoreCase
		{
			get
			{
				return new CultureAwareComparer(Globalization.Culture.SystemCulture, true);
			}
		}

		/// <summary>
		/// Comparador invariante de cultura.
		/// </summary>
		public static MessageFormattableComparer InvariantCulture
		{
			get
			{
				return _invariantCulture;
			}
		}

		/// <summary>
		/// Comparador invariante de cultura com ignore case.
		/// </summary>
		public static MessageFormattableComparer InvariantCultureIgnoreCase
		{
			get
			{
				return _invariantCultureIgnoreCase;
			}
		}

		/// <summary>
		/// Comparador ordinal.
		/// </summary>
		public static MessageFormattableComparer Ordinal
		{
			get
			{
				return _ordinal;
			}
		}

		/// <summary>
		/// Comparador ordinal com ignore case.
		/// </summary>
		public static MessageFormattableComparer OrdinalIgnoreCase
		{
			get
			{
				return _ordinalIgnoreCase;
			}
		}

		/// <summary>
		/// Construtor geral.
		/// </summary>
		static MessageFormattableComparer()
		{
			_invariantCulture = new CultureAwareComparer(System.Globalization.CultureInfo.InvariantCulture, false);
			_invariantCultureIgnoreCase = new CultureAwareComparer(System.Globalization.CultureInfo.InvariantCulture, true);
			_ordinal = new OrdinalComparer(System.Globalization.CultureInfo.InvariantCulture, false);
			_ordinalIgnoreCase = new OrdinalComparer(System.Globalization.CultureInfo.InvariantCulture, true);
		}

		/// <summary>
		/// Cria uma instancia do comparador usando a cultura informada.
		/// </summary>
		/// <param name="culture"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		public static MessageFormattableComparer Create(System.Globalization.CultureInfo culture, bool ignoreCase)
		{
			if(culture == null)
				throw new ArgumentNullException("culture");
			return new CultureAwareComparer(culture, ignoreCase);
		}

		/// <summary>
		/// Verifica se as instancias informadas são iguais.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		new public bool Equals(object x, object y)
		{
			if(x == y)
				return true;
			if((x == null) || (y == null))
				return false;
			var str = x as IMessageFormattable;
			if(str != null)
			{
				var str2 = y as IMessageFormattable;
				if(str2 != null)
					return this.Equals(str, str2);
			}
			return x.Equals(y);
		}

		/// <summary>
		/// Verifica se as instancias informadas são iguais.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public abstract bool Equals(IMessageFormattable x, IMessageFormattable y);

		/// <summary>
		/// Compara os objetos informados
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(object x, object y)
		{
			if(x == y)
				return 0;
			if(x == null)
				return -1;
			if(y == null)
				return 1;
			var str = x as IMessageFormattable;
			if(str != null)
			{
				var str2 = y as IMessageFormattable;
				if(str2 != null)
					return this.Compare(str, str2);
			}
			IComparable comparable = x as IComparable;
			if(comparable == null)
				throw new ArgumentException("Argument implement IComparable");
			return comparable.CompareTo(y);
		}

		/// <summary>
		/// Compara as insntancias informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public abstract int Compare(IMessageFormattable x, IMessageFormattable y);

		/// <summary>
		/// Recupera o hashcode do objeto informado.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(object obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			var str = obj as IMessageFormattable;
			if(str != null)
				return this.GetHashCode(str);
			return obj.GetHashCode();
		}

		/// <summary>
		/// Recupera o Hashcode da instancia informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public abstract int GetHashCode(IMessageFormattable obj);
	}
}
