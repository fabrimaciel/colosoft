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
using System.IO;
using System.Globalization;

namespace Colosoft.Logging
{
	/// <summary>
	/// Logger de arquivos.
	/// </summary>
	public class FileLogger : LoggerBase
	{
		/// <summary>
		/// Construtor Vazio.
		/// </summary>
		public FileLogger()
		{
			string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string colosoftDirectory = Path.Combine(appDataDirectory, "Colosoft");
			if(!Directory.Exists(colosoftDirectory))
				Directory.CreateDirectory(colosoftDirectory);
			FilePath = Path.Combine(colosoftDirectory, "PerformanceLogs\\logsService.txt");
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="path">Caminho do arquivo.</param>
		public FileLogger(string path)
		{
			FilePath = path;
		}

		/// <summary>
		/// Caminho do arquivo de log.
		/// </summary>
		protected string FilePath
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera as entradas do arquivo informado.
		/// </summary>
		/// <param name="path">Caminho do arquivo.</param>
		/// <returns></returns>
		public static IEnumerable<Entry> GetEntries(string path)
		{
			path.Require("path").NotNull().NotEmpty();
			if(System.IO.File.Exists(path))
				return GetEntries(System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));
			return new Entry[0];
		}

		/// <summary>
		/// Carrega as entradas contidas na stream informada.
		/// </summary>
		/// <param name="inputStream"></param>
		/// <returns></returns>
		public static IEnumerable<Entry> GetEntries(System.IO.Stream inputStream)
		{
			inputStream.Require("inputStream").NotNull();
			var reader = new System.IO.StreamReader(inputStream);
			string lastLine = "";
			var content = "";
			Entry entry = null;
			var regex = new System.Text.RegularExpressions.Regex(@"(?<category>EXCEPTION|DEBUG|INFO|WARN)\-(?<month>([0-9]\d*))\/(?<day>([0-9]\d*))\/(?<year>([0-9]\d*)) (?<hour>([0-9]\d*)):(?<minute>([0-9]\d*)):(?<second>([0-9]\d*)).(?<secondPart>([0-9]\d*)): ");
			while (!reader.EndOfStream)
			{
				lastLine = reader.ReadLine();
				var match = regex.Match(lastLine);
				if(match.Success)
				{
					if(entry != null)
					{
						entry.Message = content;
						content = "";
						yield return entry;
					}
					Category category = Category.Info;
					if(!Enum.TryParse<Category>(match.Groups["category"].Value, true, out category))
						category = Category.Info;
					entry = new Entry(category, new DateTime(int.Parse(match.Groups["year"].Value) + 2000, int.Parse(match.Groups["month"].Value), int.Parse(match.Groups["day"].Value), int.Parse(match.Groups["hour"].Value), int.Parse(match.Groups["minute"].Value), int.Parse(match.Groups["second"].Value)), null, Priority.None);
					content += lastLine.Substring(match.Index + match.Length);
				}
				else if(entry != null)
				{
					content += lastLine;
				}
			}
			if(entry != null)
			{
				entry.Message = content;
				yield return entry;
			}
		}

		/// <summary>
		/// Escreve uma nova entrada de log com uma categoria e prioridade especificada.
		/// </summary>
		/// <param name="message">Mensagem do corpo do log.</param>
		/// <param name="category">Categoria da entrada.</param>
		/// <param name="priority">Prioridade da entrada.</param>
		/// <returns>True se o log foi salvo com sucesso.</returns>
		public override bool Write(IMessageFormattable message, Category category, Priority priority)
		{
			string messageToLog = String.Format(CultureInfo.InvariantCulture, Colosoft.Properties.Resources.FileLoggerPattern, DateTime.Now.ToString("MM/dd/yy H:mm:ss.fffffff"), category.ToString().ToUpper(CultureInfo.InvariantCulture), message.Format(CultureInfo.CurrentCulture), priority.ToString());
			try
			{
				if(File.Exists(FilePath))
				{
					using (var writer = File.AppendText(FilePath))
					{
						writer.WriteLine(messageToLog);
					}
				}
				else
				{
					using (var writer = File.CreateText(FilePath))
					{
						writer.WriteLine(messageToLog);
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Escreve uma nava entrada de log do tipo de Exception.
		/// </summary>
		/// <param name="message">Mensagem do corpo do log.</param>
		/// <param name="exception">Instancia da exception ocorrida.</param>
		/// <param name="priority">Prioridade do log.</param>
		/// <returns>True se o log foi salvo com sucesso.</returns>
		public override bool Write(IMessageFormattable message, Exception exception, Priority priority)
		{
			if(exception != null)
			{
				var exceptionMessage = exception.Message;
				var stackTrace = exception.StackTrace;
				var exceptionText = new StringBuilder();
				while (exception != null)
				{
					exceptionText.AppendFormat("{0} : {1}\r\n", exception.GetType().Name, exception.Message);
					exception = exception.InnerException;
				}
				message = string.Format("Message: {0}\r\nException: {1} : {2}", message != null ? message.Format(CultureInfo.InvariantCulture) : exceptionMessage, exceptionText.ToString(), stackTrace).GetFormatter();
			}
			return Write(message, Category.Exception, priority);
		}

		/// <summary>
		/// Representa a entrada do arquivo de log.
		/// </summary>
		public class Entry
		{
			/// <summary>
			/// Data de criação.
			/// </summary>
			public DateTime Created
			{
				get;
				set;
			}

			/// <summary>
			/// Mensagem.
			/// </summary>
			public string Message
			{
				get;
				set;
			}

			/// <summary>
			/// Categoria.
			/// </summary>
			public Category Category
			{
				get;
				set;
			}

			/// <summary>
			/// Prioridade.
			/// </summary>
			public Priority Priority
			{
				get;
				set;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="category"></param>
			/// <param name="created"></param>
			/// <param name="message"></param>
			/// <param name="priority"></param>
			public Entry(Category category, DateTime created, string message, Priority priority)
			{
				Category = category;
				Created = created;
				Message = message;
				Priority = priority;
			}

			/// <summary>
			/// Recupera o texto que representa a instancia.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, Colosoft.Properties.Resources.FileLoggerPattern, DateTime.Now.ToString("MM/dd/yy H:mm:ss.fffffff"), Category.ToString().ToUpper(CultureInfo.InvariantCulture), Message != null ? Message : string.Empty, Priority.ToString());
			}
		}
	}
}
