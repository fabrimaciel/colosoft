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
using Colosoft.Query.Linq.Translators;

namespace Colosoft.Query.Linq
{
	/// <summary>
	/// Implementação básica do provedor da origem.
	/// </summary>
	public abstract class SourceProvider : ISourceProvider, IDisposable
	{
		private bool _isDisposed;

		private Type _resultMapperType = typeof(Record);

		/// <summary>
		/// Tipo mapeado para o resultado.
		/// </summary>
		public Type ResultMapperType
		{
			get
			{
				return _resultMapperType;
			}
			set
			{
				_resultMapperType = value;
			}
		}

		/// <summary>
		/// This is very the query is executed and the results are mapped to objects
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public virtual object Execute(System.Linq.Expressions.Expression query)
		{
			CheckDispose();
			var translator = new QueryTranslator();
			QueryInfo info = translator.Translate(query);
			return Execute(info);
		}

		/// <summary>
		/// Executa o consulta.
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		protected abstract object Execute(QueryInfo info);

		/// <summary>
		/// Verifica se a instancia já foi liberada.
		/// </summary>
		protected virtual void CheckDispose()
		{
			if(_isDisposed)
				throw new ObjectDisposedException(typeof(SourceProvider).Name);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063")]
		public void Dispose()
		{
			if(_isDisposed)
				return;
			_isDisposed = true;
		}
	}
}
