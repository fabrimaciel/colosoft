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
using MefContrib.Hosting.Conventions.Configuration;

namespace Colosoft.Mef
{
	/// <summary>
	/// Método para auxiliar na parte de convensões.
	/// </summary>
	public class PartConventionBuilder : MefContrib.Hosting.Conventions.IPartRegistryLocator, IEnumerable<IPartRegistry<MefContrib.Hosting.Conventions.IContractService>>
	{
		private List<IPartRegistry<MefContrib.Hosting.Conventions.IContractService>> _registries = new List<IPartRegistry<MefContrib.Hosting.Conventions.IContractService>>();

		private static System.Reflection.MethodInfo _addMethod;

		private static System.Reflection.MethodInfo _addImportingConstructorMethod;

		/// <summary>
		/// Adiciona uma nova parte.
		/// </summary>
		/// <typeparam name="TPart"></typeparam>
		/// <typeparam name="TExportAs"></typeparam>
		/// <param name="makeNoShared"></param>
		public PartConventionBuilder Add<TPart, TExportAs>(bool makeNoShared = false)
		{
			var part1 = new MefContrib.Hosting.Conventions.Configuration.PartRegistry();
			part1.Scan(f => f.Assembly(typeof(TPart).Assembly));
			var rr = part1.Part<TPart>().ExportAs<TExportAs>();
			_registries.Add(part1);
			if(makeNoShared)
				rr.MakeNonShared();
			else
				rr.MakeShared();
			return this;
		}

		/// <summary>
		/// Adiciona uma nova parte.
		/// </summary>
		/// <param name="partType"></param>
		/// <param name="exportAsType"></param>
		/// <param name="makeNoShared"></param>
		/// <returns></returns>
		public PartConventionBuilder Add(Type partType, Type exportAsType, bool makeNoShared)
		{
			partType.Require("partType").NotNull();
			exportAsType.Require("exportAsType").NotNull();
			if(_addMethod == null)
				_addMethod = typeof(PartConventionBuilder).GetMethod("Add", new Type[] {
					typeof(bool)
				});
			var method = _addMethod.MakeGenericMethod(partType, exportAsType);
			return (PartConventionBuilder)method.Invoke(this, new object[] {
				makeNoShared
			});
		}

		/// <summary>
		/// Adiciona uma nova parte com importação do construtor.
		/// </summary>
		/// <typeparam name="TPart"></typeparam>
		/// <typeparam name="TExportAs"></typeparam>
		/// <param name="makeNoShared"></param>
		/// <returns></returns>
		public PartConventionBuilder AddImportingConstructor<TPart, TExportAs>(bool makeNoShared = false)
		{
			var part1 = new MefContrib.Hosting.Conventions.Configuration.PartRegistry();
			part1.Scan(f => f.Assembly(typeof(TPart).Assembly));
			var rr = part1.Part<TPart>().ExportAs<TExportAs>().ImportConstructor();
			if(makeNoShared)
				rr.MakeNonShared();
			else
				rr.MakeShared();
			_registries.Add(part1);
			return this;
		}

		/// <summary>
		/// Adiciona um nova parte com importação do construtor.
		/// </summary>
		/// <param name="partType"></param>
		/// <param name="exportAsType"></param>
		/// <param name="makeNoShared"></param>
		/// <returns></returns>
		public PartConventionBuilder AddImportingConstructor(Type partType, Type exportAsType, bool makeNoShared)
		{
			partType.Require("partType").NotNull();
			exportAsType.Require("exportAsType").NotNull();
			if(_addImportingConstructorMethod == null)
				_addImportingConstructorMethod = typeof(PartConventionBuilder).GetMethod("AddImportingConstructor", new Type[] {
					typeof(bool)
				});
			var method = _addImportingConstructorMethod.MakeGenericMethod(partType, exportAsType);
			return (PartConventionBuilder)method.Invoke(this, new object[] {
				makeNoShared
			});
		}

		/// <summary>
		/// Recupera os registros.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IPartRegistry<MefContrib.Hosting.Conventions.IContractService>> GetRegistries()
		{
			return _registries;
		}

		/// <summary>
		/// Recupera o enumerador da instancia.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<IPartRegistry<MefContrib.Hosting.Conventions.IContractService>> GetEnumerator()
		{
			return _registries.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador da instancia.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _registries.GetEnumerator();
		}
	}
}
