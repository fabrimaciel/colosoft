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
using System.Collections;

namespace Colosoft.Business
{
	/// <summary>
	/// Implementação padrão da interface IEntityChildrenList.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
	public class EntityChildrenList<TEntity> : BaseEntityList<TEntity>, IEntityChildrenList<TEntity> where TEntity : IEntity
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter">Método usado para definir o identificador unico do pai para as entidades filas da lista.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		public EntityChildrenList(string uiContext, Action<TEntity> parentUidSetter, IEntityTypeManager entityTypeManager = null) : base(uiContext, parentUidSetter, entityTypeManager)
		{
		}

		/// <summary>
		/// Cria uma instancia já definindos os itens iniciais.
		/// </summary>
		/// <param name="items">Itens que serão usados na inicialização.</param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter"></param>
		/// <param name="entityTypeManager"></param>
		public EntityChildrenList(IEnumerable<TEntity> items, string uiContext, Action<TEntity> parentUidSetter, IEntityTypeManager entityTypeManager = null) : base(items, uiContext, parentUidSetter, entityTypeManager)
		{
		}

		/// <summary>
		/// Cria uma instancia definindo o parametro de carga tardia dos itens.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter"></param>
		/// <param name="entityTypeManager"></param>
		public EntityChildrenList(Lazy<IEnumerable<TEntity>> items, string uiContext, Action<TEntity> parentUidSetter, IEntityTypeManager entityTypeManager = null) : base(items, uiContext, parentUidSetter, entityTypeManager)
		{
		}

		/// <summary>
		/// Cria uma instancia a partir dos dados clonados.
		/// </summary>
		/// <param name="itemsLoader"></param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter"></param>
		/// <param name="entityTypeManager"></param>
		protected EntityChildrenList(Func<BaseEntityList<TEntity>, IEnumerable<TEntity>> itemsLoader, string uiContext, Action<TEntity> parentUidSetter, IEntityTypeManager entityTypeManager) : base(itemsLoader, uiContext, parentUidSetter, entityTypeManager)
		{
		}

		/// <summary>
		/// Método acioando quando a coleção interna sofrer alguam alteração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnInnerListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			base.OnInnerListCollectionChanged(sender, e);
		}

		/// <summary>
		/// Descarta o novo item pendente na coleção.
		/// </summary>
		/// <param name="itemIndex">Indice do item adicionado anteriormente na coleção.</param>
		void System.ComponentModel.ICancelAddNew.CancelNew(int itemIndex)
		{
			RemoveAt(itemIndex);
		}

		/// <summary>
		/// Confirma a inclusão do novo item na coleção.
		/// </summary>
		/// <param name="itemIndex">Indice do item adicionado.</param>
		void System.ComponentModel.ICancelAddNew.EndNew(int itemIndex)
		{
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public override object Clone()
		{
			EntityChildrenList<TEntity> result = null;
			result = new EntityChildrenList<TEntity>(f => InnerList.Select(item =>  {
				return CloneItem(f, item);
			}), UIContext, ParentUidSetter, TypeManager);
			CloneControls(result);
			((IConnectedEntity)result).Connect(((IConnectedEntity)this).SourceContext);
			return result;
		}

		/// <summary>
		/// Cria um nova instancia da lista.
		/// Esse método é usado por reflexão.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter"></param>
		/// <param name="entityTypeManager"></param>
		/// <returns></returns>
		internal static EntityChildrenList<TEntity> CreateInstance(IEnumerable<IEntity> items, string uiContext, Action<IEntity> parentUidSetter, IEntityTypeManager entityTypeManager)
		{
			items.Require("items").NotNull();
			return new EntityChildrenList<TEntity>(new Collections.NotifyCollectionChangedObserverRegisterEnumerable<TEntity>(items), uiContext, new Action<TEntity>(e => parentUidSetter(e)), entityTypeManager);
		}

		/// <summary>
		/// Cria um nova instancia da lista.
		/// Esse método é usado por reflexão.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="uiContext"></param>
		/// <param name="parentUidSetter"></param>
		/// <param name="entityTypeManager"></param>
		/// <returns></returns>
		internal static EntityChildrenList<TEntity> CreateLazyInstance(Lazy<IEnumerable<IEntity>> items, string uiContext, Action<IEntity> parentUidSetter, IEntityTypeManager entityTypeManager)
		{
			return new EntityChildrenList<TEntity>(new Lazy<IEnumerable<TEntity>>(() => new Collections.NotifyCollectionChangedObserverRegisterEnumerable<TEntity>(items.Value)), uiContext, new Action<TEntity>(e => parentUidSetter(e)), entityTypeManager);
		}
	}
}
