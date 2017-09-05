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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Implementação de um DataSource que trata lista de valores.
	/// </summary>
	public class CollectionDataSource : DataSourceControl
	{
		/// <summary>
		/// Dados associados
		/// </summary>
		private System.Collections.IEnumerable _items;

		private CollectionDataSourceView _view;

		private System.Collections.ICollection _viewNames;

		/// <summary>
		/// Itens associados.
		/// </summary>
		[DefaultValue(""), Category("Data"), Description("Collection items")]
		public System.Collections.IEnumerable Items
		{
			get
			{
				return _items;
			}
			set
			{
				_items = value;
			}
		}

		/// <summary>
		/// Recupera a visualização da instancia.
		/// </summary>
		/// <returns></returns>
		private CollectionDataSourceView GetView()
		{
			if(_view == null)
				_view = new CollectionDataSourceView(this, "DefaultView");
			return _view;
		}

		/// <summary>
		/// Recupera os nomes das possíveis views.
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.ICollection GetViewNames()
		{
			if(_viewNames == null)
				_viewNames = new string[] {
					"DefaultView"
				};
			return _viewNames;
		}

		/// <summary>
		/// Recupera a view do DataSource.
		/// </summary>
		/// <param name="viewName"></param>
		/// <returns></returns>
		protected override DataSourceView GetView(string viewName)
		{
			if((viewName == null) || ((viewName.Length != 0) && !string.Equals(viewName, "DefaultView", StringComparison.OrdinalIgnoreCase)))
				throw new ArgumentException("InvalidViewName");
			return this.GetView();
		}

		/// <summary>
		/// Implementação da visão do datasource.
		/// </summary>
		class CollectionDataSourceView : DataSourceView
		{
			private CollectionDataSource _owner;

			/// <summary>
			/// Identifica se a instancia suporta a operação de exclusão.
			/// </summary>
			public override bool CanDelete
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Identifica se a instancia suporta a operação de inserção
			/// </summary>
			public override bool CanInsert
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Identifica se a instancia suporta paginar o resultado.
			/// </summary>
			public override bool CanPage
			{
				get
				{
					return true;
				}
			}

			/// <summary>
			/// Identifica se a instancia suporta receber a quantidade total de linhas
			/// através de um recurso especifico.
			/// </summary>
			public override bool CanRetrieveTotalRowCount
			{
				get
				{
					return true;
				}
			}

			/// <summary>
			/// Identifica se a instancia suporta ordenação.
			/// </summary>
			public override bool CanSort
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Identifica se a instancia pode atualizar os dados.
			/// </summary>
			public override bool CanUpdate
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner"></param>
			public CollectionDataSourceView(CollectionDataSource owner, string name) : base(owner, "DefaultView")
			{
				_owner = owner;
			}

			/// <summary>
			/// Recupera a quantidade de itens.
			/// </summary>
			/// <returns></returns>
			private int Count()
			{
				if(_owner.Items == null)
					return 0;
				var collection = _owner as System.Collections.ICollection;
				if(collection != null)
					return collection.Count;
				int num = 0;
				var enumerator = _owner.Items.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
						num++;
				}
				finally
				{
					if(enumerator is IDisposable)
						((IDisposable)enumerator).Dispose();
				}
				return num;
			}

			/// <summary>
			/// Cria o resultado da lista de dados.
			/// </summary>
			/// <param name="list"></param>
			/// <param name="arguments"></param>
			/// <returns></returns>
			private IEnumerable<object> CreateListData(System.Collections.IList list, DataSourceSelectArguments arguments)
			{
				for(int i = arguments.StartRowIndex; i < list.Count && (arguments.MaximumRows == 0 || i < arguments.StartRowIndex + arguments.MaximumRows); i++)
				{
					object item = null;
					try
					{
						item = list[i];
					}
					catch(System.ArgumentOutOfRangeException)
					{
						yield break;
					}
					yield return item;
				}
			}

			/// <summary>
			/// Recupera a Enumeração do dados da consulta.
			/// </summary>
			/// <param name="dataObject"></param>
			/// <param name="arguments"></param>
			/// <returns></returns>
			private System.Collections.IEnumerable CreateEnumerableData(object dataObject, DataSourceSelectArguments arguments)
			{
				var enumerable = dataObject as System.Collections.IEnumerable;
				if(enumerable != null)
				{
					var list = enumerable as System.Collections.IList;
					if(list != null)
					{
						arguments.RetrieveTotalRowCount = true;
						arguments.TotalRowCount = list.Count;
						return CreateListData(list, arguments);
					}
					if(arguments.StartRowIndex >= 0 && arguments.MaximumRows > 0)
					{
						var enumerator = enumerable.GetEnumerator();
						var index = 0;
						var result = new System.Collections.ArrayList();
						var total = arguments.MaximumRows;
						while (enumerator.MoveNext() && total > 0)
						{
							if(index == arguments.StartRowIndex)
							{
								total--;
								result.Add(enumerator.Current);
							}
							else
								index++;
						}
						if(enumerator is IDisposable)
							((IDisposable)enumerator).Dispose();
						return result;
					}
					else
					{
						var enumerator = enumerable.GetEnumerator();
						var result = new System.Collections.ArrayList();
						while (enumerator.MoveNext())
							result.Add(enumerator.Current);
						if(enumerator is IDisposable)
							((IDisposable)enumerator).Dispose();
						return result;
					}
				}
				return new object[] {
					dataObject
				};
			}

			/// <summary>
			/// Executa a consulta para recupera os itens.
			/// </summary>
			/// <param name="arguments"></param>
			/// <returns></returns>
			protected override System.Collections.IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
			{
				if(CanPage)
					arguments.AddSupportedCapabilities(DataSourceCapabilities.Page);
				if(CanRetrieveTotalRowCount)
					arguments.AddSupportedCapabilities(DataSourceCapabilities.RetrieveTotalRowCount);
				if(arguments.RetrieveTotalRowCount)
					arguments.TotalRowCount = Count();
				if(_owner.Items is Colosoft.Collections.IVirtualList && arguments.MaximumRows > 0)
				{
					var virtualList = (Colosoft.Collections.IVirtualList)_owner.Items;
					virtualList.Configure(arguments.MaximumRows);
				}
				return CreateEnumerableData(_owner.Items, arguments);
			}
		}
	}
}
