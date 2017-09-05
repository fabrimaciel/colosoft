using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Colosoft.Query.Database.Oracle{
	public static class OracleTimeStampTZExtensions	{
		#if UNMANAGED
        public static DateTimeOffset ToDateTimeOffset(this global::Oracle.DataAccess.Types.OracleTimeStampTZ ts)        {
            try
            {
                return new DateTimeOffset(ts.Value, ts.GetTimeZoneOffset());
            }
            catch (Exception)            {
                throw;
            }
        }
#elif DEVART
        public static DateTimeOffset ToDateTimeOffset(this Devart.Data.Oracle.OracleTimeStamp ts)        {
            try
            {
                return new DateTimeOffset(ts.Value, ts.TimeZoneOffset);
            }
            catch (Exception)            {
                throw;
            }
        }
#else
		public static DateTimeOffset ToDateTimeOffset (this global::Oracle.ManagedDataAccess.Types.OracleTimeStampTZ a)		{
			try {
				return new DateTimeOffset (a.Value, a.GetTimeZoneOffset ());
			}
			catch (Exception) {
				throw;
			}
		}
	#endif
	}
}
