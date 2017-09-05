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
	/// Classe base para itens que suporte notificação de propriedade.
	/// </summary>
	#if SILVERLIGHT
	        [System.Runtime.Serialization.DataContract]
#endif
	[Serializable]
	public abstract class NotificationObject : System.ComponentModel.INotifyPropertyChanged
	{
		/// <summary>
		/// Raised when a property on this object has a new value.
		/// </summary>        
		#if !SILVERLIGHT
		[field: NonSerialized]
		#endif
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">The property that has a new value.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Method used to raise an event")]
		protected virtual void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
			if(handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Raises this object's PropertyChanged event for each of the properties.
		/// </summary>
		/// <param name="propertyNames">The properties that have a new value.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Method used to raise an event")]
		protected void RaisePropertyChanged(params string[] propertyNames)
		{
			if(propertyNames == null)
				throw new ArgumentNullException("propertyNames");
			foreach (var name in propertyNames)
			{
				this.RaisePropertyChanged(name);
			}
		}

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <typeparam name="T">The type of the property that has a new value</typeparam>
		/// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Method used to raise an event")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Cannot change the signature")]
		protected void RaisePropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> propertyExpression)
		{
			var propertyName = PropertySelector<T>.ExtractPropertyName(propertyExpression);
			this.RaisePropertyChanged(propertyName);
		}
	}
}
