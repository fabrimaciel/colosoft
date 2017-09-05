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

namespace Colosoft
{
	/// <summary>
	/// Enumerador com o status do item
	/// </summary>
	public enum ItemStatus : byte
	{
		/// <summary>
		/// Captado
		/// </summary>
		Taked = 0,
		/// <summary>
		/// Diagramada
		/// </summary>
		Diagrammed = 1,
		/// <summary>
		/// Produzido
		/// </summary>
		Produced = 2,
		/// <summary>
		/// Não diagramada.
		/// </summary>
		NoDiagrammed = 3,
		/// <summary>
		/// Não produzida.
		/// </summary>
		NoProduced = 4,
		/// <summary>
		/// Bloqueada.
		/// </summary>
		Blocked = 5,
		/// <summary>
		/// Cancelada.
		/// </summary>
		Cancelled = 6
	}
	/// <summary>
	/// Status do item (<see cref="ItemStatus"/>) + ativo (!= 6) e livre (&lt; 5).
	/// </summary>
	public enum ItemStatusExtended : int
	{
		/// <summary>
		/// Ativo (ItemStatus != 6).
		/// </summary>
		/// 
		Active = -2,
		/// <summary>
		/// Livre (ItemStatus &lt; 5).
		/// </summary>
		Free = -1,
		/// <summary>
		/// Captado.
		/// </summary>
		Taked = 0,
		/// <summary>
		/// Diagramado.
		/// </summary>
		Diagrammed = 1,
		/// <summary>
		/// Produzido.
		/// </summary>
		Produced = 2,
		/// <summary>
		/// Não diagramado.
		/// </summary>
		NotDiagrammed = 3,
		/// <summary>
		/// Não produzido.
		/// </summary>
		NotProduced = 4,
		/// <summary>
		/// Bloqueado.
		/// </summary>
		Blocked = 5,
		/// <summary>
		/// Cancelado.
		/// </summary>
		Cancelled = 6
	}
	/// <summary>
	/// Extensões para ItemStatus.
	/// </summary>
	public static class ItemStatusExtensions
	{
		static readonly IEnumerable<ItemStatus> _valuesList;

		static readonly IEnumerable<ItemStatusExtended> _extendedValuesList;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static ItemStatusExtensions()
		{
			_valuesList = Enum.GetValues(typeof(ItemStatus)).Cast<ItemStatus>().OrderBy(e => e).ToList();
			_extendedValuesList = Enum.GetValues(typeof(ItemStatusExtended)).Cast<ItemStatusExtended>().OrderBy(e => e).ToList();
		}

		/// <summary>
		/// Indica se o status possui um <see cref="ItemStatus"/> correspondente.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsStandard(this ItemStatusExtended value)
		{
			return ((int)value) >= 0;
		}

		/// <summary>
		/// Indica se o status corresponde ao status extendido "ativo".
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsActive(this ItemStatusExtended value)
		{
			return value == ItemStatusExtended.Active;
		}

		/// <summary>
		/// Indica se o status corresponde ao status extendido "livre".
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsFree(this ItemStatusExtended value)
		{
			return value == ItemStatusExtended.Free;
		}

		/// <summary>
		/// Converte o status para o tipo <see cref="ItemStatus"/>.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ItemStatus Convert(this ItemStatusExtended value)
		{
			return (ItemStatus)value;
		}

		/// <summary>
		/// Sequence (ordered by value) of values of the <see cref="ItemStatus"/> type.
		/// </summary>
		public static IEnumerable<ItemStatus> ValuesList
		{
			get
			{
				return _valuesList;
			}
		}

		/// <summary>
		/// Sequence (ordered by value) of values of the <see cref="ItemStatusExtended"/> type.
		/// </summary>
		public static IEnumerable<ItemStatusExtended> ExtendedValuesList
		{
			get
			{
				return _extendedValuesList;
			}
		}
	}
}
