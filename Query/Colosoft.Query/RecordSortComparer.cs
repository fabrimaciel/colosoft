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

namespace Colosoft.Query
{
	/// <summary>
	/// Implementação do comparador usado para a ordenação de registros.
	/// </summary>
	public class RecordSortComparer : IComparer<Record>
	{
		private List<Field> _fields;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="fields">Relação dos campos que serão usados no comparador.</param>
		public RecordSortComparer(IEnumerable<Field> fields)
		{
			fields.Require("fields").NotNull();
			_fields = new List<Field>(fields);
		}

		/// <summary>
		/// Compara dois <see cref="Record"/> baseado nas propriedades passadas.
		/// </summary>
		/// <param name="x">Primeiro objeto.</param>
		/// <param name="y">Segundo objeto.</param>
		/// <returns>Retorna negativo se x é menor que y, zero se for igual ou positivo se x é maior que y.</returns>
		public int Compare(Record x, Record y)
		{
			foreach (var field in _fields)
			{
				var xValue = x.GetValue(field.Index);
				var yValue = y.GetValue(field.Index);
				int comparison;
				if(xValue == null)
					return -1;
				if(yValue == null)
					return 1;
				if(xValue is string)
					comparison = Colosoft.Globalization.SystemStringComparer.Default.Compare((string)xValue, (string)yValue);
				else
					comparison = Colosoft.Globalization.SystemComparer.Default.Compare(xValue, yValue);
				if(comparison != 0)
					return field.Reverse ? comparison * -1 : comparison;
			}
			return 0;
		}

		/// <summary>
		/// Armazena os dados do campo para a ordenação.
		/// </summary>
		public class Field
		{
			/// <summary>
			/// Indice do campo do registro.
			/// </summary>
			public int Index
			{
				get;
				private set;
			}

			/// <summary>
			/// Identifica se é para realizar uma ordenação reversa.
			/// </summary>
			public bool Reverse
			{
				get;
				private set;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="index"></param>
			/// <param name="reverse"></param>
			public Field(int index, bool reverse)
			{
				Index = index;
				Reverse = reverse;
			}
		}
	}
}
