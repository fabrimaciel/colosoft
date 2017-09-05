using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Colosoft.Excel{
	public partial class FORMULA : Record	{
		public FORMULA (Record a) : base (a)		{
		}
		public UInt16 RowIndex;
		public UInt16 ColIndex;
		public UInt16 XFIndex;
		public UInt64 Result;
		public UInt16 OptionFlags;
		public UInt32 Unused;
		public Byte[] FormulaData;
		public override void Decode ()		{
			MemoryStream a = new MemoryStream (Data);
			BinaryReader b = new BinaryReader (a);
			this.RowIndex = b.ReadUInt16 ();
			this.ColIndex = b.ReadUInt16 ();
			this.XFIndex = b.ReadUInt16 ();
			this.Result = b.ReadUInt64 ();
			this.OptionFlags = b.ReadUInt16 ();
			this.Unused = b.ReadUInt32 ();
			this.FormulaData = b.ReadBytes ((int)(a.Length - a.Position));
		}
	}
}
