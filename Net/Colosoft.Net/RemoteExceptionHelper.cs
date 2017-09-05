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

namespace Colosoft.Net.Diagnostics
{
	/// <summary>
	/// Classe com método para auxiliar nos erros remotos.
	/// </summary>
	public class RemoteExceptionHelper
	{
		/// <summary>
		/// Nome do cabeçalho que identifica um erro.
		/// </summary>
		public const string ExceptionHeader = "X-Exception";

		/// <summary>
		/// Verifica se possui identificação de erro no cabeçalho.
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static bool ContainsError(System.Collections.Specialized.NameValueCollection headers)
		{
			return headers.AllKeys.Contains(ExceptionHeader);
		}

		/// <summary>
		/// Serializa os dados do erro ocorrido.
		/// </summary>
		/// <param name="exception">Erro que será serializado.</param>
		/// <param name="outputStream">Stream com os dados serializados.</param>
		public static void Serialize(Exception exception, System.IO.Stream outputStream)
		{
			if(exception == null)
				return;
			var error = new Error(exception);
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Error));
			serializer.Serialize(outputStream, error);
		}

		/// <summary>
		/// Deserializa o erro.
		/// </summary>
		/// <param name="inputStream"></param>
		/// <returns></returns>
		public static Exception Deserialize(System.IO.Stream inputStream)
		{
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Error));
			var error = (Error)serializer.Deserialize(inputStream);
			return error.GetException();
		}

		/// <summary>
		/// Classe para serializar os dados do eror.
		/// </summary>
		public class Error
		{
			/// <summary>
			/// Mensagem do erro.
			/// </summary>
			public string Message
			{
				get;
				set;
			}

			/// <summary>
			/// Tipo do erro.
			/// </summary>
			public string Type
			{
				get;
				set;
			}

			/// <summary>
			/// Pilha de chamada do erro.
			/// </summary>
			public string StackTrace
			{
				get;
				set;
			}

			/// <summary>
			/// Error interno.
			/// </summary>
			public Error Inner
			{
				get;
				set;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			public Error()
			{
			}

			/// <summary>
			/// Cria uma instancia a partir do erro informado.
			/// </summary>
			/// <param name="exception"></param>
			public Error(Exception exception)
			{
				Message = exception.Message;
				Type = string.Format("{0}, {1}", exception.GetType().FullName, exception.GetType().Assembly.FullName);
				StackTrace = exception.StackTrace;
				if(exception.InnerException != null)
					Inner = new Error(exception.InnerException);
			}

			/// <summary>
			/// Cria a exception associada com o erro.
			/// </summary>
			/// <returns></returns>
			public Exception GetException()
			{
				Exception result = null;
				var type = System.Type.GetType(Type, false);
				if(type != null)
				{
					foreach (var i in type.GetConstructors())
					{
						var parameters = i.GetParameters();
						if(Inner != null && parameters.Length == 2 && parameters[0].GetType() == typeof(string) && typeof(Exception).IsAssignableFrom(parameters[1].GetType()))
						{
							try
							{
								result = (Exception)i.Invoke(new object[] {
									Message,
									Inner.GetException()
								});
							}
							catch
							{
							}
							break;
						}
						else if(Inner == null && parameters.Length == 1 && parameters[0].GetType() == typeof(string))
						{
							try
							{
								result = (Exception)i.Invoke(new object[] {
									Message
								});
							}
							catch
							{
							}
							break;
						}
					}
				}
				if(result == null && Inner != null)
					result = new Exception(Message, Inner.GetException());
				else if(result == null)
					result = new Exception(Message);
				var stackTraceField = typeof(Exception).GetField("_stackTraceString", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				if(stackTraceField != null)
					stackTraceField.SetValue(result, StackTrace);
				return result;
			}
		}
	}
}
