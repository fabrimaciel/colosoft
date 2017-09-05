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
	/// Implementação padrão da interface ITraceableEntity.
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
	public abstract class SimpleTraceableEntity<TModel> : Entity<TModel>, ISimpleTraceableEntity<TModel> where TModel : class, Data.ISimpleTraceableModel, new()
	{
		/// <summary>
		/// Data de criação do registro.
		/// </summary>
		public DateTimeOffset CreatedDate
		{
			get
			{
				return DataModel.CreatedDate;
			}
			set
			{
				if(DataModel.CreatedDate != value && RaisePropertyChanging("CreatedDate", value))
				{
					DataModel.CreatedDate = value;
					RaisePropertyChanged("CreatedDate");
				}
			}
		}

		/// <summary>
		/// Identificador do perfil usado na criação dos dados.
		/// </summary>
		public int CreatedProfileId
		{
			get
			{
				return DataModel.CreatedProfileId;
			}
			set
			{
				if(DataModel.CreatedProfileId != value && RaisePropertyChanging("CreatedProfileId", value))
				{
					DataModel.CreatedProfileId = value;
					RaisePropertyChanged("CreatedProfileId");
				}
			}
		}

		/// <summary>
		/// Informações do perfil que criou a entidade.
		/// </summary>
		public ITraceableEntityProfileInfo CreatedProfile
		{
			get
			{
				var provider = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ITraceableEntityProfileInfoProvider>();
				return provider.GetProfileInfo(this);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="args"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		protected SimpleTraceableEntity(Colosoft.Business.EntityLoaderCreatorArgs<TModel> args) : base(args.DataModel, args.UIContext, args.TypeManager)
		{
			InitializeInternal(args.DataModel);
		}

		/// <summary>
		/// Cria a instancia com os dados do modelo de dados.
		/// </summary>
		/// <param name="dataModel"></param>
		/// <param name="uiContext"></param>
		/// <param name="entityTypeManager"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public SimpleTraceableEntity(TModel dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager) : base(dataModel, uiContext, entityTypeManager)
		{
			InitializeInternal(dataModel);
		}

		/// <summary>
		/// Construtor interno
		/// </summary>
		/// <param name="dataModel"></param>
		/// <param name="uiContext"></param>
		/// <param name="initialize">Identifica se é para inicializar.</param>
		/// <param name="entityTypeManager"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		protected SimpleTraceableEntity(TModel dataModel, string uiContext, bool initialize, IEntityTypeManager entityTypeManager) : base(dataModel, uiContext, initialize, entityTypeManager)
		{
			InitializeInternal(dataModel);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="dataModel"></param>
		private void InitializeInternal(TModel dataModel)
		{
			if(dataModel == null || dataModel.CreatedProfileId == 0)
				TraceableEntityHelper.InitializeTraceableDataModel(DataModel);
		}

		/// <summary>
		/// Método acioando quando uma propriedade da instancia pai for alterada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OwnerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OwnerPropertyChanged(sender, e);
			if(e.PropertyName == "IsActive")
				RaisePropertyChanged(e.PropertyName);
		}

		/// <summary>
		/// Copia os dados básicos.
		/// </summary>
		/// <param name="to"></param>
		public override void CopyBasicData(IEntity<TModel> to)
		{
			base.CopyBasicData(to);
			if(to is SimpleTraceableEntity<TModel>)
				TraceableEntityHelper.InitializeTraceableEntity((SimpleTraceableEntity<TModel>)to);
		}
	}
}
