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
using System.Globalization;

namespace Colosoft.Text
{
	/// <summary>
	/// Implementação do comparador que ignora cultura.
	/// </summary>
	[Serializable]
	sealed class CultureAwareComparer : MessageFormattableComparer
	{
		private System.Globalization.CultureInfo _culture;

		private System.Globalization.CompareInfo _compareInfo;

		private bool _ignoreCase;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="culture"></param>
		/// <param name="ignoreCase"></param>
		public CultureAwareComparer(System.Globalization.CultureInfo culture, bool ignoreCase)
		{
			_culture = culture;
			_compareInfo = culture.CompareInfo;
			_ignoreCase = ignoreCase;
		}

		/// <summary>
		/// Compara as instancias informadas.
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
			return _compareInfo.Compare(x.Format(_culture), y.Format(_culture), _ignoreCase ? System.Globalization.CompareOptions.IgnoreCase : System.Globalization.CompareOptions.None);
		}

		/// <summary>
		/// Verifica se a instancia é equivalente a atual.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			CultureAwareComparer comparer = obj as CultureAwareComparer;
			if(comparer == null)
				return false;
			return ((this._ignoreCase == comparer._ignoreCase) && _compareInfo.Equals(comparer._compareInfo));
		}

		/// <summary>
		/// Compara as instancias informadas.
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
			return (_compareInfo.Compare(x.Format(_culture), y.Format(_culture), _ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None) == 0);
		}

		/// <summary>
		/// Recupera o hashcode da instancia.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			int hashCode = this._compareInfo.GetHashCode();
			if(!this._ignoreCase)
				return hashCode;
			return ~hashCode;
		}

		/// <summary>
		/// Recupera o hashcode da mensagem inforamda.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override int GetHashCode(IMessageFormattable obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			if(this._ignoreCase)
				return StringComparer.InvariantCultureIgnoreCase.GetHashCode(obj.Format(_culture));
			return StringComparer.InvariantCulture.GetHashCode(obj.Format(_culture));
		}
	}
}
