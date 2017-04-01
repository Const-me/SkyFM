using SkyFM;
namespace XamlExtrensions
{
	// http://msdn.microsoft.com/en-us/library/ff637520(VS.92).aspx
	public class LocalizedStrings
	{
		public LocalizedStrings() { }
		static STR _str = new STR();
		public STR str { get { return _str; } }
	}
}