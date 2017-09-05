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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Implementação de um agregador de repositorios de informações de assemblies.
	/// </summary>
	public class AssemblyInfoRepositoryAggregate : IAssemblyInfoRepository
	{
		private List<IAssemblyInfoRepository> _assemblyInfoRepositories;

		private bool _isLoaded = false;

		/// <summary>
		/// Evento acionado quando o repositório for carregado.
		/// </summary>
		public event EventHandler Loaded;

		/// <summary>
		/// Quantidade de assemblies carregados.
		/// </summary>
		public int Count
		{
			get
			{
				var count = 0;
				foreach (var r in _assemblyInfoRepositories)
					count += r.Count;
				return count;
			}
		}

		/// <summary>
		/// Identifica se algum repositorio sofreu alteração.
		/// </summary>
		public bool IsChanged
		{
			get
			{
				foreach (var r in _assemblyInfoRepositories)
					if(r.IsChanged)
						return true;
				return false;
			}
		}

		/// <summary>
		/// Identifica se a instancia já foi carregada.
		/// </summary>
		public bool IsLoaded
		{
			get
			{
				return _isLoaded;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="assemblyInfoRepositories">Enumerador de informação de assemblies.</param>
		public AssemblyInfoRepositoryAggregate(IEnumerable<IAssemblyInfoRepository> assemblyInfoRepositories)
		{
			assemblyInfoRepositories.Require("assemblyInfoRepositories").NotNull();
			_assemblyInfoRepositories = new List<IAssemblyInfoRepository>();
			foreach (var i in assemblyInfoRepositories)
			{
				i.Loaded += new EventHandler(EntryLoaded);
				_assemblyInfoRepositories.Add(i);
			}
		}

		/// <summary>
		/// Método acionado quando uma entrada for carregada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EntryLoaded(object sender, EventArgs e)
		{
			lock (_assemblyInfoRepositories)
				_isLoaded = _assemblyInfoRepositories.Where(f => f.IsLoaded).Count() == _assemblyInfoRepositories.Count;
			if(_isLoaded)
				OnLoaded();
		}

		/// <summary>
		/// Método acionado quando a instancia for carregada.
		/// </summary>
		protected void OnLoaded()
		{
			if(Loaded != null)
				Loaded(this, EventArgs.Empty);
		}

		/// <summary>
		/// Atualiza o repositório.
		/// </summary>
		/// <param name="executeAnalyzer">Identifica se é para executar o analizador.</param>
		public void Refresh(bool executeAnalyzer)
		{
			foreach (var r in _assemblyInfoRepositories)
				r.Refresh(executeAnalyzer);
		}

		/// <summary>
		/// Recupera as informações do assembly pelo nome informado.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <param name="assemblyInfo"></param>
		/// <param name="exception">Error ocorrido.</param>
		/// <returns>True se as informações do assembly forem recuperadas com sucesso.</returns>
		public bool TryGet(string assemblyName, out AssemblyInfo assemblyInfo, out Exception exception)
		{
			foreach (var r in _assemblyInfoRepositories)
				if(r.TryGet(assemblyName, out assemblyInfo, out exception))
					return true;
			assemblyInfo = null;
			exception = null;
			return false;
		}

		/// <summary>
		/// Verifica se no repositório existe as informações do assembly informado.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		public bool Contains(string assemblyName)
		{
			foreach (var r in _assemblyInfoRepositories)
				if(r.Contains(assemblyName))
					return true;
			return false;
		}

		/// <summary>
		/// Recupera o enumerador da instancia.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			foreach (var r in _assemblyInfoRepositories)
				foreach (var j in r)
					yield return j;
		}

		/// <summary>
		/// Recupera o enumerador da instancia.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<AssemblyInfo> GetEnumerator()
		{
			foreach (var r in _assemblyInfoRepositories)
				foreach (var j in r)
					yield return j;
		}
	}
}
