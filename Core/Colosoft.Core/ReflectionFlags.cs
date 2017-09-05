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

using System.Reflection;

namespace Colosoft.Reflection
{
	class ReflectionFlags
	{
		/// <summary>
		/// Search criteria encompassing all public and non-public members.
		/// </summary>
		public static readonly BindingFlags DefaultCriteria = BindingFlags.Public | BindingFlags.NonPublic;

		/// <summary>
		/// Search criteria encompassing all public and non-public instance members.
		/// </summary>
		public static readonly BindingFlags InstanceCriteria = DefaultCriteria | BindingFlags.Instance;

		/// <summary>
		/// Search criteria encompassing all public and non-public static members, including those of parent classes.
		/// </summary>
		public static readonly BindingFlags StaticCriteria = DefaultCriteria | BindingFlags.Static | BindingFlags.FlattenHierarchy;

		/// <summary>
		/// Search criteria encompassing all members, including those of parent classes.
		/// </summary>
		public static readonly BindingFlags AllCriteria = InstanceCriteria | StaticCriteria;
	}
}
