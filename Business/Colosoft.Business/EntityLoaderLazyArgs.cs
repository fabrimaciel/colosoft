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

namespace Colosoft.Business
{
	/// <summary>
	/// Armazena os argumento que serão usados na carga tardia da entidade.
	/// </summary>
	public class EntityLoaderLazyArgs
	{
		private string[] _names;

		private Direction _direction;

		/// <summary>
		/// Cria o argumento com os nomes que serão inclusos na carga tardia.
		/// </summary>
		/// <param name="names"></param>
		public EntityLoaderLazyArgs(params string[] names)
		{
			_direction = Direction.Include;
			_names = names ?? new string[0];
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="names"></param>
		public EntityLoaderLazyArgs(Direction direction, params string[] names)
		{
			_direction = direction;
			_names = names ?? new string[0];
		}

		/// <summary>
		/// Direção de inclusão dos nomes.
		/// </summary>
		public enum Direction
		{
			/// <summary>
			/// É para incluir os nomes para a carga tardia.
			/// </summary>
			Include,
			/// <summary>
			/// É para excluir os nomes da cargar tardia.
			/// </summary>
			Exclude
		}
	}
}
