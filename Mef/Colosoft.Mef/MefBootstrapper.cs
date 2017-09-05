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
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;

namespace Colosoft.Mef
{
	/// <summary>
	/// Implementação do bootstrapping usadando o MEF.
	/// </summary>
	public class MefBootstrapper : Colosoft.Bootstrapper
	{
		/// <summary>
		/// Recupera a define o <see cref="AggregateCatalog"/> padrão para a aplicação. 
		/// </summary>
		protected AggregateCatalog AggregateCatalog
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera e define <see cref="CompositionContainer"/> padrão para a aplicação.
		/// </summary>
		protected CompositionContainer Container
		{
			get;
			set;
		}

		/// <summary>
		/// Executa o Bootstrapper
		/// </summary>
		public override void Run()
		{
			this.AggregateCatalog = this.CreateAggregateCatalog();
			this.ConfigureAggregateCatalog();
			this.RegisterDefaultTypesIfMissing();
			this.Container = this.CreateContainer();
			if(this.Container == null)
				throw new InvalidOperationException("NullCompositionContainerException");
			this.ConfigureContainer();
			this.ConfigureServiceLocator();
		}

		/// <summary>
		/// Configura o Locator
		/// </summary>
		protected override void ConfigureServiceLocator()
		{
			Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator = new Microsoft.Mef.CommonServiceLocator.MefServiceLocator(Container);
			Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(delegate {
				return serviceLocator;
			});
		}

		/// <summary>
		/// Registra os tipos.
		/// </summary>
		protected virtual void RegisterBootstrapperProvidedTypes()
		{
			this.Container.ComposeExportedValue<Microsoft.Practices.ServiceLocation.IServiceLocator>(new ServiceLocatorAdapter(this.Container));
			this.Container.ComposeExportedValue<AggregateCatalog>(this.AggregateCatalog);
		}

		/// <summary>
		/// Registra os tipos padrão.
		/// </summary>
		public virtual void RegisterDefaultTypesIfMissing()
		{
			this.AggregateCatalog = DefaultServiceRegister.RegisterRequiredServicesIfMissing(this.AggregateCatalog);
		}

		/// <summary>
		/// Configura o <see cref="AggregateCatalog"/> usado pelo MEF.
		/// </summary>
		/// <returns>Um <see cref="AggregateCatalog"/> para ser usado pelo bootstrapper.</returns>
		protected virtual AggregateCatalog CreateAggregateCatalog()
		{
			return new AggregateCatalog();
		}

		/// <summary>
		/// Configura o <see cref="AggregateCatalog"/> usado pelo MEF.
		/// </summary>
		protected virtual void ConfigureAggregateCatalog()
		{
		}

		/// <summary>
		/// Cria o <see cref="CompositionContainer"/> que será usado como container padrão.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The default export provider is in the container and disposed by MEF.")]
		protected virtual CompositionContainer CreateContainer()
		{
			return new CompositionContainer(this.AggregateCatalog, new ExportProvider[0]);
		}

		/// <summary>
		/// Configura o <see cref="CompositionContainer"/>. 
		/// Esse método deve ser sobreescrito pela classe derivada para adicionar tipos específicos requeridos pela aplicação.
		/// </summary>
		protected virtual void ConfigureContainer()
		{
			RegisterBootstrapperProvidedTypes();
		}
	}
}
