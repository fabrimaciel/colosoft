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

namespace Colosoft.Text
{
	/// <summary>
	/// Implementação do comparador Oridinal das mensagens formatáveis.
	/// </summary>
	[Serializable]
	sealed class OrdinalComparer : MessageFormattableComparer
	{
		private System.Globalization.CultureInfo _culture;

		private bool _ignoreCase;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="culture"></param>
		/// <param name="ignoreCase"></param>
		public OrdinalComparer(System.Globalization.CultureInfo culture, bool ignoreCase)
		{
			_culture = culture;
			_ignoreCase = ignoreCase;
		}

		/// <summary>
		/// Compar as instancias informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override int Compare(IMessageFormattable x, IMessageFormattable y)
		{
			if(object.ReferenceEquals(x, y))
				return 0;
			if(x == null)
				return -1;
			if(y == null)
				return 1;
			var xStr = x.Format(_culture);
			var yStr = y.Format(_culture);
			if(_ignoreCase)
				return StringComparer.OrdinalIgnoreCase.Compare(xStr, yStr);
			return string.CompareOrdinal(xStr, yStr);
		}

		/// <summary>
		/// Verifica se é a mesma instancia do comparador.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			OrdinalComparer comparer = obj as OrdinalComparer;
			if(comparer == null)
				return false;
			return (_ignoreCase == comparer._ignoreCase);
		}

		/// <summary>
		/// Verifica se as instancias informadas são iguais.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals(IMessageFormattable x, IMessageFormattable y)
		{
			if(object.ReferenceEquals(x, y))
				return true;
			if((x == null) || (y == null))
				return false;
			if(!_ignoreCase)
				return x.Equals(y);
			var xStr = x.Format(_culture);
			var yStr = y.Format(_culture);
			if(xStr.Length != yStr.Length)
				return false;
			return (StringComparer.OrdinalIgnoreCase.Compare(xStr, yStr) == 0);
		}

		/// <summary>
		/// Recupera o hashcode do comparador.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			int hashCode = "OrdinalComparer".GetHashCode();
			if(!_ignoreCase)
				return hashCode;
			return ~hashCode;
		}

		/// <summary>
		/// Recupera o hashcode da mensagem informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override int GetHashCode(IMessageFormattable obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			if(_ignoreCase)
				return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Format(_culture));
			return obj.GetHashCode();
		}
	}
}
