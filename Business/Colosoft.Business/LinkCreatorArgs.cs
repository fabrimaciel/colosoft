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
	/// Argumentos para o método que cria uma entidade de link.
	/// </summary>
	public class LinkCreatorArgs : EventArgs
	{
		/// <summary>
		/// Contexto de origem usado na consulta.
		/// </summary>
		public Colosoft.Query.ISourceContext SourceContext
		{
			get;
			private set;
		}

		/// <summary>
		/// Instancia do modelo de dados do filho.
		/// </summary>
		public Colosoft.Data.IModel ChildDataModel
		{
			get;
			private set;
		}

		/// <summary>
		/// Identifica se é para ser feita a carga tardia.
		/// </summary>
		public bool IsLazy
		{
			get;
			private set;
		}

		/// <summary>
		/// Contexto da interface com o usuário
		/// </summary>
		public string UIContext
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="sourceContext"></param>
		/// <param name="childDataModel"></param>
		/// <param name="isLazy"></param>
		/// <param name="uiContext"></param>
		public LinkCreatorArgs(Colosoft.Query.ISourceContext sourceContext, Colosoft.Data.IModel childDataModel, bool isLazy, string uiContext)
		{
			this.SourceContext = sourceContext;
			this.ChildDataModel = childDataModel;
			this.IsLazy = isLazy;
			this.UIContext = uiContext;
		}
	}
	/// <summary>
	/// Assinatura do método usado para criar um link com base 
	/// no modelo de dados do filho.
	/// </summary>
	/// <param name="e"></param>
	/// <returns></returns>
	public delegate IEntity LinkCreatorHandler (LinkCreatorArgs e);
}
