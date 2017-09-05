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
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Representa o resultado de uma operação no formato json.
	/// </summary>
	public class JsonOperationResult : System.Web.Mvc.ActionResult
	{
		private System.Web.Mvc.JsonResult _innerResult;

		/// <summary>
		/// Encoding do conteúdo.
		/// </summary>
		public Encoding ContentEncoding
		{
			get
			{
				return _innerResult.ContentEncoding;
			}
			set
			{
				_innerResult.ContentEncoding = value;
			}
		}

		/// <summary>
		/// Tipo do conteúdo.
		/// </summary>
		public string ContentType
		{
			get
			{
				return _innerResult.ContentType;
			}
			set
			{
				_innerResult.ContentType = value;
			}
		}

		/// <summary>
		/// Dados do resultado.
		/// </summary>
		public object Data
		{
			get
			{
				return _innerResult.Data;
			}
			set
			{
				_innerResult.Data = value;
			}
		}

		/// <summary>
		/// Comportamento da requisição JSON.
		/// </summary>
		public System.Web.Mvc.JsonRequestBehavior JsonRequestBehavior
		{
			get
			{
				return _innerResult.JsonRequestBehavior;
			}
			set
			{
				_innerResult.JsonRequestBehavior = value;
			}
		}

		/// <summary>
		/// Tamanho máximo do JSON.
		/// </summary>
		public int? MaxJsonLength
		{
			get
			{
				return _innerResult.MaxJsonLength;
			}
			set
			{
				_innerResult.MaxJsonLength = value;
			}
		}

		/// <summary>
		/// Limite de recursão.
		/// </summary>
		public int? RecursionLimit
		{
			get
			{
				return _innerResult.RecursionLimit;
			}
			set
			{
				_innerResult.RecursionLimit = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public JsonOperationResult()
		{
			_innerResult = new System.Web.Mvc.JsonResult();
		}

		/// <summary>
		/// Cria a instancia com base no resultado informado.
		/// </summary>
		/// <param name="jsonResult"></param>
		public JsonOperationResult(System.Web.Mvc.JsonResult jsonResult)
		{
			jsonResult.Require("jsonResult").NotNull();
			_innerResult = jsonResult;
		}

		/// <summary>
		/// Executa o resultado.
		/// </summary>
		/// <param name="context"></param>
		public override void ExecuteResult(System.Web.Mvc.ControllerContext context)
		{
			_innerResult.ExecuteResult(context);
		}

		/// <summary>
		/// Converte de forma implicita o resultado da operação.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public static implicit operator JsonOperationResult(Colosoft.Business.OperationResult result)
		{
			return new JsonOperationResult {
				Data = new {
					Success = result.Success,
					Message = result.Message.FormatOrNull()
				}
			};
		}

		/// <summary>
		/// Converte de forma implicita o resultado da operação de salvar.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public static implicit operator JsonOperationResult(Colosoft.Business.SaveResult result)
		{
			return new JsonOperationResult {
				Data = new {
					Success = result.Success,
					Message = result.Message.FormatOrNull()
				}
			};
		}

		/// <summary>
		/// Converte de forma implicita o resultado da operação de exclusão.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public static implicit operator JsonOperationResult(Colosoft.Business.DeleteResult result)
		{
			return new JsonOperationResult {
				Data = new {
					Success = result.Success,
					Message = result.Message.FormatOrNull()
				}
			};
		}

		/// <summary>
		/// Converte o resultado Json de forma implicita.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public static implicit operator JsonOperationResult(System.Web.Mvc.JsonResult result)
		{
			if(result == null)
				return null;
			var result2 = new JsonOperationResult(result);
			result2.Data = new {
				Success = true,
				Message = (string)null,
				Result = result.Data
			};
			return result2;
		}
	}
}
