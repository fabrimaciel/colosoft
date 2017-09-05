using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Colosoft.Query.Database.Oracle{
	public class OracleTakeParametersParser : ITakeParametersParser	{
		public string GetText (SqlQueryParser a, Colosoft.Query.TakeParameters b)		{
			return new GDA.Provider.Oracle.OracleProvider ().SQLCommandLimit (null, a.Text, b.Skip, b.Take);
		}
	}
}
