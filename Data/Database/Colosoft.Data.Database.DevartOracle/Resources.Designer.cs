namespace Colosoft.Data.Database.Oracle.Properties{
	using System;
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute ("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute ()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute ()]
	internal class Resources	{
		private static global::System.Resources.ResourceManager resourceMan;
		private static global::System.Globalization.CultureInfo resourceCulture;
		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Resources ()		{
		}
		[global::System.ComponentModel.EditorBrowsableAttribute (global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager {
			get {
				if (object.ReferenceEquals (resourceMan, null)) {
					global::System.Resources.ResourceManager a = new global::System.Resources.ResourceManager ("Colosoft.Data.Database.Oracle.Properties.Resources", typeof(Resources).Assembly);
					resourceMan = a;
				}
				return resourceMan;
			}
		}
		[global::System.ComponentModel.EditorBrowsableAttribute (global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Globalization.CultureInfo Culture {
			get {
				return resourceCulture;
			}
			set {
				resourceCulture = value;
			}
		}
		internal static string ConditionalParserException_DuplicateParameter {
			get {
				return ResourceManager.GetString ("ConditionalParserException_DuplicateParameter", resourceCulture);
			}
		}
		internal static string Exception_ExecuteDatabaseCommand {
			get {
				return ResourceManager.GetString ("Exception_ExecuteDatabaseCommand", resourceCulture);
			}
		}
		internal static string Exception_RowsNotAffected {
			get {
				return ResourceManager.GetString ("Exception_RowsNotAffected", resourceCulture);
			}
		}
	}
}
