using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Colosoft.Query.Database.Oracle{
	public class OracleServerData : IServerData	{
		private TimeSpan _offset = TimeSpan.Zero;
		private DateTime _lastUpdate;
		private bool _isValid = false;
		private bool IsValid {
			get {
				if (!_isValid || _lastUpdate.AddMinutes (3) < DateTime.Now)
					return false;
				return true;
			}
		}
		public DateTimeOffset Current {
			get {
				if (!IsValid) {
					try {
						using (var a = new GDA.GDASession ()) {
							var b = new GDA.DataAccess ().ExecuteScalar (a, "SELECT SYSDATE FROM dual");
							if (b is DateTime) {
								var c = (DateTime)b;
								var d = c - DateTime.Now;
								_offset = d;
								_lastUpdate = DateTime.Now;
							}
						}
						_isValid = true;
					}
					catch {
					}
				}
				return DateTime.Now.Add (_offset);
			}
		}
		public DateTimeOffset GateDateTimeOffSet ()		{
			return Current;
		}
		public DateTime GetDateTime ()		{
			return Current.DateTime;
		}
	}
}
