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

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Implementação básica do <see cref="IColumnMetadata"/>
	/// </summary>
	public abstract class BaseColumnMetaData : IColumnMetadata
	{
		private string _name;

		private string _typeName;

		private int _columnSize;

		private int _numericalPrecision;

		private bool _isNullable;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="rs"></param>
		public BaseColumnMetaData(System.Data.DataRow rs)
		{
		}

		/// <summary>
		/// Nome da coluna.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			protected set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Nome do tipo da coluna.
		/// </summary>
		public string TypeName
		{
			get
			{
				return _typeName;
			}
			protected set
			{
				_typeName = value;
			}
		}

		/// <summary>
		/// Tamanho da coluna.
		/// </summary>
		public int ColumnSize
		{
			get
			{
				return _columnSize;
			}
			protected set
			{
				_columnSize = value;
			}
		}

		/// <summary>
		/// Precisão numérica da coluna.
		/// </summary>
		public int NumericalPrecision
		{
			get
			{
				return _numericalPrecision;
			}
			protected set
			{
				_numericalPrecision = value;
			}
		}

		/// <summary>
		/// Identifica se a coluna suporta valores nulos.
		/// </summary>
		public bool IsNullable
		{
			get
			{
				return _isNullable;
			}
			protected set
			{
				_isNullable = value;
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "ColumnMetadata(" + _name + ')';
		}
	}
}
